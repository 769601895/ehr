/*
 * 变更说明：在「人事管理」下、人事代码设置与组织架构管理之间新增「人事资料定义」菜单
 * 模块：hrReportDef
 * 日期：2026-10-13
 * 影响库：ehr（主库 SysMenu）
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 说明：
 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。
 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。
 *   3. OrderNo=95，位于人事代码设置(90)与组织架构(100)之间。
 *   4. 组件指向 Web/src/views/hrms/reportDef/index.vue。
 *   5. 执行后需重新登录前端；角色需授权本菜单及按钮。
 */

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)
BEGIN
    RAISERROR('父菜单 1300000000201（人事管理）不存在，请先初始化顶级目录。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000241)
    BEGIN
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000241, 1300000000201, 2, 'hrReportDef', '/hr/reportDef', '/hrms/reportDef/index',
             '人事资料定义', 'ele-Document',
             0, 0, 1, 0, 95, 1, 0, GETDATE());
    END
    ELSE
    BEGIN
        -- 已存在时同步标题（原「人事报表定义」→「人事资料定义」）
        UPDATE [SysMenu] SET [Title] = N'人事资料定义' WHERE [Id] = 1300000000241 AND [Title] <> N'人事资料定义';
    END

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000242)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000242, 1300000000241, 3, 'hrReportDefPage', '查询', 'hrReportDef/page',
             0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000243)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000243, 1300000000241, 3, 'hrReportDefSave', '保存', 'hrReportDef/save',
             0, 0, 1, 0, 110, 1, 0, GETDATE());
END

SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Permission], [OrderNo], [Status]
FROM [SysMenu]
WHERE [Id] = 1300000000241 OR [Pid] = 1300000000241
ORDER BY [OrderNo], [Id];
