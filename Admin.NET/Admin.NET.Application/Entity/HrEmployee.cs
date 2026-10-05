namespace Admin.NET.Application;

/// <summary>
/// 人事员工（仅用于部门占用校验）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("employee", "人事员工")]
[IgnoreTable]
public class HrEmployee
{
    [SugarColumn(ColumnName = "emp_id", IsPrimaryKey = true, Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "cc_code", Length = 50, IsNullable = true)]
    public string? CcCode { get; set; }
}
