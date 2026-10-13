using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 人员资料管理
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmp", Order = 472, Description = "人员资料")]
public class HrEmpService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;

    public HrEmpService(SqlSugarRepository<HrEmployee> empRep, SqlSugarRepository<CcMstr> ccRep)
    {
        _empRep = empRep;
        _ccRep = ccRep;
    }

    /// <summary>
    /// 人员资料分页（服务端排序）
    /// </summary>
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("人员资料分页")]
    public async Task<SqlSugarPagedList<HrEmpOutput>> Page(HrEmpPageInput input)
    {
        var q = await BuildQuery(input);
        var page = await ApplyEmpSort(q, input).Select(u => new HrEmpOutput
        {
            IEmpId = u.IEmpId,
            EmpId = u.EmpId,
            CcCode = u.CcCode,
            DutyId = u.DutyId,
            PostId = u.PostId,
            EmpName = u.EmpName,
            EmpState = u.EmpState,
            EmpFree = u.EmpFree,
            EmpInDate = u.EmpInDate,
            EmpBegDate = u.EmpBegDate,
            EmpTryDate = u.EmpTryDate,
            EmpFromDate = u.EmpFromDate,
            EmpSal = u.EmpSal,
            EmpSex = u.EmpSex,
            EmpCard = u.EmpCard,
            EmpBirth = u.EmpBirth,
            EmpRgt = u.EmpRgt,
            EmpProvince = u.EmpProvince,
            EmpResidence = u.EmpResidence,
            EmpNation = u.EmpNation,
            EmpMarry = u.EmpMarry,
            EmpHome = u.EmpHome,
            EmpAddr = u.EmpAddr,
            EmpAddrter = u.EmpAddrter,
            EmpLongmon = u.EmpLongmon,
            EmpKqLeaDate = u.EmpKqLeaDate,
            EmpKqIndate = u.EmpKqIndate,
            EmpCardDate = u.EmpCardDate,
            EmpCardLimit = u.EmpCardLimit,
            EmpType = u.EmpType,
            EmpCardOverdate = u.EmpCardOverdate,
            EmpWorkDate = u.EmpWorkDate,
            EmpInDateGroup = u.EmpInDateGroup,
            EmpPic = u.EmpPic,
            BsId = u.BsId
        }).ToPagedListAsync(input.Page, input.PageSize);

        await FillCcDesc(page.Items);
        foreach (var item in page.Items)
            item.Age = CalcAge(item.EmpBirth);
        return page;
    }

    /// <summary>
    /// 人员资料详情
    /// </summary>
    [DisplayName("人员资料详情")]
    public async Task<HrEmpOutput> Detail([FromQuery] string empId)
    {
        if (string.IsNullOrWhiteSpace(empId))
            throw Oops.Oh("工号不能为空");
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId.Trim())
            ?? throw Oops.Oh("人员不存在");
        var output = ToOutput(emp);
        await FillCcDesc([output]);
        output.Age = CalcAge(output.EmpBirth);
        return output;
    }

    /// <summary>
    /// 新增人员资料
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("新增人员资料")]
    public async Task<string> Add(HrEmpSaveInput input)
    {
        var empId = input.EmpId.Trim();
        if (await _empRep.IsAnyAsync(u => u.EmpId == empId))
            throw Oops.Oh("工号已存在");
        await EnsureCcValid(input.CcCode);
        await EnsureCardUnique(input.EmpCard, null);

        var entity = new HrEmployee
        {
            IEmpId = await NextIEmpId(),
            EmpId = empId,
            EmpFree = 0,
            EmpState = input.EmpState ?? 1
        };
        ApplySaveFields(entity, input);
        await _empRep.InsertAsync(entity);
        return empId;
    }

    /// <summary>
    /// 更新人员资料（工号不可改）
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新人员资料")]
    public async Task Update(HrEmpSaveInput input)
    {
        var empId = input.EmpId.Trim();
        var entity = await _empRep.GetFirstAsync(u => u.EmpId == empId)
            ?? throw Oops.Oh("人员不存在");
        await EnsureCcValid(input.CcCode);
        await EnsureCardUnique(input.EmpCard, empId);

        ApplySaveFields(entity, input);
        if (input.EmpState.HasValue)
            entity.EmpState = input.EmpState;
        await _empRep.AsUpdateable(entity)
            .IgnoreColumns(u => new { u.IEmpId, u.EmpId, u.BsId, u.EmpFree })
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 离职（软删除：emp_state=3）
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("人员离职")]
    public async Task Delete(HrEmpDeleteInput input)
    {
        var empId = input.EmpId.Trim();
        var entity = await _empRep.GetFirstAsync(u => u.EmpId == empId)
            ?? throw Oops.Oh("人员不存在");
        if (entity.EmpState == 3)
            return;
        entity.EmpState = 3;
        await _empRep.AsUpdateable(entity)
            .UpdateColumns(u => new { u.EmpState })
            .ExecuteCommandAsync();
    }

    private async Task<ISugarQueryable<HrEmployee>> BuildQuery(HrEmpPageInput input)
    {
        var q = _empRep.AsQueryable();

        if (input.EmpState.HasValue)
            q = q.Where(u => u.EmpState == input.EmpState.Value);

        if (!string.IsNullOrWhiteSpace(input.EmpId))
            q = q.Where(u => u.EmpId.Contains(input.EmpId.Trim()));

        if (!string.IsNullOrWhiteSpace(input.EmpName))
            q = q.Where(u => u.EmpName != null && u.EmpName.Contains(input.EmpName.Trim()));

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            var kw = input.Keyword.Trim();
            q = q.Where(u => u.EmpId.Contains(kw)
                || (u.EmpName != null && u.EmpName.Contains(kw))
                || (u.EmpCard != null && u.EmpCard.Contains(kw))
                || (u.EmpAddrter != null && u.EmpAddrter.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(input.CcCode))
        {
            var cc = await _ccRep.GetFirstAsync(u => u.CcCode == input.CcCode.Trim())
                ?? throw Oops.Oh("部门不存在");
            var level = cc.CcLevel ?? "";
            var codes = await _ccRep.AsQueryable()
                .Where(u => u.CcLevel != null && u.CcLevel.StartsWith(level))
                .Select(u => u.CcCode)
                .ToListAsync();
            q = q.Where(u => u.CcCode != null && codes.Contains(u.CcCode));
        }

        return q;
    }

    private static ISugarQueryable<HrEmployee> ApplyEmpSort(ISugarQueryable<HrEmployee> query, BasePageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Field))
            return query.OrderBy(u => u.EmpId);

        if (char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        // CcDesc 为联表展示字段，回退工号
        if (string.Equals(input.Field, "CcDesc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(input.Field, "Age", StringComparison.OrdinalIgnoreCase))
            return query.OrderBy(u => u.EmpId);

        return query.OrderBuilder(input, defaultSortField: nameof(HrEmployee.EmpId), descSort: false);
    }

    private async Task EnsureCcValid(string? ccCode)
    {
        if (string.IsNullOrWhiteSpace(ccCode)) return;
        var cc = await _ccRep.GetFirstAsync(u => u.CcCode == ccCode.Trim())
            ?? throw Oops.Oh("所属部门不存在");
        if (cc.CcEnable == 1)
            throw Oops.Oh("所属部门已停用");
    }

    private async Task EnsureCardUnique(string? empCard, string? excludeEmpId)
    {
        if (string.IsNullOrWhiteSpace(empCard)) return;
        var card = empCard.Trim();
        var exists = await _empRep.IsAnyAsync(u =>
            u.EmpCard == card && (excludeEmpId == null || u.EmpId != excludeEmpId));
        if (exists)
            throw Oops.Oh("身份证号已存在");
    }

    private async Task<long> NextIEmpId()
    {
        var max = await _empRep.AsQueryable().MaxAsync(u => u.IEmpId);
        return max <= 0 ? 1 : max + 1;
    }

    private async Task FillCcDesc(IEnumerable<HrEmpOutput> items)
    {
        var list = items as IList<HrEmpOutput> ?? items.ToList();
        var codes = list.Where(u => !string.IsNullOrWhiteSpace(u.CcCode))
            .Select(u => u.CcCode!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (codes.Count == 0) return;

        var map = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .Select(u => new { u.CcCode, u.CcDesc })
            .ToListAsync();
        var dict = map.ToDictionary(u => u.CcCode, u => u.CcDesc, StringComparer.OrdinalIgnoreCase);
        foreach (var item in list)
        {
            if (!string.IsNullOrWhiteSpace(item.CcCode) && dict.TryGetValue(item.CcCode, out var desc))
                item.CcDesc = desc;
        }
    }

    private static void ApplySaveFields(HrEmployee entity, HrEmpSaveInput input)
    {
        entity.CcCode = string.IsNullOrWhiteSpace(input.CcCode) ? null : input.CcCode.Trim();
        entity.DutyId = TrimOrNull(input.DutyId);
        entity.PostId = TrimOrNull(input.PostId);
        entity.EmpName = input.EmpName.Trim();
        entity.EmpInDate = input.EmpInDate;
        entity.EmpBegDate = input.EmpBegDate;
        entity.EmpTryDate = input.EmpTryDate;
        entity.EmpFromDate = input.EmpFromDate;
        entity.EmpSal = TrimOrNull(input.EmpSal);
        entity.EmpSex = TrimOrNull(input.EmpSex);
        entity.EmpCard = TrimOrNull(input.EmpCard);
        entity.EmpBirth = input.EmpBirth;
        entity.EmpRgt = TrimOrNull(input.EmpRgt);
        entity.EmpProvince = TrimOrNull(input.EmpProvince);
        entity.EmpResidence = TrimOrNull(input.EmpResidence);
        entity.EmpNation = TrimOrNull(input.EmpNation);
        entity.EmpMarry = TrimOrNull(input.EmpMarry);
        entity.EmpHome = TrimOrNull(input.EmpHome);
        entity.EmpAddr = TrimOrNull(input.EmpAddr);
        entity.EmpAddrter = TrimOrNull(input.EmpAddrter);
        entity.EmpLongmon = input.EmpLongmon;
        entity.EmpKqLeaDate = input.EmpKqLeaDate;
        entity.EmpKqIndate = input.EmpKqIndate;
        entity.EmpCardDate = input.EmpCardDate;
        entity.EmpCardLimit = input.EmpCardLimit;
        entity.EmpType = TrimOrNull(input.EmpType);
        entity.EmpCardOverdate = input.EmpCardOverdate;
        entity.EmpWorkDate = input.EmpWorkDate;
        entity.EmpInDateGroup = input.EmpInDateGroup;
        entity.EmpPic = TrimOrNull(input.EmpPic);
    }

    private static string? TrimOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static HrEmpOutput ToOutput(HrEmployee u) => new()
    {
        IEmpId = u.IEmpId,
        EmpId = u.EmpId,
        CcCode = u.CcCode,
        DutyId = u.DutyId,
        PostId = u.PostId,
        EmpName = u.EmpName,
        EmpState = u.EmpState,
        EmpFree = u.EmpFree,
        EmpInDate = u.EmpInDate,
        EmpBegDate = u.EmpBegDate,
        EmpTryDate = u.EmpTryDate,
        EmpFromDate = u.EmpFromDate,
        EmpSal = u.EmpSal,
        EmpSex = u.EmpSex,
        EmpCard = u.EmpCard,
        EmpBirth = u.EmpBirth,
        EmpRgt = u.EmpRgt,
        EmpProvince = u.EmpProvince,
        EmpResidence = u.EmpResidence,
        EmpNation = u.EmpNation,
        EmpMarry = u.EmpMarry,
        EmpHome = u.EmpHome,
        EmpAddr = u.EmpAddr,
        EmpAddrter = u.EmpAddrter,
        EmpLongmon = u.EmpLongmon,
        EmpKqLeaDate = u.EmpKqLeaDate,
        EmpKqIndate = u.EmpKqIndate,
        EmpCardDate = u.EmpCardDate,
        EmpCardLimit = u.EmpCardLimit,
        EmpType = u.EmpType,
        EmpCardOverdate = u.EmpCardOverdate,
        EmpWorkDate = u.EmpWorkDate,
        EmpInDateGroup = u.EmpInDateGroup,
        EmpPic = u.EmpPic,
        BsId = u.BsId
    };

    private static int? CalcAge(DateTime? birth)
    {
        if (birth == null) return null;
        var today = DateTime.Today;
        var age = today.Year - birth.Value.Year;
        if (birth.Value.Date > today.AddYears(-age)) age--;
        return age < 0 ? 0 : age;
    }
}
