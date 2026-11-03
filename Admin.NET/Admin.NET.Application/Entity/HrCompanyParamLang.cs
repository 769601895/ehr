namespace Admin.NET.Application;

/// <summary>
/// 公司参数名称译文。主键 ParamId + Culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("HrCompanyParamLang", "公司参数译文")]
[IgnoreTable]
public class HrCompanyParamLang
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "参数Id")]
    public long ParamId { get; set; }

    [SugarColumn(IsPrimaryKey = true, Length = 16, ColumnDescription = "文化")]
    public string Culture { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "公司名称")]
    public string? SiteName { get; set; }
}
