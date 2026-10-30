// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application;

/// <summary>
/// 系统菜单表种子数据（执行顺序大于0且Id与框架一致，则代表重写系统种子数据）
/// </summary>
[SeedData(500)]
[IncreSeed]
public class SysMenuSeedData : ISqlSugarEntitySeedData<SysMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysMenu> HasData()
    {
        return
        [
            new SysMenu{ Id=1300000000111, Pid=1300000000101, Title="工作台", Path="/dashboard/home", Name="home", Component="/home/index", IsAffix=true, Icon="ele-HomeFilled", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            // 以下为与"工作台"平级的业务模块目录（人事/考勤/保险公积金/薪酬等）
            new SysMenu{ Id=1300000000201, Pid=0, Title="人事管理", Path="/hr", Name="hr", Component="Layout", Icon="ele-User", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-09 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1300000000202, Pid=0, Title="考勤管理", Path="/attendance", Name="attendance", Component="Layout", Icon="ele-Calendar", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-09 00:00:00"), OrderNo=200 },
            new SysMenu{ Id=1300000000203, Pid=0, Title="保险公积金管理", Path="/insurance", Name="insurance", Component="Layout", Icon="ele-OfficeBuilding", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-09 00:00:00"), OrderNo=300 },
            new SysMenu{ Id=1300000000204, Pid=0, Title="薪酬管理", Path="/salary", Name="salary", Component="Layout", Icon="ele-Money", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-09 00:00:00"), OrderNo=400 },
            // 系统代码设置及其子菜单
            new SysMenu{ Id=1300000000206, Pid=0, Title="系统代码设置", Path="/sysCode", Name="sysCode", Component="Layout", Icon="ele-Collection", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-12 00:00:00"), OrderNo=600 },
            new SysMenu{ Id=1300000000310, Pid=1300000000206, Title="岗位代码设置", Path="/sysCode/hrPost", Name="hrPost", Component="/hrms/post/index", Icon="ele-Briefcase", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-28 00:00:00"), OrderNo=10 },
            new SysMenu{ Id=1300000000320, Pid=1300000000206, Title="查找数据源设置", Path="/sysCode/hrLookupDef", Name="hrLookupDef", Component="/hrms/lookupDef/index", Icon="ele-Connection", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-29 00:00:00"), OrderNo=20 },
            // 流程配置管理：位于「系统代码设置」(600) 与「系统管理」(10000) 之间
            new SysMenu{ Id=1300000000208, Pid=0, Title="流程配置管理", Path="/moduleFlowConfig", Name="moduleFlowConfig", Component="Layout", Icon="ele-Connection", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-21 00:00:00"), OrderNo=700 },
            new SysMenu{ Id=1300000000280, Pid=1300000000208, Title="审核等级", Path="/moduleFlowConfig/wfLevel", Name="hrWfLevel", Component="/hrms/wf/level/index", Icon="ele-Medal", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=10 },
            new SysMenu{ Id=1300000000284, Pid=1300000000208, Title="审批角色", Path="/moduleFlowConfig/wfRole", Name="hrWfRole", Component="/hrms/wf/role/index", Icon="ele-UserFilled", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=20 },
            new SysMenu{ Id=1300000000288, Pid=1300000000208, Title="审批表单", Path="/moduleFlowConfig/wfForm", Name="hrWfForm", Component="/hrms/wf/form/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=30 },
            new SysMenu{ Id=1300000000292, Pid=1300000000208, Title="流程定义", Path="/moduleFlowConfig/wfDef", Name="hrWfDef", Component="/hrms/wf/def/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=40 },
            new SysMenu{ Id=1300000000296, Pid=1300000000208, Title="发起申请", Path="/moduleFlowConfig/wfStart", Name="hrWfStart", Component="/hrms/wf/start/index", Icon="ele-Promotion", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=50 },
            new SysMenu{ Id=1300000000298, Pid=1300000000208, Title="我的待办", Path="/moduleFlowConfig/wfTodo", Name="hrWfTodo", Component="/hrms/wf/todo/index", Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2026-10-24 00:00:00"), OrderNo=60 },
            // 报表管理：位于「关于项目」之后的顶级空目录，后续挂子模块
            new SysMenu{ Id=1300000000207, Pid=0, Title="报表管理", Path="/reportManage", Name="reportManage", Component="Layout", Icon="ele-PieChart", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2026-10-14 00:00:00"), OrderNo=17000 },
        ];
    }
}