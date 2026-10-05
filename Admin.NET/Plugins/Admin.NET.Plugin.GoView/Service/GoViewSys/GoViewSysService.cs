// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// 系统登录服务 🧩
/// </summary>
[UnifyProvider("GoView")]
[ApiDescriptionSettings(GoViewConst.GroupName, Module = "goview", Name = "sys", Order = 100, Description = "系统登录")]
public class GoViewSysService : IDynamicApiController
{
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SysAuthService _sysAuthService;
    private readonly SysCacheService _sysCacheService;
    private readonly SysTenantService _sysTenantService;

    public GoViewSysService(SqlSugarRepository<SysUser> sysUserRep,
        SysAuthService sysAuthService,
        SysCacheService sysCacheService,
        SysTenantService sysTenantService)
    {
        _sysUserRep = sysUserRep;
        _sysAuthService = sysAuthService;
        _sysCacheService = sysCacheService;
        _sysTenantService = sysTenantService;
    }

    /// <summary>
    /// GoView 登录 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("GoView 登录")]
    public async Task<GoViewLoginOutput> Login(GoViewLoginInput input)
    {
        // 设置默认租户
        input.TenantId ??= SqlSugarConst.DefaultTenantId;

        // 关闭默认租户验证码验证
        var tenantList = _sysCacheService.Get<List<SysTenant>>(CacheConst.KeyTenant);
        var tenant = tenantList.FirstOrDefault(u => u.Id == SqlSugarConst.DefaultTenantId);
        tenant.Captcha = false;
        _sysCacheService.Set(CacheConst.KeyTenant, tenantList);

        input.Password = CryptogramHelper.SM2Encrypt(input.Password);
        var loginResult = await _sysAuthService.Login(new LoginInput()
        {
            Account = input.Username,
            Password = input.Password,
        });

        // 启用默认租户验证码验证
        tenant.Captcha = true;
        _sysCacheService.Set(CacheConst.KeyTenant, tenantList);

        var sysUser = await _sysUserRep.AsQueryable().ClearFilter<ITenantIdFilter>().FirstAsync(u => u.Account.Equals(input.Username));
        return new GoViewLoginOutput()
        {
            Userinfo = new GoViewLoginUserInfo
            {
                Id = sysUser.Id.ToString(),
                Username = sysUser.Account,
                Nickname = sysUser.NickName,
            },
            Token = new GoViewLoginToken
            {
                TokenValue = $"Bearer {loginResult.AccessToken}"
            }
        };
    }

    /// <summary>
    /// GoView 退出 🔖
    /// </summary>
    [DisplayName("GoView 退出")]
    public async Task GetLogout()
    {
        await Task.FromResult(_sysAuthService.Logout());
    }

    /// <summary>
    /// 获取 OSS 上传接口 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [ApiDescriptionSettings(Name = "GetOssInfo")]
    [DisplayName("获取 OSS 上传接口")]
    public Task<GoViewOssUrlOutput> GetOssInfo()
    {
        return Task.FromResult(new GoViewOssUrlOutput { BucketURL = "" });
    }

    /// <summary>
    /// 获取系统基本信息 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("获取系统基本信息")]
    public async Task<dynamic> GetSysInfo([FromQuery] long tenantId)
    {
        return await _sysTenantService.GetSysInfo(tenantId);
    }
}