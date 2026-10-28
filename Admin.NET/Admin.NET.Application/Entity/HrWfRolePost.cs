namespace Admin.NET.Application;

/// <summary>
/// 审批角色-岗位
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_role_post", "审批角色岗位")]
public class HrWfRolePost : EntityBase
{
    [SugarColumn(ColumnName = "role_id", ColumnDescription = "角色Id")]
    public long RoleId { get; set; }

    [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位编码", Length = 50)]
    public string PostId { get; set; }
}
