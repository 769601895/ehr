// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Plugin.WorkWeixin.Const;
using Furion.FriendlyException;
using Furion.Logging;
using System.Reflection;
using System.Text;

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信接口基类服务 🧩
/// </summary>
public class WorkWxBaseService(
    SysCacheService sysCacheService,
    SysConfigService sysConfigService,
    IHttpRemoteService httpRemoteService) : ITransient
{
    private static readonly Lazy<HttpRemoteItem> Options = new(() => App.GetConfig<HttpRemoteItem>("HttpRemotes:WorkWeixin", true));

    /// <summary>
    /// 获取企业微信接口凭证
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetTokenAsync()
    {
        using var disposable = sysCacheService.BeginCacheLock(WorkWeixinConst.KeyLockWorkWeixin);
        var result = await SendAsync<TokenWorkWxInput, TokenWorkWxOutput>(new()
        {
            CorpId = await sysConfigService.GetConfigValueByCode(WorkWeixinConst.WorkWeixinCorpId),
            CorpSecret = await sysConfigService.GetConfigValueByCode(WorkWeixinConst.WorkWeixinCorpSecret)
        });
        if (string.IsNullOrWhiteSpace(result.AccessToken))
        {
            Log.Error("[企业微信] 获取接口凭证失败");
            return null;
        }
        sysCacheService.Set(WorkWeixinConst.KeyWorkWeixinToken, result.AccessToken, TimeSpan.FromSeconds(result.ExpiresIn));
        return result.AccessToken;
    }

    /// <summary>
    /// 发起请求
    /// </summary>
    /// <typeparam name="T">输入类型</typeparam>
    /// <typeparam name="R">返回类型</typeparam>
    /// <param name="input">输入参数</param>
    /// <returns>返回结果</returns>
    public async Task<R> SendAsync<T, R>(T input) where R : BaseWorkWxOutput
    {
        if (Options.Value == null || string.IsNullOrWhiteSpace(Options.Value.BaseAddress)) throw Oops.Oh("[企业微信] 服务配置缺失");

        var attr = typeof(T).GetCustomAttribute<HttpRemoteApiAttribute>();
        if (attr == null || string.IsNullOrWhiteSpace(attr.Action as string))
            throw Oops.Oh("[企业微信] 接口入参未正确配置[HttpRemoteApi]特性");

        // 拼接请求地址，并设置token
        var url = Options.Value.BaseAddress + $"/cgi-bin/{attr.Action}?";
        if (input is AuthWorkWxInput)
        {
            // 重试3次
            var retryMax = 3;
            var token = sysCacheService.Get<string>(WorkWeixinConst.KeyWorkWeixinToken) ?? await GetTokenAsync();
            while (retryMax-- > 0 && string.IsNullOrWhiteSpace(token))
            {
                token = await GetTokenAsync();
                await Task.Delay(1500);
            }
            url += "access_token=" + token;
        }

        HttpResponseMessage response;
        try
        {
            response = attr.HttpMethod switch
            {
                HttpMethodEnum.Get => await httpRemoteService.GetAsync(
                    url + input.ToCustomJsonPropertyQueryString(),
                    builder => builder.SetHttpClientName(Options.Value.HttpName).SetReqRemoteApiAttr(attr)),
                HttpMethodEnum.Post => await httpRemoteService.PostAsync(url,
                    builder => builder.SetHttpClientName(Options.Value.HttpName).SetReqRemoteApiAttr(attr)
                        .SetContent(new StringContent(CustomJsonHelper.Serialize(input), Encoding.UTF8, "application/json"))),
                _ => throw Oops.Oh($"[企业微信] 不支持的请求方式：{attr.HttpMethod.ToString()}：({typeof(T).FullName})"),
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw Oops.Oh("[企业微信] 服务不可用，请检查网路是否正常：" + ex.Message);
        }
        return await HandleHttpResponseAsync<R>(response);
    }

    /// <summary>
    /// 处理HTTP响应
    /// </summary>
    /// <param name="respMsg"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<R> HandleHttpResponseAsync<R>(HttpResponseMessage respMsg) where R : BaseWorkWxOutput
    {
        if (!respMsg.IsSuccessStatusCode) throw Oops.Oh("[企业微信] 请求失败");

        var responseContent = await respMsg.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent)) throw Oops.Oh("[企业微信] 响应体为空");

        try
        {
            var resp = CustomJsonHelper.Deserialize<R>(responseContent);
            if (resp?.ErrCode == 0) return resp;
            throw Oops.Oh("[企业微信] 请求失败：" + resp?.ErrMsg);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw Oops.Oh((ex is AppFriendlyException ? "" : "[企业微信] 序列化失败：") + ex.Message);
        }
    }
}