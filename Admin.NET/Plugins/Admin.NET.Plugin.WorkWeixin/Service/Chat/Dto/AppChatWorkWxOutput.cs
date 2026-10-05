// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建群聊会话输出参数
/// </summary>
public class CreateChatWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 群聊的唯一标志
    /// </summary>
    [CustomJsonProperty("chatid")]
    public string ChatId { get; set; }
}

/// <summary>
/// 获取群聊会话输出参数
/// </summary>
public class GetChatWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 群聊信息
    /// </summary>
    [CustomJsonProperty("chat_info")]
    public ChatInfoDto ChatInfo { get; set; }

    /// <summary>
    /// 群聊信息
    /// </summary>
    public class ChatInfoDto
    {
        /// <summary>
        /// 群聊唯一标志
        /// </summary>
        [CustomJsonProperty("chatid")]
        public string ChatId { get; set; }

        /// <summary>
        /// 群聊名
        /// </summary>
        [CustomJsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 群主id
        /// </summary>
        [CustomJsonProperty("owner")]
        public string Owner { get; set; }

        /// <summary>
        /// 群成员id列表
        /// </summary>
        [CustomJsonProperty("userlist")]
        public List<string> UserList { get; set; }

        /// <summary>
        /// 群聊类型，0表示普通群聊，1表示课程群聊
        /// </summary>
        [CustomJsonProperty("chat_type")]
        public int ChatType { get; set; }
    }
}