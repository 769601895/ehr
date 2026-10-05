// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建群聊会话输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2024/11/29
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/appchat/create?access_token=ACCESS_TOKEN
/// <br/>权限说明：只允许企业自建应用调用，且应用的可见范围必须是根部门
/// <br/>限制说明：群成员人数不可超过管理端配置的"群成员人数上限"，最大不可超过2000人，每企业创建群数不可超过1000/天
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90245
/// </remarks>
[HttpRemoteApi(Action = "appchat/create", Desc = "创建群聊会话", HttpMethod = HttpMethodEnum.Post)]
public class CreateChatWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 群聊名，最多50个utf8字符，超过将截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name")]
    [StringLength(50, ErrorMessage = "群聊名长度不能超过50个字符")]
    public string Name { get; set; }

    /// <summary>
    /// 指定群主的id。如果不指定，系统会随机从userlist中选一人作为群主
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [Required(ErrorMessage = "群主id不能为空")]
    [CustomJsonProperty("owner")]
    public string Owner { get; set; }

    /// <summary>
    /// 群成员id列表。至少2人，至多2000人
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userlist")]
    [NotEmpty(ErrorMessage = "群成员列表不能为空")]
    [MinLength(2, ErrorMessage = "群成员至少需要2人")]
    [MaxLength(2000, ErrorMessage = "群成员最多不能超过2000人")]
    public List<string> UserList { get; set; }

    /// <summary>
    /// 群聊的唯一标志，不能与已有的群重复；字符串类型，最长32个字符。只允许字符0-9及字母a-zA-Z。如果不填，系统会随机生成群id
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("chatid")]
    [RegularExpression("^[a-zA-Z0-9]{0,32}$", ErrorMessage = "群ID只能包含字母和数字，且长度不能超过32个字符")]
    public string ChatId { get; set; }
}

/// <summary>
/// 修改群聊会话输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/04/18
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/appchat/update?access_token=ACCESS_TOKEN
/// <br/>权限说明：只允许企业自建应用调用，且应用的可见范围必须是根部门
/// <br/>限制说明：chatid所代表的群必须是该应用所创建，群成员人数不可超过2000人，每企业变更群的次数不可超过1000次/小时
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/98913
/// </remarks>
[HttpRemoteApi(Action = "appchat/update", Desc = "修改群聊会话", HttpMethod = HttpMethodEnum.Post)]
public class UpdateChatWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 群聊id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("chatid")]
    [Required(ErrorMessage = "群聊ID不能为空")]
    public string ChatId { get; set; }

    /// <summary>
    /// 新的群聊名。若不需更新，请忽略此参数。最多50个utf8字符，超过将截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name")]
    [StringLength(50, ErrorMessage = "群聊名长度不能超过50个字符")]
    public string Name { get; set; }

    /// <summary>
    /// 新群主的id。若不需更新，请忽略此参数。课程群聊群主必须拥有课程群创建权限，del_user_list包含群主时本字段必填
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("owner")]
    public string Owner { get; set; }

    /// <summary>
    /// 添加成员的id列表
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("add_user_list")]
    [MaxLength(2000, ErrorMessage = "添加成员列表不能超过2000人")]
    public List<string> AddUserList { get; set; }

    /// <summary>
    /// 踢出成员的id列表
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("del_user_list")]
    [MaxLength(2000, ErrorMessage = "踢出成员列表不能超过2000人")]
    public List<string> DelUserList { get; set; }
}

/// <summary>
/// 获取群聊会话输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/04/20
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/appchat/get?access_token=ACCESS_TOKEN&amp;chatid=CHATID
/// <br/>权限说明：只允许企业自建应用调用，且应用的可见范围必须是根部门；chatid所代表的群必须是该应用所创建；第三方不可调用
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/98914
/// </remarks>
[HttpRemoteApi(Action = "appchat/get", Desc = "获取群聊会话", HttpMethod = HttpMethodEnum.Get)]
public class GetChatWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 群聊id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("chatid")]
    [Required(ErrorMessage = "群聊ID不能为空")]
    public string ChatId { get; set; }
}

/// <summary>
/// 应用消息推送输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2024/07/11
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/appchat/send?access_token=ACCESS_TOKEN
/// <br/>权限说明：只允许企业自建应用调用，且应用的可见范围必须是根部门
/// <br/>限制说明：chatid所代表的群必须是该应用所创建，消息发送量有限制，成员接收消息有限制
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90248
/// </remarks>
[HttpRemoteApi(Action = "appchat/send", Desc = "应用消息推送", HttpMethod = HttpMethodEnum.Post)]
public class SendChatWorkWxInput
{
}