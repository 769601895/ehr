// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text.Json;

namespace Admin.NET.Plugin.DataApproval.Service;

/// <summary>
/// 数据审批服务
/// </summary>
[ApiDescriptionSettings(DataApprovalConst.GroupName, Order = 100, Description = "数据审批")]
public class DataApprovalService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<DataApproval> _DataApprovalRep;

    public DataApprovalService(SqlSugarRepository<DataApproval> DataApprovalRep)
    {
        _DataApprovalRep = DataApprovalRep;
    }

    /// <summary>
    /// 分页查询审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<DataApprovalOutput>> Page(DataApprovalInput input)
    {
        return await _DataApprovalRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.Code.Contains(input.Keyword.Trim()) || u.Name.Contains(input.Keyword.Trim()) || u.Remark.Contains(input.Keyword.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Remark), u => u.Remark.Contains(input.Remark.Trim()))
            .Select<DataApprovalOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddDataApprovalInput input)
    {
        var entity = input.Adapt<DataApproval>();
        if (input.Code == null)
        {
            entity.Code = await LastCode("");
        }
        await _DataApprovalRep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// 更新审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateDataApprovalInput input)
    {
        var entity = input.Adapt<DataApproval>();
        await _DataApprovalRep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteDataApprovalInput input)
    {
        var entity = await _DataApprovalRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _DataApprovalRep.FakeDeleteAsync(entity);  // 假删除
    }

    /// <summary>
    /// 获取审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DataApproval> GetDetail([FromQuery] QueryByIdDataApprovalInput input)
    {
        return await _DataApprovalRep.GetByIdAsync(input.Id);
    }

    /// <summary>
    /// 根据编码获取审批流信息
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<DataApproval> GetInfo([FromQuery] string code)
    {
        return await _DataApprovalRep.GetFirstAsync(u => u.Code == code);
    }

    /// <summary>
    /// 获取审批流列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<DataApprovalOutput>> GetList([FromQuery] DataApprovalInput input)
    {
        return await _DataApprovalRep.AsQueryable().Select<DataApprovalOutput>().ToListAsync();
    }

    /// <summary>
    /// 获取今天创建的最大编号
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private async Task<string> LastCode(string prefix)
    {
        var today = DateTime.Now.Date;
        var count = await _DataApprovalRep.AsQueryable().Where(u => u.CreateTime >= today).CountAsync();
        return prefix + DateTime.Now.ToString("yyMMdd") + string.Format("{0:d2}", count + 1);
    }

    [HttpGet]
    [ApiDescriptionSettings(Name = "FlowList")]
    [DisplayName("获取审批流结构")]
    public async Task<dynamic> FlowList([FromQuery] string code)
    {
        var result = await _DataApprovalRep.AsQueryable().Where(u => u.Code == code).Select<DataApprovalOutput>().FirstAsync();
        var FlowJson = result.FlowJson != null ? JsonSerializer.Deserialize<DataApprovalItem>(result.FlowJson) : new DataApprovalItem();
        var FormJson = result.FormJson != null ? JsonSerializer.Deserialize<ApprovalFormItem>(result.FormJson) : new ApprovalFormItem();
        return new
        {
            FlowJson,
            FormJson
        };
    }

    [HttpGet]
    [ApiDescriptionSettings(Name = "FormRoutes")]
    [DisplayName("获取审批流规则")]
    public async Task<List<string>> FormRoutes()
    {
        var results = await _DataApprovalRep.AsQueryable().Select<DataApprovalOutput>().ToListAsync();
        var list = new List<string>();
        foreach (var item in results)
        {
            var FormJson = item.FormJson != null ? JsonSerializer.Deserialize<ApprovalFormItem>(item.FormJson) : new ApprovalFormItem();
            if (item.FormJson != null) list.Add(FormJson.Route);
        }
        return list;
    }
}