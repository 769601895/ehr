// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 应用推送消息工厂类
/// </summary>
public static class AppChatMessageFactory
{
    /// <summary>
    /// 文本消息
    /// </summary>
    public static TextAppChatMessage CreateTextMessage(string chatId, string content, int? safe = null)
    {
        return new TextAppChatMessage
        {
            ChatId = chatId,
            Text = new TextContent { Content = content },
            Safe = safe
        };
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public static ImageAppChatMessage CreateImageMessage(string chatId, string mediaId, int? safe = null)
    {
        return new ImageAppChatMessage
        {
            ChatId = chatId,
            Image = new MediaContent { MediaId = mediaId },
            Safe = safe
        };
    }

    /// <summary>
    /// 语音消息
    /// </summary>
    public static VoiceAppChatMessage CreateVoiceMessage(string chatId, string mediaId)
    {
        return new VoiceAppChatMessage
        {
            ChatId = chatId,
            Voice = new MediaContent { MediaId = mediaId }
        };
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public static VideoAppChatMessage CreateVideoMessage(string chatId, string mediaId, string title = null, string description = null, int? safe = null)
    {
        return new VideoAppChatMessage
        {
            ChatId = chatId,
            Video = new VideoContent { MediaId = mediaId, Title = title, Description = description },
            Safe = safe
        };
    }

    /// <summary>
    /// 文件消息
    /// </summary>
    public static FileAppChatMessage CreateFileMessage(string chatId, string mediaId, int? safe = null)
    {
        return new FileAppChatMessage
        {
            ChatId = chatId,
            File = new MediaContent { MediaId = mediaId },
            Safe = safe
        };
    }

    /// <summary>
    /// 文本卡片消息
    /// </summary>
    public static TextCardAppChatMessage CreateTextCardMessage(string chatId, string title, string description, string url, string btnTxt = null, int? safe = null)
    {
        return new TextCardAppChatMessage
        {
            ChatId = chatId,
            TextCard = new TextCardContent { Title = title, Description = description, Url = url, BtnTxt = btnTxt },
            Safe = safe
        };
    }

    /// <summary>
    /// Markdown消息
    /// </summary>
    public static MarkdownAppChatMessage CreateMarkdownMessage(string chatId, string content)
    {
        return new MarkdownAppChatMessage
        {
            ChatId = chatId,
            Markdown = new MarkdownContent { Content = content }
        };
    }
}