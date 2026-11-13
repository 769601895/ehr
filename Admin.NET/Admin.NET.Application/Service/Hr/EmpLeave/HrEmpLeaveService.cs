using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>员工离职办理（独立模块，不接审批流程正式入口）</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpLeave", Order = 472, Description = "员工离职办理")]
public class HrEmpLeaveService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmpLeave> _leaveRep;
    private readonly SqlSugarRepository<HrEmpLeaveRestore> _restoreRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<HrWfDef> _wfDefRep;
    private readonly HrEmpLeaveApplyService _apply;
    private readonly HrCodeMstrService _codeSvc;
    private readonly HrUserCcScopeService _scope;
    private readonly UserManager _userManager;

    public HrEmpLeaveService(
        SqlSugarRepository<HrEmpLeave> leaveRep,
        SqlSugarRepository<HrEmpLeaveRestore> restoreRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<HrWfDef> wfDefRep,
        HrEmpLeaveApplyService apply,
        HrCodeMstrService codeSvc,
        HrUserCcScopeService scope,
        UserManager userManager)
    {
        _leaveRep = leaveRep;
        _restoreRep = restoreRep;
        _empRep = empRep;
        _ccRep = ccRep;
        _wfDefRep = wfDefRep;
        _apply = apply;
        _codeSvc = codeSvc;
        _scope = scope;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("离职分页")]
    public async Task<SqlSugarPagedList<HrEmpLeaveOutput>> Page(HrEmpLeavePageInput input)
    {
        input ??= new HrEmpLeavePageInput();
        var keyword = input.Keyword?.Trim();
        var statusList = input.StatusList?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList();
        var query = _leaveRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.EmpId.Contains(keyword!) || (u.EmpName != null && u.EmpName.Contains(keyword!)))
            .WhereIF(statusList is { Count: > 0 }, u => statusList!.Contains(u.Status))
            .WhereIF(statusList is not { Count: > 0 } && !string.IsNullOrWhiteSpace(input.Status), u => u.Status == input.Status)
            .WhereIF(!string.IsNullOrWhiteSpace(input.LeaveType), u => u.LeaveType == input.LeaveType)
            .WhereIF(input.LeaveDateBegin.HasValue, u => u.LeaveDate >= input.LeaveDateBegin!.Value.Date)
            .WhereIF(input.LeaveDateEnd.HasValue, u => u.LeaveDate <= input.LeaveDateEnd!.Value.Date);

        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "leaveDate";
            input.Order = input.DescStr ?? "desc";
        }

        var page = await query
            .OrderBuilder(input, defaultSortField: "LeaveDate", descSort: true)
            .Select(u => new HrEmpLeaveOutput
            {
                Id = u.Id,
                LeaveNo = u.LeaveNo,
                EmpId = u.EmpId,
                EmpName = u.EmpName,
                LeaveDate = u.LeaveDate,
                PlanEffectDate = u.PlanEffectDate,
                LeaveType = u.LeaveType,
                LeaveReason = u.LeaveReason,
                SalarySettle = u.SalarySettle,
                IsBlacklist = u.IsBlacklist,
                CcCodeSnap = u.CcCodeSnap,
                Remark = u.Remark,
                Status = u.Status,
                SubmitTime = u.SubmitTime,
                ApproveTime = u.ApproveTime,
                RejectReason = u.RejectReason,
                EffectiveTime = u.EffectiveTime,
                VoidTime = u.VoidTime,
                VoidReason = u.VoidReason,
                CreateTime = u.CreateTime,
                CreateUserName = u.CreateUserName
            })
            .ToPagedListAsync(input.Page, input.PageSize);

        await FillTexts(page.Items);
        return page;
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("离职详情")]
    public async Task<HrEmpLeaveOutput> Detail([FromQuery] long id)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("离职单不存在");
        var o = ToOutput(bill);
        await FillTexts([o]);
        return o;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDraft"), HttpPost]
    [DisplayName("保存离职草稿")]
    public async Task<long> SaveDraft(HrEmpLeaveSaveDraftInput input)
    {
        ValidateDraft(input);
        var empId = input.EmpId.Trim();
        var emp = await LoadEmpInScopeAsync(empId);
        if (emp.EmpState is not (HrEmpLeaveConst.EmpStateActive or HrEmpLeaveConst.EmpStateSuspend))
            throw Oops.Oh("仅在职/停职人员可办理离职");
        await AssertCanOpenLeave(empId, input.Id);

        var leaveDate = input.LeaveDate.Date;
        var plan = leaveDate.AddDays(1);
        var settle = string.IsNullOrWhiteSpace(input.SalarySettle)
            ? HrEmpLeaveConst.DefaultSalarySettle
            : input.SalarySettle.Trim();

        if (input.Id is > 0)
        {
            var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
            if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可修改");
            bill.EmpId = empId;
            bill.EmpName = emp.EmpName;
            bill.LeaveDate = leaveDate;
            bill.PlanEffectDate = plan;
            bill.LeaveType = input.LeaveType.Trim();
            bill.LeaveReason = input.LeaveReason.Trim();
            bill.SalarySettle = settle;
            bill.IsBlacklist = input.IsBlacklist;
            bill.CcCodeSnap = emp.CcCode;
            bill.Remark = input.Remark?.Trim();
            await _leaveRep.UpdateAsync(bill);
            return bill.Id;
        }

        var entity = new HrEmpLeave
        {
            Id = YitIdHelper.NextId(),
            LeaveNo = await NextNoAsync(HrEmpLeaveConst.LeaveNoPrefix),
            EmpId = empId,
            EmpName = emp.EmpName,
            LeaveDate = leaveDate,
            PlanEffectDate = plan,
            LeaveType = input.LeaveType.Trim(),
            LeaveReason = input.LeaveReason.Trim(),
            SalarySettle = settle,
            IsBlacklist = input.IsBlacklist,
            CcCodeSnap = emp.CcCode,
            Remark = input.Remark?.Trim(),
            Status = HrEmpLeaveConst.StatusDraft
        };
        await _leaveRep.InsertAsync(entity);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteDraft"), HttpPost]
    [DisplayName("删除离职草稿")]
    public async Task DeleteDraft(HrEmpLeaveIdInput input)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可删除");
        await _leaveRep.FakeDeleteAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Submit"), HttpPost]
    [DisplayName("提交离职")]
    public async Task<HrEmpLeaveSubmitOutput> Submit(HrEmpLeaveIdInput input)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可提交");
        ValidateBill(bill);
        await AssertCanOpenLeave(bill.EmpId, bill.Id);
        await LoadEmpInScopeAsync(bill.EmpId);

        var now = DateTime.Now;
        bill.SubmitTime = now;
        bill.SubmitUserId = _userManager.UserId;

        var hasWf = await _wfDefRep.IsAnyAsync(u => u.BizType == HrEmpLeaveConst.WfBizType && u.Enabled);
        if (hasWf)
        {
            bill.Status = HrEmpLeaveConst.StatusInReview;
            await _leaveRep.UpdateAsync(bill);
            return new HrEmpLeaveSubmitOutput { AutoApproved = false, Status = bill.Status };
        }

        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _leaveRep.UpdateAsync(bill);

        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await _apply.TryApplyLeaveAsync(bill.Id);

        var latest = await _leaveRep.GetFirstAsync(u => u.Id == bill.Id);
        return new HrEmpLeaveSubmitOutput { AutoApproved = true, Status = latest?.Status ?? bill.Status };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Approve"), HttpPost]
    [DisplayName("离职审核通过")]
    public async Task Approve(HrEmpLeaveIdInput input)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可通过");
        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _leaveRep.UpdateAsync(bill);
        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await _apply.TryApplyLeaveAsync(bill.Id);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Reject"), HttpPost]
    [DisplayName("离职驳回")]
    public async Task Reject(HrEmpLeaveRejectInput input)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可驳回");
        bill.Status = HrEmpLeaveConst.StatusRejected;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = input.RejectReason?.Trim();
        await _leaveRep.UpdateAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Void"), HttpPost]
    [DisplayName("撤销离职")]
    public async Task Void(HrEmpLeaveVoidInput input)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("离职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusEffective) throw Oops.Oh("仅已生效可撤销");
        await _apply.VoidLeaveAsync(bill, input.VoidReason?.Trim());
    }

    private async Task AssertCanOpenLeave(string empId, long? excludeId)
    {
        var lq = _leaveRep.AsQueryable()
            .Where(u => u.EmpId == empId && HrEmpLeaveConst.OpenStatuses.Contains(u.Status));
        if (excludeId is > 0) lq = lq.Where(u => u.Id != excludeId);
        if (await lq.AnyAsync())
            throw Oops.Oh("该员工已有在途离职单，不能再发起");

        var rq = _restoreRep.AsQueryable()
            .Where(u => u.EmpId == empId
                && u.Kind == HrEmpLeaveConst.KindReinstate
                && HrEmpLeaveConst.OpenStatuses.Contains(u.Status));
        if (await rq.AnyAsync())
            throw Oops.Oh("该员工已有在途复职单，不能办理离职");
    }

    // 停职在途拦截由停职服务负责；离职允许对停职人员开单（R5）

    private async Task<HrEmployee> LoadEmpInScopeAsync(string empId)
    {
        var q = await _scope.ApplyEmpScopeAsync(_empRep.AsQueryable().Where(u => u.EmpId == empId));
        var rows = await q.Take(1).ToListAsync();
        var emp = rows.FirstOrDefault() ?? throw Oops.Oh("人员不存在或不在管辖范围内");
        await _scope.EnsureCcInScopeAsync(emp.CcCode);
        return emp;
    }

    private static void ValidateDraft(HrEmpLeaveSaveDraftInput input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.EmpId)) throw Oops.Oh("请选择员工");
        if (input.LeaveDate == default) throw Oops.Oh("请填写离职日");
        if (string.IsNullOrWhiteSpace(input.LeaveType)) throw Oops.Oh("请选择离职类型");
        if (string.IsNullOrWhiteSpace(input.LeaveReason)) throw Oops.Oh("请选择离职原因");
    }

    private static void ValidateBill(HrEmpLeave bill)
    {
        if (string.IsNullOrWhiteSpace(bill.EmpId)) throw Oops.Oh("请选择员工");
        if (bill.LeaveDate == default) throw Oops.Oh("请填写离职日");
        if (string.IsNullOrWhiteSpace(bill.LeaveType)) throw Oops.Oh("请选择离职类型");
        if (string.IsNullOrWhiteSpace(bill.LeaveReason)) throw Oops.Oh("请选择离职原因");
        if (string.IsNullOrWhiteSpace(bill.SalarySettle)) throw Oops.Oh("请选择薪资结算");
    }

    private async Task<string> NextNoAsync(string prefixHead)
    {
        var day = DateTime.Today.ToString("yyyyMMdd");
        var prefix = prefixHead + day;
        var lasts = await _leaveRep.AsQueryable()
            .Where(u => u.LeaveNo.StartsWith(prefix))
            .OrderByDescending(u => u.LeaveNo)
            .Select(u => u.LeaveNo).Take(1).ToListAsync();
        var last = lasts.FirstOrDefault();
        var seq = 1;
        if (!string.IsNullOrEmpty(last) && last.Length >= prefix.Length + 4
            && int.TryParse(last.AsSpan(prefix.Length), out var n))
            seq = n + 1;
        return $"{prefix}{seq:D4}";
    }

    private static HrEmpLeaveOutput ToOutput(HrEmpLeave u) => new()
    {
        Id = u.Id,
        LeaveNo = u.LeaveNo,
        EmpId = u.EmpId,
        EmpName = u.EmpName,
        LeaveDate = u.LeaveDate,
        PlanEffectDate = u.PlanEffectDate,
        LeaveType = u.LeaveType,
        LeaveReason = u.LeaveReason,
        SalarySettle = u.SalarySettle,
        IsBlacklist = u.IsBlacklist,
        CcCodeSnap = u.CcCodeSnap,
        Remark = u.Remark,
        Status = u.Status,
        SubmitTime = u.SubmitTime,
        ApproveTime = u.ApproveTime,
        RejectReason = u.RejectReason,
        EffectiveTime = u.EffectiveTime,
        VoidTime = u.VoidTime,
        VoidReason = u.VoidReason,
        CreateTime = u.CreateTime,
        CreateUserName = u.CreateUserName
    };

    private async Task FillTexts(IEnumerable<HrEmpLeaveOutput> items)
    {
        var list = items.ToList();
        if (list.Count == 0) return;
        var typeMap = (await _codeSvc.GetCodeList(HrEmpLeaveConst.CodeLeaveType, includeDeleted: true)).ToDictionary(u => u.Value, u => u.Label, StringComparer.OrdinalIgnoreCase);
        var reasonMap = (await _codeSvc.GetCodeList(HrEmpLeaveConst.CodeLeaveReason, includeDeleted: true)).ToDictionary(u => u.Value, u => u.Label, StringComparer.OrdinalIgnoreCase);
        var settleMap = (await _codeSvc.GetCodeList(HrEmpLeaveConst.CodeSalarySettle, includeDeleted: true)).ToDictionary(u => u.Value, u => u.Label, StringComparer.OrdinalIgnoreCase);
        var codes = list.Select(u => u.CcCodeSnap).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var deptMap = codes.Count == 0
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : (await _ccRep.AsQueryable().Where(u => codes.Contains(u.CcCode)).Select(u => new { u.CcCode, u.CcDesc }).ToListAsync())
                .ToDictionary(u => u.CcCode, u => u.CcDesc ?? u.CcCode, StringComparer.OrdinalIgnoreCase);

        foreach (var o in list)
        {
            o.StatusText = StatusText(o.Status);
            if (typeMap.TryGetValue(o.LeaveType, out var tt)) o.LeaveTypeText = tt;
            if (reasonMap.TryGetValue(o.LeaveReason, out var rt)) o.LeaveReasonText = rt;
            if (settleMap.TryGetValue(o.SalarySettle, out var st)) o.SalarySettleText = st;
            if (!string.IsNullOrWhiteSpace(o.CcCodeSnap) && deptMap.TryGetValue(o.CcCodeSnap, out var dn))
                o.CcDescSnap = dn;
        }
    }

    internal static string StatusText(string? s) => s switch
    {
        HrEmpLeaveConst.StatusDraft => "草稿",
        HrEmpLeaveConst.StatusInReview => "审核中",
        HrEmpLeaveConst.StatusApproved => "待生效",
        HrEmpLeaveConst.StatusEffective => "已生效",
        HrEmpLeaveConst.StatusRejected => "已驳回",
        HrEmpLeaveConst.StatusVoided => "已作废",
        _ => s ?? ""
    };
}
