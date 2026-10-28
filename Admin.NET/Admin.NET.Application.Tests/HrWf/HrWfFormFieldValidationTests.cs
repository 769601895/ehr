using Xunit;

namespace Admin.NET.Application.Tests.HrWf;

public class HrWfFormFieldValidationTests
{
    public static bool HasDuplicateFieldKeys(IEnumerable<string?> keys)
    {
        var list = keys.Where(u => !string.IsNullOrWhiteSpace(u)).Select(u => u!.Trim()).ToList();
        return list.Count != list.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }

    public static bool RequiredMissing(IEnumerable<(string Key, bool Required)> fields, IDictionary<string, object?> data)
    {
        foreach (var (key, required) in fields.Where(u => u.Required))
        {
            if (!data.TryGetValue(key, out var val) || val == null || (val is string s && string.IsNullOrWhiteSpace(s)))
                return true;
        }
        return false;
    }

    [Fact]
    public void DuplicateKeys_CaseInsensitive_Detected()
    {
        Assert.True(HasDuplicateFieldKeys(["subject", "Subject", "amount"]));
        Assert.False(HasDuplicateFieldKeys(["subject", "amount", "reason"]));
    }

    [Fact]
    public void Required_EmptyString_Fails()
    {
        Assert.True(RequiredMissing(
            [("subject", true), ("amount", false)],
            new Dictionary<string, object?> { ["subject"] = "  ", ["amount"] = 1 }));
    }

    [Fact]
    public void Required_Filled_Passes()
    {
        Assert.False(RequiredMissing(
            [("subject", true), ("reason", true)],
            new Dictionary<string, object?> { ["subject"] = "采购", ["reason"] = "耗材" }));
    }

    [Fact]
    public void OptionalMissing_Ok()
    {
        Assert.False(RequiredMissing([("amount", false)], new Dictionary<string, object?>()));
    }
}
