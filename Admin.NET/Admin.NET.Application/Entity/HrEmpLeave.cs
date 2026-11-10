namespace Admin.NET.Application;

/// <summary>员工离职单 hr_emp_leave</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_leave", "员工离职单")]
public class HrEmpLeave : EntityBase
{
    [SugarColumn(ColumnName = "leave_no", ColumnDescription = "离职单号", Length = 50, IsNullable = false)]
    public string LeaveNo { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = false)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "emp_name", ColumnDescription = "姓名快照", Length = 100, IsNullable = true)]
    public string? EmpName { get; set; }

    [SugarColumn(ColumnName = "leave_date", ColumnDescription = "离职日", IsNullable = false)]
    public DateTime LeaveDate { get; set; }

    [SugarColumn(ColumnName = "plan_effect_date", ColumnDescription = "计划生效日", IsNullable = false)]
    public DateTime PlanEffectDate { get; set; }

    [SugarColumn(ColumnName = "leave_type", ColumnDescription = "离职类型", Length = 50, IsNullable = false)]
    public string LeaveType { get; set; }

    [SugarColumn(ColumnName = "leave_reason", ColumnDescription = "离职原因", Length = 50, IsNullable = false)]
    public string LeaveReason { get; set; }

    [SugarColumn(ColumnName = "salary_settle", ColumnDescription = "薪资结算", Length = 50, IsNullable = false)]
    public string SalarySettle { get; set; }

    [SugarColumn(ColumnName = "is_blacklist", ColumnDescription = "是否黑名单", IsNullable = false)]
    public bool IsBlacklist { get; set; }

    [SugarColumn(ColumnName = "cc_code_snap", ColumnDescription = "部门快照", Length = 50, IsNullable = true)]
    public string? CcCodeSnap { get; set; }

    [SugarColumn(ColumnName = "emp_state_before", ColumnDescription = "生效前状态", IsNullable = true)]
    public int? EmpStateBefore { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20, IsNullable = false)]
    public string Status { get; set; }

    [SugarColumn(ColumnName = "submit_time", ColumnDescription = "提交时间", IsNullable = true)]
    public DateTime? SubmitTime { get; set; }

    [SugarColumn(ColumnName = "submit_user_id", ColumnDescription = "提交人", IsNullable = true)]
    public long? SubmitUserId { get; set; }

    [SugarColumn(ColumnName = "approve_time", ColumnDescription = "审核时间", IsNullable = true)]
    public DateTime? ApproveTime { get; set; }

    [SugarColumn(ColumnName = "approve_user_id", ColumnDescription = "审核人", IsNullable = true)]
    public long? ApproveUserId { get; set; }

    [SugarColumn(ColumnName = "reject_reason", ColumnDescription = "驳回原因", Length = 500, IsNullable = true)]
    public string? RejectReason { get; set; }

    [SugarColumn(ColumnName = "effective_time", ColumnDescription = "生效时间", IsNullable = true)]
    public DateTime? EffectiveTime { get; set; }

    [SugarColumn(ColumnName = "void_time", ColumnDescription = "撤销时间", IsNullable = true)]
    public DateTime? VoidTime { get; set; }

    [SugarColumn(ColumnName = "void_user_id", ColumnDescription = "撤销人", IsNullable = true)]
    public long? VoidUserId { get; set; }

    [SugarColumn(ColumnName = "void_reason", ColumnDescription = "撤销原因", Length = 500, IsNullable = true)]
    public string? VoidReason { get; set; }
}
