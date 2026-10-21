namespace Admin.NET.Application;

/// <summary>
/// 人事代码描述译文。主键 code_field + code_value + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("code_mstr_lang", "人事代码译文")]
[IgnoreTable]
public class CodeMstrLang
{
    [SugarColumn(ColumnName = "code_field", IsPrimaryKey = true, Length = 50)]
    public string CodeField { get; set; }

    [SugarColumn(ColumnName = "code_value", IsPrimaryKey = true, Length = 50)]
    public string CodeValue { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "code_desc", Length = 256, IsNullable = true)]
    public string CodeDesc { get; set; }
}
