/*
 * 恢复「人事资料定义」目录（Id=1300000000241 曾被高级查询权限误占），
 * 并将高级查询方案权限迁到新 Id 1300000000261~0264。
 * 日期：2026-11-10  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @hrPid BIGINT = 1300000000201;          -- 人事管理（定义目录挂这里，勿挂人员资料页下）
DECLARE @hrEmpId BIGINT = 1300000000221;        -- 人员资料管理（仅挂权限按钮）
DECLARE @reportDefDir BIGINT = 1300000000241;

-- ========== 1. 恢复人事资料定义目录（与人员资料管理同级） ==========
UPDATE dbo.SysMenu
SET Pid = @hrPid,
    Type = 1,
    Name = N'hrReportDef',
    Path = N'/hr/reportDef',
    Component = N'Layout',
    Permission = NULL,
    Title = N'人事资料定义',
    Icon = N'ele-Document',
    IsIframe = 0,
    IsHide = 0,
    IsKeepAlive = 1,
    IsAffix = 0,
    OrderNo = 95,
    Status = 1,
    IsDelete = 0,
    UpdateTime = @now
WHERE Id = @reportDefDir;

-- 列表/表单定义挂回目录
UPDATE dbo.SysMenu
SET Pid = @reportDefDir, Type = 2, IsDelete = 0, Status = 1, UpdateTime = @now
WHERE Id IN (1300000000244, 1300000000245);

-- 原目录上的旧按钮保持软删（历史种子约定）
UPDATE dbo.SysMenu
SET IsDelete = 1, Status = 2, UpdateTime = @now
WHERE Id IN (1300000000242, 1300000000243);

-- 误占的 0240（高级查询列表）改为软删，权限迁到新 Id
UPDATE dbo.SysMenu
SET IsDelete = 1, Status = 2, UpdateTime = @now
WHERE Id = 1300000000240 AND Name = N'hrAdvQuerySchemeList';

-- ========== 2. 高级查询方案权限（新 Id，挂人员资料管理） ==========
;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000261, @hrEmpId, N'hrAdvQuerySchemeList', N'高级查询方案列表', N'hrAdvQueryScheme/list', 200),
        (1300000000262, @hrEmpId, N'hrAdvQuerySchemeSave', N'保存高级查询方案', N'hrAdvQueryScheme/save', 210),
        (1300000000263, @hrEmpId, N'hrAdvQuerySchemeDelete', N'删除高级查询方案', N'hrAdvQueryScheme/delete', 220),
        (1300000000264, @hrEmpId, N'hrAdvQuerySchemeDetail', N'高级查询方案详情', N'hrAdvQueryScheme/detail', 230)
    ) AS v(Id, Pid, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, s.Pid, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET Pid = s.Pid, Type = 3, Permission = s.Permission, Title = s.Title, Name = s.Name,
               IsDelete = 0, Status = 1, OrderNo = s.OrderNo, Path = NULL, Component = NULL, UpdateTime = @now;

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000002261, 1300000000261),
        (1300000002262, 1300000000262),
        (1300000002263, 1300000000263),
        (1300000002264, 1300000000264),
        -- 确保报表定义目录/子菜单仍授权
        (1300000001719, 1300000000241),
        (1300000001722, 1300000000244),
        (1300000001726, 1300000000245)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

-- 校验
SELECT Id, Pid, Type, Name, Title, IsDelete
FROM dbo.SysMenu
WHERE Id IN (1300000000240,1300000000241,1300000000242,1300000000243,1300000000244,1300000000245,
             1300000000261,1300000000262,1300000000263,1300000000264)
ORDER BY Id;
