using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>员工停职处理</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpSuspend", Order = 469, Description = "员工停职")]
public class HrEmpSuspendService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmpSuspend> _suspendRep;
    private readonly SqlSugarRepository<HrEmpLeave> _leaveRep;
    private readonly SqlSugarRepository<HrEmpLeaveRestore> _restoreRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrEmhHist> _histRep;
    private readonly SqlSugarRepository<HrWfDef> _wfDefRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly HrUserCcScopeService _scope;
    private readonly UserManager _userManager;

    public HrEmpSuspendService(
        SqlSugarRepository<HrEmpSuspend> suspendRep,
        SqlSugarRepository<HrEmpLeave> leaveRep,
        SqlSugarRepository<HrEmpLeaveRestore> restoreRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrEmhHist> histRep,
        SqlSugarRepository<HrWfDef> wfDefRep,
        SqlSugarRepository<CcMstr> ccRep,
        HrUserCcScopeService scope,
        UserManager userManager)
    {
        _suspendRep = suspendRep;
        _leaveRep = leaveRep;
        _restoreRep = restoreRep;
        _empRep = empRep;
        _histRep = histRep;
        _wfDefRep = wfDefRep;
        _ccRep = ccRep;
        _scope = scope;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("停职分页")]
    public async Task<SqlSugarPagedList<HrEmpSuspendOutput>> Page(HrEmpSuspendPageInput input)
    {
        input ??= new HrEmpSuspendPageInput();
        var keyword = input.Keyword?.Trim();
        var statusList = input.StatusList?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList();
        var query = _suspendRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.EmpId.Contains(keyword!) || (u.EmpName != null && u.EmpName.Contains(keyword!))
                    || u.SuspendNo.Contains(keyword!))
            .WhereIF(statusList is { Count: > 0 }, u => statusList!.Contains(u.Status))
            .WhereIF(statusList is not { Count: > 0 } && !string.IsNullOrWhiteSpace(input.Status), u => u.Status == input.Status)
            .WhereIF(input.SuspendDateBegin.HasValue, u => u.SuspendDate >= input.SuspendDateBegin!.Value.Date)
            .WhereIF(input.SuspendDateEnd.HasValue, u => u.SuspendDate <= input.SuspendDateEnd!.Value.Date);

        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "suspendDate";
            input.Order = input.DescStr ?? "desc";
        }
        if (char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];

        var page = await query
            .OrderBuilder(input, defaultSortField: "SuspendDate", descSort: true)
            .Select(u => new HrEmpSuspendOutput
            {
                Id = u.Id,
                SuspendNo = u.SuspendNo,
                EmpId = u.EmpId,
                EmpName = u.EmpName,
                SuspendDate = u.SuspendDate,
                PlanEffectDate = u.PlanEffectDate,
                SuspendReason = u.SuspendReason,
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

        await FillDept(page.Items);
        foreach (var o in page.Items)
            o.StatusText = HrEmpLeaveService.StatusText(o.Status);
        return page;
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("停职详情")]
    public async Task<HrEmpSuspendOutput> Detail([FromQuery] long id)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("停职单不存在");
        var o = ToOutput(bill);
        await FillDept([o]);
        o.StatusText = HrEmpLeaveService.StatusText(o.Status);
        return o;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDraft"), HttpPost]
    [DisplayName("保存停职草稿")]
    public async Task<long> SaveDraft(HrEmpSuspendSaveDraftInput input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.EmpId)) throw Oops.Oh("请输入工号");
        if (input.SuspendDate == default) throw Oops.Oh("请填写停职日");

        var empId = input.EmpId.Trim();
        var emp = await LoadEmpInScopeAsync(empId);
        if (emp.EmpState != HrEmpLeaveConst.EmpStateActive)
            throw Oops.Oh("仅在职人员可办理停职");
        await AssertCanOpenSuspend(empId, input.Id);

        var d = input.SuspendDate.Date;
        if (input.Id is > 0)
        {
            var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
            if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可修改");
            bill.EmpId = empId;
            bill.EmpName = emp.EmpName;
            bill.SuspendDate = d;
            bill.PlanEffectDate = d;
            bill.SuspendReason = input.SuspendReason?.Trim();
            bill.CcCodeSnap = emp.CcCode;
            bill.Remark = input.Remark?.Trim();
            await _suspendRep.UpdateAsync(bill);
            return bill.Id;
        }

        var entity = new HrEmpSuspend
        {
            Id = YitIdHelper.NextId(),
            SuspendNo = await NextNoAsync(),
            EmpId = empId,
            EmpName = emp.EmpName,
            SuspendDate = d,
            PlanEffectDate = d,
            SuspendReason = input.SuspendReason?.Trim(),
            CcCodeSnap = emp.CcCode,
            Remark = input.Remark?.Trim(),
            Status = HrEmpLeaveConst.StatusDraft
        };
        await _suspendRep.InsertAsync(entity);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteDraft"), HttpPost]
    [DisplayName("删除停职草稿")]
    public async Task DeleteDraft(HrEmpSuspendIdInput input)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可删除");
        await _suspendRep.FakeDeleteAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Submit"), HttpPost]
    [DisplayName("提交停职")]
    public async Task<HrEmpSuspendSubmitOutput> Submit(HrEmpSuspendIdInput input)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可提交");
        await AssertCanOpenSuspend(bill.EmpId, bill.Id);
        await LoadEmpInScopeAsync(bill.EmpId);

        var now = DateTime.Now;
        bill.SubmitTime = now;
        bill.SubmitUserId = _userManager.UserId;

        // 停职不强制走离职 WF；无独立 WF 则自动通过
        var hasWf = await _wfDefRep.IsAnyAsync(u => u.BizType == "hrEmpSuspend" && u.Enabled);
        if (hasWf)
        {
            bill.Status = HrEmpLeaveConst.StatusInReview;
            await _suspendRep.UpdateAsync(bill);
            return new HrEmpSuspendSubmitOutput { AutoApproved = false, Status = bill.Status };
        }

        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _suspendRep.UpdateAsync(bill);
        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await TryApplyAsync(bill.Id);

        var latest = await _suspendRep.GetFirstAsync(u => u.Id == bill.Id);
        return new HrEmpSuspendSubmitOutput { AutoApproved = true, Status = latest?.Status ?? bill.Status };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Approve"), HttpPost]
    [DisplayName("停职审核通过")]
    public async Task Approve(HrEmpSuspendIdInput input)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可通过");
        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _suspendRep.UpdateAsync(bill);
        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await TryApplyAsync(bill.Id);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Reject"), HttpPost]
    [DisplayName("停职驳回")]
    public async Task Reject(HrEmpSuspendRejectInput input)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可驳回");
        bill.Status = HrEmpLeaveConst.StatusRejected;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = input.RejectReason?.Trim();
        await _suspendRep.UpdateAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Void"), HttpPost]
    [DisplayName("撤销停职")]
    public async Task Void(HrEmpSuspendVoidInput input)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("停职单不存在");
        if (bill.Status != HrEmpLeaveConst.StatusEffective) throw Oops.Oh("仅已生效可撤销");
        if (await _restoreRep.IsAnyAsync(u =>
                u.EmpId == bill.EmpId && u.Kind == HrEmpLeaveConst.KindReinstate
                && HrEmpLeaveConst.OpenStatuses.Contains(u.Status)))
            throw Oops.Oh("该员工有在途复职单，不能撤销停职");

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在");
        emp.EmpState = bill.EmpStateBefore is > 0
            ? bill.EmpStateBefore
            : HrEmpLeaveConst.EmpStateActive;
        await _empRep.AsUpdateable(emp).UpdateColumns(u => new { u.EmpState }).ExecuteCommandAsync();

        bill.Status = HrEmpLeaveConst.StatusVoided;
        bill.VoidTime = DateTime.Now;
        bill.VoidUserId = _userManager.UserId;
        bill.VoidReason = input.VoidReason?.Trim();
        await _suspendRep.UpdateAsync(bill);
    }

    /// <summary>Job 调用：到日生效</summary>
    [NonAction]
    public async Task<int> ApplyDueAsync()
    {
        var today = DateTime.Today;
        var ids = await _suspendRep.AsQueryable()
            .Where(u => u.Status == HrEmpLeaveConst.StatusApproved && u.PlanEffectDate <= today)
            .Select(u => u.Id).ToListAsync();
        var ok = 0;
        foreach (var id in ids)
        {
            try
            {
                if (await TryApplyAsync(id)) ok++;
            }
            catch
            {
                // Job 外层记日志
            }
        }
        return ok;
    }

    [NonAction]
    [UnitOfWork]
    public async Task<bool> TryApplyAsync(long id)
    {
        var bill = await _suspendRep.GetFirstAsync(u => u.Id == id);
        if (bill == null) return false;
        if (bill.Status != HrEmpLeaveConst.StatusApproved) return false;
        if (bill.PlanEffectDate.Date > DateTime.Today) return false;

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法停职生效");
        if (emp.EmpState != HrEmpLeaveConst.EmpStateActive)
            throw Oops.Oh("员工非在职状态，无法停职生效");

        bill.EmpStateBefore = emp.EmpState;
        emp.EmpState = HrEmpLeaveConst.EmpStateSuspend;
        await _empRep.AsUpdateable(emp).UpdateColumns(u => new { u.EmpState }).ExecuteCommandAsync();

        bill.Status = HrEmpLeaveConst.StatusEffective;
        bill.EffectiveTime = DateTime.Now;
        await _suspendRep.UpdateAsync(bill);

        await _histRep.InsertAsync(new HrEmhHist
        {
            EmpId = Trunc(bill.EmpId, 50),
            EmhNbr = Trunc(bill.SuspendNo, 50),
            EmhDate = bill.SuspendDate.Date,
            EmhWdate = DateTime.Today,
            EmhType = HrWfConsts.EmhTypeSuspend,
            EmhReason = Trunc(bill.SuspendReason, 80),
            EmhOcc = Trunc(bill.CcCodeSnap, 50),
            EmhNcc = Trunc(bill.CcCodeSnap, 50),
            EmhUser = Trunc(_userManager.RealName ?? _userManager.Account, 50),
            EmhTime = DateTime.Now,
            EmhRemark = Trunc("停职生效", 50)
        });
        return true;
    }

    private async Task AssertCanOpenSuspend(string empId, long? excludeId)
    {
        var sq = _suspendRep.AsQueryable()
            .Where(u => u.EmpId == empId && HrEmpSuspendConst.OpenStatuses.Contains(u.Status));
        if (excludeId is > 0) sq = sq.Where(u => u.Id != excludeId);
        if (await sq.AnyAsync())
            throw Oops.Oh("该员工已有在途停职单，不能再发起");

        if (await _leaveRep.IsAnyAsync(u => u.EmpId == empId && HrEmpLeaveConst.OpenStatuses.Contains(u.Status)))
            throw Oops.Oh("该员工已有在途离职单，不能办理停职");
        if (await _restoreRep.IsAnyAsync(u =>
                u.EmpId == empId && u.Kind == HrEmpLeaveConst.KindReinstate
                && HrEmpLeaveConst.OpenStatuses.Contains(u.Status)))
            throw Oops.Oh("该员工已有在途复职单，不能办理停职");
    }

    private async Task<HrEmployee> LoadEmpInScopeAsync(string empId)
    {
        var q = await _scope.ApplyEmpScopeAsync(_empRep.AsQueryable().Where(u => u.EmpId == empId));
        var rows = await q.Take(1).ToListAsync();
        var emp = rows.FirstOrDefault() ?? throw Oops.Oh("人员不存在或不在管辖范围内");
        await _scope.EnsureCcInScopeAsync(emp.CcCode);
        return emp;
    }

    private async Task<string> NextNoAsync()
    {
        var day = DateTime.Today.ToString("yyyyMMdd");
        var prefix = HrEmpSuspendConst.NoPrefix + day;
        var lasts = await _suspendRep.AsQueryable()
            .Where(u => u.SuspendNo.StartsWith(prefix))
            .OrderByDescending(u => u.SuspendNo)
            .Select(u => u.SuspendNo).Take(1).ToListAsync();
        var last = lasts.FirstOrDefault();
        var seq = 1;
        if (!string.IsNullOrEmpty(last) && last.Length >= prefix.Length + 4
            && int.TryParse(last.AsSpan(prefix.Length), out var n))
            seq = n + 1;
        return $"{prefix}{seq:D4}";
    }

    private async Task FillDept(IEnumerable<HrEmpSuspendOutput> items)
    {
        var list = items as IList<HrEmpSuspendOutput> ?? items.ToList();
        var codes = list.Where(u => !string.IsNullOrWhiteSpace(u.CcCodeSnap)).Select(u => u.CcCodeSnap!).Distinct().ToList();
        if (codes.Count == 0) return;
        var map = await _ccRep.AsQueryable().Where(u => codes.Contains(u.CcCode))
            .Select(u => new { u.CcCode, u.CcDesc }).ToListAsync();
        var dict = map.ToDictionary(u => u.CcCode, u => u.CcDesc, StringComparer.OrdinalIgnoreCase);
        foreach (var o in list)
            if (!string.IsNullOrWhiteSpace(o.CcCodeSnap) && dict.TryGetValue(o.CcCodeSnap, out var d))
                o.CcDescSnap = d;
    }

    private static HrEmpSuspendOutput ToOutput(HrEmpSuspend u) => new()
    {
        Id = u.Id,
        SuspendNo = u.SuspendNo,
        EmpId = u.EmpId,
        EmpName = u.EmpName,
        SuspendDate = u.SuspendDate,
        PlanEffectDate = u.PlanEffectDate,
        SuspendReason = u.SuspendReason,
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

    private static string? Trunc(string? s, int len)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Length <= len ? s : s[..len];
    }
}
