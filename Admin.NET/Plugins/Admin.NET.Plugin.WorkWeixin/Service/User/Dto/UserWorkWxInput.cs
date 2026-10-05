// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建成员输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2025/07/25
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token=ACCESS_TOKEN
/// <br/>权限说明：仅通讯录同步助手或第三方通讯录应用可调用
/// <br/>限制说明：每个部门下的部门、成员总数不能超过3万个
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90195
/// </remarks>
[HttpRemoteApi(Action = "user/create", Desc = "创建成员", HttpMethod = HttpMethodEnum.Post)]
public class CreateUserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号，企业内必须唯一。长度为1~64个字节。只能由数字、字母和"_-@."四种字符组成，且第一个字符必须是数字或字母。
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9_\\-@.]*$", ErrorMessage = "用户ID只能由数字、字母和_-@.组成，且必须以数字或字母开头")]
    public string UserId { get; set; }

    /// <summary>
    /// 成员名称。长度为1~64个utf8字符
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("name")]
    [Required(ErrorMessage = "成员名称不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "成员名称长度必须在1-64个字符之间")]
    public string Name { get; set; }

    /// <summary>
    /// 成员别名。长度1~64个utf8字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("alias")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "别名长度必须在1-64个字符之间")]
    public string Alias { get; set; }

    /// <summary>
    /// 手机号码。企业内必须唯一，mobile/email二者不能同时为空
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("mobile")]
    public string Mobile { get; set; }

    /// <summary>
    /// 邮箱。企业内必须唯一，mobile/email二者不能同时为空
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("email")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "邮箱长度必须在6-64个字节之间")]
    public string Email { get; set; }

    /// <summary>
    /// 成员所属部门id列表，不超过100个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("department")]
    [MaxLength(100, ErrorMessage = "部门数量不能超过100个")]
    public List<long> Department { get; set; }

    /// <summary>
    /// 部门内的排序值，个数必须和参数department的个数一致
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("order")]
    public List<long> Order { get; set; }

    /// <summary>
    /// 职务信息。长度为0~128个字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("position")]
    [StringLength(128, ErrorMessage = "职务信息长度不能超过128个字符")]
    public string Position { get; set; }

    /// <summary>
    /// 性别。1表示男性，2表示女性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("gender")]
    [Range(1, 2, ErrorMessage = "性别只能是1(男)或2(女)")]
    public int? Gender { get; set; }

    /// <summary>
    /// 企业邮箱账号
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("biz_mail")]
    [EmailAddress(ErrorMessage = "企业邮箱格式不正确")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "企业邮箱长度必须在6-64个字节之间")]
    public string BizMail { get; set; }

    /// <summary>
    /// 座机。32字节以内，由纯数字、"-"、"+"或","组成
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("telephone")]
    [StringLength(32, ErrorMessage = "座机号码长度不能超过32个字节")]
    [RegularExpression("^[0-9\\-\\+,]*$", ErrorMessage = "座机号码只能包含数字、-、+或,")]
    public string Telephone { get; set; }

    /// <summary>
    /// 在所在的部门内是否为部门负责人。1表示为部门负责人，0表示非部门负责人
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("is_leader_in_dept")]
    public List<int> IsLeaderInDept { get; set; }

    /// <summary>
    /// 直属上级UserID，设置范围为企业内成员，可以设置最多1个上级
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("direct_leader")]
    [MaxLength(1, ErrorMessage = "直属上级最多只能设置1个")]
    public List<string> DirectLeader { get; set; }

    /// <summary>
    /// 成员头像的mediaid，通过素材管理接口上传图片获得的mediaid
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("avatar_mediaid")]
    public string AvatarMediaId { get; set; }

    /// <summary>
    /// 启用/禁用成员。1表示启用成员，0表示禁用成员
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("enable")]
    [Range(0, 1, ErrorMessage = "启用状态只能是0(禁用)或1(启用)")]
    public int? Enable { get; set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("extattr")]
    public ExtAttrDto ExtAttr { get; set; }

    /// <summary>
    /// 是否邀请该成员使用企业微信，默认值为true
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("to_invite")]
    public bool? ToInvite { get; set; }

    /// <summary>
    /// 对外职务，长度12个汉字内
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("external_position")]
    [StringLength(36, ErrorMessage = "对外职务长度不能超过12个汉字")]
    public string ExternalPosition { get; set; }

    /// <summary>
    /// 成员对外属性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("external_profile")]
    public ExternalProfileDto ExternalProfile { get; set; }

    /// <summary>
    /// 地址。长度最大128个字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("address")]
    [StringLength(128, ErrorMessage = "地址长度不能超过128个字符")]
    public string Address { get; set; }

    /// <summary>
    /// 主部门
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("main_department")]
    public long? MainDepartment { get; set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public class ExtAttrDto
    {
        /// <summary>
        /// 扩展属性列表
        /// </summary>
        [CustomJsonProperty("attrs")]
        public List<ExtAttrItemDto> Attrs { get; set; }
    }

    /// <summary>
    /// 扩展属性项
    /// </summary>
    public class ExtAttrItemDto
    {
        /// <summary>
        /// 属性类型：0-文本，1-网页
        /// </summary>
        [CustomJsonProperty("type")]
        [Range(0, 1, ErrorMessage = "扩展属性类型只能是0(文本)或1(网页)")]
        public int Type { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [CustomJsonProperty("name")]
        [Required(ErrorMessage = "扩展属性名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 文本属性值（当type=0时使用）
        /// </summary>
        [CustomJsonProperty("text")]
        public ExtAttrTextDto Text { get; set; }

        /// <summary>
        /// 网页属性值（当type=1时使用）
        /// </summary>
        [CustomJsonProperty("web")]
        public ExtAttrWebDto Web { get; set; }
    }

    /// <summary>
    /// 扩展属性文本值
    /// </summary>
    public class ExtAttrTextDto
    {
        /// <summary>
        /// 文本值
        /// </summary>
        [CustomJsonProperty("value")]
        [Required(ErrorMessage = "文本值不能为空")]
        public string Value { get; set; }
    }

    /// <summary>
    /// 扩展属性网页值
    /// </summary>
    public class ExtAttrWebDto
    {
        /// <summary>
        /// 网页链接
        /// </summary>
        [CustomJsonProperty("url")]
        [Required(ErrorMessage = "网页链接不能为空")]
        [Url(ErrorMessage = "网页链接格式不正确")]
        public string Url { get; set; }

        /// <summary>
        /// 网页标题
        /// </summary>
        [CustomJsonProperty("title")]
        [Required(ErrorMessage = "网页标题不能为空")]
        public string Title { get; set; }
    }

    /// <summary>
    /// 成员对外属性
    /// </summary>
    public class ExternalProfileDto
    {
        /// <summary>
        /// 企业简称
        /// </summary>
        [CustomJsonProperty("external_corp_name")]
        public string ExternalCorpName { get; set; }

        /// <summary>
        /// 视频号信息
        /// </summary>
        [CustomJsonProperty("wechat_channels")]
        public WechatChannelsDto WechatChannels { get; set; }

        /// <summary>
        /// 对外属性列表
        /// </summary>
        [CustomJsonProperty("external_attr")]
        public List<ExternalAttrItemDto> ExternalAttr { get; set; }
    }

    /// <summary>
    /// 视频号信息
    /// </summary>
    public class WechatChannelsDto
    {
        /// <summary>
        /// 视频号名称
        /// </summary>
        [CustomJsonProperty("nickname")]
        public string Nickname { get; set; }
    }

    /// <summary>
    /// 对外属性项
    /// </summary>
    public class ExternalAttrItemDto
    {
        /// <summary>
        /// 属性类型：0-文本，1-网页，2-小程序
        /// </summary>
        [CustomJsonProperty("type")]
        [Range(0, 2, ErrorMessage = "对外属性类型只能是0(文本)、1(网页)或2(小程序)")]
        public int Type { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [CustomJsonProperty("name")]
        [Required(ErrorMessage = "对外属性名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 文本属性值（当type=0时使用）
        /// </summary>
        [CustomJsonProperty("text")]
        public ExtAttrTextDto Text { get; set; }

        /// <summary>
        /// 网页属性值（当type=1时使用）
        /// </summary>
        [CustomJsonProperty("web")]
        public ExtAttrWebDto Web { get; set; }

        /// <summary>
        /// 小程序属性值（当type=2时使用）
        /// </summary>
        [CustomJsonProperty("miniprogram")]
        public ExternalAttrMiniprogramDto Miniprogram { get; set; }
    }

    /// <summary>
    /// 对外属性小程序值
    /// </summary>
    public class ExternalAttrMiniprogramDto
    {
        /// <summary>
        /// 小程序appid
        /// </summary>
        [CustomJsonProperty("appid")]
        [Required(ErrorMessage = "小程序appid不能为空")]
        public string AppId { get; set; }

        /// <summary>
        /// 小程序页面路径
        /// </summary>
        [CustomJsonProperty("pagepath")]
        [Required(ErrorMessage = "小程序页面路径不能为空")]
        public string PagePath { get; set; }

        /// <summary>
        /// 小程序标题
        /// </summary>
        [CustomJsonProperty("title")]
        [Required(ErrorMessage = "小程序标题不能为空")]
        public string Title { get; set; }
    }
}

/// <summary>
/// 读取成员输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2025/03/06
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=ACCESS_TOKEN&amp;userid=USERID
/// <br/>权限说明：应用须拥有指定成员的查看权限
/// <br/>限制说明：应用只能获取可见范围内的成员信息，且每种应用获取的字段有所不同
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90196
/// </remarks>
[HttpRemoteApi(Action = "user/get", Desc = "读取成员", HttpMethod = HttpMethodEnum.Get)]
public class UserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号，企业内必须唯一。不区分大小写，长度为1~64个字节
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    public string UserId { get; set; }
}

/// <summary>
/// 更新成员输入参数
/// </summary>
/// <remarks>
/// <br/>最后更新：2025/07/25
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token=ACCESS_TOKEN
/// <br/>权限说明：仅通讯录同步助手或第三方通讯录应用可调用
/// <br/>限制说明：每个部门下的部门、成员总数不能超过3万个
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90197
/// </remarks>
[HttpRemoteApi(Action = "user/update", Desc = "更新成员", HttpMethod = HttpMethodEnum.Post)]
public class UpdateUserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号，企业内必须唯一。不区分大小写，长度为1~64个字节
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9_\\-@.]*$", ErrorMessage = "用户ID只能由数字、字母和_-@.组成，且必须以数字或字母开头")]
    public string UserId { get; set; }

    /// <summary>
    /// 成员名称。长度为1~64个utf8字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "成员名称长度必须在1-64个字符之间")]
    public string Name { get; set; }

    /// <summary>
    /// 别名。长度为1-64个utf8字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("alias")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "别名长度必须在1-64个字符之间")]
    public string Alias { get; set; }

    /// <summary>
    /// 手机号码。企业内必须唯一
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("mobile")]
    public string Mobile { get; set; }

    /// <summary>
    /// 邮箱。企业内必须唯一
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("email")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "邮箱长度必须在6-64个字节之间")]
    public string Email { get; set; }

    /// <summary>
    /// 企业邮箱账号。长度6~63个字节，且为有效的企业邮箱格式
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("biz_mail")]
    [EmailAddress(ErrorMessage = "企业邮箱格式不正确")]
    [StringLength(63, MinimumLength = 6, ErrorMessage = "企业邮箱长度必须在6-63个字节之间")]
    public string BizMail { get; set; }

    /// <summary>
    /// 企业邮箱别名。最多可设置5个别名，更新时为覆盖式更新
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("biz_mail_alias")]
    public BizMailAliasDto BizMailAlias { get; set; }

    /// <summary>
    /// 成员所属部门id列表，不超过100个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("department")]
    [MaxLength(100, ErrorMessage = "部门数量不能超过100个")]
    public List<long> Department { get; set; }

    /// <summary>
    /// 部门内的排序值，数量必须和department一致
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("order")]
    public List<long> Order { get; set; }

    /// <summary>
    /// 职务信息。长度为0~128个utf8字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("position")]
    [StringLength(128, ErrorMessage = "职务信息长度不能超过128个字符")]
    public string Position { get; set; }

    /// <summary>
    /// 性别。1表示男性，2表示女性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("gender")]
    [Range(1, 2, ErrorMessage = "性别只能是1(男)或2(女)")]
    public int? Gender { get; set; }

    /// <summary>
    /// 座机。由1-32位的纯数字、"-"、"+"或","组成
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("telephone")]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "座机号码长度必须在1-32个字节之间")]
    [RegularExpression("^[0-9\\-\\+,]*$", ErrorMessage = "座机号码只能包含数字、-、+或,")]
    public string Telephone { get; set; }

    /// <summary>
    /// 部门负责人字段，个数必须和department一致，表示在所在的部门内是否为负责人。0-否，1-是
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("is_leader_in_dept")]
    public List<int> IsLeaderInDept { get; set; }

    /// <summary>
    /// 直属上级，可以设置企业范围内成员为直属上级，最多设置1个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("direct_leader")]
    [MaxLength(1, ErrorMessage = "直属上级最多只能设置1个")]
    public List<string> DirectLeader { get; set; }

    /// <summary>
    /// 成员头像的mediaid，通过素材管理接口上传图片获得的mediaid
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("avatar_mediaid")]
    public string AvatarMediaId { get; set; }

    /// <summary>
    /// 启用/禁用成员。1表示启用成员，0表示禁用成员
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("enable")]
    [Range(0, 1, ErrorMessage = "启用状态只能是0(禁用)或1(启用)")]
    public int? Enable { get; set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("extattr")]
    public CreateUserWorkWxInput.ExtAttrDto ExtAttr { get; set; }

    /// <summary>
    /// 对外职务，不超过12个汉字
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("external_position")]
    [StringLength(36, ErrorMessage = "对外职务长度不能超过12个汉字")]
    public string ExternalPosition { get; set; }

    /// <summary>
    /// 成员对外属性
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("external_profile")]
    public CreateUserWorkWxInput.ExternalProfileDto ExternalProfile { get; set; }

    /// <summary>
    /// 地址。长度最大128个字符
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("address")]
    [StringLength(128, ErrorMessage = "地址长度不能超过128个字符")]
    public string Address { get; set; }

    /// <summary>
    /// 主部门
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("main_department")]
    public long? MainDepartment { get; set; }

    /// <summary>
    /// 新的用户ID（如果userid由系统自动生成，则仅允许修改一次）
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("new_userid")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "新用户ID长度必须在1-64个字节之间")]
    [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9_\\-@.]*$", ErrorMessage = "新用户ID只能由数字、字母和_-@.组成，且必须以数字或字母开头")]
    public string NewUserId { get; set; }

    /// <summary>
    /// 验证部门相关字段的一致性
    /// </summary>
    public bool ValidateDepartmentFields()
    {
        if (Department != null && Order != null && Department.Count != Order.Count)
        {
            return false;
        }

        if (Department != null && IsLeaderInDept != null && Department.Count != IsLeaderInDept.Count)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取验证错误信息
    /// </summary>
    public string GetValidationErrors()
    {
        if (Department != null && Order != null && Department.Count != Order.Count)
        {
            return "部门数量和排序值数量不一致";
        }

        if (Department != null && IsLeaderInDept != null && Department.Count != IsLeaderInDept.Count)
        {
            return "部门数量和负责人标识数量不一致";
        }

        return null;
    }

    /// <summary>
    /// 企业邮箱别名
    /// </summary>
    public class BizMailAliasDto
    {
        /// <summary>
        /// 企业邮箱别名列表，最多可设置5个别名
        /// </summary>
        [CustomJsonProperty("item")]
        [MaxLength(5, ErrorMessage = "企业邮箱别名最多只能设置5个")]
        public List<string> Items { get; set; } = new List<string>();

        /// <summary>
        /// 清空所有别名
        /// </summary>
        public void Clear()
        {
            Items?.Clear();
        }

        /// <summary>
        /// 添加别名
        /// </summary>
        public bool AddAlias(string alias)
        {
            if (Items == null) Items = new List<string>();
            if (Items.Count >= 5) return false;

            Items.Add(alias);
            return true;
        }

        /// <summary>
        /// 移除别名
        /// </summary>
        public bool RemoveAlias(string alias)
        {
            return Items?.Remove(alias) ?? false;
        }
    }
}

