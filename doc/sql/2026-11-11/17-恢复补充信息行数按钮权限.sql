/*
 * 恢复「补充信息行数」为人员资料下的按钮权限（Type=3）
 *
 * 现象：Id 1300000000231 被改成 Type=1 + Path=/hr/code + Layout，
 *       左侧出现假菜单「补充信息行数」，点开成空白页签。
 * 正身：hrEmpChild/count —— 人员资料页查询家庭/教育/履历/职称行数，不是独立模块。
 * 人事代码设置真实菜单已在 1300000000270（/hr/codeMstr）。
 */

UPDATE [SysMenu]
SET
    [Pid] = 1300000000221,
    [Type] = 3,
    [Name] = N'hrEmpChildCount',
    [Title] = N'补充信息行数',
    [Permission] = N'hrEmpChild/count',
    [Path] = NULL,
    [Component] = NULL,
    [Icon] = NULL,
    [IsIframe] = 0,
    [IsHide] = 0,
    [IsKeepAlive] = 1,
    [IsAffix] = 0,
    [OrderNo] = 130,
    [Status] = 1,
    [IsDelete] = 0,
    [UpdateTime] = GETDATE()
WHERE [Id] = 1300000000231
  AND (
        [Type] <> 3
     OR [Pid] <> 1300000000221
     OR ISNULL([Path], N'') <> N''
     OR ISNULL([Component], N'') <> N''
     OR [Name] <> N'hrEmpChildCount'
     OR [Permission] <> N'hrEmpChild/count'
  );

-- 若行不存在则补插
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000231)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000221)
        RAISERROR(N'父菜单 1300000000221（人员资料管理）不存在。', 16, 1);
    ELSE
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000231, 1300000000221, 3, N'hrEmpChildCount', N'补充信息行数', N'hrEmpChild/count',
            0, 0, 1, 0, 130, 1, 0, GETDATE());
END
GO

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001890, 1300000000101, 1300000000231
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000231);
GO

SELECT Id, Pid, Type, Name, Title, Path, Component, Permission, IsDelete, Status
FROM SysMenu
WHERE Id = 1300000000231;
GO
