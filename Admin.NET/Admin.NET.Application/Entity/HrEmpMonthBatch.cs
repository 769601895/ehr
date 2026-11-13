namespace Admin.NET.Application;

/// <summary>人事月报批次 hr_emp_month_batch</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_month_batch", "人事月报批次")]
[SugarIndex("ux_hr_emp_month_batch_site_period", nameof(SiteCode), OrderByType.Asc, nameof(PayPeriod), OrderByType.Asc, IsUnique = true)]
public class HrEmpMonthBatch : EntityBase
{
    [SugarColumn(ColumnName = "site_code", ColumnDescription = "厂别代码", Length = 32, IsNullable = false)]
    public string SiteCode { get; set; }

    [SugarColumn(ColumnName = "site_name", ColumnDescription = "厂别名称", Length = 100, IsNullable = true)]
    public string? SiteName { get; set; }

    [SugarColumn(ColumnName = "pay_period", ColumnDescription = "薪酬月", Length = 6, IsNullable = false)]
    public string PayPeriod { get; set; }

    [SugarColumn(ColumnName = "att_month", ColumnDescription = "出勤归属月", Length = 6, IsNullable = false)]
    public string AttMonth { get; set; }

    [SugarColumn(ColumnName = "att_from", ColumnDescription = "考勤开始日", IsNullable = false)]
    public DateTime AttFrom { get; set; }

    [SugarColumn(ColumnName = "att_to", ColumnDescription = "考勤结束日", IsNullable = false)]
    public DateTime AttTo { get; set; }

    [SugarColumn(ColumnName = "status", ColumnDescription = "批次状态", Length = 20, IsNullable = false)]
    public string Status { get; set; }

    [SugarColumn(ColumnName = "param_snapshot", ColumnDescription = "参数快照", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ParamSnapshot { get; set; }

    [SugarColumn(ColumnName = "field_schema_snapshot", ColumnDescription = "字段方案快照", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? FieldSchemaSnapshot { get; set; }

    [SugarColumn(ColumnName = "freeze_version", ColumnDescription = "冻结版本号", IsNullable = false)]
    public int FreezeVersion { get; set; }

    [SugarColumn(ColumnName = "frozen_time", ColumnDescription = "冻结时间", IsNullable = true)]
    public DateTime? FrozenTime { get; set; }

    [SugarColumn(ColumnName = "frozen_user_id", ColumnDescription = "冻结人", IsNullable = true)]
    public long? FrozenUserId { get; set; }

    [SugarColumn(ColumnName = "unfreeze_reason", ColumnDescription = "解冻原因", Length = 500, IsNullable = true)]
    public string? UnfreezeReason { get; set; }

    [SugarColumn(ColumnName = "consumed_time", ColumnDescription = "消费时间", IsNullable = true)]
    public DateTime? ConsumedTime { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}