/// <summary>
/// 删除成员
/// </summary>
/// <remarks>
/// <br/>最后更新：2021/08/09
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token=ACCESS_TOKEN&amp;userid=USERID
/// <br/>权限说明：仅通讯录同步助手或第三方通讯录应用可调用
/// <br/>特殊说明：若是绑定了腾讯企业邮，则会同时删除邮箱账号
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90198
/// </remarks>
[HttpRemoteApi(Action = "user/delete", Desc = "删除成员", HttpMethod = HttpMethodEnum.Get)]
public class DeleteUserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    public string UserId { get; set; }
}

/// <summary>
/// 批量删除成员
/// </summary>
/// <remarks>
/// <br/>最后更新：2019/06/13
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token=ACCESS_TOKEN
/// <br/>权限说明：仅通讯录同步助手或第三方通讯录应用可调用
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90199
/// </remarks>
[HttpRemoteApi(Action = "user/batchdelete", Desc = "批量删除成员", HttpMethod = HttpMethodEnum.Post)]
public class BatchDeleteUserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID列表。对应管理端的账号。最多支持200个。若存在无效UserID，直接返回错误
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("useridlist")]
    [Required(ErrorMessage = "用户ID列表不能为空")]
    [MinLength(1, ErrorMessage = "至少需要指定一个用户ID")]
    [MaxLength(200, ErrorMessage = "单次最多只能删除200个用户")]
    public List<string> UserIdList { get; set; }
}

