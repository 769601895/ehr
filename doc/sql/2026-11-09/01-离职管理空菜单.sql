/*
 * 变更说明：人事管理下、异动管理后挂「离职管理」空菜单（占位页），并授权超管
 * 日期：2026-11-09
 * 影响库：主库（SysMenu / SysRoleMenu）
 * 是否幂等：是
 * 菜单位置：Pid=人事管理 1300000000201；OrderNo=125（异动管理 120 之后）
 * 菜单 Id：1300000000410；角色菜单 Id：1300000004110
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @pid BIGINT = 1300000000201; -- 人事管理
DECLARE @menuId BIGINT = 1300000000410;
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @roleMenuId BIGINT = 1300000004110;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @pid)
BEGIN
    RAISERROR(N'父菜单 1300000000201（人事管理）不存在', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@menuId, @pid, 2, N'hrEmpLeave', N'/hr/empLeave', N'/hrms/empLeave/index',
         N'离职管理', N'ele-SwitchButton',
         0, 0, 1, 0, 125, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @pid, Type = 2, Name = N'hrEmpLeave',
        Path = N'/hr/empLeave', Component = N'/hrms/empLeave/index',
        Title = N'离职管理', Icon = N'ele-SwitchButton', Permission = NULL,
        IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0,
        OrderNo = 125, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = @menuId;

IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = @roleId AND MenuId = @menuId)
    AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE Id = @roleMenuId)
    INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
    VALUES (@roleMenuId, @roleId, @menuId);

SELECT Id, Pid, Title, Name, Path, Component, OrderNo
FROM dbo.SysMenu
WHERE Id = @menuId;
