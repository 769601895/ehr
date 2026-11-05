namespace Admin.NET.Application;

public static class HrEmpImportConsts
{
    public const string ModeAdd = "Add";
    public const string ModeUpdate = "Update";
    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";
    public const string EmpIdFieldKey = "empId";
    public const string FixedNameAdd = "新增模板";
    public const string FixedNameUpdate = "修改模板";
    public const int MaxRows = 1000;
    public const long MaxFileBytes = 5 * 1024 * 1024;

    public static string FixedName(string mode)
        => string.Equals(mode, ModeUpdate, StringComparison.OrdinalIgnoreCase) ? FixedNameUpdate : FixedNameAdd;

    /// <summary>禁止进入导入字段池的字段（状态类）</summary>
    public static readonly HashSet<string> BannedFieldKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "empState",
        "empFree"
    };
}
