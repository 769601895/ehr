/*
 * 变更说明：将「离职管理」改为目录，下挂三个空子菜单（与旧系统截图一致）
 *   01 离职原因设置 / 02 员工离职处理 / 03 员工离职恢复
 * 日期：2026-11-09
 * 影响库：主库（SysMenu / SysRoleMenu）
 * 是否幂等：是
 * 父菜单：1300000000410（离职管理，OrderNo=125）
 * 子菜单：1300000000411~0413；角色菜单：1300000004111~4113
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @hrPid BIGINT = 1300000000201; -- 人事管理
DECLARE @dirId BIGINT = 1300000000410; -- 离职管理
DECLARE @roleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @hrPid)
BEGIN
    RAISERROR(N'父菜单 1300000000201（人事管理）不存在', 16, 1);
    RETURN;
END

-- 离职管理：菜单 → 目录
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @dirId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@dirId, @hrPid, 1, N'hrEmpLeave', N'/hr/empLeave', N'Layout',
         N'离职管理', N'ele-SwitchButton',
         0, 0, 1, 0, 125, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @hrPid, Type = 1, Name = N'hrEmpLeave',
        Path = N'/hr/empLeave', Component = N'Layout',
        Title = N'离职管理', Icon = N'ele-SwitchButton', Permission = NULL,
        IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0,
        OrderNo = 125, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = @dirId;

-- 01 离职原因设置
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000411)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (1300000000411, @dirId, 2, N'hrLeaveReason', N'/hr/empLeave/reason', N'/hrms/empLeave/reason/index',
         N'离职原因设置', N'ele-DocumentChecked',
         0, 0, 1, 0, 100, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @dirId, Type = 2, Name = N'hrLeaveReason',
        Path = N'/hr/empLeave/reason', Component = N'/hrms/empLeave/reason/index',
        Title = N'离职原因设置', Icon = N'ele-DocumentChecked', Permission = NULL,
        OrderNo = 100, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = 1300000000411;

-- 02 员工离职处理
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000412)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (1300000000412, @dirId, 2, N'hrEmpLeaveProcess', N'/hr/empLeave/process', N'/hrms/empLeave/process/index',
         N'员工离职处理', N'ele-Folder',
         0, 0, 1, 0, 110, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @dirId, Type = 2, Name = N'hrEmpLeaveProcess',
        Path = N'/hr/empLeave/process', Component = N'/hrms/empLeave/process/index',
        Title = N'员工离职处理', Icon = N'ele-Folder', Permission = NULL,
        OrderNo = 110, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = 1300000000412;

-- 03 员工离职恢复
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000413)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (1300000000413, @dirId, 2, N'hrEmpLeaveRestore', N'/hr/empLeave/restore', N'/hrms/empLeave/restore/index',
         N'员工离职恢复', N'ele-FolderOpened',
         0, 0, 1, 0, 120, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @dirId, Type = 2, Name = N'hrEmpLeaveRestore',
        Path = N'/hr/empLeave/restore', Component = N'/hrms/empLeave/restore/index',
        Title = N'员工离职恢复', Icon = N'ele-FolderOpened', Permission = NULL,
        OrderNo = 120, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = 1300000000413;

-- 超管授权（目录 + 三个子菜单）
;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000004110, 1300000000410),
        (1300000004111, 1300000000411),
        (1300000004112, 1300000000412),
        (1300000004113, 1300000000413)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

SELECT Id, Pid, Type, Title, Name, Path, Component, OrderNo
FROM dbo.SysMenu
WHERE Id = @dirId OR Pid = @dirId
ORDER BY Pid, OrderNo, Id;
