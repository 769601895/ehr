using System.Globalization;
using System.Text.Json;

namespace Admin.NET.Application;

/// <summary>
/// 节点 ConditionJson 求值。契约：{"field":"amount","op":">","value":1000}，op ∈ =,!=,&gt;,≥,&lt;,≤。
/// null/空白 → true；缺 field 或无法比较 → false（安全侧跳过）。
/// </summary>
public static class HrWfConditionEval
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private sealed class Cond
    {
        public string? Field { get; set; }
        public string? Op { get; set; }
        public JsonElement Value { get; set; }
    }

    public static bool IsMatch(string? conditionJson, IReadOnlyDictionary<string, object?> formData)
    {
        if (string.IsNullOrWhiteSpace(conditionJson))
            return true;

        Cond? cond;
        try
        {
            cond = JsonSerializer.Deserialize<Cond>(conditionJson, JsonOpts);
        }
        catch
        {
            return false;
        }

        if (cond == null || string.IsNullOrWhiteSpace(cond.Field) || string.IsNullOrWhiteSpace(cond.Op))
            return false;

        formData ??= new Dictionary<string, object?>();
        if (!TryGetFormValue(formData, cond.Field!, out var left))
            return false;

        var op = cond.Op!.Trim();
        var right = JsonElementToObject(cond.Value);

        return op switch
        {
            "=" or "==" => CompareEqual(left, right),
            "!=" or "<>" => !CompareEqual(left, right),
            ">" => CompareOrdered(left, right) is > 0,
            ">=" => CompareOrdered(left, right) is >= 0,
            "<" => CompareOrdered(left, right) is < 0,
            "<=" => CompareOrdered(left, right) is <= 0,
            _ => false
        };
    }

    private static bool TryGetFormValue(IReadOnlyDictionary<string, object?> formData, string field, out object? value)
    {
        if (formData.TryGetValue(field, out value))
            return true;
        foreach (var kv in formData)
        {
            if (string.Equals(kv.Key, field, StringComparison.OrdinalIgnoreCase))
            {
                value = kv.Value;
                return true;
            }
        }
        value = null;
        return false;
    }

    private static bool CompareEqual(object? left, object? right)
    {
        if (left == null && right == null) return true;
        if (left == null || right == null) return false;
        if (TryToDecimal(left, out var ld) && TryToDecimal(right, out var rd))
            return ld == rd;
        return string.Equals(Convert.ToString(left, CultureInfo.InvariantCulture),
            Convert.ToString(right, CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase);
    }

    private static int? CompareOrdered(object? left, object? right)
    {
        if (TryToDecimal(left, out var ld) && TryToDecimal(right, out var rd))
            return ld.CompareTo(rd);
        var ls = Convert.ToString(left, CultureInfo.InvariantCulture);
        var rs = Convert.ToString(right, CultureInfo.InvariantCulture);
        if (ls == null || rs == null) return null;
        return string.Compare(ls, rs, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryToDecimal(object? v, out decimal d)
    {
        switch (v)
        {
            case null:
                d = 0;
                return false;
            case decimal x:
                d = x;
                return true;
            case int i:
                d = i;
                return true;
            case long l:
                d = l;
                return true;
            case double dbl:
                d = (decimal)dbl;
                return true;
            case float f:
                d = (decimal)f;
                return true;
            case JsonElement je:
                if (je.ValueKind == JsonValueKind.Number && je.TryGetDecimal(out d))
                    return true;
                if (je.ValueKind == JsonValueKind.String &&
                    decimal.TryParse(je.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return true;
                d = 0;
                return false;
            default:
                return decimal.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out d);
        }
    }

    private static object? JsonElementToObject(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Undefined => null,
        _ => el.GetRawText()
    };
}
