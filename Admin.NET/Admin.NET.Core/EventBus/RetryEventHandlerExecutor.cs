namespace Admin.NET.Core;

/// <summary>
/// 事件执行器（超时控制，失败重试、熔断等）
/// </summary>
public class RetryEventHandlerExecutor : IEventHandlerExecutor
{
    private readonly ILogger<RetryEventHandlerExecutor> _logger;
    private readonly SqlSugarScopeProvider _db;

    public RetryEventHandlerExecutor(ILogger<RetryEventHandlerExecutor> logger)
    {
        _logger = logger;
        // 切换日志独立数据库
        _db = SqlSugarSetup.LogDb;
    }

    public async Task ExecuteAsync(EventHandlerExecutingContext context, Func<EventHandlerExecutingContext, Task> handler)
    {
        var attr = context.Attribute;
        // 判断是否自定义了重试失败回调服务
        // var fallbackPolicyService = attr?.FallbackPolicy == null ? null : App.GetRequiredService(attr.FallbackPolicy) as IEventFallbackPolicy;

        // 是否需要记录日志
        var shouldLog = ShouldLog(context);

        var db = _db.CopyNew();
        SysLogEvent log = new()
        {
            RunId = context.Source.EventId,
            Status = EventStatusEnum.Running,
            EventId = context.Source.EventId,
            ExecutingTime = context.ExecutingTime,
            ConsumeOnce = context.Source.ConsumeOnce,
            CreateTimeOfEvent = context.Source.CreatedTime,
        };

        if (shouldLog)
        {
            try
            {
                log.Attribute = attr?.ToJson();
                log.HandlerMethod = context.HandlerMethod.Name;
                log.Properties = context.Properties != null ? JSON.Serialize(context.Properties) : null;
                log.Payload = context.Source.Payload != null ? JSON.Serialize(context.Source.Payload) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"序列化事件参数失败: {context.Source.EventId}");
            }
            finally
            {
                try
                {
                    await db.Insertable(log).ExecuteCommandAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"日志写入失败: {context.Source.EventId}");
                }
            }
        }

        var exceptionList = new List<string>();
        var numRetries = attr?.NumRetries ?? 0;
        var retryTimeout = attr?.RetryTimeout ?? 1000;
        var exceptionTypes = attr?.ExceptionTypes;

        await Retry.InvokeAsync(
            async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await handler(context);
                    log.Status = EventStatusEnum.Success;
                }
                catch (Exception ex)
                {
                    log.Status = EventStatusEnum.Retry;
                    try
                    {
                        log.Exception = JSON.Serialize(SerializableException.FromException(ex));
                    }
                    catch (Exception serializeEx)
                    {
                        _logger.LogError(serializeEx, $"序列化异常信息失败: {context.Source.EventId}");
                    }
                    exceptionList.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {ex.Message}");
                    _logger.LogError(ex, $"事件处理器 {context.Source.EventId} 执行出错");
                    throw;
                }
                finally
                {
                    log.ExecutedTime = DateTime.Now;
                    log.Elapsed = stopwatch.ElapsedMilliseconds;
                    if (exceptionList.Count > 0 && exceptionList.Count >= numRetries) log.Status = EventStatusEnum.Error;
                    log.RetryRecord = string.Join("\n", exceptionList);

                    // ===== 仅在需要时才更新日志 =====
                    if (shouldLog)
                    {
                        try
                        {
                            await db.Updateable(log).ExecuteCommandAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"日志更新失败: {context.Source.EventId}");
                        }
                    }
                }
            },
            numRetries, // 重试次数
            retryTimeout, // 重试间隔时间（毫秒）
            exceptionTypes: exceptionTypes, // 触发重试的异常类型
                                            //, fallbackPolicy: fallbackPolicyService == null ? null : async (Exception ex) => { await fallbackPolicyService.CallbackAsync(context, ex); }
            retryAction: (total, times) =>
            {
                _logger.LogWarning($"事件处理器 {context.Source.EventId} 第 {times}/{total} 次重试");
            }
        );
    }

    /// <summary>
    /// 日志排除规则
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static bool ShouldLog(EventHandlerExecutingContext context)
    {
        return context.HandlerMethod.GetCustomAttribute<SuppressMonitorAttribute>() == null;
    }
}