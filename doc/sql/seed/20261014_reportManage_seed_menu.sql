/*
 * 变更说明：在「关于项目」之后新增顶级空目录「报表管理」（后续挂子模块）
 * 模块：reportManage
 * 日期：2026-10-14
 * 影响库：ehr（主库 SysMenu）
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 说明：
 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。
 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。
 *   3. OrderNo=17000，位于「关于项目」(16000) 之后。
 *   4. Path=/reportManage，与系统「报表开发」(/report) 区分。
 *   5. 本脚本仅建空目录；子菜单另脚本追加。
 *   6. 执行后需重新登录前端；角色需授权本菜单（见同目录 roleMenu 脚本）。
 *   7. 空目录在未挂子菜单前，侧栏可能不显示，属正常行为。
 */

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000207)
BEGIN
    INSERT INTO [SysMenu]
        ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
         [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES
        (1300000000207, 0, 1, 'reportManage', '/reportManage', 'Layout',
         N'报表管理', 'ele-PieChart',
         0, 0, 1, 0, 17000, 1, 0, GETDATE());
END;

SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [OrderNo], [Status]
FROM [SysMenu]
WHERE [Id] = 1300000000207;
