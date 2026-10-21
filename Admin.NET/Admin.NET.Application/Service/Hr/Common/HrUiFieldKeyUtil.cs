using System.Text;

namespace Admin.NET.Application;

public static class HrUiFieldKeyUtil
{
    /// <summary>
    /// 已是 camelCase 则沿用；否则 snake / pascal 转 camelCase（emp_id → empId）。
    /// </summary>
    public static string FromColumn(string column)
    {
        if (string.IsNullOrWhiteSpace(column)) return "";
        var s = column.Trim();
        if (!s.Contains('_'))
        {
            if (char.IsLower(s[0])) return s;
            return char.ToLowerInvariant(s[0]) + s[1..];
        }

        var parts = s.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        for (var i = 0; i < parts.Length; i++)
        {
            var p = parts[i].ToLowerInvariant();
            if (p.Length == 0) continue;
            if (sb.Length == 0)
                sb.Append(p);
            else
                sb.Append(char.ToUpperInvariant(p[0])).Append(p.AsSpan(1));
        }
        return sb.ToString();
    }

    public static string Unique(string key, ISet<string> used)
    {
        if (string.IsNullOrWhiteSpace(key)) key = "field";
        if (!used.Contains(key)) return key;
        var n = 2;
        while (used.Contains(key + "_" + n)) n++;
        return key + "_" + n;
    }
}
