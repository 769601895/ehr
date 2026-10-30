namespace Admin.NET.Application;

/// <summary>
/// 岗位名称译文。主键 post_code + culture。
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_post_lang", "岗位代码译文")]
[IgnoreTable]
public class HrPostLang
{
    [SugarColumn(ColumnName = "post_code", IsPrimaryKey = true, Length = 50)]
    public string PostCode { get; set; }

    [SugarColumn(ColumnName = "culture", IsPrimaryKey = true, Length = 16)]
    public string Culture { get; set; }

    [SugarColumn(ColumnName = "post_name", Length = 100, IsNullable = true)]
    public string? PostName { get; set; }
}
