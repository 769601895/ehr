namespace Admin.NET.Application;

/// <summary>导入单元格 → 业务码（码与显示名都认）</summary>
public class HrEmpImportValueResolver : ITransient
{
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<HrPost> _postRep;
    private readonly SqlSugarRepository<HrPostLang> _postLangRep;
    private readonly HrCodeMstrService _codeSvc;
    private readonly HrLookupDefService _lookupSvc;
    private readonly SqlSugarRepository<HrProvince> _provinceRep;
    private readonly SqlSugarRepository<HrProvinceLang> _provinceLangRep;
    private readonly SqlSugarRepository<HrCity> _cityRep;
    private readonly SqlSugarRepository<HrCityLang> _cityLangRep;

    private Dictionary<string, List<HrCodeItemOutput>>? _codeCache;
    private Dictionary<string, List<HrCodeItemOutput>>? _lookupCache;

    public HrEmpImportValueResolver(
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<HrPost> postRep,
        SqlSugarRepository<HrPostLang> postLangRep,
        HrCodeMstrService codeSvc,
        HrLookupDefService lookupSvc,
        SqlSugarRepository<HrProvince> provinceRep,
        SqlSugarRepository<HrProvinceLang> provinceLangRep,
        SqlSugarRepository<HrCity> cityRep,
        SqlSugarRepository<HrCityLang> cityLangRep)
    {
        _ccRep = ccRep;
        _postRep = postRep;
        _postLangRep = postLangRep;
        _codeSvc = codeSvc;
        _lookupSvc = lookupSvc;
        _provinceRep = provinceRep;
        _provinceLangRep = provinceLangRep;
        _cityRep = cityRep;
        _cityLangRep = cityLangRep;
    }

    public async Task<string?> ResolveAsync(HrUiFieldPoolItemOutput meta, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var text = raw.Trim();

        var key = meta.FieldKey ?? "";
        if (key.Equals("ccCode", StringComparison.OrdinalIgnoreCase))
            return await ResolveCcAsync(text);
        if (key.Equals("postId", StringComparison.OrdinalIgnoreCase))
            return await ResolvePostAsync(text);
        if (key.Equals("empProvince", StringComparison.OrdinalIgnoreCase))
            return await ResolveProvinceAsync(text);
        if (key.Equals("empCity", StringComparison.OrdinalIgnoreCase))
            return await ResolveCityAsync(text);

        if (!string.IsNullOrWhiteSpace(meta.LookupSource))
            return await ResolveOptionsAsync(await GetLookupOptionsAsync(meta.LookupSource!), text, meta.Label);

        if (!string.IsNullOrWhiteSpace(meta.CodeField))
            return await ResolveOptionsAsync(await GetCodeOptionsAsync(meta.CodeField!), text, meta.Label);

        // 文本/日期等原样
        return text;
    }

    private async Task<string> ResolveCcAsync(string text)
    {
        var byCode = await _ccRep.GetFirstAsync(u => u.CcCode == text);
        if (byCode != null) return byCode.CcCode;

        var byName = await _ccRep.AsQueryable()
            .Where(u => u.CcDesc == text || u.CcDescEn == text)
            .Take(2)
            .ToListAsync();
        if (byName.Count == 1) return byName[0].CcCode;
        if (byName.Count > 1) throw Oops.Oh($"部门「{text}」匹配到多条，请填写部门编码");
        throw Oops.Oh($"部门不存在：{text}");
    }

    private async Task<string> ResolvePostAsync(string text)
    {
        var byCode = await _postRep.GetFirstAsync(u => u.PostCode == text);
        if (byCode != null) return byCode.PostCode;

        var culture = HrUiCulture.Current();
        var langHits = await _postLangRep.AsQueryable()
            .Where(u => u.Culture == culture && u.PostName == text)
            .Select(u => u.PostCode)
            .Take(2)
            .ToListAsync();
        if (langHits.Count == 1) return langHits[0];
        if (langHits.Count > 1) throw Oops.Oh($"岗位「{text}」匹配到多条，请填写岗位编码");

        var byName = await _postRep.AsQueryable()
            .Where(u => u.PostName == text)
            .Take(2)
            .ToListAsync();
        if (byName.Count == 1) return byName[0].PostCode;
        if (byName.Count > 1) throw Oops.Oh($"岗位「{text}」匹配到多条，请填写岗位编码");
        throw Oops.Oh($"岗位不存在：{text}");
    }

