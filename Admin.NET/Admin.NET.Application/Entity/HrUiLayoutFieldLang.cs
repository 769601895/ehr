namespace Admin.NET.Application;

/// <summary>
/// 布局字段按语言稀疏覆盖。主键 layout_field_id + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_layout_field_lang", "人事UI布局字段译文")]
public class HrUiLayoutFieldLang
{
    [SugarColumn(ColumnName = "layout_field_id", IsPrimaryKey = true)]
    public long LayoutFieldId { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "label", Length = 256, IsNullable = true)]
    public string Label { get; set; }
}