/// <summary>
/// 获取部门成员
/// </summary>
/// <remarks>
/// <br/>最后更新：2022/08/22
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token=ACCESS_TOKEN&amp;department_id=DEPARTMENT_ID
/// <br/>权限说明：应用须拥有指定部门的查看权限
/// <br/>限制说明：如需获取该部门及其子部门的所有成员，需逐层递归获取
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90200
/// </remarks>
[HttpRemoteApi(Action = "user/simplelist", Desc = "获取部门成员", HttpMethod = HttpMethodEnum.Get)]
public class DeptUserSimpleListWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 获取的部门id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("department_id")]
    [Required(ErrorMessage = "部门ID不能为空")]
    public long DepartmentId { get; set; }

    /// <summary>
    /// 是否递归获取子部门成员
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("fetch_child")]
    public bool? FetchChild { get; set; }
}

/// <summary>
/// 获取部门成员详情
/// </summary>
/// <remarks>
/// <br/>最后更新：2024/07/24
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token=ACCESS_TOKEN&amp;department_id=DEPARTMENT_ID
/// <br/>权限说明：应用须拥有指定部门的查看权限
/// <br/>限制说明：应用只能获取可见范围内的成员信息，且每种应用获取的字段有所不同
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90201
/// </remarks>
[HttpRemoteApi(Action = "user/list", Desc = "获取部门成员详情", HttpMethod = HttpMethodEnum.Get)]
public class DeptUserDetailListWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 获取的部门id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("department_id")]
    [Required(ErrorMessage = "部门ID不能为空")]
    public long DepartmentId { get; set; }

    /// <summary>
    /// 是否递归获取子部门成员
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("fetch_child")]
    public bool? FetchChild { get; set; }
}

