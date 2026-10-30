using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Application;

/// <summary>每日扫描审批待生效队列并写主档</summary>
[JobDetail("hrWfEffectApplyJob", Description = "审批业务到日生效写主档", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "hrWfEffectApplyTrigger", Description = "每天扫描待生效", StartTime = "2026-01-01 01:30:00")]
public class HrWfEffectApplyJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public HrWfEffectApplyJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(HrWfEffectApplyJob));
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var apply = scope.ServiceProvider.GetRequiredService<HrWfEffectApplyService>();
        var n = await apply.ApplyDuePendingsAsync();
        _logger.LogInformation("HrWfEffectApplyJob 完成，成功生效 {Count} 条", n);
        context.Result = $"applied={n}";
    }
}
