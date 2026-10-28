namespace Admin.NET.Application;

/// <summary>角色树保存前校验助手（纯逻辑）。</summary>
public static class HrWfRoleTreeValidation
{
    /// <summary>
    /// 收集「自身等级（可空）+ 祖先有值等级」序列（近→远）。虚拟节点 LevelId 空则不计入。
    /// </summary>
    public static List<long> CollectPathLevelIds(
        long nodeId,
        long? parentId,
        long? selfLevelId,
        IReadOnlyDictionary<long, HrWfRoleNode> byId)
    {
        var levels = new List<long>();
        if (selfLevelId is > 0)
            levels.Add(selfLevelId.Value);

        var visited = new HashSet<long> { nodeId };
        var curId = parentId;
        while (curId is > 0)
        {
            if (!visited.Add(curId.Value))
                throw new InvalidOperationException("Wf.RoleTreeCycle");
            if (!byId.TryGetValue(curId.Value, out var cur))
                break;
            if (cur.LevelId is > 0)
                levels.Add(cur.LevelId.Value);
            curId = cur.ParentId;
        }
        return levels;
    }

    /// <summary>校验防环 + 路径等级唯一；失败抛 InvalidOperationException（消息为 Wf.* 键）。</summary>
    public static void AssertSaveValid(
        long nodeId,
        long? newParentId,
        long? newLevelId,
        IReadOnlyDictionary<long, HrWfRoleNode> byId)
    {
        if (HrWfReportPath.WouldCreateCycle(nodeId, newParentId, byId))
            throw new InvalidOperationException("Wf.RoleTreeCycle");

        var levels = CollectPathLevelIds(nodeId, newParentId, newLevelId, byId);
        HrWfReportPath.AssertUniqueLevelsOnPath(levels);
    }
}
