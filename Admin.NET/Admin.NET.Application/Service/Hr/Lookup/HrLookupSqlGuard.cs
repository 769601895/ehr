using System.Text.RegularExpressions;

namespace Admin.NET.Application;

/// <summary>查找数据源表名/列名白名单，防注入。</summary>
public static partial class HrLookupSqlGuard
{
    public static bool IsSafeIdent(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return IdentRegex().IsMatch(name.Trim());
    }

    public static string RequireIdent(string? name, string fieldLabel)
    {
        var v = name?.Trim();
        if (!IsSafeIdent(v))
            throw Oops.Oh($"{fieldLabel}不合法，仅允许字母数字下划线且不以数字开头");
        return v!;
    }

    public static string Quote(string ident) => "[" + RequireIdent(ident, "标识符").Replace("]", "]]") + "]";

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex IdentRegex();
}
