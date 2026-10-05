// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信标签服务 🧩
/// </summary>
public class WorkWxTagService(WorkWxBaseService baseService) : ITransient
{
    /// <summary>
    /// 创建标签
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<CreateTagWorkWxOutput> Create(CreateTagWorkWxInput input)
    {
        return await baseService.SendAsync<CreateTagWorkWxInput, CreateTagWorkWxOutput>(input);
    }

    /// <summary>
    /// 更新标签
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Update(UpdateTagWorkWxInput input)
    {
        return await baseService.SendAsync<UpdateTagWorkWxInput, BaseWorkWxOutput>(input);
    }

    /// <summary>
    /// 删除标签
    /// </summary>
    /// <param name="id">标签ID</param>
    /// <returns></returns>
    public async Task<BaseWorkWxOutput> Delete(long id)
    {
        return await baseService.SendAsync<DeleteTagWorkWxInput, BaseWorkWxOutput>(new() { TagId = id });
    }

    /// <summary>
    /// 获取标签成员
    /// </summary>
    /// <param name="id">标签ID</param>
    /// <returns></returns>
    public async Task<TagMembersWorkWxOutput> Get(long id)
    {
        return await baseService.SendAsync<TagMembersWorkWxInput, TagMembersWorkWxOutput>(new() { TagId = id });
    }

    /// <summary>
    /// 增加标签成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<AddTagMembersWorkWxOutput> AddTagMembers(AddTagMembersWorkWxInput input)
    {
        return await baseService.SendAsync<AddTagMembersWorkWxInput, AddTagMembersWorkWxOutput>(input);
    }

    /// <summary>
    /// 删除标签成员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DeleteTagMembersWorkWxOutput> DeleteTagMembers(DeleteTagMembersWorkWxInput input)
    {
        return await baseService.SendAsync<DeleteTagMembersWorkWxInput, DeleteTagMembersWorkWxOutput>(input);
    }

    /// <summary>
    /// 获取标签列表
    /// </summary>
    /// <returns></returns>
    public async Task<TagListWorkWxOutput> GetList()
    {
        return await baseService.SendAsync<TagListWorkWxInput, TagListWorkWxOutput>(new());
    }
}