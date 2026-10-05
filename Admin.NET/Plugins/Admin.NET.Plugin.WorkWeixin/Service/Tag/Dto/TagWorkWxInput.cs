// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建标签
/// </summary>
/// <remarks>
/// <br/>最后更新：2020/06/08
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/tag/create?access_token=ACCESS_TOKEN
/// <br/>权限说明：创建的标签属于该应用，只有该应用的secret才可以增删成员
/// <br/>限制说明：标签总数不能超过3000个
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90210
/// </remarks>
[HttpRemoteApi(Action = "tag/create", Desc = "创建标签", HttpMethod = HttpMethodEnum.Post)]
public class CreateTagWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签名称，长度限制为32个字以内（汉字或英文字母），标签名不可与其他标签重名
    /// </summary>
    [CustomJsonProperty("tagname")]
    [Required]
    [StringLength(32, MinimumLength = 1)]
    public string TagName { get; set; }

    /// <summary>
    /// 标签id，非负整型，指定此参数时新增的标签会生成对应的标签id，不指定时则以目前最大的id自增
    /// </summary>
    [CustomJsonProperty("tagid")]
    public long? TagId { get; set; }
}

/// <summary>
/// 更新标签名字
/// </summary>
/// <remarks>
/// <br/>最后更新：2020/04/02
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/tag/update?access_token=ACCESS_TOKEN
/// <br/>权限说明：调用的应用必须是指定标签的创建者
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90211
/// </remarks>
[HttpRemoteApi(Action = "tag/update", Desc = "更新标签名字", HttpMethod = HttpMethodEnum.Post)]
public class UpdateTagWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签ID
    /// </summary>
    [CustomJsonProperty("tagid")]
    [Required]
    public long TagId { get; set; }

    /// <summary>
    /// 标签名称，长度限制为32个字（汉字或英文字母），标签不可与其他标签重名
    /// </summary>
    [CustomJsonProperty("tagname")]
    [Required]
    [StringLength(32, MinimumLength = 1)]
    public string TagName { get; set; }
}

/// <summary>
/// 删除标签
/// </summary>
/// <remarks>
/// <br/>最后更新：2020/04/02
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/tag/delete?access_token=ACCESS_TOKEN&amp;tagid=TAGID
/// <br/>权限说明：调用的应用必须是指定标签的创建者
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90212
/// </remarks>
[HttpRemoteApi(Action = "tag/delete", Desc = "删除标签", HttpMethod = HttpMethodEnum.Get)]
public class DeleteTagWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签ID
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("tagid")]
    [Required(ErrorMessage = "标签ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "标签ID必须大于0")]
    public long TagId { get; set; }
}

/// <summary>
/// 获取标签成员
/// </summary>
[HttpRemoteApi(Action = "tag/get", Desc = "获取标签成员", HttpMethod = HttpMethodEnum.Get)]
public class TagMembersWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签ID
    /// </summary>
    [CustomJsonProperty("tagid")]
    [Required]
    public long TagId { get; set; }
}

/// <summary>
/// 增加标签成员
/// </summary>
[HttpRemoteApi(Action = "tag/addtagusers", Desc = "增加标签成员", HttpMethod = HttpMethodEnum.Post)]
public class AddTagMembersWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签ID
    /// </summary>
    [CustomJsonProperty("tagid")]
    [Required]
    public long TagId { get; set; }

    /// <summary>
    /// 企业成员ID列表
    /// </summary>
    [CustomJsonProperty("userlist")]
    [MaxLength(1000)]
    public List<string> UserList { get; set; }

    /// <summary>
    /// 企业部门ID列表
    /// </summary>
    [CustomJsonProperty("partylist")]
    [MaxLength(100)]
    public List<long> PartyList { get; set; }
}

/// <summary>
/// 删除标签成员
/// </summary>
[HttpRemoteApi(Action = "tag/deltagusers", Desc = "删除标签成员", HttpMethod = HttpMethodEnum.Post)]
public class DeleteTagMembersWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 标签ID
    /// </summary>
    [CustomJsonProperty("tagid")]
    [Required]
    public long TagId { get; set; }

    /// <summary>
    /// 企业成员ID列表
    /// </summary>
    [CustomJsonProperty("userlist")]
    [MaxLength(1000)]
    public List<string> UserList { get; set; }

    /// <summary>
    /// 企业部门ID列表
    /// </summary>
    [CustomJsonProperty("partylist")]
    [MaxLength(100)]
    public List<long> PartyList { get; set; }
}

/// <summary>
/// 获取标签列表
/// </summary>
[HttpRemoteApi(Action = "tag/list", Desc = "获取标签列表", HttpMethod = HttpMethodEnum.Get)]
public class TagListWorkWxInput : AuthWorkWxInput
{
}