/*

 * 变更说明：新增顶级目录「系统代码设置」（与合同管理平级），后续挂子模块

 * 模块：sysCode

 * 日期：2026-10-12

 * 影响库：ehr（主库 SysMenu）

 * 是否幂等：是（IF NOT EXISTS）

 *

 * 说明：

 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。

 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。

 *   3. 本脚本仅建空目录；子菜单另脚本追加。

 *   4. 执行后需重新登录前端，侧栏方可显示；角色需授权本菜单。

 */



IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000206)

BEGIN

    INSERT INTO [SysMenu]

        ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],

         [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

    VALUES

        (1300000000206, 0, 1, 'sysCode', '/sysCode', 'Layout',

         '系统代码设置', 'ele-Collection',

         0, 0, 1, 0, 600, 1, 0, GETDATE());

END;



SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [OrderNo], [Status]

FROM [SysMenu]

WHERE [Id] = 1300000000206;


