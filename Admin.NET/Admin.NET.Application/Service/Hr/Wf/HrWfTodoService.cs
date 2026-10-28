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

        var query = _instRep.AsQueryable()
            .Where(i => i.ApplicantUserId == uid)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BizType), i => i.BizType == input.BizType)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), i =>
                (i.Title != null && i.Title.Contains(keyword!)) || i.BizId.Contains(keyword!))
            .Select(i => new HrWfTodoOutput
            {
                InstId = i.Id,
                TaskId = null,
                BizType = i.BizType,
                BizId = i.BizId,
                Title = i.Title,
                Status = i.Status,
                CurrentNodeKey = i.CurrentNodeKey,
                NodeKey = i.CurrentNodeKey,
                StartedTime = i.StartedTime,
                FinishedTime = i.FinishedTime
            });

        NormalizeSort(input);
        return await query
            .OrderBuilder(input, defaultSortField: nameof(HrWfTodoOutput.StartedTime), descSort: true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
