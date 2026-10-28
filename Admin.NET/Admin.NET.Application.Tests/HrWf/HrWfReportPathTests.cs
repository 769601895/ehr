using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfReportPathTests
{
    [Fact]
    public void SelectStartRole_PicksDeepest()
    {
        var matched = new List<HrWfRoleNode>
        {
            new(1, null, 10, 0),
            new(2, 1, 20, 1),
            new(3, 2, 30, 2),
        };

        var start = HrWfReportPath.SelectStartRole(matched);
        Assert.Equal(3, start.Id);
    }

    [Fact]
    public void SelectStartRole_SameDepth_Throws()
    {
        var matched = new List<HrWfRoleNode>
        {
            new(2, 1, 20, 1),
            new(4, 1, 21, 1),
        };

        var ex = Assert.Throws<InvalidOperationException>(() => HrWfReportPath.SelectStartRole(matched));
        Assert.Equal("Wf.AmbiguousStartRole", ex.Message);
    }

    [Fact]
    public void BuildAncestorPath_SkipsStart_IncludesRoot()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
            [3] = new(3, 2, 30, 2),
        };

        var path = HrWfReportPath.BuildAncestorPath(3, byId);
        Assert.Equal(new long[] { 2, 1 }, path);
    }

    [Fact]
    public void BuildAncestorPath_Cycle_Throws()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, 2, 10, 0),
            [2] = new(2, 1, 20, 1),
        };

        var ex = Assert.Throws<InvalidOperationException>(() => HrWfReportPath.BuildAncestorPath(1, byId));
        Assert.Equal("Wf.RoleTreeCycle", ex.Message);
    }

    [Fact]
    public void AssertUniqueLevels_Duplicate_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            HrWfReportPath.AssertUniqueLevelsOnPath([10, 20, 10]));
        Assert.Equal("Wf.DuplicateLevelOnPath", ex.Message);
    }
}
