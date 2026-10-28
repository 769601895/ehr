using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfConditionEvalTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_ReturnsTrue(string? json)
    {
        Assert.True(HrWfConditionEval.IsMatch(json, new Dictionary<string, object?>()));
    }

    [Fact]
    public void Amount_GreaterThan_Boundary()
    {
        var form = new Dictionary<string, object?> { ["amount"] = 1000 };
        Assert.False(HrWfConditionEval.IsMatch("""{"field":"amount","op":">","value":1000}""", form));
        Assert.True(HrWfConditionEval.IsMatch("""{"field":"amount","op":">=","value":1000}""", form));
        Assert.True(HrWfConditionEval.IsMatch("""{"field":"amount","op":">","value":999}""", form));
    }

    [Fact]
    public void UnknownOp_ReturnsFalse()
    {
        var form = new Dictionary<string, object?> { ["amount"] = 100 };
        Assert.False(HrWfConditionEval.IsMatch("""{"field":"amount","op":"~~","value":1}""", form));
    }

    [Fact]
    public void MissingField_ReturnsFalse()
    {
        Assert.False(HrWfConditionEval.IsMatch("""{"op":">","value":1}""", new Dictionary<string, object?>()));
    }

    [Fact]
    public void Equals_String()
    {
        var form = new Dictionary<string, object?> { ["status"] = "ok" };
        Assert.True(HrWfConditionEval.IsMatch("""{"field":"status","op":"=","value":"ok"}""", form));
        Assert.True(HrWfConditionEval.IsMatch("""{"field":"status","op":"!=","value":"bad"}""", form));
    }
}
