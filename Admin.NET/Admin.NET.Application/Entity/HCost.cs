namespace Admin.NET.Application;

/// <summary>
/// 成本中心。用于部门成本中心下拉。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_cost", "成本中心")]
[IgnoreTable]
public class HCost
{
    [SugarColumn(ColumnName = "cost_id", ColumnDescription = "成本中心代码", IsPrimaryKey = true, Length = 50)]
    public string CostId { get; set; }

    [SugarColumn(ColumnName = "cost_name", ColumnDescription = "成本中心名称", Length = 100, IsNullable = true)]
    public string? CostName { get; set; }

    [SugarColumn(ColumnName = "cost_site", ColumnDescription = "厂别", Length = 50, IsNullable = true)]
    public string? CostSite { get; set; }

    [SugarColumn(ColumnName = "cost_state", ColumnDescription = "状态(Y启用/N停用)", Length = 10, IsNullable = true)]
    public string? CostState { get; set; }
}
