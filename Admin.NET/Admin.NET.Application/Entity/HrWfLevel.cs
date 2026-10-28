namespace Admin.NET.Application;

/// <summary>
/// 审核等级
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_level", "审核等级")]
public class HrWfLevel : EntityBase
{
    [SugarColumn(ColumnName = "code", ColumnDescription = "等级编码", Length = 50)]
    public string Code { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "显示名", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "audit_order", ColumnDescription = "审核顺序")]
    public int AuditOrder { get; set; }

    [SugarColumn(ColumnName = "is_functional", ColumnDescription = "是否职能审核")]
    public bool IsFunctional { get; set; }

    [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
