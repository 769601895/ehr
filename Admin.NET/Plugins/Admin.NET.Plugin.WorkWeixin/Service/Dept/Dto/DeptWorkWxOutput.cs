// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 创建部门输出参数
/// </summary>
public class CreateDepartmentWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 创建的部门id
    /// </summary>
    [CustomJsonProperty("id")]
    public long Id { get; set; }
}

/// <summary>
/// 部门基础信息
/// </summary>
public class DeptWorkWxBaseInfo
{
    /// <summary>
    /// 部门id
    /// </summary>
    [CustomJsonProperty("id")]
    public long Id { get; set; }

    /// <summary>
    /// 父部门id
    /// </summary>
    [CustomJsonProperty("parentid")]
    public long ParentId { get; set; }

    /// <summary>
    /// 排序值
    /// </summary>
    [CustomJsonProperty("order")]
    public long Order { get; set; }
}

/// <summary>
/// 获取子部门ID列表输出参数
/// </summary>
public class DeptSimpleListWorkWxOutput : BaseWorkWxOutput
{
    /// <summary>
    /// 部门列表数据
    /// </summary>
    [CustomJsonProperty("department_id")]
    public List<DeptWorkWxBaseInfo> DeptList { get; set; } = new List<DeptWorkWxBaseInfo>();

    /// <summary>
    /// 部门数量
    /// </summary>
    public int Count => DeptList?.Count ?? 0;

    /// <summary>
    /// 获取部门ID列表
    /// </summary>
    public List<long> DeptIds => DeptList?.Select(d => d.Id).ToList() ?? new List<long>();

    /// <summary>
    /// 按父部门分组
    /// </summary>
    public Dictionary<long, List<DeptWorkWxBaseInfo>> GroupByParent()
    {
        var result = new Dictionary<long, List<DeptWorkWxBaseInfo>>();

        if (DeptList == null) return result;

        foreach (var dept in DeptList)
        {
            if (!result.ContainsKey(dept.ParentId))
            {
                result[dept.ParentId] = new List<DeptWorkWxBaseInfo>();
            }
            result[dept.ParentId].Add(dept);
        }

        return result;
    }
}