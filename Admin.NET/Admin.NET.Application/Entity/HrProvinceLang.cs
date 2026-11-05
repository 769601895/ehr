namespace Admin.NET.Application;

/// <summary>
/// 省份名称译文。主键 province_code + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_province_lang", "省份代码译文")]
[IgnoreTable]
public class HrProvinceLang
{
    [SugarColumn(ColumnName = "province_code", IsPrimaryKey = true, Length = 12)]
    public string ProvinceCode { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "province_name", Length = 50, IsNullable = true)]
    public string? ProvinceName { get; set; }
}
