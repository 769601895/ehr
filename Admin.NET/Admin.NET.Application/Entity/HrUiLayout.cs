namespace Admin.NET.Application;

/// <summary>
/// 人事 UI 布局方案头
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_ui_layout", "人事UI布局方案")]
public class HrUiLayout : EntityBase
{
    [SugarColumn(ColumnName = "module_code", ColumnDescription = "模块码", Length = 50)]
    public string ModuleCode { get; set; }

    /// <summary>list=列表 form=表单</summary>
    [SugarColumn(ColumnName = "scene", ColumnDescription = "场景", Length = 20)]
    public string Scene { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "方案名称", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "is_default", ColumnDescription = "是否系统默认")]
    public bool IsDefault { get; set; }

    [SugarColumn(ColumnName = "owner_user_id", ColumnDescription = "所属用户", IsNullable = true)]
    public long? OwnerUserId { get; set; }

    [SugarColumn(ColumnName = "version", ColumnDescription = "版本")]
    public int Version { get; set; } = 1;
}
