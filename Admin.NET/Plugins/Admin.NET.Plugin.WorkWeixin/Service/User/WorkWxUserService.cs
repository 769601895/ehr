// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信成员服务 🧩
/// </summary>
public class WorkWxUserService(WorkWxBaseService baseService) : ITransient
{
    /// <summary>
    /// 创建成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<CreateUserWorkWxOutput> Create(CreateUserWorkWxInput input)
    {
        return await baseService.SendAsync<CreateUserWorkWxInput, CreateUserWorkWxOutput>(input);
    }

    /// <summary>
    /// 读取成员
    /// </summary>
    /// <param name="userId">成员UserID。对应管理端的账号，企业内必须唯一。不区分大小写，长度为1~64个字节</param>
    /// <returns></returns>
    public async Task<UserWorkWxOutput> Get(string userId)
    {
        return await baseService.SendAsync<UserWorkWxInput, UserWorkWxOutput>(new() { UserId = userId });
    }

    /// <summary>
    /// 修改成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Update(UpdateUserWorkWxInput input)
    {
        return await baseService.SendAsync<UpdateUserWorkWxInput, BaseWorkWxOutput>(input);
    }

    /// <summary>
    /// 删除成员
    /// </summary>
    /// <param name="userId">成员UserID。对应管理端的账号</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Delete(string userId)
    {
        return await baseService.SendAsync<DeleteUserWorkWxInput, BaseWorkWxOutput>(new() { UserId = userId });
    }

    /// <summary>
    /// 批量删除成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> DeleteUser(BatchDeleteUserWorkWxInput input)
    {
        return await baseService.SendAsync<BatchDeleteUserWorkWxInput, BaseWorkWxOutput>(input);
    }

    /// <summary>
    /// 获取部门成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DeptUserSimpleListWorkWxOutput> SimpleListDept(DeptUserSimpleListWorkWxInput input)
    {
        return await baseService.SendAsync<DeptUserSimpleListWorkWxInput, DeptUserSimpleListWorkWxOutput>(input);
    }

    /// <summary>
    /// 获取部门成员详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DeptUserDetailListWorkWxOutput> DeptUserDetail(DeptUserDetailListWorkWxInput input)
    {
        return await baseService.SendAsync<DeptUserDetailListWorkWxInput, DeptUserDetailListWorkWxOutput>(input);
    }

    /// <summary>
    /// userid转openid
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ConvToOpenIdWorkWxOutput> UserIdToOpenId(ConvToOpenIdWorkWxInput input)
    {
        return await baseService.SendAsync<ConvToOpenIdWorkWxInput, ConvToOpenIdWorkWxOutput>(input);
    }

    /// <summary>
    /// openid转userid
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ConvToUserIdWorkWxOutput> OpenIdToUserId(ConvToUserIdWorkWxInput input)
    {
        return await baseService.SendAsync<ConvToUserIdWorkWxInput, ConvToUserIdWorkWxOutput>(input);
    }

    /// <summary>
    /// 登录二次验证
    /// </summary>
    /// <param name="userId">成员UserID。对应管理端的账号</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> AuthSucc(string userId)
    {
        return await baseService.SendAsync<UserAuthSuccessWorkWxInput, BaseWorkWxOutput>(new() { UserId = userId });
    }

    /// <summary>
    /// 邀请成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<InviteUserWorkWxOutput> InviteUser(InviteUserWorkWxInput input)
    {
        return await baseService.SendAsync<InviteUserWorkWxInput, InviteUserWorkWxOutput>(input);
    }

    /// <summary>
    /// 获取加入企业二维码
    /// </summary>
    /// <param name="sizeType">qrcode尺寸类型，1: 171 x 171; 2: 399 x 399; 3: 741 x 741; 4: 2052 x 2052</param>
    /// <returns></returns>
    public async Task<JoinQrcodeWorkWxOutput> GetJoinQrcode(int? sizeType)
    {
        return await baseService.SendAsync<JoinQrcodeWorkWxInput, JoinQrcodeWorkWxOutput>(new() { SizeType = sizeType });
    }

    /// <summary>
    /// 通过手机号获取成员ID
    /// </summary>
    /// <param name="mobile">用户在企业微信通讯录中的手机号码。长度为5~32个字节</param>
    /// <returns></returns>
    public async Task<UserIdWorkWxOutput> GetUserIdByMobile(string mobile)
    {
        return await baseService.SendAsync<UserIdByMobileWorkWxInput, UserIdWorkWxOutput>(new() { Mobile = mobile });
    }

    /// <summary>
    /// 通过邮箱获取成员ID
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="emailType">邮箱类型：1-企业邮箱（默认）；2-个人邮箱</param>
    /// <returns></returns>
    public async Task<UserIdWorkWxOutput> GetUserIdByEmail(string email, int? emailType = 1)
    {
        return await baseService.SendAsync<UserIdByEmailWorkWxInput, UserIdWorkWxOutput>(new() { Email = email, EmailType = emailType });
    }

    /// <summary>
    /// 获取成员ID列表
    /// </summary>
    /// <param name="cursor">用于分页查询的游标，字符串类型，由上一次调用返回，首次调用不填</param>
    /// <param name="limit">分页，预期请求的数据量，取值范围 1 ~ 10000</param>
    /// <returns></returns>
    public async Task<UserIdListWorkWxOutput> GetUserIdList(string cursor = null, int? limit = 20)
    {
        return await baseService.SendAsync<UserIdListWorkWxInput, UserIdListWorkWxOutput>(new() { Cursor = cursor, Limit = limit });
    }
}