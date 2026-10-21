namespace Admin.NET.Application;

/// <summary>
/// 模块注册。一期只在内存登记 hrEmp，未知模块的读写都拒绝。
/// </summary>
public static class HrUiModuleCatalog
{
    public sealed record Module(string Code, string Name, string MainTable, string[] Scenes);

    public static readonly Module HrEmp = new(HrUiModuleCodes.HrEmp, "人事资料", "h_emp_mstr", ["list", "form"]);

    private static readonly Module[] All = [HrEmp];

    public static Module Require(string moduleCode)
    {
        if (string.IsNullOrWhiteSpace(moduleCode))
            throw Oops.Oh("HR_UI_MODULE_UNKNOWN");
        var code = moduleCode.Trim();
        if (code.Length > 64 || !IsModuleCode(code))
            throw Oops.Oh("HR_UI_MODULE_UNKNOWN");
        var found = All.FirstOrDefault(m => m.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return found ?? throw Oops.Oh("HR_UI_MODULE_UNKNOWN");
    }

    public static void EnsureScene(string scene)
    {
        if (scene is not ("list" or "form"))
            throw Oops.Oh("HR_UI_SCENE_UNSUPPORTED");
    }

    private static bool IsModuleCode(string code)
    {
        if (!char.IsLetter(code[0])) return false;
        for (var i = 1; i < code.Length; i++)
        {
            var c = code[i];
            if (!char.IsLetterOrDigit(c) && c != '_') return false;
        }
        return true;
    }
}
