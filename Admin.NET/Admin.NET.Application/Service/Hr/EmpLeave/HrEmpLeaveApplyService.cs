using Furion.DatabaseAccessor;
using Microsoft.Extensions.Logging;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>离职/复职到日写主档 + 薪资指令</summary>
public class HrEmpLeaveApplyService : ITransient
{
    private readonly SqlSugarRepository<HrEmpLeave> _leaveRep;
    private readonly SqlSugarRepository<HrEmpLeaveRestore> _restoreRep;
    private readonly SqlSugarRepository<HrEmpLeavePayCmd> _payRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrEmhHist> _histRep;
    private readonly UserManager _userManager;
    private readonly ILogger _logger;

    public HrEmpLeaveApplyService(
        SqlSugarRepository<HrEmpLeave> leaveRep,
        SqlSugarRepository<HrEmpLeaveRestore> restoreRep,
        SqlSugarRepository<HrEmpLeavePayCmd> payRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrEmhHist> histRep,
        UserManager userManager,
        ILoggerFactory loggerFactory)
    {
        _leaveRep = leaveRep;
        _restoreRep = restoreRep;
        _payRep = payRep;
        _empRep = empRep;
        _histRep = histRep;
        _userManager = userManager;
        _logger = loggerFactory.CreateLogger(nameof(HrEmpLeaveApplyService));
    }

