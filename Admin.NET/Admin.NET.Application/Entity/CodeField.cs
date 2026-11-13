namespace Admin.NET.Application;

/// <summary>
/// 代码类元数据（code_mstr.code_field 的说明与软删）。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("code_field", "代码类")]
[IgnoreTable]
public class CodeField
{
    [SugarColumn(ColumnName = "code_field", IsPrimaryKey = true, Length = 50)]
    public string CodeFieldKey { get; set; }

    /// <summary>主显示名（通常简体中文）</summary>
    [SugarColumn(ColumnName = "field_name", Length = 100)]
    public string FieldName { get; set; }

    /// <summary>用途说明（主语言）</summary>
    [SugarColumn(ColumnName = "usage_desc", Length = 256, IsNullable = true)]
    public string? UsageDesc { get; set; }

    [SugarColumn(ColumnName = "order_no")]
    public int OrderNo { get; set; } = 100;

    [SugarColumn(ColumnName = "is_delete")]
    public bool IsDelete { get; set; }

    [SugarColumn(ColumnName = "delete_time", IsNullable = true)]
    public DateTime? DeleteTime { get; set; }

    [SugarColumn(ColumnName = "delete_user_id", IsNullable = true)]
    public long? DeleteUserId { get; set; }

    [SugarColumn(ColumnName = "delete_user_name", Length = 64, IsNullable = true)]
    public string? DeleteUserName { get; set; }

    [SugarColumn(ColumnName = "create_time", IsNullable = true)]
    public DateTime? CreateTime { get; set; }

    [SugarColumn(ColumnName = "update_time", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}
