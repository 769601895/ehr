/*
 * 停职表 + 停复职管理菜单；复职菜单迁入该目录
 * 日期：2026-11-10  幂等：是
 */
SET NOCOUNT ON;

-- ========== 表 ==========
IF OBJECT_ID(N'dbo.hr_emp_suspend', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_suspend
    (
        Id               bigint         NOT NULL,
        suspend_no       nvarchar(50)   NOT NULL,
        emp_id           nvarchar(50)   NOT NULL,
        emp_name         nvarchar(100)  NULL,
        suspend_date     date           NOT NULL,
        plan_effect_date date           NOT NULL,
        suspend_reason   nvarchar(200)  NULL,
        cc_code_snap     nvarchar(50)   NULL,
        emp_state_before int            NULL,
        remark           nvarchar(500)  NULL,
        status           nvarchar(20)   NOT NULL,
        submit_time      datetime       NULL,
        submit_user_id   bigint         NULL,
        approve_time     datetime       NULL,
        approve_user_id  bigint         NULL,
        reject_reason    nvarchar(500)  NULL,
        effective_time   datetime       NULL,
        void_time        datetime       NULL,
        void_user_id     bigint         NULL,
        void_reason      nvarchar(500)  NULL,
        CreateTime       datetime       NULL,
        UpdateTime       datetime       NULL,
        CreateUserId     bigint         NULL,
        CreateUserName   nvarchar(64)   NULL,
        UpdateUserId     bigint         NULL,
        UpdateUserName   nvarchar(64)   NULL,
        IsDelete         bit            NOT NULL CONSTRAINT DF_hr_emp_suspend_del DEFAULT (0),
        DeleteTime       datetime       NULL,
        CONSTRAINT PK_hr_emp_suspend PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_suspend_no ON dbo.hr_emp_suspend (suspend_no) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_suspend_emp ON dbo.hr_emp_suspend (emp_id, suspend_date) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_suspend_status ON dbo.hr_emp_suspend (status, plan_effect_date) WHERE IsDelete = 0;
END

-- ========== 菜单 ==========
DECLARE @now DATETIME = GETDATE();
DECLARE @hrPid BIGINT = 1300000000201;
DECLARE @dirId BIGINT = 1300000000440;
DECLARE @suspendId BIGINT = 1300000000441;
DECLARE @restoreId BIGINT = 1300000000413;
DECLARE @roleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @dirId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@dirId, @hrPid, 1, N'hrEmpSuspendDir', N'/hr/empSuspend', N'Layout',
         N'停复职管理', N'ele-Clock',
         0, 0, 1, 0, 126, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Title = N'停复职管理', Pid = @hrPid, Type = 1, IsDelete = 0, Status = 1, OrderNo = 126, UpdateTime = @now
    WHERE Id = @dirId;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @suspendId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@suspendId, @dirId, 2, N'hrEmpSuspendProcess', N'/hr/empSuspend/process', N'/hrms/empSuspend/process/index',
         N'员工停职处理', N'ele-Folder',
         0, 0, 1, 0, 100, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = @dirId, Path = N'/hr/empSuspend/process', Component = N'/hrms/empSuspend/process/index',
        Title = N'员工停职处理', IsDelete = 0, Status = 1, OrderNo = 100, UpdateTime = @now
    WHERE Id = @suspendId;

-- 复职管理迁入停复职目录
UPDATE dbo.SysMenu
SET Pid = @dirId, Title = N'复职管理', OrderNo = 110, IsDelete = 0, Status = 1, UpdateTime = @now
WHERE Id = @restoreId;

-- 按钮权限（停职）
;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000442, @suspendId, N'hrEmpSuspendPage', N'查询', N'hrEmpSuspend/page', 100),
        (1300000000443, @suspendId, N'hrEmpSuspendDetail', N'详情', N'hrEmpSuspend/detail', 105),
        (1300000000444, @suspendId, N'hrEmpSuspendSaveDraft', N'保存草稿', N'hrEmpSuspend/saveDraft', 110),
        (1300000000445, @suspendId, N'hrEmpSuspendDeleteDraft', N'删除草稿', N'hrEmpSuspend/deleteDraft', 120),
        (1300000000446, @suspendId, N'hrEmpSuspendSubmit', N'提交', N'hrEmpSuspend/submit', 130),
        (1300000000447, @suspendId, N'hrEmpSuspendApprove', N'通过', N'hrEmpSuspend/approve', 140),
        (1300000000448, @suspendId, N'hrEmpSuspendReject', N'驳回', N'hrEmpSuspend/reject', 150),
        (1300000000449, @suspendId, N'hrEmpSuspendVoid', N'撤销', N'hrEmpSuspend/void', 160)
    ) AS v(Id, Pid, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, s.Pid, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET Pid = s.Pid, Permission = s.Permission, Title = s.Title, IsDelete = 0, Status = 1, OrderNo = s.OrderNo, UpdateTime = @now;

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000004140, 1300000000440),
        (1300000004141, 1300000000441),
        (1300000004142, 1300000000442),
        (1300000004143, 1300000000443),
        (1300000004144, 1300000000444),
        (1300000004145, 1300000000445),
        (1300000004146, 1300000000446),
        (1300000004147, 1300000000447),
        (1300000004148, 1300000000448),
        (1300000004149, 1300000000449),
        (1300000004113, 1300000000413)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);
