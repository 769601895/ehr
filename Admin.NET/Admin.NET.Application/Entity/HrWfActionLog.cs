namespace Admin.NET.Application;

/// <summary>
/// 审批操作日志
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_action_log", "审批操作日志")]
public class HrWfActionLog : EntityBase
{
    [SugarColumn(ColumnName = "inst_id", ColumnDescription = "实例Id")]
    public long InstId { get; set; }

    [SugarColumn(ColumnName = "task_id", ColumnDescription = "任务Id", IsNullable = true)]
    public long? TaskId { get; set; }

    [SugarColumn(ColumnName = "action_type", ColumnDescription = "动作类型", Length = 30)]
    public string ActionType { get; set; }

    [SugarColumn(ColumnName = "actor_user_id", ColumnDescription = "操作人", IsNullable = true)]
    public long? ActorUserId { get; set; }

    [SugarColumn(ColumnName = "node_key", ColumnDescription = "节点键", Length = 50, IsNullable = true)]
    public string? NodeKey { get; set; }

    [SugarColumn(ColumnName = "from_level_id", ColumnDescription = "升级前等级", IsNullable = true)]
    public long? FromLevelId { get; set; }

    [SugarColumn(ColumnName = "to_level_id", ColumnDescription = "升级后等级", IsNullable = true)]
    public long? ToLevelId { get; set; }

    [SugarColumn(ColumnName = "opinion", ColumnDescription = "意见", Length = 500, IsNullable = true)]
    public string? Opinion { get; set; }

    [SugarColumn(ColumnName = "detail_json", ColumnDescription = "明细JSON", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? DetailJson { get; set; }

    [SugarColumn(ColumnName = "action_time", ColumnDescription = "动作时间")]
    public DateTime ActionTime { get; set; }
}
