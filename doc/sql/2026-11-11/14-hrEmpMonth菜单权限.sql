/*
 * 变更说明：人事月报菜单与按钮权限（挂人事管理）
 * 日期：2026-11-11
 * 影响库：主库 SysMenu / SysRoleMenu
 * 是否幂等：是
 * 注意：Id 使用 0520 段，勿占用 0500（业务界面配置）
 * 若曾执行过错误版 14（占 0500），请再执行 15-恢复报表管理菜单并修正人事月报Id.sql
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @hrPid BIGINT = 1300000000201;          -- 人事管理
DECLARE @menuId BIGINT = 1300000000520;         -- 人事月报
DECLARE @detailId BIGINT = 1300000000532;       -- 人事月报明细（隐藏）
DECLARE @roleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @hrPid)
BEGIN
    RAISERROR(N'未找到菜单「人事管理」(Id=1300000000201)', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@menuId, @hrPid, 2, N'hrEmpMonth', N'/hr/empMonth', N'/hrms/empMonth/index', N'人事月报', N'ele-Calendar', 0, 0, 1, 0, 250, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @hrPid, Type = 2, Name = N'hrEmpMonth', Title = N'人事月报', Path = N'/hr/empMonth',
        Component = N'/hrms/empMonth/index', Icon = N'ele-Calendar', IsHide = 0, IsDelete = 0, Status = 1, OrderNo = 250
    WHERE Id = @menuId;

/* 明细必须与列表同挂人事管理（同级隐藏页）。挂在列表下会变成子路由，列表无 router-view → 页签切换明细空白 */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @detailId)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@detailId, @hrPid, 2, N'hrEmpMonthDetail', N'/hr/empMonth/detail', N'/hrms/empMonth/detail', N'人事月报明细', N'ele-Document', 0, 1, 1, 0, 251, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @hrPid, Path = N'/hr/empMonth/detail', Component = N'/hrms/empMonth/detail',
        Title = N'人事月报明细', Name = N'hrEmpMonthDetail', IsHide = 1, IsDelete = 0, Status = 1 WHERE Id = @detailId AND Name = N'hrEmpMonthDetail';

;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000521, N'hrEmpMonthPage', N'查询', N'hrEmpMonth/page', 100),
        (1300000000522, N'hrEmpMonthGet', N'详情', N'hrEmpMonth/get', 105),
        (1300000000523, N'hrEmpMonthCreate', N'开批', N'hrEmpMonth/create', 110),
        (1300000000524, N'hrEmpMonthDelete', N'删除', N'hrEmpMonth/delete', 120),
        (1300000000525, N'hrEmpMonthGenerate', N'生成', N'hrEmpMonth/generate', 130),
        (1300000000526, N'hrEmpMonthAdjust', N'调整', N'hrEmpMonth/adjustLine', 140),
        (1300000000527, N'hrEmpMonthFreeze', N'冻结', N'hrEmpMonth/freeze', 150),
        (1300000000528, N'hrEmpMonthUnfreeze', N'解冻', N'hrEmpMonth/unfreeze', 160),
        (1300000000529, N'hrEmpMonthExport', N'导出', N'hrEmpMonth/export', 170),
        (1300000000530, N'hrEmpMonthValidate', N'校验', N'hrEmpMonth/validate', 180),
        (1300000000531, N'hrEmpMonthPageLines', N'行查询', N'hrEmpMonth/pageLines', 190)
    ) AS v(Id, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET Pid = @menuId, Type = 3, Name = s.Name, Title = s.Title, Permission = s.Permission,
    OrderNo = s.OrderNo, IsDelete = 0, Status = 1, IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0
WHEN NOT MATCHED THEN INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, @menuId, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now);

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000005200, 1300000000520),
        (1300000005201, 1300000000521),
        (1300000005202, 1300000000522),
        (1300000005203, 1300000000523),
        (1300000005204, 1300000000524),
        (1300000005205, 1300000000525),
        (1300000005206, 1300000000526),
        (1300000005207, 1300000000527),
        (1300000005208, 1300000000528),
        (1300000005209, 1300000000529),
        (1300000005210, 1300000000530),
        (1300000005211, 1300000000531),
        (1300000005212, 1300000000532)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);
GO
