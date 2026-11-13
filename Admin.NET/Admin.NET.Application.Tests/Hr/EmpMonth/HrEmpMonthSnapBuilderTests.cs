using Admin.NET.Application;
using Xunit;

namespace Admin.NET.Application.Tests.Hr.EmpMonth;

/// <summary>快照装配纯逻辑夹具（不依赖 DI）</summary>
public class HrEmpMonthSnapBuilderTests
{
    [Fact]
    public void CoreFieldKeys_AreExcluded_FromEmpFieldSnapshotShape()
    {
        // 契约：固有 Key 不得出现在 EmpFieldSnapshot.fields（由 BuildEmpFieldSnapshot 过滤 InCoreColumns）
        Assert.Contains("ccCode", HrEmpMonthConst.CoreFieldKeys);
        Assert.Contains("empId", HrEmpMonthConst.CoreFieldKeys);
        Assert.DoesNotContain("empType", HrEmpMonthConst.CoreFieldKeys);
    }

    [Fact]
    public void FieldSchemaSnapshot_ContainsKeys()
    {
        var json = HrEmpMonthSnapBuilder.BuildFieldSchemaSnapshot(
            ["ccCode", "empType"],
            ["ccCode", "postId"]);
        Assert.Contains("monthSnapFieldKeys", json);
        Assert.Contains("empType", json);
        Assert.Contains("chgEnabledFieldKeys", json);
        Assert.Contains("postId", json);
    }
}
