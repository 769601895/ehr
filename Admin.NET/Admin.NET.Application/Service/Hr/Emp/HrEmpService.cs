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
    private readonly HrEmpIdGenerator _empIdGen;
    private readonly HrEmpIdCardService _idCardSvc;
    private readonly HrUserCcScopeService _userCcScope;

    public HrEmpService(
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        HrEmpIdGenerator empIdGen,
        HrEmpIdCardService idCardSvc,
        HrUserCcScopeService userCcScope)
    {
        _empRep = empRep;
        _ccRep = ccRep;
        _empIdGen = empIdGen;
        _idCardSvc = idCardSvc;
        _userCcScope = userCcScope;
    }

    /// <summary>
    /// 人员资料分页（服务端排序）
    /// </summary>
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("人员资料分页")]
    public async Task<SqlSugarPagedList<HrEmpOutput>> Page(HrEmpPageInput input)
    {
        var q = await BuildQuery(input);
        var page = await ApplyEmpSort(q, input).ToPagedListAsync(input.Page, input.PageSize);
        var items = page.Items.Select(ToOutput).ToList();
        await FillCcDesc(items);
        foreach (var item in items)
            item.Age = CalcAge(item.EmpBirth);
        return new SqlSugarPagedList<HrEmpOutput>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            Total = page.Total,
            TotalPages = page.TotalPages,
            Items = items,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage
        };
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
    /// 按工号精确查询管辖内员工（表单输入工号后回填姓名/部门；不在管辖则失败）
    /// </summary>
    [ApiDescriptionSettings(Name = "LookupByEmpId"), HttpPost]
    [DisplayName("按工号查管辖内员工")]
    public async Task<HrEmpOutput> LookupByEmpId(HrEmpLookupInput input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.EmpId))
            throw Oops.Oh("工号不能为空");

        var empId = input.EmpId.Trim();
        var q = _empRep.AsQueryable().Where(u => u.EmpId == empId);
        if (input.EmpState.HasValue)
            q = q.Where(u => u.EmpState == input.EmpState.Value);

        q = await _userCcScope.ApplyEmpScopeAsync(q);
        var rows = await q.Take(1).ToListAsync();
        var emp = rows.FirstOrDefault()
            ?? throw Oops.Oh("人员不存在或不在管辖范围内");

        var output = ToOutput(emp);
        await FillCcDesc([output]);
        output.Age = CalcAge(output.EmpBirth);
        return output;
    }

    /// <summary>
    /// 预览工号（按部门归属公司参数；不占号）
    /// </summary>
    [ApiDescriptionSettings(Name = "PreviewEmpId"), HttpPost]
    [DisplayName("预览工号")]
    public Task<HrEmpIdPreviewOutput> PreviewEmpId(HrEmpIdPreviewInput input)
        => _empIdGen.PreviewAsync(input?.CcCode);

    /// <summary>
    /// 二代证读卡功能开关（公司参数 IdCardV2Enabled，按当前用户管辖部门同属厂别）
    /// </summary>
    [ApiDescriptionSettings(Name = "IdCardFeature"), HttpPost]
    [DisplayName("二代证读卡功能")]
    public Task<HrEmpIdCardFeatureOutput> IdCardFeature(HrEmpIdCardFeatureInput input)
        => _idCardSvc.GetFeatureAsync(input);

    /// <summary>
    /// 解析身份证：校验（中国）并带出生日/性别/籍贯省市码
    /// </summary>
    [ApiDescriptionSettings(Name = "ParseIdCard"), HttpPost]
    [DisplayName("解析身份证")]
    public Task<HrEmpIdCardParseOutput> ParseIdCard(HrEmpIdCardParseInput input)
        => _idCardSvc.ParseAsync(input);

    /// <summary>
    /// 新增人员资料
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("新增人员资料")]
    public async Task<string> Add(HrEmpSaveInput input)
    {
        await EnsureCcValid(input.CcCode);
        await _userCcScope.EnsureCcInScopeAsync(input.CcCode);
        await _idCardSvc.ApplyOnSaveAsync(input);

        // 有填用工号；空则按公司参数自动发号（手工模式必须手填）
        string empId;
        if (!string.IsNullOrWhiteSpace(input.EmpId))
        {
            empId = input.EmpId.Trim();
            await _empIdGen.EnsureEmpIdAvailableAsync(empId);
        }
        else
        {
            empId = await _empIdGen.AllocateAsync(input.CcCode)
                ?? throw Oops.Oh("工号不能为空");
        }

        await EnsureCardUnique(input.EmpCard, null);

        var entity = new HrEmployee
        {
            IEmpId = await NextIEmpId(),
            EmpId = empId,
            EmpFree = 0,
            EmpState = input.EmpState ?? 1
        };
        ApplySaveFields(entity, input);
        entity.EmpId = empId;
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
        if (string.IsNullOrWhiteSpace(input.EmpId))
            throw Oops.Oh("工号不能为空");
        var empId = input.EmpId.Trim();
        var entity = await _empRep.GetFirstAsync(u => u.EmpId == empId)
            ?? throw Oops.Oh("人员不存在");
        await EnsureCcValid(input.CcCode);
        await _userCcScope.EnsureCcInScopeAsync(input.CcCode);
        await _idCardSvc.ApplyOnSaveAsync(input);
        await EnsureCardUnique(input.EmpCard, empId);

        ApplySaveFields(entity, input);
        if (input.EmpState.HasValue)
            entity.EmpState = input.EmpState;
        // 复合主键 iempid+emp_id：显式 Where 绑定参数；勿把主键放进 IgnoreColumns（会丢 @iempid）
        var iEmpId = entity.IEmpId;
        await _empRep.AsUpdateable(entity)
            .Where(u => u.IEmpId == iEmpId && u.EmpId == empId)
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
        var iEmpId = entity.IEmpId;
        await _empRep.AsUpdateable(entity)
            .Where(u => u.IEmpId == iEmpId && u.EmpId == empId)
            .UpdateColumns(u => new { u.EmpState })
            .ExecuteCommandAsync();
    }

    private async Task<ISugarQueryable<HrEmployee>> BuildQuery(HrEmpPageInput input)
    {
        var q = _empRep.AsQueryable();
        q = await _userCcScope.ApplyEmpScopeAsync(q);

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
                || (u.EmpName != null && u.EmpName.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(input.CcCode))
        {
            // 允许点选共同父级；结果仍被 ApplyEmpScope 限制在管辖内
            await _userCcScope.EnsureCcNavigableAsync(input.CcCode);
            var cc = await _ccRep.GetFirstAsync(u => u.CcCode == input.CcCode.Trim())
                ?? throw Oops.Oh("部门不存在");
            var level = cc.CcLevel ?? "";
            var codes = await _ccRep.AsQueryable()
                .Where(u => u.CcLevel != null && u.CcLevel.StartsWith(level))
                .Select(u => u.CcCode)
                .ToListAsync();
            q = q.Where(u => u.CcCode != null && codes.Contains(u.CcCode));
        }

        // 高级查询（白名单）；与左树部门/状态条件并且
        if (input.Filter != null)
            q = HrAdvancedFilter.ApplyEmp(q, input.Filter);

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
        entity.EmpCity = TrimOrNull(input.EmpCity);
        entity.EmpCountry = TrimOrNull(input.EmpCountry);
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
        entity.EmpCardOverdate = input.EmpCardOverdate;
        // 发证日与到期日均有值时按月差生成期限；否则保留入参
        entity.EmpCardLimit = CalcCardLimitMonths(input.EmpCardDate, input.EmpCardOverdate) ?? input.EmpCardLimit;
        entity.EmpType = TrimOrNull(input.EmpType);
        entity.EmpWorkDate = input.EmpWorkDate;
        entity.EmpInDateGroup = input.EmpInDateGroup;
        entity.EmpPic = TrimOrNull(input.EmpPic);
        HrEmpCharFields.ApplyFromInput(entity, input);
    }

    private static string? TrimOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static HrEmpOutput ToOutput(HrEmployee u)
    {
        var o = new HrEmpOutput
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
            EmpCity = u.EmpCity,
            EmpCountry = u.EmpCountry,
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
        HrEmpCharFields.CopyFromEntity(u, o);
        return o;
    }

    private static int? CalcAge(DateTime? birth)
    {
        if (birth == null) return null;
        var today = DateTime.Today;
        var age = today.Year - birth.Value.Year;
        if (birth.Value.Date > today.AddYears(-age)) age--;
        return age < 0 ? 0 : age;
    }

    /// <summary>身份证期限（月）= 到期日 − 发证日的日历月差；缺一端则返回 null。</summary>
    private static int? CalcCardLimitMonths(DateTime? issueDate, DateTime? expireDate)
    {
        if (issueDate == null || expireDate == null) return null;
        var months = (expireDate.Value.Year - issueDate.Value.Year) * 12
                     + (expireDate.Value.Month - issueDate.Value.Month);
        return months < 0 ? 0 : months;
    }
}
