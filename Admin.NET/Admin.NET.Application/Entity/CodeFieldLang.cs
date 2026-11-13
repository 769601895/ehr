namespace Admin.NET.Application;

/// <summary>
/// 代码类名称/用途译文。主键 code_field + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("code_field_lang", "代码类译文")]
[IgnoreTable]
public class CodeFieldLang
{
    [SugarColumn(ColumnName = "code_field", IsPrimaryKey = true, Length = 50)]
    public string CodeFieldKey { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "field_name", Length = 100, IsNullable = true)]
    public string? FieldName { get; set; }

    [SugarColumn(ColumnName = "usage_desc", Length = 256, IsNullable = true)]
    public string? UsageDesc { get; set; }
}
