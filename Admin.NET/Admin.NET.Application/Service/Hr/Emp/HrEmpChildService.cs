using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 人员补充信息（家庭、教育、履历、职称）。每节一张表，按行保存。
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpChild", Order = 473, Description = "人员补充信息")]
public class HrEmpChildService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrEmpFamily> _familyRep;
    private readonly SqlSugarRepository<HrEmpEdu> _eduRep;
    private readonly SqlSugarRepository<HrEmpCareer> _careerRep;
    private readonly SqlSugarRepository<HrEmpTitle> _titleRep;

    public HrEmpChildService(
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrEmpFamily> familyRep,
        SqlSugarRepository<HrEmpEdu> eduRep,
        SqlSugarRepository<HrEmpCareer> careerRep,
        SqlSugarRepository<HrEmpTitle> titleRep)
    {
        _empRep = empRep;
        _familyRep = familyRep;
        _eduRep = eduRep;
        _careerRep = careerRep;
        _titleRep = titleRep;
    }

    [HttpGet]
    [DisplayName("补充信息行数")]
    public async Task<HrEmpChildCountOutput> Count([FromQuery] string empId)
    {
        empId = RequireEmpId(empId);
        return new HrEmpChildCountOutput
        {
            Family = await _familyRep.CountAsync(u => u.EmpId == empId),
            Edu = await _eduRep.CountAsync(u => u.EmpId == empId),
            Work = await _careerRep.CountAsync(u => u.EmpId == empId),
            Title = await _titleRep.CountAsync(u => u.EmpId == empId)
        };
    }

    [HttpGet]
    [DisplayName("补充信息列表")]
    public async Task<List<Dictionary<string, object?>>> List([FromQuery] string section, [FromQuery] string empId)
    {
        empId = RequireEmpId(empId);
        return Normalize(section) switch
        {
            "family" => (await _familyRep.AsQueryable().Where(u => u.EmpId == empId).OrderBy(u => u.SortNo).OrderBy(u => u.Id).ToListAsync())
                .Select(u => Row(u.Id, u.SortNo, ("relation", u.Relation), ("name", u.MemberName), ("workplace", u.Workplace), ("duty", u.Duty))).ToList(),
            "edu" => (await _eduRep.AsQueryable().Where(u => u.EmpId == empId).OrderBy(u => u.SortNo).OrderBy(u => u.Id).ToListAsync())
                .Select(u => Row(u.Id, u.SortNo, ("startDate", DateText(u.StartDate)), ("endDate", DateText(u.EndDate)), ("school", u.School), ("major", u.Major), ("degree", u.Degree), ("highest", u.IsHighest ? "是" : "否"), ("remark", u.Remark))).ToList(),
            "work" => (await _careerRep.AsQueryable().Where(u => u.EmpId == empId).OrderBy(u => u.SortNo).OrderBy(u => u.Id).ToListAsync())
                .Select(u => Row(u.Id, u.SortNo, ("startDate", DateText(u.StartDate)), ("endDate", DateText(u.EndDate)), ("workplace", u.Workplace), ("deptName", u.DeptName), ("postName", u.PostName), ("remark", u.Remark))).ToList(),
            "title" => (await _titleRep.AsQueryable().Where(u => u.EmpId == empId).OrderBy(u => u.SortNo).OrderBy(u => u.Id).ToListAsync())
                .Select(u => Row(u.Id, u.SortNo, ("titleName", u.TitleName), ("titleLevel", u.TitleLevel), ("certNo", u.CertNo), ("obtainDate", DateText(u.ObtainDate)), ("remark", u.Remark))).ToList(),
            _ => throw Oops.Oh("未知的补充信息")
        };
    }

    [HttpPost]
    [DisplayName("保存补充信息行")]
    public async Task<string> Save(HrEmpChildSaveInput input)
    {
        var empId = RequireEmpId(input.EmpId);
        if (!await _empRep.IsAnyAsync(u => u.EmpId == empId))
            throw Oops.Oh("人员不存在");
        var values = input.Values ?? new Dictionary<string, string?>();
        var id = ParseId(input.Id);
        return Normalize(input.Section) switch
        {
            "family" => await SaveFamily(empId, id, values),
            "edu" => await SaveEdu(empId, id, values),
            "work" => await SaveCareer(empId, id, values),
            "title" => await SaveTitle(empId, id, values),
            _ => throw Oops.Oh("未知的补充信息")
        };
    }

    [HttpPost]
    [DisplayName("删除补充信息行")]
    public async Task Delete(HrEmpChildDeleteInput input)
    {
        var empId = RequireEmpId(input.EmpId);
        var id = ParseId(input.Id) ?? throw Oops.Oh("记录不存在");
        var n = Normalize(input.Section) switch
        {
            "family" => await _familyRep.DeleteAsync(u => u.Id == id && u.EmpId == empId),
            "edu" => await _eduRep.DeleteAsync(u => u.Id == id && u.EmpId == empId),
            "work" => await _careerRep.DeleteAsync(u => u.Id == id && u.EmpId == empId),
            "title" => await _titleRep.DeleteAsync(u => u.Id == id && u.EmpId == empId),
            _ => throw Oops.Oh("未知的补充信息")
        };
        if (!n) throw Oops.Oh("记录不存在");
    }

    [HttpPost]
    [DisplayName("调整补充信息顺序")]
    public async Task Sort(HrEmpChildSortInput input)
    {
        var empId = RequireEmpId(input.EmpId);
        var ids = (input.Ids ?? []).Select(s => ParseId(s) ?? throw Oops.Oh("排序数据无效")).ToList();
        var table = Normalize(input.Section) switch
        {
            "family" => "h_emp_family",
            "edu" => "h_emp_edu",
            "work" => "h_emp_career",
            "title" => "h_emp_title",
            _ => throw Oops.Oh("未知的补充信息")
        };
        var existing = await _empRep.Context.Ado.SqlQueryAsync<long>($"SELECT id FROM {table} WHERE emp_id=@empId", new { empId });
        if (existing.Count != ids.Count || ids.Any(id => !existing.Contains(id)))
            throw Oops.Oh("排序数据已变化，请刷新");
        for (var i = 0; i < ids.Count; i++)
        {
            await _empRep.Context.Ado.ExecuteCommandAsync(
                $"UPDATE {table} SET sort_no=@sortNo WHERE id=@id AND emp_id=@empId",
                new { sortNo = i + 1, id = ids[i], empId });
        }
    }

    private async Task<string> SaveFamily(string empId, long? id, Dictionary<string, string?> values)
    {
        var name = Text(values, "name", 50);
        if (string.IsNullOrWhiteSpace(name)) throw Oops.Oh("姓名不能为空");
        if (id == null)
        {
            var entity = new HrEmpFamily
            {
                Id = YitIdHelper.NextId(),
                EmpId = empId,
                SortNo = await NextSort(_familyRep.AsQueryable().Where(u => u.EmpId == empId).Select(u => u.SortNo)),
                Relation = Text(values, "relation", 20),
                MemberName = name,
                Workplace = Text(values, "workplace", 100),
                Duty = Text(values, "duty", 50)
            };
            await _familyRep.InsertAsync(entity);
            return entity.Id.ToString();
        }
        var row = await _familyRep.GetFirstAsync(u => u.Id == id && u.EmpId == empId) ?? throw Oops.Oh("记录不存在");
        row.Relation = Text(values, "relation", 20);
        row.MemberName = name;
        row.Workplace = Text(values, "workplace", 100);
        row.Duty = Text(values, "duty", 50);
        await _familyRep.UpdateAsync(row);
        return row.Id.ToString();
    }

    private async Task<string> SaveEdu(string empId, long? id, Dictionary<string, string?> values)
    {
        var school = Text(values, "school", 100);
        if (string.IsNullOrWhiteSpace(school)) throw Oops.Oh("院校名称不能为空");
        if (id == null)
        {
            var entity = new HrEmpEdu
            {
                Id = YitIdHelper.NextId(),
                EmpId = empId,
                SortNo = await NextSort(_eduRep.AsQueryable().Where(u => u.EmpId == empId).Select(u => u.SortNo)),
                StartDate = Date(values, "startDate"),
                EndDate = Date(values, "endDate"),
                School = school,
                Major = Text(values, "major", 100),
                Degree = Text(values, "degree", 20),
                IsHighest = Yes(values, "highest"),
                Remark = Text(values, "remark", 500)
            };
            await _eduRep.InsertAsync(entity);
            return entity.Id.ToString();
        }
        var row = await _eduRep.GetFirstAsync(u => u.Id == id && u.EmpId == empId) ?? throw Oops.Oh("记录不存在");
        row.StartDate = Date(values, "startDate");
        row.EndDate = Date(values, "endDate");
        row.School = school;
        row.Major = Text(values, "major", 100);
        row.Degree = Text(values, "degree", 20);
        row.IsHighest = Yes(values, "highest");
        row.Remark = Text(values, "remark", 500);
        await _eduRep.UpdateAsync(row);
        return row.Id.ToString();
    }

    private async Task<string> SaveCareer(string empId, long? id, Dictionary<string, string?> values)
    {
        var workplace = Text(values, "workplace", 100);
        if (string.IsNullOrWhiteSpace(workplace)) throw Oops.Oh("工作单位不能为空");
        if (id == null)
        {
            var entity = new HrEmpCareer
            {
                Id = YitIdHelper.NextId(),
                EmpId = empId,
                SortNo = await NextSort(_careerRep.AsQueryable().Where(u => u.EmpId == empId).Select(u => u.SortNo)),
                StartDate = Date(values, "startDate"),
                EndDate = Date(values, "endDate"),
                Workplace = workplace,
                DeptName = Text(values, "deptName", 100),
                PostName = Text(values, "postName", 50),
                Remark = Text(values, "remark", 500)
            };
            await _careerRep.InsertAsync(entity);
            return entity.Id.ToString();
        }
        var row = await _careerRep.GetFirstAsync(u => u.Id == id && u.EmpId == empId) ?? throw Oops.Oh("记录不存在");
        row.StartDate = Date(values, "startDate");
        row.EndDate = Date(values, "endDate");
        row.Workplace = workplace;
        row.DeptName = Text(values, "deptName", 100);
        row.PostName = Text(values, "postName", 50);
        row.Remark = Text(values, "remark", 500);
        await _careerRep.UpdateAsync(row);
        return row.Id.ToString();
    }

    private async Task<string> SaveTitle(string empId, long? id, Dictionary<string, string?> values)
    {
        var titleName = Text(values, "titleName", 50);
        if (string.IsNullOrWhiteSpace(titleName)) throw Oops.Oh("职称名称不能为空");
        if (id == null)
        {
            var entity = new HrEmpTitle
            {
                Id = YitIdHelper.NextId(),
                EmpId = empId,
                SortNo = await NextSort(_titleRep.AsQueryable().Where(u => u.EmpId == empId).Select(u => u.SortNo)),
                TitleName = titleName,
                TitleLevel = Text(values, "titleLevel", 20),
                CertNo = Text(values, "certNo", 50),
                ObtainDate = Date(values, "obtainDate"),
                Remark = Text(values, "remark", 500)
            };
            await _titleRep.InsertAsync(entity);
            return entity.Id.ToString();
        }
        var row = await _titleRep.GetFirstAsync(u => u.Id == id && u.EmpId == empId) ?? throw Oops.Oh("记录不存在");
        row.TitleName = titleName;
        row.TitleLevel = Text(values, "titleLevel", 20);
        row.CertNo = Text(values, "certNo", 50);
        row.ObtainDate = Date(values, "obtainDate");
        row.Remark = Text(values, "remark", 500);
        await _titleRep.UpdateAsync(row);
        return row.Id.ToString();
    }

    private static async Task<int> NextSort(ISugarQueryable<int> sortNos)
    {
        var list = await sortNos.ToListAsync();
        return list.Count == 0 ? 1 : list.Max() + 1;
    }

    private static Dictionary<string, object?> Row(long id, int sortNo, params (string Key, object? Value)[] cells)
    {
        var row = new Dictionary<string, object?>
        {
            ["id"] = id.ToString(),
            ["sortNo"] = sortNo
        };
        foreach (var (key, value) in cells)
            row[key] = value;
        return row;
    }

    private static string Normalize(string? section) => (section ?? "").Trim().ToLowerInvariant();

    private static string RequireEmpId(string? empId)
    {
        if (string.IsNullOrWhiteSpace(empId)) throw Oops.Oh("工号不能为空");
        return empId.Trim();
    }

    private static long? ParseId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        if (!long.TryParse(id.Trim(), out var value) || value <= 0) throw Oops.Oh("记录不存在");
        return value;
    }

    private static string? Text(Dictionary<string, string?> values, string key, int max)
    {
        if (!values.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw)) return null;
        var text = raw.Trim();
        return text.Length <= max ? text : text[..max];
    }

    private static DateTime? Date(Dictionary<string, string?> values, string key)
    {
        if (!values.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw)) return null;
        if (!DateTime.TryParse(raw.Trim(), out var value)) throw Oops.Oh("日期无效");
        return value.Date;
    }

    private static bool Yes(Dictionary<string, string?> values, string key)
    {
        if (!values.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw)) return false;
        return raw.Trim() is "是" or "1" or "true" or "True" or "Y" or "y";
    }

    private static string? DateText(DateTime? value) => value?.ToString("yyyy-MM-dd");
}
