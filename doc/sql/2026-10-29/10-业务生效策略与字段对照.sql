-- 业务生效策略 + 表单字段对照（审核通过 ≠ 即时写主档）
-- 异动：异动日当日（day_offset=0）；离职：离职日次日（day_offset=1）
-- 库：ehrnew（HR）

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRAN;

IF OBJECT_ID(N'dbo.hr_wf_effect_policy', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_effect_policy
    (
        Id               bigint         NOT NULL,
        biz_type         nvarchar(50)   NOT NULL,
        enabled          bit            NOT NULL CONSTRAINT DF_hr_wf_effect_policy_enabled DEFAULT (1),
        base_date_source nvarchar(20)   NOT NULL CONSTRAINT DF_hr_wf_effect_policy_src DEFAULT (N'Form'),
        base_date_field  nvarchar(64)   NOT NULL,
        day_offset       int            NOT NULL CONSTRAINT DF_hr_wf_effect_policy_offset DEFAULT (0),
        apply_mode       nvarchar(20)   NOT NULL CONSTRAINT DF_hr_wf_effect_policy_mode DEFAULT (N'ImmediateIfDue'),
        target_entity    nvarchar(50)   NOT NULL CONSTRAINT DF_hr_wf_effect_policy_entity DEFAULT (N'HrEmployee'),
        remark           nvarchar(200)  NULL,
        CreateTime       datetime       NULL,
        UpdateTime       datetime       NULL,
        CreateUserId     bigint         NULL,
        CreateUserName   nvarchar(64)   NULL,
        UpdateUserId     bigint         NULL,
        UpdateUserName   nvarchar(64)   NULL,
        IsDelete         bit            NOT NULL CONSTRAINT DF_hr_wf_effect_policy_del DEFAULT (0),
        DeleteTime       datetime       NULL,
        CONSTRAINT PK_hr_wf_effect_policy PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_effect_policy_biz
        ON dbo.hr_wf_effect_policy (biz_type)
        WHERE IsDelete = 0 AND enabled = 1;
END;

IF OBJECT_ID(N'dbo.hr_wf_effect_field_map', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_effect_field_map
    (
        Id             bigint         NOT NULL,
        biz_type       nvarchar(50)   NOT NULL,
        form_field_key nvarchar(64)   NOT NULL,
        role           nvarchar(20)   NOT NULL,
        target_entity  nvarchar(50)   NOT NULL CONSTRAINT DF_hr_wf_effect_fmap_entity DEFAULT (N'HrEmployee'),
        db_column      nvarchar(64)   NULL,
        pair_group     nvarchar(64)   NULL,
        enabled        bit            NOT NULL CONSTRAINT DF_hr_wf_effect_fmap_enabled DEFAULT (1),
        sort_no        int            NOT NULL CONSTRAINT DF_hr_wf_effect_fmap_sort DEFAULT (100),
        remark         nvarchar(200)  NULL,
        CreateTime     datetime       NULL,
        UpdateTime     datetime       NULL,
        CreateUserId   bigint         NULL,
        CreateUserName nvarchar(64)   NULL,
        UpdateUserId   bigint         NULL,
        UpdateUserName nvarchar(64)   NULL,
        IsDelete       bit            NOT NULL CONSTRAINT DF_hr_wf_effect_fmap_del DEFAULT (0),
        DeleteTime     datetime       NULL,
        CONSTRAINT PK_hr_wf_effect_field_map PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_effect_fmap_biz_key
        ON dbo.hr_wf_effect_field_map (biz_type, form_field_key)
        WHERE IsDelete = 0;
    CREATE INDEX IX_hr_wf_effect_fmap_biz
        ON dbo.hr_wf_effect_field_map (biz_type, enabled, sort_no);
END;

IF OBJECT_ID(N'dbo.hr_wf_effect_pending', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_effect_pending
    (
        Id               bigint          NOT NULL,
        biz_type         nvarchar(50)    NOT NULL,
        biz_id           nvarchar(64)    NOT NULL,
        emp_id           nvarchar(50)    NOT NULL,
        plan_effect_date date            NOT NULL,
        status           nvarchar(20)    NOT NULL CONSTRAINT DF_hr_wf_effect_pending_st DEFAULT (N'Pending'),
        payload_json     nvarchar(max)   NULL,
        applied_time     datetime        NULL,
        remark           nvarchar(200)   NULL,
        CreateTime       datetime        NULL,
        UpdateTime       datetime        NULL,
        CreateUserId     bigint          NULL,
        CreateUserName   nvarchar(64)    NULL,
        UpdateUserId     bigint          NULL,
        UpdateUserName   nvarchar(64)    NULL,
        IsDelete         bit             NOT NULL CONSTRAINT DF_hr_wf_effect_pending_del DEFAULT (0),
        DeleteTime       datetime        NULL,
        CONSTRAINT PK_hr_wf_effect_pending PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_effect_pending_biz
        ON dbo.hr_wf_effect_pending (biz_type, biz_id)
        WHERE IsDelete = 0;
    CREATE INDEX IX_hr_wf_effect_pending_due
        ON dbo.hr_wf_effect_pending (status, plan_effect_date)
        WHERE IsDelete = 0;
END;

DECLARE @now datetime = GETDATE();
DECLARE @uid bigint = 1300000000101;
DECLARE @uname nvarchar(64) = N'超级管理员';

-- 策略：异动当日 / 离职次日
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_effect_policy WHERE biz_type = N'hrEmh' AND IsDelete = 0)
INSERT INTO dbo.hr_wf_effect_policy
    (Id, biz_type, enabled, base_date_source, base_date_field, day_offset, apply_mode, target_entity, remark,
     CreateTime, CreateUserId, CreateUserName, IsDelete)
VALUES
    (864500000000001, N'hrEmh', 1, N'Form', N'emh_date', 0, N'ImmediateIfDue', N'HrEmployee',
     N'员工异动：异动日当日生效；表单需有 emh_date', @now, @uid, @uname, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_effect_policy WHERE biz_type = N'hrEmpLeave' AND IsDelete = 0)
INSERT INTO dbo.hr_wf_effect_policy
    (Id, biz_type, enabled, base_date_source, base_date_field, day_offset, apply_mode, target_entity, remark,
     CreateTime, CreateUserId, CreateUserName, IsDelete)
VALUES
    (864500000000002, N'hrEmpLeave', 1, N'Form', N'leave_date', 1, N'ImmediateIfDue', N'HrEmployee',
     N'员工离职：离职日次日生效', @now, @uid, @uname, 0);

-- hrEmh 字段对照（与当前异动审批表单对齐）
;WITH map(id, form_field_key, role, db_column, pair_group, sort_no, remark) AS
(
    SELECT 864500010000001, N'emp_id',   N'Subject',        NULL,       NULL,   10, N'当事人工号'
    UNION ALL SELECT 864500010000002, N'emh_date', N'EffectBaseDate', NULL, NULL, 20, N'异动日期（表单需配置该字段）'
    UNION ALL SELECT 864500010000003, N'emp_name', N'Context',        NULL, NULL, 30, N'姓名展示'
    UNION ALL SELECT 864500010000004, N'emh_occ',  N'Before',  N'cc_code',  N'dept', 40, N'原部门'
    UNION ALL SELECT 864500010000005, N'emh_ncc',  N'After',   N'cc_code',  N'dept', 50, N'新部门→写回'
    UNION ALL SELECT 864500010000006, N'opost_id', N'Before',  N'post_id',  N'post', 60, N'原岗位'
    UNION ALL SELECT 864500010000007, N'npost_id', N'After',   N'post_id',  N'post', 70, N'新岗位→写回'
)
INSERT INTO dbo.hr_wf_effect_field_map
    (Id, biz_type, form_field_key, role, target_entity, db_column, pair_group, enabled, sort_no, remark,
     CreateTime, CreateUserId, CreateUserName, IsDelete)
SELECT m.id, N'hrEmh', m.form_field_key, m.role, N'HrEmployee', m.db_column, m.pair_group, 1, m.sort_no, m.remark,
       @now, @uid, @uname, 0
FROM map m
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_wf_effect_field_map x
    WHERE x.biz_type = N'hrEmh' AND x.form_field_key = m.form_field_key AND x.IsDelete = 0
);

COMMIT;

SELECT N'hr_wf_effect_policy' AS tbl, biz_type, base_date_field, day_offset, apply_mode, enabled
FROM dbo.hr_wf_effect_policy WHERE IsDelete = 0 ORDER BY biz_type;

SELECT N'hr_wf_effect_field_map' AS tbl, form_field_key, role, db_column, pair_group, sort_no
FROM dbo.hr_wf_effect_field_map WHERE biz_type = N'hrEmh' AND IsDelete = 0 ORDER BY sort_no;
