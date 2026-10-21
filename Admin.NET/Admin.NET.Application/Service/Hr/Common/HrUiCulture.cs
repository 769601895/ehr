using Furion.Localization;

namespace Admin.NET.Application;

/// <summary>
/// 当前 UI 文化：优先本次请求的 Accept-Language（与界面语言一致），否则用 SetCulture。
/// </summary>
public static class HrUiCulture
{
    public const string Fallback = "zh-CN";

    public static readonly string[] Supported = ["zh-CN", "zh-TW", "en", "th", "vi"];

    public static string Current()
    {
        // 前端每次请求都带当前界面语言。切换语言时 SetCulture 可能还没落库，不能让旧文化盖住本次请求。
        var fromHeader = TryAcceptLanguage();
        if (fromHeader != null)
            return fromHeader;

        try
        {
            var selected = L.GetSelectCulture();
            var name = selected?.Culture?.Name;
            if (IsExplicit(name))
                return Normalize(name);
        }
        catch
        {
            // ignore
        }

        return Fallback;
    }

    /// <summary>仅当请求头本身是受支持文化时采用，避免把未知语言归一成 zh-CN 后盖住 SetCulture。</summary>
    private static string TryAcceptLanguage()
    {
        try
        {
            var al = App.HttpContext?.Request?.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(al)) return null;
            var first = al.Split(',')[0].Split(';')[0].Trim();
            if (!IsExplicit(first)) return null;
            return Normalize(first);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsExplicit(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim().Replace('_', '-');
        if (n.Equals("zh-CN", StringComparison.OrdinalIgnoreCase) || n.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase))
            return true;
        if (n.Equals("zh-TW", StringComparison.OrdinalIgnoreCase) || n.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase))
            return true;
        if (n.StartsWith("en", StringComparison.OrdinalIgnoreCase)) return true;
        if (n.StartsWith("th", StringComparison.OrdinalIgnoreCase)) return true;
        if (n.StartsWith("vi", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Fallback;
        var n = name.Trim().Replace('_', '-');
        if (n.Equals("zh-CN", StringComparison.OrdinalIgnoreCase) || n.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase))
            return "zh-CN";
        if (n.Equals("zh-TW", StringComparison.OrdinalIgnoreCase) || n.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase))
            return "zh-TW";
        if (n.StartsWith("en", StringComparison.OrdinalIgnoreCase)) return "en";
        if (n.StartsWith("th", StringComparison.OrdinalIgnoreCase)) return "th";
        if (n.StartsWith("vi", StringComparison.OrdinalIgnoreCase)) return "vi";
        return Fallback;
    }
}
