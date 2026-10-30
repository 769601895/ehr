using Xunit;

namespace Admin.NET.Application.Tests.HrLookup;

public class HrLookupResolveTests
{
    [Fact]
    public void ResolveCodeDesc_PrefersTranslation()
    {
        var map = new Dictionary<string, string> { ["en"] = "HR Manager" };
        var text = HrUiI18nResolver.ResolveCodeDesc("000006", "人力资源部经理", "en", map);
        Assert.Equal("HR Manager", text);
    }

    [Fact]
    public void ResolveCodeDesc_FallsBackToMain()
    {
        var text = HrUiI18nResolver.ResolveCodeDesc("000006", "人力资源部经理", "en", null);
        Assert.Equal("人力资源部经理", text);
    }

    [Fact]
    public void ResolveCodeDesc_FallsBackToValue()
    {
        var text = HrUiI18nResolver.ResolveCodeDesc("000006", "", "en", null);
        Assert.Equal("000006", text);
    }
}
