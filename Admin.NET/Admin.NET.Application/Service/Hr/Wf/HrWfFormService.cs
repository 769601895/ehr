using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审批表单方案（默认模式 B）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfForm", Order = 482, Description = "审批表单方案")]
public class HrWfFormService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfForm> _rep;
    private readonly SqlSugarRepository<HrWfFormField> _fieldRep;
    private readonly SqlSugarRepository<HrWfDef> _defRep;

    public HrWfFormService(
        SqlSugarRepository<HrWfForm> rep,
        SqlSugarRepository<HrWfFormField> fieldRep,
        SqlSugarRepository<HrWfDef> defRep)
    {
        _rep = rep;
        _fieldRep = fieldRep;
        _defRep = defRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("审批表单分页")]
    public async Task<SqlSugarPagedList<HrWfFormOutput>> Page(HrWfFormPageInput input)
    {
        input ??= new HrWfFormPageInput();
        var keyword = input.Keyword?.Trim();
        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), u => u.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.Code.Contains(keyword!) || u.Name.Contains(keyword!))
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled);

        NormalizeSort(input);
        return await query
            .OrderBuilder(input, defaultSortField: nameof(HrWfForm.Id), descSort: true)
            .Select(u => new HrWfFormOutput
            {
                Id = u.Id,
                BizType = u.BizType,
                Code = u.Code,
                Name = u.Name,
                FormMode = u.FormMode,
                Version = u.Version,
                Enabled = u.Enabled,
                Remark = u.Remark
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("审批表单详情")]
    public async Task<HrWfFormOutput> Get([FromQuery] long id)
    {
        var form = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
        return await ToOutputAsync(form);
    }

    [ApiDescriptionSettings(Name = "GetByBizType"), HttpGet]
    [DisplayName("按业务类型取启用表单")]
    public async Task<HrWfFormOutput> GetByBizType([FromQuery] string bizType)
    {
        if (string.IsNullOrWhiteSpace(bizType)) throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
        var form = await _rep.GetFirstAsync(u => u.BizType == bizType && u.Enabled)
            ?? throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
        return await ToOutputAsync(form);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存审批表单")]
    public async Task<long> Save(HrWfFormSaveInput input)
    {
        var bizType = input.BizType.Trim();
        var code = input.Code.Trim();
        var name = input.Name.Trim();
        var mode = string.IsNullOrWhiteSpace(input.FormMode) ? HrWfConsts.FormModeB : input.FormMode.Trim();

        ValidateFields(input.Fields);

        long formId;
        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
            if (input.Enabled)
                await DisableOtherEnabledAsync(bizType, entity.Id);

            entity.BizType = bizType;
            entity.Code = code;
            entity.Name = name;
            entity.FormMode = mode;
            entity.Enabled = input.Enabled;
            entity.Remark = input.Remark?.Trim();
            entity.Version += 1;
            await _rep.UpdateAsync(entity);
            formId = entity.Id;
        }
        else
        {
            if (input.Enabled)
                await DisableOtherEnabledAsync(bizType, null);

            var inserted = await _rep.InsertReturnEntityAsync(new HrWfForm
            {
                BizType = bizType,
                Code = code,
                Name = name,
                FormMode = mode,
                Enabled = input.Enabled,
                Remark = input.Remark?.Trim(),
                Version = 1
            });
            formId = inserted.Id;
        }

        await ReplaceFieldsAsync(formId, input.Fields);
        return formId;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除审批表单")]
    public async Task Delete(HrWfFormDeleteInput input)
    {
        if (!await _rep.IsAnyAsync(u => u.Id == input.Id))
            throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
        if (await _defRep.IsAnyAsync(u => u.FormId == input.Id && u.Enabled))
            throw Oops.Oh(HrWfErrorCodeEnum.FormInUse);

        await _fieldRep.AsDeleteable().Where(u => u.FormId == input.Id).ExecuteCommandAsync();
        await _rep.FakeDeleteAsync(input.Id);
    }

    private async Task<HrWfFormOutput> ToOutputAsync(HrWfForm form)
    {
        var fields = await _fieldRep.AsQueryable()
            .Where(u => u.FormId == form.Id)
            .OrderBy(u => u.SortNo)
            .Select(u => new HrWfFormFieldItem
            {
                Id = u.Id,
                FieldKey = u.FieldKey,
                Label = u.Label,
                ControlType = u.ControlType,
                FieldSource = u.FieldSource,
                PoolFieldKey = u.PoolFieldKey,
                Required = u.Required,
                Readonly = u.Readonly,
                SortNo = u.SortNo,
                GroupCode = u.GroupCode,
                SpanSlots = u.SpanSlots,
                CodeField = u.CodeField,
                DefaultValue = u.DefaultValue,
                Placeholder = u.Placeholder,
                ExtraJson = u.ExtraJson
            })
            .ToListAsync();

        return new HrWfFormOutput
        {
            Id = form.Id,
            BizType = form.BizType,
            Code = form.Code,
            Name = form.Name,
            FormMode = form.FormMode,
            Version = form.Version,
            Enabled = form.Enabled,
            Remark = form.Remark,
            Fields = fields
        };
    }

    private async Task DisableOtherEnabledAsync(string bizType, long? exceptId)
    {
        await _rep.AsUpdateable()
            .SetColumns(u => u.Enabled == false)
            .Where(u => u.BizType == bizType && u.Enabled)
            .WhereIF(exceptId.HasValue, u => u.Id != exceptId)
            .ExecuteCommandAsync();
    }

    private async Task ReplaceFieldsAsync(long formId, List<HrWfFormFieldItem>? fields)
    {
        await _fieldRep.AsDeleteable().Where(u => u.FormId == formId).ExecuteCommandAsync();
        var rows = (fields ?? [])
            .Select((f, i) => new HrWfFormField
            {
                FormId = formId,
                FieldKey = f.FieldKey.Trim(),
                Label = f.Label.Trim(),
                ControlType = string.IsNullOrWhiteSpace(f.ControlType) ? "text" : f.ControlType.Trim(),
                FieldSource = string.IsNullOrWhiteSpace(f.FieldSource) ? HrWfConsts.FieldSourceCustom : f.FieldSource.Trim(),
                PoolFieldKey = f.PoolFieldKey?.Trim(),
                Required = f.Required,
                Readonly = f.Readonly,
                SortNo = f.SortNo != 0 ? f.SortNo : (i + 1) * 10,
                GroupCode = f.GroupCode?.Trim(),
                SpanSlots = f.SpanSlots,
                CodeField = f.CodeField?.Trim(),
                DefaultValue = f.DefaultValue?.Trim(),
                Placeholder = f.Placeholder?.Trim(),
                ExtraJson = f.ExtraJson
            })
            .ToList();
        if (rows.Count > 0)
            await _fieldRep.InsertRangeAsync(rows);
    }

    private static void ValidateFields(List<HrWfFormFieldItem>? fields)
    {
        var keys = (fields ?? []).Select(u => u.FieldKey?.Trim()).Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
        if (keys.Count != keys.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            throw Oops.Oh(HrWfErrorCodeEnum.FormFieldKeyDup);
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
