namespace Admin.NET.Application;

/// <summary>异动可改字段配置 hr_emp_chg_field</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_emp_chg_field", "异动可改字段配置")]
public class HrEmpChgField : EntityBase
{
    /// <summary>对应人员字段池 field_key</summary>
    [SugarColumn(ColumnName = "field_key", ColumnDescription = "字段键", Length = 64)]
    public string FieldKey { get; set; }

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "sort_no", ColumnDescription = "排序")]
    public int SortNo { get; set; }

    /// <summary>异动后是否必填</summary>
    [SugarColumn(ColumnName = "required", ColumnDescription = "必填")]
    public bool Required { get; set; }
}
