// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text;
using System.Text.RegularExpressions;

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建成员输出参数
/// </summary>
public class CreateUserWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 因填写不存在的部门，新增的部门列表
    /// </summary>
    [CustomJsonProperty("created_department_list")]
    public CreatedDepartmentListDto CreatedDepartmentList { get; set; }

    /// <summary>
    /// 新增部门列表
    /// </summary>
    public class CreatedDepartmentListDto
    {
        /// <summary>
        /// 部门信息列表
        /// </summary>
        [CustomJsonProperty("department_info")]
        public List<DepartmentInfo> DepartmentInfo { get; set; }
    }

    /// <summary>
    /// 部门信息
    /// </summary>
    public class DepartmentInfo
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [CustomJsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [CustomJsonProperty("id")]
        public long Id { get; set; }
    }
}

/// <summary>
/// 读取成员输出参数
/// </summary>
/// <remarks>
/// <br/>注意：应用只能获取可见范围内的成员信息，且每种应用获取的字段有所不同
/// <br/>从2022年6月20号开始，新创建的自建应用与代开发应用不再返回敏感字段
/// </remarks>
public class UserWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 成员UserID。对应管理端的账号，企业内必须唯一。不区分大小写，长度为1~64个字节；第三方应用返回的值为open_userid
    /// </summary>
    [CustomJsonProperty("userid")]
    public string UserId { get; set; }

    /// <summary>
    /// 成员名称；第三方不可获取，调用时返回userid以代替name；代开发自建应用需要管理员授权才返回
    /// </summary>
    [CustomJsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// 成员所属部门id列表，仅返回该应用有查看权限的部门id
    /// </summary>
    [CustomJsonProperty("department")]
    public List<long> Department { get; set; }

    /// <summary>
    /// 部门内的排序值，默认为0。数量必须和department一致，数值越大排序越前面
    /// </summary>
    [CustomJsonProperty("order")]
    public List<long> Order { get; set; }

    /// <summary>
    /// 职务信息；代开发自建应用需要管理员授权才返回
    /// </summary>
    [CustomJsonProperty("position")]
    public string Position { get; set; }

    /// <summary>
    /// 手机号码，代开发自建应用需要管理员授权且成员oauth2授权获取
    /// </summary>
    [CustomJsonProperty("mobile")]
    public string Mobile { get; set; }

    /// <summary>
    /// 性别。0表示未定义，1表示男性，2表示女性
    /// </summary>
    [CustomJsonProperty("gender")]
    public int? Gender { get; set; }

    /// <summary>
    /// 性别描述
    /// </summary>
    public string GenderDesc => Gender switch
    {
        1 => "男",
        2 => "女",
        0 => "未定义",
        _ => "未知"
    };

    /// <summary>
    /// 邮箱，代开发自建应用需要管理员授权且成员oauth2授权获取
    /// </summary>
    [CustomJsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// 企业邮箱，代开发自建应用需要管理员授权且成员oauth2授权获取
    /// </summary>
    [CustomJsonProperty("biz_mail")]
    public string BizMail { get; set; }

    /// <summary>
    /// 表示在所在的部门内是否为部门负责人，数量与department一致
    /// </summary>
    [CustomJsonProperty("is_leader_in_dept")]
    public List<int> IsLeaderInDept { get; set; }

    /// <summary>
    /// 直属上级UserID，返回在应用可见范围内的直属上级列表，最多有1个直属上级
    /// </summary>
    [CustomJsonProperty("direct_leader")]
    public List<string> DirectLeader { get; set; }

    /// <summary>
    /// 头像url。代开发自建应用需要管理员授权且成员oauth2授权获取
    /// </summary>
    [CustomJsonProperty("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// 头像缩略图url
    /// </summary>
    [CustomJsonProperty("thumb_avatar")]
    public string ThumbAvatar { get; set; }

    /// <summary>
    /// 座机。代开发自建应用需要管理员授权才返回
    /// </summary>
    [CustomJsonProperty("telephone")]
    public string Telephone { get; set; }

    /// <summary>
    /// 别名
    /// </summary>
    [CustomJsonProperty("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// 地址。代开发自建应用需要管理员授权且成员oauth2授权获取
    /// </summary>
    [CustomJsonProperty("address")]
    public string Address { get; set; }

    /// <summary>
    /// 全局唯一ID。对于同一个服务商，不同应用获取到企业内同一个成员的open_userid是相同的
    /// </summary>
    [CustomJsonProperty("open_userid")]
    public string OpenUserId { get; set; }

    /// <summary>
    /// 主部门，仅当应用对主部门有查看权限时返回
    /// </summary>
    [CustomJsonProperty("main_department")]
    public long? MainDepartment { get; set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    [CustomJsonProperty("extattr")]
    public CreateUserWorkWxInput.ExtAttrDto ExtAttr { get; set; }

    /// <summary>
    /// 激活状态: 1=已激活，2=已禁用，4=未激活，5=退出企业
    /// </summary>
    [CustomJsonProperty("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusDesc => Status switch
    {
        1 => "已激活",
        2 => "已禁用",
        4 => "未激活",
        5 => "退出企业",
        _ => "未知状态"
    };

    /// <summary>
    /// 员工个人二维码URL
    /// </summary>
    [CustomJsonProperty("qr_code")]
    public string QrCode { get; set; }

    /// <summary>
    /// 对外职务
    /// </summary>
    [CustomJsonProperty("external_position")]
    public string ExternalPosition { get; set; }

    /// <summary>
    /// 成员对外属性
    /// </summary>
    [CustomJsonProperty("external_profile")]
    public CreateUserWorkWxInput.ExternalProfileDto ExternalProfile { get; set; }

    /// <summary>
    /// 是否为部门负责人（主部门）
    /// </summary>
    public bool IsLeader
    {
        get
        {
            if (IsLeaderInDept == null || Department == null || MainDepartment == null)
                return false;

            var mainDeptIndex = Department.IndexOf(MainDepartment.Value);
            return mainDeptIndex >= 0 && mainDeptIndex < IsLeaderInDept.Count && IsLeaderInDept[mainDeptIndex] == 1;
        }
    }

    /// <summary>
    /// 是否已激活
    /// </summary>
    public bool IsActive => Status == 1;

    /// <summary>
    /// 获取主部门名称（需要外部传入部门映射）
    /// </summary>
    public string GetMainDepartmentName(Dictionary<long, string> departmentMap)
    {
        if (MainDepartment.HasValue && departmentMap != null && departmentMap.TryGetValue(MainDepartment.Value, out var name))
            return name;
        return null;
    }

    /// <summary>
    /// 视频号信息（响应）
    /// </summary>
    public class WechatChannelsResponse
    {
        /// <summary>
        /// 视频号名称
        /// </summary>
        [CustomJsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// 视频号状态
        /// </summary>
        [CustomJsonProperty("status")]
        public int? Status { get; set; }
    }
}

/// <summary>
/// 获取部门成员输出参数
/// </summary>
public class DeptUserSimpleListWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 成员列表
    /// </summary>
    [CustomJsonProperty("userlist")]
    public List<DepartmentUserSimpleInfo> UserList { get; set; } = new List<DepartmentUserSimpleInfo>();

    /// <summary>
    /// 成员数量
    /// </summary>
    public int Count => UserList?.Count ?? 0;

    /// <summary>
    /// 是否包含成员
    /// </summary>
    public bool HasUsers => Count > 0;

    /// <summary>
    /// 获取用户ID列表
    /// </summary>
    public List<string> UserIds => UserList?.Select(u => u.UserId).ToList() ?? new List<string>();

    /// <summary>
    /// 获取OpenUserID列表
    /// </summary>
    public List<string> OpenUserIds => UserList?.Where(u => !string.IsNullOrEmpty(u.OpenUserId))
                                               .Select(u => u.OpenUserId)
                                               .ToList() ?? new List<string>();

    /// <summary>
    /// 部门成员简略信息
    /// </summary>
    public class DepartmentUserSimpleInfo
    {
        /// <summary>
        /// 成员UserID。对应管理端的账号
        /// </summary>
        [CustomJsonProperty("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// 成员名称，第三方应用可能返回userid代替name
        /// </summary>
        [CustomJsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 成员所属部门列表。列表项为部门ID，32位整型
        /// </summary>
        [CustomJsonProperty("department")]
        public List<long> Department { get; set; }

        /// <summary>
        /// 全局唯一ID。对于同一个服务商，不同应用获取到企业内同一个成员的open_userid是相同的
        /// </summary>
        [CustomJsonProperty("open_userid")]
        public string OpenUserId { get; set; }

        /// <summary>
        /// 获取显示名称（优先显示name，如果没有则显示userid）
        /// </summary>
        public string DisplayName => !string.IsNullOrEmpty(Name) ? Name : UserId;

        /// <summary>
        /// 是否包含指定部门
        /// </summary>
        public bool ContainsDepartment(long departmentId)
        {
            return Department?.Contains(departmentId) ?? false;
        }

        /// <summary>
        /// 获取主部门（第一个部门）
        /// </summary>
        public long? MainDepartment => Department?.FirstOrDefault();
    }
}

/// <summary>
/// 获取部门成员详情输出参数
/// </summary>
public class DeptUserDetailListWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 成员列表
    /// </summary>
    [CustomJsonProperty("userlist")]
    public List<DepartmentUserDetailInfo> UserList { get; set; } = new List<DepartmentUserDetailInfo>();

    /// <summary>
    /// 成员数量
    /// </summary>
    public int Count => UserList?.Count ?? 0;

    /// <summary>
    /// 是否包含成员
    /// </summary>
    public bool HasUsers => Count > 0;

    /// <summary>
    /// 获取已激活的成员列表
    /// </summary>
    public List<DepartmentUserDetailInfo> ActiveUsers => UserList?.Where(u => u.IsActive).ToList() ?? new List<DepartmentUserDetailInfo>();

    /// <summary>
    /// 获取部门负责人列表
    /// </summary>
    public List<DepartmentUserDetailInfo> LeaderUsers => UserList?.Where(u => u.IsLeader).ToList() ?? new List<DepartmentUserDetailInfo>();

    /// <summary>
    /// 获取用户ID列表
    /// </summary>
    public List<string> UserIds => UserList?.Select(u => u.UserId).ToList() ?? new List<string>();

    /// <summary>
    /// 按状态分组统计
    /// </summary>
    public Dictionary<string, int> GetStatusStatistics()
    {
        return UserList?
            .GroupBy(u => u.StatusDesc)
            .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<string, int>();
    }

    /// <summary>
    /// 部门成员详细信息
    /// </summary>
    public class DepartmentUserDetailInfo
    {
        /// <summary>
        /// 成员UserID。对应管理端的账号
        /// </summary>
        [CustomJsonProperty("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// 成员名称；第三方不可获取，调用时返回userid以代替name
        /// </summary>
        [CustomJsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [CustomJsonProperty("english_name")]
        public string EnglishName { get; set; }

        /// <summary>
        /// 成员所属部门id列表，仅返回该应用有查看权限的部门id
        /// </summary>
        [CustomJsonProperty("department")]
        public List<long> Department { get; set; }

        /// <summary>
        /// 部门内的排序值，默认为0。数量必须和department一致
        /// </summary>
        [CustomJsonProperty("order")]
        public List<long> Order { get; set; }

        /// <summary>
        /// 职务信息；代开发自建应用需要管理员授权才返回
        /// </summary>
        [CustomJsonProperty("position")]
        public string Position { get; set; }

        /// <summary>
        /// 手机号码，代开发自建应用需要管理员授权才返回
        /// </summary>
        [CustomJsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 性别。0表示未定义，1表示男性，2表示女性
        /// </summary>
        [CustomJsonProperty("gender")]
        public int? Gender { get; set; }

        /// <summary>
        /// 性别描述
        /// </summary>
        public string GenderDesc => Gender switch
        {
            1 => "男",
            2 => "女",
            0 => "未定义",
            _ => "未知"
        };

        /// <summary>
        /// 邮箱，代开发自建应用需要管理员授权才返回
        /// </summary>
        [CustomJsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// 企业邮箱，代开发自建应用不返回
        /// </summary>
        [CustomJsonProperty("biz_mail")]
        public string BizMail { get; set; }

        /// <summary>
        /// 表示在所在的部门内是否为部门负责人。0-否；1-是
        /// </summary>
        [CustomJsonProperty("is_leader_in_dept")]
        public List<int> IsLeaderInDept { get; set; }

        /// <summary>
        /// 直属上级UserID，返回在应用可见范围内的直属上级列表
        /// </summary>
        [CustomJsonProperty("direct_leader")]
        public List<string> DirectLeader { get; set; }

        /// <summary>
        /// 头像url。第三方仅通讯录应用可获取
        /// </summary>
        [CustomJsonProperty("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 头像缩略图url。第三方仅通讯录应用可获取
        /// </summary>
        [CustomJsonProperty("thumb_avatar")]
        public string ThumbAvatar { get; set; }

        /// <summary>
        /// 座机。代开发自建应用需要管理员授权才返回
        /// </summary>
        [CustomJsonProperty("telephone")]
        public string Telephone { get; set; }

        /// <summary>
        /// 别名；第三方仅通讯录应用可获取
        /// </summary>
        [CustomJsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [CustomJsonProperty("extattr")]
        public CreateUserWorkWxInput.ExtAttrDto ExtAttr { get; set; }

        /// <summary>
        /// 激活状态: 1=已激活，2=已禁用，4=未激活，5=退出企业
        /// </summary>
        [CustomJsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusDesc => Status switch
        {
            1 => "已激活",
            2 => "已禁用",
            4 => "未激活",
            5 => "退出企业",
            _ => "未知状态"
        };

        /// <summary>
        /// 地址。代开发自建应用需要管理员授权才返回
        /// </summary>
        [CustomJsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 全局唯一ID。仅第三方应用可获取
        /// </summary>
        [CustomJsonProperty("open_userid")]
        public string OpenUserId { get; set; }

        /// <summary>
        /// 主部门，仅当应用对主部门有查看权限时返回
        /// </summary>
        [CustomJsonProperty("main_department")]
        public long? MainDepartment { get; set; }

        /// <summary>
        /// 员工个人二维码URL
        /// </summary>
        [CustomJsonProperty("qr_code")]
        public string QrCode { get; set; }

        /// <summary>
        /// 对外职务
        /// </summary>
        [CustomJsonProperty("external_position")]
        public string ExternalPosition { get; set; }

        /// <summary>
        /// 成员对外属性
        /// </summary>
        [CustomJsonProperty("external_profile")]
        public CreateUserWorkWxInput.ExternalProfileDto ExternalProfile { get; set; }

        /// <summary>
        /// 获取显示名称（优先显示name，如果没有则显示userid）
        /// </summary>
        public string DisplayName => !string.IsNullOrEmpty(Name) ? Name : UserId;

        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool IsActive => Status == 1;

        /// <summary>
        /// 是否为部门负责人（主部门）
        /// </summary>
        public bool IsLeader
        {
            get
            {
                if (IsLeaderInDept == null || Department == null || MainDepartment == null)
                    return false;

                var mainDeptIndex = Department.IndexOf(MainDepartment.Value);
                return mainDeptIndex >= 0 && mainDeptIndex < IsLeaderInDept.Count && IsLeaderInDept[mainDeptIndex] == 1;
            }
        }

        /// <summary>
        /// 是否包含指定部门
        /// </summary>
        public bool ContainsDepartment(long departmentId)
        {
            return Department?.Contains(departmentId) ?? false;
        }

        /// <summary>
        /// 获取主部门排序值
        /// </summary>
        public long? GetMainDepartmentOrder()
        {
            if (Order == null || Department == null || MainDepartment == null)
                return null;

            var mainDeptIndex = Department.IndexOf(MainDepartment.Value);
            return mainDeptIndex >= 0 && mainDeptIndex < Order.Count ? Order[mainDeptIndex] : (long?)null;
        }
    }
}

/// <summary>
/// userid转openid输出参数
/// </summary>
public class ConvToOpenIdWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 企业微信成员userid对应的openid
    /// </summary>
    [CustomJsonProperty("openid")]
    public string OpenId { get; set; }
}

/// <summary>
/// openid转userid输出参数
/// </summary>
public class ConvToUserIdWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 该openid在企业微信对应的成员userid
    /// </summary>
    [CustomJsonProperty("userid")]
    public string UserId { get; set; }
}

/// <summary>
/// 邀请成员输出参数
/// </summary>
public class InviteUserWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 非法成员列表
    /// </summary>
    [CustomJsonProperty("invaliduser")]
    public List<string> InvalidUser { get; set; } = new List<string>();

    /// <summary>
    /// 非法部门列表
    /// </summary>
    [CustomJsonProperty("invalidparty")]
    public List<long> InvalidParty { get; set; } = new List<long>();

    /// <summary>
    /// 非法标签列表
    /// </summary>
    [CustomJsonProperty("invalidtag")]
    public List<long> InvalidTag { get; set; } = new List<long>();

    /// <summary>
    /// 有效成员数量
    /// </summary>
    public int ValidUserCount => (InputUserCount - InvalidUserCount);

    /// <summary>
    /// 有效部门数量
    /// </summary>
    public int ValidPartyCount => (InputPartyCount - InvalidPartyCount);

    /// <summary>
    /// 有效标签数量
    /// </summary>
    public int ValidTagCount => (InputTagCount - InvalidTagCount);

    /// <summary>
    /// 非法成员数量
    /// </summary>
    public int InvalidUserCount => InvalidUser?.Count ?? 0;

    /// <summary>
    /// 非法部门数量
    /// </summary>
    public int InvalidPartyCount => InvalidParty?.Count ?? 0;

    /// <summary>
    /// 非法标签数量
    /// </summary>
    public int InvalidTagCount => InvalidTag?.Count ?? 0;

    /// <summary>
    /// 输入成员数量（需要在调用时设置）
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public int InputUserCount { get; set; }

    /// <summary>
    /// 输入部门数量（需要在调用时设置）
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public int InputPartyCount { get; set; }

    /// <summary>
    /// 输入标签数量（需要在调用时设置）
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public int InputTagCount { get; set; }

    /// <summary>
    /// 是否全部有效
    /// </summary>
    public bool IsAllValid => InvalidUserCount == 0 && InvalidPartyCount == 0 && InvalidTagCount == 0;

    /// <summary>
    /// 获取邀请结果摘要
    /// </summary>
    public string GetInviteSummary()
    {
        var summary = new StringBuilder();
        summary.Append("邀请结果: ");

        if (InvalidUserCount > 0)
        {
            summary.Append($"{InvalidUserCount}个无效成员");
        }

        if (InvalidPartyCount > 0)
        {
            if (summary.Length > 12) summary.Append(", ");
            summary.Append($"{InvalidPartyCount}个无效部门");
        }

        if (InvalidTagCount > 0)
        {
            if (summary.Length > 12) summary.Append(", ");
            summary.Append($"{InvalidTagCount}个无效标签");
        }

        if (IsAllValid)
        {
            summary.Append("全部有效");
        }

        return summary.ToString();
    }
}

/// <summary>
/// 获取加入企业二维码输出参数
/// </summary>
public class JoinQrcodeWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 二维码链接，有效期7天
    /// </summary>
    [CustomJsonProperty("join_qrcode")]
    public string JoinQrcode { get; set; }

    /// <summary>
    /// 二维码尺寸描述
    /// </summary>
    public string QrcodeSizeDesc => SizeType switch
    {
        1 => "171x171",
        2 => "399x399",
        3 => "741x741",
        4 => "2052x2052",
        _ => "未知尺寸"
    };

    /// <summary>
    /// 尺寸类型（从URL中解析）
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public int? SizeType
    {
        get
        {
            if (string.IsNullOrEmpty(JoinQrcode)) return null;
            var match = Regex.Match(JoinQrcode, @"qr_size=(\d)");
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }
    }
}

/// <summary>
/// 成员Id输出参数
/// </summary>
public class UserIdWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 用户userid
    /// </summary>
    [CustomJsonProperty("userid")]
    public string UserId { get; set; }
}

/// <summary>
/// 获取成员ID列表输出参数
/// </summary>
public class UserIdListWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 分页游标，下次请求时填写以获取之后分页的记录
    /// </summary>
    [CustomJsonProperty("next_cursor")]
    public string NextCursor { get; set; }

    /// <summary>
    /// 用户-部门关系列表
    /// </summary>
    [CustomJsonProperty("dept_user")]
    public List<DeptUserInfo> DeptUser { get; set; } = new List<DeptUserInfo>();

    /// <summary>
    /// 用户数量
    /// </summary>
    public int UserCount => DeptUser?.Count ?? 0;

    /// <summary>
    /// 是否还有更多数据
    /// </summary>
    public bool HasMore => !string.IsNullOrEmpty(NextCursor);

    /// <summary>
    /// 获取去重后的用户ID列表
    /// </summary>
    public List<string> DistinctUserIds => DeptUser?.Select(d => d.UserId).Distinct().ToList() ?? new List<string>();

    /// <summary>
    /// 按部门分组用户
    /// </summary>
    public Dictionary<long, List<string>> GroupUsersByDepartment()
    {
        var result = new Dictionary<long, List<string>>();

        if (DeptUser == null) return result;

        foreach (var item in DeptUser)
        {
            if (!result.ContainsKey(item.Department))
            {
                result[item.Department] = new List<string>();
            }
            result[item.Department].Add(item.UserId);
        }

        return result;
    }

    /// <summary>
    /// 按用户分组部门
    /// </summary>
    public Dictionary<string, List<long>> GroupDepartmentsByUser()
    {
        var result = new Dictionary<string, List<long>>();

        if (DeptUser == null) return result;

        foreach (var item in DeptUser)
        {
            if (!result.ContainsKey(item.UserId))
            {
                result[item.UserId] = new List<long>();
            }
            result[item.UserId].Add(item.Department);
        }

        return result;
    }

    /// <summary>
    /// 用户-部门关系信息
    /// </summary>
    public class DeptUserInfo
    {
        /// <summary>
        /// 用户userid，当用户在多个部门下时会有多条记录
        /// </summary>
        [CustomJsonProperty("userid")]
        public string UserId { get; set; }

        /// <summary>
        /// 用户所属部门
        /// </summary>
        [CustomJsonProperty("department")]
        public long Department { get; set; }
    }
}