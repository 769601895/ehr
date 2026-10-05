// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application;

/// <summary>
/// 代码自动生成DEMO主表
/// </summary>
/// <remarks>这个类型主要用于给新人快速体验代码自动生成的主从表功能</remarks>
[SugarTable(null, "代码自动生成DEMO主表")]
public class TestCodeGenDemoMaster : EntityBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", Length = 20)]
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 120)]
    public string? Remark { get; set; }

    /// <summary>
    /// 字典1
    /// </summary>
    [SugarColumn(ColumnDescription = "字典1")]
    public string? Dict1 { get; set; }

    /// <summary>
    /// 上传控件(保存主键)
    /// </summary>
    [SugarColumn(ColumnDescription = "上传控件(保存主键)", IsNullable = true)]
    public long? UploadImageId { get; set; }

    /// <summary>
    /// 上传控件(保存路径)
    /// </summary>
    [SugarColumn(ColumnDescription = "上传控件(保存路径)", IsNullable = true)]
    public string? UploadImage { get; set; }
}

/// <summary>
/// 代码自动生成DEMO子表
/// </summary>
/// <remarks>这个类型主要用于给新人快速体验代码自动生成功能和测试该功能是否正常</remarks>
[SugarTable(null, "代码自动生成DEMO子表")]
public class TestCodeGenDemoSlave : EntityBase
{
    /// <summary>
    /// 主表记录ID(如果不是测试主从表，可以手工输入)
    /// </summary>
    [SugarColumn(ColumnDescription = "主表记录ID", DefaultValue = "0")]
    public long MasterId { get; set; }

    /// <summary>
    /// 流水号
    /// </summary>
    [BindSerial(nameof(SerialTypeProvider.Test))]
    [SugarColumn(ColumnDescription = "流水号", Length = 128)]
    public string? SeqNo { get; set; }

    /// <summary>
    /// 文本
    /// </summary>
    [SugarColumn(ColumnDescription = "文本", Length = 128)]
    public string? Name { get; set; }

    /// <summary>
    /// 数值
    /// </summary>
    [SugarColumn(ColumnDescription = "数值")]
    public int? Age { get; set; }

    /// <summary>
    /// 时间选择
    /// </summary>
    [SugarColumn(ColumnDescription = "时间选择")]
    public DateTime? Date1 { get; set; }

    /// <summary>
    /// 开关
    /// </summary>
    [SugarColumn(ColumnDescription = "开关")]
    public bool? IsOk { get; set; }

    /// <summary>
    /// 外键(sys_user)
    /// </summary>
    [SugarColumn(ColumnDescription = "外键(sys_user)")]
    public long? UserId { get; set; }

    /// <summary>
    /// 树选择框1(sys_org)
    /// </summary>
    [SugarColumn(ColumnDescription = "树选择框1(sys_org)")]
    public long? OrgId { get; set; }

    /// <summary>
    /// 树选择框2(sys_menu)
    /// </summary>
    [SugarColumn(ColumnDescription = "树选择框2(sys_menu)")]
    public long? MenuId { get; set; }

    /// <summary>
    /// 字典1
    /// </summary>
    [SugarColumn(ColumnDescription = "字典1")]
    public string? Dict1 { get; set; }

    /// <summary>
    /// 枚举1
    /// </summary>
    [SugarColumn(ColumnDescription = "枚举1")]
    public int? Enum1 { get; set; }

    /// <summary>
    /// 常量1
    /// </summary>
    [SugarColumn(ColumnDescription = "常量1")]
    public int? Const1 { get; set; }

    /// <summary>
    /// 上传控件(保存主键)
    /// </summary>
    [SugarColumn(ColumnDescription = "上传控件(保存主键)", IsNullable = true)]
    public long? UploadImageId { get; set; }

    /// <summary>
    /// 上传控件(保存路径)
    /// </summary>
    [SugarColumn(ColumnDescription = "上传控件(保存路径)", IsNullable = true)]
    public string? UploadImage { get; set; }
}