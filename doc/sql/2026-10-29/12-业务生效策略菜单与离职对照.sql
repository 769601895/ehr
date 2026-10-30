-- 业务生效策略菜单（流程配置管理下）+ 离职字段对照种子
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @uid BIGINT = 1300000000101;
DECLARE @uname NVARCHAR(64) = N'超级管理员';
DECLARE @pid BIGINT = 1300000000208; -- 流程配置管理
DECLARE @menuId BIGINT = 1300000000305;
DECLARE @roleId BIGINT = 1300000000101;

-- 1) 菜单
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@menuId, @pid, 2, N'hrWfEffectPolicy', N'/moduleFlowConfig/wfEffectPolicy', N'/hrms/wf/effectPolicy/index',
         N'业务生效策略', N'ele-Timer',
         0, 0, 1, 0, 45, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @pid, Type = 2, Name = N'hrWfEffectPolicy',
        Path = N'/moduleFlowConfig/wfEffectPolicy', Component = N'/hrms/wf/effectPolicy/index',
        Title = N'业务生效策略', Icon = N'ele-Timer',
        IsDelete = 0, Status = 1, OrderNo = 45, UpdateTime = @now
    WHERE Id = @menuId;

-- 按钮权限（避开已占用的 0306/0310/0311…）
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000307)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000307, @menuId, 3, N'hrWfEffectPolicySave', N'保存策略', N'hrWfEffectPolicy/savePolicy', 0, 0, 1, 0, 110, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000308)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000308, @menuId, 3, N'hrWfEffectFieldMapSave', N'保存对照', N'hrWfEffectPolicy/saveFieldMap', 0, 0, 1, 0, 120, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000309)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000309, @menuId, 3, N'hrWfEffectApplyDue', N'扫描生效', N'hrWfEffectPolicy/applyDue', 0, 0, 1, 0, 130, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000330)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000330, @menuId, 3, N'hrWfEffectPolicyList', N'查询', N'hrWfEffectPolicy/listPolicies', 0, 0, 1, 0, 100, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000331)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000331, @menuId, 3, N'hrWfEffectPolicyMaps', N'对照查询', N'hrWfEffectPolicy/listFieldMaps', 0, 0, 1, 0, 105, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000332)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000332, @menuId, 3, N'hrWfEffectVoidBtn', N'撤销', N'hrWfEffectPolicy/void', 0, 0, 1, 0, 140, 1, 0, @now);

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000003400, 1300000000305),
        (1300000003401, 1300000000307),
        (1300000003402, 1300000000308),
        (1300000003403, 1300000000309),
        (1300000003404, 1300000000330),
        (1300000003405, 1300000000331),
        (1300000003406, 1300000000332)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

-- 2) 离职对照：leave_date 基准日；写回 emp_lea_date 由服务用计划生效日；强制 emp_state=3
;WITH map(id, form_field_key, role, db_column, pair_group, sort_no, remark) AS
(
    SELECT 864500020000101, N'emp_id', N'Subject', NULL, NULL, 10, N'当事人工号'
    UNION ALL SELECT 864500020000102, N'leave_date', N'EffectBaseDate', NULL, NULL, 20, N'离职日（策略 day_offset=1 → 次日生效）'
    UNION ALL SELECT 864500020000104, N'emp_name', N'Context', NULL, NULL, 40, N'姓名'
)
INSERT INTO dbo.hr_wf_effect_field_map
    (Id, biz_type, form_field_key, role, target_entity, db_column, pair_group, enabled, sort_no, remark,
     CreateTime, CreateUserId, CreateUserName, IsDelete)
SELECT m.id, N'hrEmpLeave', m.form_field_key, m.role, N'HrEmployee', m.db_column, m.pair_group, 1, m.sort_no, m.remark,
       @now, @uid, @uname, 0
FROM map m
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_wf_effect_field_map x
    WHERE x.biz_type = N'hrEmpLeave' AND x.form_field_key = m.form_field_key AND x.IsDelete = 0
);

-- 保证 hrEmpLeave 策略存在
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_effect_policy WHERE biz_type = N'hrEmpLeave' AND IsDelete = 0)
INSERT INTO dbo.hr_wf_effect_policy
    (Id, biz_type, enabled, base_date_source, base_date_field, day_offset, apply_mode, target_entity, remark,
     CreateTime, CreateUserId, CreateUserName, IsDelete)
VALUES
    (864500000000002, N'hrEmpLeave', 1, N'Form', N'leave_date', 1, N'ImmediateIfDue', N'HrEmployee',
     N'员工离职：离职日次日生效；写 h_emh_hist', @now, @uid, @uname, 0);

PRINT N'业务生效策略菜单与离职对照已同步';
SELECT Id, Title, Path FROM dbo.SysMenu WHERE Id = @menuId OR Pid = @menuId ORDER BY Id;
SELECT form_field_key, role, db_column FROM dbo.hr_wf_effect_field_map WHERE biz_type=N'hrEmpLeave' AND IsDelete=0 ORDER BY sort_no;
