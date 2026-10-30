namespace Admin.NET.Application;

/// <summary>
/// Level 审批人解析：等级 → 管辖角色 → 岗位 → 员工 → SysUser.JobNum
/// </summary>
public class HrWfAssigneeResolver : ITransient
{
    private readonly SqlSugarRepository<HrWfLevel> _levelRep;
    private readonly SqlSugarRepository<HrWfRole> _roleRep;
    private readonly SqlSugarRepository<HrWfRoleScope> _scopeRep;
    private readonly SqlSugarRepository<HrWfRolePost> _postRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<SysUser> _userRep;

    public HrWfAssigneeResolver(
        SqlSugarRepository<HrWfLevel> levelRep,
        SqlSugarRepository<HrWfRole> roleRep,
        SqlSugarRepository<HrWfRoleScope> scopeRep,
        SqlSugarRepository<HrWfRolePost> postRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<SysUser> userRep)
    {
        _levelRep = levelRep;
        _roleRep = roleRep;
        _scopeRep = scopeRep;
        _postRep = postRep;
        _empRep = empRep;
        _ccRep = ccRep;
        _userRep = userRep;
    }

    /// <summary>
    /// 解析审批人用户 Id（去重）。空人且 escalateIfEmpty 时按 AuditOrder 升下一非职能等级。
    /// </summary>
    public async Task<List<long>> ResolveByLevelAsync(long levelId, string subjectCcCode, bool escalateIfEmpty = true)
    {
        if (string.IsNullOrWhiteSpace(subjectCcCode))
            return [];

        var subject = await _ccRep.GetFirstAsync(u => u.CcCode == subjectCcCode);
        if (subject == null || string.IsNullOrWhiteSpace(subject.CcLevel))
            return [];

        var visited = new HashSet<long>();
        var currentLevelId = levelId;

        while (true)
        {
            if (!visited.Add(currentLevelId))
                return [];

            var level = await _levelRep.GetFirstAsync(u => u.Id == currentLevelId && u.Enabled);
            if (level == null) return [];

            var users = await ResolveUsersForLevelAsync(currentLevelId, subject.CcCode, subject.CcLevel!);
            if (users.Count > 0)
                return users;

            if (!escalateIfEmpty || level.IsFunctional)
                return [];

            var next = await _levelRep.AsQueryable()
                .Where(u => u.Enabled && !u.IsFunctional && u.AuditOrder > level.AuditOrder)
                .OrderBy(u => u.AuditOrder)
                .Take(1)
                .ToListAsync();
            if (next.Count == 0) return [];
            currentLevelId = next[0].Id;
        }
    }

    /// <summary>仅当前等级，不升级（供升级日志对照）</summary>
    public async Task<List<long>> ResolveUsersForLevelOnlyAsync(long levelId, string subjectCcCode)
    {
        if (string.IsNullOrWhiteSpace(subjectCcCode)) return [];
        var subject = await _ccRep.GetFirstAsync(u => u.CcCode == subjectCcCode);
        if (subject == null || string.IsNullOrWhiteSpace(subject.CcLevel)) return [];
        return await ResolveUsersForLevelAsync(levelId, subject.CcCode, subject.CcLevel!);
    }

    /// <summary>仅指定角色 + 管辖校验；不升级。管辖不覆盖则空列表。</summary>
    public async Task<List<long>> ResolveUsersForRoleAsync(long roleId, string subjectCcCode)
    {
        if (string.IsNullOrWhiteSpace(subjectCcCode)) return [];
        var subject = await _ccRep.GetFirstAsync(u => u.CcCode == subjectCcCode);
        if (subject == null || string.IsNullOrWhiteSpace(subject.CcLevel)) return [];

        var role = await _roleRep.GetFirstAsync(u => u.Id == roleId && u.Enabled);
        if (role == null) return [];

        return await ResolveUsersForRolesAsync([roleId], subject.CcCode, subject.CcLevel!);
    }

    /// <summary>按人事岗位编码找在岗员工→用户；不按管辖、不要求在汇报路径上。</summary>
    public async Task<List<long>> ResolveUsersForPostAsync(string postId)
    {
        if (string.IsNullOrWhiteSpace(postId)) return [];
        var pid = postId.Trim();
        var empIds = await _empRep.AsQueryable()
            .Where(u => u.PostId == pid && (u.EmpState == null || u.EmpState != 3))
            .Select(u => u.EmpId)
            .Distinct()
            .ToListAsync();
        if (empIds.Count == 0) return [];

        return await _userRep.AsQueryable()
            .Where(u => u.JobNum != null && empIds.Contains(u.JobNum) && u.Status == StatusEnum.Enable)
            .Select(u => u.Id)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>取在职/停职员工所属部门内码；离职或不存在返回 null。</summary>
    public async Task<string?> GetEmployeeCcCodeAsync(string empId)
    {
        if (string.IsNullOrWhiteSpace(empId)) return null;
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId.Trim() && (u.EmpState == null || u.EmpState != 3));
        return string.IsNullOrWhiteSpace(emp?.CcCode) ? null : emp!.CcCode!.Trim();
    }

