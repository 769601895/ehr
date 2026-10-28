using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审核等级
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfLevel", Order = 480, Description = "审核等级")]
public class HrWfLevelService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfLevel> _rep;
    private readonly SqlSugarRepository<HrWfRole> _roleRep;
    private readonly HrWfAssigneeResolver _resolver;

    public HrWfLevelService(
        SqlSugarRepository<HrWfLevel> rep,
        SqlSugarRepository<HrWfRole> roleRep,
        HrWfAssigneeResolver resolver)
    {
        _rep = rep;
        _roleRep = roleRep;
        _resolver = resolver;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("审核等级分页")]
    public async Task<SqlSugarPagedList<HrWfLevelOutput>> Page(HrWfLevelPageInput input)
    {
        input ??= new HrWfLevelPageInput();
        var keyword = input.Keyword?.Trim();
        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.Code.Contains(keyword!) || u.Name.Contains(keyword!))
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled);

        NormalizeSort(input);
        return await query
            .OrderBuilder(input, defaultSortField: nameof(HrWfLevel.AuditOrder), descSort: false)
            .Select<HrWfLevelOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("审核等级详情")]
    public async Task<HrWfLevelOutput> Get([FromQuery] long id)
    {
        var row = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh(HrWfErrorCodeEnum.LevelNotFound);
        return row.Adapt<HrWfLevelOutput>();
    }

    [ApiDescriptionSettings(Name = "ListEnabled"), HttpGet]
    [DisplayName("启用审核等级列表")]
    public async Task<List<HrWfLevelOutput>> ListEnabled()
    {
        return await _rep.AsQueryable()
            .Where(u => u.Enabled)
            .OrderBy(u => u.AuditOrder)
            .OrderBy(u => u.OrderNo)
            .Select<HrWfLevelOutput>()
            .ToListAsync();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存审核等级")]
    public async Task<long> Save(HrWfLevelSaveInput input)
    {
        var code = input.Code.Trim();
        var name = input.Name.Trim();

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(HrWfErrorCodeEnum.LevelNotFound);
            var dup = await _rep.IsAnyAsync(u => u.Code == code && u.Id != entity.Id);
            if (dup) throw Oops.Oh(HrWfErrorCodeEnum.LevelCodeDup);

            entity.Code = code;
            entity.Name = name;
            entity.AuditOrder = input.AuditOrder;
            entity.IsFunctional = input.IsFunctional;
            entity.OrderNo = input.OrderNo;
            entity.Enabled = input.Enabled;
            entity.Remark = input.Remark?.Trim();
            await _rep.UpdateAsync(entity);
            return entity.Id;
        }

        if (await _rep.IsAnyAsync(u => u.Code == code))
            throw Oops.Oh(HrWfErrorCodeEnum.LevelCodeDup);

        var inserted = await _rep.InsertReturnEntityAsync(new HrWfLevel
        {
            Code = code,
            Name = name,
            AuditOrder = input.AuditOrder,
            IsFunctional = input.IsFunctional,
            OrderNo = input.OrderNo,
            Enabled = input.Enabled,
            Remark = input.Remark?.Trim()
        });
        return inserted.Id;
    }

    /// <summary>预览 Level 找人（验收用）</summary>
    [ApiDescriptionSettings(Name = "PreviewAssignees"), HttpGet]
    [DisplayName("预览等级审批人")]
    public async Task<List<long>> PreviewAssignees([FromQuery] long levelId, [FromQuery] string subjectCcCode, [FromQuery] bool escalateIfEmpty = true)
    {
        return await _resolver.ResolveByLevelAsync(levelId, subjectCcCode, escalateIfEmpty);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除审核等级")]
    public async Task Delete(HrWfLevelDeleteInput input)
    {
        var exists = await _rep.IsAnyAsync(u => u.Id == input.Id);
        if (!exists) throw Oops.Oh(HrWfErrorCodeEnum.LevelNotFound);
        if (await _roleRep.IsAnyAsync(u => u.LevelId == input.Id))
            throw Oops.Oh(HrWfErrorCodeEnum.LevelHasRoles);
        await _rep.FakeDeleteAsync(input.Id);
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
