using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审批流程定义
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfDef", Order = 483, Description = "审批流程定义")]
public class HrWfDefService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfDef> _rep;
    private readonly SqlSugarRepository<HrWfDefNode> _nodeRep;
    private readonly SqlSugarRepository<HrWfForm> _formRep;

    public HrWfDefService(
        SqlSugarRepository<HrWfDef> rep,
        SqlSugarRepository<HrWfDefNode> nodeRep,
        SqlSugarRepository<HrWfForm> formRep)
    {
        _rep = rep;
        _nodeRep = nodeRep;
        _formRep = formRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("流程定义分页")]
    public async Task<SqlSugarPagedList<HrWfDefOutput>> Page(HrWfDefPageInput input)
    {
        input ??= new HrWfDefPageInput();
        var keyword = input.Keyword?.Trim();
        var query = _rep.AsQueryable()
            .LeftJoin<HrWfForm>((d, f) => d.FormId == f.Id)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), (d, f) => d.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), (d, f) => d.Code.Contains(keyword!) || d.Name.Contains(keyword!))
            .WhereIF(input.Enabled.HasValue, (d, f) => d.Enabled == input.Enabled)
            .Select((d, f) => new HrWfDefOutput
            {
                Id = d.Id,
                BizType = d.BizType,
                Code = d.Code,
                Name = d.Name,
                FormId = d.FormId,
                FormName = f.Name,
                Version = d.Version,
                Enabled = d.Enabled,
                AllowEditWhileRunning = d.AllowEditWhileRunning,
                EditByApplicant = d.EditByApplicant,
                EditByAssignee = d.EditByAssignee,
                DefaultTimeoutHours = d.DefaultTimeoutHours,
                Remark = d.Remark
            });

        NormalizeSort(input);
        return await query
            .MergeTable()
            .OrderBuilder(input, defaultSortField: nameof(HrWfDefOutput.Id), descSort: true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("流程定义详情")]
    public async Task<HrWfDefOutput> Get([FromQuery] long id)
    {
        var def = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        return await ToOutputAsync(def);
    }

    [ApiDescriptionSettings(Name = "GetEnabledByBizType"), HttpGet]
    [DisplayName("按业务类型取启用流程")]
    public async Task<HrWfDefOutput> GetEnabledByBizType([FromQuery] string bizType)
    {
        if (string.IsNullOrWhiteSpace(bizType)) throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        var def = await _rep.GetFirstAsync(u => u.BizType == bizType && u.Enabled)
            ?? throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        return await ToOutputAsync(def);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存流程定义")]
    public async Task<long> Save(HrWfDefSaveInput input)
    {
        if (!await _formRep.IsAnyAsync(u => u.Id == input.FormId))
            throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);

        var nodes = input.Nodes ?? [];
        if (nodes.Count == 0) throw Oops.Oh(HrWfErrorCodeEnum.DefNoNodes);
        if (nodes.Select(u => u.NodeKey?.Trim()).Where(u => !string.IsNullOrWhiteSpace(u))
                .GroupBy(u => u!, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
            throw Oops.Oh(HrWfErrorCodeEnum.DefNodeKeyDup);

        var bizType = input.BizType.Trim();
        long defId;

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
            if (input.Enabled)
                await DisableOtherEnabledAsync(bizType, entity.Id);

            ApplyHead(entity, input, bizType);
            entity.Version += 1;
            await _rep.UpdateAsync(entity);
            defId = entity.Id;
        }
        else
        {
            if (input.Enabled)
                await DisableOtherEnabledAsync(bizType, null);

            var entity = new HrWfDef();
            ApplyHead(entity, input, bizType);
            entity.Version = 1;
            var inserted = await _rep.InsertReturnEntityAsync(entity);
            defId = inserted.Id;
        }

        await ReplaceNodesAsync(defId, nodes);
        return defId;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除流程定义")]
    public async Task Delete(HrWfDefDeleteInput input)
    {
        if (!await _rep.IsAnyAsync(u => u.Id == input.Id))
            throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        await _nodeRep.AsDeleteable().Where(u => u.DefId == input.Id).ExecuteCommandAsync();
        await _rep.FakeDeleteAsync(input.Id);
    }

    private static void ApplyHead(HrWfDef entity, HrWfDefSaveInput input, string bizType)
    {
        entity.BizType = bizType;
        entity.Code = input.Code.Trim();
        entity.Name = input.Name.Trim();
        entity.FormId = input.FormId;
        entity.Enabled = input.Enabled;
        entity.AllowEditWhileRunning = input.AllowEditWhileRunning;
        entity.EditByApplicant = input.EditByApplicant;
        entity.EditByAssignee = input.EditByAssignee;
        entity.DefaultTimeoutHours = input.DefaultTimeoutHours;
        entity.Remark = input.Remark?.Trim();
    }

    private async Task DisableOtherEnabledAsync(string bizType, long? exceptId)
    {
        await _rep.AsUpdateable()
            .SetColumns(u => u.Enabled == false)
            .Where(u => u.BizType == bizType && u.Enabled)
            .WhereIF(exceptId.HasValue, u => u.Id != exceptId)
            .ExecuteCommandAsync();
    }

    private async Task ReplaceNodesAsync(long defId, List<HrWfDefNodeItem> nodes)
    {
        await _nodeRep.AsDeleteable().Where(u => u.DefId == defId).ExecuteCommandAsync();
        var rows = nodes
            .OrderBy(u => u.OrderNo)
            .Select((n, i) => new HrWfDefNode
            {
                DefId = defId,
                NodeKey = n.NodeKey.Trim(),
                Name = n.Name.Trim(),
                OrderNo = n.OrderNo != 0 ? n.OrderNo : (i + 1) * 10,
                NodeMode = string.IsNullOrWhiteSpace(n.NodeMode) ? HrWfConsts.NodeOr : n.NodeMode.Trim(),
                AssigneeRuleJson = n.AssigneeRuleJson,
                ConditionJson = n.ConditionJson,
                TimeoutHours = n.TimeoutHours,
                Enabled = n.Enabled
            })
            .ToList();
        await _nodeRep.InsertRangeAsync(rows);
    }

    private async Task<HrWfDefOutput> ToOutputAsync(HrWfDef def)
    {
        var form = await _formRep.GetFirstAsync(u => u.Id == def.FormId);
        var nodes = await _nodeRep.AsQueryable()
            .Where(u => u.DefId == def.Id)
            .OrderBy(u => u.OrderNo)
            .Select(u => new HrWfDefNodeItem
            {
                Id = u.Id,
                NodeKey = u.NodeKey,
                Name = u.Name,
                OrderNo = u.OrderNo,
                NodeMode = u.NodeMode,
                AssigneeRuleJson = u.AssigneeRuleJson,
                ConditionJson = u.ConditionJson,
                TimeoutHours = u.TimeoutHours,
                Enabled = u.Enabled
            })
            .ToListAsync();

        return new HrWfDefOutput
        {
            Id = def.Id,
            BizType = def.BizType,
            Code = def.Code,
            Name = def.Name,
            FormId = def.FormId,
            FormName = form?.Name,
            Version = def.Version,
            Enabled = def.Enabled,
            AllowEditWhileRunning = def.AllowEditWhileRunning,
            EditByApplicant = def.EditByApplicant,
            EditByAssignee = def.EditByAssignee,
            DefaultTimeoutHours = def.DefaultTimeoutHours,
            Remark = def.Remark,
            Nodes = nodes
        };
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
