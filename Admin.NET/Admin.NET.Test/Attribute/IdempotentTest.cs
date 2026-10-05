// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Xunit.Abstractions;

namespace Admin.NET.Test.Attribute;

/// <summary>
/// 防止重复请求测试
/// </summary>
public class IdempotentTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const string BaseAddress = "http://localhost:5005";
    private readonly HttpClient _client;

    public IdempotentTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = new HttpClient();
        _client.BaseAddress = new Uri(BaseAddress);
    }

    private async Task<string> PostAsync(string action)
    {
        var result = await _client.PostAsync(action, null);
        return await result.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task IdempotentTest1()
    {
        // 模拟重复请求50次
        var tasks = Task.WhenAll(Enumerable.Range(0, 50).Select(u => PostAsync("/api/test/idempotentTest1?input=1")));

        var resultList = await tasks;
        _testOutputHelper.WriteLine(string.Join('\n', resultList));
        Assert.Equal(1, resultList.Count(u => u == "请求成功"));
        Assert.Equal(resultList.Length - 1, resultList.Count(u => u == "您的操作过于频繁，请稍后再试！"));
    }

    [Fact]
    public async Task IdempotentTest2()
    {
        // 模拟重复请求50次
        var tasks = Task.WhenAll(Enumerable.Range(0, 50).Select(u => PostAsync("/api/test/idempotentTest2?input=1")));

        var resultList = await tasks;
        _testOutputHelper.WriteLine(string.Join('\n', resultList));
        Assert.Equal(1, resultList.Count(u => u == "请求成功"));
        Assert.Equal(resultList.Length - 1, resultList.Count(string.IsNullOrWhiteSpace));
    }
}