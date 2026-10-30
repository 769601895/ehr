using Furion.DatabaseAccessor;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Application;

/// <summary>
/// 异动写主档 / 撤销回滚（一期唯一写员工资料入口，不走流程 Handler）
/// </summary>
public class HrEmpChgApplyService : ITransient
{
    private readonly SqlSugarRepository<HrEmpChg> _chgRep;
    private readonly SqlSugarRepository<HrEmpChgLine> _lineRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrEmhHist> _histRep;
    private readonly UserManager _userManager;
    private readonly ILogger _logger;

    public HrEmpChgApplyService(
        SqlSugarRepository<HrEmpChg> chgRep,
        SqlSugarRepository<HrEmpChgLine> lineRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrEmhHist> histRep,
        UserManager userManager,
        ILoggerFactory loggerFactory)
    {
        _chgRep = chgRep;
        _lineRep = lineRep;
        _empRep = empRep;
        _histRep = histRep;
        _userManager = userManager;
        _logger = loggerFactory.CreateLogger(nameof(HrEmpChgApplyService));
    }

    /// <summary>扫描待生效且异动日已到的单据，逐单生效</summary>
    public async Task<int> ApplyDueAsync()
    {
        var today = DateTime.Today;
        var ids = await _chgRep.AsQueryable()
            .Where(u => u.Status == HrEmpChgConst.StatusApproved && u.ChgDate <= today)
            .Select(u => u.Id)
            .ToListAsync();

        var ok = 0;
        foreach (var id in ids)
        {
            try
            {
                if (await TryApplyAsync(id))
                    ok++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "异动生效失败 chgId={Id}", id);
            }
        }
        return ok;
    }

    /// <summary>
    /// 生效写回：把明细「异动后」写入 h_emp_mstr。
    /// value_after 为空则跳过该列，绝不清空岗位。
    /// </summary>
    [UnitOfWork]
    public async Task<bool> TryApplyAsync(long chgId)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == chgId);
        if (bill == null) return false;
        if (bill.Status != HrEmpChgConst.StatusApproved) return false;
        if (bill.ChgDate.Date > DateTime.Today) return false;

        // 超管无软删过滤器，须显式排除已删明细
        var lines = await _lineRep.AsQueryable()
            .Where(u => u.ChgId == chgId && !u.IsDelete)
            .OrderBy(u => u.SortNo)
            .ToListAsync();

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法生效");

        var written = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line.ValueAfter))
                continue;

            var after = line.ValueAfter.Trim();
            if (string.Equals(line.DbColumn, HrEmpChgConst.ColCcCode, StringComparison.OrdinalIgnoreCase))
            {
                emp.CcCode = after;
                written.Add(HrEmpChgConst.ColCcCode);
            }
            else if (string.Equals(line.DbColumn, HrEmpChgConst.ColPostId, StringComparison.OrdinalIgnoreCase))
            {
                emp.PostId = after;
                written.Add(HrEmpChgConst.ColPostId);
            }
        }

        if (!written.Contains(HrEmpChgConst.ColCcCode, StringComparer.OrdinalIgnoreCase))
            throw Oops.Oh("生效失败：新部门未写入");

        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.CcCode, u.PostId })
            .ExecuteCommandAsync();

        var now = DateTime.Now;
        bill.Status = HrEmpChgConst.StatusEffective;
        bill.EffectiveTime = now;
        bill.AppliedColumns = string.Join(",", written);
        await _chgRep.UpdateAsync(bill);

        await InsertHistAsync(bill, lines);
        return true;
    }

    /// <summary>已生效撤销：按异动前回滚 AppliedColumns</summary>
    [UnitOfWork]
    public async Task VoidAsync(HrEmpChg bill, List<HrEmpChgLine> lines, string? reason)
    {
        var later = await _chgRep.AsQueryable()
            .Where(u => u.EmpId == bill.EmpId
                        && u.Id != bill.Id
                        && u.ChgDate > bill.ChgDate
                        && HrEmpChgConst.BlockStatuses.Contains(u.Status))
            .AnyAsync();
        if (later)
            throw Oops.Oh("存在更晚的异动记录，不能撤销本单");

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == bill.EmpId)
            ?? throw Oops.Oh("员工不存在，无法撤销");

        var cols = (bill.AppliedColumns ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (cols.Length == 0)
        {
            // 兼容未记 applied_columns：回滚有 after 的列
            cols = lines
                .Where(u => !string.IsNullOrWhiteSpace(u.ValueAfter))
                .Select(u => u.DbColumn)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        foreach (var col in cols)
        {
            var line = lines.FirstOrDefault(u =>
                string.Equals(u.DbColumn, col, StringComparison.OrdinalIgnoreCase));
            if (line == null) continue;

            if (string.Equals(col, HrEmpChgConst.ColCcCode, StringComparison.OrdinalIgnoreCase))
                emp.CcCode = line.ValueBefore;
            else if (string.Equals(col, HrEmpChgConst.ColPostId, StringComparison.OrdinalIgnoreCase))
                emp.PostId = line.ValueBefore;
        }

        await _empRep.AsUpdateable(emp)
            .UpdateColumns(u => new { u.CcCode, u.PostId })
            .ExecuteCommandAsync();

        bill.Status = HrEmpChgConst.StatusVoided;
        bill.VoidTime = DateTime.Now;
        bill.VoidUserId = _userManager.UserId;
        bill.VoidReason = reason;
        await _chgRep.UpdateAsync(bill);
    }

    private async Task InsertHistAsync(HrEmpChg bill, List<HrEmpChgLine> lines)
    {
        string? Val(string fieldKey, bool after)
        {
            var line = lines.FirstOrDefault(u =>
                string.Equals(u.FieldKey, fieldKey, StringComparison.OrdinalIgnoreCase));
            return after ? line?.ValueAfter : line?.ValueBefore;
        }

        var user = Trunc(_userManager.RealName ?? _userManager.Account ?? "system", 50);
        var now = DateTime.Now;
        var hist = new HrEmhHist
        {
            EmpId = bill.EmpId,
            EmhNbr = Trunc(bill.ChgNo, 50),
            EmhDate = bill.ChgDate.Date,
            EmhWdate = now.Date,
            EmhType = Trunc(bill.ChgType, 50),
            EmhReason = Trunc(bill.ChgReason, 80),
            EmhOcc = Trunc(Val(HrEmpChgConst.FieldCcCode, false), 50),
            EmhNcc = Trunc(Val(HrEmpChgConst.FieldCcCode, true), 50),
            EmhOpost = Trunc(Val(HrEmpChgConst.FieldPostId, false), 50),
            EmhNpost = Trunc(Val(HrEmpChgConst.FieldPostId, true), 50),
            EmhUser = user,
            EmhTime = now,
            EmhRemark = Trunc("异动模块生效", 50),
            EmhAuditOk = 1,
            EmhApplyCommit = 1,
            EmhInure = 1,
            EmhInureDate = now,
            EmhInureUser = user,
            EmhImport = "N"
        };
        await _histRep.InsertAsync(hist);
    }

    private static string? Trunc(string? s, int max)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Length <= max ? s : s[..max];
    }
}
