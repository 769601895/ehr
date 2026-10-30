using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 员工异动管理（独立模块，不接审批流程）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrEmpChg", Order = 473, Description = "员工异动管理")]
public class HrEmpChgService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrEmpChg> _chgRep;
    private readonly SqlSugarRepository<HrEmpChgLine> _lineRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<HrPost> _postRep;
    private readonly SqlSugarRepository<HrWfDef> _wfDefRep;
    private readonly HrEmpChgApplyService _apply;
    private readonly HrCodeMstrService _codeSvc;
    private readonly UserManager _userManager;

    public HrEmpChgService(
        SqlSugarRepository<HrEmpChg> chgRep,
        SqlSugarRepository<HrEmpChgLine> lineRep,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<HrPost> postRep,
        SqlSugarRepository<HrWfDef> wfDefRep,
        HrEmpChgApplyService apply,
        HrCodeMstrService codeSvc,
        UserManager userManager)
    {
        _chgRep = chgRep;
        _lineRep = lineRep;
        _empRep = empRep;
        _ccRep = ccRep;
        _postRep = postRep;
        _wfDefRep = wfDefRep;
        _apply = apply;
        _codeSvc = codeSvc;
        _userManager = userManager;
    }

    /// <summary>异动分页（服务端排序）</summary>
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("异动分页")]
    public async Task<SqlSugarPagedList<HrEmpChgOutput>> Page(HrEmpChgPageInput input)
    {
        input ??= new HrEmpChgPageInput();
        var keyword = input.Keyword?.Trim();
        var status = input.Status?.Trim();
        var chgType = input.ChgType?.Trim();

        var query = _chgRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.EmpId.Contains(keyword!) || (u.EmpName != null && u.EmpName.Contains(keyword!)))
            .WhereIF(!string.IsNullOrWhiteSpace(status), u => u.Status == status)
            .WhereIF(!string.IsNullOrWhiteSpace(chgType), u => u.ChgType == chgType)
            .WhereIF(input.ChgDateBegin.HasValue, u => u.ChgDate >= input.ChgDateBegin!.Value.Date)
            .WhereIF(input.ChgDateEnd.HasValue, u => u.ChgDate <= input.ChgDateEnd!.Value.Date);

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: "ChgDate", descSort: true)
            .Select(u => new HrEmpChgOutput
            {
                Id = u.Id,
                ChgNo = u.ChgNo,
                EmpId = u.EmpId,
                EmpName = u.EmpName,
                ChgDate = u.ChgDate,
                ChgType = u.ChgType,
                ChgReason = u.ChgReason,
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

        await FillListExtras(page.Items);
        return page;
    }

    /// <summary>异动详情</summary>
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("异动详情")]
    public async Task<HrEmpChgOutput> Detail([FromQuery] long id)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("异动单不存在");
        // 超管无软删过滤器，须显式排除已删明细，否则前后值/生效会混入历史行
        var lines = await _lineRep.AsQueryable()
            .Where(u => u.ChgId == id && !u.IsDelete)
            .OrderBy(u => u.SortNo)
            .ToListAsync();

        var output = ToOutput(bill);
        output.Lines = lines.Select(ToLineOutput).ToList();
        await FillLineTexts(output.Lines);
        await FillStatusAndType([output]);
        FillOccNcc(output, lines);
        await FillDeptTexts([output]);
        return output;
    }

    /// <summary>按工号生成默认明细：前=后=当前主档</summary>
    [ApiDescriptionSettings(Name = "PreviewDefaults"), HttpGet]
    [DisplayName("预览异动默认值")]
    public async Task<List<HrEmpChgLineOutput>> PreviewDefaults([FromQuery] string empId)
    {
        if (string.IsNullOrWhiteSpace(empId))
            throw Oops.Oh("请选择员工");
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId.Trim())
            ?? throw Oops.Oh("人员不存在");
        var lines = BuildDefaultLines(emp);
        await FillLineTexts(lines);
        return lines;
    }

    /// <summary>保存草稿</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDraft"), HttpPost]
    [DisplayName("保存异动草稿")]
    public async Task<long> SaveDraft(HrEmpChgSaveDraftInput input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.EmpId))
            throw Oops.Oh("请选择员工");
        if (input.ChgDate == default)
            throw Oops.Oh("请填写异动日期");

        var empId = input.EmpId.Trim();
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == empId)
            ?? throw Oops.Oh("人员不存在");

        await AssertCanOpen(empId, input.ChgDate.Date, input.Id);

        if (input.Id is > 0)
        {
            var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
                ?? throw Oops.Oh("异动单不存在");
            if (bill.Status != HrEmpChgConst.StatusDraft)
                throw Oops.Oh("仅草稿可修改");

            var empChanged = !string.Equals(bill.EmpId, empId, StringComparison.OrdinalIgnoreCase);
            // 换人或显式重载：明细前后值按新主档重置
            if (empChanged)
                input.ReloadFromEmp = true;

            bill.EmpId = empId;
            bill.EmpName = emp.EmpName;
            bill.ChgDate = input.ChgDate.Date;
            bill.ChgType = input.ChgType?.Trim() ?? bill.ChgType ?? "";
            bill.ChgReason = input.ChgReason?.Trim();
            bill.Remark = input.Remark?.Trim();
            await _chgRep.UpdateAsync(bill);

            var lines = await RebuildLinesAsync(bill, emp, input);
            await ReplaceLinesAsync(bill.Id, lines);
            return bill.Id;
        }

        var entity = new HrEmpChg
        {
            Id = YitIdHelper.NextId(),
            ChgNo = await NextChgNoAsync(),
            EmpId = empId,
            EmpName = emp.EmpName,
            ChgDate = input.ChgDate.Date,
            ChgType = input.ChgType?.Trim() ?? "",
            ChgReason = input.ChgReason?.Trim(),
            Remark = input.Remark?.Trim(),
            Status = HrEmpChgConst.StatusDraft
        };
        await _chgRep.InsertAsync(entity);
        var newLines = await RebuildLinesAsync(entity, emp, input);
        await ReplaceLinesAsync(entity.Id, newLines);
        return entity.Id;
    }

    /// <summary>删除草稿</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "DeleteDraft"), HttpPost]
    [DisplayName("删除异动草稿")]
    public async Task DeleteDraft(HrEmpChgIdInput input)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("异动单不存在");
        if (bill.Status != HrEmpChgConst.StatusDraft)
            throw Oops.Oh("仅草稿可删除");
        await _lineRep.FakeDeleteAsync(u => u.ChgId == bill.Id);
        await _chgRep.FakeDeleteAsync(bill);
    }

    /// <summary>
    /// 提交：有启用审批流程(hrEmpChg) → 审核中；
    /// 未配置 → 直接通过（待生效/到日写主档），无需再点「通过」。
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Submit"), HttpPost]
    [DisplayName("提交异动")]
    public async Task<HrEmpChgSubmitOutput> Submit(HrEmpChgIdInput input)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("异动单不存在");
        if (bill.Status != HrEmpChgConst.StatusDraft)
            throw Oops.Oh("仅草稿可提交");

        if (string.IsNullOrWhiteSpace(bill.EmpId))
            throw Oops.Oh("请选择员工");
        if (bill.ChgDate == default)
            throw Oops.Oh("请填写异动日期");
        if (string.IsNullOrWhiteSpace(bill.ChgType))
            throw Oops.Oh("请选择异动类别");

        var lines = await _lineRep.AsQueryable()
            .Where(u => u.ChgId == bill.Id && !u.IsDelete)
            .ToListAsync();
        var deptAfter = lines.FirstOrDefault(u =>
            string.Equals(u.FieldKey, HrEmpChgConst.FieldCcCode, StringComparison.OrdinalIgnoreCase))?.ValueAfter;
        if (string.IsNullOrWhiteSpace(deptAfter))
            throw Oops.Oh("新部门不能为空");

        await AssertCanOpen(bill.EmpId, bill.ChgDate.Date, bill.Id);

        var now = DateTime.Now;
        bill.SubmitTime = now;
        bill.SubmitUserId = _userManager.UserId;

        // 仅认本模块业务类型；测试用 hrEmh 不参与判定
        var hasWf = await _wfDefRep.IsAnyAsync(u =>
            u.BizType == HrEmpChgConst.WfBizType && u.Enabled);

        if (hasWf)
        {
            bill.Status = HrEmpChgConst.StatusInReview;
            await _chgRep.UpdateAsync(bill);
            return new HrEmpChgSubmitOutput
            {
                AutoApproved = false,
                Status = bill.Status
            };
        }

        // 无审批流程：提交即通过
        bill.Status = HrEmpChgConst.StatusApproved;
        bill.ApproveTime = now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _chgRep.UpdateAsync(bill);

        if (bill.ChgDate.Date <= DateTime.Today)
            await _apply.TryApplyAsync(bill.Id);

        // Apply 后可能已变为 Effective，再读一次状态
        var latest = await _chgRep.GetFirstAsync(u => u.Id == bill.Id);
        return new HrEmpChgSubmitOutput
        {
            AutoApproved = true,
            Status = latest?.Status ?? bill.Status
        };
    }

    /// <summary>审核通过 → 待生效；若异动日已到则立即写主档</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Approve"), HttpPost]
    [DisplayName("异动审核通过")]
    public async Task Approve(HrEmpChgIdInput input)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("异动单不存在");
        if (bill.Status != HrEmpChgConst.StatusInReview)
            throw Oops.Oh("仅审核中可通过");

        bill.Status = HrEmpChgConst.StatusApproved;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = null;
        await _chgRep.UpdateAsync(bill);

        if (bill.ChgDate.Date <= DateTime.Today)
            await _apply.TryApplyAsync(bill.Id);
    }

    /// <summary>驳回</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Reject"), HttpPost]
    [DisplayName("异动驳回")]
    public async Task Reject(HrEmpChgRejectInput input)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("异动单不存在");
        if (bill.Status != HrEmpChgConst.StatusInReview)
            throw Oops.Oh("仅审核中可驳回");

        bill.Status = HrEmpChgConst.StatusRejected;
        bill.ApproveTime = DateTime.Now;
        bill.ApproveUserId = _userManager.UserId;
        bill.RejectReason = input.RejectReason?.Trim();
        await _chgRep.UpdateAsync(bill);
    }

    /// <summary>撤销已生效单并回滚主档</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Void"), HttpPost]
    [DisplayName("撤销异动")]
    public async Task Void(HrEmpChgVoidInput input)
    {
        var bill = await _chgRep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("异动单不存在");
        if (bill.Status != HrEmpChgConst.StatusEffective)
            throw Oops.Oh("仅已生效可撤销");

        var lines = await _lineRep.AsQueryable()
            .Where(u => u.ChgId == bill.Id && !u.IsDelete)
            .ToListAsync();
        await _apply.VoidAsync(bill, lines, input.VoidReason?.Trim());
    }

    // ----------------- 私有 -----------------

    private async Task AssertCanOpen(string empId, DateTime chgDate, long? excludeId)
    {
        var q = _chgRep.AsQueryable()
            .Where(u => u.EmpId == empId && HrEmpChgConst.BlockStatuses.Contains(u.Status));
        if (excludeId is > 0)
            q = q.Where(u => u.Id != excludeId);

        var list = await q.Select(u => new { u.Id, u.Status, u.ChgDate }).ToListAsync();

        if (list.Any(u => u.Status == HrEmpChgConst.StatusInReview))
            throw Oops.Oh("该员工已有审核中的异动单，不能再发起");
        if (list.Any(u => u.Status == HrEmpChgConst.StatusApproved))
            throw Oops.Oh("该员工已有待生效的异动单，不能再发起");
        if (list.Any(u => u.ChgDate.Date > chgDate.Date))
            throw Oops.Oh("已存在更晚日期的异动记录，不能插入更早的异动");
        if (list.Any(u => u.ChgDate.Date == chgDate.Date))
            throw Oops.Oh("该员工同一异动日已有单据，请先处理或撤销后再开");
        if (list.Count(u => u.Status == HrEmpChgConst.StatusDraft) >= 1)
            throw Oops.Oh("该员工已有草稿异动单，请先处理后再开");
    }

    private async Task<List<HrEmpChgLine>> RebuildLinesAsync(HrEmpChg bill, HrEmployee emp, HrEmpChgSaveDraftInput input)
    {
        Dictionary<string, string?> afterMap = new(StringComparer.OrdinalIgnoreCase);
        if (input.Lines != null)
        {
            foreach (var l in input.Lines)
            {
                if (string.IsNullOrWhiteSpace(l.FieldKey)) continue;
                afterMap[l.FieldKey.Trim()] = l.ValueAfter?.Trim();
            }
        }

        List<HrEmpChgLineOutput> baseLines;
        if (input.ReloadFromEmp || input.Id is not > 0)
        {
            baseLines = BuildDefaultLines(emp);
        }
        else
        {
            var exist = await _lineRep.AsQueryable()
                .Where(u => u.ChgId == bill.Id && !u.IsDelete)
                .OrderBy(u => u.SortNo)
                .ToListAsync();
            // 同一 FieldKey 只留一行，避免历史脏数据或超管未过滤软删导致重复
            baseLines = exist.Count == 0
                ? BuildDefaultLines(emp)
                : exist
                    .GroupBy(u => u.FieldKey, StringComparer.OrdinalIgnoreCase)
                    .Select(g => ToLineOutput(g.OrderByDescending(x => x.Id).First()))
                    .ToList();
        }

        var result = new List<HrEmpChgLine>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var bl in baseLines)
        {
            if (string.IsNullOrWhiteSpace(bl.FieldKey) || !seen.Add(bl.FieldKey))
                continue;
            var after = afterMap.TryGetValue(bl.FieldKey, out var v) ? v : bl.ValueAfter;
            // 未传 after 且非 reload 新建：保留原 after；新建默认 before=after 已设
            result.Add(new HrEmpChgLine
            {
                Id = YitIdHelper.NextId(),
                ChgId = bill.Id,
                FieldKey = bl.FieldKey,
                DbColumn = bl.DbColumn,
                Label = bl.Label,
                ValueBefore = bl.ValueBefore,
                ValueAfter = after,
                SortNo = bl.SortNo
            });
        }

        // 保证两行齐
        EnsureLine(result, bill.Id, HrEmpChgConst.FieldCcCode, HrEmpChgConst.ColCcCode, HrEmpChgConst.LabelCcCode, 10, emp.CcCode, afterMap);
        EnsureLine(result, bill.Id, HrEmpChgConst.FieldPostId, HrEmpChgConst.ColPostId, HrEmpChgConst.LabelPostId, 20, emp.PostId, afterMap);
        return result;
    }

    private static void EnsureLine(
        List<HrEmpChgLine> lines, long chgId, string fieldKey, string dbCol, string label, int sort,
        string? before, Dictionary<string, string?> afterMap)
    {
        if (lines.Any(u => string.Equals(u.FieldKey, fieldKey, StringComparison.OrdinalIgnoreCase)))
            return;
        afterMap.TryGetValue(fieldKey, out var after);
        lines.Add(new HrEmpChgLine
        {
            Id = YitIdHelper.NextId(),
            ChgId = chgId,
            FieldKey = fieldKey,
            DbColumn = dbCol,
            Label = label,
            ValueBefore = before,
            ValueAfter = after ?? before,
            SortNo = sort
        });
    }

    private async Task ReplaceLinesAsync(long chgId, List<HrEmpChgLine> lines)
    {
        await _lineRep.FakeDeleteAsync(u => u.ChgId == chgId);
        if (lines.Count > 0)
            await _lineRep.InsertRangeAsync(lines);
    }

    private static List<HrEmpChgLineOutput> BuildDefaultLines(HrEmployee emp) =>
    [
        new()
        {
            FieldKey = HrEmpChgConst.FieldCcCode,
            DbColumn = HrEmpChgConst.ColCcCode,
            Label = HrEmpChgConst.LabelCcCode,
            ValueBefore = emp.CcCode,
            ValueAfter = emp.CcCode,
            SortNo = 10
        },
        new()
        {
            FieldKey = HrEmpChgConst.FieldPostId,
            DbColumn = HrEmpChgConst.ColPostId,
            Label = HrEmpChgConst.LabelPostId,
            ValueBefore = emp.PostId,
            ValueAfter = emp.PostId,
            SortNo = 20
        }
    ];

    private async Task<string> NextChgNoAsync()
    {
        var day = DateTime.Today.ToString("yyyyMMdd");
        var prefix = HrEmpChgConst.NoPrefix + day;
        var lasts = await _chgRep.AsQueryable()
            .Where(u => u.ChgNo.StartsWith(prefix))
            .OrderByDescending(u => u.ChgNo)
            .Select(u => u.ChgNo)
            .Take(1)
            .ToListAsync();
        var last = lasts.FirstOrDefault();
        var seq = 1;
        if (!string.IsNullOrEmpty(last) && last.Length >= prefix.Length + 4
            && int.TryParse(last.AsSpan(prefix.Length), out var n))
            seq = n + 1;
        return $"{prefix}{seq:D4}";
    }

    private static void NormalizeSort(HrEmpChgPageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Field))
        {
            input.Field = "chgDate";
            input.Order = input.DescStr ?? "desc";
        }
    }

    private static HrEmpChgOutput ToOutput(HrEmpChg u) => new()
    {
        Id = u.Id,
        ChgNo = u.ChgNo,
        EmpId = u.EmpId,
        EmpName = u.EmpName,
        ChgDate = u.ChgDate,
        ChgType = u.ChgType,
        ChgReason = u.ChgReason,
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

    private static HrEmpChgLineOutput ToLineOutput(HrEmpChgLine u) => new()
    {
        Id = u.Id,
        FieldKey = u.FieldKey,
        DbColumn = u.DbColumn,
        Label = u.Label,
        ValueBefore = u.ValueBefore,
        ValueAfter = u.ValueAfter,
        SortNo = u.SortNo
    };

    private static void FillOccNcc(HrEmpChgOutput o, List<HrEmpChgLine> lines)
    {
        var dept = lines.FirstOrDefault(u =>
            string.Equals(u.FieldKey, HrEmpChgConst.FieldCcCode, StringComparison.OrdinalIgnoreCase));
        o.OccCode = dept?.ValueBefore;
        o.NccCode = dept?.ValueAfter;
    }

    private async Task FillListExtras(IEnumerable<HrEmpChgOutput> items)
    {
        var list = items.ToList();
        if (list.Count == 0) return;

        var ids = list.Select(u => u.Id).ToList();
        var deptLines = await _lineRep.AsQueryable()
            .Where(u => ids.Contains(u.ChgId)
                        && !u.IsDelete
                        && u.FieldKey == HrEmpChgConst.FieldCcCode)
            .ToListAsync();
        // GroupBy：超管无软删过滤时仍可能残留同 ChgId 多行，避免 ToDictionary 抛错
        var map = deptLines
            .GroupBy(u => u.ChgId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First());
        foreach (var o in list)
        {
            if (map.TryGetValue(o.Id, out var line))
            {
                o.OccCode = line.ValueBefore;
                o.NccCode = line.ValueAfter;
            }
        }

        await FillStatusAndType(list);
        await FillDeptTexts(list);
    }

    private async Task FillStatusAndType(List<HrEmpChgOutput> list)
    {
        var typeItems = await _codeSvc.GetCodeList(HrEmpChgConst.CodeFieldType);
        var typeMap = typeItems
            .GroupBy(u => u.Value, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Label, StringComparer.OrdinalIgnoreCase);
        foreach (var o in list)
        {
            o.StatusText = StatusText(o.Status);
            if (!string.IsNullOrWhiteSpace(o.ChgType) && typeMap.TryGetValue(o.ChgType, out var t))
                o.ChgTypeText = t;
            else
                o.ChgTypeText = o.ChgType;
        }
    }

    private async Task FillDeptTexts(List<HrEmpChgOutput> list)
    {
        var codes = list.SelectMany(u => new[] { u.OccCode, u.NccCode })
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (codes.Count == 0) return;
        var rows = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .Select(u => new { u.CcCode, u.CcDesc })
            .ToListAsync();
        var map = rows
            .GroupBy(u => u.CcCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().CcDesc, StringComparer.OrdinalIgnoreCase);
        foreach (var o in list)
        {
            if (!string.IsNullOrWhiteSpace(o.OccCode) && map.TryGetValue(o.OccCode!, out var ot))
                o.OccText = ot;
            if (!string.IsNullOrWhiteSpace(o.NccCode) && map.TryGetValue(o.NccCode!, out var nt))
                o.NccText = nt;
        }
    }

    private async Task FillLineTexts(List<HrEmpChgLineOutput> lines)
    {
        var ccCodes = lines
            .Where(u => string.Equals(u.FieldKey, HrEmpChgConst.FieldCcCode, StringComparison.OrdinalIgnoreCase))
            .SelectMany(u => new[] { u.ValueBefore, u.ValueAfter })
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Dictionary<string, string?> ccMap = new(StringComparer.OrdinalIgnoreCase);
        if (ccCodes.Count > 0)
        {
            var rows = await _ccRep.AsQueryable()
                .Where(u => ccCodes.Contains(u.CcCode))
                .Select(u => new { u.CcCode, u.CcDesc })
                .ToListAsync();
            foreach (var r in rows)
                ccMap[r.CcCode] = r.CcDesc;
        }

        var postCodes = lines
            .Where(u => string.Equals(u.FieldKey, HrEmpChgConst.FieldPostId, StringComparison.OrdinalIgnoreCase))
            .SelectMany(u => new[] { u.ValueBefore, u.ValueAfter })
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Dictionary<string, string?> postMap = new(StringComparer.OrdinalIgnoreCase);
        if (postCodes.Count > 0)
        {
            var rows = await _postRep.AsQueryable()
                .Where(u => postCodes.Contains(u.PostCode))
                .Select(u => new { u.PostCode, u.PostName })
                .ToListAsync();
            foreach (var r in rows)
                postMap[r.PostCode] = r.PostName;
        }

        foreach (var line in lines)
        {
            if (string.Equals(line.FieldKey, HrEmpChgConst.FieldCcCode, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(line.ValueBefore) && ccMap.TryGetValue(line.ValueBefore!, out var bt))
                    line.ValueBeforeText = bt;
                if (!string.IsNullOrWhiteSpace(line.ValueAfter) && ccMap.TryGetValue(line.ValueAfter!, out var at))
                    line.ValueAfterText = at;
            }
            else if (string.Equals(line.FieldKey, HrEmpChgConst.FieldPostId, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(line.ValueBefore) && postMap.TryGetValue(line.ValueBefore!, out var bt))
                    line.ValueBeforeText = bt;
                if (!string.IsNullOrWhiteSpace(line.ValueAfter) && postMap.TryGetValue(line.ValueAfter!, out var at))
                    line.ValueAfterText = at;
            }
        }
    }

    private static string StatusText(string? status) => status switch
    {
        HrEmpChgConst.StatusDraft => "草稿",
        HrEmpChgConst.StatusInReview => "审核中",
        HrEmpChgConst.StatusApproved => "待生效",
        HrEmpChgConst.StatusEffective => "已生效",
        HrEmpChgConst.StatusRejected => "已驳回",
        HrEmpChgConst.StatusVoided => "已作废",
        _ => status ?? ""
    };
}
