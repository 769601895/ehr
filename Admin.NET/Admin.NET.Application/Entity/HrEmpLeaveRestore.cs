namespace Admin.NET.Application;

/// <summary>员工复职/纠错审计单 hr_emp_leave_restore</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_leave_restore", "员工复职单")]
public class HrEmpLeaveRestore : EntityBase
{
    [SugarColumn(ColumnName = "restore_no", ColumnDescription = "复职单号", Length = 50, IsNullable = false)]
    public string RestoreNo { get; set; }

    /// <summary>Reinstate=同工号复职；Correct=离职纠错审计</summary>
    [SugarColumn(ColumnName = "kind", ColumnDescription = "单据种类", Length = 20, IsNullable = true)]
    public string? Kind { get; set; }

    /// <summary>关联离职单（纠错审计）</summary>
    [SugarColumn(ColumnName = "leave_id", ColumnDescription = "离职单Id", IsNullable = true)]
    public long? LeaveId { get; set; }

    [SugarColumn(ColumnName = "leave_no", ColumnDescription = "离职单号", Length = 50, IsNullable = true)]
    public string? LeaveNo { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = false)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "emp_name", ColumnDescription = "姓名快照", Length = 100, IsNullable = true)]
    public string? EmpName { get; set; }

    [SugarColumn(ColumnName = "restore_date", ColumnDescription = "复职日", IsNullable = false)]
    public DateTime RestoreDate { get; set; }

    [SugarColumn(ColumnName = "plan_effect_date", ColumnDescription = "计划生效日", IsNullable = false)]
    public DateTime PlanEffectDate { get; set; }

    [SugarColumn(ColumnName = "clear_blacklist", ColumnDescription = "清除黑名单", IsNullable = false)]
    public bool ClearBlacklist { get; set; }

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

    [SugarColumn(ColumnName = "operate_user_id", ColumnDescription = "操作人", IsNullable = true)]
    public long? OperateUserId { get; set; }

    [SugarColumn(ColumnName = "operate_user_name", ColumnDescription = "操作人姓名", Length = 64, IsNullable = true)]
    public string? OperateUserName { get; set; }

    [SugarColumn(ColumnName = "operate_time", ColumnDescription = "操作时间", IsNullable = true)]
    public DateTime? OperateTime { get; set; }
}
