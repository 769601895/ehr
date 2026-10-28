namespace Admin.NET.Application;

/// <summary>
/// 将领域层 <c>Wf.*</c> 键转为带本地化文案的友好异常。
/// </summary>
public static class HrWfErrors
{
    /// <summary>按错误码抛出（推荐服务层直接使用）。</summary>
    public static Exception Oh(HrWfErrorCodeEnum code) => Oops.Oh(code);

    /// <summary>
    /// 将 <c>Wf.Xxx</c> 或枚举名映射为友好异常；无法识别时原样提示。
    /// </summary>
    public static Exception Oh(string? keyOrMessage)
    {
        if (TryParse(keyOrMessage, out var code))
            return Oops.Oh(code);
        return Oops.Oh(string.IsNullOrWhiteSpace(keyOrMessage) ? "操作失败" : keyOrMessage);
    }

    public static bool TryParse(string? keyOrMessage, out HrWfErrorCodeEnum code)
    {
        code = default;
        if (string.IsNullOrWhiteSpace(keyOrMessage))
            return false;

        var name = keyOrMessage.Trim();
        if (name.StartsWith("Wf.", StringComparison.OrdinalIgnoreCase))
            name = name[3..];

        return Enum.TryParse(name, ignoreCase: true, out code)
               && Enum.IsDefined(typeof(HrWfErrorCodeEnum), code);
    }
}
