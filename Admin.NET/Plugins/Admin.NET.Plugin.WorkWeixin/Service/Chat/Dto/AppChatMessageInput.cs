// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 应用推送消息基类
/// </summary>
[HttpRemoteApi(Action = "appchat/send", Desc = "应用消息推送", HttpMethod = HttpMethodEnum.Post)]
public abstract class AppChatMessageInput : AuthWorkWxInput
{
    /// <summary>
    /// 群聊id
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("chatid")]
    [Required(ErrorMessage = "群聊ID不能为空")]
    public string ChatId { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("msgtype")]
    [Required(ErrorMessage = "消息类型不能为空")]
    public abstract string MsgType { get; }

    /// <summary>
    /// 表示是否是保密消息，0表示否，1表示是，默认0
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("safe")]
    [Range(0, 1, ErrorMessage = "安全标识只能是0或1")]
    public int? Safe { get; set; }
}

/// <summary>
/// 文本消息
/// </summary>
/// <remarks>
/// <br/>特殊说明：content字段可以支持换行，换行符请用转义过的'\n'
/// </remarks>
public class TextAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "text";

    /// <summary>
    /// 文本消息内容
    /// </summary>
    [CustomJsonProperty("text")]
    [Required(ErrorMessage = "文本消息内容不能为空")]
    public TextContent Text { get; set; } = new TextContent();
}

/// <summary>
/// 图片消息
/// </summary>
public class ImageAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "image";

    /// <summary>
    /// 图片消息内容
    /// </summary>
    [CustomJsonProperty("image")]
    [Required(ErrorMessage = "图片消息内容不能为空")]
    public MediaContent Image { get; set; } = new MediaContent();
}

/// <summary>
/// 语音消息
/// </summary>
public class VoiceAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "voice";

    /// <summary>
    /// 语音消息内容
    /// </summary>
    [CustomJsonProperty("voice")]
    [Required(ErrorMessage = "语音消息内容不能为空")]
    public MediaContent Voice { get; set; } = new MediaContent();
}

/// <summary>
/// 视频消息
/// </summary>
public class VideoAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "video";

    /// <summary>
    /// 视频消息内容
    /// </summary>
    [CustomJsonProperty("video")]
    [Required(ErrorMessage = "视频消息内容不能为空")]
    public VideoContent Video { get; set; } = new VideoContent();
}

/// <summary>
/// 文件消息
/// </summary>
public class FileAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "file";

    /// <summary>
    /// 文件消息内容
    /// </summary>
    [CustomJsonProperty("file")]
    [Required(ErrorMessage = "文件消息内容不能为空")]
    public MediaContent File { get; set; } = new MediaContent();
}

/// <summary>
/// 文本卡片消息
/// </summary>
public class TextCardAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "textcard";

    /// <summary>
    /// 文本卡片消息内容
    /// </summary>
    [CustomJsonProperty("textcard")]
    [Required(ErrorMessage = "文本卡片消息内容不能为空")]
    public TextCardContent TextCard { get; set; } = new TextCardContent();
}

/// <summary>
/// 图文消息
/// </summary>
public class NewsAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "news";

    /// <summary>
    /// 图文消息内容
    /// </summary>
    [CustomJsonProperty("news")]
    [Required(ErrorMessage = "图文消息内容不能为空")]
    public NewsContent News { get; set; } = new NewsContent();
}

/// <summary>
/// 图文消息（mpnews）
/// </summary>
public class MpNewsAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "mpnews";

    /// <summary>
    /// 图文消息内容
    /// </summary>
    [CustomJsonProperty("mpnews")]
    [Required(ErrorMessage = "图文消息内容不能为空")]
    public MpNewsContent MpNews { get; set; } = new MpNewsContent();
}

/// <summary>
/// Markdown消息
/// </summary>
public class MarkdownAppChatMessage : AppChatMessageInput
{
    public override string MsgType => "markdown";

    /// <summary>
    /// Markdown消息内容
    /// </summary>
    [CustomJsonProperty("markdown")]
    [Required(ErrorMessage = "Markdown消息内容不能为空")]
    public MarkdownContent Markdown { get; set; } = new MarkdownContent();
}

/// <summary>
/// 文本消息内容
/// </summary>
public class TextContent
{
    /// <summary>
    /// 消息内容，最长不超过2048个字节
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("content")]
    [Required(ErrorMessage = "消息内容不能为空")]
    [StringLength(2048, ErrorMessage = "消息内容长度不能超过2048个字节")]
    public string Content { get; set; }
}

/// <summary>
/// 媒体内容基类
/// </summary>
public class MediaContent
{
    /// <summary>
    /// 媒体文件id，可以调用上传临时素材接口获取
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("media_id")]
    [Required(ErrorMessage = "媒体文件ID不能为空")]
    public string MediaId { get; set; }
}

/// <summary>
/// 视频消息内容
/// </summary>
public class VideoContent : MediaContent
{
    /// <summary>
    /// 视频消息的标题，不超过128个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("title")]
    [StringLength(128, ErrorMessage = "视频标题长度不能超过128个字节")]
    public string Title { get; set; }

