namespace Admin.NET.Application;

/// <summary>高级查询方案 hr_adv_query_scheme</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_adv_query_scheme", "高级查询方案")]
[SugarIndex("ix_hr_adv_query_scheme_mod_owner", nameof(ModuleCode), OrderByType.Asc, nameof(OwnerUserId), OrderByType.Asc)]
public class HrAdvQueryScheme : EntityBase
{
    [SugarColumn(ColumnName = "module_code", ColumnDescription = "模块码", Length = 64, IsNullable = false)]
    public string ModuleCode { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "方案名称", Length = 100, IsNullable = false)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "filter_json", ColumnDescription = "条件树JSON", ColumnDataType = "nvarchar(max)", IsNullable = false)]
    public string FilterJson { get; set; }

    [SugarColumn(ColumnName = "owner_user_id", ColumnDescription = "所有者", IsNullable = false)]
    public long OwnerUserId { get; set; }

    [SugarColumn(ColumnName = "is_shared", ColumnDescription = "是否分享给角色")]
    public bool IsShared { get; set; }

    /// <summary>分享角色 Id JSON 数组，如 [1,2]</summary>
    [SugarColumn(ColumnName = "share_role_ids", ColumnDescription = "分享角色Ids", Length = 1000, IsNullable = true)]
    public string? ShareRoleIds { get; set; }
}
