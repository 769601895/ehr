namespace Admin.NET.Application;

/// <summary>
/// 字段池译文。主键 module_code + field_key + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_field_def_lang", "人事UI字段池译文")]
public class HrUiFieldDefLang
{
    [SugarColumn(ColumnName = "module_code", IsPrimaryKey = true, Length = 64)]
    public string ModuleCode { get; set; }

    [SugarColumn(ColumnName = "field_key", IsPrimaryKey = true, Length = 64)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "label", Length = 256, IsNullable = true)]
    public string Label { get; set; }
}
