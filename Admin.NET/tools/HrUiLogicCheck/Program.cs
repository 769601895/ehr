using Admin.NET.Application;

var failed = 0;
void Check(bool ok, string name)
{
    if (ok) Console.WriteLine("PASS " + name);
    else
    {
        Console.WriteLine("FAIL " + name);
        failed++;
    }
}

Check(HrUiFieldKeyUtil.FromColumn("emp_id") == "empId", "FromColumn emp_id");
Check(HrUiFieldKeyUtil.FromColumn("EmpId") == "empId", "FromColumn EmpId");
Check(HrUiFieldKeyUtil.FromColumn("emp_kq_lea_date") == "empKqLeaDate", "FromColumn emp_kq_lea_date");
Check(HrUiFieldKeyUtil.Unique("empId", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "empId" }) == "empId_2", "Unique suffix");

var layout = new Dictionary<string, string> { ["zh-CN"] = "员工编号" };
var field = new Dictionary<string, string> { ["en"] = "Emp No" };
Check(HrUiI18nResolver.ResolveFieldLabel("empId", "工号", "en", layout, field) == "Emp No", "layout zh does not block en");
Check(HrUiI18nResolver.ResolveFieldLabel("empId", "工号", "zh-CN", layout, field) == "员工编号", "layout zh used");
Check(HrUiI18nResolver.FormatCodeLabel("1", "男") == "1 男", "code format");
Check(HrUiI18nResolver.FormatCodeLabel("1", "1") == "1", "code no double");
Check(HrUiCulture.Normalize("en-US") == "en", "culture en");
Check(HrUiCulture.Normalize("ja") == "zh-CN", "culture fallback");

var plan = HrUiFieldSyncPlanner.Plan("h_emp_mstr",
[
    new HrUiSyncRow { Id = 1, FieldKey = "empId", Origin = "physical", DbColumn = "emp_id", Enabled = true, DataType = "nvarchar", IsNullable = false, TableName = "h_emp_mstr" },
    new HrUiSyncRow { Id = 2, FieldKey = "age", Origin = "computed", Enabled = true },
    new HrUiSyncRow { Id = 3, FieldKey = "ccDesc", Origin = "lookup", Enabled = true },
    new HrUiSyncRow { Id = 4, FieldKey = "gone", Origin = "physical", DbColumn = "gone_col", Enabled = true, DataType = "nvarchar", TableName = "h_emp_mstr" },
],
[
    new HrUiSyncColumn { DbColumn = "emp_id", DataType = "nvarchar", IsNullable = false },
    new HrUiSyncColumn { DbColumn = "new_col", DataType = "datetime", IsNullable = true, Description = "新列" },
]);
Check(plan.Inserts.Count == 1 && plan.Inserts[0].FieldKey == "newCol", "sync insert");
Check(plan.DisableIds.Contains(4) && !plan.DisableIds.Contains(2) && !plan.DisableIds.Contains(3), "sync disable only physical missing");

var root = Directory.GetParent(AppContext.BaseDirectory);
while (root != null && !File.Exists(Path.Combine(root.FullName, "doc", "sql", "2026-10-17", "hr_ui_field_def_seed_hrEmp.sql")))
    root = root.Parent;
if (root == null) { Check(false, "find repo"); }
else
{
    var def = File.ReadAllText(Path.Combine(root.FullName, "doc/sql/2026-10-17/hr_ui_field_def_seed_hrEmp.sql"));
    var scene = File.ReadAllText(Path.Combine(root.FullName, "doc/sql/2026-10-17/hr_ui_field_scene_seed_hrEmp.sql"));
    var layoutSvc = File.ReadAllText(Path.Combine(root.FullName, "Admin.NET/Admin.NET.Application/Service/Hr/Layout/HrUiLayoutService.cs"));
    Check(def.Contains("N'age'") && def.Contains("N'computed'"), "seed age computed");
    Check(scene.Contains("N'ccDesc', N'list'") || scene.Contains("N'ccDesc',N'list'"), "scene ccDesc list");
    Check(!scene.Contains("N'ccDesc', N'form'") && !scene.Contains("N'ccDesc',N'form'"), "scene no ccDesc form");
    Check(!layoutSvc.Contains("GetPool"), "hardcoded GetPool removed");
}

Console.WriteLine(failed == 0 ? "ALL PASS" : $"FAILED {failed}");
return failed == 0 ? 0 : 1;
