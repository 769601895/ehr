/*
 * 变更说明：代码设置·明细 listDeleted / restore 按钮权限
 * 日期：2026-11-11
 * 前置：20-代码设置菜单迁移.sql；22-code_mstr软删列.sql
 * Id：0558~0559（本模块续号）；角色菜单 Id：1768~1769
 * 是否幂等：是（仅本模块 Name）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @menuId BIGINT = 1300000000270;
DECLARE @now DATETIME = GETDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId AND Name = N'hrCodeMstr')
BEGIN
    RAISERROR(N'菜单 1300000000270 不是 hrCodeMstr，禁止写入。', 16, 1);
    RETURN;
END

DECLARE @btns TABLE (
    Id BIGINT, Name NVARCHAR(64), Title NVARCHAR(64), Permission NVARCHAR(128), OrderNo INT
);
INSERT INTO @btns (Id, Name, Title, Permission, OrderNo) VALUES
(1300000000558, N'hrCodeMstrListDeleted', N'已删除码值', N'hrCodeMstr/listDeleted', 230),
(1300000000559, N'hrCodeMstrRestore', N'恢复码值', N'hrCodeMstr/restore', 240);

DECLARE @id BIGINT, @name NVARCHAR(64), @title NVARCHAR(64), @perm NVARCHAR(128), @ord INT;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT Id, Name, Title, Permission, OrderNo FROM @btns;
OPEN c;
FETCH NEXT FROM c INTO @id, @name, @title, @perm, @ord;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @id AND Name <> @name)
    BEGIN
        RAISERROR(N'SysMenu Id 已被其它模块占用，请换号段。', 16, 1);
        CLOSE c; DEALLOCATE c;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @id)
    BEGIN
        INSERT INTO dbo.SysMenu (
            Id, Pid, Type, Name, Title, Permission, Path, Component,
            IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime
        )
        VALUES (
            @id, @menuId, 3, @name, @title, @perm, NULL, NULL,
            0, 0, 1, 0, @ord, 1, 0, @now
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.SysMenu
        SET Pid = @menuId, Type = 3, Title = @title, Permission = @perm,
            IsDelete = 0, Status = 1, OrderNo = @ord, UpdateTime = @now
        WHERE Id = @id AND Name = @name;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = @id)
    BEGIN
        INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
        VALUES (1300000001760 + (@id - 1300000000550), 1300000000101, @id);
    END

    FETCH NEXT FROM c INTO @id, @name, @title, @perm, @ord;
END
CLOSE c;
DEALLOCATE c;
GO
