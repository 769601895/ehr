using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Application;

/// <summary>扫描异动待生效单据，到日写员工主档</summary>
[JobDetail("hrEmpChgApplyJob", Description = "异动到日生效写主档", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "hrEmpChgApplyTrigger", Description = "每天扫描异动待生效", StartTime = "2026-01-01 01:40:00")]
public class HrEmpChgApplyJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public HrEmpChgApplyJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(HrEmpChgApplyJob));
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var apply = scope.ServiceProvider.GetRequiredService<HrEmpChgApplyService>();
        var n = await apply.ApplyDueAsync();
        _logger.LogInformation("HrEmpChgApplyJob 完成，成功生效 {Count} 条", n);
        context.Result = $"applied={n}";
    }
}
