namespace Admin.NET.Application;

/// <summary>
/// 人事 UI 字段池
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_field_def", "人事UI字段池")]
[SugarIndex("UX_hr_ui_field_def_key", nameof(ModuleCode), OrderByType.Asc, nameof(FieldKey), OrderByType.Asc, true)]
public class HrUiFieldDef : EntityBase
{
    [SugarColumn(ColumnName = "module_code", ColumnDescription = "模块码", Length = 64)]
    public string ModuleCode { get; set; }

    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 64)]
    public string FieldKey { get; set; }

    /// <summary>physical / computed / lookup</summary>
    [SugarColumn(ColumnName = "origin", ColumnDescription = "来源", Length = 20)]
    public string Origin { get; set; } = "physical";

    [SugarColumn(ColumnName = "db_column", ColumnDescription = "库列名", Length = 64, IsNullable = true)]
    public string DbColumn { get; set; }

    [SugarColumn(ColumnName = "table_name", ColumnDescription = "主表", Length = 128, IsNullable = true)]
    public string TableName { get; set; }

    [SugarColumn(ColumnName = "label", ColumnDescription = "主语言显示名", Length = 256)]
    public string Label { get; set; }

    [SugarColumn(ColumnName = "data_type", ColumnDescription = "数据类型", Length = 50, IsNullable = true)]
    public string DataType { get; set; }

    [SugarColumn(ColumnName = "is_nullable", ColumnDescription = "可空")]
    public bool IsNullable { get; set; } = true;

    [SugarColumn(ColumnName = "control_type", ColumnDescription = "控件类型", Length = 20)]
    public string ControlType { get; set; } = "text";

    [SugarColumn(ColumnName = "code_field", ColumnDescription = "代码类", Length = 50, IsNullable = true)]
    public string CodeField { get; set; }

    /// <summary>查找数据源编码（hr_lookup_def.code）；与 CodeField 互斥</summary>
    [SugarColumn(ColumnName = "lookup_source", ColumnDescription = "查找数据源", Length = 64, IsNullable = true)]
    public string? LookupSource { get; set; }

    [SugarColumn(ColumnName = "default_width_px", ColumnDescription = "默认列宽", IsNullable = true)]
    public int? DefaultWidthPx { get; set; }

    [SugarColumn(ColumnName = "default_span_slots", ColumnDescription = "默认占位")]
    public int DefaultSpanSlots { get; set; } = 1;

    [SugarColumn(ColumnName = "group_code", ColumnDescription = "分组", Length = 20, IsNullable = true)]
    public string GroupCode { get; set; }

    [SugarColumn(ColumnName = "is_in_pool", ColumnDescription = "是否入池")]
    public bool IsInPool { get; set; } = true;

    [SugarColumn(ColumnName = "sortable", ColumnDescription = "可排序")]
    public bool Sortable { get; set; } = true;

    [SugarColumn(ColumnName = "required", ColumnDescription = "默认必填")]
    public bool Required { get; set; }

    [SugarColumn(ColumnName = "readonly", ColumnDescription = "默认只读")]
    public bool Readonly { get; set; }

    [SugarColumn(ColumnName = "locked", ColumnDescription = "锁定")]
    public bool Locked { get; set; }

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;
}
