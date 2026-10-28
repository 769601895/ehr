using System.Text.Json;
using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfJsonContractTests
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void AssigneeRule_Level_RoundTrip()
    {
        var rule = JsonSerializer.Deserialize<HrWfAssigneeRule>("""{"type":"Level","levelId":1300000004001}""", Opts);
        Assert.NotNull(rule);
        Assert.Equal("Level", rule!.Type);
        Assert.Equal(1300000004001, rule.LevelId);
    }

    [Fact]
    public void AssigneeRule_FixedUser_RoundTrip()
    {
        var rule = JsonSerializer.Deserialize<HrWfAssigneeRule>("""{"type":"FixedUser","userIds":[1,2,3]}""", Opts);
        Assert.NotNull(rule);
        Assert.Equal("FixedUser", rule!.Type);
        Assert.Equal(new long[] { 1, 2, 3 }, rule.UserIds);
    }

    [Fact]
    public void AssigneeRule_Post_RoundTrip()
    {
        var rule = JsonSerializer.Deserialize<HrWfAssigneeRule>("""{"type":"Post","postId":"P_FIN_CLERK"}""", Opts);
        Assert.NotNull(rule);
        Assert.Equal(HrWfConsts.RulePost, rule!.Type);
        Assert.Equal("P_FIN_CLERK", rule.PostId);
    }

    [Fact]
    public void NodesSnapshot_ReportPathMode_RoundTrip()
    {
        var snap = new HrWfNodesSnapshot
        {
            DefId = 1,
            Version = 1,
            AssigneeResolveMode = HrWfConsts.ModeReportPath,
            ReportPathRoleIds = [3, 2, 1],
            Nodes = []
        };
        var back = JsonSerializer.Deserialize<HrWfNodesSnapshot>(JsonSerializer.Serialize(snap, Opts), Opts);
        Assert.Equal(HrWfConsts.ModeReportPath, back!.AssigneeResolveMode);
        Assert.Equal(new long[] { 3, 2, 1 }, back.ReportPathRoleIds);
    }

    [Fact]
    public void FormSchemaSnapshot_ContainsFields()
    {
        var snap = new HrWfSchemaSnapshot
        {
            FormId = 1,
            BizType = HrWfConsts.BizTypeGeneral,
            FormMode = HrWfConsts.FormModeB,
            Version = 1,
            Fields =
            [
                new HrWfFormFieldItem
                {
                    FieldKey = "subject",
                    Label = "申请主题",
                    ControlType = "text",
                    Required = true,
                    SortNo = 10,
                    SpanSlots = 24
                }
            ]
        };
        var back = JsonSerializer.Deserialize<HrWfSchemaSnapshot>(JsonSerializer.Serialize(snap, Opts), Opts);
        Assert.NotNull(back);
        Assert.Equal(HrWfConsts.BizTypeGeneral, back!.BizType);
        Assert.Single(back.Fields);
        Assert.Equal("subject", back.Fields[0].FieldKey);
        Assert.True(back.Fields[0].Required);
    }

    [Fact]
    public void NodesSnapshot_PreservesEditSwitches()
    {
        var snap = new HrWfNodesSnapshot
        {
            DefId = 10,
            Version = 2,
            AllowEditWhileRunning = false,
            Nodes =
            [
                new HrWfDefNodeItem
                {
                    NodeKey = "n1",
                    Name = "部门主管",
                    OrderNo = 10,
                    NodeMode = HrWfConsts.NodeOr,
                    AssigneeRuleJson = """{"type":"Level","levelId":1}""",
                    Enabled = true
                }
            ]
        };
        var back = JsonSerializer.Deserialize<HrWfNodesSnapshot>(JsonSerializer.Serialize(snap, Opts), Opts);
        Assert.NotNull(back);
        Assert.False(back!.AllowEditWhileRunning);
        Assert.Equal("n1", back.Nodes[0].NodeKey);
        var rule = JsonSerializer.Deserialize<HrWfAssigneeRule>(back.Nodes[0].AssigneeRuleJson, Opts);
        Assert.Equal(HrWfConsts.RuleLevel, rule!.Type);
    }

    [Fact]
    public void Consts_P0StatusStrings_Stable()
    {
        Assert.Equal("Running", HrWfConsts.InstRunning);
        Assert.Equal("Pending", HrWfConsts.TaskPending);
        Assert.Equal("Approved", HrWfConsts.InstApproved);
        Assert.Equal("Rejected", HrWfConsts.InstRejected);
        Assert.Equal("hrWfGeneral", HrWfConsts.BizTypeGeneral);
        Assert.Equal("B", HrWfConsts.FormModeB);
    }
}
