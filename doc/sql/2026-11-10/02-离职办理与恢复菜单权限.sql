/*
 * 变更说明：员工离职处理 / 员工离职恢复 按钮权限 + 超管授权
 * 日期：2026-11-10
 * 前置：02-离职管理子模块空菜单.sql（0412/0413）
 * 是否幂等：是
 */
SET NOCOUNT ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @processId BIGINT = 1300000000412;
DECLARE @restoreId BIGINT = 1300000000413;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @processId)
BEGIN
    RAISERROR(N'菜单 离职处理 1300000000412 不存在', 16, 1);
    RETURN;
END
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @restoreId)
BEGIN
    RAISERROR(N'菜单 离职恢复 1300000000413 不存在', 16, 1);
    RETURN;
END

-- 确保组件路径
UPDATE dbo.SysMenu SET Component = N'/hrms/empLeave/process/index', Path = N'/hr/empLeave/process', IsDelete = 0, Status = 1 WHERE Id = @processId;
UPDATE dbo.SysMenu SET Component = N'/hrms/empLeave/restore/index', Path = N'/hr/empLeave/restore', IsDelete = 0, Status = 1 WHERE Id = @restoreId;

;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000420, @processId, N'hrEmpLeavePage', N'查询', N'hrEmpLeave/page', 100),
        (1300000000421, @processId, N'hrEmpLeaveDetail', N'详情', N'hrEmpLeave/detail', 105),
        (1300000000422, @processId, N'hrEmpLeaveSaveDraft', N'保存草稿', N'hrEmpLeave/saveDraft', 110),
        (1300000000423, @processId, N'hrEmpLeaveDeleteDraft', N'删除草稿', N'hrEmpLeave/deleteDraft', 120),
        (1300000000424, @processId, N'hrEmpLeaveSubmit', N'提交', N'hrEmpLeave/submit', 130),
        (1300000000425, @processId, N'hrEmpLeaveApprove', N'通过', N'hrEmpLeave/approve', 140),
        (1300000000426, @processId, N'hrEmpLeaveReject', N'驳回', N'hrEmpLeave/reject', 150),
        (1300000000427, @processId, N'hrEmpLeaveVoid', N'撤销', N'hrEmpLeave/void', 160),
        (1300000000430, @restoreId, N'hrEmpLeaveRestorePage', N'查询', N'hrEmpLeaveRestore/page', 100),
        (1300000000431, @restoreId, N'hrEmpLeaveRestoreDetail', N'详情', N'hrEmpLeaveRestore/detail', 105),
        (1300000000432, @restoreId, N'hrEmpLeaveRestoreSaveDraft', N'保存草稿', N'hrEmpLeaveRestore/saveDraft', 110),
        (1300000000433, @restoreId, N'hrEmpLeaveRestoreDeleteDraft', N'删除草稿', N'hrEmpLeaveRestore/deleteDraft', 120),
        (1300000000434, @restoreId, N'hrEmpLeaveRestoreSubmit', N'提交', N'hrEmpLeaveRestore/submit', 130),
        (1300000000435, @restoreId, N'hrEmpLeaveRestoreApprove', N'通过', N'hrEmpLeaveRestore/approve', 140),
        (1300000000436, @restoreId, N'hrEmpLeaveRestoreReject', N'驳回', N'hrEmpLeaveRestore/reject', 150),
        (1300000000437, @restoreId, N'hrEmpLeaveRestoreVoid', N'撤销', N'hrEmpLeaveRestore/void', 160)
    ) AS v(Id, Pid, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, s.Pid, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET Pid = s.Pid, Type = 3, Name = s.Name, Title = s.Title, Permission = s.Permission,
               IsDelete = 0, Status = 1, OrderNo = s.OrderNo, UpdateTime = @now;

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000004120, 1300000000420),(1300000004121, 1300000000421),(1300000004122, 1300000000422),(1300000004123, 1300000000423),
        (1300000004124, 1300000000424),(1300000004125, 1300000000425),(1300000004126, 1300000000426),(1300000004127, 1300000000427),
        (1300000004130, 1300000000430),(1300000004131, 1300000000431),(1300000004132, 1300000000432),(1300000004133, 1300000000433),
        (1300000004134, 1300000000434),(1300000004135, 1300000000435),(1300000004136, 1300000000436),(1300000004137, 1300000000437)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

SELECT Id, Title, Permission FROM dbo.SysMenu WHERE Pid IN (@processId, @restoreId) OR Id IN (@processId, @restoreId) ORDER BY Pid, OrderNo, Id;
