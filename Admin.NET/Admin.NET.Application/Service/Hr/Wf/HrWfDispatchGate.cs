namespace Admin.NET.Application;

/// <summary>派单收尾门闩：有候选节点但全部跳过 → 失败；无剩余节点 → 通过。</summary>
public static class HrWfDispatchGate
{
    public static bool IsAllSkippedFailure(bool hadCandidate) => hadCandidate;
}
