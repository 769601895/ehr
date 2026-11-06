using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 人事用户管辖范围（Usercc）：解析、过滤、从 ExtOrg 同步
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrUserCcScope", Order = 470, Description = "人事用户管辖")]
public class HrUserCcScopeService : IDynamicApiController, ITransient
{
    public const string CacheKeyPrefix = "hr:usercc:";

    private readonly UserManager _userManager;
    private readonly SysCacheService _sysCacheService;
    private readonly SqlSugarRepository<Usercc> _userCcRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SqlSugarRepository<SysUserExtOrg> _extOrgRep;

    public HrUserCcScopeService(
        UserManager userManager,
        SysCacheService sysCacheService,
        SqlSugarRepository<Usercc> userCcRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<SysUser> sysUserRep,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SqlSugarRepository<SysUserExtOrg> extOrgRep)
    {
        _userManager = userManager;
        _sysCacheService = sysCacheService;
        _userCcRep = userCcRep;
        _ccRep = ccRep;
        _sysUserRep = sysUserRep;
        _sysOrgRep = sysOrgRep;
        _extOrgRep = extOrgRep;
    }

    /// <summary>当前用户是否人事全量范围（超管）</summary>
    [NonAction]
    public bool IsAllScope() => _userManager.SuperAdmin;

    /// <summary>当前用户是否人事全量范围</summary>
    [NonAction]
    public bool IsAllScope(long userId) =>
        userId == _userManager.UserId ? _userManager.SuperAdmin : false;

    /// <summary>读取授权行（可缓存）；不含 level</summary>
    [NonAction]
    public async Task<List<Usercc>> GetAuthRowsAsync(long userId)
    {
        var key = CacheKeyPrefix + userId;
        var cached = _sysCacheService.Get<List<Usercc>>(key);
        if (cached != null) return cached;

        var rows = await _userCcRep.AsQueryable()
            .Where(u => u.UserId == userId)
            .ToListAsync();
        _sysCacheService.Set(key, rows, TimeSpan.FromHours(8));
        return rows;
    }

    /// <summary>有效管辖根（JOIN 当前 cc_mstr；根不存在则跳过）</summary>
    [NonAction]
    public async Task<List<HrUserCcEffectiveRoot>> GetEffectiveRootsAsync(long userId)
    {
        if (IsAllScope() && userId == _userManager.UserId)
            return [];

        var rows = await GetAuthRowsAsync(userId);
        if (rows.Count == 0) return [];

        var codes = rows.Select(u => u.CcCode).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var depts = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .Select(u => new { u.CcCode, u.CcLevel, u.CcEnable })
            .ToListAsync();
        var map = depts.ToDictionary(u => u.CcCode, u => u, StringComparer.OrdinalIgnoreCase);

        var list = new List<HrUserCcEffectiveRoot>();
        foreach (var row in rows)
        {
            if (!map.TryGetValue(row.CcCode, out var d)) continue;
            if (string.IsNullOrWhiteSpace(d.CcLevel)) continue;
            // 停用根默认不生效
            if (d.CcEnable == 1) continue;
            list.Add(new HrUserCcEffectiveRoot
            {
                CcCode = d.CcCode,
                CcLevel = d.CcLevel,
                IncludeChild = row.IncludeChild
            });
        }
        return list;
    }

    /// <summary>当前登录用户有效根</summary>
    [NonAction]
    public Task<List<HrUserCcEffectiveRoot>> GetCurrentEffectiveRootsAsync()
        => GetEffectiveRootsAsync(_userManager.UserId);

    /// <summary>部门查询套管辖（非全量且无根 → 恒假）</summary>
    [NonAction]
    public async Task<ISugarQueryable<CcMstr>> ApplyOrgScopeAsync(ISugarQueryable<CcMstr> query, long? userId = null)
    {
        var uid = userId ?? _userManager.UserId;
        if (uid == _userManager.UserId && IsAllScope()) return query;

        var roots = await GetEffectiveRootsAsync(uid);
        if (roots.Count == 0)
            return query.Where(u => false);

        return ApplyLevelFilter(query, roots);
    }

