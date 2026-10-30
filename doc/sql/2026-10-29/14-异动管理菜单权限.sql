/*
 * 变更说明：人事管理下挂「异动管理」菜单及按钮权限，并授权超管角色
 * 日期：2026-10-29
 * 影响库：主库（SysMenu / SysRoleMenu）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @pid BIGINT = 1300000000201; -- 人事管理
DECLARE @menuId BIGINT = 1300000000350;
DECLARE @roleId BIGINT = 1300000000101;

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
        (@menuId, @pid, 2, N'hrEmpChg', N'/hr/empChg', N'/hrms/empChg/index',
         N'异动管理', N'ele-Switch',
         0, 0, 1, 0, 120, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @pid, Type = 2, Name = N'hrEmpChg',
        Path = N'/hr/empChg', Component = N'/hrms/empChg/index',
        Title = N'异动管理', Icon = N'ele-Switch',
        IsDelete = 0, Status = 1, OrderNo = 120, UpdateTime = @now
    WHERE Id = @menuId;

-- 按钮权限
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000351)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000351, @menuId, 3, N'hrEmpChgPage', N'查询', N'hrEmpChg/page', 0, 0, 1, 0, 100, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000352)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000352, @menuId, 3, N'hrEmpChgDetail', N'详情', N'hrEmpChg/detail', 0, 0, 1, 0, 105, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000353)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000353, @menuId, 3, N'hrEmpChgSaveDraft', N'保存草稿', N'hrEmpChg/saveDraft', 0, 0, 1, 0, 110, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000354)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000354, @menuId, 3, N'hrEmpChgDeleteDraft', N'删除草稿', N'hrEmpChg/deleteDraft', 0, 0, 1, 0, 120, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000355)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000355, @menuId, 3, N'hrEmpChgSubmit', N'提交', N'hrEmpChg/submit', 0, 0, 1, 0, 130, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000356)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000356, @menuId, 3, N'hrEmpChgApprove', N'通过', N'hrEmpChg/approve', 0, 0, 1, 0, 140, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000357)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000357, @menuId, 3, N'hrEmpChgReject', N'驳回', N'hrEmpChg/reject', 0, 0, 1, 0, 150, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000358)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000358, @menuId, 3, N'hrEmpChgVoid', N'撤销', N'hrEmpChg/void', 0, 0, 1, 0, 160, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000359)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000359, @menuId, 3, N'hrEmpChgPreview', N'预览默认', N'hrEmpChg/previewDefaults', 0, 0, 1, 0, 170, 1, 0, @now);

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000003500, 1300000000350),
        (1300000003501, 1300000000351),
        (1300000003502, 1300000000352),
        (1300000003503, 1300000000353),
        (1300000003504, 1300000000354),
        (1300000003505, 1300000000355),
        (1300000003506, 1300000000356),
        (1300000003507, 1300000000357),
        (1300000003508, 1300000000358),
        (1300000003509, 1300000000359)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

SELECT Id, Title, Permission, Type FROM dbo.SysMenu WHERE Id BETWEEN 1300000000350 AND 1300000000359 ORDER BY Id;
