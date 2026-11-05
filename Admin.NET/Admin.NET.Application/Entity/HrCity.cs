namespace Admin.NET.Application;

/// <summary>
/// 城市代码（籍贯主数据）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_city", "城市代码")]
public class HrCity : EntityBase
{
    [SugarColumn(ColumnName = "province_code", ColumnDescription = "所属省代码", Length = 12)]
    public string ProvinceCode { get; set; }

    [SugarColumn(ColumnName = "city_code", ColumnDescription = "市代码", Length = 12)]
    public string CityCode { get; set; }

    [SugarColumn(ColumnName = "city_name", ColumnDescription = "市名称", Length = 50)]
    public string CityName { get; set; }

    [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>Active=现用，Inactive=停用</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; } = HrRegionConsts.StatusActive;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
