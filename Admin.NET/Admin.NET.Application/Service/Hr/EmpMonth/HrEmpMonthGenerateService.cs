using System.Text.Json;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>人事月报名单生成（D8 简化规则）</summary>
public class HrEmpMonthGenerateService : ITransient
{
    private readonly SqlSugarRepository<HrEmpMonthBatch> _batchRep;
    private readonly SqlSugarRepository<HrEmpMonthLine> _lineRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<HrEmpLeavePayCmd> _payCmdRep;
    private readonly SqlSugarRepository<HrEmpChg> _chgRep;
    private readonly SqlSugarRepository<HrEmpSuspend> _suspendRep;
    private readonly HrUserCcScopeService _scope;
    private readonly HrEmpMonthSnapBuilder _snap;

    public HrEmpMonthGenerateService(
        SqlSugarRepository<HrEmpMonthBatch> batchRep,
        SqlSugarRepository<HrEmpMonthLine> lineRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<HrEmpLeavePayCmd> payCmdRep,
        SqlSugarRepository<HrEmpChg> chgRep,
        SqlSugarRepository<HrEmpSuspend> suspendRep,
        HrUserCcScopeService scope,
        HrEmpMonthSnapBuilder snap)
    {
        _batchRep = batchRep;
        _lineRep = lineRep;
        _empRep = empRep;
        _ccRep = ccRep;
        _payCmdRep = payCmdRep;
        _chgRep = chgRep;
        _suspendRep = suspendRep;
        _scope = scope;
        _snap = snap;
    }

