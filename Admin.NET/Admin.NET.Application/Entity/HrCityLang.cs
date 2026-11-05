namespace Admin.NET.Application;

/// <summary>
/// 城市名称译文。主键 city_code + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_city_lang", "城市代码译文")]
[IgnoreTable]
public class HrCityLang
{
    [SugarColumn(ColumnName = "city_code", IsPrimaryKey = true, Length = 12)]
    public string CityCode { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "city_name", Length = 50, IsNullable = true)]
    public string? CityName { get; set; }
}
