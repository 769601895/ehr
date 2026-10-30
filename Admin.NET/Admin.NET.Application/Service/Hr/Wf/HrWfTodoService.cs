namespace Admin.NET.Application;

/// <summary>
/// 我的待办 / 已办 / 我发起的
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfTodo", Order = 485, Description = "审批待办")]
public class HrWfTodoService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfTask> _taskRep;
    private readonly SqlSugarRepository<HrWfInst> _instRep;
    private readonly UserManager _userManager;

    public HrWfTodoService(
        SqlSugarRepository<HrWfTask> taskRep,
        SqlSugarRepository<HrWfInst> instRep,
        UserManager userManager)
    {
        _taskRep = taskRep;
        _instRep = instRep;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "PageMyTodo"), HttpPost]
    [DisplayName("我的待办")]
    public async Task<SqlSugarPagedList<HrWfTodoOutput>> PageMyTodo(HrWfTodoPageInput input)
    {
        input ??= new HrWfTodoPageInput();
        var uid = _userManager.UserId;
        var keyword = input.Keyword?.Trim();

        var query = _taskRep.AsQueryable()
            .InnerJoin<HrWfInst>((t, i) => t.InstId == i.Id)
            .Where((t, i) => t.AssigneeId == uid && t.Status == HrWfConsts.TaskPending)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), (t, i) => i.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), (t, i) =>
                (i.Title != null && i.Title.Contains(keyword!)) || i.BizId.Contains(keyword!))
            .Select((t, i) => new HrWfTodoOutput
            {
                InstId = i.Id,
                TaskId = t.Id,
                BizType = i.BizType,
                BizId = i.BizId,
                Title = i.Title,
                Status = i.Status,
                CurrentNodeKey = i.CurrentNodeKey,
                NodeKey = t.NodeKey,
                StartedTime = i.StartedTime,
                FinishedTime = i.FinishedTime
            });

        NormalizeSort(input);
        return await query
            .MergeTable()
            .OrderBuilder(input, defaultSortField: nameof(HrWfTodoOutput.StartedTime), descSort: true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "PageMyDone"), HttpPost]
    [DisplayName("我的已办")]
    public async Task<SqlSugarPagedList<HrWfTodoOutput>> PageMyDone(HrWfTodoPageInput input)
    {
        input ??= new HrWfTodoPageInput();
        var uid = _userManager.UserId;
        var keyword = input.Keyword?.Trim();

        var query = _taskRep.AsQueryable()
            .InnerJoin<HrWfInst>((t, i) => t.InstId == i.Id)
            .Where((t, i) => t.AssigneeId == uid &&
                             (t.Status == HrWfConsts.TaskApproved || t.Status == HrWfConsts.TaskRejected))
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), (t, i) => i.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), (t, i) =>
                (i.Title != null && i.Title.Contains(keyword!)) || i.BizId.Contains(keyword!))
            .Select((t, i) => new HrWfTodoOutput
            {
                InstId = i.Id,
                TaskId = t.Id,
                BizType = i.BizType,
                BizId = i.BizId,
                Title = i.Title,
                Status = t.Status,
                CurrentNodeKey = i.CurrentNodeKey,
                NodeKey = t.NodeKey,
                StartedTime = i.StartedTime,
                FinishedTime = t.FinishedTime
            });

        NormalizeSort(input);
        return await query
            .MergeTable()
            .OrderBuilder(input, defaultSortField: nameof(HrWfTodoOutput.FinishedTime), descSort: true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "PageMyApplied"), HttpPost]
    [DisplayName("我发起的")]
    public async Task<SqlSugarPagedList<HrWfTodoOutput>> PageMyApplied(HrWfTodoPageInput input)
    {
        input ??= new HrWfTodoPageInput();
        var uid = _userManager.UserId;
        var keyword = input.Keyword?.Trim();

        NormalizeSort(input);
        if (string.IsNullOrWhiteSpace(input.Field))
            input.Field = nameof(HrWfInst.StartedTime);
        // 先在实体上排序（SugarColumn→started_time），再投影 DTO，避免 OrderBuilder 用 StartedTime 打到库表
        MapAppliedSortField(input);

        var query = _instRep.AsQueryable()
            .Where(i => i.ApplicantUserId == uid)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), i => i.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), i =>
                (i.Title != null && i.Title.Contains(keyword!)) || i.BizId.Contains(keyword!))
            .OrderBuilder(input, defaultSortField: nameof(HrWfInst.StartedTime), descSort: true)
            // 勿写 TaskId = null：SqlSugar 会译成字符串常量，触发 TaskId can't convert string to int64
            .Select(i => new HrWfTodoOutput
            {
                InstId = i.Id,
                BizType = i.BizType,
                BizId = i.BizId,
                Title = i.Title,
                Status = i.Status,
                CurrentNodeKey = i.CurrentNodeKey,
                NodeKey = i.CurrentNodeKey,
                StartedTime = i.StartedTime,
                FinishedTime = i.FinishedTime
            });

        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }

    /// <summary>前端 DTO 字段 → 实例实体可排序字段</summary>
    private static void MapAppliedSortField(BasePageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Field)) return;
        input.Field = input.Field switch
        {
            "InstId" or "Id" => nameof(HrWfInst.Id),
            "BizType" => nameof(HrWfInst.BizType),
            "BizId" => nameof(HrWfInst.BizId),
            "Title" => nameof(HrWfInst.Title),
            "Status" => nameof(HrWfInst.Status),
            "CurrentNodeKey" or "NodeKey" => nameof(HrWfInst.CurrentNodeKey),
            "StartedTime" => nameof(HrWfInst.StartedTime),
            "FinishedTime" => nameof(HrWfInst.FinishedTime),
            _ => nameof(HrWfInst.StartedTime)
        };
    }
}
