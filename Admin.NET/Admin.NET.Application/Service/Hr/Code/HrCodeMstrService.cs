using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 代码设置。下拉读在这里实现，hrOrg/codeList 只转发。
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrCodeMstr", Order = 475, Description = "代码设置")]
public class HrCodeMstrService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CodeMstr> _codeRep;
    private readonly SqlSugarRepository<CodeMstrLang> _langRep;
    private readonly SqlSugarRepository<CodeField> _fieldRep;
    private readonly SqlSugarRepository<CodeFieldLang> _fieldLangRep;
    private readonly UserManager _userManager;

    public HrCodeMstrService(
        SqlSugarRepository<CodeMstr> codeRep,
        SqlSugarRepository<CodeMstrLang> langRep,
        SqlSugarRepository<CodeField> fieldRep,
        SqlSugarRepository<CodeFieldLang> fieldLangRep,
        UserManager userManager)
    {
        _codeRep = codeRep;
        _langRep = langRep;
        _fieldRep = fieldRep;
        _fieldLangRep = fieldLangRep;
        _userManager = userManager;
    }

    #region 代码类

    [ApiDescriptionSettings(Name = "GetField"), HttpGet]
    [DisplayName("代码类详情")]
    public async Task<HrCodeFieldOutput> GetField([FromQuery] string codeField)
    {
        var key = codeField?.Trim();
        var row = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == key && !u.IsDelete)
            ?? throw Oops.Oh("代码类不存在");
        // 编辑回显用主表原文，避免把译文写回主字段
        return new HrCodeFieldOutput
        {
            CodeField = row.CodeFieldKey,
            FieldName = row.FieldName,
            UsageDesc = row.UsageDesc,
            OrderNo = row.OrderNo,
            IsDelete = row.IsDelete
        };
    }

    [ApiDescriptionSettings(Name = "ListField"), HttpGet]
    [DisplayName("代码类列表")]
    public async Task<List<HrCodeFieldOutput>> ListField([FromQuery] string keyword = null)
    {
        await EnsureFieldsSyncedAsync();
        var culture = HrUiCulture.Current();
        var kw = keyword?.Trim();
        var rows = await _fieldRep.AsQueryable()
            .Where(u => !u.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(kw), u =>
                u.CodeFieldKey.Contains(kw) || u.FieldName.Contains(kw) || u.UsageDesc.Contains(kw))
            .OrderBy(u => u.OrderNo)
            .OrderBy(u => u.CodeFieldKey)
            .ToListAsync();
        return await MapFieldsAsync(rows, culture);
    }

    [ApiDescriptionSettings(Name = "ListDeletedField"), HttpGet]
    [DisplayName("已删除代码类")]
    public async Task<List<HrCodeFieldOutput>> ListDeletedField([FromQuery] string keyword = null)
    {
        var culture = HrUiCulture.Current();
        var kw = keyword?.Trim();
        var rows = await _fieldRep.AsQueryable()
            .Where(u => u.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(kw), u =>
                u.CodeFieldKey.Contains(kw) || u.FieldName.Contains(kw) || u.UsageDesc.Contains(kw))
            .OrderBy(u => u.DeleteTime, OrderByType.Desc)
            .OrderBy(u => u.CodeFieldKey)
            .ToListAsync();
        return await MapFieldsAsync(rows, culture);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveField"), HttpPost]
    [DisplayName("保存代码类")]
    public async Task SaveField(HrCodeFieldSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var fieldName = input.FieldName?.Trim();
        if (string.IsNullOrWhiteSpace(codeField) || string.IsNullOrWhiteSpace(fieldName))
            throw Oops.Oh("HR_UI_CODE_FIELD_EMPTY");

        var now = DateTime.Now;
        var current = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == codeField);
        if (input.IsNew)
        {
            if (current != null)
            {
                if (!current.IsDelete) throw Oops.Oh("代码类编码已存在");
                // 已删除同编码：恢复并更新说明（编码不可改，等同找回后改名）
                current.IsDelete = false;
                current.DeleteTime = null;
                current.DeleteUserId = null;
                current.DeleteUserName = null;
                current.FieldName = fieldName;
                current.UsageDesc = input.UsageDesc?.Trim();
                current.OrderNo = input.OrderNo;
                current.UpdateTime = now;
                await _fieldRep.UpdateAsync(current);
            }
            else
            {
                await _fieldRep.InsertAsync(new CodeField
                {
                    CodeFieldKey = codeField,
                    FieldName = fieldName,
                    UsageDesc = input.UsageDesc?.Trim(),
                    OrderNo = input.OrderNo,
                    IsDelete = false,
                    CreateTime = now,
                    UpdateTime = now
                });
            }
        }
        else
        {
            if (current == null || current.IsDelete) throw Oops.Oh("代码类不存在");
            current.FieldName = fieldName;
            current.UsageDesc = input.UsageDesc?.Trim();
            current.OrderNo = input.OrderNo;
            current.UpdateTime = now;
            await _fieldRep.UpdateAsync(current);
        }

        if (input.Langs is { Count: > 0 })
            await ReplaceFieldLangsAsync(codeField, input.Langs);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteField"), HttpPost]
    [DisplayName("删除代码类")]
    public async Task DeleteField(HrCodeFieldDeleteInput input)
    {
        var codeField = input.CodeField?.Trim();
        var row = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == codeField && !u.IsDelete)
            ?? throw Oops.Oh("代码类不存在");
        row.IsDelete = true;
        row.DeleteTime = DateTime.Now;
        row.DeleteUserId = _userManager.UserId;
        row.DeleteUserName = Trunc(_userManager.RealName ?? _userManager.Account, 64);
        row.UpdateTime = row.DeleteTime;
        await _fieldRep.UpdateAsync(row);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "RestoreField"), HttpPost]
    [DisplayName("恢复代码类")]
    public async Task RestoreField(HrCodeFieldDeleteInput input)
    {
        var codeField = input.CodeField?.Trim();
        var row = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == codeField && u.IsDelete)
            ?? throw Oops.Oh("已删除代码类不存在");
        row.IsDelete = false;
        row.DeleteTime = null;
        row.DeleteUserId = null;
        row.DeleteUserName = null;
        row.UpdateTime = DateTime.Now;
        await _fieldRep.UpdateAsync(row);
    }

    [ApiDescriptionSettings(Name = "FieldLangList"), HttpGet]
    [DisplayName("代码类译文")]
    public async Task<List<HrCodeFieldLangOutput>> FieldLangList([FromQuery] string codeField)
    {
        var key = codeField?.Trim();
        var rows = await _fieldLangRep.AsQueryable()
            .Where(u => u.CodeFieldKey == key)
            .ToListAsync();
        return rows.Select(u => new HrCodeFieldLangOutput
        {
            Culture = u.Culture,
            FieldName = u.FieldName,
            UsageDesc = u.UsageDesc
        }).ToList();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveFieldLang"), HttpPost]
    [DisplayName("保存代码类译文")]
    public async Task SaveFieldLang(HrCodeFieldLangSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var exists = await _fieldRep.IsAnyAsync(u => u.CodeFieldKey == codeField && !u.IsDelete);
        if (!exists) throw Oops.Oh("代码类不存在");
        await ReplaceFieldLangsAsync(codeField, input.Langs ?? []);
    }

    #endregion

    #region 码值

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("代码分页")]
    public async Task<SqlSugarPagedList<HrCodeMstrOutput>> Page(HrCodeMstrPageInput input)
    {
        input ??= new HrCodeMstrPageInput();
        var keyword = input.Keyword?.Trim();
        var field = input.CodeField?.Trim();
        var query = _codeRep.AsQueryable()
            .Where(u => !u.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(field), u => u.CodeField == field)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.CodeValue.Contains(keyword) || u.CodeDesc.Contains(keyword));

        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        return await query
            .OrderBuilder(input, defaultSortField: nameof(CodeMstr.CodeValue), descSort: false)
            .Select<HrCodeMstrOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "ListDeleted"), HttpGet]
    [DisplayName("已删除码值")]
    public async Task<List<HrCodeMstrOutput>> ListDeleted([FromQuery] string codeField, [FromQuery] string keyword = null)
    {
        var field = codeField?.Trim();
        if (string.IsNullOrWhiteSpace(field)) return [];
        var kw = keyword?.Trim();
        return await _codeRep.AsQueryable()
            .Where(u => u.CodeField == field && u.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(kw), u => u.CodeValue.Contains(kw) || u.CodeDesc.Contains(kw))
            .OrderBy(u => u.DeleteTime, OrderByType.Desc)
            .OrderBy(u => u.CodeValue)
            .Select<HrCodeMstrOutput>()
            .ToListAsync();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存代码")]
    public async Task Save(HrCodeMstrSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        if (string.IsNullOrWhiteSpace(codeField) || string.IsNullOrWhiteSpace(codeValue))
            throw Oops.Oh("HR_UI_CODE_FIELD_EMPTY");

        await EnsureFieldExistsAsync(codeField);

        var oldValue = string.IsNullOrWhiteSpace(input.OldCodeValue) ? codeValue : input.OldCodeValue.Trim();
        var current = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == oldValue && !u.IsDelete);
        if (current == null && !string.Equals(oldValue, codeValue, StringComparison.Ordinal))
            throw Oops.Oh("HR_CODE_NOT_FOUND");

        if (current == null)
        {
            var soft = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && u.IsDelete);
            if (soft != null)
            {
                soft.IsDelete = false;
                soft.DeleteTime = null;
                soft.DeleteUserId = null;
                soft.DeleteUserName = null;
                soft.CodeDesc = input.CodeDesc?.Trim();
                soft.CodeSort = input.CodeSort?.Trim();
                await _codeRep.UpdateAsync(soft);
                return;
            }

            var dup = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && !u.IsDelete);
            if (dup) throw Oops.Oh("HR_CODE_DUP");
            await _codeRep.InsertAsync(new CodeMstr
            {
                CodeField = codeField,
                CodeValue = codeValue,
                CodeDesc = input.CodeDesc?.Trim(),
                CodeSort = input.CodeSort?.Trim(),
                IsDelete = false
            });
            return;
        }

        if (!string.Equals(oldValue, codeValue, StringComparison.Ordinal))
        {
            var dup = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && !u.IsDelete);
            if (dup) throw Oops.Oh("HR_CODE_DUP");
            var softTarget = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && u.IsDelete);
            if (softTarget != null)
            {
                await _codeRep.DeleteAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && u.IsDelete);
            }

            current.IsDelete = true;
            current.DeleteTime = DateTime.Now;
            current.DeleteUserId = _userManager.UserId;
            current.DeleteUserName = Trunc(_userManager.RealName ?? _userManager.Account, 64);
            await _codeRep.UpdateAsync(current);

            await _codeRep.InsertAsync(new CodeMstr
            {
                CodeField = codeField,
                CodeValue = codeValue,
                CodeDesc = input.CodeDesc?.Trim(),
                CodeSort = input.CodeSort?.Trim(),
                IsDelete = false
            });
            await _langRep.AsUpdateable()
                .SetColumns(u => u.CodeValue == codeValue)
                .Where(u => u.CodeField == codeField && u.CodeValue == oldValue)
                .ExecuteCommandAsync();
            return;
        }

        current.CodeDesc = input.CodeDesc?.Trim();
        current.CodeSort = input.CodeSort?.Trim();
        await _codeRep.UpdateAsync(current);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除代码")]
    public async Task Delete(HrCodeMstrDeleteInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        var row = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && !u.IsDelete)
            ?? throw Oops.Oh("HR_CODE_NOT_FOUND");
        // 一律软删进回收站；译文保留，业务仍可按旧码值解析描述
        row.IsDelete = true;
        row.DeleteTime = DateTime.Now;
        row.DeleteUserId = _userManager.UserId;
        row.DeleteUserName = Trunc(_userManager.RealName ?? _userManager.Account, 64);
        await _codeRep.UpdateAsync(row);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Restore"), HttpPost]
    [DisplayName("恢复码值")]
    public async Task Restore(HrCodeMstrDeleteInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        var row = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && u.IsDelete)
            ?? throw Oops.Oh("已删除码值不存在");
        if (await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && !u.IsDelete))
            throw Oops.Oh("HR_CODE_DUP");
        row.IsDelete = false;
        row.DeleteTime = null;
        row.DeleteUserId = null;
        row.DeleteUserName = null;
        await _codeRep.UpdateAsync(row);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("代码译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] string codeField, [FromQuery] string codeValue)
    {
        var rows = await _langRep.AsQueryable()
            .Where(u => u.CodeField == codeField && u.CodeValue == codeValue)
            .ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.CodeDesc }).ToList();
    }

    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存代码译文")]
    public async Task SaveLang(HrCodeLangSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        var exists = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue && !u.IsDelete);
        if (!exists) throw Oops.Oh("HR_CODE_NOT_FOUND");

        await _langRep.DeleteAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
        var rows = (input.Labels ?? [])
            .Select(u => new CodeMstrLang
            {
                CodeField = codeField,
                CodeValue = codeValue,
                Culture = HrUiCulture.Normalize(u.Culture),
                CodeDesc = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.CodeDesc))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }

    /// <summary>
    /// value=code_value；label 与 text 都是当前文化的描述（下拉、列表只显示描述）。顺序仍是 code_sort、code_value。
    /// 代码类软删后下拉返回空，避免继续选用。
    /// includeDeleted=false：仅有效码值（下拉/录入校验）；true：含软删，供历史单据按旧码值解析显示名。
    /// </summary>
    [NonAction]
    public Task<List<HrCodeItemOutput>> GetCodeList(string codeField) => GetCodeList(codeField, includeDeleted: false);

    [NonAction]
    public async Task<List<HrCodeItemOutput>> GetCodeList(string codeField, bool includeDeleted)
    {
        if (string.IsNullOrWhiteSpace(codeField))
            return [];

        var meta = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == codeField);
        if (meta is { IsDelete: true })
            return [];

        var list = await _codeRep.AsQueryable()
            .Where(u => u.CodeField == codeField)
            .WhereIF(!includeDeleted, u => !u.IsDelete)
            .OrderBy(u => u.CodeSort)
            .OrderBy(u => u.CodeValue)
            .ToListAsync();
        if (list.Count == 0) return [];

        var culture = HrUiCulture.Current();
        var values = list.Select(u => u.CodeValue).ToList();
        var langs = await _langRep.AsQueryable()
            .Where(u => u.CodeField == codeField && values.Contains(u.CodeValue) && u.Culture == culture)
            .ToListAsync();
        var langMap = langs.ToDictionary(u => u.CodeValue, u => u.CodeDesc, StringComparer.OrdinalIgnoreCase);

        return list.Select(u =>
        {
            IReadOnlyDictionary<string, string> one = langMap.TryGetValue(u.CodeValue, out var desc) && desc != null
                ? new Dictionary<string, string> { [culture] = desc }
                : null;
            var text = HrUiI18nResolver.ResolveCodeDesc(u.CodeValue, u.CodeDesc, culture, one);
            return new HrCodeItemOutput
            {
                Value = u.CodeValue,
                Label = text,
                Text = text
            };
        }).ToList();
    }

    #endregion

    #region helpers

    private async Task EnsureFieldsSyncedAsync()
    {
        var existing = await _fieldRep.AsQueryable().Select(u => u.CodeFieldKey).ToListAsync();
        var existingSet = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
        var fromMstr = await _codeRep.AsQueryable().Select(u => u.CodeField).Distinct().ToListAsync();
        var missing = fromMstr
            .Where(u => !string.IsNullOrWhiteSpace(u) && !existingSet.Contains(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (missing.Count == 0) return;

        var now = DateTime.Now;
        var rows = missing.Select(f => new CodeField
        {
            CodeFieldKey = f.Trim(),
            FieldName = f.Trim(),
            UsageDesc = "（自动同步，请补充用途说明）",
            OrderNo = 900,
            IsDelete = false,
            CreateTime = now,
            UpdateTime = now
        }).ToList();
        await _fieldRep.InsertRangeAsync(rows);
    }

    private async Task EnsureFieldExistsAsync(string codeField)
    {
        var row = await _fieldRep.GetFirstAsync(u => u.CodeFieldKey == codeField);
        if (row == null)
        {
            var now = DateTime.Now;
            await _fieldRep.InsertAsync(new CodeField
            {
                CodeFieldKey = codeField,
                FieldName = codeField,
                UsageDesc = "（随码值自动建立，请补充用途说明）",
                OrderNo = 900,
                IsDelete = false,
                CreateTime = now,
                UpdateTime = now
            });
            return;
        }
        if (row.IsDelete)
            throw Oops.Oh("代码类已删除，请先恢复后再维护码值");
    }

    private async Task<List<HrCodeFieldOutput>> MapFieldsAsync(List<CodeField> rows, string culture)
    {
        if (rows.Count == 0) return [];
        var keys = rows.Select(u => u.CodeFieldKey).ToList();
        var langs = await _fieldLangRep.AsQueryable()
            .Where(u => keys.Contains(u.CodeFieldKey) && u.Culture == culture)
            .ToListAsync();
        var langMap = langs.ToDictionary(u => u.CodeFieldKey, u => u, StringComparer.OrdinalIgnoreCase);

        return rows.Select(u =>
        {
            langMap.TryGetValue(u.CodeFieldKey, out var lang);
            var name = !string.IsNullOrWhiteSpace(lang?.FieldName) ? lang.FieldName.Trim() : u.FieldName;
            var usage = !string.IsNullOrWhiteSpace(lang?.UsageDesc) ? lang.UsageDesc.Trim() : u.UsageDesc;
            return new HrCodeFieldOutput
            {
                CodeField = u.CodeFieldKey,
                FieldName = name,
                UsageDesc = usage,
                OrderNo = u.OrderNo,
                IsDelete = u.IsDelete,
                DeleteTime = u.DeleteTime,
                DeleteUserName = u.DeleteUserName
            };
        }).ToList();
    }

    private async Task ReplaceFieldLangsAsync(string codeField, List<HrCodeFieldLangItemInput> langs)
    {
        await _fieldLangRep.DeleteAsync(u => u.CodeFieldKey == codeField);
        var rows = langs
            .Select(u => new CodeFieldLang
            {
                CodeFieldKey = codeField,
                Culture = HrUiCulture.Normalize(u.Culture),
                FieldName = u.FieldName?.Trim(),
                UsageDesc = u.UsageDesc?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.Culture)
                        && (!string.IsNullOrWhiteSpace(u.FieldName) || !string.IsNullOrWhiteSpace(u.UsageDesc)))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _fieldLangRep.InsertRangeAsync(rows);
    }

    private static string Trunc(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max) return value;
        return value[..max];
    }

    #endregion
}
