// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 企业微信成员更新工具类
/// </summary>
public class WorkWxUserUpdateHelper
{
    /// <summary>
    /// 从现有成员信息创建更新对象
    /// </summary>
    public static UpdateUserWorkWxInput CreateFromExisting(UserWorkWxOutput existingUser)
    {
        return new UpdateUserWorkWxInput
        {
            UserId = existingUser.UserId,
            Name = existingUser.Name,
            Alias = existingUser.Alias,
            Mobile = existingUser.Mobile,
            Email = existingUser.Email,
            Department = existingUser.Department,
            Order = existingUser.Order,
            Position = existingUser.Position,
            Gender = existingUser.Gender,
            Telephone = existingUser.Telephone,
            IsLeaderInDept = existingUser.IsLeaderInDept,
            DirectLeader = existingUser.DirectLeader,
            Enable = existingUser.Status == 1 ? 1 : (existingUser.Status == 2 ? 0 : (int?)null),
            ExtAttr = existingUser.ExtAttr,
            ExternalPosition = existingUser.ExternalPosition,
            ExternalProfile = existingUser.ExternalProfile,
            Address = existingUser.Address,
            MainDepartment = existingUser.MainDepartment
        };
    }

    /// <summary>
    /// 创建部门更新信息
    /// </summary>
    public static UpdateUserWorkWxInput CreateDepartmentUpdate(string userId, List<long> departments, List<long> orders = null, List<int> isLeaders = null)
    {
        var input = new UpdateUserWorkWxInput
        {
            UserId = userId,
            Department = departments
        };

        if (orders != null && orders.Count == departments.Count)
        {
            input.Order = orders;
        }

        if (isLeaders != null && isLeaders.Count == departments.Count)
        {
            input.IsLeaderInDept = isLeaders;
        }

        return input;
    }

    /// <summary>
    /// 创建基础信息更新
    /// </summary>
    public static UpdateUserWorkWxInput CreateBasicInfoUpdate(string userId, string name = null, string mobile = null, string email = null, string position = null)
    {
        return new UpdateUserWorkWxInput
        {
            UserId = userId,
            Name = name,
            Mobile = mobile,
            Email = email,
            Position = position
        };
    }

    /// <summary>
    /// 创建状态更新
    /// </summary>
    public static UpdateUserWorkWxInput CreateStatusUpdate(string userId, bool enable)
    {
        return new UpdateUserWorkWxInput
        {
            UserId = userId,
            Enable = enable ? 1 : 0
        };
    }
}