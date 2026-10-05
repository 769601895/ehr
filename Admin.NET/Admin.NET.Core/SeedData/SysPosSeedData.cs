// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统职位表种子数据
/// </summary>
[IgnoreUpdateSeed]
public class SysPosSeedData : ISqlSugarEntitySeedData<SysPos>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysPos> HasData()
    {
        var defaultTenantId = SqlSugarConst.DefaultTenantId;
        return
        [
            new SysPos{ Id=defaultTenantId, Name="党委书记", Code="dwsj", Type="管理", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="党委书记", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 10, Name="局长", Code="jz", Type="管理", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="局长", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 20, Name="副局长", Code="fjz", Type="管理", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="副局长", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 30, Name="科长", Code="kz", Type = "操作", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="科长", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 40, Name="副科长", Code="fkz", Type = "操作", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="副科长", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 50, Name="职员", Code="zy", Type = "操作", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="职员", TenantId=defaultTenantId },
            new SysPos{ Id=defaultTenantId + 60, Name="其他", Code="qt", Type = "其他", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="其他", TenantId=defaultTenantId },
        ];
    }
}