    /// <summary>
    /// 视频消息的描述，不超过512个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("description")]
    [StringLength(512, ErrorMessage = "视频描述长度不能超过512个字节")]
    public string Description { get; set; }
}

/// <summary>
/// 文本卡片消息内容
/// </summary>
public class TextCardContent
{
    /// <summary>
    /// 标题，不超过128个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("title")]
    [Required(ErrorMessage = "标题不能为空")]
    [StringLength(128, ErrorMessage = "标题长度不能超过128个字节")]
    public string Title { get; set; }

    /// <summary>
    /// 描述，不超过512个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("description")]
    [Required(ErrorMessage = "描述不能为空")]
    [StringLength(512, ErrorMessage = "描述长度不能超过512个字节")]
    public string Description { get; set; }

    /// <summary>
    /// 点击后跳转的链接
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("url")]
    [Required(ErrorMessage = "跳转链接不能为空")]
    [Url(ErrorMessage = "链接格式不正确")]
    public string Url { get; set; }

    /// <summary>
    /// 按钮文字。默认为"详情"，不超过4个文字，超过自动截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("btntxt")]
    [StringLength(4, ErrorMessage = "按钮文字长度不能超过4个字符")]
    public string BtnTxt { get; set; }
}

/// <summary>
/// 图文消息文章
/// </summary>
public class Article
{
    /// <summary>
    /// 标题，不超过128个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("title")]
    [Required(ErrorMessage = "文章标题不能为空")]
    [StringLength(128, ErrorMessage = "文章标题长度不能超过128个字节")]
    public string Title { get; set; }

    /// <summary>
    /// 描述，不超过512个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("description")]
    [StringLength(512, ErrorMessage = "文章描述长度不能超过512个字节")]
    public string Description { get; set; }

    /// <summary>
    /// 点击后跳转的链接
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("url")]
    [Required(ErrorMessage = "文章链接不能为空")]
    [Url(ErrorMessage = "文章链接格式不正确")]
    public string Url { get; set; }

    /// <summary>
    /// 图文消息的图片链接，支持JPG、PNG格式
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("picurl")]
    [Url(ErrorMessage = "图片链接格式不正确")]
    public string PicUrl { get; set; }
}

/// <summary>
/// 图文消息内容
/// </summary>
public class NewsContent
{
    /// <summary>
    /// 图文消息，一个图文消息支持1到8条图文
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("articles")]
    [Required(ErrorMessage = "图文消息不能为空")]
    [MinLength(1, ErrorMessage = "至少需要1条图文消息")]
    [MaxLength(8, ErrorMessage = "最多只能有8条图文消息")]
    public List<Article> Articles { get; set; } = new List<Article>();
}

/// <summary>
/// 图文消息（mpnews）文章
/// </summary>
public class MpArticle
{
    /// <summary>
    /// 标题，不超过128个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("title")]
    [Required(ErrorMessage = "文章标题不能为空")]
    [StringLength(128, ErrorMessage = "文章标题长度不能超过128个字节")]
    public string Title { get; set; }

    /// <summary>
    /// 图文消息缩略图的media_id，可以通过素材管理接口获得
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("thumb_media_id")]
    [Required(ErrorMessage = "缩略图媒体ID不能为空")]
    public string ThumbMediaId { get; set; }

    /// <summary>
    /// 图文消息的作者，不超过64个字节
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("author")]
    [StringLength(64, ErrorMessage = "作者长度不能超过64个字节")]
    public string Author { get; set; }

    /// <summary>
    /// 图文消息点击"阅读原文"之后的页面链接
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("content_source_url")]
    [Url(ErrorMessage = "原文链接格式不正确")]
    public string ContentSourceUrl { get; set; }

    /// <summary>
    /// 图文消息的内容，支持html标签，不超过666K个字节
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("content")]
    [Required(ErrorMessage = "文章内容不能为空")]
    public string Content { get; set; }

    /// <summary>
    /// 图文消息的描述，不超过512个字节，超过会自动截断
    /// </summary>
    /// <remarks>是否必填：否</remarks>
    [CustomJsonProperty("digest")]
    [StringLength(512, ErrorMessage = "摘要长度不能超过512个字节")]
    public string Digest { get; set; }
}

/// <summary>
/// 图文消息（mpnews）内容
/// </summary>
public class MpNewsContent
{
    /// <summary>
    /// 图文消息，一个图文消息支持1到8条图文
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("articles")]
    [Required(ErrorMessage = "图文消息不能为空")]
    [MinLength(1, ErrorMessage = "至少需要1条图文消息")]
    [MaxLength(8, ErrorMessage = "最多只能有8条图文消息")]
    public List<MpArticle> Articles { get; set; } = new List<MpArticle>();
}

/// <summary>
/// Markdown消息内容
/// </summary>
public class MarkdownContent
{
    /// <summary>
    /// markdown内容，最长不超过2048个字节，必须是utf8编码
    /// </summary>
    /// <remarks>是否必填：是</remarks>
    [CustomJsonProperty("content")]
    [Required(ErrorMessage = "Markdown内容不能为空")]
    [StringLength(2048, ErrorMessage = "Markdown内容长度不能超过2048个字节")]
    public string Content { get; set; }
}