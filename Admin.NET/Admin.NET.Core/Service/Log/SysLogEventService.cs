// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统事件日志服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 365, Description = "事件日志")]
public class SysLogEventService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogEvent> _sysLogEventRep;

    public SysLogEventService(SqlSugarRepository<SysLogEvent> sysLogEventRep)
    {
        _sysLogEventRep = sysLogEventRep;
    }

    /// <summary>
    /// 获取事件日志分页列表 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取事件日志分页列表")]
    public async Task<SqlSugarPagedList<SysLogEvent>> Page(PageEventLogInput input)
    {
        return await _sysLogEventRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.RunId), u => u.RunId.Contains(input.RunId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.EventId), u => u.EventId.Contains(input.EventId))
            .WhereIF(input.Status != null, u => u.Status == input.Status)
            .IgnoreColumns(u => new { u.Payload, u.Exception, u.RetryRecord, u.Properties })
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取事件日志详情 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取事件日志详情")]
    public async Task<SysLogEvent> GetDetail(long id)
    {
        return await _sysLogEventRep.GetByIdAsync(id);
    }

    /// <summary>
    /// 清空事件日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("清空事件日志")]
    public void Clear()
    {
        _sysLogEventRep.AsSugarClient().DbMaintenance.TruncateTable<SysLogEvent>();
    }

    /// <summary>
    /// 导出事件日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Export"), NonUnify]
    [DisplayName("导出事件日志")]
    public async Task<IActionResult> ExportLogEvent(LogInput input)
    {
        var logEventList = await _sysLogEventRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                    u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .Select<ExportLogDto>().ToListAsync();
        if (logEventList == null || logEventList.Count < 1)
            throw Oops.Oh("日志数据为空，导出已取消");

        var res = await new ExcelExporter().ExportAsByteArray(logEventList);
        return new FileStreamResult(new MemoryStream(res), "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmm") + "事件日志.xlsx" };
    }

    /// <summary>
    /// 按年按天数统计消息日志 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("按年按天数统计消息日志")]
    public async Task<List<StatLogOutput>> GetYearDayStats()
    {
        var _db = _sysLogEventRep.AsSugarClient();

        // SqlServer 数据库，全局设置 IsWithNoLockQuery = true 时 Reportable 查询会报错
        // 通过禁用当前上下文的全局设置 Nolock，解决 Reportable 查询报错的问题
        _db.CurrentConnectionConfig.MoreSettings.IsWithNoLockQuery = false;

        var now = DateTime.Now;
        var days = (now - now.AddYears(-1)).Days + 1;
        var day365 = Enumerable.Range(0, days).Select(u => now.AddDays(-u)).ToList();
        var queryableLeft = _db.Reportable(day365).ToQueryable<DateTime>();

        var queryableRight = _db.Queryable<SysLogEvent>(); //.SplitTable(tab => tab);
        var list = await _db.Queryable(queryableLeft, queryableRight, JoinType.Left,
            (x1, x2) => x1.ColumnName.Date == x2.CreateTime.Date)
            .GroupBy((x1, x2) => x1.ColumnName)
            .Select((x1, x2) => new StatLogOutput
            {
                Count = SqlFunc.AggregateSum(SqlFunc.IIF(x2.Id > 0, 1, 0)),
                Date = x1.ColumnName.ToString("yyyy-MM-dd")
            })
            .MergeTable()
            .OrderBy(x => x.Date)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 按状态统计事件数量 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("按状态统计事件数量")]
    public async Task<List<object>> GetStatusStats()
    {
        var _db = _sysLogEventRep.AsSugarClient();
        _db.CurrentConnectionConfig.MoreSettings.IsWithNoLockQuery = false;

        var list = await _db.Queryable<SysLogEvent>()
            .GroupBy(u => u.Status)
            .Select(u => new
            {
                Status = u.Status,
                Count = SqlFunc.AggregateCount(u.Id)
            })
            .ToListAsync();

        return list.Cast<object>().ToList();
    }
}