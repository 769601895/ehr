namespace Admin.NET.Application;

/// <summary>汇报路径纯逻辑（无 IO）：起点角色消歧、祖先链、路径等级唯一。</summary>
public static class HrWfReportPath
{
    /// <summary>多岗位命中时取树深最大；同深抛 <c>Wf.AmbiguousStartRole</c>。</summary>
    public static HrWfRoleNode SelectStartRole(IReadOnlyList<HrWfRoleNode> matchedByPost)
    {
        if (matchedByPost == null || matchedByPost.Count == 0)
            throw new InvalidOperationException("Wf.AmbiguousStartRole");

        var maxDepth = matchedByPost.Max(u => u.Depth);
        var deepest = matchedByPost.Where(u => u.Depth == maxDepth).ToList();
        if (deepest.Count != 1)
            throw new InvalidOperationException("Wf.AmbiguousStartRole");
        return deepest[0];
    }

    /// <summary>
    /// 从 start 的父节点走到根（不含 start）。无父返回空列表；遇环抛 <c>Wf.RoleTreeCycle</c>。
    /// </summary>
    public static List<long> BuildAncestorPath(long startRoleId, IReadOnlyDictionary<long, HrWfRoleNode> byId)
    {
        if (byId == null || !byId.TryGetValue(startRoleId, out var start))
            return [];

        var path = new List<long>();
        var visited = new HashSet<long> { startRoleId };
        var curId = start.ParentId;
        while (curId is > 0)
        {
            if (!visited.Add(curId.Value))
                throw new InvalidOperationException("Wf.RoleTreeCycle");
            if (!byId.TryGetValue(curId.Value, out var cur))
                break;
            path.Add(cur.Id);
            curId = cur.ParentId;
        }
        return path;
    }

    /// <summary>路径上 LevelId 不可重复；重复抛 <c>Wf.DuplicateLevelOnPath</c>。</summary>
    public static void AssertUniqueLevelsOnPath(IEnumerable<long> levelIds)
    {
        var seen = new HashSet<long>();
        foreach (var levelId in levelIds)
        {
            if (!seen.Add(levelId))
                throw new InvalidOperationException("Wf.DuplicateLevelOnPath");
        }
    }

    /// <summary>判断将 nodeId 挂到 newParentId 下是否成环（newParent 是 node 自身或其子孙）。</summary>
    public static bool WouldCreateCycle(long nodeId, long? newParentId, IReadOnlyDictionary<long, HrWfRoleNode> byId)
    {
        if (newParentId is not > 0) return false;
        if (newParentId == nodeId) return true;

        var visited = new HashSet<long>();
        var curId = newParentId;
        while (curId is > 0)
        {
            if (curId == nodeId) return true;
            if (!visited.Add(curId.Value)) return true; // 已有脏环
            if (!byId.TryGetValue(curId.Value, out var cur)) break;
            curId = cur.ParentId;
        }
        return false;
    }

    /// <summary>在路径角色中按 LevelId 找唯一角色 Id；找不到返回 null。</summary>
    public static long? FindRoleIdByLevel(IEnumerable<long> pathRoleIds, IReadOnlyDictionary<long, HrWfRoleNode> byId, long levelId)
    {
        foreach (var roleId in pathRoleIds)
        {
            if (byId.TryGetValue(roleId, out var node) && node.LevelId is > 0 && node.LevelId == levelId)
                return roleId;
        }
        return null;
    }

    /// <summary>计算节点深度（根=0）；遇环抛 <c>Wf.RoleTreeCycle</c>。</summary>
    public static int ComputeDepth(long roleId, IReadOnlyDictionary<long, HrWfRoleNode> byId)
    {
        var depth = 0;
        var visited = new HashSet<long>();
        var curId = (long?)roleId;
        while (curId is > 0)
        {
            if (!visited.Add(curId.Value))
                throw new InvalidOperationException("Wf.RoleTreeCycle");
            if (!byId.TryGetValue(curId.Value, out var cur) || cur.ParentId is not > 0)
                break;
            depth++;
            curId = cur.ParentId;
        }
        return depth;
    }
}

/// <summary>汇报树节点轻量视图（Depth 由调用方预计算或 ComputeDepth；LevelId 空=组织虚拟节点）。</summary>
public record HrWfRoleNode(long Id, long? ParentId, long? LevelId, int Depth);
