namespace Admin.NET.Application;

/// <summary>
/// 员工异动历史 h_emh_hist（生效写回留痕；与旧系统同名表对齐）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emh_hist", "员工异动历史")]
public class HrEmhHist
{
    [SugarColumn(ColumnName = "emh_id", ColumnDescription = "异动内码", IsPrimaryKey = true, IsIdentity = true)]
    public int EmhId { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", Length = 50, IsNullable = true)]
    public string? EmpId { get; set; }

    [SugarColumn(ColumnName = "emh_nbr", ColumnDescription = "异动单号", Length = 50, IsNullable = true)]
    public string? EmhNbr { get; set; }

    [SugarColumn(ColumnName = "emh_date", ColumnDescription = "异动日期", IsNullable = true)]
    public DateTime? EmhDate { get; set; }

    [SugarColumn(ColumnName = "emh_wdate", ColumnDescription = "申请/处理日", IsNullable = true)]
    public DateTime? EmhWdate { get; set; }

    [SugarColumn(ColumnName = "emh_type", ColumnDescription = "异动类型", Length = 50, IsNullable = true)]
    public string? EmhType { get; set; }

    [SugarColumn(ColumnName = "emh_reason", ColumnDescription = "原因", Length = 80, IsNullable = true)]
    public string? EmhReason { get; set; }

    [SugarColumn(ColumnName = "emh_occ", ColumnDescription = "原部门", Length = 50, IsNullable = true)]
    public string? EmhOcc { get; set; }

    [SugarColumn(ColumnName = "emh_ncc", ColumnDescription = "新部门", Length = 50, IsNullable = true)]
    public string? EmhNcc { get; set; }

    [SugarColumn(ColumnName = "emh_opost", ColumnDescription = "原岗位", Length = 50, IsNullable = true)]
    public string? EmhOpost { get; set; }

    [SugarColumn(ColumnName = "emh_npost", ColumnDescription = "新岗位", Length = 50, IsNullable = true)]
    public string? EmhNpost { get; set; }

    [SugarColumn(ColumnName = "emh_user", ColumnDescription = "操作人", Length = 50, IsNullable = true)]
    public string? EmhUser { get; set; }

    [SugarColumn(ColumnName = "emh_time", ColumnDescription = "操作时间", IsNullable = true)]
    public DateTime? EmhTime { get; set; }

    [SugarColumn(ColumnName = "emh_remark", ColumnDescription = "备注", Length = 50, IsNullable = true)]
    public string? EmhRemark { get; set; }

    [SugarColumn(ColumnName = "emh_audit_ok", ColumnDescription = "审核通过", IsNullable = true)]
    public int? EmhAuditOk { get; set; }

    [SugarColumn(ColumnName = "emh_apply_commit", ColumnDescription = "已提交", IsNullable = true)]
    public int? EmhApplyCommit { get; set; }

    [SugarColumn(ColumnName = "emh_inure_date", ColumnDescription = "生效时间", IsNullable = true)]
    public DateTime? EmhInureDate { get; set; }

    [SugarColumn(ColumnName = "emh_inure_user", ColumnDescription = "生效人", Length = 50, IsNullable = true)]
    public string? EmhInureUser { get; set; }

    [SugarColumn(ColumnName = "emh_inure", ColumnDescription = "已生效", IsNullable = true)]
    public int? EmhInure { get; set; }

    [SugarColumn(ColumnName = "emh_import", ColumnDescription = "导入标记", Length = 10, IsNullable = true)]
    public string? EmhImport { get; set; }
}
