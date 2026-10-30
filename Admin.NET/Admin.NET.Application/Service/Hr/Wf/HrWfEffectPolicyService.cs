using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>业务生效策略与字段对照维护</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfEffectPolicy", Order = 486, Description = "业务生效策略")]
public class HrWfEffectPolicyService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfEffectPolicy> _policyRep;
    private readonly SqlSugarRepository<HrWfEffectFieldMap> _mapRep;
    private readonly HrWfEffectApplyService _apply;

    public HrWfEffectPolicyService(
        SqlSugarRepository<HrWfEffectPolicy> policyRep,
        SqlSugarRepository<HrWfEffectFieldMap> mapRep,
        HrWfEffectApplyService apply)
    {
        _policyRep = policyRep;
        _mapRep = mapRep;
        _apply = apply;
    }

    [ApiDescriptionSettings(Name = "ListPolicies"), HttpGet]
    [DisplayName("生效策略列表")]
    public async Task<List<HrWfEffectPolicyOutput>> ListPolicies()
    {
        return await _policyRep.AsQueryable()
            .OrderBy(u => u.BizType)
            .Select<HrWfEffectPolicyOutput>()
            .ToListAsync();
    }

    [ApiDescriptionSettings(Name = "GetByBizType"), HttpGet]
    [DisplayName("按业务类型取生效策略")]
    public async Task<HrWfEffectPolicyOutput?> GetByBizType([FromQuery] string bizType)
    {
        var row = await _apply.GetEnabledPolicyAsync(bizType);
        return row?.Adapt<HrWfEffectPolicyOutput>();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SavePolicy"), HttpPost]
    [DisplayName("保存生效策略")]
    public async Task<long> SavePolicy(HrWfEffectPolicySaveInput input)
    {
        var biz = input.BizType.Trim();
        if (input.Id is > 0)
        {
            var entity = await _policyRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("策略不存在");
            if (await _policyRep.IsAnyAsync(u => u.BizType == biz && u.Enabled && input.Enabled && u.Id != entity.Id))
                throw Oops.Oh("同业务类型已有启用中的策略");
            ApplyPolicy(entity, input, biz);
            await _policyRep.UpdateAsync(entity);
            return entity.Id;
        }

        if (input.Enabled && await _policyRep.IsAnyAsync(u => u.BizType == biz && u.Enabled))
            throw Oops.Oh("同业务类型已有启用中的策略");

        var inserted = await _policyRep.InsertReturnEntityAsync(ApplyPolicy(new HrWfEffectPolicy
        {
            Id = Yitter.IdGenerator.YitIdHelper.NextId()
        }, input, biz));
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeletePolicy"), HttpPost]
    [DisplayName("删除生效策略")]
    public async Task DeletePolicy([FromQuery] long id)
    {
        var entity = await _policyRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("策略不存在");
        await _policyRep.DeleteAsync(entity);
    }

    [ApiDescriptionSettings(Name = "ListFieldMaps"), HttpGet]
    [DisplayName("按业务类型取字段对照")]
    public async Task<List<HrWfEffectFieldMapOutput>> ListFieldMaps([FromQuery] string? bizType)
    {
        var q = _mapRep.AsQueryable();
        if (!string.IsNullOrWhiteSpace(bizType))
            q = q.Where(u => u.BizType == bizType.Trim());
        return await q.OrderBy(u => u.BizType).OrderBy(u => u.SortNo)
            .Select<HrWfEffectFieldMapOutput>()
            .ToListAsync();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveFieldMap"), HttpPost]
    [DisplayName("保存字段对照")]
    public async Task<long> SaveFieldMap(HrWfEffectFieldMapSaveInput input)
    {
        var biz = input.BizType.Trim();
        var key = input.FormFieldKey.Trim();
        if (input.Id is > 0)
        {
            var entity = await _mapRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("对照不存在");
            if (await _mapRep.IsAnyAsync(u => u.BizType == biz && u.FormFieldKey == key && u.Id != entity.Id))
                throw Oops.Oh("同业务类型下表单字段键已存在");
            ApplyMap(entity, input, biz, key);
            await _mapRep.UpdateAsync(entity);
            return entity.Id;
        }

        if (await _mapRep.IsAnyAsync(u => u.BizType == biz && u.FormFieldKey == key))
            throw Oops.Oh("同业务类型下表单字段键已存在");

        var inserted = await _mapRep.InsertReturnEntityAsync(ApplyMap(new HrWfEffectFieldMap
        {
            Id = Yitter.IdGenerator.YitIdHelper.NextId()
        }, input, biz, key));
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteFieldMap"), HttpPost]
    [DisplayName("删除字段对照")]
    public async Task DeleteFieldMap([FromQuery] long id)
    {
        var entity = await _mapRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("对照不存在");
        await _mapRep.DeleteAsync(entity);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "ApplyDue"), HttpPost]
    [DisplayName("手动扫描并生效到期单据")]
    public Task<int> ApplyDue() => _apply.ApplyDuePendingsAsync();

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Void"), HttpPost]
    [DisplayName("撤销生效/待生效")]
    public Task<HrWfEffectApplyResult> Void(HrWfEffectVoidInput input) => _apply.VoidAsync(input);

    private static HrWfEffectPolicy ApplyPolicy(HrWfEffectPolicy e, HrWfEffectPolicySaveInput input, string biz)
    {
        e.BizType = biz;
        e.Enabled = input.Enabled;
        e.BaseDateSource = string.IsNullOrWhiteSpace(input.BaseDateSource) ? HrWfConsts.EffectBaseFromForm : input.BaseDateSource.Trim();
        e.BaseDateField = input.BaseDateField.Trim();
        e.DayOffset = input.DayOffset;
        e.ApplyMode = string.IsNullOrWhiteSpace(input.ApplyMode) ? HrWfConsts.EffectModeImmediateIfDue : input.ApplyMode.Trim();
        e.TargetEntity = string.IsNullOrWhiteSpace(input.TargetEntity) ? "HrEmployee" : input.TargetEntity.Trim();
        e.Remark = input.Remark?.Trim();
        return e;
    }

    private static HrWfEffectFieldMap ApplyMap(HrWfEffectFieldMap e, HrWfEffectFieldMapSaveInput input, string biz, string key)
    {
        e.BizType = biz;
        e.FormFieldKey = key;
        e.Role = input.Role.Trim();
        e.TargetEntity = string.IsNullOrWhiteSpace(input.TargetEntity) ? "HrEmployee" : input.TargetEntity.Trim();
        e.DbColumn = string.IsNullOrWhiteSpace(input.DbColumn) ? null : input.DbColumn.Trim();
        e.PairGroup = string.IsNullOrWhiteSpace(input.PairGroup) ? null : input.PairGroup.Trim();
        e.Enabled = input.Enabled;
        e.SortNo = input.SortNo;
        e.Remark = input.Remark?.Trim();
        return e;
    }
}
