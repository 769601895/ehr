/*
 * 变更说明：修复人事月报误占 Id=1300000000500（业务界面配置）导致「报表管理」子模块消失
 * 1) 恢复「业务界面配置」0500 → 报表管理
 * 2) 恢复「字段元数据」0260 → 报表管理
 * 3) 人事月报改用 0520 段 Id，清理误挂在 0501–0512 的月报按钮/明细
 * 日期：2026-11-11
 * 影响库：主库 SysMenu / SysRoleMenu
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @reportPid BIGINT = 1300000000207;  -- 报表管理
DECLARE @hrPid BIGINT = 1300000000201;      -- 人事管理
DECLARE @uiConfigId BIGINT = 1300000000500; -- 业务界面配置（原占用）
DECLARE @fieldDefId BIGINT = 1300000000260; -- 字段元数据
DECLARE @empMonthId BIGINT = 1300000000520; -- 人事月报（新）
DECLARE @empMonthDetailId BIGINT = 1300000000532;
DECLARE @roleId BIGINT = 1300000000101;

/* ---------- 0. 确保报表管理目录存在且可用 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @reportPid)
BEGIN
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@reportPid, 0, 1, N'reportManage', N'/reportManage', N'Layout', N'报表管理', N'ele-PieChart', 0, 0, 1, 0, 17000, 1, 0, @now);
END
ELSE
    UPDATE dbo.SysMenu
    SET Type = 1, Name = N'reportManage', Path = N'/reportManage', Component = N'Layout',
        Title = N'报表管理', IsHide = 0, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = @reportPid;

/* ---------- 1. 清理误占 0501–0512 的人事月报按钮/明细（归还 0500 给业务界面配置） ---------- */
UPDATE dbo.SysMenu
SET IsDelete = 1, Status = 2, UpdateTime = @now
WHERE Id BETWEEN 1300000000501 AND 1300000000512
  AND (Name LIKE N'hrEmpMonth%' OR Permission LIKE N'hrEmpMonth/%' OR Title LIKE N'人事月报%');

/* 若 0500 当前是人事月报，先软删角色授权里仅指向错误语义的按钮（菜单本体下一步恢复） */
DELETE FROM dbo.SysRoleMenu
WHERE MenuId BETWEEN 1300000000501 AND 1300000000512
  AND MenuId IN (SELECT Id FROM dbo.SysMenu WHERE Name LIKE N'hrEmpMonth%' OR Permission LIKE N'hrEmpMonth/%');

/* ---------- 2. 恢复业务界面配置 0500 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @uiConfigId)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@uiConfigId, @reportPid, 2, N'hrUiConfig', N'/reportManage/uiConfig', N'/hrms/uiConfig/index',
            N'业务界面配置', N'ele-SetUp', 0, 0, 1, 0, 100, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @reportPid, Type = 2, Name = N'hrUiConfig', Path = N'/reportManage/uiConfig',
        Component = N'/hrms/uiConfig/index', Title = N'业务界面配置', Icon = N'ele-SetUp',
        IsHide = 0, Status = 1, IsDelete = 0, OrderNo = 100, UpdateTime = @now
    WHERE Id = @uiConfigId;

IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = @roleId AND MenuId = @uiConfigId)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE Id = 1300000002500)
        INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId) VALUES (1300000002500, @roleId, @uiConfigId);
    ELSE
        INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
        VALUES (1300000002501, @roleId, @uiConfigId); -- 2500 已被占用时兜底
END

/* ---------- 3. 恢复字段元数据 0260 ---------- */
IF EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @fieldDefId)
    UPDATE dbo.SysMenu
    SET Pid = @reportPid, Type = 2, Name = N'hrUiFieldDef', Path = N'/reportManage/fieldDef',
        Component = N'/hrms/fieldDef/index', Title = N'字段元数据', Icon = N'ele-Tickets',
        IsHide = 0, Status = 1, IsDelete = 0, OrderNo = 90, UpdateTime = @now
    WHERE Id = @fieldDefId;

/* ---------- 4. 人事月报改挂 0520 段（人事管理下） ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @hrPid)
BEGIN
    RAISERROR(N'未找到菜单「人事管理」(Id=1300000000201)', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @empMonthId)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@empMonthId, @hrPid, 2, N'hrEmpMonth', N'/hr/empMonth', N'/hrms/empMonth/index', N'人事月报', N'ele-Calendar', 0, 0, 1, 0, 250, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @hrPid, Type = 2, Name = N'hrEmpMonth', Path = N'/hr/empMonth', Component = N'/hrms/empMonth/index',
        Title = N'人事月报', Icon = N'ele-Calendar', IsHide = 0, Status = 1, IsDelete = 0, OrderNo = 250, UpdateTime = @now
    WHERE Id = @empMonthId;

/* 明细与列表同挂人事管理，避免列表子路由无 router-view 导致页签空白 */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @empMonthDetailId)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Path, Component, Title, Icon, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (@empMonthDetailId, @hrPid, 2, N'hrEmpMonthDetail', N'/hr/empMonth/detail', N'/hrms/empMonth/detail', N'人事月报明细', N'ele-Document', 0, 1, 1, 0, 251, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @hrPid, Path = N'/hr/empMonth/detail', Component = N'/hrms/empMonth/detail',
        Title = N'人事月报明细', Name = N'hrEmpMonthDetail', IsHide = 1, Status = 1, IsDelete = 0, UpdateTime = @now
    WHERE Id = @empMonthDetailId AND Name = N'hrEmpMonthDetail';

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
WHEN MATCHED THEN UPDATE SET Pid = @empMonthId, Type = 3, Name = s.Name, Title = s.Title, Permission = s.Permission,
    OrderNo = s.OrderNo, IsDelete = 0, Status = 1, IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0, UpdateTime = @now
WHEN NOT MATCHED THEN INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, @empMonthId, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now);

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

/* 旧错误授权：0500 若曾被当成人事月报授权可保留（现已是业务界面配置，仍需授权）；清理 5100 段指向 0501–0512 的残留 */
DELETE rm
FROM dbo.SysRoleMenu rm
WHERE rm.Id BETWEEN 1300000005100 AND 1300000005112
  AND rm.MenuId BETWEEN 1300000000500 AND 1300000000512
  AND EXISTS (
      SELECT 1 FROM dbo.SysMenu m
      WHERE m.Id = rm.MenuId AND (m.IsDelete = 1 OR m.Name LIKE N'hrEmpMonth%')
  );

/* ---------- 校验输出 ---------- */
SELECT Id, Pid, Type, Name, Title, Path, Component, IsHide, IsDelete, Status, OrderNo
FROM dbo.SysMenu
WHERE Id = @reportPid OR Pid = @reportPid
   OR Id IN (@empMonthId, @empMonthDetailId) OR Pid = @empMonthId
ORDER BY CASE WHEN Id = @reportPid THEN 0 WHEN Pid = @reportPid THEN 1 ELSE 2 END, OrderNo, Id;
GO
