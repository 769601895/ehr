using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfScopeLogicTests
{
    [Fact]
    public void IncludeChild_PrefixMatch_CoversDescendant()
    {
        Assert.True(HrWfAssigneeResolver.ScopeCovers("D010101", "010101", "D01", "01", true));
    }

    [Fact]
    public void IncludeChild_SameLevel_CoversSelf()
    {
        Assert.True(HrWfAssigneeResolver.ScopeCovers("D01", "01", "D01", "01", true));
    }

    [Fact]
    public void IncludeChild_SiblingBranch_DoesNotCover()
    {
        Assert.False(HrWfAssigneeResolver.ScopeCovers("D02", "02", "D01", "01", true));
    }

    [Fact]
    public void IncludeChild_ParentOfRoot_DoesNotCover()
    {
        Assert.False(HrWfAssigneeResolver.ScopeCovers("D01", "01", "D0101", "0101", true));
    }

    [Fact]
    public void ExactOnly_SameCode_Covers()
    {
        Assert.True(HrWfAssigneeResolver.ScopeCovers("A01", "0101", "A01", "0101", false));
    }

    [Fact]
    public void ExactOnly_ChildCode_DoesNotCover()
    {
        Assert.False(HrWfAssigneeResolver.ScopeCovers("A0101", "010101", "A01", "0101", false));
    }

    [Theory]
    [InlineData("a01", "A01")]
    [InlineData("A01", "a01")]
    public void ExactOnly_CcCode_IsCaseInsensitive(string subject, string root)
    {
        Assert.True(HrWfAssigneeResolver.ScopeCovers(subject, "01", root, "01", false));
    }

    [Fact]
    public void CrossPlant_SameOrderDifferentPrefix_Isolated()
    {
        Assert.False(HrWfAssigneeResolver.ScopeCovers("B_PLANT_01", "0201", "A_PLANT", "0101", true));
    }
}
