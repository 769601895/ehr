using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfRoleTreeValidationTests
{
    [Fact]
    public void WouldCreateCycle_ParentIsSelf_True()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
        };
        Assert.True(HrWfReportPath.WouldCreateCycle(1, 1, byId));
    }

    [Fact]
    public void WouldCreateCycle_ParentIsDescendant_True()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
            [3] = new(3, 2, 30, 2),
        };
        Assert.True(HrWfReportPath.WouldCreateCycle(1, 3, byId));
    }

    [Fact]
    public void WouldCreateCycle_ValidReparent_False()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
            [3] = new(3, null, 30, 0),
        };
        Assert.False(HrWfReportPath.WouldCreateCycle(2, 3, byId));
    }

    [Fact]
    public void PathLevelsUniqueAfterReparent_Duplicate_Throws()
    {
        var levelsOnNewPath = new[] { 20L, 10L, 20L };
        var ex = Assert.Throws<InvalidOperationException>(() =>
            HrWfReportPath.AssertUniqueLevelsOnPath(levelsOnNewPath));
        Assert.Equal("Wf.DuplicateLevelOnPath", ex.Message);
    }

    [Fact]
    public void PathLevelsUniqueAfterReparent_Ok()
    {
        HrWfReportPath.AssertUniqueLevelsOnPath([30L, 20L, 10L]);
    }

    [Fact]
    public void CollectPathLevelIds_IncludesSelfAndAncestors()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
            [3] = new(3, 2, 30, 2),
        };
        var levels = HrWfRoleTreeValidation.CollectPathLevelIds(3, parentId: 2, selfLevelId: 30, byId);
        Assert.Equal(new long[] { 30, 20, 10 }, levels);
    }

    [Fact]
    public void CollectPathLevelIds_UsesOverridesForSavePreview()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, 10, 0),
            [2] = new(2, 1, 20, 1),
            [3] = new(3, 2, 30, 2),
        };
        var levels = HrWfRoleTreeValidation.CollectPathLevelIds(3, parentId: 1, selfLevelId: 40, byId);
        Assert.Equal(new long[] { 40, 10 }, levels);
    }

    [Fact]
    public void CollectPathLevelIds_VirtualRoot_SkippedInLevels()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [1] = new(1, null, null, 0), // 审核流程虚拟根
            [2] = new(2, 1, 20, 1),
        };
        var levels = HrWfRoleTreeValidation.CollectPathLevelIds(2, parentId: 1, selfLevelId: 20, byId);
        Assert.Equal(new long[] { 20 }, levels);
    }

    [Fact]
    public void AssertSaveValid_VirtualRoot_Ok()
    {
        var byId = new Dictionary<long, HrWfRoleNode>
        {
            [-1] = new(-1, null, null, 0),
        };
        HrWfRoleTreeValidation.AssertSaveValid(-1, null, null, byId);
    }
}
