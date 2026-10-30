namespace Admin.NET.Application;

/// <summary>员工异动主表 hr_emp_chg</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_chg", "员工异动主表")]
public class HrEmpChg : EntityBase
{
    /// <summary>异动单号</summary>
    [SugarColumn(ColumnName = "chg_no", ColumnDescription = "异动单号", Length = 50, IsNullable = false)]
    public string ChgNo { get; set; }

    /// <summary>工号</summary>
    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = false)]
    public string EmpId { get; set; }

    /// <summary>姓名快照</summary>
    [SugarColumn(ColumnName = "emp_name", ColumnDescription = "姓名快照", Length = 100, IsNullable = true)]
    public string? EmpName { get; set; }

    /// <summary>异动日期</summary>
    [SugarColumn(ColumnName = "chg_date", ColumnDescription = "异动日期", IsNullable = false)]
    public DateTime ChgDate { get; set; }

    /// <summary>异动类别代码</summary>
    [SugarColumn(ColumnName = "chg_type", ColumnDescription = "异动类别", Length = 50, IsNullable = false)]
    public string ChgType { get; set; }

    /// <summary>异动原因</summary>
    [SugarColumn(ColumnName = "chg_reason", ColumnDescription = "异动原因", Length = 500, IsNullable = true)]
    public string? ChgReason { get; set; }

    /// <summary>备注</summary>
    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>状态</summary>
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

    /// <summary>本次生效实际写入的主档列（逗号分隔）</summary>
    [SugarColumn(ColumnName = "applied_columns", ColumnDescription = "已写入列", Length = 500, IsNullable = true)]
    public string? AppliedColumns { get; set; }
}
