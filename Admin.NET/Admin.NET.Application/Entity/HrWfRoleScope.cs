namespace Admin.NET.Application;

/// <summary>
/// 审批角色管辖部门
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_role_scope", "审批角色管辖部门")]
public class HrWfRoleScope : EntityBase
{
    [SugarColumn(ColumnName = "role_id", ColumnDescription = "角色Id")]
    public long RoleId { get; set; }

    [SugarColumn(ColumnName = "cc_code", ColumnDescription = "部门内码", Length = 64)]
    public string CcCode { get; set; }

    [SugarColumn(ColumnName = "include_child", ColumnDescription = "含下级")]
    public bool IncludeChild { get; set; } = true;
}
