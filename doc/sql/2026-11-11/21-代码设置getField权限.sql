/*
 * 变更说明：代码设置补充 getField 按钮权限
 * 日期：2026-11-11
 * 前置：20-代码设置菜单迁移.sql
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @menuId BIGINT = 1300000000270;
DECLARE @id BIGINT = 1300000000557;
DECLARE @name NVARCHAR(64) = N'hrCodeMstrGetField';

IF EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @id AND Name <> @name)
BEGIN
    RAISERROR(N'SysMenu Id 1300000000557 已被其它模块占用。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @id)
BEGIN
    INSERT INTO dbo.SysMenu (
        Id, Pid, Type, Name, Title, Permission, Path, Component,
        IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime
    )
    VALUES (
        @id, @menuId, 3, @name, N'代码类详情', N'hrCodeMstr/getField', NULL, NULL,
        0, 0, 1, 0, 220, 1, 0, @now
    );
END
ELSE
BEGIN
    UPDATE dbo.SysMenu
    SET Pid = @menuId, Type = 3, Title = N'代码类详情', Permission = N'hrCodeMstr/getField',
        IsDelete = 0, Status = 1, OrderNo = 220, UpdateTime = @now
    WHERE Id = @id AND Name = @name;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = @id)
BEGIN
    INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
    VALUES (1300000001767, 1300000000101, @id);
END

SELECT Id, Pid, Name, Title, Permission FROM dbo.SysMenu WHERE Id = @id;
GO
