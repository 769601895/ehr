namespace Admin.NET.Application;

/// <summary>
/// 审批流程定义节点
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_def_node", "审批流程定义节点")]
public class HrWfDefNode : EntityBase
{
    [SugarColumn(ColumnName = "def_id", ColumnDescription = "流程定义Id")]
    public long DefId { get; set; }

    [SugarColumn(ColumnName = "node_key", ColumnDescription = "节点键", Length = 50)]
    public string NodeKey { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "节点名称", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "order_no", ColumnDescription = "顺序")]
    public int OrderNo { get; set; }

    /// <summary>Serial / And / Or</summary>
    [SugarColumn(ColumnName = "node_mode", ColumnDescription = "节点模式", Length = 20)]
    public string NodeMode { get; set; } = "Or";

    [SugarColumn(ColumnName = "assignee_rule_json", ColumnDescription = "审批人规则JSON", ColumnDataType = "nvarchar(max)")]
    public string AssigneeRuleJson { get; set; }

    [SugarColumn(ColumnName = "condition_json", ColumnDescription = "条件JSON", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ConditionJson { get; set; }

    [SugarColumn(ColumnName = "timeout_hours", ColumnDescription = "超时小时", IsNullable = true)]
    public int? TimeoutHours { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;
}
