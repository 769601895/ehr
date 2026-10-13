namespace Admin.NET.Application;

/// <summary>
/// 人事 UI 布局字段明细
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_layout_field", "人事UI布局字段")]
public class HrUiLayoutField : EntityBase
{
    [SugarColumn(ColumnName = "layout_id", ColumnDescription = "方案Id")]
    public long LayoutId { get; set; }

    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 50)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "db_column", ColumnDescription = "库列名", Length = 50, IsNullable = true)]
    public string? DbColumn { get; set; }

    [SugarColumn(ColumnName = "label", ColumnDescription = "显示名", Length = 50)]
    public string Label { get; set; }

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "visible", ColumnDescription = "是否显示")]
    public bool Visible { get; set; } = true;

    [SugarColumn(ColumnName = "width_px", ColumnDescription = "列宽px", IsNullable = true)]
    public int? WidthPx { get; set; }

    [SugarColumn(ColumnName = "span_slots", ColumnDescription = "表单占位格数", IsNullable = true)]
    public int? SpanSlots { get; set; }

    [SugarColumn(ColumnName = "group_code", ColumnDescription = "分组", Length = 20, IsNullable = true)]
    public string? GroupCode { get; set; }

    [SugarColumn(ColumnName = "control_type", ColumnDescription = "控件类型", Length = 20)]
    public string ControlType { get; set; } = "text";

    [SugarColumn(ColumnName = "required", ColumnDescription = "是否必填")]
    public bool Required { get; set; }

    [SugarColumn(ColumnName = "readonly", ColumnDescription = "是否只读")]
    public bool Readonly { get; set; }
}