    /// <summary>
    /// 生成/重生成。fullReset=true 清空全部行；false 保留 ManualLock 行的纳入标记并重刷快照。
    /// 不改 ParamSnapshot（开批已固化）。
    /// </summary>
    public async Task GenerateAsync(long batchId, bool fullReset)
    {
        var batch = await _batchRep.GetByIdAsync(batchId) ?? throw Oops.Oh("月报批次不存在");
        if (!HrEmpMonthConst.GeneratableStatuses.Contains(batch.Status))
            throw Oops.Oh("当前状态不可生成名单");

        var attFrom = batch.AttFrom.Date;
        var attTo = batch.AttTo.Date;
        var payPeriod = batch.PayPeriod;

        var lockedKeep = new Dictionary<string, HrEmpMonthLine>(StringComparer.OrdinalIgnoreCase);
        if (fullReset)
        {
            await _lineRep.DeleteAsync(u => u.BatchId == batchId);
        }
        else
        {
            var locked = await _lineRep.GetListAsync(u => u.BatchId == batchId && u.ManualLock);
            foreach (var l in locked)
                lockedKeep[l.EmpId] = l;
            await _lineRep.DeleteAsync(u => u.BatchId == batchId && !u.ManualLock);
            if (locked.Count > 0)
                await _lineRep.DeleteAsync(u => u.BatchId == batchId && u.ManualLock);
        }

        var monthSnapFields = await _snap.LoadMonthSnapFieldsAsync();
        var chgKeys = await _snap.LoadChgEnabledFieldKeysAsync();
        var monthSnapKeys = monthSnapFields.Select(f => f.FieldKey).ToArray();

        var payEmpIds = await _payCmdRep.AsQueryable()
            .Where(u => u.PayPeriod == payPeriod && u.Status == HrEmpLeaveConst.PayCmdActive)
            .Select(u => u.EmpId)
            .ToListAsync();
        var payEmpSet = new HashSet<string>(payEmpIds, StringComparer.OrdinalIgnoreCase);

        var empQ = await _scope.ApplyEmpScopeAsync(_empRep.AsQueryable());
        // OR 纳入粗规则
        empQ = empQ.Where(u =>
            // 入职不晚于窗末日，且（未离职 或 离职日>=窗起）→ 窗内有在职/停职可能
            ((u.EmpInDate == null || u.EmpInDate <= attTo)
             && (u.EmpLeaDate == null || u.EmpLeaDate >= attFrom)
             && (u.EmpState == 1 || u.EmpState == 2
                 || (u.EmpLeaDate != null && u.EmpLeaDate >= attFrom && u.EmpLeaDate <= attTo)))
            || (u.EmpInDate != null && u.EmpInDate >= attFrom && u.EmpInDate <= attTo)
            || (u.EmpLeaDate != null && u.EmpLeaDate >= attFrom && u.EmpLeaDate <= attTo)
            || payEmpSet.Contains(u.EmpId));

        var candidates = await empQ.ToListAsync();

        // 补充：仅有本月 PayCmd 但可能管辖内漏查的（已在 scope 内）
        if (payEmpSet.Count > 0)
        {
            var have = new HashSet<string>(candidates.Select(c => c.EmpId), StringComparer.OrdinalIgnoreCase);
            var missing = payEmpSet.Where(id => !have.Contains(id)).ToList();
            if (missing.Count > 0)
            {
                var extraQ = await _scope.ApplyEmpScopeAsync(
                    _empRep.AsQueryable().Where(u => missing.Contains(u.EmpId)));
                candidates.AddRange(await extraQ.ToListAsync());
            }
        }

        // AND 排除
        candidates = candidates
            .Where(u =>
            {
                if (u.EmpInDate != null && u.EmpInDate.Value.Date > attTo) return false;
                if (u.EmpKqIndate != null && u.EmpKqIndate.Value.Date > attTo) return false;
                // 窗前已离职且窗内无在职天数，且无本月指令
                if (u.EmpLeaDate != null && u.EmpLeaDate.Value.Date < attFrom
                    && !payEmpSet.Contains(u.EmpId)
                    && (u.EmpState == 3 || u.EmpState == null))
                    return false;
                return true;
            })
            .GroupBy(u => u.EmpId, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        // 合并手工锁行中不在自动名单的人（仍需在管辖内）
        foreach (var kv in lockedKeep)
        {
            if (candidates.Any(c => c.EmpId.Equals(kv.Key, StringComparison.OrdinalIgnoreCase)))
                continue;
            var empQ2 = await _scope.ApplyEmpScopeAsync(
                _empRep.AsQueryable().Where(u => u.EmpId == kv.Key));
            var row = await empQ2.FirstAsync();
            if (row != null) candidates.Add(row);
        }

        var ccCodes = candidates.Select(c => c.CcCode).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
        var ccNameMap = ccCodes.Count == 0
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : (await _ccRep.AsQueryable().Where(u => ccCodes.Contains(u.CcCode))
                .Select(u => new { u.CcCode, u.CcDesc }).ToListAsync())
                .GroupBy(u => u.CcCode, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().CcDesc ?? g.Key, StringComparer.OrdinalIgnoreCase);

        var empIds = candidates.Select(c => c.EmpId).ToList();
        var payCmds = empIds.Count == 0
            ? []
            : await _payCmdRep.AsQueryable()
                .Where(u => empIds.Contains(u.EmpId) && u.PayPeriod == payPeriod
                            && (u.Status == HrEmpLeaveConst.PayCmdActive || u.Status == HrEmpLeaveConst.PayCmdConsumed))
                .ToListAsync();
        var payByEmp = payCmds
            .GroupBy(u => u.EmpId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First(), StringComparer.OrdinalIgnoreCase);

        var chgEmpIds = empIds.Count == 0
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(
                await _chgRep.AsQueryable()
                    .Where(u => empIds.Contains(u.EmpId)
                                && u.Status == HrEmpChgConst.StatusEffective
                                && u.ChgDate >= attFrom && u.ChgDate <= attTo)
                    .Select(u => u.EmpId)
                    .ToListAsync(),
                StringComparer.OrdinalIgnoreCase);

        var suspendEmpIds = empIds.Count == 0
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(
                await _suspendRep.AsQueryable()
                    .Where(u => empIds.Contains(u.EmpId)
                                && u.Status == HrEmpLeaveConst.StatusEffective
                                && u.SuspendDate >= attFrom && u.SuspendDate <= attTo)
                    .Select(u => u.EmpId)
                    .ToListAsync(),
                StringComparer.OrdinalIgnoreCase);

        var pendingChgEmpIds = empIds.Count == 0
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(
                await _chgRep.AsQueryable()
                    .Where(u => empIds.Contains(u.EmpId)
                                && (u.Status == HrEmpChgConst.StatusApproved || u.Status == HrEmpChgConst.StatusInReview)
                                && u.ChgDate >= attFrom && u.ChgDate <= attTo.AddMonths(1))
                    .Select(u => u.EmpId)
                    .ToListAsync(),
                StringComparer.OrdinalIgnoreCase);

        var lines = new List<HrEmpMonthLine>();
        foreach (var emp in candidates)
        {
            var warns = new List<string>();
            lockedKeep.TryGetValue(emp.EmpId, out var lockRow);

            var includeFlag = lockRow?.IncludeFlag ?? HrEmpMonthConst.IncludeInclude;
            var empFieldJson = _snap.BuildEmpFieldSnapshot(emp, monthSnapFields, warns);
            var (chgJson, chgSummary, hasChg, chgWarns) = await _snap.BuildChgFieldSnapshotAsync(
                emp.EmpId, emp, attFrom, attTo, chgKeys, monthSnapKeys);
            warns.AddRange(chgWarns);

            if (chgEmpIds.Contains(emp.EmpId) || hasChg)
            {
                hasChg = true;
                warns.Add(HrEmpMonthConst.WarnHasChgManualCheck);
            }
            if (pendingChgEmpIds.Contains(emp.EmpId))
                warns.Add(HrEmpMonthConst.WarnChgPending);

            var isSuspend = emp.EmpState == 2 || suspendEmpIds.Contains(emp.EmpId);
            var isNewHire = emp.EmpInDate != null && emp.EmpInDate.Value.Date >= attFrom && emp.EmpInDate.Value.Date <= attTo;
            var isLeaver = emp.EmpLeaDate != null && emp.EmpLeaDate.Value.Date >= attFrom && emp.EmpLeaDate.Value.Date <= attTo;

            payByEmp.TryGetValue(emp.EmpId, out var payCmd);
            var chgDto = HrEmpMonthSnapBuilder.TryParseChg(chgJson);
            var ccAtStart = HrEmpMonthSnapBuilder.ResolveCcCodeAtStart(chgDto, emp.CcCode);
            var ccName = emp.CcCode != null && ccNameMap.TryGetValue(emp.CcCode, out var n) ? n : null;

            var needAtt = includeFlag == HrEmpMonthConst.IncludeInclude
                          && HasAttOverlap(emp, attFrom, attTo);

            lines.Add(new HrEmpMonthLine
            {
                Id = YitIdHelper.NextId(),
                BatchId = batchId,
                EmpId = emp.EmpId,
                IEmpId = emp.IEmpId,
                EmpName = emp.EmpName,
                IncludeFlag = includeFlag,
                ManualLock = lockRow?.ManualLock ?? false,
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
                NeedAttendance = needAtt,
                IsNewHire = isNewHire,
                IsLeaver = isLeaver,
                IsSuspend = isSuspend,
                HasChgInPeriod = hasChg,
                LeaveSettleCode = payCmd?.SettleCode,
                LeavePayCmdStatus = payCmd?.Status,
                PayCmdId = payCmd?.Id,
                CcCodeAtStart = ccAtStart,
                CcCodeAtEnd = emp.CcCode,
                ChgSummary = chgSummary,
                ChgFieldSnapshot = chgJson,
                CostCcCode = emp.CcCode,
                ExcludeReason = lockRow?.ExcludeReason,
                IncludeReason = lockRow?.IncludeReason,
                Remark = lockRow?.Remark,
                EmpFieldSnapshot = empFieldJson,
                WarnCodes = string.Join(",", warns.Distinct()),
                ErrorCodes = null
            });
        }

        if (lines.Count > 0)
            await _lineRep.InsertRangeAsync(lines);

        batch.FieldSchemaSnapshot = HrEmpMonthSnapBuilder.BuildFieldSchemaSnapshot(monthSnapKeys, chgKeys);
        batch.Status = HrEmpMonthConst.StatusGenerated;
        await _batchRep.UpdateAsync(batch);
    }

    private static bool HasAttOverlap(HrEmployee emp, DateTime attFrom, DateTime attTo)
    {
        var start = (emp.EmpKqIndate ?? emp.EmpInDate)?.Date;
        var end = (emp.EmpKqLeaDate ?? emp.EmpLeaDate)?.Date;
        if (start == null && end == null) return true;
        var s = start ?? DateTime.MinValue.Date;
        var e = end ?? DateTime.MaxValue.Date;
        return s <= attTo && e >= attFrom;
    }
}
