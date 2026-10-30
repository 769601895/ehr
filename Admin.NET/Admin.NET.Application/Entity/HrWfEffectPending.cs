namespace Admin.NET.Application;

/// <summary>审批待生效队列（纯表单 B / 无业务明细时）</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_effect_pending", "审批待生效队列")]
public class HrWfEffectPending : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "biz_id", ColumnDescription = "业务单/实例Id", Length = 64)]
    public string BizId { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "plan_effect_date", ColumnDescription = "计划生效日")]
    public DateTime PlanEffectDate { get; set; }

    /// <summary>Pending | Applied | Cancelled</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; } = HrWfConsts.EffectPending;

    [SugarColumn(ColumnName = "payload_json", ColumnDescription = "前后值快照", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? PayloadJson { get; set; }

    [SugarColumn(ColumnName = "applied_time", ColumnDescription = "实际生效时间", IsNullable = true)]
    public DateTime? AppliedTime { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 200, IsNullable = true)]
    public string? Remark { get; set; }
}
