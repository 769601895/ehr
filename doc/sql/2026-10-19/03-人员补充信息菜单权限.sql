/*
 * 人员补充信息按钮权限（挂在人员资料管理菜单下）
 * 前置：已有菜单 1300000000221（人员资料管理）
 * 超管可直接访问；sys_admin 角色同步授权。
 */

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000221)
BEGIN
    RAISERROR(N'父菜单 1300000000221（人员资料管理）不存在。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000231)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000231, 1300000000221, 3, 'hrEmpChildCount', N'补充信息行数', 'hrEmpChild/count',
            0, 0, 1, 0, 130, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000232)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000232, 1300000000221, 3, 'hrEmpChildList', N'补充信息列表', 'hrEmpChild/list',
            0, 0, 1, 0, 131, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000233)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000233, 1300000000221, 3, 'hrEmpChildSave', N'保存补充信息行', 'hrEmpChild/save',
            0, 0, 1, 0, 132, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000234)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000234, 1300000000221, 3, 'hrEmpChildDelete', N'删除补充信息行', 'hrEmpChild/delete',
            0, 0, 1, 0, 133, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000235)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000235, 1300000000221, 3, 'hrEmpChildSort', N'调整补充信息顺序', 'hrEmpChild/sort',
            0, 0, 1, 0, 134, 1, 0, GETDATE());
END
GO

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001890, 1300000000101, 1300000000231
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000231);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001891, 1300000000101, 1300000000232
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000232);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001892, 1300000000101, 1300000000233
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000233);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001893, 1300000000101, 1300000000234
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000234);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001894, 1300000000101, 1300000000235
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000235);
GO
