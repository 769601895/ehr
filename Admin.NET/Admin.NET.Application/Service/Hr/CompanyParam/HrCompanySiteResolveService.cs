namespace Admin.NET.Application;

/// <summary>
/// 部门 → 归属公司 SiteCode 解析（公司参数取参前置）
/// </summary>
public class HrCompanySiteResolveService : ITransient
{
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<SysOrg> _orgRep;
    private readonly UserManager _userManager;

    public HrCompanySiteResolveService(
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<SysOrg> orgRep,
        UserManager userManager)
    {
        _ccRep = ccRep;
        _orgRep = orgRep;
        _userManager = userManager;
    }

    /// <summary>按部门编码解析归属厂别；找不到则抛错。</summary>
    public async Task<string> ResolveSiteCodeByDeptAsync(string ccCode)
    {
        var site = await TryResolveSiteCodeByDeptAsync(ccCode);
        if (string.IsNullOrWhiteSpace(site))
            throw Oops.Oh("未配置归属公司");
        return site;
    }

    /// <summary>按部门编码解析归属厂别；找不到返回 null。</summary>
    public async Task<string?> TryResolveSiteCodeByDeptAsync(string? ccCode)
    {
        if (string.IsNullOrWhiteSpace(ccCode)) return null;

        var code = ccCode.Trim();
        var org = await _ccRep.GetFirstAsync(u => u.CcCode == code);
        if (org == null) return null;

        if (!string.IsNullOrWhiteSpace(org.BelongSiteCode))
            return org.BelongSiteCode.Trim();

        // 向上：优先 cc_level 前缀祖先（内存匹配，避免 SQL 对 StartsWith 方向翻译问题）
        if (!string.IsNullOrWhiteSpace(org.CcLevel))
        {
            var level = org.CcLevel.Trim();
            var withBelong = await _ccRep.AsQueryable()
                .Where(u => u.BelongSiteCode != null && u.BelongSiteCode != "")
                .ToListAsync();
            var hit = withBelong
                .Where(u => !string.IsNullOrWhiteSpace(u.CcLevel)
                            && level.StartsWith(u.CcLevel.Trim(), StringComparison.Ordinal)
                            && !string.Equals(u.CcLevel.Trim(), level, StringComparison.Ordinal))
                .OrderByDescending(u => u.CcLevel!.Trim().Length)
                .FirstOrDefault(u => u.OrgType == HrCompanyParamConsts.OrgTypeCompany)
                ?? withBelong
                .Where(u => !string.IsNullOrWhiteSpace(u.CcLevel)
                            && level.StartsWith(u.CcLevel.Trim(), StringComparison.Ordinal)
                            && !string.Equals(u.CcLevel.Trim(), level, StringComparison.Ordinal))
                .OrderByDescending(u => u.CcLevel!.Trim().Length)
                .FirstOrDefault();
            if (hit != null)
                return hit.BelongSiteCode!.Trim();
        }

        var guard = 0;
        var parentCode = org.CcCodeParent;
        var parentId = org.ParentId;
        while (guard++ < 64)
        {
            CcMstr? parent = null;
            if (!string.IsNullOrWhiteSpace(parentCode))
                parent = await _ccRep.GetFirstAsync(u => u.CcCode == parentCode);
            else if (parentId is > 0)
                parent = await _ccRep.GetFirstAsync(u => u.CcIdentityId == parentId.Value);
            if (parent == null) break;

            if (!string.IsNullOrWhiteSpace(parent.BelongSiteCode)
                && parent.OrgType == HrCompanyParamConsts.OrgTypeCompany)
                return parent.BelongSiteCode.Trim();
            if (!string.IsNullOrWhiteSpace(parent.BelongSiteCode))
                return parent.BelongSiteCode.Trim();

            parentCode = parent.CcCodeParent;
            parentId = parent.ParentId;
        }

        return null;
    }

    /// <summary>
    /// 多个管辖部门均能解析且 SiteCode 一致时返回该厂别；否则 null。
    /// </summary>
    public async Task<string?> TryResolveSiteCodeByManagedDeptsAsync(IEnumerable<string>? managedCcCodes)
    {
        var codes = (managedCcCodes ?? [])
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (codes.Count == 0) return null;

        string? site = null;
        foreach (var cc in codes)
        {
            var s = await TryResolveSiteCodeByDeptAsync(cc);
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (site == null) site = s;
            else if (!string.Equals(site, s, StringComparison.OrdinalIgnoreCase))
                return null;
        }
        return site;
    }

    /// <summary>
    /// 无选手动部门时：用当前用户 SysOrg.Code（与 cc_mstr.cc_code 对齐时）做管辖同属兜底。
    /// </summary>
    public async Task<string?> TryResolveDefaultSiteCodeForCurrentUserAsync()
    {
        var orgIds = new HashSet<long>();
        if (_userManager.OrgId > 0) orgIds.Add(_userManager.OrgId);
        foreach (var id in _userManager.OrgIds ?? [])
            if (id > 0) orgIds.Add(id);
        if (orgIds.Count == 0) return null;

        var codes = await _orgRep.AsQueryable()
            .Where(u => orgIds.Contains(u.Id) && u.Code != null && u.Code != "")
            .Select(u => u.Code!)
            .ToListAsync();
        if (codes.Count == 0) return null;

        // 仅保留在人事组织中存在的编码
        var hrCodes = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .Select(u => u.CcCode)
            .ToListAsync();
        return await TryResolveSiteCodeByManagedDeptsAsync(hrCodes);
    }

    /// <summary>
    /// 优先部门；否则管辖同属兜底。均失败则抛错。
    /// </summary>
    public async Task<string> ResolveSiteCodeAsync(string? ccCode)
    {
        if (!string.IsNullOrWhiteSpace(ccCode))
            return await ResolveSiteCodeByDeptAsync(ccCode);

        var fallback = await TryResolveDefaultSiteCodeForCurrentUserAsync();
        if (!string.IsNullOrWhiteSpace(fallback))
            return fallback!;

        throw Oops.Oh("无法确定公司参数，请先选择部门");
    }
}
