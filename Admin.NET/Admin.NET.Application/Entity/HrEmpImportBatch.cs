namespace Admin.NET.Application;

/// <summary>员工资料导入批次历史</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_import_batch", "员工资料导入批次")]
public class HrEmpImportBatch : EntityBase
{
    [SugarColumn(ColumnName = "batch_no", ColumnDescription = "批次号", Length = 40)]
    public string BatchNo { get; set; }

    [SugarColumn(ColumnName = "import_mode", ColumnDescription = "作业类型", Length = 16)]
    public string ImportMode { get; set; }

    [SugarColumn(ColumnName = "tpl_id", ColumnDescription = "模板Id", IsNullable = true)]
    public long? TplId { get; set; }

    [SugarColumn(ColumnName = "tpl_name_snapshot", ColumnDescription = "模板名快照", Length = 100, IsNullable = true)]
    public string? TplNameSnapshot { get; set; }

    [SugarColumn(ColumnName = "field_keys_snapshot", ColumnDescription = "字段快照JSON", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? FieldKeysSnapshot { get; set; }

    [SugarColumn(ColumnName = "file_name", ColumnDescription = "文件名", Length = 256, IsNullable = true)]
    public string? FileName { get; set; }

    [SugarColumn(ColumnName = "total_count", ColumnDescription = "总行数")]
    public int TotalCount { get; set; }

    [SugarColumn(ColumnName = "success_count", ColumnDescription = "成功行数")]
    public int SuccessCount { get; set; }

    [SugarColumn(ColumnName = "fail_count", ColumnDescription = "失败行数")]
    public int FailCount { get; set; }

    /// <summary>Success / Failed</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 16)]
    public string Status { get; set; }

    [SugarColumn(ColumnName = "error_summary", ColumnDescription = "错误摘要", Length = 1000, IsNullable = true)]
    public string? ErrorSummary { get; set; }

    [SugarColumn(ColumnName = "operator_user_id", ColumnDescription = "操作人")]
    public long OperatorUserId { get; set; }
}