    /// <summary>员工查询套管辖</summary>
    [NonAction]
    public async Task<ISugarQueryable<HrEmployee>> ApplyEmpScopeAsync(ISugarQueryable<HrEmployee> query, long? userId = null)
    {
        var uid = userId ?? _userManager.UserId;
        if (uid == _userManager.UserId && IsAllScope()) return query;

        var roots = await GetEffectiveRootsAsync(uid);
        if (roots.Count == 0)
            return query.Where(u => false);

        var ccQuery = ApplyLevelFilter(_ccRep.AsQueryable(), roots);
        var codes = await ccQuery.Select(u => u.CcCode).ToListAsync();
        if (codes.Count == 0)
            return query.Where(u => false);

        return query.Where(u => u.CcCode != null && codes.Contains(u.CcCode));
    }

    /// <summary>校验部门在管辖内（写操作）；全量跳过；空码跳过</summary>
    [NonAction]
    public async Task EnsureCcInScopeAsync(string? ccCode, long? userId = null)
    {
        if (string.IsNullOrWhiteSpace(ccCode)) return;
        var uid = userId ?? _userManager.UserId;
        if (uid == _userManager.UserId && IsAllScope()) return;

        var roots = await GetEffectiveRootsAsync(uid);
        if (roots.Count == 0)
            throw Oops.Oh("未配置人事管辖部门，无法操作该部门数据");

        var cc = await _ccRep.GetFirstAsync(u => u.CcCode == ccCode.Trim())
            ?? throw Oops.Oh("所属部门不存在");
        if (!IsLevelInRoots(cc.CcLevel, roots))
            throw Oops.Oh("无权操作该部门数据");
    }

    /// <summary>
    /// 树节点导航：管辖内部门，或其祖先（共同父级）可点选；列表仍会与管辖求交。
    /// </summary>
    [NonAction]
    public async Task EnsureCcNavigableAsync(string? ccCode, long? userId = null)
    {
        if (string.IsNullOrWhiteSpace(ccCode)) return;
        var uid = userId ?? _userManager.UserId;
        if (uid == _userManager.UserId && IsAllScope()) return;

        var roots = await GetEffectiveRootsAsync(uid);
        if (roots.Count == 0)
            throw Oops.Oh("未配置人事管辖部门，无法查看该部门数据");

        var cc = await _ccRep.GetFirstAsync(u => u.CcCode == ccCode.Trim())
            ?? throw Oops.Oh("部门不存在");
        var level = cc.CcLevel ?? "";
        if (IsLevelInRoots(level, roots) || IsAncestorOfRoots(level, roots))
            return;
        throw Oops.Oh("无权查看该部门数据");
    }

    /// <summary>是否有有效管辖（非全量用户）</summary>
    [NonAction]
    public async Task<bool> HasEffectiveScopeAsync(long? userId = null)
    {
        var uid = userId ?? _userManager.UserId;
        if (uid == _userManager.UserId && IsAllScope()) return true;
        var roots = await GetEffectiveRootsAsync(uid);
        return roots.Count > 0;
    }

