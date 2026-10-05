// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建部门
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/08/15
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token=ACCESS_TOKEN
/// <br/>权限说明：第三方仅通讯录应用可以调用
/// <br/>限制说明：部门最大层级15层，部门总数不超过3万个，每个部门下节点不超过3万个
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90205
/// </remarks>
[HttpRemoteApi(Action = "department/create", Desc = "创建部门", HttpMethod = HttpMethodEnum.Post)]
public class CreateDeptWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 部门名称。同一个层级的部门名称不能重复。长度限制为1~64个UTF-8字符，字符不能包括\:*?"&lt;&gt;|
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("name")]
    [Required(ErrorMessage = "部门名称不能为空")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "部门名称长度必须在1-64个字符之间")]
    [RegularExpression(@"^[^\\:*?""<>|]+$", ErrorMessage = "部门名称不能包含\\:*?\"<>|字符")]
    public string Name { get; set; }

    /// <summary>
    /// 英文名称。同一个层级的部门名称不能重复。需要在管理后台开启多语言支持才能生效
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name_en")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "英文名称长度必须在1-64个字符之间")]
    [RegularExpression(@"^[^\\:*?""<>|]*$", ErrorMessage = "英文名称不能包含\\:*?\"<>|字符")]
    public string NameEn { get; set; }

    /// <summary>
    /// 父部门id，32位整型
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("parentid")]
    [Required(ErrorMessage = "父部门ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "父部门ID必须大于0")]
    public long ParentId { get; set; }

    /// <summary>
    /// 在父部门中的次序值。order值大的排序靠前
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("order")]
    [Range(0, 4294967295, ErrorMessage = "排序值必须在0-4294967295之间")]
    public long? Order { get; set; }

    /// <summary>
    /// 部门id，32位整型，指定时必须大于1
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("id")]
    [Range(2, long.MaxValue, ErrorMessage = "部门ID必须大于1")]
    public long? Id { get; set; }
}

/// <summary>
/// 更新部门
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/08/15
/// <br/>请求方式：POST（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/department/update?access_token=ACCESS_TOKEN
/// <br/>权限说明：应用须拥有指定部门的管理权限，第三方仅通讯录应用可以调用
/// <br/>限制说明：部门最大层级15层，部门总数不超过3万个，每个部门下节点不超过3万个
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90206
/// </remarks>
[HttpRemoteApi(Action = "department/update", Desc = "更新部门", HttpMethod = HttpMethodEnum.Post)]
public class UpdateDeptWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 部门id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("id")]
    [Required(ErrorMessage = "部门ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "部门ID必须大于0")]
    public long Id { get; set; }

    /// <summary>
    /// 部门名称。长度限制为1~64个UTF-8字符，字符不能包括\:*?"&lt;&gt;|
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "部门名称长度必须在1-64个字符之间")]
    [RegularExpression(@"^[^\\:*?""<>|]+$", ErrorMessage = "部门名称不能包含\\:*?\"<>|字符")]
    public string Name { get; set; }

    /// <summary>
    /// 英文名称，需要在管理后台开启多语言支持才能生效
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("name_en")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "英文名称长度必须在1-64个字符之间")]
    [RegularExpression(@"^[^\\:*?""<>|]*$", ErrorMessage = "英文名称不能包含\\:*?\"<>|字符")]
    public string NameEn { get; set; }

    /// <summary>
    /// 父部门id
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("parentid")]
    [Range(1, long.MaxValue, ErrorMessage = "父部门ID必须大于0")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 在父部门中的次序值。order值大的排序靠前
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("order")]
    [Range(0, 4294967295, ErrorMessage = "排序值必须在0-4294967295之间")]
    public long? Order { get; set; }
}

/// <summary>
/// 删除部门
/// </summary>
/// <remarks>
/// <br/>最后更新：2020/03/30
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token=ACCESS_TOKEN&amp;id=ID
/// <br/>权限说明：应用须拥有指定部门的管理权限，第三方仅通讯录应用可以调用
/// <br/>限制说明：不能删除根部门；不能删除含有子部门、成员的部门
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/90207
/// </remarks>
[HttpRemoteApi(Action = "department/delete", Desc = "删除部门", HttpMethod = HttpMethodEnum.Get)]
public class DeleteDeptWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 部门id。（注：不能删除根部门；不能删除含有子部门、成员的部门）
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("id")]
    [Required(ErrorMessage = "部门ID不能为空")]
    [Range(2, long.MaxValue, ErrorMessage = "部门ID必须大于1（不能删除根部门）")]
    public long Id { get; set; }
}

/// <summary>
/// 获取子部门ID列表
/// </summary>
/// <remarks>
/// <br/>最后更新：2023/09/06
/// <br/>请求方式：GET（HTTPS）
/// <br/>请求地址：https://qyapi.weixin.qq.com/cgi-bin/department/simplelist?access_token=ACCESS_TOKEN&amp;id=ID
/// <br/>文档地址：https://developer.work.weixin.qq.com/document/path/95350
/// </remarks>
[HttpRemoteApi(Action = "department/simplelist", Desc = "获取子部门ID列表", HttpMethod = HttpMethodEnum.Get)]
public class DeptSimpleListWorkWxInput : AuthWorkWxInput
{
    /// <summary>
    /// 部门id。获取指定部门及其下的子部门（递归）
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("id")]
    public long? Id { get; set; }
}