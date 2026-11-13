/*
 * 变更说明：人事代码设置 → 系统代码设置 / 代码设置；补充代码类 API 按钮
 * 日期：2026-11-11
 * 前置：系统代码目录 1300000000206；原菜单 1300000000270~0274
 * 是否幂等：是（仅更新本模块 Name=hrCodeMstr）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @sysCode BIGINT = 1300000000206;
DECLARE @menuId BIGINT = 1300000000270;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @sysCode AND IsDelete = 0)
BEGIN
    RAISERROR(N'菜单 1300000000206（系统代码设置）不存在。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId AND Name = N'hrCodeMstr')
BEGIN
    RAISERROR(N'菜单 1300000000270 不是 hrCodeMstr，禁止覆盖。', 16, 1);
    RETURN;
END

UPDATE dbo.SysMenu
SET Pid = @sysCode,
    Title = N'代码设置',
    Path = N'/sysCode/codeMstr',
    Component = N'/hrms/codeMstr/index',
    OrderNo = 50,
    IsDelete = 0,
    Status = 1,
    UpdateTime = @now
WHERE Id = @menuId AND Name = N'hrCodeMstr';

/* 新按钮：代码类列表/保存/删除/恢复/译文 */
DECLARE @btns TABLE (
    Id BIGINT, Name NVARCHAR(64), Title NVARCHAR(64), Permission NVARCHAR(128), OrderNo INT
);
INSERT INTO @btns (Id, Name, Title, Permission, OrderNo) VALUES
(1300000000550, N'hrCodeMstrListField', N'代码类列表', N'hrCodeMstr/listField', 150),
(1300000000551, N'hrCodeMstrSaveField', N'保存代码类', N'hrCodeMstr/saveField', 160),
(1300000000552, N'hrCodeMstrDeleteField', N'删除代码类', N'hrCodeMstr/deleteField', 170),
(1300000000553, N'hrCodeMstrRestoreField', N'恢复代码类', N'hrCodeMstr/restoreField', 180),
(1300000000554, N'hrCodeMstrFieldLangList', N'代码类译文查询', N'hrCodeMstr/fieldLangList', 190),
(1300000000555, N'hrCodeMstrSaveFieldLang', N'代码类译文保存', N'hrCodeMstr/saveFieldLang', 200),
(1300000000556, N'hrCodeMstrListDeletedField', N'已删除代码类', N'hrCodeMstr/listDeletedField', 210),
(1300000000557, N'hrCodeMstrGetField', N'代码类详情', N'hrCodeMstr/getField', 220);

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

    /* sys_admin 授权 */
    IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = @id)
    BEGIN
        INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
        VALUES (1300000001760 + (@id - 1300000000550), 1300000000101, @id);
    END

    FETCH NEXT FROM c INTO @id, @name, @title, @perm, @ord;
END
CLOSE c;
DEALLOCATE c;

/* 确保页面本身仍授权给 sys_admin */
IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = @menuId)
BEGIN
    INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
    VALUES (1300000001745, 1300000000101, @menuId);
END

SELECT Id, Pid, Name, Title, Path, Permission, Type, OrderNo
FROM dbo.SysMenu
WHERE Id = @menuId OR Pid = @menuId
ORDER BY Type, OrderNo, Id;
GO