/// <summary>
/// userid转openid
/// </summary>
/// <remarks>
/// <br/>最后更新：2022/01/20
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_openid?access_token=ACCESS_TOKEN
/// <br/>权限说明：成员必须处于应用的可见范围内
/// <br/>使用场景：企业支付，在使用企业红包和向员工付款时使用
/// <br/>限制说明：需要成员使用微信登录企业微信或者关注微信插件才能转成openid
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90202
/// </remarks>
[HttpRemoteApi(Action = "user/convert_to_openid", Desc = "userid转openid", HttpMethod = HttpMethodEnum.Post)]
public class ConvToOpenIdWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 企业内的成员id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    public string UserId { get; set; }
}

/// <summary>
/// openid转userid
/// </summary>
/// <remarks>
/// <br/>最后更新：2022/01/20
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_userid?access_token=ACCESS_TOKEN
/// <br/>权限说明：管理组需对openid对应的企业微信成员有查看权限
/// <br/>使用场景：企业支付之后的结果查询
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90202
/// </remarks>
[HttpRemoteApi(Action = "user/convert_to_userid", Desc = "openid转userid", HttpMethod = HttpMethodEnum.Post)]
public class ConvToUserIdWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 在使用企业支付之后，返回结果的openid
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("openid")]
    [Required(ErrorMessage = "OpenID不能为空")]
    public string OpenId { get; set; }
}

