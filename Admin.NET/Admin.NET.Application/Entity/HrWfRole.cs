namespace Admin.NET.Application;

/// <summary>
/// 审批角色
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_role", "审批角色")]
public class HrWfRole : EntityBase
{
    /// <summary>审核等级；为空表示组织虚拟节点（不参与审核，仅挂子节点）</summary>
    [SugarColumn(ColumnName = "level_id", ColumnDescription = "审核等级Id", IsNullable = true)]
    public long? LevelId { get; set; }

    [SugarColumn(ColumnName = "code", ColumnDescription = "角色编码", Length = 50)]
    public string Code { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "显示名", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父级分类", IsNullable = true)]
    public long? ParentId { get; set; }

    [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
