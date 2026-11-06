namespace Admin.NET.Application;

/// <summary>
/// 用户人事管辖部门（只存管辖根 CcCode，查询时取当前 cc_level 前缀）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("Usercc", "用户人事管辖部门")]
[SugarIndex("uk_usercc_user_cc", nameof(UserId), OrderByType.Asc, nameof(CcCode), OrderByType.Asc, IsUnique = true)]
[SugarIndex("ix_usercc_userid", nameof(UserId), OrderByType.Asc)]
[SugarIndex("ix_usercc_cccode", nameof(CcCode), OrderByType.Asc)]
public class Usercc
{
    [SugarColumn(ColumnDescription = "主键", IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnDescription = "系统用户Id")]
    public long UserId { get; set; }

    [SugarColumn(ColumnDescription = "管辖根部门内码", Length = 50)]
    public string CcCode { get; set; }

    [SugarColumn(ColumnDescription = "含下级")]
    public bool IncludeChild { get; set; } = true;

    [SugarColumn(ColumnDescription = "创建时间", IsNullable = true)]
    public DateTime? CreateTime { get; set; }

    [SugarColumn(ColumnDescription = "创建人", IsNullable = true)]
    public long? CreateUserId { get; set; }
}