    public async Task<int> ApplyDueAsync()
    {
        var today = DateTime.Today;
        var leaveIds = await _leaveRep.AsQueryable()
            .Where(u => u.Status == HrEmpLeaveConst.StatusApproved && u.PlanEffectDate <= today)
            .Select(u => u.Id).ToListAsync();
        var restoreIds = await _restoreRep.AsQueryable()
            .Where(u => u.Kind == HrEmpLeaveConst.KindReinstate
                && u.Status == HrEmpLeaveConst.StatusApproved && u.PlanEffectDate <= today)
            .Select(u => u.Id).ToListAsync();

        var ok = 0;
        foreach (var id in leaveIds)
        {
            try
            {
                if (await TryApplyLeaveAsync(id)) ok++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "离职生效失败 leaveId={Id}", id);
            }
        }
        foreach (var id in restoreIds)
        {
            try
            {
                if (await TryApplyRestoreAsync(id)) ok++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "复职生效失败 restoreId={Id}", id);
            }
        }
        return ok;
    }

    [UnitOfWork]
    public async Task<bool> TryApplyLeaveAsync(long leaveId)
    {
        var bill = await _leaveRep.GetFirstAsync(u => u.Id == leaveId);
        if (bill == null) return false;
        if (bill.Status != HrEmpLeaveConst.StatusApproved) return false;
        if (bill.PlanEffectDate.Date > DateTime.Today) return false;

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法生效");
        if (emp.EmpState == HrEmpLeaveConst.EmpStateLeave)
            throw Oops.Oh("员工已是离职状态，无法重复生效");

        bill.EmpStateBefore = emp.EmpState;
        emp.EmpState = HrEmpLeaveConst.EmpStateLeave;
        emp.EmpLeaDate = bill.LeaveDate.Date;
        if (bill.IsBlacklist)
            emp.EmpBlack = 1;

        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.EmpState, u.EmpLeaDate, u.EmpBlack })
            .ExecuteCommandAsync();

        var now = DateTime.Now;
        bill.Status = HrEmpLeaveConst.StatusEffective;
        bill.EffectiveTime = now;
        await _leaveRep.UpdateAsync(bill);

        await UpsertPayCmdAsync(bill);
        await InsertHistAsync(bill.EmpId, bill.LeaveNo, bill.LeaveDate, HrWfConsts.EmhTypeLeave,
            bill.LeaveType, bill.LeaveReason, bill.CcCodeSnap, bill.CcCodeSnap, "离职生效");
        return true;
    }

    [UnitOfWork]
    public async Task VoidLeaveAsync(HrEmpLeave bill, string? reason)
    {
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法撤销");

        emp.EmpState = bill.EmpStateBefore is > 0 and < 3
            ? bill.EmpStateBefore
            : HrEmpLeaveConst.EmpStateActive;
        emp.EmpLeaDate = null;
        // 黑名单不因撤销自动清除（标记仅人工）
        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.EmpState, u.EmpLeaDate })
            .ExecuteCommandAsync();

        var now = DateTime.Now;
        var opName = Trunc(_userManager.RealName ?? _userManager.Account, 64);
        bill.Status = HrEmpLeaveConst.StatusVoided;
        bill.VoidTime = now;
        bill.VoidUserId = _userManager.UserId;
        bill.VoidReason = reason;
        await _leaveRep.UpdateAsync(bill);

        var cmds = await _payRep.GetListAsync(u =>
            u.LeaveId == bill.Id && u.Status == HrEmpLeaveConst.PayCmdActive);
        foreach (var c in cmds)
        {
            c.Status = HrEmpLeaveConst.PayCmdVoided;
            await _payRep.UpdateAsync(c);
        }

        // 纠错审计记录（kind=Correct；不进入复职列表）
        await _restoreRep.InsertAsync(new HrEmpLeaveRestore
        {
            Id = YitIdHelper.NextId(),
            RestoreNo = await NextCorrectNoAsync(),
            Kind = HrEmpLeaveConst.KindCorrect,
            LeaveId = bill.Id,
            LeaveNo = bill.LeaveNo,
            EmpId = bill.EmpId,
            EmpName = bill.EmpName,
            RestoreDate = now.Date,
            PlanEffectDate = now.Date,
            ClearBlacklist = false,
            Remark = reason,
            Status = HrEmpLeaveConst.StatusEffective,
            EffectiveTime = now,
            OperateUserId = _userManager.UserId,
            OperateUserName = opName,
            OperateTime = now,
            CreateTime = now,
            CreateUserId = _userManager.UserId,
            CreateUserName = opName
        });
    }

    private async Task<string> NextCorrectNoAsync()
    {
        var day = DateTime.Today.ToString("yyyyMMdd");
        var prefix = HrEmpLeaveConst.CorrectNoPrefix + day;
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

    /// <summary>同工号复职生效：主档改在职；原离职单保持已生效</summary>
    [UnitOfWork]
    public async Task<bool> TryApplyRestoreAsync(long restoreId)
    {
        var bill = await _restoreRep.GetFirstAsync(u => u.Id == restoreId);
        if (bill == null) return false;
        if (bill.Kind != HrEmpLeaveConst.KindReinstate) return false;
        if (bill.Status != HrEmpLeaveConst.StatusApproved) return false;
        if (bill.PlanEffectDate.Date > DateTime.Today) return false;

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法复职");
        if (emp.EmpState != HrEmpLeaveConst.EmpStateSuspend)
            throw Oops.Oh("员工非停职状态，无法复职");

        // 停职→在职（同工号）；停职单保持已生效历史
        emp.EmpState = HrEmpLeaveConst.EmpStateActive;
        if (bill.ClearBlacklist)
            emp.EmpBlack = 0;

        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.EmpState, u.EmpBlack })
            .ExecuteCommandAsync();

        bill.Status = HrEmpLeaveConst.StatusEffective;
        bill.EffectiveTime = DateTime.Now;
        await _restoreRep.UpdateAsync(bill);

        await InsertHistAsync(bill.EmpId, bill.RestoreNo, bill.RestoreDate, HrWfConsts.EmhTypeReinstate,
            null, null, emp.CcCode, emp.CcCode, "停职复职生效");
        return true;
    }

    [UnitOfWork]
    public async Task VoidRestoreAsync(HrEmpLeaveRestore bill, string? reason)
    {
        if (bill.Kind != HrEmpLeaveConst.KindReinstate)
            throw Oops.Oh("非复职单不可撤销");

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法撤销复职");

        // 撤销复职 → 回到停职
        emp.EmpState = HrEmpLeaveConst.EmpStateSuspend;
        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.EmpState })
            .ExecuteCommandAsync();

        bill.Status = HrEmpLeaveConst.StatusVoided;
        bill.VoidTime = DateTime.Now;
        bill.VoidUserId = _userManager.UserId;
        bill.VoidReason = reason;
        await _restoreRep.UpdateAsync(bill);
    }

    private async Task UpsertPayCmdAsync(HrEmpLeave bill)
    {
        var period = bill.LeaveDate.ToString("yyyyMM");
        var exist = await _payRep.GetFirstAsync(u =>
            u.LeaveId == bill.Id && u.Status == HrEmpLeaveConst.PayCmdActive);
        if (exist != null)
        {
            exist.SettleCode = bill.SalarySettle;
            exist.LeaveDate = bill.LeaveDate.Date;
            exist.PayPeriod = period;
            await _payRep.UpdateAsync(exist);
            return;
        }
        await _payRep.InsertAsync(new HrEmpLeavePayCmd
        {
            Id = YitIdHelper.NextId(),
            LeaveId = bill.Id,
            LeaveNo = bill.LeaveNo,
            EmpId = bill.EmpId,
            LeaveDate = bill.LeaveDate.Date,
            SettleCode = bill.SalarySettle,
            PayPeriod = period,
            Status = HrEmpLeaveConst.PayCmdActive
        });
    }

    private async Task InsertHistAsync(
        string empId, string nbr, DateTime bizDate, string emhType,
        string? type, string? reason, string? occ, string? ncc, string remark)
    {
        await _histRep.InsertAsync(new HrEmhHist
        {
            EmpId = Trunc(empId, 50),
            EmhNbr = Trunc(nbr, 50),
            EmhDate = bizDate.Date,
            EmhWdate = DateTime.Today,
            EmhType = Trunc(emhType, 50),
            EmhReason = Trunc(string.IsNullOrWhiteSpace(reason) ? type : reason, 80),
            EmhOcc = Trunc(occ, 50),
            EmhNcc = Trunc(ncc, 50),
            EmhUser = Trunc(_userManager.RealName ?? _userManager.Account, 50),
            EmhTime = DateTime.Now,
            EmhRemark = Trunc(remark, 50)
        });
    }

    private static string? Trunc(string? s, int len)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Length <= len ? s : s[..len];
    }
}
