using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>同工号复职管理</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpLeaveRestore", Order = 471, Description = "同工号复职")]
public class HrEmpLeaveRestoreService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmpLeaveRestore> _restoreRep;
    private readonly SqlSugarRepository<HrEmpLeave> _leaveRep;
    private readonly SqlSugarRepository<HrEmpSuspend> _suspendRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrWfDef> _wfDefRep;
    private readonly HrEmpLeaveApplyService _apply;
    private readonly HrUserCcScopeService _scope;
    private readonly UserManager _userManager;

    public HrEmpLeaveRestoreService(
        SqlSugarRepository<HrEmpLeaveRestore> restoreRep,
        SqlSugarRepository<HrEmpLeave> leaveRep,
        SqlSugarRepository<HrEmpSuspend> suspendRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrWfDef> wfDefRep,
        HrEmpLeaveApplyService apply,
        HrUserCcScopeService scope,
        UserManager userManager)
    {
        _restoreRep = restoreRep;
        _leaveRep = leaveRep;
        _suspendRep = suspendRep;
        _empRep = empRep;
        _wfDefRep = wfDefRep;
        _apply = apply;
        _scope = scope;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("复职分页")]
    public async Task<SqlSugarPagedList<HrEmpLeaveRestoreOutput>> Page(HrEmpLeaveRestorePageInput input)
    {
        input ??= new HrEmpLeaveRestorePageInput();
        var keyword = input.Keyword?.Trim();
        var kind = string.IsNullOrWhiteSpace(input.Kind) ? HrEmpLeaveConst.KindReinstate : input.Kind.Trim();
        var statusList = input.StatusList?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList();

        var query = _restoreRep.AsQueryable()
            .Where(u => u.Kind == kind)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.EmpId.Contains(keyword!) || (u.EmpName != null && u.EmpName.Contains(keyword!))
                    || u.RestoreNo.Contains(keyword!))
            .WhereIF(statusList is { Count: > 0 }, u => statusList!.Contains(u.Status))
            .WhereIF(statusList is not { Count: > 0 } && !string.IsNullOrWhiteSpace(input.Status), u => u.Status == input.Status)
            .WhereIF(input.RestoreDateBegin.HasValue, u => u.RestoreDate >= input.RestoreDateBegin!.Value.Date)
            .WhereIF(input.RestoreDateEnd.HasValue, u => u.RestoreDate <= input.RestoreDateEnd!.Value.Date);

        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "restoreDate";
            input.Order = input.DescStr ?? "desc";
        }
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];

        var page = await query
            .OrderBuilder(input, defaultSortField: "RestoreDate", descSort: true)
            .Select(u => new HrEmpLeaveRestoreOutput
            {
                Id = u.Id,
                RestoreNo = u.RestoreNo,
                Kind = u.Kind,
                LeaveId = u.LeaveId,
                LeaveNo = u.LeaveNo,
                EmpId = u.EmpId,
                EmpName = u.EmpName,
                RestoreDate = u.RestoreDate,
                PlanEffectDate = u.PlanEffectDate,
                ClearBlacklist = u.ClearBlacklist,
                Remark = u.Remark,
                Status = u.Status,
                SubmitTime = u.SubmitTime,
                ApproveTime = u.ApproveTime,
                RejectReason = u.RejectReason,
                EffectiveTime = u.EffectiveTime,
                VoidTime = u.VoidTime,
                VoidReason = u.VoidReason,
                OperateUserId = u.OperateUserId,
                OperateUserName = u.OperateUserName,
                OperateTime = u.OperateTime,
                CreateTime = u.CreateTime,
                CreateUserName = u.CreateUserName
            })
            .ToPagedListAsync(input.Page, input.PageSize);

        foreach (var o in page.Items)
            o.StatusText = HrEmpLeaveService.StatusText(o.Status);
        return page;
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("复职详情")]
    public async Task<HrEmpLeaveRestoreOutput> Detail([FromQuery] long id)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("复职单不存在");
        var o = ToOutput(bill);
        o.StatusText = HrEmpLeaveService.StatusText(o.Status);
        return o;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDraft"), HttpPost]
    [DisplayName("保存复职草稿")]
    public async Task<long> SaveDraft(HrEmpLeaveRestoreSaveDraftInput input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.EmpId)) throw Oops.Oh("请输入工号");
        if (input.RestoreDate == default) throw Oops.Oh("请填写复职日");

        var empId = input.EmpId.Trim();
        var emp = await LoadEmpInScopeAsync(empId);
        if (emp.EmpState != HrEmpLeaveConst.EmpStateSuspend)
            throw Oops.Oh("仅停职人员可办理复职（离职后再聘请走再入职另案）");
        await AssertCanOpenRestore(empId, input.Id);

        var restoreDate = input.RestoreDate.Date;
        if (input.Id is > 0)
        {
            var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
            if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可修改");
            if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可编辑");
            bill.EmpId = empId;
            bill.EmpName = emp.EmpName;
            bill.RestoreDate = restoreDate;
            bill.PlanEffectDate = restoreDate; // 复职日当天生效
            bill.ClearBlacklist = input.ClearBlacklist;
            bill.Remark = input.Remark?.Trim();
            await _restoreRep.UpdateAsync(bill);
            return bill.Id;
        }

        var entity = new HrEmpLeaveRestore
        {
            Id = YitIdHelper.NextId(),
            RestoreNo = await NextNoAsync(),
            Kind = HrEmpLeaveConst.KindReinstate,
            EmpId = empId,
            EmpName = emp.EmpName,
            RestoreDate = restoreDate,
            PlanEffectDate = restoreDate,
            ClearBlacklist = input.ClearBlacklist,
            Remark = input.Remark?.Trim(),
            Status = HrEmpLeaveConst.StatusDraft
        };
        await _restoreRep.InsertAsync(entity);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteDraft"), HttpPost]
    [DisplayName("删除复职草稿")]
    public async Task DeleteDraft(HrEmpLeaveIdInput input)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可删除");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可删除");
        await _restoreRep.FakeDeleteAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Submit"), HttpPost]
    [DisplayName("提交复职")]
    public async Task<HrEmpLeaveSubmitOutput> Submit(HrEmpLeaveIdInput input)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可提交");
        if (bill.Status != HrEmpLeaveConst.StatusDraft) throw Oops.Oh("仅草稿可提交");
        await AssertCanOpenRestore(bill.EmpId, bill.Id);
        await LoadEmpInScopeAsync(bill.EmpId);

        var now = DateTime.Now;
        bill.SubmitTime = now;
        bill.SubmitUserId = _userManager.UserId;

        var hasWf = await _wfDefRep.IsAnyAsync(u => u.BizType == HrEmpLeaveConst.WfBizType && u.Enabled);
        if (hasWf)
        {
            bill.Status = HrEmpLeaveConst.StatusInReview;
            await _restoreRep.UpdateAsync(bill);
            return new HrEmpLeaveSubmitOutput { AutoApproved = false, Status = bill.Status };
        }

        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _restoreRep.UpdateAsync(bill);

        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await _apply.TryApplyRestoreAsync(bill.Id);

        var latest = await _restoreRep.GetFirstAsync(u => u.Id == bill.Id);
        return new HrEmpLeaveSubmitOutput { AutoApproved = true, Status = latest?.Status ?? bill.Status };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Approve"), HttpPost]
    [DisplayName("复职审核通过")]
    public async Task Approve(HrEmpLeaveIdInput input)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可审核");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可通过");
        bill.Status = HrEmpLeaveConst.StatusApproved;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _restoreRep.UpdateAsync(bill);
        if (bill.PlanEffectDate.Date <= DateTime.Today)
            await _apply.TryApplyRestoreAsync(bill.Id);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Reject"), HttpPost]
    [DisplayName("复职驳回")]
    public async Task Reject(HrEmpLeaveRejectInput input)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可驳回");
        if (bill.Status != HrEmpLeaveConst.StatusInReview) throw Oops.Oh("仅审核中可驳回");
        bill.Status = HrEmpLeaveConst.StatusRejected;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = input.RejectReason?.Trim();
        await _restoreRep.UpdateAsync(bill);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Void"), HttpPost]
    [DisplayName("撤销复职")]
    public async Task Void(HrEmpLeaveVoidInput input)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("复职单不存在");
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) throw Oops.Oh("非复职单不可撤销");
        if (bill.Status != HrEmpLeaveConst.StatusEffective) throw Oops.Oh("仅已生效可撤销");
        await _apply.VoidRestoreAsync(bill, input.VoidReason?.Trim());
    }

    private async Task AssertCanOpenRestore(string empId, long? excludeId)
    {
        var rq = _restoreRep.AsQueryable()
            .Where(u => u.EmpId == empId
                && u.Kind == HrEmpLeaveConst.KindReinstate
                && HrEmpLeaveConst.OpenStatuses.Contains(u.Status));
        if (excludeId is > 0) rq = rq.Where(u => u.Id != excludeId);
        if (await rq.AnyAsync())
            throw Oops.Oh("该员工已有在途复职单，不能再发起");

        if (await _leaveRep.IsAnyAsync(u => u.EmpId == empId && HrEmpLeaveConst.OpenStatuses.Contains(u.Status)))
            throw Oops.Oh("该员工已有在途离职单，不能办理复职");
        if (await _suspendRep.IsAnyAsync(u => u.EmpId == empId && HrEmpSuspendConst.OpenStatuses.Contains(u.Status)))
            throw Oops.Oh("该员工已有在途停职单，不能办理复职");
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
        var prefix = HrEmpLeaveConst.RestoreNoPrefix + day;
        var lasts = await _restoreRep.AsQueryable()
            .Where(u => u.RestoreNo.StartsWith(prefix))
            .OrderByDescending(u => u.RestoreNo)
            .Select(u => u.RestoreNo).Take(1).ToListAsync();
        var last = lasts.FirstOrDefault();
        var seq = 1;
        if (!string.IsNullOrEmpty(last) && last.Length >= prefix.Length + 4
            && int.TryParse(last.AsSpan(prefix.Length), out var n))
            seq = n + 1;
        return $"{prefix}{seq:D4}";
    }

    private static HrEmpLeaveRestoreOutput ToOutput(HrEmpLeaveRestore u) => new()
    {
        Id = u.Id,
        RestoreNo = u.RestoreNo,
        Kind = u.Kind,
        LeaveId = u.LeaveId,
        LeaveNo = u.LeaveNo,
        EmpId = u.EmpId,
        EmpName = u.EmpName,
        RestoreDate = u.RestoreDate,
        PlanEffectDate = u.PlanEffectDate,
        ClearBlacklist = u.ClearBlacklist,
        Remark = u.Remark,
        Status = u.Status,
        StatusText = HrEmpLeaveService.StatusText(u.Status),
        SubmitTime = u.SubmitTime,
        ApproveTime = u.ApproveTime,
        RejectReason = u.RejectReason,
        EffectiveTime = u.EffectiveTime,
        VoidTime = u.VoidTime,
        VoidReason = u.VoidReason,
        OperateUserId = u.OperateUserId,
        OperateUserName = u.OperateUserName ?? u.CreateUserName,
        OperateTime = u.OperateTime ?? u.EffectiveTime ?? u.CreateTime,
        CreateTime = u.CreateTime,
        CreateUserName = u.CreateUserName
    };
}
