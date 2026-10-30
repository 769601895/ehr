using System.Text.Json;
using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审批运行时：Start / Approve / Reject
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfRuntime", Order = 484, Description = "审批运行时")]
public class HrWfRuntimeService : IDynamicApiController, ITransient
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly SqlSugarRepository<HrWfInst> _instRep;
    private readonly SqlSugarRepository<HrWfTask> _taskRep;
    private readonly SqlSugarRepository<HrWfActionLog> _logRep;
    private readonly SqlSugarRepository<HrWfDef> _defRep;
    private readonly SqlSugarRepository<HrWfDefNode> _nodeRep;
    private readonly SqlSugarRepository<HrWfForm> _formRep;
    private readonly SqlSugarRepository<HrWfFormField> _fieldRep;
    private readonly SqlSugarRepository<HrWfRole> _roleRep;
    private readonly HrWfAssigneeResolver _resolver;
    private readonly UserManager _userManager;
    private readonly IEnumerable<IHrWfBizHandler> _handlers;

    public HrWfRuntimeService(
        SqlSugarRepository<HrWfInst> instRep,
        SqlSugarRepository<HrWfTask> taskRep,
        SqlSugarRepository<HrWfActionLog> logRep,
        SqlSugarRepository<HrWfDef> defRep,
        SqlSugarRepository<HrWfDefNode> nodeRep,
        SqlSugarRepository<HrWfForm> formRep,
        SqlSugarRepository<HrWfFormField> fieldRep,
        SqlSugarRepository<HrWfRole> roleRep,
        HrWfAssigneeResolver resolver,
        UserManager userManager,
        IEnumerable<IHrWfBizHandler> handlers)
    {
        _instRep = instRep;
        _taskRep = taskRep;
        _logRep = logRep;
        _defRep = defRep;
        _nodeRep = nodeRep;
        _formRep = formRep;
        _fieldRep = fieldRep;
        _roleRep = roleRep;
        _resolver = resolver;
        _userManager = userManager;
        _handlers = handlers;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Start"), HttpPost]
    [DisplayName("发起审批")]
    public async Task<HrWfStartOutput> Start(HrWfStartInput input)
    {
        var bizType = input.BizType.Trim();
        var applicantId = input.ApplicantUserId is > 0 ? input.ApplicantUserId.Value : _userManager.UserId;

        var def = await _defRep.GetFirstAsync(u => u.BizType == bizType && u.Enabled)
            ?? throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        var form = await _formRep.GetFirstAsync(u => u.Id == def.FormId && u.Enabled)
            ?? throw Oops.Oh(HrWfErrorCodeEnum.FormNotFound);
        var fields = await _fieldRep.AsQueryable()
            .Where(u => u.FormId == form.Id)
            .OrderBy(u => u.SortNo)
            .ToListAsync();
        var nodes = await _nodeRep.AsQueryable()
            .Where(u => u.DefId == def.Id && u.Enabled)
            .OrderBy(u => u.OrderNo)
            .ToListAsync();
        if (nodes.Count == 0) throw Oops.Oh(HrWfErrorCodeEnum.DefNoNodes);

        var formDataDict = NormalizeFormDict(input.FormData);
        ValidateRequired(fields, formDataDict);

        if (string.IsNullOrWhiteSpace(input.SubjectEmpId))
            throw Oops.Oh(HrWfErrorCodeEnum.SubjectEmpRequired);

        var subjectEmpId = input.SubjectEmpId.Trim();
        // 单据部门：优先入参；否则按当事人所属部门自动解析（管辖找人用）
        var subjectCc = input.SubjectCcCode?.Trim();
        if (string.IsNullOrWhiteSpace(subjectCc))
        {
            subjectCc = await _resolver.GetEmployeeCcCodeAsync(subjectEmpId);
            if (string.IsNullOrWhiteSpace(subjectCc))
                throw Oops.Oh("当事人未设置所属部门，无法发起审批");
        }

        var reportPath = await BuildReportPathForStartAsync(subjectEmpId);

        var formDataJson = JsonSerializer.Serialize(formDataDict, JsonOpts);
        var schemaSnap = new HrWfSchemaSnapshot
        {
            FormId = form.Id,
            BizType = form.BizType,
            FormMode = form.FormMode,
            Version = form.Version,
            Fields = fields.Select(ToFieldItem).ToList()
        };
        var nodesSnap = new HrWfNodesSnapshot
        {
            DefId = def.Id,
            Version = def.Version,
            AllowEditWhileRunning = def.AllowEditWhileRunning,
            EditByApplicant = def.EditByApplicant,
            EditByAssignee = def.EditByAssignee,
            AssigneeResolveMode = HrWfConsts.ModeReportPath,
            ReportPathRoleIds = reportPath,
            Nodes = nodes.Select(ToNodeItem).ToList()
        };

        var instId = Yitter.IdGenerator.YitIdHelper.NextId();
        var bizId = string.IsNullOrWhiteSpace(input.BizId) ? instId.ToString() : input.BizId.Trim();

        if (await _instRep.IsAnyAsync(u => u.BizType == bizType && u.BizId == bizId && u.Status == HrWfConsts.InstRunning))
            throw Oops.Oh(HrWfErrorCodeEnum.DuplicateRunning);

        var title = input.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title) && formDataDict.TryGetValue("subject", out var subj))
            title = subj?.ToString();

        var inst = new HrWfInst
        {
            Id = instId,
            BizType = bizType,
            BizId = bizId,
            DefId = def.Id,
            FormId = form.Id,
            Status = HrWfConsts.InstRunning,
            Title = title,
            ApplicantUserId = applicantId,
            SubjectEmpId = subjectEmpId,
            SubjectCcCode = subjectCc,
            FormDataJson = formDataJson,
            FormSchemaSnapshot = JsonSerializer.Serialize(schemaSnap, JsonOpts),
            DefNodesSnapshot = JsonSerializer.Serialize(nodesSnap, JsonOpts),
            AllowEditWhileRunning = def.AllowEditWhileRunning,
            EditByApplicant = def.EditByApplicant,
            EditByAssignee = def.EditByAssignee,
            StartedTime = DateTime.Now
        };
        await _instRep.InsertAsync(inst);

        await AddLogAsync(inst.Id, null, HrWfConsts.ActionStart, applicantId, null, null, null, null);

        await DispatchFromAsync(inst, nodesSnap, startIndex: 0, actorUserId: applicantId);

        var fresh = await _instRep.GetFirstAsync(u => u.Id == inst.Id);
        return new HrWfStartOutput
        {
            InstId = fresh!.Id,
            BizId = fresh.BizId,
            Status = fresh.Status,
            CurrentNodeKey = fresh.CurrentNodeKey
        };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Approve"), HttpPost]
    [DisplayName("同意")]
    public async Task Approve(HrWfApproveInput input)
    {
        var (task, inst, nodesSnap) = await LoadTaskContextAsync(input.TaskId);
        await TryApplyPatchAsync(inst, task, input.FormDataPatch);

        task.Status = HrWfConsts.TaskApproved;
        task.Opinion = input.Opinion?.Trim();
        task.FinishedTime = DateTime.Now;
        await _taskRep.UpdateAsync(task);

        await CancelSiblingPendingAsync(inst.Id, task.NodeKey, task.Id);
        await AddLogAsync(inst.Id, task.Id, HrWfConsts.ActionApprove, _userManager.UserId, task.NodeKey, input.Opinion, null, null);

        var idx = nodesSnap.Nodes.FindIndex(u => u.NodeKey == task.NodeKey);
        await DispatchFromAsync(inst, nodesSnap, startIndex: idx + 1, actorUserId: _userManager.UserId);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Reject"), HttpPost]
    [DisplayName("驳回")]
    public async Task Reject(HrWfRejectInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Opinion))
            throw Oops.Oh(HrWfErrorCodeEnum.RejectOpinionRequired);

        var (task, inst, _) = await LoadTaskContextAsync(input.TaskId);
        await TryApplyPatchAsync(inst, task, input.FormDataPatch);

        task.Status = HrWfConsts.TaskRejected;
        task.Opinion = input.Opinion.Trim();
        task.FinishedTime = DateTime.Now;
        await _taskRep.UpdateAsync(task);

        await CancelSiblingPendingAsync(inst.Id, task.NodeKey, task.Id);

        inst.Status = HrWfConsts.InstRejected;
        inst.FinishedTime = DateTime.Now;
        inst.CurrentNodeKey = null;
        await _instRep.UpdateAsync(inst);

        await AddLogAsync(inst.Id, task.Id, HrWfConsts.ActionReject, _userManager.UserId, task.NodeKey, input.Opinion, null, null);
        await InvokeHandlerAsync(inst, h => h.OnRejectedAsync(ToCallback(inst)));
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "PatchForm"), HttpPost]
    [DisplayName("补丁更新表单")]
    public async Task PatchForm(HrWfPatchFormInput input)
    {
        var inst = await _instRep.GetFirstAsync(u => u.Id == input.InstId) ?? throw Oops.Oh(HrWfErrorCodeEnum.InstNotFound);
        if (inst.Status != HrWfConsts.InstRunning) throw Oops.Oh(HrWfErrorCodeEnum.FormEditDenied);
        EnsureCanEdit(inst, null);
        MergeFormData(inst, input.FormDataPatch);
        await _instRep.UpdateAsync(inst);
        await AddLogAsync(inst.Id, null, HrWfConsts.ActionPatchForm, _userManager.UserId, inst.CurrentNodeKey, null, null,
            JsonSerializer.Serialize(input.FormDataPatch, JsonOpts));
    }

    [ApiDescriptionSettings(Name = "GetDetail"), HttpGet]
    [DisplayName("审批实例详情")]
    public async Task<HrWfInstDetailOutput> GetDetail([FromQuery] long instId)
    {
        var inst = await _instRep.GetFirstAsync(u => u.Id == instId) ?? throw Oops.Oh(HrWfErrorCodeEnum.InstNotFound);
        var tasks = await _taskRep.AsQueryable()
            .Where(u => u.InstId == instId)
            .OrderBy(u => u.Id)
            .ToListAsync();

        var myPending = tasks.FirstOrDefault(u =>
            u.AssigneeId == _userManager.UserId && u.Status == HrWfConsts.TaskPending);

        var canEdit = false;
        try
        {
            if (inst.Status == HrWfConsts.InstRunning)
            {
                EnsureCanEdit(inst, myPending);
                canEdit = true;
            }
        }
        catch
        {
            canEdit = false;
        }

        return new HrWfInstDetailOutput
        {
            InstId = inst.Id,
            BizType = inst.BizType,
            BizId = inst.BizId,
            Status = inst.Status,
            Title = inst.Title,
            ApplicantUserId = inst.ApplicantUserId,
            SubjectCcCode = inst.SubjectCcCode,
            CurrentNodeKey = inst.CurrentNodeKey,
            StartedTime = inst.StartedTime,
            FinishedTime = inst.FinishedTime,
            AllowEditWhileRunning = inst.AllowEditWhileRunning,
            EditByApplicant = inst.EditByApplicant,
            EditByAssignee = inst.EditByAssignee,
            FormSchema = JsonSerializer.Deserialize<HrWfSchemaSnapshot>(inst.FormSchemaSnapshot, JsonOpts),
            FormData = NormalizeFormDict(
                JsonSerializer.Deserialize<Dictionary<string, object?>>(inst.FormDataJson, JsonOpts)),
            DefNodes = JsonSerializer.Deserialize<HrWfNodesSnapshot>(inst.DefNodesSnapshot, JsonOpts),
            Tasks = tasks.Select(u => new HrWfTaskItemOutput
            {
                TaskId = u.Id,
                NodeKey = u.NodeKey,
                AssigneeId = u.AssigneeId,
                Status = u.Status,
                Opinion = u.Opinion,
                FinishedTime = u.FinishedTime,
                FromEscalate = u.FromEscalate
            }).ToList(),
            CanApprove = myPending != null,
            CanReject = myPending != null,
            CanEditForm = canEdit,
            MyPendingTaskId = myPending?.Id
        };
    }

    private async Task DispatchFromAsync(HrWfInst inst, HrWfNodesSnapshot nodesSnap, int startIndex, long actorUserId)
    {
        var nodes = nodesSnap.Nodes;
        var mode = string.IsNullOrWhiteSpace(nodesSnap.AssigneeResolveMode)
            ? HrWfConsts.ModeLegacyLevel
            : nodesSnap.AssigneeResolveMode!;
        var formData = JsonSerializer.Deserialize<Dictionary<string, object?>>(inst.FormDataJson, JsonOpts)
                       ?? new Dictionary<string, object?>();
        var pathRoles = nodesSnap.ReportPathRoleIds ?? [];
        Dictionary<long, HrWfRoleNode>? pathById = null;
        var hadCandidate = false;

        for (var i = startIndex; i < nodes.Count; i++)
        {
            var node = nodes[i];
            if (!node.Enabled) continue;
            hadCandidate = true;

            if (!HrWfConditionEval.IsMatch(node.ConditionJson, formData))
            {
                await AddLogAsync(inst.Id, null, HrWfConsts.ActionSkipCondition, actorUserId, node.NodeKey, null, null, null);
                continue;
            }

            var (assignees, skipAction) = await ResolveAssigneesAsync(node, inst.SubjectCcCode, mode, pathRoles, async () =>
            {
                pathById ??= await LoadPathRoleNodesAsync(pathRoles);
                return pathById;
            });

            if (assignees.Count == 0)
            {
                await AddLogAsync(inst.Id, null, skipAction ?? HrWfConsts.ActionSkipEmpty, actorUserId, node.NodeKey, null, null, null);
                continue;
            }

            inst.CurrentNodeKey = node.NodeKey;
            inst.Status = HrWfConsts.InstRunning;
            await _instRep.UpdateAsync(inst);

            var tasks = assignees.Distinct().Select(uid => new HrWfTask
            {
                InstId = inst.Id,
                NodeKey = node.NodeKey,
                AssigneeId = uid,
                Status = HrWfConsts.TaskPending,
                FromEscalate = false
            }).ToList();
            await _taskRep.InsertRangeAsync(tasks);
            return;
        }

        // 无剩余启用节点 → 正常通过；有候选但全跳过 → R7b 失败
        if (HrWfDispatchGate.IsAllSkippedFailure(hadCandidate))
        {
            inst.Status = HrWfConsts.InstCancelled;
            inst.CurrentNodeKey = null;
            inst.FinishedTime = DateTime.Now;
            await _instRep.UpdateAsync(inst);
            await AddLogAsync(inst.Id, null, "NoAssignee", actorUserId, null, null, null,
                """{"reason":"Wf.NoAssigneeOnPath"}""");
            throw Oops.Oh(HrWfErrorCodeEnum.NoAssigneeOnPath);
        }

        inst.Status = HrWfConsts.InstApproved;
        inst.CurrentNodeKey = null;
        inst.FinishedTime = DateTime.Now;
        await _instRep.UpdateAsync(inst);
        await InvokeHandlerAsync(inst, h => h.OnApprovedAsync(ToCallback(inst)));
    }

    /// <summary>
    /// 开单：当事人岗位 → 起点角色 → 祖先路径（不含本人）。无岗位/无角色/已是根 → 开单失败。
    /// </summary>
    private async Task<List<long>> BuildReportPathForStartAsync(string? subjectEmpId)
    {
        if (string.IsNullOrWhiteSpace(subjectEmpId))
            throw Oops.Oh(HrWfErrorCodeEnum.SubjectEmpRequired);

        var postId = await _resolver.GetEmployeePostIdAsync(subjectEmpId);
        if (string.IsNullOrWhiteSpace(postId))
            throw Oops.Oh(HrWfErrorCodeEnum.SubjectPostMissing);

        var roles = await _resolver.ListRolesByPostAsync(postId);
        if (roles.Count == 0)
            throw Oops.Oh(HrWfErrorCodeEnum.SubjectPostNoRole);

        var allRoles = await _roleRep.AsQueryable()
            .Select(u => new { u.Id, u.ParentId, u.LevelId })
            .ToListAsync();
        var byId = allRoles.ToDictionary(
            u => u.Id,
            u => new HrWfRoleNode(u.Id, u.ParentId, u.LevelId, 0));

        // 补 Depth
        byId = byId.ToDictionary(
            kv => kv.Key,
            kv => kv.Value with { Depth = HrWfReportPath.ComputeDepth(kv.Key, byId) });

        try
        {
            var matched = roles.Select(r => byId[r.Id]).ToList();
            var start = HrWfReportPath.SelectStartRole(matched);
            if (start.ParentId is not > 0)
                throw Oops.Oh(HrWfErrorCodeEnum.SubjectIsRoot);

            var path = HrWfReportPath.BuildAncestorPath(start.Id, byId);
            if (path.Count == 0)
                throw Oops.Oh(HrWfErrorCodeEnum.SubjectIsRoot);

            var levelIds = path.Select(id => byId[id].LevelId).Where(id => id is > 0).Select(id => id!.Value);
            HrWfReportPath.AssertUniqueLevelsOnPath(levelIds);
            return path;
        }
        catch (InvalidOperationException ex)
        {
            throw HrWfErrors.Oh(ex.Message);
        }
    }

    private async Task<Dictionary<long, HrWfRoleNode>> LoadPathRoleNodesAsync(List<long> pathRoleIds)
    {
        if (pathRoleIds.Count == 0) return new Dictionary<long, HrWfRoleNode>();
        var rows = await _roleRep.AsQueryable()
            .Where(u => pathRoleIds.Contains(u.Id))
            .Select(u => new { u.Id, u.ParentId, u.LevelId })
            .ToListAsync();
        return rows.ToDictionary(u => u.Id, u => new HrWfRoleNode(u.Id, u.ParentId, u.LevelId, 0));
    }

    private async Task<(List<long> users, string? skipAction)> ResolveAssigneesAsync(
        HrWfDefNodeItem node,
        string subjectCcCode,
        string mode,
        List<long> pathRoleIds,
        Func<Task<Dictionary<long, HrWfRoleNode>>> loadPathById)
    {
        var rule = JsonSerializer.Deserialize<HrWfAssigneeRule>(node.AssigneeRuleJson ?? "{}", JsonOpts)
            ?? new HrWfAssigneeRule { Type = HrWfConsts.RuleLevel };

        if (string.Equals(rule.Type, HrWfConsts.RuleFixedUser, StringComparison.OrdinalIgnoreCase))
        {
            var ids = (rule.UserIds ?? []).Where(u => u > 0).Distinct().ToList();
            return (ids, ids.Count == 0 ? HrWfConsts.ActionSkipEmpty : null);
        }

        if (string.Equals(rule.Type, HrWfConsts.RulePost, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(rule.PostId))
                return ([], HrWfConsts.ActionSkipEmpty);
            var users = await _resolver.ResolveUsersForPostAsync(rule.PostId);
            return (users, users.Count == 0 ? HrWfConsts.ActionSkipEmpty : null);
        }

        if (string.Equals(rule.Type, HrWfConsts.RuleLevel, StringComparison.OrdinalIgnoreCase) && rule.LevelId is > 0)
        {
            if (string.Equals(mode, HrWfConsts.ModeReportPath, StringComparison.OrdinalIgnoreCase))
            {
                var byId = await loadPathById();
                var roleId = HrWfReportPath.FindRoleIdByLevel(pathRoleIds, byId, rule.LevelId.Value);
                if (roleId == null)
                    return ([], HrWfConsts.ActionSkipNotOnPath);

                var users = await _resolver.ResolveUsersForRoleAsync(roleId.Value, subjectCcCode);
                // 有角色但管辖不覆盖 / 无人 → SkipEmpty（或 SkipScope 语义并入空人）
                return (users, users.Count == 0 ? HrWfConsts.ActionSkipEmpty : null);
            }

            var legacy = await _resolver.ResolveByLevelAsync(rule.LevelId.Value, subjectCcCode, escalateIfEmpty: true);
            return (legacy, legacy.Count == 0 ? HrWfConsts.ActionSkipEmpty : null);
        }

        // DeptManager / Reporter：未实现 → 跳过
        return ([], HrWfConsts.ActionSkipEmpty);
    }

    private async Task<(HrWfTask task, HrWfInst inst, HrWfNodesSnapshot nodesSnap)> LoadTaskContextAsync(long taskId)
    {
        var task = await _taskRep.GetFirstAsync(u => u.Id == taskId) ?? throw Oops.Oh(HrWfErrorCodeEnum.TaskNotPending);
        if (task.Status != HrWfConsts.TaskPending) throw Oops.Oh(HrWfErrorCodeEnum.TaskNotPending);
        if (task.AssigneeId != _userManager.UserId) throw Oops.Oh(HrWfErrorCodeEnum.NotAssignee);

        var inst = await _instRep.GetFirstAsync(u => u.Id == task.InstId) ?? throw Oops.Oh(HrWfErrorCodeEnum.InstNotFound);
        if (inst.Status != HrWfConsts.InstRunning) throw Oops.Oh(HrWfErrorCodeEnum.TaskNotPending);

        var nodesSnap = JsonSerializer.Deserialize<HrWfNodesSnapshot>(inst.DefNodesSnapshot, JsonOpts)
            ?? throw Oops.Oh(HrWfErrorCodeEnum.DefNotFound);
        return (task, inst, nodesSnap);
    }

    private async Task CancelSiblingPendingAsync(long instId, string nodeKey, long exceptTaskId)
    {
        var now = DateTime.Now;
        await _taskRep.AsUpdateable()
            .SetColumns(u => u.Status == HrWfConsts.TaskCancelled)
            .SetColumns(u => u.FinishedTime == now)
            .Where(u => u.InstId == instId && u.NodeKey == nodeKey && u.Status == HrWfConsts.TaskPending && u.Id != exceptTaskId)
            .ExecuteCommandAsync();
    }

    private Task TryApplyPatchAsync(HrWfInst inst, HrWfTask task, Dictionary<string, object?>? patch)
    {
        if (patch == null || patch.Count == 0) return Task.CompletedTask;
        EnsureCanEdit(inst, task);
        MergeFormData(inst, patch);
        return _instRep.UpdateAsync(inst);
    }

    private void EnsureCanEdit(HrWfInst inst, HrWfTask? myPending)
    {
        if (!inst.AllowEditWhileRunning) throw Oops.Oh(HrWfErrorCodeEnum.FormEditDenied);
        var uid = _userManager.UserId;
        var ok = (inst.EditByApplicant && uid == inst.ApplicantUserId)
                 || (inst.EditByAssignee && myPending != null && myPending.AssigneeId == uid);
        if (!ok) throw Oops.Oh(HrWfErrorCodeEnum.FormEditDenied);
    }

    private static void MergeFormData(HrWfInst inst, Dictionary<string, object?> patch)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(inst.FormDataJson, JsonOpts)
                   ?? new Dictionary<string, object?>();
        // 存量值可能是 JsonElement，先规范；再合并补丁
        dict = NormalizeFormDict(dict);
        foreach (var kv in NormalizeFormDict(patch))
            dict[kv.Key] = kv.Value;
        inst.FormDataJson = JsonSerializer.Serialize(dict, JsonOpts);
    }

    private static void ValidateRequired(List<HrWfFormField> fields, Dictionary<string, object?> data)
    {
        foreach (var f in fields.Where(u => u.Required))
        {
            if (!data.TryGetValue(f.FieldKey, out var val) || val == null || (val is string s && string.IsNullOrWhiteSpace(s)))
                throw Oops.Oh(HrWfErrorCodeEnum.ValidationFailed);
        }
    }

    /// <summary>
    /// 统一表单字典：把 STJ JsonElement / Newtonsoft JToken 打成 CLR 标量，避免 API 输出 {"ValueKind":3}。
    /// </summary>
    private static Dictionary<string, object?> NormalizeFormDict(Dictionary<string, object?>? src)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (src == null || src.Count == 0) return result;
        foreach (var kv in src)
            result[kv.Key] = NormalizeFormValue(kv.Value);
        return result;
    }

    private static object? NormalizeFormValue(object? value) => value switch
    {
        null => null,
        JsonElement je => JsonElementToObject(je),
        Newtonsoft.Json.Linq.JValue jv => jv.Value,
        Newtonsoft.Json.Linq.JToken jt => jt.ToObject<object?>(),
        string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal => value,
        DateTime or DateTimeOffset or Guid => value,
        _ => value
    };

    private static object? JsonElementToObject(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Undefined => null,
        JsonValueKind.Array or JsonValueKind.Object => el.GetRawText(),
        _ => el.GetRawText()
    };

    private static HrWfFormFieldItem ToFieldItem(HrWfFormField u) => new()
    {
        Id = u.Id,
        FieldKey = u.FieldKey,
        Label = u.Label,
        ControlType = u.ControlType,
        FieldSource = u.FieldSource,
        PoolFieldKey = u.PoolFieldKey,
        Required = u.Required,
        Readonly = u.Readonly,
        SortNo = u.SortNo,
        GroupCode = u.GroupCode,
        SpanSlots = u.SpanSlots,
        CodeField = u.CodeField,
        LookupSource = u.LookupSource,
        DefaultValue = u.DefaultValue,
        Placeholder = u.Placeholder,
        ExtraJson = u.ExtraJson
    };

    private static HrWfDefNodeItem ToNodeItem(HrWfDefNode u) => new()
    {
        Id = u.Id,
        NodeKey = u.NodeKey,
        Name = u.Name,
        OrderNo = u.OrderNo,
        NodeMode = u.NodeMode,
        AssigneeRuleJson = u.AssigneeRuleJson,
        ConditionJson = u.ConditionJson,
        TimeoutHours = u.TimeoutHours,
        Enabled = u.Enabled
    };

    private async Task AddLogAsync(long instId, long? taskId, string actionType, long? actorUserId,
        string? nodeKey, string? opinion, long? fromLevelId, string? detailJson)
    {
        await _logRep.InsertAsync(new HrWfActionLog
        {
            InstId = instId,
            TaskId = taskId,
            ActionType = actionType,
            ActorUserId = actorUserId,
            NodeKey = nodeKey,
            FromLevelId = fromLevelId,
            Opinion = opinion?.Trim(),
            DetailJson = detailJson,
            ActionTime = DateTime.Now
        });
    }

    private static HrWfCallback ToCallback(HrWfInst inst) => new()
    {
        InstId = inst.Id,
        BizType = inst.BizType,
        BizId = inst.BizId,
        FormDataJson = inst.FormDataJson,
        ApplicantUserId = inst.ApplicantUserId,
        SubjectCcCode = inst.SubjectCcCode
    };

    private async Task InvokeHandlerAsync(HrWfInst inst, Func<IHrWfBizHandler, Task> action)
    {
        var handler = _handlers.FirstOrDefault(u =>
            string.Equals(u.BizType, inst.BizType, StringComparison.OrdinalIgnoreCase));
        if (handler == null) return;
        await action(handler);
    }
}
