namespace Admin.NET.Application;

/// <summary>
/// 字段能否进入某场景设计器。主键 module_code + field_key + scene。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_field_scene", "人事UI字段场景")]
public class HrUiFieldScene
{
    [SugarColumn(ColumnName = "module_code", IsPrimaryKey = true, Length = 64)]
    public string ModuleCode { get; set; }

    [SugarColumn(ColumnName = "field_key", IsPrimaryKey = true, Length = 64)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "scene", IsPrimaryKey = true, Length = 20)]
    public string Scene { get; set; }
}
