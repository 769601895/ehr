namespace Admin.NET.Application;

/// <summary>
/// 模块注册。未知模块的读写都拒绝。
/// </summary>
public static class HrUiModuleCatalog
{
    public sealed record Module(string Code, string Name, string MainTable, string[] Scenes);

    public static readonly Module HrEmp = new(HrUiModuleCodes.HrEmp, "人事资料", "h_emp_mstr", ["list", "form", "monthSnap"]);
    public static readonly Module HrEmpChg = new(HrUiModuleCodes.HrEmpChg, "异动管理", "hr_emp_chg", ["list", "form"]);

    private static readonly Module[] All = [HrEmp, HrEmpChg];

    public static IReadOnlyList<Module> List() => All;

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

    /// <summary>按模块已登记 Scenes 校验场景。</summary>
    public static void EnsureScene(Module module, string scene)
    {
        if (module == null) throw Oops.Oh("HR_UI_MODULE_UNKNOWN");
        if (string.IsNullOrWhiteSpace(scene))
            throw Oops.Oh("HR_UI_SCENE_UNSUPPORTED");
        var s = scene.Trim();
        if (!module.Scenes.Any(x => x.Equals(s, StringComparison.OrdinalIgnoreCase)))
            throw Oops.Oh("HR_UI_SCENE_UNSUPPORTED");
    }

    /// <summary>无模块上下文时：场景须落在任一已登记模块的 Scenes 并集中。</summary>
    public static void EnsureScene(string scene)
    {
        if (string.IsNullOrWhiteSpace(scene))
            throw Oops.Oh("HR_UI_SCENE_UNSUPPORTED");
        var s = scene.Trim();
        if (!All.SelectMany(m => m.Scenes).Any(x => x.Equals(s, StringComparison.OrdinalIgnoreCase)))
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
