namespace Admin.NET.Application;

/// <summary>
/// 工号流水号段（按厂别 + 作用域）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("HrEmpIdSeq", "工号流水号段")]
[SugarIndex("ux_HrEmpIdSeq_site_scope", nameof(SiteCode), OrderByType.Asc, nameof(ScopeKey), OrderByType.Asc, IsUnique = true)]
public class HrEmpIdSeq
{
    [SugarColumn(IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnDescription = "厂别", Length = 32)]
    public string SiteCode { get; set; }

    /// <summary>作用域：* / yyyy / yyyyMM 等</summary>
    [SugarColumn(ColumnDescription = "号段作用域", Length = 32)]
    public string ScopeKey { get; set; }

    /// <summary>已发最大流水；下次发号 = NextVal + 1</summary>
    [SugarColumn(ColumnDescription = "已发最大流水")]
    public long NextVal { get; set; }

    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}
