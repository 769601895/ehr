namespace Admin.NET.Application;

/// <summary>
/// 岗位代码
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_post", "岗位代码")]
public class HrPost : EntityBase
{
    [SugarColumn(ColumnName = "post_code", ColumnDescription = "岗位代码", Length = 50)]
    public string PostCode { get; set; }

    [SugarColumn(ColumnName = "post_name", ColumnDescription = "岗位名称", Length = 100)]
    public string PostName { get; set; }

    [SugarColumn(ColumnName = "grade", ColumnDescription = "等级")]
    public int Grade { get; set; }

    /// <summary>Active=现用，Inactive=停用</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; } = HrPostConsts.StatusActive;

    [SugarColumn(ColumnName = "cc_code", ColumnDescription = "所属部门内码", Length = 64, IsNullable = true)]
    public string? CcCode { get; set; }

    [SugarColumn(ColumnName = "need_audit", ColumnDescription = "岗位是否审核")]
    public bool NeedAudit { get; set; } = true;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
