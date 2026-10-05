namespace Admin.NET.Core;

/// <summary>
/// 事件状态枚举
/// </summary>
[Description("事件状态")]
public enum EventStatusEnum
{
    /// <summary>
    /// 执行中
    /// </summary>
    [Description("执行中")]
    Running = 1,

    /// <summary>
    /// 成功
    /// </summary>
    [Description("成功")]
    Success = 2,

    /// <summary>
    /// 重试中
    /// </summary>
    [Description("重试中")]
    Retry = 3,

    /// <summary>
    /// 异常
    /// </summary>
    [Description("异常")]
    Error = 4,

    /// <summary>
    /// 超时
    /// </summary>
    [Description("超时")]
    Timeout = 5
}