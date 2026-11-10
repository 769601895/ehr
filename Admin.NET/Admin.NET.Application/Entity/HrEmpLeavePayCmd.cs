namespace Admin.NET.Application;

/// <summary>离职薪资结算指令 hr_emp_leave_pay_cmd（算薪后接消费）</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_leave_pay_cmd", "离职薪资结算指令")]
public class HrEmpLeavePayCmd : EntityBase
{
    [SugarColumn(ColumnName = "leave_id", ColumnDescription = "离职单Id", IsNullable = false)]
    public long LeaveId { get; set; }

    [SugarColumn(ColumnName = "leave_no", ColumnDescription = "离职单号", Length = 50, IsNullable = false)]
    public string LeaveNo { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = false)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "leave_date", ColumnDescription = "离职日", IsNullable = false)]
    public DateTime LeaveDate { get; set; }

    [SugarColumn(ColumnName = "settle_code", ColumnDescription = "结算码", Length = 50, IsNullable = false)]
    public string SettleCode { get; set; }

    [SugarColumn(ColumnName = "pay_period", ColumnDescription = "薪酬月yyyyMM", Length = 6, IsNullable = false)]
    public string PayPeriod { get; set; }

    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20, IsNullable = false)]
    public string Status { get; set; }
}
