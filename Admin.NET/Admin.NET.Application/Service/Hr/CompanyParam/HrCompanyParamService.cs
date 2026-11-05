using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 公司参数设置
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrCompanyParam", Order = 465, Description = "公司参数设置")]
public class HrCompanyParamService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrCompanyParam> _rep;
    private readonly SqlSugarRepository<HrCompanyParamLang> _langRep;
    private readonly SqlSugarRepository<CodeMstr> _codeRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;

    public HrCompanyParamService(
        SqlSugarRepository<HrCompanyParam> rep,
        SqlSugarRepository<HrCompanyParamLang> langRep,
        SqlSugarRepository<CodeMstr> codeRep,
        SqlSugarRepository<CcMstr> ccRep)
    {
        _rep = rep;
        _langRep = langRep;
        _codeRep = codeRep;
        _ccRep = ccRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("公司参数分页")]
    public async Task<SqlSugarPagedList<HrCompanyParamOutput>> Page(HrCompanyParamPageInput input)
    {
        input ??= new HrCompanyParamPageInput();
        var keyword = input.Keyword?.Trim();
        var culture = HrUiCulture.Current();

        // 显式过滤软删：超管会跳过全局 IsDelete 过滤器，否则假删除后列表仍可见
        var query = _rep.AsQueryable()
            .Where(u => !u.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.SiteCode.Contains(keyword!) || u.SiteName.Contains(keyword!));

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(HrCompanyParam.SiteCode), descSort: false)
            .ToPagedListAsync(input.Page, input.PageSize);

        var output = new SqlSugarPagedList<HrCompanyParamOutput>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            Total = page.Total,
            TotalPages = page.TotalPages,
            Items = page.Items.Select(Map).ToList(),
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage
        };
        await FillSiteNamesAsync(output.Items, culture);
        return output;
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("公司参数明细")]
    public async Task<HrCompanyParamOutput> Detail([FromQuery] long? id, [FromQuery] string? siteCode)
    {
        HrCompanyParam? row = null;
        if (id is > 0)
            row = await _rep.GetFirstAsync(u => u.Id == id.Value);
        else if (!string.IsNullOrWhiteSpace(siteCode))
            row = await _rep.GetFirstAsync(u => u.SiteCode == siteCode.Trim());

        if (row == null) throw Oops.Oh(HrCompanyParamConsts.ErrNotFound);

        var output = Map(row);
        await FillSiteNamesAsync([output], HrUiCulture.Current());
        return output;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("新增公司参数")]
    public async Task<long> Add(HrCompanyParamAddInput input)
    {
        var code = input.SiteCode?.Trim();
        var name = input.SiteName?.Trim();
        if (string.IsNullOrWhiteSpace(code)) throw Oops.Oh(HrCompanyParamConsts.ErrSiteCodeEmpty);
        if (string.IsNullOrWhiteSpace(name)) throw Oops.Oh("公司名称不能为空");
        if (await _rep.IsAnyAsync(u => u.SiteCode == code))
            throw Oops.Oh(HrCompanyParamConsts.ErrSiteDup);

        var entity = HrCompanyParamDefaults.New(code, name);
        var inserted = await _rep.InsertReturnEntityAsync(entity);
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新公司参数")]
    public async Task Update(HrCompanyParamUpdateInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh(HrCompanyParamConsts.ErrNotFound);

        var code = input.SiteCode?.Trim();
        var name = input.SiteName?.Trim();
        if (string.IsNullOrWhiteSpace(code)) throw Oops.Oh(HrCompanyParamConsts.ErrSiteCodeEmpty);
        if (string.IsNullOrWhiteSpace(name)) throw Oops.Oh("公司名称不能为空");
        if (await _rep.IsAnyAsync(u => u.SiteCode == code && u.Id != entity.Id))
            throw Oops.Oh(HrCompanyParamConsts.ErrSiteDup);

        ValidateRanges(input);
        await ValidateCodesAsync(input);

        entity.SiteCode = code;
        entity.SiteName = name;
        ApplyDetail(entity, input);
        await _rep.UpdateAsync(entity);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除公司参数")]
    public async Task Delete(HrCompanyParamDeleteInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh(HrCompanyParamConsts.ErrNotFound);
        var siteCode = entity.SiteCode;

        await _langRep.DeleteAsync(u => u.ParamId == entity.Id);
        // 清理组织上对该厂别的归属引用
        if (!string.IsNullOrWhiteSpace(siteCode))
        {
            await _ccRep.AsUpdateable()
                .SetColumns(u => new CcMstr { BelongSiteCode = null })
                .Where(u => u.BelongSiteCode == siteCode)
                .ExecuteCommandAsync();
        }
        // 物理删除：超管跳过软删过滤器，假删除后列表仍会看到该行
        await _rep.DeleteAsync(entity);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("公司参数名称译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] long id)
    {
        if (id <= 0) return [];
        var rows = await _langRep.AsQueryable().Where(u => u.ParamId == id).ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.SiteName }).ToList();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存公司参数名称译文")]
    public async Task SaveLang(HrCompanyParamLangSaveInput input)
    {
        if (!await _rep.IsAnyAsync(u => u.Id == input.Id))
            throw Oops.Oh(HrCompanyParamConsts.ErrNotFound);

        await _langRep.DeleteAsync(u => u.ParamId == input.Id);
        var rows = (input.Labels ?? [])
            .Select(u => new HrCompanyParamLang
            {
                ParamId = input.Id,
                Culture = HrUiCulture.Normalize(u.Culture),
                SiteName = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.SiteName))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }

    /// <summary>公司型组织分页（组织类型=公司）</summary>
    [ApiDescriptionSettings(Name = "CompanyOrgPage"), HttpPost]
    [DisplayName("归属公司设置分页")]
    public async Task<SqlSugarPagedList<HrCompanyOrgOutput>> CompanyOrgPage(HrCompanyOrgPageInput input)
    {
        input ??= new HrCompanyOrgPageInput();
        var keyword = input.Keyword?.Trim();
        var culture = HrUiCulture.Current();

        var query = _ccRep.AsQueryable()
            .Where(u => u.OrgType == HrCompanyParamConsts.OrgTypeCompany)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.CcCode.Contains(keyword!) || u.CcDesc.Contains(keyword!)
                     || (u.CcDept != null && u.CcDept.Contains(keyword!))
                     || (u.BelongSiteCode != null && u.BelongSiteCode.Contains(keyword!)));

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(CcMstr.CcLevel), descSort: false)
            .ToPagedListAsync(input.Page, input.PageSize);

        var items = page.Items.Select(u => new HrCompanyOrgOutput
        {
            CcCode = u.CcCode,
            CcDesc = u.CcDesc,
            CcDept = u.CcDept,
            OrgType = u.OrgType,
            CcLevel = u.CcLevel,
            BelongSiteCode = u.BelongSiteCode
        }).ToList();

        await FillOrgTypeNamesAsync(items, culture);
        await FillBelongSiteNamesAsync(items, culture);

        return new SqlSugarPagedList<HrCompanyOrgOutput>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            Total = page.Total,
            TotalPages = page.TotalPages,
            Items = items,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage
        };
    }

    /// <summary>厂别下拉（公司参数主表）</summary>
    [ApiDescriptionSettings(Name = "SiteOptions"), HttpGet]
    [DisplayName("公司参数厂别下拉")]
    public async Task<List<HrCompanyParamSiteOption>> SiteOptions()
    {
        var culture = HrUiCulture.Current();
        var rows = await _rep.AsQueryable().Where(u => !u.IsDelete).OrderBy(u => u.SiteCode).ToListAsync();
        var list = rows.Select(u => new HrCompanyParamSiteOption
        {
            SiteCode = u.SiteCode,
            SiteName = u.SiteName
        }).ToList();
        if (list.Count == 0) return list;

        var idByCode = rows.ToDictionary(u => u.SiteCode, u => u.Id, StringComparer.OrdinalIgnoreCase);
        var ids = idByCode.Values.Distinct().ToList();
        var langs = await _langRep.AsQueryable()
            .Where(u => ids.Contains(u.ParamId) && u.Culture == culture)
            .ToListAsync();
        var nameById = langs.ToDictionary(u => u.ParamId, u => u.SiteName);
        foreach (var item in list)
        {
            if (idByCode.TryGetValue(item.SiteCode, out var id)
                && nameById.TryGetValue(id, out var n)
                && !string.IsNullOrWhiteSpace(n))
                item.SiteName = n!;
        }
        return list;
    }

    /// <summary>批量保存公司型组织的归属厂别</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveCompanyOrgSite"), HttpPost]
    [DisplayName("保存归属公司设置")]
    public async Task SaveCompanyOrgSite(HrCompanyOrgSiteSaveInput input)
    {
        var items = (input.Items ?? [])
            .Where(u => !string.IsNullOrWhiteSpace(u.CcCode))
            .GroupBy(u => u.CcCode.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (items.Count == 0) return;

        var codes = items.Select(u => u.CcCode.Trim()).ToList();
        var orgs = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .ToListAsync();
        var orgMap = orgs.ToDictionary(u => u.CcCode, StringComparer.OrdinalIgnoreCase);

        var siteCodes = items
            .Select(u => u.BelongSiteCode?.Trim())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        HashSet<string> validSites = [];
        if (siteCodes.Count > 0)
        {
            var sites = await _rep.AsQueryable()
                .Where(u => siteCodes.Contains(u.SiteCode))
                .Select(u => u.SiteCode)
                .ToListAsync();
            validSites = sites.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        foreach (var item in items)
        {
            var code = item.CcCode.Trim();
            if (!orgMap.TryGetValue(code, out var org))
                throw Oops.Oh(HrCompanyParamConsts.ErrNotFound);
            if (org.OrgType != HrCompanyParamConsts.OrgTypeCompany)
                throw Oops.Oh(HrCompanyParamConsts.ErrOrgNotCompany);

            var site = string.IsNullOrWhiteSpace(item.BelongSiteCode) ? null : item.BelongSiteCode.Trim();
            if (site != null && !validSites.Contains(site))
                throw Oops.Oh(HrCompanyParamConsts.ErrBelongSiteInvalid);

            org.BelongSiteCode = site;
            await _ccRep.AsUpdateable(org)
                .UpdateColumns(u => new { u.BelongSiteCode })
                .WhereColumns(u => new { u.CcCode })
                .ExecuteCommandAsync();
        }
    }

    private async Task FillSiteNamesAsync(IEnumerable<HrCompanyParamOutput>? items, string culture)
    {
        if (items == null) return;
        var list = items.ToList();
        if (list.Count == 0) return;
        var ids = list.Select(u => u.Id).Distinct().ToList();
        var langs = await _langRep.AsQueryable()
            .Where(u => ids.Contains(u.ParamId) && u.Culture == culture)
            .ToListAsync();
        var map = langs.ToDictionary(u => u.ParamId, u => u.SiteName);
        foreach (var item in list)
        {
            if (map.TryGetValue(item.Id, out var n) && !string.IsNullOrWhiteSpace(n))
                item.SiteName = n!;
        }
    }

    private async Task FillOrgTypeNamesAsync(List<HrCompanyOrgOutput> items, string culture)
    {
        if (items.Count == 0) return;
        var values = items.Select(u => u.OrgType).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (values.Count == 0) return;

        var codes = await _codeRep.AsQueryable()
            .Where(u => u.CodeField == "OrgType" && values.Contains(u.CodeValue))
            .ToListAsync();
        var descMap = codes.ToDictionary(u => u.CodeValue, u => u.CodeDesc, StringComparer.OrdinalIgnoreCase);

        // 译文优先（若存在 code_mstr_lang 由业务侧统一；此处用主表描述即可）
        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.OrgType) && descMap.TryGetValue(item.OrgType!, out var d))
                item.OrgTypeName = d;
            else
                item.OrgTypeName = item.OrgType;
        }
        _ = culture;
    }

    private async Task FillBelongSiteNamesAsync(List<HrCompanyOrgOutput> items, string culture)
    {
        var siteCodes = items
            .Select(u => u.BelongSiteCode)
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (siteCodes.Count == 0) return;

        var sites = await _rep.AsQueryable()
            .Where(u => siteCodes.Contains(u.SiteCode))
            .ToListAsync();
        if (sites.Count == 0) return;

        var ids = sites.Select(u => u.Id).ToList();
        var langs = await _langRep.AsQueryable()
            .Where(u => ids.Contains(u.ParamId) && u.Culture == culture)
            .ToListAsync();
        var langMap = langs.ToDictionary(u => u.ParamId, u => u.SiteName);
        var nameByCode = sites.ToDictionary(
            u => u.SiteCode,
            u => langMap.TryGetValue(u.Id, out var n) && !string.IsNullOrWhiteSpace(n) ? n! : u.SiteName,
            StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.BelongSiteCode)
                && nameByCode.TryGetValue(item.BelongSiteCode!, out var name))
                item.BelongSiteName = name;
        }
    }

    private async Task ValidateCodesAsync(HrCompanyParamUpdateInput input)
    {
        var pairs = new (string Field, string? Value)[]
        {
            (nameof(HrCompanyParam.EmpIdMode), input.EmpIdMode.ToString()),
            (nameof(HrCompanyParam.ProbationOnHire), input.ProbationOnHire.ToString()),
            (nameof(HrCompanyParam.ProbationDayInclusive), input.ProbationDayInclusive.ToString()),
            (nameof(HrCompanyParam.DimissionAudit), input.DimissionAudit.ToString()),
            (nameof(HrCompanyParam.TransferAudit), input.TransferAudit.ToString()),
            (nameof(HrCompanyParam.RegularizeAudit), input.RegularizeAudit.ToString()),
            (nameof(HrCompanyParam.IdCardV2Enabled), input.IdCardV2Enabled.ToString()),
            (nameof(HrCompanyParam.HireDayAttMode), input.HireDayAttMode.ToString()),
            (nameof(HrCompanyParam.LeaveDayAttMode), input.LeaveDayAttMode.ToString()),
            (nameof(HrCompanyParam.RequiredAttDayMode), input.RequiredAttDayMode.ToString()),
            (nameof(HrCompanyParam.PayrollAttMonth), input.PayrollAttMonth.ToString()),
            (nameof(HrCompanyParam.OtAuditMode), input.OtAuditMode.ToString()),
            (nameof(HrCompanyParam.LeaveAudit), input.LeaveAudit.ToString()),
            (nameof(HrCompanyParam.OtCrossdayMode), input.OtCrossdayMode.ToString()),
            (nameof(HrCompanyParam.CardMakeupAudit), input.CardMakeupAudit.ToString()),
            (nameof(HrCompanyParam.InsMonthReportMode), input.InsMonthReportMode.ToString()),
            (nameof(HrCompanyParam.InsFeeToPayroll), input.InsFeeToPayroll.ToString()),
            (nameof(HrCompanyParam.InsSameMonthReport), input.InsSameMonthReport.ToString()),
        };

        foreach (var (field, value) in pairs)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;
            var ok = await _codeRep.IsAnyAsync(u => u.CodeField == field && u.CodeValue == value);
            if (!ok) throw Oops.Oh(HrCompanyParamConsts.ErrCodeInvalid);
        }
    }

    private static void ValidateRanges(HrCompanyParamUpdateInput input)
    {
        if (input.AttCloseDay is < 0 or > 31) throw Oops.Oh("考勤结束日须在 0–31");
        if (input.OtMinUnitHour < 0) throw Oops.Oh("加班最小单位不能为负");
        if (input.EmpIdLength < 0) throw Oops.Oh("工号长度不能为负");
    }

    private static void ApplyDetail(HrCompanyParam entity, HrCompanyParamUpdateInput input)
    {
        entity.EmpIdMode = input.EmpIdMode;
        entity.EmpIdLength = input.EmpIdLength;
        entity.EmpIdFixedPrefix = input.EmpIdMode is 4 or 8 or 9 ? input.EmpIdFixedPrefix?.Trim() : null;
        entity.ProbationOnHire = input.ProbationOnHire;
        entity.ProbationHireDays = input.ProbationHireDays;
        entity.ProbationDayInclusive = input.ProbationDayInclusive;
        entity.DimissionAudit = input.DimissionAudit;
        entity.TransferAudit = input.TransferAudit;
        entity.RegularizeAudit = input.RegularizeAudit;
        entity.IdCardV2Enabled = input.IdCardV2Enabled;
        entity.HireDayAttMode = input.HireDayAttMode;
        entity.LeaveDayAttMode = input.LeaveDayAttMode;
        entity.AttCloseDay = input.AttCloseDay;
        entity.RequiredAttDayMode = input.RequiredAttDayMode;
        entity.PayrollAttMonth = input.PayrollAttMonth;
        entity.OtAuditMode = input.OtAuditMode;
        entity.LeaveAudit = input.LeaveAudit;
        entity.OtCrossdayMode = input.OtCrossdayMode;
        entity.CardMakeupAudit = input.CardMakeupAudit;
        entity.OtMinUnitHour = input.OtMinUnitHour;
        entity.InsMonthReportMode = input.InsMonthReportMode;
        entity.InsFeeToPayroll = input.InsFeeToPayroll;
        entity.InsSameMonthReport = input.InsSameMonthReport;
    }

    private static HrCompanyParamOutput Map(HrCompanyParam u) => new()
    {
        Id = u.Id,
        SiteCode = u.SiteCode,
        SiteName = u.SiteName,
        EmpIdMode = u.EmpIdMode,
        EmpIdLength = u.EmpIdLength,
        EmpIdFixedPrefix = u.EmpIdFixedPrefix,
        ProbationOnHire = u.ProbationOnHire,
        ProbationHireDays = u.ProbationHireDays,
        ProbationDayInclusive = u.ProbationDayInclusive,
        DimissionAudit = u.DimissionAudit,
        TransferAudit = u.TransferAudit,
        RegularizeAudit = u.RegularizeAudit,
        IdCardV2Enabled = u.IdCardV2Enabled,
        HireDayAttMode = u.HireDayAttMode,
        LeaveDayAttMode = u.LeaveDayAttMode,
        AttCloseDay = u.AttCloseDay,
        RequiredAttDayMode = u.RequiredAttDayMode,
        PayrollAttMonth = u.PayrollAttMonth,
        OtAuditMode = u.OtAuditMode,
        LeaveAudit = u.LeaveAudit,
        OtCrossdayMode = u.OtCrossdayMode,
        CardMakeupAudit = u.CardMakeupAudit,
        OtMinUnitHour = u.OtMinUnitHour,
        InsMonthReportMode = u.InsMonthReportMode,
        InsFeeToPayroll = u.InsFeeToPayroll,
        InsSameMonthReport = u.InsSameMonthReport,
    };

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
