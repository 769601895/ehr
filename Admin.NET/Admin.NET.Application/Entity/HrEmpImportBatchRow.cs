namespace Admin.NET.Application;

/// <summary>导入失败行明细</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_import_batch_row", "员工资料导入失败行")]
public class HrEmpImportBatchRow : EntityBase
{
    [SugarColumn(ColumnName = "batch_id", ColumnDescription = "批次Id")]
    public long BatchId { get; set; }

    [SugarColumn(ColumnName = "row_no", ColumnDescription = "Excel行号")]
    public int RowNo { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 40, IsNullable = true)]
    public string? EmpId { get; set; }

    [SugarColumn(ColumnName = "error_message", ColumnDescription = "错误信息", Length = 1000)]
    public string ErrorMessage { get; set; }
}
