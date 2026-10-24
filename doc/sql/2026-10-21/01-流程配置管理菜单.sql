/*
 * 变更说明：在「系统代码设置」与「系统管理」之间新增顶级空目录「流程配置管理」
 * 模块：moduleFlowConfig
 * 日期：2026-10-21
 * 影响库：ehrnew（主库 SysMenu）
 * 是否幂等：是（IF NOT EXISTS / UPDATE）
 *
 * 说明：
 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。
 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。
 *   3. OrderNo=700，位于「系统代码设置」(600) 之后、「系统管理」(10000) 之前。
 *   4. 本脚本仅建空目录；子菜单另脚本追加。
 *   5. 执行后需重新登录前端；角色需授权本菜单（见同目录角色菜单脚本）。
 */

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000208)
BEGIN
    INSERT INTO [SysMenu]
        ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
         [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES
        (1300000000208, 0, 1, 'moduleFlowConfig', '/moduleFlowConfig', 'Layout',
         N'流程配置管理', 'ele-Connection',
         0, 0, 1, 0, 700, 1, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE [SysMenu] SET [Title] = N'流程配置管理' WHERE [Id] = 1300000000208 AND [Title] <> N'流程配置管理';
END;

SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [OrderNo], [Status]
FROM [SysMenu]
WHERE [Id] = 1300000000208;
