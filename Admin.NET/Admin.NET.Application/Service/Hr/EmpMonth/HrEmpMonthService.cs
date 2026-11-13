using System.Text.Json;
using Furion.DatabaseAccessor;
using MiniExcelLibs;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>人事月报</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpMonth", Order = 475, Description = "人事月报")]
public class HrEmpMonthService : IDynamicApiController, ITransient
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly SqlSugarRepository<HrEmpMonthBatch> _batchRep;
    private readonly SqlSugarRepository<HrEmpMonthLine> _lineRep;
    private readonly SqlSugarRepository<HrCompanyParam> _paramRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly HrEmpMonthGenerateService _generate;
    private readonly HrEmpMonthSnapBuilder _snap;
    private readonly HrCompanySiteResolveService _siteResolve;
    private readonly HrUserCcScopeService _scope;
    private readonly UserManager _userManager;

    public HrEmpMonthService(
        SqlSugarRepository<HrEmpMonthBatch> batchRep,
        SqlSugarRepository<HrEmpMonthLine> lineRep,
        SqlSugarRepository<HrCompanyParam> paramRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        HrEmpMonthGenerateService generate,
        HrEmpMonthSnapBuilder snap,
        HrCompanySiteResolveService siteResolve,
        HrUserCcScopeService scope,
        UserManager userManager)
    {
        _batchRep = batchRep;
        _lineRep = lineRep;
        _paramRep = paramRep;
        _empRep = empRep;
        _ccRep = ccRep;
        _generate = generate;
        _snap = snap;
        _siteResolve = siteResolve;
        _scope = scope;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("月报批次分页")]
    public async Task<SqlSugarPagedList<HrEmpMonthBatchOutput>> Page(HrEmpMonthBatchPageInput input)
    {
        input ??= new HrEmpMonthBatchPageInput();
        var keyword = input.Keyword?.Trim();
        var query = _batchRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.SiteCode), u => u.SiteCode == input.SiteCode)
            .WhereIF(!string.IsNullOrWhiteSpace(input.PayPeriod), u => u.PayPeriod == input.PayPeriod)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status), u => u.Status == input.Status)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.SiteCode.Contains(keyword!) || (u.SiteName != null && u.SiteName.Contains(keyword!))
                     || u.PayPeriod.Contains(keyword!));

        query = HrEmpMonthFieldRegistry.ApplyBatch(query, input.Filter);

        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "payPeriod";
            input.Order = input.DescStr ?? "desc";
        }

        var page = await query
            .OrderBuilder(input, defaultSortField: "PayPeriod", descSort: true)
            .Select(u => new HrEmpMonthBatchOutput
            {
                Id = u.Id,
                SiteCode = u.SiteCode,
                SiteName = u.SiteName,
                PayPeriod = u.PayPeriod,
                AttMonth = u.AttMonth,
                AttFrom = u.AttFrom,
                AttTo = u.AttTo,
                Status = u.Status,
                ParamSnapshot = u.ParamSnapshot,
                FieldSchemaSnapshot = u.FieldSchemaSnapshot,
                FreezeVersion = u.FreezeVersion,
                FrozenTime = u.FrozenTime,
                FrozenUserId = u.FrozenUserId,
                UnfreezeReason = u.UnfreezeReason,
                ConsumedTime = u.ConsumedTime,
                Remark = u.Remark,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);

        foreach (var item in page.Items)
            item.StatusText = StatusText(item.Status);
        return page;
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("月报批次详情")]
    public async Task<HrEmpMonthBatchOutput> Get([FromQuery] long id)
    {
        var b = await _batchRep.GetByIdAsync(id) ?? throw Oops.Oh("月报批次不存在");
        var cnt = await _lineRep.CountAsync(u => u.BatchId == id);
        return ToBatchOutput(b, cnt);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Create"), HttpPost]
    [DisplayName("开批")]
    public async Task<long> Create(HrEmpMonthCreateInput input)
    {
        var siteCode = input.SiteCode?.Trim() ?? throw Oops.Oh("厂别不能为空");
        var payPeriod = input.PayPeriod?.Trim() ?? throw Oops.Oh("薪酬月不能为空");
        await EnsureCanOpenSiteAsync(siteCode);

        var param = await _paramRep.GetFirstAsync(u => u.SiteCode == siteCode)
            ?? throw Oops.Oh("公司参数不存在，无法开批");

        var exists = await _batchRep.IsAnyAsync(u => u.SiteCode == siteCode && u.PayPeriod == payPeriod);
        if (exists) throw Oops.Oh("该厂别与薪酬月已存在月报批次");

        var period = HrEmpMonthPeriodResolver.Resolve(payPeriod, param.AttCloseDay, param.PayrollAttMonth);
        var paramSnap = JsonSerializer.Serialize(new
        {
            param.AttCloseDay,
            param.PayrollAttMonth,
            param.HireDayAttMode,
            param.LeaveDayAttMode,
            param.RequiredAttDayMode
        }, JsonOpts);

        var entity = new HrEmpMonthBatch
        {
            Id = YitIdHelper.NextId(),
            SiteCode = siteCode,
            SiteName = param.SiteName,
            PayPeriod = period.PayPeriod,
            AttMonth = period.AttMonth,
            AttFrom = period.AttFrom,
            AttTo = period.AttTo,
            Status = HrEmpMonthConst.StatusDraft,
            ParamSnapshot = paramSnap,
            FreezeVersion = 0,
            Remark = input.Remark?.Trim()
        };
        await _batchRep.InsertAsync(entity);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除草稿批次")]
    public async Task Delete(BaseIdInput input)
    {
        var b = await _batchRep.GetByIdAsync(input.Id) ?? throw Oops.Oh("月报批次不存在");
        if (b.Status != HrEmpMonthConst.StatusDraft)
            throw Oops.Oh("仅草稿批次可删除");
        var hasLine = await _lineRep.IsAnyAsync(u => u.BatchId == b.Id);
        if (hasLine) throw Oops.Oh("批次已有行，请先清空或仅删除空草稿");
        await _batchRep.DeleteAsync(b);
    }

    [ApiDescriptionSettings(Name = "ResolvePeriodPreview"), HttpPost]
    [DisplayName("预览考勤月界")]
    public async Task<HrEmpMonthPeriodPreviewOutput> ResolvePeriodPreview(HrEmpMonthPeriodPreviewInput input)
    {
        var siteCode = input.SiteCode?.Trim() ?? throw Oops.Oh("厂别不能为空");
        var payPeriod = input.PayPeriod?.Trim() ?? throw Oops.Oh("薪酬月不能为空");
        var param = await _paramRep.GetFirstAsync(u => u.SiteCode == siteCode)
            ?? throw Oops.Oh("公司参数不存在");
        var period = HrEmpMonthPeriodResolver.Resolve(payPeriod, param.AttCloseDay, param.PayrollAttMonth);
        return new HrEmpMonthPeriodPreviewOutput
        {
            PayPeriod = period.PayPeriod,
            AttMonth = period.AttMonth,
            AttFrom = period.AttFrom,
            AttTo = period.AttTo,
            AttCloseDay = param.AttCloseDay,
            PayrollAttMonth = param.PayrollAttMonth
        };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Generate"), HttpPost]
    [DisplayName("生成名单")]
    public async Task Generate(HrEmpMonthGenerateInput input)
    {
        await _generate.GenerateAsync(input.BatchId, input.FullReset);
    }

    [ApiDescriptionSettings(Name = "PageLines"), HttpPost]
    [DisplayName("月报行分页")]
    public async Task<SqlSugarPagedList<HrEmpMonthLineOutput>> PageLines(HrEmpMonthLinePageInput input)
    {
        input ??= new HrEmpMonthLinePageInput();
        if (input.BatchId <= 0) throw Oops.Oh("批次Id无效");
        _ = await _batchRep.GetByIdAsync(input.BatchId) ?? throw Oops.Oh("月报批次不存在");

        var keyword = input.Keyword?.Trim();
        var query = _lineRep.AsQueryable()
            .Where(u => u.BatchId == input.BatchId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CcCode), u => u.CcCode == input.CcCode)
            .WhereIF(!string.IsNullOrWhiteSpace(input.IncludeFlag), u => u.IncludeFlag == input.IncludeFlag)
            .WhereIF(input.HasWarn == true, u => u.WarnCodes != null && u.WarnCodes != "")
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.EmpId.Contains(keyword!) || (u.EmpName != null && u.EmpName.Contains(keyword!)));

        // 行管辖：按行部门过滤（非全量）
        if (!_scope.IsAllScope())
        {
            var scopedEmp = await _scope.ApplyEmpScopeAsync(_empRep.AsQueryable());
            var scopedIds = await scopedEmp.Select(u => u.EmpId).ToListAsync();
            query = query.Where(u => scopedIds.Contains(u.EmpId));
        }

        query = HrEmpMonthFieldRegistry.ApplyLine(query, input.Filter);

        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "empId";
            input.Order = "asc";
        }

        var page = await query
            .OrderBuilder(input, defaultSortField: "EmpId", descSort: false)
            .Select(u => new HrEmpMonthLineOutput
            {
                Id = u.Id,
                BatchId = u.BatchId,
                EmpId = u.EmpId,
                IEmpId = u.IEmpId,
                EmpName = u.EmpName,
                IncludeFlag = u.IncludeFlag,
                ManualLock = u.ManualLock,
                EmpState = u.EmpState,
                EmpSal = u.EmpSal,
                CcCode = u.CcCode,
                CcName = u.CcName,
                PostId = u.PostId,
                DutyId = u.DutyId,
                EmpInDate = u.EmpInDate,
                EmpLeaDate = u.EmpLeaDate,
                EmpKqIndate = u.EmpKqIndate,
                EmpKqLeaDate = u.EmpKqLeaDate,
                NeedAttendance = u.NeedAttendance,
                IsNewHire = u.IsNewHire,
                IsLeaver = u.IsLeaver,
                IsSuspend = u.IsSuspend,
                HasChgInPeriod = u.HasChgInPeriod,
                LeaveSettleCode = u.LeaveSettleCode,
                LeavePayCmdStatus = u.LeavePayCmdStatus,
                PayCmdId = u.PayCmdId,
                CcCodeAtStart = u.CcCodeAtStart,
                CcCodeAtEnd = u.CcCodeAtEnd,
                ChgSummary = u.ChgSummary,
                ChgFieldSnapshot = u.ChgFieldSnapshot,
                CostCcCode = u.CostCcCode,
                ExcludeReason = u.ExcludeReason,
                IncludeReason = u.IncludeReason,
                Remark = u.Remark,
                EmpFieldSnapshot = u.EmpFieldSnapshot,
                WarnCodes = u.WarnCodes,
                ErrorCodes = u.ErrorCodes
            })
            .ToPagedListAsync(input.Page, input.PageSize);

        foreach (var row in page.Items)
            row.EmpSnapMap = BuildEmpSnapMap(row.EmpFieldSnapshot);
        return page;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "AdjustLine"), HttpPost]
    [DisplayName("调整月报行")]
    public async Task AdjustLine(HrEmpMonthAdjustLineInput input)
    {
        var batch = await _batchRep.GetByIdAsync(input.BatchId) ?? throw Oops.Oh("月报批次不存在");
        if (batch.Status is not (HrEmpMonthConst.StatusGenerated or HrEmpMonthConst.StatusReopened))
            throw Oops.Oh("当前状态不可调整行（请先生成）");

        var flag = input.IncludeFlag?.Trim();
        if (flag is not (HrEmpMonthConst.IncludeInclude or HrEmpMonthConst.IncludeExclude))
            throw Oops.Oh("纳入标志无效");

        if (input.LineId is > 0)
        {
            var line = await _lineRep.GetFirstAsync(u => u.Id == input.LineId && u.BatchId == input.BatchId)
                ?? throw Oops.Oh("月报行不存在");
            await EnsureEmpInScopeAsync(line.EmpId);
            if (flag == HrEmpMonthConst.IncludeExclude)
            {
                if (string.IsNullOrWhiteSpace(input.ExcludeReason))
                    throw Oops.Oh("排除须填写原因");
                line.IncludeFlag = HrEmpMonthConst.IncludeExclude;
                line.ExcludeReason = input.ExcludeReason.Trim();
                line.IncludeReason = null;
            }
            else
            {
                line.IncludeFlag = HrEmpMonthConst.IncludeInclude;
                line.ExcludeReason = null;
                if (!string.IsNullOrWhiteSpace(input.IncludeReason))
                    line.IncludeReason = input.IncludeReason.Trim();
            }
            line.ManualLock = true;
            if (input.Remark != null) line.Remark = input.Remark.Trim();
            line.NeedAttendance = flag == HrEmpMonthConst.IncludeInclude && line.NeedAttendance;
            if (flag == HrEmpMonthConst.IncludeExclude) line.NeedAttendance = false;
            await _lineRep.UpdateAsync(line);
            return;
        }

        // 手工纳入新人
        if (flag != HrEmpMonthConst.IncludeInclude)
            throw Oops.Oh("新建行仅支持纳入");
        if (string.IsNullOrWhiteSpace(input.IncludeReason))
            throw Oops.Oh("手工纳入须填写原因");
        var empId = input.EmpId?.Trim() ?? throw Oops.Oh("工号不能为空");
        var emp = await EnsureEmpInScopeAsync(empId);
        if (await _lineRep.IsAnyAsync(u => u.BatchId == input.BatchId && u.EmpId == empId))
            throw Oops.Oh("该工号已在本批名单中");

        var ccName = emp.CcCode == null ? null
            : (await _ccRep.GetFirstAsync(u => u.CcCode == emp.CcCode))?.CcDesc;

        await _lineRep.InsertAsync(new HrEmpMonthLine
        {
            Id = YitIdHelper.NextId(),
            BatchId = input.BatchId,
            EmpId = emp.EmpId,
            IEmpId = emp.IEmpId,
            EmpName = emp.EmpName,
            IncludeFlag = HrEmpMonthConst.IncludeInclude,
            ManualLock = true,
            EmpState = emp.EmpState,
            EmpSal = emp.EmpSal,
            CcCode = emp.CcCode,
            CcName = ccName,
            PostId = emp.PostId,
            DutyId = emp.DutyId,
            EmpInDate = emp.EmpInDate?.Date,
            EmpLeaDate = emp.EmpLeaDate?.Date,
            EmpKqIndate = emp.EmpKqIndate?.Date,
            EmpKqLeaDate = emp.EmpKqLeaDate?.Date,
            NeedAttendance = true,
            CcCodeAtEnd = emp.CcCode,
            CostCcCode = emp.CcCode,
            IncludeReason = input.IncludeReason.Trim(),
            Remark = input.Remark?.Trim(),
            EmpFieldSnapshot = JsonSerializer.Serialize(new EmpFieldSnapshotDto(), JsonOpts),
            ChgFieldSnapshot = JsonSerializer.Serialize(new ChgFieldSnapshotDto(), JsonOpts)
        });
    }

    [ApiDescriptionSettings(Name = "Validate"), HttpPost]
    [DisplayName("校验月报批次")]
    public async Task<HrEmpMonthValidateOutput> Validate(BaseIdInput input)
    {
        var batch = await _batchRep.GetByIdAsync(input.Id) ?? throw Oops.Oh("月报批次不存在");
        var lines = await _lineRep.GetListAsync(u => u.BatchId == batch.Id);
        var errors = new List<string>();
        var warns = new List<string>();

        var dup = lines.GroupBy(u => u.EmpId, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        foreach (var d in dup)
            errors.Add($"重复工号:{d}");
        foreach (var l in lines.Where(u => string.IsNullOrWhiteSpace(u.EmpId)))
            errors.Add($"空工号行 Id={l.Id}");

        foreach (var l in lines.Where(u => !string.IsNullOrWhiteSpace(u.WarnCodes)))
            warns.Add($"{l.EmpId}:{l.WarnCodes}");

        if (lines.Count == 0)
            errors.Add("名单为空，不可冻结");

        return new HrEmpMonthValidateOutput
        {
            ErrorCount = errors.Count,
            WarnCount = warns.Count,
            Errors = errors.Take(50).ToList(),
            Warns = warns.Take(50).ToList(),
            CanFreeze = errors.Count == 0
        };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Freeze"), HttpPost]
    [DisplayName("冻结月报")]
    public async Task Freeze(HrEmpMonthFreezeInput input)
    {
        var batch = await _batchRep.GetByIdAsync(input.BatchId) ?? throw Oops.Oh("月报批次不存在");
        if (batch.Status is not (HrEmpMonthConst.StatusGenerated or HrEmpMonthConst.StatusReopened))
            throw Oops.Oh("仅已生成/已解冻可冻结");

        var v = await Validate(new BaseIdInput { Id = batch.Id });
        if (!v.CanFreeze) throw Oops.Oh("存在阻断错误，无法冻结：" + string.Join("；", v.Errors.Take(5)));
        if (v.WarnCount > 0 && !input.ConfirmWarn)
            throw Oops.Oh("存在警告，请确认后重试冻结");

        batch.Status = HrEmpMonthConst.StatusFrozen;
        batch.FreezeVersion += 1;
        batch.FrozenTime = DateTime.Now;
        batch.FrozenUserId = _userManager.UserId;
        await _batchRep.UpdateAsync(batch);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Unfreeze"), HttpPost]
    [DisplayName("解冻月报")]
    public async Task Unfreeze(HrEmpMonthUnfreezeInput input)
    {
        var batch = await _batchRep.GetByIdAsync(input.BatchId) ?? throw Oops.Oh("月报批次不存在");
        if (batch.Status == HrEmpMonthConst.StatusConsumed)
            throw Oops.Oh("已消费批次禁止解冻，请先回滚薪酬消费标记");
        if (batch.Status != HrEmpMonthConst.StatusFrozen)
            throw Oops.Oh("仅已冻结可解冻");
        if (string.IsNullOrWhiteSpace(input.Reason))
            throw Oops.Oh("解冻须填写原因");

        batch.Status = HrEmpMonthConst.StatusReopened;
        batch.UnfreezeReason = input.Reason.Trim();
        await _batchRep.UpdateAsync(batch);
    }

    [ApiDescriptionSettings(Name = "Export"), HttpGet]
    [DisplayName("导出月报")]
    public async Task<IActionResult> Export([FromQuery] long batchId)
    {
        var batch = await _batchRep.GetByIdAsync(batchId) ?? throw Oops.Oh("月报批次不存在");
        var lines = await _lineRep.AsQueryable()
            .Where(u => u.BatchId == batchId)
            .OrderBy(u => u.EmpId)
            .ToListAsync();

        var rows = lines.Select(u => new Dictionary<string, object?>
        {
            ["工号"] = u.EmpId,
            ["姓名"] = u.EmpName,
            ["纳入标志"] = u.IncludeFlag,
            ["部门"] = u.CcCode,
            ["部门名称"] = u.CcName,
            ["岗位"] = u.PostId,
            ["职务"] = u.DutyId,
            ["薪酬类别"] = u.EmpSal,
            ["人员状态"] = u.EmpState,
            ["入职日"] = u.EmpInDate,
            ["离职日"] = u.EmpLeaDate,
            ["考勤开始日"] = u.EmpKqIndate,
            ["考勤结束日"] = u.EmpKqLeaDate,
            ["是否需考勤"] = u.NeedAttendance,
            ["新进"] = u.IsNewHire,
            ["离职"] = u.IsLeaver,
            ["停职"] = u.IsSuspend,
            ["月内异动"] = u.HasChgInPeriod,
            ["异动摘要"] = u.ChgSummary,
            ["警告"] = u.WarnCodes
        }).ToList();

        var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, rows);
        ms.Position = 0;
        var name = $"empMonth_{batch.SiteCode}_{batch.PayPeriod}.xlsx";
        return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = name
        };
    }

    [ApiDescriptionSettings(Name = "GetFrozenBatch"), HttpPost]
    [DisplayName("获取冻结批次")]
    public async Task<HrEmpMonthBatchOutput> GetFrozenBatch(HrEmpMonthFrozenQueryInput input)
    {
        var batch = await LoadFrozenAsync(input.SiteCode, input.PayPeriod);
        var cnt = await _lineRep.CountAsync(u => u.BatchId == batch.Id);
        return ToBatchOutput(batch, cnt);
    }

    [ApiDescriptionSettings(Name = "PageFrozenLines"), HttpPost]
    [DisplayName("冻结行分页")]
    public async Task<SqlSugarPagedList<HrEmpMonthLineOutput>> PageFrozenLines(HrEmpMonthFrozenQueryInput input)
    {
        var batch = await LoadFrozenAsync(input.SiteCode, input.PayPeriod);
        input.Page = input.Page <= 0 ? 1 : input.Page;
        input.PageSize = input.PageSize <= 0 ? 50 : input.PageSize;
        return await PageLines(new HrEmpMonthLinePageInput
        {
            BatchId = batch.Id,
            Page = input.Page,
            PageSize = input.PageSize,
            Field = input.Field,
            Order = input.Order,
            DescStr = input.DescStr,
            IncludeFlag = HrEmpMonthConst.IncludeInclude,
            Filter = input.Filter,
            Keyword = input.Keyword
        });
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "MarkConsumed"), HttpPost]
    [DisplayName("标记已消费")]
    public async Task MarkConsumed(HrEmpMonthMarkConsumedInput input)
    {
        var site = input.SiteCode?.Trim() ?? throw Oops.Oh("厂别不能为空");
        var period = input.PayPeriod?.Trim() ?? throw Oops.Oh("薪酬月不能为空");
        var batch = await _batchRep.GetFirstAsync(u => u.SiteCode == site && u.PayPeriod == period)
            ?? throw Oops.Oh("月报批次不存在");
        if (batch.Status == HrEmpMonthConst.StatusConsumed)
            return; // 幂等
        if (batch.Status != HrEmpMonthConst.StatusFrozen)
            throw Oops.Oh("仅已冻结批次可标记消费");
        batch.Status = HrEmpMonthConst.StatusConsumed;
        batch.ConsumedTime = DateTime.Now;
        await _batchRep.UpdateAsync(batch);
    }

    [ApiDescriptionSettings(Name = "ListMonthSnapFields"), HttpGet]
    [DisplayName("月报快照字段")]
    public async Task<List<MonthSnapFieldOutput>> ListMonthSnapFields()
    {
        var fields = await _snap.LoadMonthSnapFieldsAsync();
        return fields.Select(f => new MonthSnapFieldOutput
        {
            FieldKey = f.FieldKey,
            DbColumn = f.DbColumn,
            Label = f.Label,
            DataType = f.DataType,
            InCoreColumns = f.InCoreColumns
        }).ToList();
    }

    [ApiDescriptionSettings(Name = "GetChgEnabledFields"), HttpGet]
    [DisplayName("启用异动可改字段")]
    public async Task<List<string>> GetChgEnabledFields()
        => (await _snap.LoadChgEnabledFieldKeysAsync()).ToList();

    private async Task<HrEmpMonthBatch> LoadFrozenAsync(string? siteCode, string? payPeriod)
    {
        var site = siteCode?.Trim() ?? throw Oops.Oh("厂别不能为空");
        var period = payPeriod?.Trim() ?? throw Oops.Oh("薪酬月不能为空");
        var batch = await _batchRep.GetFirstAsync(u => u.SiteCode == site && u.PayPeriod == period)
            ?? throw Oops.Oh("月报批次不存在");
        if (batch.Status is not (HrEmpMonthConst.StatusFrozen or HrEmpMonthConst.StatusConsumed))
            throw Oops.Oh("批次未冻结，不可按冻结契约读取");
        return batch;
    }

    private async Task EnsureCanOpenSiteAsync(string siteCode)
    {
        if (_scope.IsAllScope()) return;
        var roots = await _scope.GetCurrentEffectiveRootsAsync();
        if (roots.Count == 0) throw Oops.Oh("未配置人事管辖，无法开批");
        var sites = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in roots)
        {
            var s = await _siteResolve.TryResolveSiteCodeByDeptAsync(r.CcCode);
            if (!string.IsNullOrWhiteSpace(s)) sites.Add(s);
        }
        if (!sites.Contains(siteCode))
            throw Oops.Oh("无权对该厂别开立人事月报");
    }

    private async Task<HrEmployee> EnsureEmpInScopeAsync(string empId)
    {
        var q = await _scope.ApplyEmpScopeAsync(_empRep.AsQueryable().Where(u => u.EmpId == empId));
        var emp = await q.FirstAsync();
        return emp ?? throw Oops.Oh("人员不存在或不在管辖范围内");
    }

    private static HrEmpMonthBatchOutput ToBatchOutput(HrEmpMonthBatch b, int? lineCount = null) => new()
    {
        Id = b.Id,
        SiteCode = b.SiteCode,
        SiteName = b.SiteName,
        PayPeriod = b.PayPeriod,
        AttMonth = b.AttMonth,
        AttFrom = b.AttFrom,
        AttTo = b.AttTo,
        Status = b.Status,
        StatusText = StatusText(b.Status),
        ParamSnapshot = b.ParamSnapshot,
        FieldSchemaSnapshot = b.FieldSchemaSnapshot,
        FreezeVersion = b.FreezeVersion,
        FrozenTime = b.FrozenTime,
        FrozenUserId = b.FrozenUserId,
        UnfreezeReason = b.UnfreezeReason,
        ConsumedTime = b.ConsumedTime,
        Remark = b.Remark,
        CreateTime = b.CreateTime,
        LineCount = lineCount
    };

    private static string StatusText(string? s) => s switch
    {
        HrEmpMonthConst.StatusDraft => "草稿",
        HrEmpMonthConst.StatusGenerated => "已生成",
        HrEmpMonthConst.StatusFrozen => "已冻结",
        HrEmpMonthConst.StatusReopened => "已解冻",
        HrEmpMonthConst.StatusConsumed => "已消费",
        _ => s ?? ""
    };

    private static Dictionary<string, string?>? BuildEmpSnapMap(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            var dto = JsonSerializer.Deserialize<EmpFieldSnapshotDto>(json, JsonOpts);
            return dto?.Fields?.ToDictionary(f => f.FieldKey, f => f.Value, StringComparer.OrdinalIgnoreCase);
        }
        catch { return null; }
    }
}
