using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Application;

/// <summary>扫描离职/停职/复职待生效单据，到日写员工主档</summary>
[JobDetail("hrEmpLeaveApplyJob", Description = "离职/停职/复职到日生效写主档", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "hrEmpLeaveApplyTrigger", Description = "每天扫描离职/停职/复职待生效", StartTime = "2026-01-01 01:50:00")]
public class HrEmpLeaveApplyJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public HrEmpLeaveApplyJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(HrEmpLeaveApplyJob));
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var leaveApply = scope.ServiceProvider.GetRequiredService<HrEmpLeaveApplyService>();
        var suspendSvc = scope.ServiceProvider.GetRequiredService<HrEmpSuspendService>();
        var n1 = await leaveApply.ApplyDueAsync();
        var n2 = await suspendSvc.ApplyDueAsync();
        _logger.LogInformation("HrEmpLeaveApplyJob 完成，离职/复职={Leave} 停职={Suspend}", n1, n2);
        context.Result = $"leaveRestore={n1};suspend={n2}";
    }
}
