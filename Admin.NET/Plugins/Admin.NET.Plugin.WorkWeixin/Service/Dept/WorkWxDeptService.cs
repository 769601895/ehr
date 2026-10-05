// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信部门服务 🧩
/// </summary>
public class WorkWxDeptService(WorkWxBaseService baseService) : ITransient
{
    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseIdWorkWxOutput> Create(CreateDeptWorkWxInput input)
    {
        return await baseService.SendAsync<CreateDeptWorkWxInput, BaseIdWorkWxOutput>(input);
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Update(UpdateDeptWorkWxInput input)
    {
        return await baseService.SendAsync<UpdateDeptWorkWxInput, BaseWorkWxOutput>(input);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门id</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Delete(long id)
    {
        return await baseService.SendAsync<DeleteDeptWorkWxInput, BaseWorkWxOutput>(new() { Id = id });
    }

    /// <summary>
    /// 获取子部门ID列表
    /// </summary>
    /// <param name="id">部门id。获取指定部门及其下的子部门（递归）</param>
    /// <returns></returns>
    public async Task<DeptSimpleListWorkWxOutput> Get(long id)
    {
        return await baseService.SendAsync<DeptSimpleListWorkWxInput, DeptSimpleListWorkWxOutput>(new() { Id = id });
    }
}