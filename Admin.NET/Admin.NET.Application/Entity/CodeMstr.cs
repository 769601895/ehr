namespace Admin.NET.Application;

/// <summary>
/// 人事代码表
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("code_mstr", "人事代码表")]
[IgnoreTable]
public class CodeMstr
{
    [SugarColumn(ColumnName = "code_field", IsPrimaryKey = true, Length = 50)]
    public string CodeField { get; set; }

    [SugarColumn(ColumnName = "code_value", IsPrimaryKey = true, Length = 50)]
    public string CodeValue { get; set; }

    [SugarColumn(ColumnName = "code_desc", Length = 50, IsNullable = true)]
    public string? CodeDesc { get; set; }

    [SugarColumn(ColumnName = "code_sort", Length = 50, IsNullable = true)]
    public string? CodeSort { get; set; }
}