/// <summary>
/// 登录二次验证
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/11/15
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/authsucc?access_token=ACCESS_TOKEN&amp;userid=USERID
/// <br/>使用场景：安全性要求高的企业进行成员验证，开启二次验证后使用
/// <br/>流程说明：成员登录→跳转企业验证页面→获取code→根据code获取userid→调用本接口完成验证
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90203
/// </remarks>
[HttpRemoteApi(Action = "user/authsucc", Desc = "登录二次验证", HttpMethod = HttpMethodEnum.Get)]
public class UserAuthSuccessWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("userid")]
    [Required(ErrorMessage = "用户ID不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "用户ID长度必须在1-64个字节之间")]
    public string UserId { get; set; }
}

/// <summary>
/// 邀请成员
/// </summary>
/// <remarks>
/// <br/>最后更新：2024/11/04
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/batch/invite?access_token=ACCESS_TOKEN
/// <br/>权限说明：须拥有指定成员、部门或标签的查看权限，第三方仅通讯录应用可调用
/// <br/>限制说明：user, party, tag三者不能同时为空；邀请频率有限制
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90213
/// </remarks>
[HttpRemoteApi(Action = "batch/invite", Desc = "邀请成员", HttpMethod = HttpMethodEnum.Post)]
public class InviteUserWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 成员ID列表, 最多支持1000个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("user")]
    [MaxLength(1000, ErrorMessage = "成员ID列表最多支持1000个")]
    public List<string> User { get; set; }

    /// <summary>
    /// 部门ID列表，最多支持100个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("party")]
    [MaxLength(100, ErrorMessage = "部门ID列表最多支持100个")]
    public List<long> Party { get; set; }

    /// <summary>
    /// 标签ID列表，最多支持100个
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("tag")]
    [MaxLength(100, ErrorMessage = "标签ID列表最多支持100个")]
    public List<long> Tag { get; set; }

    /// <summary>
    /// 验证邀请参数
    /// </summary>
    public (bool IsValid, string ErrorMessage) Validate()
    {
        if ((User == null || User.Count == 0) &&
            (Party == null || Party.Count == 0) &&
            (Tag == null || Tag.Count == 0))
        {
            return (false, "user, party, tag三者不能同时为空");
        }

        return (true, null);
    }
}

