namespace Admin.NET.Application;

/// <summary>
/// 字段名与码描述回退。空串视为没有译文。失败回退主语言，不抛错。
/// </summary>
public static class HrUiI18nResolver
{
    public static string ResolveFieldLabel(
        string fieldKey,
        string poolLabel,
        string culture,
        IReadOnlyDictionary<string, string> layoutLang,
        IReadOnlyDictionary<string, string> fieldLang)
    {
        if (Try(layoutLang, culture, out var layout)) return layout;
        if (Try(fieldLang, culture, out var field)) return field;
        if (!string.IsNullOrWhiteSpace(poolLabel)) return poolLabel.Trim();
        return string.IsNullOrWhiteSpace(fieldKey) ? "" : fieldKey;
    }

    public static string ResolveCodeDesc(string codeValue, string mainDesc, string culture, IReadOnlyDictionary<string, string> lang)
    {
        if (Try(lang, culture, out var translated)) return translated;
        if (!string.IsNullOrWhiteSpace(mainDesc)) return mainDesc.Trim();
        return codeValue ?? "";
    }

    /// <summary>
    /// 现网格式「码值 描述」。描述回退成码值本身时不重复拼接。
    /// </summary>
    public static string FormatCodeLabel(string codeValue, string desc)
    {
        var value = codeValue ?? "";
        if (string.IsNullOrWhiteSpace(desc) || string.Equals(desc.Trim(), value, StringComparison.Ordinal))
            return value;
        return value + " " + desc.Trim();
    }

    private static bool Try(IReadOnlyDictionary<string, string> map, string culture, out string value)
    {
        value = null;
        if (map == null || string.IsNullOrWhiteSpace(culture)) return false;
        if (!map.TryGetValue(culture, out var raw)) return false;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        value = raw.Trim();
        return true;
    }
}
