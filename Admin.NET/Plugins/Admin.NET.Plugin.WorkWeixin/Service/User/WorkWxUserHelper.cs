// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信成员工具类
/// </summary>
public class WorkWxUserHelper
{
    /// <summary>
    /// 检查是否包含敏感字段（根据应用类型判断）
    /// </summary>
    public static bool HasSensitiveFields(UserWorkWxOutput user, bool isNewApp)
    {
        if (isNewApp)
        {
            // 新应用无法获取的敏感字段
            return string.IsNullOrEmpty(user.Avatar) &&
                   string.IsNullOrEmpty(user.Mobile) &&
                   string.IsNullOrEmpty(user.Email) &&
                   string.IsNullOrEmpty(user.BizMail) &&
                   string.IsNullOrEmpty(user.QrCode) &&
                   string.IsNullOrEmpty(user.Address) &&
                   (user.Gender == null || user.Gender == 0);
        }
        return true;
    }

    /// <summary>
    /// 获取显示名称（优先显示name，如果没有则显示userid）
    /// </summary>
    public static string GetDisplayName(UserWorkWxOutput user)
    {
        return !string.IsNullOrEmpty(user.Name) ? user.Name : user.UserId;
    }

    /// <summary>
    /// 获取完整的部门信息
    /// </summary>
    public static string GetDepartmentInfo(UserWorkWxOutput user, Dictionary<long, string> departmentMap)
    {
        if (user.Department == null || departmentMap == null)
            return string.Empty;

        var departments = user.Department
            .Select(deptId => departmentMap.TryGetValue(deptId, out var name) ? $"{name}({deptId})" : deptId.ToString())
            .ToList();

        return string.Join(", ", departments);
    }
}