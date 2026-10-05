// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Plugin.Ai;

namespace Admin.NET.Application.Service.LLM;

/// <summary>
/// LLM测试,可以切换模型
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100, Description = "LLM测试,可以切换模型")]
public class LLMChangeModelTestService : IDynamicApiController, ITransient
{
    private readonly ILLMFactory _llmFactory;

    public LLMChangeModelTestService(ILLMFactory llmFactory)
    {
        _llmFactory = llmFactory;
    }

    /// <summary>
    /// 演示大模型的使用，可以切换模型。
    /// 例如：可以切换到不同的模型，如：OpenAI、Azure OpenAI、Google Gemini等。
    /// </summary>
    /// <param name="modelInput"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "TestSwitch", Description = "测试模型切换")]
    public async Task<string> TestSwitchAsync(LLMModelInput modelInput)
    {
        var kernel = _llmFactory.CreateKernel(modelInput);
        var result = await kernel.InvokePromptAsync("请介绍自己");
        return result.ToString();
    }
}