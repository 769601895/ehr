namespace Admin.NET.Application;

/// <summary>
/// 审批任务
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_task", "审批任务")]
public class HrWfTask : EntityBase
{
    [SugarColumn(ColumnName = "inst_id", ColumnDescription = "实例Id")]
    public long InstId { get; set; }

    [SugarColumn(ColumnName = "node_key", ColumnDescription = "节点键", Length = 50)]
    public string NodeKey { get; set; }

    [SugarColumn(ColumnName = "assignee_id", ColumnDescription = "待办人")]
    public long AssigneeId { get; set; }

    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; }

    [SugarColumn(ColumnName = "opinion", ColumnDescription = "意见", Length = 500, IsNullable = true)]
    public string? Opinion { get; set; }

    [SugarColumn(ColumnName = "claimed_time", ColumnDescription = "签收时间", IsNullable = true)]
    public DateTime? ClaimedTime { get; set; }

    [SugarColumn(ColumnName = "finished_time", ColumnDescription = "完成时间", IsNullable = true)]
    public DateTime? FinishedTime { get; set; }

    [SugarColumn(ColumnName = "from_escalate", ColumnDescription = "是否升级产生")]
    public bool FromEscalate { get; set; }
}
