/*
 * 变更说明：员工异动管理一期 — 建表、可改字段种子、异动类别代码
 * 日期：2026-10-29
 * 影响库：HR 业务库（如 ehrnew）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @uid BIGINT = 1300000000101;
DECLARE @uname NVARCHAR(64) = N'超级管理员';

BEGIN TRAN;

-- ========== 主表 ==========
IF OBJECT_ID(N'dbo.hr_emp_chg', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_chg
    (
        Id               bigint         NOT NULL,
        chg_no           nvarchar(50)   NOT NULL,
        emp_id           nvarchar(50)   NOT NULL,
        emp_name         nvarchar(100)  NULL,
        chg_date         date           NOT NULL,
        chg_type         nvarchar(50)   NOT NULL,
        chg_reason       nvarchar(500)  NULL,
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
        applied_columns  nvarchar(500)  NULL,
        CreateTime       datetime       NULL,
        UpdateTime       datetime       NULL,
        CreateUserId     bigint         NULL,
        CreateUserName   nvarchar(64)   NULL,
        UpdateUserId     bigint         NULL,
        UpdateUserName   nvarchar(64)   NULL,
        IsDelete         bit            NOT NULL CONSTRAINT DF_hr_emp_chg_del DEFAULT (0),
        DeleteTime       datetime       NULL,
        CONSTRAINT PK_hr_emp_chg PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_chg_no ON dbo.hr_emp_chg (chg_no) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_chg_emp_date ON dbo.hr_emp_chg (emp_id, chg_date) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_chg_status ON dbo.hr_emp_chg (status) WHERE IsDelete = 0;
END;

-- ========== 明细 ==========
IF OBJECT_ID(N'dbo.hr_emp_chg_line', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_chg_line
    (
        Id             bigint          NOT NULL,
        chg_id         bigint          NOT NULL,
        field_key      nvarchar(64)    NOT NULL,
        db_column      nvarchar(64)    NOT NULL,
        label          nvarchar(256)   NULL,
        value_before   nvarchar(max)   NULL,
        value_after    nvarchar(max)   NULL,
        sort_no        int             NOT NULL CONSTRAINT DF_hr_emp_chg_line_sort DEFAULT (100),
        CreateTime     datetime        NULL,
        UpdateTime     datetime        NULL,
        CreateUserId   bigint          NULL,
        CreateUserName nvarchar(64)    NULL,
        UpdateUserId   bigint          NULL,
        UpdateUserName nvarchar(64)    NULL,
        IsDelete       bit             NOT NULL CONSTRAINT DF_hr_emp_chg_line_del DEFAULT (0),
        DeleteTime     datetime        NULL,
        CONSTRAINT PK_hr_emp_chg_line PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_chg_line_key
        ON dbo.hr_emp_chg_line (chg_id, field_key) WHERE IsDelete = 0;
END;

-- ========== 可改字段配置 ==========
IF OBJECT_ID(N'dbo.hr_emp_chg_field', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_chg_field
    (
        Id             bigint         NOT NULL,
        field_key      nvarchar(64)   NOT NULL,
        enabled        bit            NOT NULL CONSTRAINT DF_hr_emp_chg_field_en DEFAULT (1),
        sort_no        int            NOT NULL CONSTRAINT DF_hr_emp_chg_field_sort DEFAULT (100),
        required       bit            NOT NULL CONSTRAINT DF_hr_emp_chg_field_req DEFAULT (0),
        CreateTime     datetime       NULL,
        UpdateTime     datetime       NULL,
        CreateUserId   bigint         NULL,
        CreateUserName nvarchar(64)   NULL,
        UpdateUserId   bigint         NULL,
        UpdateUserName nvarchar(64)   NULL,
        IsDelete       bit            NOT NULL CONSTRAINT DF_hr_emp_chg_field_del DEFAULT (0),
        DeleteTime     datetime       NULL,
        CONSTRAINT PK_hr_emp_chg_field PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_chg_field_key
        ON dbo.hr_emp_chg_field (field_key) WHERE IsDelete = 0;
END;

-- 种子：部门必填、岗位选填
IF NOT EXISTS (SELECT 1 FROM dbo.hr_emp_chg_field WHERE field_key = N'ccCode' AND IsDelete = 0)
    INSERT INTO dbo.hr_emp_chg_field
        (Id, field_key, enabled, sort_no, required, CreateTime, CreateUserId, CreateUserName, IsDelete)
    VALUES (864600000000001, N'ccCode', 1, 10, 1, @now, @uid, @uname, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.hr_emp_chg_field WHERE field_key = N'postId' AND IsDelete = 0)
    INSERT INTO dbo.hr_emp_chg_field
        (Id, field_key, enabled, sort_no, required, CreateTime, CreateUserId, CreateUserName, IsDelete)
    VALUES (864600000000002, N'postId', 1, 20, 0, @now, @uid, @uname, 0);

-- 异动类别 EmpChgType
MERGE dbo.code_mstr AS t
USING (VALUES
    (N'EmpChgType', N'Dept',   N'部门调动', N'10'),
    (N'EmpChgType', N'Post',   N'岗位调整', N'20'),
    (N'EmpChgType', N'Promo',  N'晋升',     N'30'),
    (N'EmpChgType', N'Demote', N'降职',     N'40'),
    (N'EmpChgType', N'Flat',   N'平调',     N'50'),
    (N'EmpChgType', N'Second', N'借调',     N'60'),
    (N'EmpChgType', N'Other',  N'其它',     N'90')
) AS s(code_field, code_value, code_desc, code_sort)
ON t.code_field = s.code_field AND t.code_value = s.code_value
WHEN MATCHED THEN
    UPDATE SET code_desc = s.code_desc, code_sort = s.code_sort
WHEN NOT MATCHED THEN
    INSERT (code_field, code_value, code_desc, code_sort)
    VALUES (s.code_field, s.code_value, s.code_desc, s.code_sort);

COMMIT;

SELECT N'hr_emp_chg' AS tbl, COUNT(*) AS cnt FROM dbo.hr_emp_chg;
SELECT field_key, required, sort_no FROM dbo.hr_emp_chg_field WHERE IsDelete = 0 ORDER BY sort_no;
SELECT code_value, code_desc FROM dbo.code_mstr WHERE code_field = N'EmpChgType' ORDER BY code_sort;
