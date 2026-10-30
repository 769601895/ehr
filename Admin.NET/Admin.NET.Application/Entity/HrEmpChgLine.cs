namespace Admin.NET.Application;

/// <summary>员工异动明细 hr_emp_chg_line（异动前/后快照）</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_chg_line", "员工异动明细")]
public class HrEmpChgLine : EntityBase
{
    /// <summary>主表 Id</summary>
    [SugarColumn(ColumnName = "chg_id", ColumnDescription = "异动主表Id")]
    public long ChgId { get; set; }

    /// <summary>字段键，如 ccCode</summary>
    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 64)]
    public string FieldKey { get; set; }

    /// <summary>主档物理列名快照</summary>
    [SugarColumn(ColumnName = "db_column", ColumnDescription = "主档列名", Length = 64)]
    public string DbColumn { get; set; }

    /// <summary>显示名快照</summary>
    [SugarColumn(ColumnName = "label", ColumnDescription = "显示名", Length = 256, IsNullable = true)]
    public string? Label { get; set; }

    /// <summary>异动前代码值</summary>
    [SugarColumn(ColumnName = "value_before", ColumnDescription = "异动前", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ValueBefore { get; set; }

    /// <summary>异动后代码值；空则生效时跳过该列</summary>
    [SugarColumn(ColumnName = "value_after", ColumnDescription = "异动后", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ValueAfter { get; set; }

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; }
}