/// <summary>
/// 获取加入企业二维码
/// </summary>
/// <remarks>
/// <br/>最后更新：2019/11/30
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/corp/get_join_qrcode?access_token=ACCESS_TOKEN&amp;size_type=SIZE_TYPE
/// <br/>权限说明：须拥有通讯录的管理权限，使用通讯录同步的Secret
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90214
/// </remarks>
[HttpRemoteApi(Action = "corp/get_join_qrcode", Desc = "获取加入企业二维码", HttpMethod = HttpMethodEnum.Get)]
public class JoinQrcodeWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// qrcode尺寸类型，1: 171 x 171; 2: 399 x 399; 3: 741 x 741; 4: 2052 x 2052
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("size_type")]
    [Range(1, 4, ErrorMessage = "尺寸类型必须在1-4之间")]
    public int? SizeType { get; set; } = 3;
}

/// <summary>
/// 手机号获取userid
/// </summary>
/// <remarks>
/// <br/>最后更新：2022/08/16
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/getuserid?access_token=ACCESS_TOKEN
/// <br/>权限说明：应用须拥有指定成员的查看权限
/// <br/>限制说明：请确保手机号的正确性，若出错的次数超出企业规模人数的20%，会导致1天不可调用
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/95402
/// </remarks>
[HttpRemoteApi(Action = "user/getuserid", Desc = "手机号获取userid", HttpMethod = HttpMethodEnum.Post)]
public class UserIdByMobileWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 用户在企业微信通讯录中的手机号码。长度为5~32个字节
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("mobile")]
    [Required(ErrorMessage = "手机号不能为空")]
    [StringLength(32, MinimumLength = 5, ErrorMessage = "手机号长度必须在5-32个字节之间")]
    public string Mobile { get; set; }
}

