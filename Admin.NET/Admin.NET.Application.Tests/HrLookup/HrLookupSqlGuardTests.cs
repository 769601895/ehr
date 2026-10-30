using Xunit;

namespace Admin.NET.Application.Tests.HrLookup;

public class HrLookupSqlGuardTests
{
    [Theory]
    [InlineData("post_code", true)]
    [InlineData("hr_post", true)]
    [InlineData("IsDelete", true)]
    [InlineData("a", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("post-code", false)]
    [InlineData("post code", false)]
    [InlineData("hr_post;drop", false)]
    [InlineData("1post", false)]
    [InlineData("dbo.hr_post", false)]
    public void IsSafeIdent(string? name, bool expected)
    {
        Assert.Equal(expected, HrLookupSqlGuard.IsSafeIdent(name));
    }
}
