// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 微信公众号服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 230, Description = "微信公众号")]
public class SysWechatService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysOAuthUser> _sysOAuthUserRep;
    private readonly WechatApiClientFactory _wechatApiClientFactory;
    private readonly WechatApiClient _wechatApiClient;
    private readonly SysAuthService _sysAuthService;

    public SysWechatService(
        UserManager userManager,
        SysAuthService sysAuthService,
        SysConfigService sysConfigService,
        SqlSugarRepository<SysOAuthUser> sysOAuthUserRep,
        WechatApiClientFactory wechatApiClientFactory)
    {
        _sysAuthService = sysAuthService;
        _sysOAuthUserRep = sysOAuthUserRep;
        _wechatApiClientFactory = wechatApiClientFactory;
        _wechatApiClient = wechatApiClientFactory.CreateWechatClient();
    }

    /// <summary>
    /// 生成网页授权Url 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("生成网页授权Url")]
    public string GenAuthUrl(GenAuthUrlInput input)
    {
        return _wechatApiClient.GenerateParameterizedUrlForConnectOAuth2Authorize(input.RedirectUrl, input.Scope, input.State);
    }

    /// <summary>
    /// 获取微信用户OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("获取微信用户OpenId")]
    public async Task<string> SnsOAuth2([FromQuery] WechatOAuth2Input input)
    {
        var reqOAuth2 = new SnsOAuth2AccessTokenRequest()
        {
            Code = input.Code,
        };
        var resOAuth2 = await _wechatApiClient.ExecuteSnsOAuth2AccessTokenAsync(reqOAuth2);
        if (resOAuth2.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resOAuth2.ErrorMessage + " " + resOAuth2.ErrorCode);

        var wxUser = await _sysOAuthUserRep.GetFirstAsync(p => p.OpenId == resOAuth2.OpenId);
        if (wxUser == null)
        {
            var reqUserInfo = new SnsUserInfoRequest()
            {
                OpenId = resOAuth2.OpenId,
                AccessToken = resOAuth2.AccessToken,
            };
            var resUserInfo = await _wechatApiClient.ExecuteSnsUserInfoAsync(reqUserInfo);
            wxUser = resUserInfo.Adapt<SysOAuthUser>();
            wxUser.Avatar = resUserInfo.HeadImageUrl;
            wxUser.NickName = resUserInfo.Nickname;
            wxUser.OpenId = resOAuth2.OpenId;
            wxUser.UnionId = resOAuth2.UnionId;
            wxUser.AccessToken = resOAuth2.AccessToken;
            wxUser.RefreshToken = resOAuth2.RefreshToken;
            wxUser = await _sysOAuthUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            wxUser.AccessToken = resOAuth2.AccessToken;
            wxUser.RefreshToken = resOAuth2.RefreshToken;
            await _sysOAuthUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return resOAuth2.OpenId;
    }

    /// <summary>
    /// 微信用户登录OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("微信用户登录OpenId")]
    public async Task<dynamic> OpenIdLogin(WechatUserLogin input)
    {
        var wxUser = await _sysOAuthUserRep.AsQueryable().Includes(u => u.SysUser, u => u.SysOrg).FirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null) throw Oops.Oh("微信用户登录OpenId错误");
        if (wxUser.SysUser == null) throw Oops.Oh("微信用户未关联");
        var tokenInfo = await _sysAuthService.CreateToken(wxUser.SysUser);
        return new
        {
            wxUser.Avatar,
            accessToken = tokenInfo.AccessToken
        };
    }

    /// <summary>
    /// 获取配置签名参数(wx.config) 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取配置签名参数(wx.config)")]
    public async Task<dynamic> GenConfigPara(SignatureInput input)
    {
        string ticket = await _wechatApiClientFactory.TryGetWechatJsApiTicketAsync();
        return _wechatApiClient.GenerateParametersForJSSDKConfig(ticket, input.Url);
    }

    /// <summary>
    /// 获取模板列表 🔖
    /// </summary>
    [DisplayName("获取模板列表")]
    public async Task<dynamic> GetMessageTemplateList()
    {
        var accessToken = await GetCgibinToken();
        var reqTemplate = new CgibinTemplateGetAllPrivateTemplateRequest()
        {
            AccessToken = accessToken
        };
        var resTemplate = await _wechatApiClient.ExecuteCgibinTemplateGetAllPrivateTemplateAsync(reqTemplate);
        if (resTemplate.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resTemplate.ErrorMessage + " " + resTemplate.ErrorCode);

        return resTemplate.TemplateList;
    }

    /// <summary>
    /// 发送模板消息 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送模板消息")]
    public async Task<dynamic> SendTemplateMessage(MessageTemplateSendInput input)
    {
        var dataInfo = input.Data.ToDictionary(k => k.Key, k => k.Value);
        var messageData = new Dictionary<string, CgibinMessageTemplateSendRequest.Types.DataItem>();
        foreach (var item in dataInfo)
        {
            messageData.Add(item.Key, new CgibinMessageTemplateSendRequest.Types.DataItem() { Value = "" + item.Value.Value.ToString() + "" });
        }

        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinMessageTemplateSendRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId,
            ToUserOpenId = input.ToUserOpenId,
            Url = input.Url,
            MiniProgram = new CgibinMessageTemplateSendRequest.Types.MiniProgram
            {
                AppId = _wechatApiClientFactory._wechatOptions.WxOpenAppId,
                PagePath = input.MiniProgramPagePath,
            },
            Data = messageData
        };
        var resMessage = await _wechatApiClient.ExecuteCgibinMessageTemplateSendAsync(reqMessage);
        return resMessage;
    }

    /// <summary>
    /// 删除模板 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteMessageTemplate"), HttpPost]
    [DisplayName("删除模板")]
    public async Task<dynamic> DeleteMessageTemplate(DeleteMessageTemplateInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinTemplateDeletePrivateTemplateRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId
        };
        var resTemplate = await _wechatApiClient.ExecuteCgibinTemplateDeletePrivateTemplateAsync(reqMessage);
        return resTemplate;
    }

    /// <summary>
    /// 获取Access_token
    /// </summary>
    private async Task<string> GetCgibinToken()
    {
        return await _wechatApiClientFactory.TryGetWechatAccessTokenAsync();
    }
}