/// <summary>
/// 邮箱获取userid
/// </summary>
/// <remarks>
/// <br/>最后更新：2022/07/19
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/get_userid_by_email?access_token=ACCESS_TOKEN
/// <br/>权限说明：应用须拥有指定成员的查看权限
/// <br/>限制说明：请确保邮箱的正确性，若出错的次数较多，会导致1天不可调用
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/95892
/// </remarks>
[HttpRemoteApi(Action = "user/get_userid_by_email", Desc = "邮箱获取userid", HttpMethod = HttpMethodEnum.Post)]
public class UserIdByEmailWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("email")]
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; }

    /// <summary>
    /// 邮箱类型：1-企业邮箱（默认）；2-个人邮箱
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("email_type")]
    [Range(1, 2, ErrorMessage = "邮箱类型只能是1(企业邮箱)或2(个人邮箱)")]
    public int? EmailType { get; set; } = 1;

    /// <summary>
    /// 邮箱类型描述
    /// </summary>
    public string EmailTypeDesc => EmailType switch
    {
        1 => "企业邮箱",
        2 => "个人邮箱",
        _ => "未知类型"
    };
}

/// <summary>
/// 获取成员ID列表
/// </summary>
/// <remarks>
/// <br/>最后更新：2024/07/23
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/list_id?access_token=ACCESS_TOKEN
/// <br/>权限说明：仅支持通过"通讯录同步secret"调用
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/96067
/// </remarks>
[HttpRemoteApi(Action = "user/list_id", Desc = "获取成员ID列表", HttpMethod = HttpMethodEnum.Post)]
public class UserIdListWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 用于分页查询的游标，字符串类型，由上一次调用返回，首次调用不填
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("cursor")]
    public string Cursor { get; set; }

    /// <summary>
    /// 分页，预期请求的数据量，取值范围 1 ~ 10000
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("limit")]
    [Range(1, 10000, ErrorMessage = "分页大小必须在1-10000之间")]
    public int? Limit { get; set; } = 1000;
}