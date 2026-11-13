namespace Admin.NET.Application;

/// <summary>人事月报行 hr_emp_month_line</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_month_line", "人事月报行")]
[SugarIndex("ux_hr_emp_month_line_batch_emp", nameof(BatchId), OrderByType.Asc, nameof(EmpId), OrderByType.Asc, IsUnique = true)]
public class HrEmpMonthLine : EntityBase
{
    [SugarColumn(ColumnName = "batch_id", ColumnDescription = "批次Id", IsNullable = false)]
    public long BatchId { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = false)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "iempid", ColumnDescription = "内部主键", IsNullable = false)]
    public long IEmpId { get; set; }

    [SugarColumn(ColumnName = "emp_name", ColumnDescription = "姓名", Length = 100, IsNullable = true)]
    public string? EmpName { get; set; }

    [SugarColumn(ColumnName = "include_flag", ColumnDescription = "纳入标志", Length = 20, IsNullable = false)]
    public string IncludeFlag { get; set; }

    [SugarColumn(ColumnName = "manual_lock", ColumnDescription = "手工锁定", IsNullable = false)]
    public bool ManualLock { get; set; }

    [SugarColumn(ColumnName = "emp_state", ColumnDescription = "人员状态", IsNullable = true)]
    public int? EmpState { get; set; }

    [SugarColumn(ColumnName = "emp_sal", ColumnDescription = "薪酬类别", Length = 2, IsNullable = true)]
    public string? EmpSal { get; set; }

    [SugarColumn(ColumnName = "cc_code", ColumnDescription = "部门编码", Length = 50, IsNullable = true)]
    public string? CcCode { get; set; }

    [SugarColumn(ColumnName = "cc_name", ColumnDescription = "部门名称", Length = 100, IsNullable = true)]
    public string? CcName { get; set; }

    [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位", Length = 50, IsNullable = true)]
    public string? PostId { get; set; }

    [SugarColumn(ColumnName = "duty_id", ColumnDescription = "职务", Length = 50, IsNullable = true)]
    public string? DutyId { get; set; }

    [SugarColumn(ColumnName = "emp_in_date", ColumnDescription = "入职日", IsNullable = true)]
    public DateTime? EmpInDate { get; set; }

    [SugarColumn(ColumnName = "emp_lea_date", ColumnDescription = "离职日", IsNullable = true)]
    public DateTime? EmpLeaDate { get; set; }

    [SugarColumn(ColumnName = "emp_kq_indate", ColumnDescription = "考勤开始日", IsNullable = true)]
    public DateTime? EmpKqIndate { get; set; }

    [SugarColumn(ColumnName = "emp_kq_lea_date", ColumnDescription = "考勤结束日", IsNullable = true)]
    public DateTime? EmpKqLeaDate { get; set; }

    [SugarColumn(ColumnName = "need_attendance", ColumnDescription = "是否需考勤", IsNullable = false)]
    public bool NeedAttendance { get; set; }

    [SugarColumn(ColumnName = "is_new_hire", ColumnDescription = "是否新进", IsNullable = false)]
    public bool IsNewHire { get; set; }

    [SugarColumn(ColumnName = "is_leaver", ColumnDescription = "是否离职", IsNullable = false)]
    public bool IsLeaver { get; set; }

    [SugarColumn(ColumnName = "is_suspend", ColumnDescription = "是否停职", IsNullable = false)]
    public bool IsSuspend { get; set; }

    [SugarColumn(ColumnName = "has_chg_in_period", ColumnDescription = "月内有异动", IsNullable = false)]
    public bool HasChgInPeriod { get; set; }

    [SugarColumn(ColumnName = "leave_settle_code", ColumnDescription = "离职结算码", Length = 50, IsNullable = true)]
    public string? LeaveSettleCode { get; set; }

    [SugarColumn(ColumnName = "leave_pay_cmd_status", ColumnDescription = "薪资指令状态", Length = 20, IsNullable = true)]
    public string? LeavePayCmdStatus { get; set; }

    [SugarColumn(ColumnName = "pay_cmd_id", ColumnDescription = "薪资指令Id", IsNullable = true)]
    public long? PayCmdId { get; set; }

    [SugarColumn(ColumnName = "cc_code_at_start", ColumnDescription = "期初部门", Length = 50, IsNullable = true)]
    public string? CcCodeAtStart { get; set; }

    [SugarColumn(ColumnName = "cc_code_at_end", ColumnDescription = "期末部门", Length = 50, IsNullable = true)]
    public string? CcCodeAtEnd { get; set; }

    [SugarColumn(ColumnName = "chg_summary", ColumnDescription = "异动摘要", Length = 500, IsNullable = true)]
    public string? ChgSummary { get; set; }

    [SugarColumn(ColumnName = "chg_field_snapshot", ColumnDescription = "异动字段快照", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ChgFieldSnapshot { get; set; }

    [SugarColumn(ColumnName = "cost_cc_code", ColumnDescription = "成本部门", Length = 50, IsNullable = true)]
    public string? CostCcCode { get; set; }

    [SugarColumn(ColumnName = "exclude_reason", ColumnDescription = "排除原因", Length = 50, IsNullable = true)]
    public string? ExcludeReason { get; set; }

    [SugarColumn(ColumnName = "include_reason", ColumnDescription = "纳入原因", Length = 50, IsNullable = true)]
    public string? IncludeReason { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(ColumnName = "emp_field_snapshot", ColumnDescription = "人事字段快照", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? EmpFieldSnapshot { get; set; }

    [SugarColumn(ColumnName = "warn_codes", ColumnDescription = "警告码", Length = 500, IsNullable = true)]
    public string? WarnCodes { get; set; }

    [SugarColumn(ColumnName = "error_codes", ColumnDescription = "错误码", Length = 500, IsNullable = true)]
    public string? ErrorCodes { get; set; }
}
