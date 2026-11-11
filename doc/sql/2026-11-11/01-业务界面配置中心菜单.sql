/*
 * 业务界面配置中心菜单（挂在「报表管理」下）
 * 日期：2026-11-11  幂等：是
 * 注意：勿占用 0241/0244/0245
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @reportManagePid BIGINT = 1300000000207; -- 报表管理
DECLARE @menuId BIGINT = 1300000000500;
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @grantId BIGINT = 1300000002500;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
BEGIN
    INSERT INTO dbo.SysMenu (
        Id, Pid, Type, Name, Path, Component, Title, Icon,
        IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime
    )
    VALUES (
        @menuId, @reportManagePid, 2, N'hrUiConfig', N'/reportManage/uiConfig', N'/hrms/uiConfig/index',
        N'业务界面配置', N'ele-SetUp',
        0, 0, 1, 0, 100, 1, 0, @now
    );
END
ELSE
BEGIN
    UPDATE dbo.SysMenu
    SET Pid = @reportManagePid, Type = 2, Name = N'hrUiConfig', Path = N'/reportManage/uiConfig',
        Component = N'/hrms/uiConfig/index', Title = N'业务界面配置', Icon = N'ele-SetUp',
        IsHide = 0, Status = 1, IsDelete = 0, OrderNo = 100, UpdateTime = @now
    WHERE Id = @menuId;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = @roleId AND MenuId = @menuId)
   AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE Id = @grantId)
BEGIN
    INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId) VALUES (@grantId, @roleId, @menuId);
END

SELECT Id, Pid, Type, Name, Title, Path, Component, IsDelete
FROM dbo.SysMenu WHERE Id = @menuId;
