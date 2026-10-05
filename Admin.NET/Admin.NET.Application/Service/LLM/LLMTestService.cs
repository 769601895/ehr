// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application.Service.LLM;

/// <summary>
/// LLM测试，不可切换模型
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100, Description = "LLM测试，不可切换模型")]
public class LLMTestService : IDynamicApiController, ITransient
{
    private readonly Kernel _kernel;

    public LLMTestService(Kernel kernel)
    {
        _kernel = kernel;
    }

    /// <summary>
    /// 演示使用常规大模型的使用，只能使用配置的默认模型，不能切换模型。
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Test", Description = "测试")]
    public async Task<string> TestAsync()
    {
        //1. 非流式输出
        var result = await _kernel.InvokePromptAsync("请介绍自己");
        return result.ToString();
        //2. 流式输出
        // var chat = _kernel.GetRequiredService<IChatCompletionService>();
        // ChatHistory chatHistory = [];
        // chatHistory.AddUserMessage("请介绍自己");
        // var response =  chat.GetStreamingChatMessageContentsAsync(
        //     chatHistory: chatHistory,
        //     kernel: _kernel
        // );
        // var result = "";
        // await foreach (var chunk in response)
        // {
        //     result += chunk.Content ?? "";
        //     Console.WriteLine(chunk.Content);
        // }
        // return result;
    }
}