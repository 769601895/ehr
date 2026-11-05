namespace Admin.NET.Application;

/// <summary>导入模板字段</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_import_tpl_field", "员工资料导入模板字段")]
public class HrEmpImportTplField : EntityBase
{
    [SugarColumn(ColumnName = "tpl_id", ColumnDescription = "模板Id")]
    public long TplId { get; set; }

    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 64)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "顺序")]
    public int SortNo { get; set; } = 100;

    [SugarColumn(ColumnName = "is_locked", ColumnDescription = "是否锁定")]
    public bool IsLocked { get; set; }
}
