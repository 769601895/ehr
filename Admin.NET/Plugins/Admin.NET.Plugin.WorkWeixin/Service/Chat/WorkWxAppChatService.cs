// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信群聊会话服务 🧩
/// </summary>
public class WorkWxAppChatService(WorkWxBaseService baseService) : ITransient
{
    /// <summary>
    /// 创建群聊会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<CreateChatWorkWxOutput> CreateChat(CreateChatWorkWxInput input)
    {
        return await baseService.SendAsync<CreateChatWorkWxInput, CreateChatWorkWxOutput>(input);
    }

    /// <summary>
    /// 修改群聊会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> UpdateChat(UpdateChatWorkWxInput input)
    {
        return await baseService.SendAsync<UpdateChatWorkWxInput, BaseWorkWxOutput>(input);
    }

    /// <summary>
    /// 推送纯文本消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="content">内容</param>
    /// <param name="safe">安全标识</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendText(string chatId, string content, int? safe = null)
    {
        return await baseService.SendAsync<TextAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateTextMessage(chatId, content, safe));
    }

    /// <summary>
    /// 推送文本卡片消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="title">标题</param>
    /// <param name="description">内容</param>
    /// <param name="url">跳转链接</param>
    /// <param name="btnTxt">按钮文本</param>
    /// <param name="safe">安全标识</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendTextCard(string chatId, string title, string description, string url, string btnTxt = null, int? safe = null)
    {
        return await baseService.SendAsync<TextCardAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateTextCardMessage(chatId, title, description, url, btnTxt, safe));
    }

    /// <summary>
    /// 推送Markdown消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="content">内容</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendMarkdown(string chatId, string content)
    {
        return await baseService.SendAsync<MarkdownAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateMarkdownMessage(chatId, content));
    }

    /// <summary>
    /// 推送图片消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="mediaId">媒体文件ID</param>
    /// <param name="safe">安全标识</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendImage(string chatId, string mediaId, int? safe = null)
    {
        return await baseService.SendAsync<ImageAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateImageMessage(chatId, mediaId, safe));
    }

    /// <summary>
    /// 推送语音消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="mediaId">媒体文件ID</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendVoice(string chatId, string mediaId)
    {
        return await baseService.SendAsync<VoiceAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateVoiceMessage(chatId, mediaId));
    }

    /// <summary>
    /// 推送视频消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="mediaId">媒体文件ID</param>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="safe">安全标识</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendVideo(string chatId, string mediaId, string title = null, string description = null, int? safe = null)
    {
        return await baseService.SendAsync<VideoAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateVideoMessage(chatId, mediaId, title, description, safe));
    }

    /// <summary>
    /// 推送文件消息
    /// </summary>
    /// <param name="chatId">群聊Id</param>
    /// <param name="mediaId">媒体文件ID</param>
    /// <param name="safe">安全标识</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> SendFile(string chatId, string mediaId, int? safe = null)
    {
        return await baseService.SendAsync<FileAppChatMessage, BaseWorkWxOutput>(AppChatMessageFactory.CreateFileMessage(chatId, mediaId, safe));
    }
}