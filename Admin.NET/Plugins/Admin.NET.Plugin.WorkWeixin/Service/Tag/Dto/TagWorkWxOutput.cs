// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建标签输出
/// </summary>
public class CreateTagWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 标签id
    /// </summary>
    [CustomJsonProperty("tagid")]
    public long TagId { get; set; }
}

/// <summary>
/// 标签成员信息
/// </summary>
public class TagMemberWorkWxInfo
{
    /// <summary>
    /// 成员账号
    /// </summary>
    [CustomJsonProperty("userid")]
    public string UserId { get; set; }

    /// <summary>
    /// 成员名称
    /// </summary>
    [CustomJsonProperty("name")]
    public string Name { get; set; }
}

/// <summary>
/// 获取标签成员输出
/// </summary>
public class TagMembersWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 标签名
    /// </summary>
    [CustomJsonProperty("tagname")]
    public string TagName { get; set; }

    /// <summary>
    /// 标签中包含的成员列表
    /// </summary>
    [CustomJsonProperty("userlist")]
    public List<TagMemberWorkWxInfo> UserList { get; set; } = new List<TagMemberWorkWxInfo>();

    /// <summary>
    /// 标签中包含的部门id列表
    /// </summary>
    [CustomJsonProperty("partylist")]
    public List<long> PartyList { get; set; } = new List<long>();
}

/// <summary>
/// 增加标签成员输出
/// </summary>
public class AddTagMembersWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 非法的成员账号列表
    /// </summary>
    [CustomJsonProperty("invalidlist")]
    public string InvalidList { get; set; }

    /// <summary>
    /// 非法的部门id列表
    /// </summary>
    [CustomJsonProperty("invalidparty")]
    public List<long> InvalidParty { get; set; } = new List<long>();
}

/// <summary>
/// 删除标签成员输出
/// </summary>
public class DeleteTagMembersWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 非法的成员账号列表
    /// </summary>
    [CustomJsonProperty("invalidlist")]
    public string InvalidList { get; set; }

    /// <summary>
    /// 非法的部门id列表
    /// </summary>
    [CustomJsonProperty("invalidparty")]
    public List<long> InvalidParty { get; set; } = new List<long>();
}

/// <summary>
/// 标签基本信息
/// </summary>
public class TagInfoWorkWx
{
    /// <summary>
    /// 标签id
    /// </summary>
    [CustomJsonProperty("tagid")]
    public long TagId { get; set; }

    /// <summary>
    /// 标签名
    /// </summary>
    [CustomJsonProperty("tagname")]
    public string TagName { get; set; }
}

/// <summary>
/// 获取标签列表输出
/// </summary>
public class TagListWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 标签列表
    /// </summary>
    [CustomJsonProperty("taglist")]
    public List<TagInfoWorkWx> TagList { get; set; } = new List<TagInfoWorkWx>();
}