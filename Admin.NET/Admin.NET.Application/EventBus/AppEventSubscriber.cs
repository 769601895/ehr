// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application;

/// <summary>
/// 事件订阅
/// </summary>
public class AppEventSubscriber : IEventSubscriber, ISingleton, IDisposable
{
    private readonly IServiceScope _serviceScope;

    public AppEventSubscriber(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
    }

    /// <summary>新增用户后：主机构+ExtOrg → Usercc</summary>
    [EventSubscribe(UserEventTypeEnum.Add)]
    public async Task OnUserAdd(EventHandlerExecutingContext context)
    {
        var user = context.GetPayload<SysUser>();
        if (user == null || user.Id <= 0) return;
        var scope = _serviceScope.ServiceProvider.GetRequiredService<HrUserCcScopeService>();
        await scope.SyncFromExtOrgAsync(user.Id);
    }

    /// <summary>更新用户后：同步人事管辖 Usercc</summary>
    [EventSubscribe(UserEventTypeEnum.Update)]
    public async Task OnUserUpdate(EventHandlerExecutingContext context)
    {
        var input = context.GetPayload<AddUserInput>();
        if (input == null || input.Id <= 0) return;
        var scope = _serviceScope.ServiceProvider.GetRequiredService<HrUserCcScopeService>();
        await scope.SyncFromExtOrgAsync(input.Id);
    }

    /// <summary>删除用户后：清理 Usercc</summary>
    [EventSubscribe(UserEventTypeEnum.Delete)]
    public async Task OnUserDelete(EventHandlerExecutingContext context)
    {
        var input = context.GetPayload<DeleteUserInput>();
        if (input == null || input.Id <= 0) return;
        var scope = _serviceScope.ServiceProvider.GetRequiredService<HrUserCcScopeService>();
        await scope.ClearUserAsync(input.Id);
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}