    private async Task<string> ResolveProvinceAsync(string text)
    {
        var byCode = await _provinceRep.GetFirstAsync(u => u.ProvinceCode == text);
        if (byCode != null) return byCode.ProvinceCode;

        var culture = HrUiCulture.Current();
        var langHits = await _provinceLangRep.AsQueryable()
            .Where(u => u.Culture == culture && u.ProvinceName == text)
            .Select(u => u.ProvinceCode)
            .Take(2)
            .ToListAsync();
        if (langHits.Count == 1) return langHits[0]!;
        if (langHits.Count > 1) throw Oops.Oh($"省份「{text}」匹配到多条，请填写省代码");

        var byName = await _provinceRep.AsQueryable()
            .Where(u => u.ProvinceName == text)
            .Take(2)
            .ToListAsync();
        if (byName.Count == 1) return byName[0].ProvinceCode;
        if (byName.Count > 1) throw Oops.Oh($"省份「{text}」匹配到多条，请填写省代码");
        throw Oops.Oh($"省份不存在：{text}");
    }

    private async Task<string> ResolveCityAsync(string text)
    {
        var byCode = await _cityRep.GetFirstAsync(u => u.CityCode == text);
        if (byCode != null) return byCode.CityCode;

        var culture = HrUiCulture.Current();
        var langHits = await _cityLangRep.AsQueryable()
            .Where(u => u.Culture == culture && u.CityName == text)
            .Select(u => u.CityCode)
            .Take(2)
            .ToListAsync();
        if (langHits.Count == 1) return langHits[0]!;
        if (langHits.Count > 1) throw Oops.Oh($"城市「{text}」匹配到多条，请填写市代码");

        var byName = await _cityRep.AsQueryable()
            .Where(u => u.CityName == text)
            .Take(2)
            .ToListAsync();
        if (byName.Count == 1) return byName[0].CityCode;
        if (byName.Count > 1) throw Oops.Oh($"城市「{text}」匹配到多条，请填写市代码");
        throw Oops.Oh($"城市不存在：{text}");
    }

    private async Task<List<HrCodeItemOutput>> GetCodeOptionsAsync(string codeField)
    {
        _codeCache ??= new(StringComparer.OrdinalIgnoreCase);
        if (_codeCache.TryGetValue(codeField, out var cached)) return cached;
        var list = await _codeSvc.GetCodeList(codeField);
        _codeCache[codeField] = list;
        return list;
    }

    private async Task<List<HrCodeItemOutput>> GetLookupOptionsAsync(string source)
    {
        _lookupCache ??= new(StringComparer.OrdinalIgnoreCase);
        if (_lookupCache.TryGetValue(source, out var cached)) return cached;
        var list = await _lookupSvc.GetOptions(source);
        _lookupCache[source] = list;
        return list;
    }

    private static Task<string> ResolveOptionsAsync(List<HrCodeItemOutput> options, string text, string? fieldLabel)
    {
        var byValue = options.Where(u => string.Equals(u.Value, text, StringComparison.OrdinalIgnoreCase)).ToList();
        if (byValue.Count == 1) return Task.FromResult(byValue[0].Value!);
        if (byValue.Count > 1)
            throw Oops.Oh($"{fieldLabel ?? "字段"}「{text}」匹配到多条编码，请填写代码");

        var byLabel = options.Where(u =>
            string.Equals(u.Label, text, StringComparison.OrdinalIgnoreCase)
            || string.Equals(u.Text, text, StringComparison.OrdinalIgnoreCase)).ToList();
        if (byLabel.Count == 1) return Task.FromResult(byLabel[0].Value!);
        if (byLabel.Count > 1)
            throw Oops.Oh($"{fieldLabel ?? "字段"}「{text}」匹配到多条，请填写代码");
        throw Oops.Oh($"{fieldLabel ?? "字段"}无效：{text}");
    }
}
