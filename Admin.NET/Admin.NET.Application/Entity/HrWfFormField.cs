namespace Admin.NET.Application;

/// <summary>
/// 审批表单字段
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_form_field", "审批表单字段")]
public class HrWfFormField : EntityBase
{
    [SugarColumn(ColumnName = "form_id", ColumnDescription = "表单方案Id")]
    public long FormId { get; set; }

    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 50)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "label", ColumnDescription = "显示名", Length = 100)]
    public string Label { get; set; }

    [SugarColumn(ColumnName = "control_type", ColumnDescription = "控件类型", Length = 20)]
    public string ControlType { get; set; } = "text";

    /// <summary>Custom / Pool</summary>
    [SugarColumn(ColumnName = "field_source", ColumnDescription = "字段来源", Length = 20)]
    public string FieldSource { get; set; } = "Custom";

    [SugarColumn(ColumnName = "pool_field_key", ColumnDescription = "池字段键", Length = 50, IsNullable = true)]
    public string? PoolFieldKey { get; set; }

    [SugarColumn(ColumnName = "required", ColumnDescription = "必填")]
    public bool Required { get; set; }

    [SugarColumn(ColumnName = "readonly", ColumnDescription = "只读")]
    public bool Readonly { get; set; }

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "group_code", ColumnDescription = "分组", Length = 20, IsNullable = true)]
    public string? GroupCode { get; set; }

    [SugarColumn(ColumnName = "span_slots", ColumnDescription = "栅格占位", IsNullable = true)]
    public int? SpanSlots { get; set; }

    [SugarColumn(ColumnName = "code_field", ColumnDescription = "代码类", Length = 50, IsNullable = true)]
    public string? CodeField { get; set; }

    [SugarColumn(ColumnName = "default_value", ColumnDescription = "默认值", Length = 256, IsNullable = true)]
    public string? DefaultValue { get; set; }

    [SugarColumn(ColumnName = "placeholder", ColumnDescription = "占位提示", Length = 100, IsNullable = true)]
    public string? Placeholder { get; set; }

    [SugarColumn(ColumnName = "extra_json", ColumnDescription = "扩展JSON", ColumnDataType = "nvarchar(max)", IsNullable = true)]
    public string? ExtraJson { get; set; }
}
