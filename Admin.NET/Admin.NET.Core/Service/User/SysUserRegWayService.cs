// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统用户注册方案服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 490, Description = "用户注册方案")]
public class SysUserRegWayService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUserRegWay> _sysUserRegWayRep;

    public SysUserRegWayService(UserManager userManager, SqlSugarRepository<SysUserRegWay> sysUserRegWayRep)
    {
        _userManager = userManager;
        _sysUserRegWayRep = sysUserRegWayRep;
    }

    /// <summary>
    /// 获取注册方案分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取注册方案分页列表")]
    public async Task<SqlSugarPagedList<PageUserRegWayOutput>> Page(PageUserRegWayInput input)
    {
        return await _sysUserRegWayRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .Select(u => new PageUserRegWayOutput
            {
                TenantName = u.Tenant.Title,
                RoleName = u.SysRole.Name,
                OrgName = u.SysOrg.Name,
                PosName = u.SysPos.Name,
            }, true)
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取注册方案列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取注册方案列表")]
    public async Task<List<UserRegWayOutput>> List(PageUserRegWayInput input)
    {
        return await _sysUserRegWayRep.AsQueryable()
            .WhereIF(input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .Select(u => new UserRegWayOutput
            {
                TenantName = u.Tenant.Title,
                OrgName = u.SysOrg.Name,
                RoleName = u.SysRole.Name,
                PosName = u.SysPos.Name,
            }, true)
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();
    }

    /// <summary>
    /// 增加注册方案 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加注册方案")]
    public async Task<long> Add(AddUserRegWayInput input)
    {
        var entity = input.Adapt<SysUserRegWay>();
        if (await _sysUserRegWayRep.IsAnyAsync(u => u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D2101);

        return await _sysUserRegWayRep.InsertReturnSnowflakeIdAsync(entity);
    }

    /// <summary>
    /// 更新注册方案 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新注册方案")]
    public async Task Update(UpdateUserRegWayInput input)
    {
        if (await _sysUserRegWayRep.IsAnyAsync(u => u.Id != input.Id && u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D2101);

        await _sysUserRegWayRep.AsUpdateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除注册方案 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除注册方案")]
    public async Task Delete(BaseIdInput input)
    {
        await _sysUserRegWayRep.DeleteByIdAsync(input.Id);
    }
}