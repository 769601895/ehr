namespace Admin.NET.Application;

/// <summary>
/// 审批表单方案
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_form", "审批表单方案")]
public class HrWfForm : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "code", ColumnDescription = "方案编码", Length = 50)]
    public string Code { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 100)]
    public string Name { get; set; }

    /// <summary>B=引擎自定义；A=池字段</summary>
    [SugarColumn(ColumnName = "form_mode", ColumnDescription = "表单模式", Length = 10)]
    public string FormMode { get; set; } = "B";

    [SugarColumn(ColumnName = "version", ColumnDescription = "版本")]
    public int Version { get; set; } = 1;

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
