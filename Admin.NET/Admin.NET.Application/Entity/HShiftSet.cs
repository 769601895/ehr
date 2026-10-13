namespace Admin.NET.Application;

/// <summary>
/// 考勤组别。用于部门考勤组别下拉。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_shift_set", "考勤组别")]
[IgnoreTable]
public class HShiftSet
{
    [SugarColumn(ColumnName = "shift_id", ColumnDescription = "考勤组别代码", IsPrimaryKey = true, Length = 50)]
    public string ShiftId { get; set; }

    [SugarColumn(ColumnName = "shift_desc", ColumnDescription = "考勤组别描述", Length = 100, IsNullable = true)]
    public string? ShiftDesc { get; set; }

    [SugarColumn(ColumnName = "shift_site", ColumnDescription = "厂区", Length = 50, IsNullable = true)]
    public string? ShiftSite { get; set; }

    [SugarColumn(ColumnName = "shift_tpksite", ColumnDescription = "集团厂别", Length = 50, IsNullable = true)]
    public string? ShiftTpkSite { get; set; }
}