    /// <summary>从账号主机构 + ExtOrg 同步 Usercc（默认含下级）</summary>
    [NonAction]
    public async Task<HrUserCcSyncResult> SyncFromExtOrgAsync(long userId)
    {
        var user = await _sysUserRep.GetByIdAsync(userId);
        if (user == null)
            return new HrUserCcSyncResult { UserId = userId };

        var orgIds = new HashSet<long>();
        if (user.OrgId > 0) orgIds.Add(user.OrgId);
        var ext = await _extOrgRep.GetListAsync(u => u.UserId == userId);
        foreach (var e in ext)
            if (e.OrgId > 0) orgIds.Add(e.OrgId);

        var skipped = new List<string>();
        var ccCodes = new List<string>();
        if (orgIds.Count > 0)
        {
            var orgs = await _sysOrgRep.AsQueryable()
                .Where(u => orgIds.Contains(u.Id) && u.Code != null && u.Code != "")
                .Select(u => new { u.Id, u.Code })
                .ToListAsync();
            var codes = orgs.Select(u => u.Code!.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (codes.Count > 0)
            {
                var valid = await _ccRep.AsQueryable()
                    .Where(u => codes.Contains(u.CcCode))
                    .Select(u => u.CcCode)
                    .ToListAsync();
                var validSet = new HashSet<string>(valid, StringComparer.OrdinalIgnoreCase);
                foreach (var c in codes)
                {
                    if (validSet.Contains(c)) ccCodes.Add(c);
                    else skipped.Add(c);
                }
            }
        }

        ccCodes = ccCodes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        await ReplaceUserCcAsync(userId, ccCodes, includeChild: true);
        return new HrUserCcSyncResult
        {
            UserId = userId,
            SavedCount = ccCodes.Count,
            SkippedCodes = skipped
        };
    }

    /// <summary>全量用户从 ExtOrg 同步（管理员运维）</summary>
    [ApiDescriptionSettings(Name = "SyncAllFromExtOrg"), HttpPost]
    [DisplayName("同步全部用户人事管辖")]
    public async Task<HrUserCcSyncAllResult> SyncAllFromExtOrg()
    {
        if (!_userManager.SuperAdmin)
            throw Oops.Oh("仅超级管理员可执行全量同步");

        var userIds = await _sysUserRep.AsQueryable().Select(u => u.Id).ToListAsync();
        var result = new HrUserCcSyncAllResult();
        var rows = 0;
        foreach (var id in userIds)
        {
            var one = await SyncFromExtOrgAsync(id);
            rows += one.SavedCount;
            foreach (var s in one.SkippedCodes)
                result.Warnings.Add($"user={id} code={s}");
        }
        result.UserCount = userIds.Count;
        result.RowCount = rows;
        return result;
    }

    /// <summary>清除用户管辖缓存</summary>
    [NonAction]
    public void ClearCache(long userId) => _sysCacheService.Remove(CacheKeyPrefix + userId);

    /// <summary>删除用户全部 Usercc</summary>
    [NonAction]
    public async Task ClearUserAsync(long userId)
    {
        await _userCcRep.DeleteAsync(u => u.UserId == userId);
        ClearCache(userId);
    }

    private async Task ReplaceUserCcAsync(long userId, List<string> ccCodes, bool includeChild)
    {
        await _userCcRep.DeleteAsync(u => u.UserId == userId);
        if (ccCodes.Count > 0)
        {
            var now = DateTime.Now;
            var rows = ccCodes.Select(c => new Usercc
            {
                Id = YitIdHelper.NextId(),
                UserId = userId,
                CcCode = c,
                IncludeChild = includeChild,
                CreateTime = now,
                CreateUserId = _userManager.UserId > 0 ? _userManager.UserId : null
            }).ToList();
            await _userCcRep.InsertRangeAsync(rows);
        }
        ClearCache(userId);
    }

    private static ISugarQueryable<CcMstr> ApplyLevelFilter(ISugarQueryable<CcMstr> query, List<HrUserCcEffectiveRoot> roots)
    {
        var exp = Expressionable.Create<CcMstr>();
        foreach (var r in roots)
        {
            var level = r.CcLevel;
            if (r.IncludeChild)
                exp = exp.Or(u => u.CcLevel != null && u.CcLevel.StartsWith(level));
            else
                exp = exp.Or(u => u.CcLevel == level);
        }
        return query.Where(exp.ToExpression());
    }

    private static bool IsLevelInRoots(string? level, List<HrUserCcEffectiveRoot> roots)
    {
        if (string.IsNullOrWhiteSpace(level)) return false;
        foreach (var r in roots)
        {
            if (r.IncludeChild)
            {
                if (level.StartsWith(r.CcLevel, StringComparison.Ordinal)) return true;
            }
            else if (string.Equals(level, r.CcLevel, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    /// <summary>level 是否为某个管辖根的严格祖先（路径展示用）</summary>
    private static bool IsAncestorOfRoots(string? level, List<HrUserCcEffectiveRoot> roots)
    {
        if (string.IsNullOrWhiteSpace(level)) return false;
        foreach (var r in roots)
        {
            if (string.IsNullOrWhiteSpace(r.CcLevel)) continue;
            if (r.CcLevel.Length > level.Length
                && r.CcLevel.StartsWith(level, StringComparison.Ordinal))
                return true;
        }
        return false;
    }
}
