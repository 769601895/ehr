using System.Globalization;
using System.Text.Json;
using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审批业务生效：Pending → 写主档 + h_emh_hist；支持撤销回滚。
/// </summary>
public class HrWfEffectApplyService : ITransient
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private static readonly HashSet<string> AllowedEmpColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "cc_code", "post_id", "emp_state", "emp_lea_date"
    };

    private readonly SqlSugarRepository<HrWfEffectPolicy> _policyRep;
    private readonly SqlSugarRepository<HrWfEffectFieldMap> _mapRep;
    private readonly SqlSugarRepository<HrWfEffectPending> _pendingRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HrEmhHist> _histRep;
    private readonly UserManager _userManager;

    public HrWfEffectApplyService(
        SqlSugarRepository<HrWfEffectPolicy> policyRep,
        SqlSugarRepository<HrWfEffectFieldMap> mapRep,
        SqlSugarRepository<HrWfEffectPending> pendingRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HrEmhHist> histRep,
        UserManager userManager)
    {
        _policyRep = policyRep;
        _mapRep = mapRep;
        _pendingRep = pendingRep;
        _empRep = empRep;
        _histRep = histRep;
        _userManager = userManager;
    }

    public async Task<HrWfEffectPolicy?> GetEnabledPolicyAsync(string bizType)
    {
        if (string.IsNullOrWhiteSpace(bizType)) return null;
        return await _policyRep.GetFirstAsync(u => u.BizType == bizType.Trim() && u.Enabled);
    }

    public async Task<List<HrWfEffectFieldMap>> ListEnabledMapsAsync(string bizType)
    {
        if (string.IsNullOrWhiteSpace(bizType)) return [];
        return await _mapRep.AsQueryable()
            .Where(u => u.BizType == bizType.Trim() && u.Enabled)
            .OrderBy(u => u.SortNo)
            .ToListAsync();
    }

    [UnitOfWork]
    public async Task<HrWfEffectApplyResult> OnApprovedAsync(HrWfCallback ctx)
    {
        var bizType = ctx.BizType?.Trim() ?? "";
        var policy = await GetEnabledPolicyAsync(bizType);
        if (policy == null)
            return new HrWfEffectApplyResult { Applied = false, Message = "无启用生效策略，跳过写主档" };

        var maps = await ListEnabledMapsAsync(bizType);
        if (maps.Count == 0)
            return new HrWfEffectApplyResult { Applied = false, Message = "无字段对照，跳过写主档" };

        var form = ParseFormDict(ctx.FormDataJson);
        var empId = ResolveSubject(maps, form);
        if (string.IsNullOrWhiteSpace(empId))
            throw Oops.Oh("生效对照缺少当事人工号（Subject），无法写主档");

        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId);
        if (emp == null)
            throw Oops.Oh($"员工 {empId} 不存在");

        var (planDate, dateRemark) = ResolvePlanEffectDate(policy, maps, form);
        var payload = BuildPayload(maps, form, emp, bizType, planDate);
        if (payload.After.Count == 0)
            return new HrWfEffectApplyResult { Applied = false, Message = "无 After 写回字段，跳过", PlanEffectDate = planDate };

        var bizId = string.IsNullOrWhiteSpace(ctx.BizId) ? ctx.InstId.ToString() : ctx.BizId.Trim();
        var pending = await UpsertPendingAsync(bizType, bizId, empId!, planDate, payload, dateRemark);

        var due = pending.PlanEffectDate.Date <= DateTime.Today;
        var immediate = string.Equals(policy.ApplyMode, HrWfConsts.EffectModeImmediateIfDue, StringComparison.OrdinalIgnoreCase);
        if (!due || !immediate)
            return new HrWfEffectApplyResult
            {
                Applied = false,
                Message = due ? "JobOnly，等待任务扫描" : $"待生效，计划日 {pending.PlanEffectDate:yyyy-MM-dd}",
                PlanEffectDate = pending.PlanEffectDate
            };

        return await TryApplyPendingAsync(pending.Id);
    }

    [UnitOfWork]
    public async Task CancelPendingAsync(string bizType, string bizId, string? remark = null)
    {
        if (string.IsNullOrWhiteSpace(bizType) || string.IsNullOrWhiteSpace(bizId)) return;
        var row = await _pendingRep.GetFirstAsync(u =>
            u.BizType == bizType.Trim() && u.BizId == bizId.Trim() && u.Status == HrWfConsts.EffectPending);
        if (row == null) return;
        row.Status = HrWfConsts.EffectCancelled;
        row.Remark = remark?.Trim();
        await _pendingRep.UpdateAsync(row);
    }

    /// <summary>撤销：未生效则取消；已生效则按 Before 回滚主档并作废异动历史。</summary>
    [UnitOfWork]
    public async Task<HrWfEffectApplyResult> VoidAsync(HrWfEffectVoidInput input)
    {
        var bizType = input.BizType.Trim();
        var bizId = input.BizId.Trim();
        var pending = await _pendingRep.GetFirstAsync(u => u.BizType == bizType && u.BizId == bizId)
            ?? throw Oops.Oh("未找到待生效/已生效记录，无法撤销");

        if (pending.Status == HrWfConsts.EffectVoided || pending.Status == HrWfConsts.EffectCancelled)
            return new HrWfEffectApplyResult { Applied = false, Message = $"状态已是 {pending.Status}" };

        if (pending.Status == HrWfConsts.EffectPending)
        {
            pending.Status = HrWfConsts.EffectVoided;
            pending.Remark = string.IsNullOrWhiteSpace(input.Reason) ? "撤销（未生效）" : input.Reason.Trim();
            await _pendingRep.UpdateAsync(pending);
            return new HrWfEffectApplyResult { Applied = false, Message = "已撤销待生效", PlanEffectDate = pending.PlanEffectDate };
        }

        // Applied → 回滚主档
        var payload = JsonSerializer.Deserialize<HrWfEffectPayload>(pending.PayloadJson ?? "{}", JsonOpts)
                      ?? new HrWfEffectPayload();
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == pending.EmpId)
            ?? throw Oops.Oh($"员工 {pending.EmpId} 不存在，无法回滚");

        ApplyEmpColumns(emp, payload.Before);
        // 离职撤销：若 Before 无 emp_state，恢复在职
        if (string.Equals(bizType, HrWfConsts.BizTypeLeave, StringComparison.OrdinalIgnoreCase)
            && !payload.Before.ContainsKey("emp_state"))
            emp.EmpState = 1;

        await UpdateEmpAsync(emp, payload.Before.Keys.Concat(payload.After.Keys).Distinct(StringComparer.OrdinalIgnoreCase));

        if (payload.HistId is > 0)
        {
            var hist = await _histRep.GetFirstAsync(u => u.EmhId == payload.HistId.Value);
            if (hist != null)
            {
                hist.EmhInure = 0;
                hist.EmhRemark = Trunc((string.IsNullOrWhiteSpace(input.Reason) ? "已撤销" : input.Reason.Trim()), 50);
                hist.EmhTime = DateTime.Now;
                hist.EmhUser = Trunc(_userManager.RealName ?? _userManager.Account, 50);
                await _histRep.UpdateAsync(hist);
            }
        }

        pending.Status = HrWfConsts.EffectVoided;
        pending.Remark = string.IsNullOrWhiteSpace(input.Reason) ? "撤销回滚主档" : input.Reason.Trim();
        await _pendingRep.UpdateAsync(pending);

        return new HrWfEffectApplyResult
        {
            Applied = true,
            Message = "已撤销并回滚主档",
            PlanEffectDate = pending.PlanEffectDate,
            HistId = payload.HistId
        };
    }

    public async Task<int> ApplyDuePendingsAsync(DateTime? asOfDate = null)
    {
        var day = (asOfDate ?? DateTime.Today).Date;
        var list = await _pendingRep.AsQueryable()
            .Where(u => u.Status == HrWfConsts.EffectPending && u.PlanEffectDate <= day)
            .OrderBy(u => u.PlanEffectDate)
            .Take(200)
            .ToListAsync();

        var ok = 0;
        foreach (var row in list)
        {
            try
            {
                var r = await TryApplyPendingAsync(row.Id);
                if (r.Applied) ok++;
            }
            catch
            {
                // 单条失败不阻断
            }
        }
        return ok;
    }

    [UnitOfWork]
    public async Task<HrWfEffectApplyResult> TryApplyPendingAsync(long pendingId)
    {
        var pending = await _pendingRep.GetFirstAsync(u => u.Id == pendingId)
            ?? throw Oops.Oh("待生效记录不存在");
        if (pending.Status == HrWfConsts.EffectApplied)
            return new HrWfEffectApplyResult { Applied = true, Message = "已生效（幂等）", PlanEffectDate = pending.PlanEffectDate, HistId = ReadHistId(pending.PayloadJson) };
        if (pending.Status != HrWfConsts.EffectPending)
            return new HrWfEffectApplyResult { Applied = false, Message = $"状态为 {pending.Status}，跳过" };

        if (pending.PlanEffectDate.Date > DateTime.Today)
            return new HrWfEffectApplyResult
            {
                Applied = false,
                Message = $"未到计划日 {pending.PlanEffectDate:yyyy-MM-dd}",
                PlanEffectDate = pending.PlanEffectDate
            };

        var payload = JsonSerializer.Deserialize<HrWfEffectPayload>(pending.PayloadJson ?? "{}", JsonOpts)
                      ?? new HrWfEffectPayload();
        if (payload.After.Count == 0)
            return new HrWfEffectApplyResult { Applied = false, Message = "payload 无 After" };

        // 离职生效时员工尚在职；调动同理。已离职仅允许非离职业务跳过校验时仍取员工
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == pending.EmpId)
            ?? throw Oops.Oh($"员工 {pending.EmpId} 不存在，无法生效");

        if (!string.Equals(pending.BizType, HrWfConsts.BizTypeLeave, StringComparison.OrdinalIgnoreCase)
            && emp.EmpState == 3)
            throw Oops.Oh($"员工 {pending.EmpId} 已离职，无法做异动生效");

        ApplyEmpColumns(emp, payload.After);
        await UpdateEmpAsync(emp, payload.After.Keys);

        var histId = await InsertHistAsync(pending, payload);
        payload.HistId = histId;
        pending.PayloadJson = JsonSerializer.Serialize(payload, JsonOpts);
        pending.Status = HrWfConsts.EffectApplied;
        pending.AppliedTime = DateTime.Now;
        await _pendingRep.UpdateAsync(pending);

        return new HrWfEffectApplyResult
        {
            Applied = true,
            Message = "已写主档并记入 h_emh_hist",
            PlanEffectDate = pending.PlanEffectDate,
            HistId = histId
        };
    }

    private async Task<int> InsertHistAsync(HrWfEffectPending pending, HrWfEffectPayload payload)
    {
        var form = payload.Form ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        string? F(string key) => form.TryGetValue(key, out var v) ? v : null;
        string? Col(string col, bool after)
        {
            var dict = after ? payload.After : payload.Before;
            return dict.TryGetValue(col, out var v) ? v : null;
        }

        var isLeave = string.Equals(pending.BizType, HrWfConsts.BizTypeLeave, StringComparison.OrdinalIgnoreCase);
        var user = Trunc(_userManager.RealName ?? _userManager.Account ?? "system", 50);
        var now = DateTime.Now;

        var hist = new HrEmhHist
        {
            EmpId = pending.EmpId,
            EmhNbr = Trunc(pending.BizId, 50),
            EmhDate = pending.PlanEffectDate,
            EmhWdate = now.Date,
            EmhType = isLeave ? HrWfConsts.EmhTypeLeave : HrWfConsts.EmhTypeTransfer,
            EmhReason = Trunc(F("emh_reason") ?? F("leave_reason") ?? pending.Remark, 80),
            EmhOcc = Trunc(F("emh_occ") ?? Col("cc_code", false), 50),
            EmhNcc = Trunc(F("emh_ncc") ?? Col("cc_code", true), 50),
            EmhOpost = Trunc(F("opost_id") ?? F("emh_opost") ?? Col("post_id", false), 50),
            EmhNpost = Trunc(F("npost_id") ?? F("emh_npost") ?? Col("post_id", true), 50),
            EmhUser = user,
            EmhTime = now,
            EmhRemark = Trunc(isLeave ? "离职生效" : "异动生效", 50),
            EmhAuditOk = 1,
            EmhApplyCommit = 1,
            EmhInure = 1,
            EmhInureDate = now,
            EmhInureUser = user,
            EmhImport = "N"
        };

        var inserted = await _histRep.InsertReturnEntityAsync(hist);
        return inserted.EmhId;
    }

    private async Task<HrWfEffectPending> UpsertPendingAsync(
        string bizType, string bizId, string empId, DateTime planDate, HrWfEffectPayload payload, string? remark)
    {
        var json = JsonSerializer.Serialize(payload, JsonOpts);
        var exist = await _pendingRep.GetFirstAsync(u => u.BizType == bizType && u.BizId == bizId);
        if (exist != null)
        {
            if (exist.Status == HrWfConsts.EffectApplied)
                throw Oops.Oh("该单据已生效写主档，勿重复通过");
            exist.EmpId = empId;
            exist.PlanEffectDate = planDate.Date;
            exist.Status = HrWfConsts.EffectPending;
            exist.PayloadJson = json;
            exist.AppliedTime = null;
            exist.Remark = remark;
            await _pendingRep.UpdateAsync(exist);
            return exist;
        }

        var entity = new HrWfEffectPending
        {
            Id = Yitter.IdGenerator.YitIdHelper.NextId(),
            BizType = bizType,
            BizId = bizId,
            EmpId = empId,
            PlanEffectDate = planDate.Date,
            Status = HrWfConsts.EffectPending,
            PayloadJson = json,
            Remark = remark
        };
        await _pendingRep.InsertAsync(entity);
        return entity;
    }

    private static HrWfEffectPayload BuildPayload(
        List<HrWfEffectFieldMap> maps,
        Dictionary<string, object?> form,
        HrEmployee emp,
        string bizType,
        DateTime planDate)
    {
        var payload = new HrWfEffectPayload();
        foreach (var kv in form)
            payload.Form[kv.Key] = kv.Value?.ToString()?.Trim();

        foreach (var m in maps.Where(u => !string.IsNullOrWhiteSpace(u.DbColumn)))
        {
            var col = m.DbColumn!.Trim();
            if (!AllowedEmpColumns.Contains(col)) continue;
            form.TryGetValue(m.FormFieldKey, out var raw);
            var val = raw?.ToString()?.Trim();

            if (string.Equals(m.Role, HrWfConsts.EffectRoleAfter, StringComparison.OrdinalIgnoreCase))
                payload.After[col] = val;
            else if (string.Equals(m.Role, HrWfConsts.EffectRoleBefore, StringComparison.OrdinalIgnoreCase))
            {
                // 表单无值则用当前主档快照
                payload.Before[col] = !string.IsNullOrWhiteSpace(val) ? val : ReadEmpCol(emp, col);
            }
        }

        // 离职：强制 After emp_state=3，lea_date=计划生效日（若未映射）
        if (string.Equals(bizType, HrWfConsts.BizTypeLeave, StringComparison.OrdinalIgnoreCase))
        {
            if (!payload.Before.ContainsKey("emp_state"))
                payload.Before["emp_state"] = emp.EmpState?.ToString();
            if (!payload.Before.ContainsKey("emp_lea_date"))
                payload.Before["emp_lea_date"] = emp.EmpLeaDate?.ToString("yyyy-MM-dd");
            payload.After["emp_state"] = "3";
            if (!payload.After.ContainsKey("emp_lea_date") || string.IsNullOrWhiteSpace(payload.After["emp_lea_date"]))
                payload.After["emp_lea_date"] = planDate.ToString("yyyy-MM-dd");
        }

        return payload;
    }

    private static string? ReadEmpCol(HrEmployee emp, string col) => col.ToLowerInvariant() switch
    {
        "cc_code" => emp.CcCode,
        "post_id" => emp.PostId,
        "emp_state" => emp.EmpState?.ToString(),
        "emp_lea_date" => emp.EmpLeaDate?.ToString("yyyy-MM-dd"),
        _ => null
    };

    private static void ApplyEmpColumns(HrEmployee emp, Dictionary<string, string?> values)
    {
        foreach (var kv in values)
        {
            if (string.IsNullOrWhiteSpace(kv.Key) || !AllowedEmpColumns.Contains(kv.Key)) continue;
            switch (kv.Key.Trim().ToLowerInvariant())
            {
                case "cc_code":
                    emp.CcCode = NullIfEmpty(kv.Value);
                    break;
                case "post_id":
                    emp.PostId = NullIfEmpty(kv.Value);
                    break;
                case "emp_state":
                    if (int.TryParse(kv.Value, out var st)) emp.EmpState = st;
                    break;
                case "emp_lea_date":
                    emp.EmpLeaDate = ParseDate(kv.Value);
                    break;
            }
        }
    }

    private async Task UpdateEmpAsync(HrEmployee emp, IEnumerable<string> cols)
    {
        var set = new HashSet<string>(cols.Where(c => !string.IsNullOrWhiteSpace(c)), StringComparer.OrdinalIgnoreCase);
        // 离职强制更新状态列
        if (set.Contains("emp_state") || set.Contains("emp_lea_date"))
        {
            set.Add("emp_state");
            set.Add("emp_lea_date");
        }

        var iEmpId = emp.IEmpId;
        var empIdKey = emp.EmpId;
        var q = _empRep.AsUpdateable(emp).Where(u => u.IEmpId == iEmpId && u.EmpId == empIdKey);

        // SqlSugar UpdateColumns 需表达式；按涉及列组合
        if (set.Contains("cc_code") && set.Contains("post_id") && set.Contains("emp_state"))
            await q.UpdateColumns(u => new { u.CcCode, u.PostId, u.EmpState, u.EmpLeaDate }).ExecuteCommandAsync();
        else if (set.Contains("emp_state") || set.Contains("emp_lea_date"))
            await q.UpdateColumns(u => new { u.EmpState, u.EmpLeaDate }).ExecuteCommandAsync();
        else if (set.Contains("cc_code") && set.Contains("post_id"))
            await q.UpdateColumns(u => new { u.CcCode, u.PostId }).ExecuteCommandAsync();
        else if (set.Contains("cc_code"))
            await q.UpdateColumns(u => new { u.CcCode }).ExecuteCommandAsync();
        else if (set.Contains("post_id"))
            await q.UpdateColumns(u => new { u.PostId }).ExecuteCommandAsync();
        else
            await q.UpdateColumns(u => new { u.CcCode, u.PostId, u.EmpState, u.EmpLeaDate }).ExecuteCommandAsync();
    }

    private static (DateTime planDate, string? remark) ResolvePlanEffectDate(
        HrWfEffectPolicy policy, List<HrWfEffectFieldMap> maps, Dictionary<string, object?> form)
    {
        var field = policy.BaseDateField?.Trim();
        if (string.IsNullOrWhiteSpace(field))
        {
            var fromMap = maps.FirstOrDefault(u =>
                string.Equals(u.Role, HrWfConsts.EffectRoleBaseDate, StringComparison.OrdinalIgnoreCase));
            field = fromMap?.FormFieldKey;
        }

        DateTime? baseDate = null;
        if (!string.IsNullOrWhiteSpace(field) && form.TryGetValue(field!, out var raw))
            baseDate = ParseDate(raw);

        if (baseDate == null)
            return (DateTime.Today, "基准日缺失，暂按通过日");

        return (baseDate.Value.Date.AddDays(policy.DayOffset), null);
    }

    private static string? ResolveSubject(List<HrWfEffectFieldMap> maps, Dictionary<string, object?> form)
    {
        var key = maps.FirstOrDefault(u =>
            string.Equals(u.Role, HrWfConsts.EffectRoleSubject, StringComparison.OrdinalIgnoreCase))?.FormFieldKey;
        if (string.IsNullOrWhiteSpace(key)) return null;
        return form.TryGetValue(key, out var v) ? v?.ToString()?.Trim() : null;
    }

    private static Dictionary<string, object?> ParseFormDict(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, JsonOpts);
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (dict == null) return result;
            foreach (var kv in dict)
                result[kv.Key] = JsonElementToObject(kv.Value);
            return result;
        }
        catch
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static object? JsonElementToObject(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null or JsonValueKind.Undefined => null,
        _ => el.GetRawText()
    };

    private static DateTime? ParseDate(object? raw)
    {
        if (raw == null) return null;
        if (raw is DateTime dt) return dt.Date;
        var s = raw.ToString()?.Trim();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d))
            return d.Date;
        if (DateTime.TryParse(s, out d)) return d.Date;
        return null;
    }

    private static string? NullIfEmpty(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private static string? Trunc(string? s, int max) =>
        string.IsNullOrEmpty(s) ? s : (s.Length <= max ? s : s[..max]);

    private static int? ReadHistId(string? json)
    {
        try
        {
            var p = JsonSerializer.Deserialize<HrWfEffectPayload>(json ?? "{}", JsonOpts);
            return p?.HistId;
        }
        catch { return null; }
    }
}
