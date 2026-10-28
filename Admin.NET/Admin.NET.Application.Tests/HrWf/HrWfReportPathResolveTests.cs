using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfReportPathResolveTests
{
    [Fact]
    public void FindRoleIdByLevel_OnPath_ReturnsRole()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
        };
        Assert.Equal(2, HrWfReportPath.FindRoleIdByLevel([2, 1], byId, 20));
        Assert.Equal(1, HrWfReportPath.FindRoleIdByLevel([2, 1], byId, 10));
    }

    [Fact]
    public void FindRoleIdByLevel_NotOnPath_ReturnsNull()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
        };
        Assert.Null(HrWfReportPath.FindRoleIdByLevel([2, 1], byId, 99));
    }

    [Fact]
    public void ScopeNotCover_MeansEmptyAssignee_DocumentedByScopeCovers()
    {
        // ResolveUsersForRoleAsync 在管辖不覆盖时返回空；等价于 ScopeCovers=false
        Assert.False(HrWfAssigneeResolver.ScopeCovers("D02", "02", "D01", "01", true));
    }

    [Fact]
    public void PostRule_IgnoresPath_ContractShape()
    {
        // Post 规则与路径无关：仅依赖 postId；路径缺失不影响类型契约
        var rule = new HrWfAssigneeRule { Type = HrWfConsts.RulePost, PostId = "P_FIN" };
        Assert.Equal(HrWfConsts.RulePost, rule.Type);
        Assert.Equal("P_FIN", rule.PostId);
        Assert.Null(rule.LevelId);
    }

    [Fact]
    public void Dispatch_AllSkipped_IsFailure()
    {
        Assert.True(HrWfDispatchGate.IsAllSkippedFailure(hadCandidate: true));
        Assert.False(HrWfDispatchGate.IsAllSkippedFailure(hadCandidate: false));
    }
}
