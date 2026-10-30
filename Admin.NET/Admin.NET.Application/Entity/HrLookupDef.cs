namespace Admin.NET.Application;

/// <summary>
/// 查找数据源（字段池下拉可配主档表）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_lookup_def", "查找数据源")]
public class HrLookupDef : EntityBase
{
    [SugarColumn(ColumnName = "code", ColumnDescription = "编码", Length = 64)]
    public string Code { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "table_name", ColumnDescription = "主表", Length = 128)]
    public string TableName { get; set; }

    [SugarColumn(ColumnName = "value_column", ColumnDescription = "值列", Length = 64)]
    public string ValueColumn { get; set; }

    [SugarColumn(ColumnName = "label_column", ColumnDescription = "显示列", Length = 64)]
    public string LabelColumn { get; set; }

    [SugarColumn(ColumnName = "order_column", ColumnDescription = "排序列", Length = 64, IsNullable = true)]
    public string? OrderColumn { get; set; }

    [SugarColumn(ColumnName = "filter_use_is_delete", ColumnDescription = "过滤未删除")]
    public bool FilterUseIsDelete { get; set; } = true;

    [SugarColumn(ColumnName = "filter_enabled_column", ColumnDescription = "启用列", Length = 64, IsNullable = true)]
    public string? FilterEnabledColumn { get; set; }

    [SugarColumn(ColumnName = "filter_enabled_value", ColumnDescription = "启用值", Length = 64, IsNullable = true)]
    public string? FilterEnabledValue { get; set; }

    [SugarColumn(ColumnName = "filter_equal_column", ColumnDescription = "等值列", Length = 64, IsNullable = true)]
    public string? FilterEqualColumn { get; set; }

    [SugarColumn(ColumnName = "filter_equal_value", ColumnDescription = "等值", Length = 128, IsNullable = true)]
    public string? FilterEqualValue { get; set; }

    [SugarColumn(ColumnName = "lang_table", ColumnDescription = "翻译表", Length = 128, IsNullable = true)]
    public string? LangTable { get; set; }

    [SugarColumn(ColumnName = "lang_value_column", ColumnDescription = "译文值列", Length = 64, IsNullable = true)]
    public string? LangValueColumn { get; set; }

    [SugarColumn(ColumnName = "lang_culture_column", ColumnDescription = "文化列", Length = 64, IsNullable = true)]
    public string? LangCultureColumn { get; set; }

    [SugarColumn(ColumnName = "lang_label_column", ColumnDescription = "译文显示列", Length = 64, IsNullable = true)]
    public string? LangLabelColumn { get; set; }

    [SugarColumn(ColumnName = "maintain_path", ColumnDescription = "维护页路由", Length = 256, IsNullable = true)]
    public string? MaintainPath { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
