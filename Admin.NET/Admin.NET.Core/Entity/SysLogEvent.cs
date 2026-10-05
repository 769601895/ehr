namespace Admin.NET.Core;

/// <summary>
/// 系统事件日志表
/// </summary>
[SugarTable(null, "系统事件日志表")]
[SugarIndex("i_{table}_r", nameof(RunId), OrderByType.Asc)]
[SugarIndex("i_{table}_e", nameof(EventId), OrderByType.Asc)]
[SysTable]
[LogTable]
public partial class SysLogEvent : EntityBase
{
    /// <summary>
    /// 运行ID
    /// </summary>
    [SugarColumn(ColumnDescription = "运行ID", Length = 64)]
    [MaxLength(64)]
    public string RunId { get; set; }

    /// <summary>
    /// 事件ID
    /// </summary>
    [SugarColumn(ColumnDescription = "事件ID", Length = 64)]
    [MaxLength(64)]
    public string EventId { get; set; }

    /// <summary>
    /// 事件参数
    /// </summary>
    [SugarColumn(ColumnDescription = "事件参数", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Payload { get; set; }

    /// <summary>
    /// 事件创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "事件创建时间")]
    public DateTime CreateTimeOfEvent { get; set; }

    /// <summary>
    /// 执行前时间
    /// </summary>
    [SugarColumn(ColumnDescription = "执行前时间")]
    public DateTime ExecutingTime { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Exception { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "耗时（毫秒）")]
    public long? Elapsed { get; set; }

    /// <summary>
    /// 执行后时间
    /// </summary>
    [SugarColumn(ColumnDescription = "执行后时间")]
    public DateTime? ExecutedTime { get; set; }

    /// <summary>
    /// 订阅特性
    /// </summary>
    [SugarColumn(ColumnDescription = "订阅特性", Length = 512)]
    public string? Attribute { get; set; }

    /// <summary>
    /// 触发方法
    /// </summary>
    [SugarColumn(ColumnDescription = "触发方法", Length = 128)]
    public string? HandlerMethod { get; set; }

    /// <summary>
    /// 共享上下文数据
    /// </summary>
    [SugarColumn(ColumnDescription = "共享上下文数据", Length = 1024)]
    public string? Properties { get; set; }

    /// <summary>
    /// 消息是否只消费一次
    /// </summary>
    [SugarColumn(ColumnDescription = "消息是否只消费一次")]
    public bool ConsumeOnce { get; set; }

    /// <summary>
    /// 重试记录
    /// </summary>
    [SugarColumn(ColumnDescription = "重试记录", Length = 1024)]
    public string? RetryRecord { get; set; }

    /// <summary>
    /// 事件状态
    /// </summary>
    [SugarColumn(ColumnDescription = "事件状态")]
    public EventStatusEnum Status { get; set; }
}