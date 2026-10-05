// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 需授权基类，继承此类，自动追加令牌
/// </summary>
public class AuthWorkWxInput
{
}

/// <summary>
/// 企业微信接口输出基类
/// </summary>
public class BaseWorkWxOutput
{
    /// <summary>
    /// 返回码
    /// </summary>
    [CustomJsonProperty("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// 对返回码的文本描述内容
    /// </summary>
    [CustomJsonProperty("errmsg")]
    public string ErrMsg { get; set; }
}

/// <summary>
/// 企业微信接口输出基类
/// </summary>
public class BasePageWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 分页游标，下次请求时填写以获取之后分页的记录。如果该字段返回空则表示已没有更多数据
    /// </summary>
    [CustomJsonProperty("next_cursor")]
    public string NextCursor { get; set; }
}

/// <summary>
/// 带id的输出参数
/// </summary>
public class BaseIdWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// id
    /// </summary>
    [CustomJsonProperty("id")]
    public long? Id { get; set; }
}

/// <summary>
/// 获取接口凭证输入参数
/// </summary>
/// https://developer.work.weixin.qq.com/document/path/91039
[HttpRemoteApi(Action = "gettoken", Desc = "获取接口凭证", HttpMethod = HttpMethodEnum.Get, IgnoreLog = true)]
public class TokenWorkWxInput
{
    /// <summary>
    /// 企业标识
    /// </summary>
    [CustomJsonProperty("corpid")]
    public string CorpId { get; set; }

    /// <summary>
    /// 企业密钥
    /// </summary>
    [CustomJsonProperty("corpsecret")]
    public string CorpSecret { get; set; }
}

/// <summary>
/// 获取接口凭证输出参数
/// </summary>
public class TokenWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 获取到的凭证
    /// </summary>
    [CustomJsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// 凭证的有效时间（秒）
    /// </summary>
    [CustomJsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}