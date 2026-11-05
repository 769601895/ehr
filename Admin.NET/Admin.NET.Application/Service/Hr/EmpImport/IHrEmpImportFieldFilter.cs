namespace Admin.NET.Application;

/// <summary>
/// 导入字段权限过滤预留（二期接入人事字段权限）。本期默认恒等。
/// </summary>
public interface IHrEmpImportFieldFilter
{
    Task<List<HrUiFieldPoolItemOutput>> FilterAsync(long userId, string importMode, List<HrUiFieldPoolItemOutput> poolItems);
}

/// <summary>本期：原样返回</summary>
public class HrEmpImportFieldFilterPassthrough : IHrEmpImportFieldFilter, ITransient
{
    public Task<List<HrUiFieldPoolItemOutput>> FilterAsync(long userId, string importMode, List<HrUiFieldPoolItemOutput> poolItems)
        => Task.FromResult(poolItems ?? []);
}
