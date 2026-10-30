namespace Admin.NET.Application;

public static class HrUiControlGuess
{
    public static readonly string[] ControlTypes = ["text", "number", "date", "select", "photo", "cascader", "computed"];
    public static readonly string[] Origins = ["physical", "computed", "lookup"];

    public static string FromDataType(string dataType)
    {
        var t = (dataType ?? "").ToLowerInvariant();
        if (t.Contains("date") || t.Contains("time")) return "date";
        if (t.Contains("int") || t.Contains("decimal") || t.Contains("numeric") || t.Contains("float") || t.Contains("money") || t.Contains("real"))
            return "number";
        return "text";
    }

    /// <summary>绑了代码类或查找数据源的字段一律按 select 下拉，避免元数据控件仍是 text 时表单只能手输。</summary>
    public static string Resolve(string controlType, string? codeField, string? lookupSource = null)
    {
        if (!string.IsNullOrWhiteSpace(codeField) || !string.IsNullOrWhiteSpace(lookupSource)) return "select";
        return string.IsNullOrWhiteSpace(controlType) ? "text" : controlType.Trim();
    }
}
