namespace Admin.NET.Application;

/// <summary>
/// 人事部门主档
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("cc_mstr", "人事部门主档")]
[SugarIndex("ix_cc_mstr_cc_level", nameof(CcLevel), OrderByType.Asc)]
[SugarIndex("ix_cc_mstr_ParentID", nameof(ParentId), OrderByType.Asc)]
public class CcMstr
{
    [SugarColumn(ColumnName = "cc_code", ColumnDescription = "部门编码", IsPrimaryKey = true, Length = 50)]
    public string CcCode { get; set; }

    [SugarColumn(ColumnName = "cc_code_parent", ColumnDescription = "父部门编码", Length = 50, IsNullable = true)]
    public string? CcCodeParent { get; set; }

    [SugarColumn(ColumnName = "cc_dept", ColumnDescription = "部门代号", Length = 50, IsNullable = true)]
    public string? CcDept { get; set; }

    [SugarColumn(ColumnName = "cc_desc", ColumnDescription = "部门名称", Length = 122)]
    public string CcDesc { get; set; }

    [SugarColumn(ColumnName = "cc_level", ColumnDescription = "层级码", Length = 50)]
    public string CcLevel { get; set; }

    [SugarColumn(ColumnName = "cc_group", ColumnDescription = "部门组", IsNullable = true)]
    public int? CcGroup { get; set; }

    [SugarColumn(ColumnName = "cc_indet", ColumnDescription = "缩进/标识", Length = 50, IsNullable = true)]
    public string? CcIndet { get; set; }

    [SugarColumn(ColumnName = "cc_sub", ColumnDescription = "下属标志", IsNullable = true)]
    public float? CcSub { get; set; }

    [SugarColumn(ColumnName = "cc_site", ColumnDescription = "厂区", Length = 50, IsNullable = true)]
    public string? CcSite { get; set; }

    [SugarColumn(ColumnName = "cc_state", ColumnDescription = "状态码", IsNullable = true)]
    public int? CcState { get; set; }

    [SugarColumn(ColumnName = "cc_cost", ColumnDescription = "成本类型", Length = 50, IsNullable = true)]
    public string? CcCost { get; set; }

    [SugarColumn(ColumnName = "cc_wage_rate", ColumnDescription = "工资率", IsNullable = true)]
    public float? CcWageRate { get; set; }

    [SugarColumn(ColumnName = "cc_stat_basic", ColumnDescription = "统计分类", Length = 50, IsNullable = true)]
    public string? CcStatBasic { get; set; }

    /// <summary>0=启用，1=停用</summary>
    [SugarColumn(ColumnName = "cc_enable", ColumnDescription = "停用标志(0启用/1停用)", IsNullable = true)]
    public int? CcEnable { get; set; }

    [SugarColumn(ColumnName = "cc_dele_date", ColumnDescription = "删除日期", IsNullable = true)]
    public DateTime? CcDeleDate { get; set; }

    [SugarColumn(ColumnName = "cc_count_group", ColumnDescription = "统计组", Length = 60, IsNullable = true)]
    public string? CcCountGroup { get; set; }

    [SugarColumn(ColumnName = "cc_user", ColumnDescription = "维护人", Length = 50, IsNullable = true)]
    public string? CcUser { get; set; }

    [SugarColumn(ColumnName = "cc_date", ColumnDescription = "维护时间", IsNullable = true)]
    public DateTime? CcDate { get; set; }

    [SugarColumn(ColumnName = "cc_kqgroup", ColumnDescription = "考勤组", IsNullable = true)]
    public int? CcKqgroup { get; set; }

    [SugarColumn(ColumnName = "cc_woca", ColumnDescription = "工时类别", Length = 50, IsNullable = true)]
    public string? CcWoca { get; set; }

    [SugarColumn(ColumnName = "cc_bz_unit", ColumnDescription = "编制单位", Length = 50, IsNullable = true)]
    public string? CcBzUnit { get; set; }

    [SugarColumn(ColumnName = "cc_dele_remark", ColumnDescription = "删除备注", Length = 120, IsNullable = true)]
    public string? CcDeleRemark { get; set; }

    [SugarColumn(ColumnName = "cc_desc_en", ColumnDescription = "英文名称", Length = 150, IsNullable = true)]
    public string? CcDescEn { get; set; }

    [SugarColumn(ColumnName = "cc_level_censor", ColumnDescription = "审核级别", Length = 50, IsNullable = true)]
    public string? CcLevelCensor { get; set; }

    [SugarColumn(ColumnName = "cc_xz_group", ColumnDescription = "薪酬组", Length = 50, IsNullable = true)]
    public string? CcXzGroup { get; set; }

    [SugarColumn(ColumnName = "cc_create_date", ColumnDescription = "创建日期", IsNullable = true)]
    public DateTime? CcCreateDate { get; set; }

    [SugarColumn(ColumnName = "machine_floor", ColumnDescription = "楼层", Length = 10, IsNullable = true)]
    public string? MachineFloor { get; set; }

    [SugarColumn(ColumnName = "kq_new_sysuser", ColumnDescription = "考勤同步人", Length = 25, IsNullable = true)]
    public string? KqNewSysuser { get; set; }

    [SugarColumn(ColumnName = "kq_new_sysdate", ColumnDescription = "考勤同步时间", IsNullable = true)]
    public DateTime? KqNewSysdate { get; set; }

    [SugarColumn(ColumnName = "ID", ColumnDescription = "自增辅助键", IsIdentity = true, IsNullable = false)]
    public int CcIdentityId { get; set; }

    [SugarColumn(ColumnName = "ParentID", ColumnDescription = "父节点辅助Id", IsNullable = true)]
    public int? ParentId { get; set; }

    [SugarColumn(ColumnName = "cc_code2", ColumnDescription = "辅助编码", Length = 50, IsNullable = true)]
    public string? CcCode2 { get; set; }
}
