// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Plugin.WorkWeixin.Const;

namespace Admin.NET.Plugin.WorkWeixin;

/// <summary>
/// 系统配置表种子数据
/// </summary>
[IgnoreUpdateSeed]
public class SysConfigSeedData : ISqlSugarEntitySeedData<SysConfig>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysConfig> HasData()
    {
        return
        [
            new SysConfig{ Id=1330000000001, Name="企业微信CorpId", Code=WorkWeixinConst.WorkWeixinCorpId, Value="", SysFlag=YesNoEnum.Y, Remark= "企业微信接口获取凭证的CorpId", OrderNo=1000, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2025-04-10 00:00:00") },
            new SysConfig{ Id=1330000000002, Name="企业微信CorpSecret", Code=WorkWeixinConst.WorkWeixinCorpSecret, Value="", SysFlag=YesNoEnum.Y, Remark= "企业微信接口获取凭证的CorpSecret", OrderNo=1000, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2025-04-10 00:00:00") },
        ];
    }
}