    /// <summary>按员工工号取岗位；无岗位返回 null。</summary>
    public async Task<string?> GetEmployeePostIdAsync(string empId)
    {
        if (string.IsNullOrWhiteSpace(empId)) return null;
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId.Trim() && (u.EmpState == null || u.EmpState != 3));
        return string.IsNullOrWhiteSpace(emp?.PostId) ? null : emp!.PostId!.Trim();
    }

    /// <summary>启用中且绑定了该岗位的角色列表。</summary>
    public async Task<List<HrWfRole>> ListRolesByPostAsync(string postId)
    {
        if (string.IsNullOrWhiteSpace(postId)) return [];
        var pid = postId.Trim();
        var roleIds = await _postRep.AsQueryable()
            .Where(u => u.PostId == pid)
            .Select(u => u.RoleId)
            .Distinct()
            .ToListAsync();
        if (roleIds.Count == 0) return [];
        // 虚拟组织节点（无 LevelId）不参与起审命中
        return await _roleRep.AsQueryable()
            .Where(u => roleIds.Contains(u.Id) && u.Enabled && u.LevelId != null)
            .ToListAsync();
    }

    private async Task<List<long>> ResolveUsersForLevelAsync(long levelId, string subjectCcCode, string subjectCcLevel)
    {
        var roles = await _roleRep.AsQueryable()
            .Where(u => u.LevelId == levelId && u.Enabled)
            .ToListAsync();
        if (roles.Count == 0) return [];
        return await ResolveUsersForRolesAsync(roles.Select(u => u.Id).ToList(), subjectCcCode, subjectCcLevel);
    }

    private async Task<List<long>> ResolveUsersForRolesAsync(List<long> roleIds, string subjectCcCode, string subjectCcLevel)
    {
        if (roleIds.Count == 0) return [];

        var scopes = await _scopeRep.AsQueryable()
            .Where(u => roleIds.Contains(u.RoleId))
            .ToListAsync();
        if (scopes.Count == 0) return [];

        var scopeCcCodes = scopes.Select(u => u.CcCode).Distinct().ToList();
        var scopeDepts = await _ccRep.AsQueryable()
            .Where(u => scopeCcCodes.Contains(u.CcCode))
            .ToListAsync();
        var deptMap = scopeDepts.ToDictionary(u => u.CcCode, u => u, StringComparer.OrdinalIgnoreCase);

        var matchedRoleIds = new HashSet<long>();
        foreach (var group in scopes.GroupBy(u => u.RoleId))
        {
            foreach (var scope in group)
            {
                if (!deptMap.TryGetValue(scope.CcCode, out var root) || string.IsNullOrWhiteSpace(root.CcLevel))
                    continue;
                if (ScopeCovers(subjectCcCode, subjectCcLevel, root.CcCode, root.CcLevel!, scope.IncludeChild))
                {
                    matchedRoleIds.Add(group.Key);
                    break;
                }
            }
        }
        if (matchedRoleIds.Count == 0) return [];

        var postIds = await _postRep.AsQueryable()
            .Where(u => matchedRoleIds.Contains(u.RoleId))
            .Select(u => u.PostId)
            .Distinct()
            .ToListAsync();
        if (postIds.Count == 0) return [];

        var empIds = await _empRep.AsQueryable()
            .Where(u => u.PostId != null && postIds.Contains(u.PostId) && (u.EmpState == null || u.EmpState != 3))
            .Select(u => u.EmpId)
            .Distinct()
            .ToListAsync();
        if (empIds.Count == 0) return [];

        return await _userRep.AsQueryable()
            .Where(u => u.JobNum != null && empIds.Contains(u.JobNum) && u.Status == StatusEnum.Enable)
            .Select(u => u.Id)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>管辖是否覆盖单据部门（Usercc 语义）</summary>
    public static bool ScopeCovers(string subjectCcCode, string subjectCcLevel, string rootCcCode, string rootCcLevel, bool includeChild)
    {
        if (!includeChild)
            return string.Equals(subjectCcCode, rootCcCode, StringComparison.OrdinalIgnoreCase);

        return subjectCcLevel.StartsWith(rootCcLevel, StringComparison.Ordinal);
    }
}
