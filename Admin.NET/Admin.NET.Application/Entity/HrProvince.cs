namespace Admin.NET.Application;

/// <summary>
/// 省份代码（籍贯主数据）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_province", "省份代码")]
public class HrProvince : EntityBase
{
    [SugarColumn(ColumnName = "province_code", ColumnDescription = "省代码", Length = 12)]
    public string ProvinceCode { get; set; }

    [SugarColumn(ColumnName = "province_name", ColumnDescription = "省名称", Length = 50)]
    public string ProvinceName { get; set; }

    [SugarColumn(ColumnName = "order_no", ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>Active=现用，Inactive=停用</summary>
    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; } = HrRegionConsts.StatusActive;

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
