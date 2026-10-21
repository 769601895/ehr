using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 人事代码。下拉读在这里实现，hrOrg/codeList 只转发。
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrCodeMstr", Order = 475, Description = "人事代码")]
public class HrCodeMstrService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CodeMstr> _codeRep;
    private readonly SqlSugarRepository<CodeMstrLang> _langRep;

    public HrCodeMstrService(SqlSugarRepository<CodeMstr> codeRep, SqlSugarRepository<CodeMstrLang> langRep)
    {
        _codeRep = codeRep;
        _langRep = langRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("人事代码分页")]
    public async Task<SqlSugarPagedList<HrCodeMstrOutput>> Page(HrCodeMstrPageInput input)
    {
        input ??= new HrCodeMstrPageInput();
        var keyword = input.Keyword?.Trim();
        var field = input.CodeField?.Trim();
        var query = _codeRep.AsQueryable()
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

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存人事代码")]
    public async Task Save(HrCodeMstrSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        if (string.IsNullOrWhiteSpace(codeField) || string.IsNullOrWhiteSpace(codeValue))
            throw Oops.Oh("HR_UI_CODE_FIELD_EMPTY");

        var oldValue = string.IsNullOrWhiteSpace(input.OldCodeValue) ? codeValue : input.OldCodeValue.Trim();
        var current = await _codeRep.GetFirstAsync(u => u.CodeField == codeField && u.CodeValue == oldValue);
        if (current == null && !string.Equals(oldValue, codeValue, StringComparison.Ordinal))
            throw Oops.Oh("HR_CODE_NOT_FOUND");

        if (current == null)
        {
            var dup = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
            if (dup) throw Oops.Oh("HR_CODE_DUP");
            await _codeRep.InsertAsync(new CodeMstr
            {
                CodeField = codeField,
                CodeValue = codeValue,
                CodeDesc = input.CodeDesc?.Trim(),
                CodeSort = input.CodeSort?.Trim()
            });
            return;
        }

        if (!string.Equals(oldValue, codeValue, StringComparison.Ordinal))
        {
            var dup = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
            if (dup) throw Oops.Oh("HR_CODE_DUP");
            await _codeRep.DeleteAsync(u => u.CodeField == codeField && u.CodeValue == oldValue);
            await _codeRep.InsertAsync(new CodeMstr
            {
                CodeField = codeField,
                CodeValue = codeValue,
                CodeDesc = input.CodeDesc?.Trim(),
                CodeSort = input.CodeSort?.Trim()
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
    [DisplayName("删除人事代码")]
    public async Task Delete(HrCodeMstrDeleteInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        var exists = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
        if (!exists) throw Oops.Oh("HR_CODE_NOT_FOUND");
        await _langRep.DeleteAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
        await _codeRep.DeleteAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("人事代码译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] string codeField, [FromQuery] string codeValue)
    {
        var rows = await _langRep.AsQueryable()
            .Where(u => u.CodeField == codeField && u.CodeValue == codeValue)
            .ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.CodeDesc }).ToList();
    }

    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存人事代码译文")]
    public async Task SaveLang(HrCodeLangSaveInput input)
    {
        var codeField = input.CodeField?.Trim();
        var codeValue = input.CodeValue?.Trim();
        var exists = await _codeRep.IsAnyAsync(u => u.CodeField == codeField && u.CodeValue == codeValue);
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
    /// </summary>
    [NonAction]
    public async Task<List<HrCodeItemOutput>> GetCodeList(string codeField)
    {
        if (string.IsNullOrWhiteSpace(codeField))
            return [];
        var list = await _codeRep.AsQueryable()
            .Where(u => u.CodeField == codeField)
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
}
