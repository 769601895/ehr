/*
 * 变更说明：在“人事管理”菜单（Id=1300000000201）下新增“组织架构管理”子菜单及按钮权限
 *            组件指向 Web/src/views/hrms/org/index.vue，对应后端 hrOrg 接口
 * 模块：hrOrg
 * 日期：2026-10-10
 * 影响库：ehr（主库 SysMenu）
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 说明：
 *   1. 替代已废弃的 20261009_hrOrg_seed_orgMenu.sql；若曾执行删除脚本，可再执行本脚本恢复。
 *   2. MenuTypeEnum：Dir=1、Menu=2、Button=3。
 *   3. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。
 *   4. 执行后需重新登录前端，侧栏方可显示；角色需授权本菜单及按钮。
 */

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)
BEGIN
    RAISERROR('父菜单 1300000000201（人事管理）不存在，请先初始化顶级目录。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000211)
    BEGIN
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000211, 1300000000201, 2, 'hrOrg', '/hr/org', '/hrms/org/index',
             '组织架构管理', 'ele-Share',
             0, 0, 1, 0, 100, 1, 0, GETDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000217)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000217, 1300000000211, 3, 'hrOrgList', '查询', 'hrOrg/list',
             0, 0, 1, 0, 80, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000218)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000218, 1300000000211, 3, 'hrOrgPage', '分页查询', 'hrOrg/page',
             0, 0, 1, 0, 90, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000212)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000212, 1300000000211, 3, 'hrOrgAdd', '新增', 'hrOrg/add',
             0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000213)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000213, 1300000000211, 3, 'hrOrgUpdate', '编辑', 'hrOrg/update',
             0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000214)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000214, 1300000000211, 3, 'hrOrgDelete', '停用', 'hrOrg/delete',
             0, 0, 1, 0, 120, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000215)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000215, 1300000000211, 3, 'hrOrgMerge', '合并', 'hrOrg/mergeDept',
             0, 0, 1, 0, 130, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000216)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Title], [Permission],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000216, 1300000000211, 3, 'hrOrgTransfer', '调动', 'hrOrg/transferDept',
             0, 0, 1, 0, 140, 1, 0, GETDATE());
END

SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Permission], [OrderNo], [Status]
FROM [SysMenu]
WHERE [Id] = 1300000000211 OR [Pid] = 1300000000211
ORDER BY [OrderNo], [Id];
