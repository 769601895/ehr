namespace Admin.NET.Application;

/// <summary>员工资料导入模板（个人私有）</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_import_tpl", "员工资料导入模板")]
public class HrEmpImportTpl : EntityBase
{
    [SugarColumn(ColumnName = "owner_user_id", ColumnDescription = "所属用户")]
    public long OwnerUserId { get; set; }

    /// <summary>Add / Update</summary>
    [SugarColumn(ColumnName = "import_mode", ColumnDescription = "作业类型", Length = 16)]
    public string ImportMode { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "模板名称", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
