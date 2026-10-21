using Admin.NET.Application;
using Xunit;

namespace Admin.NET.Test.Hr;

/// <summary>
/// 不依赖 Furion 测试宿主；仅验证纯函数与种子约定。
/// </summary>
public class HrUiFieldPoolLogicTests
{
    [Theory]
    [InlineData("emp_id", "empId")]
    [InlineData("empId", "empId")]
    [InlineData("EmpId", "empId")]
    [InlineData("EMP_ID", "empId")]
    [InlineData("cc_code", "ccCode")]
    [InlineData("emp_kq_lea_date", "empKqLeaDate")]
    public void FromColumn_MatchesEmpDto(string column, string expected)
    {
        Assert.Equal(expected, HrUiFieldKeyUtil.FromColumn(column));
    }

    [Fact]
    public void Unique_AppendsSuffix()
    {
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "empId" };
        Assert.Equal("empId_2", HrUiFieldKeyUtil.Unique("empId", used));
    }

    [Fact]
    public void ResolveFieldLabel_LayoutChineseDoesNotBlockEnglishPool()
    {
        var layout = new Dictionary<string, string> { ["zh-CN"] = "员工编号" };
        var field = new Dictionary<string, string> { ["en"] = "Emp No" };
        Assert.Equal("Emp No", HrUiI18nResolver.ResolveFieldLabel("empId", "工号", "en", layout, field));
        Assert.Equal("员工编号", HrUiI18nResolver.ResolveFieldLabel("empId", "工号", "zh-CN", layout, field));
    }

    [Fact]
    public void ResolveFieldLabel_FallsBackToPoolThenKey()
    {
        Assert.Equal("工号", HrUiI18nResolver.ResolveFieldLabel("empId", "工号", "th", null, null));
        Assert.Equal("empId", HrUiI18nResolver.ResolveFieldLabel("empId", "  ", "th", null, new Dictionary<string, string> { ["th"] = "  " }));
    }

    [Fact]
    public void FormatCodeLabel_DoesNotRepeatValue()
    {
        Assert.Equal("1 男", HrUiI18nResolver.FormatCodeLabel("1", "男"));
        Assert.Equal("1", HrUiI18nResolver.FormatCodeLabel("1", "1"));
        Assert.Equal("1", HrUiI18nResolver.FormatCodeLabel("1", ""));
        Assert.Equal("Male", HrUiI18nResolver.ResolveCodeDesc("1", "男", "en", new Dictionary<string, string> { ["en"] = "Male" }));
        Assert.Equal("1 Male", HrUiI18nResolver.FormatCodeLabel("1", HrUiI18nResolver.ResolveCodeDesc("1", "男", "en", new Dictionary<string, string> { ["en"] = "Male" })));
    }

    [Fact]
    public void Sync_DoesNotTouchComputedOrRewriteLabel()
    {
        var existing = new List<HrUiSyncRow>
        {
            new() { Id = 1, FieldKey = "empId", Origin = "physical", DbColumn = "emp_id", Enabled = true, DataType = "nvarchar", IsNullable = false, TableName = "h_emp_mstr" },
            new() { Id = 2, FieldKey = "age", Origin = "computed", DbColumn = null, Enabled = true, DataType = "int", TableName = null },
            new() { Id = 3, FieldKey = "ccDesc", Origin = "lookup", DbColumn = null, Enabled = true },
            new() { Id = 4, FieldKey = "gone", Origin = "physical", DbColumn = "gone_col", Enabled = true, DataType = "nvarchar", TableName = "h_emp_mstr" },
            new() { Id = 5, FieldKey = "back", Origin = "physical", DbColumn = "back_col", Enabled = false, DataType = "int", IsNullable = true, TableName = "old" },
        };
        var columns = new List<HrUiSyncColumn>
        {
            new() { DbColumn = "emp_id", DataType = "nvarchar", IsNullable = false },
            new() { DbColumn = "back_col", DataType = "nvarchar", IsNullable = false, Description = "回来了" },
            new() { DbColumn = "new_col", DataType = "datetime", IsNullable = true, Description = "新列" },
        };

        var plan = HrUiFieldSyncPlanner.Plan("h_emp_mstr", existing, columns);

        Assert.Single(plan.Inserts);
        Assert.Equal("newCol", plan.Inserts[0].FieldKey);
        Assert.Equal("date", plan.Inserts[0].ControlType);
        Assert.DoesNotContain(plan.DisableIds, id => id == 2 || id == 3);
        Assert.Contains(4L, plan.DisableIds);
        Assert.Contains(5L, plan.ReenableIds);
        Assert.Contains(plan.Structural, s => s.Id == 5 && s.DataType == "nvarchar");
        Assert.DoesNotContain(plan.Structural, s => s.Id == 1);
    }

    [Fact]
    public void CultureNormalize_UnknownFallsBack()
    {
        Assert.Equal("zh-CN", HrUiCulture.Normalize(null));
        Assert.Equal("zh-CN", HrUiCulture.Normalize("ja"));
        Assert.Equal("en", HrUiCulture.Normalize("en-US"));
        Assert.Equal("zh-TW", HrUiCulture.Normalize("zh-TW"));
    }

    [Fact]
    public void SeedSql_CoversPoolKeysAndScenes()
    {
        var root = FindRepo();
        var def = File.ReadAllText(Path.Combine(root, "doc/sql/2026-10-17/hr_ui_field_def_seed_hrEmp.sql"));
        var scene = File.ReadAllText(Path.Combine(root, "doc/sql/2026-10-17/hr_ui_field_scene_seed_hrEmp.sql"));
        var keys = new[]
        {
            "empId","empName","empCard","empCardOverdate","empCardDate","empCardLimit","empSex","empBirth","age","empRgt",
            "empNation","empProvince","empResidence","empMarry","empAddr","empHome","empAddrter","empPic","ccCode","ccDesc",
            "dutyId","postId","empWorkDate","empInDateGroup","empInDate","empBegDate","empTryDate","empFromDate","empLongmon",
            "empKqIndate","empKqLeaDate","empType","empSal","empState"
        };
        foreach (var key in keys)
            Assert.Contains($"N'{key}'", def);
        Assert.Contains("N'age', N'computed'", def.Replace(" ", ""));
        Assert.Contains("N'ccDesc', N'lookup'", def.Replace(" ", ""));
        Assert.Contains("N'ccDesc', N'list'", scene);
        Assert.DoesNotContain("N'ccDesc', N'form'", scene);
        Assert.Contains("N'empPic', N'form'", scene);
        Assert.DoesNotContain("N'empPic', N'list'", scene);
        Assert.DoesNotContain("GetPool", File.ReadAllText(Path.Combine(root, "Admin.NET/Admin.NET.Application/Service/Hr/Layout/HrUiLayoutService.cs")));
    }

    private static string FindRepo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "doc", "sql", "2026-10-17", "hr_ui_field_def_seed_hrEmp.sql")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("repo root");
    }
}
