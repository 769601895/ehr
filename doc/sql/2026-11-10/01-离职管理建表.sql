/*
 * 变更说明：离职单 / 恢复单 / 薪资结算指令表
 * 日期：2026-11-10
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_emp_leave', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_leave
    (
        Id               bigint         NOT NULL,
        leave_no         nvarchar(50)   NOT NULL,
        emp_id           nvarchar(50)   NOT NULL,
        emp_name         nvarchar(100)  NULL,
        leave_date       date           NOT NULL,
        plan_effect_date date           NOT NULL,
        leave_type       nvarchar(50)   NOT NULL,
        leave_reason     nvarchar(50)   NOT NULL,
        salary_settle    nvarchar(50)   NOT NULL,
        is_blacklist     bit            NOT NULL CONSTRAINT DF_hr_emp_leave_black DEFAULT (0),
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
        IsDelete         bit            NOT NULL CONSTRAINT DF_hr_emp_leave_del DEFAULT (0),
        DeleteTime       datetime       NULL,
        CONSTRAINT PK_hr_emp_leave PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_leave_no ON dbo.hr_emp_leave (leave_no) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_leave_emp ON dbo.hr_emp_leave (emp_id, leave_date) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_leave_status ON dbo.hr_emp_leave (status, plan_effect_date) WHERE IsDelete = 0;
END

IF OBJECT_ID(N'dbo.hr_emp_leave_restore', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_leave_restore
    (
        Id               bigint         NOT NULL,
        restore_no       nvarchar(50)   NOT NULL,
        emp_id           nvarchar(50)   NOT NULL,
        emp_name         nvarchar(100)  NULL,
        restore_date     date           NOT NULL,
        plan_effect_date date           NOT NULL,
        clear_blacklist  bit            NOT NULL CONSTRAINT DF_hr_emp_leave_restore_clear DEFAULT (0),
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
        IsDelete         bit            NOT NULL CONSTRAINT DF_hr_emp_leave_restore_del DEFAULT (0),
        DeleteTime       datetime       NULL,
        CONSTRAINT PK_hr_emp_leave_restore PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_leave_restore_no ON dbo.hr_emp_leave_restore (restore_no) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_leave_restore_emp ON dbo.hr_emp_leave_restore (emp_id, restore_date) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_leave_restore_status ON dbo.hr_emp_leave_restore (status, plan_effect_date) WHERE IsDelete = 0;
END

IF OBJECT_ID(N'dbo.hr_emp_leave_pay_cmd', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_leave_pay_cmd
    (
        Id             bigint         NOT NULL,
        leave_id       bigint         NOT NULL,
        leave_no       nvarchar(50)   NOT NULL,
        emp_id         nvarchar(50)   NOT NULL,
        leave_date     date           NOT NULL,
        settle_code    nvarchar(50)   NOT NULL,
        pay_period     nvarchar(6)    NOT NULL,
        status         nvarchar(20)   NOT NULL,
        CreateTime     datetime       NULL,
        UpdateTime     datetime       NULL,
        CreateUserId   bigint         NULL,
        CreateUserName nvarchar(64)   NULL,
        UpdateUserId   bigint         NULL,
        UpdateUserName nvarchar(64)   NULL,
        IsDelete       bit            NOT NULL CONSTRAINT DF_hr_emp_leave_pay_cmd_del DEFAULT (0),
        DeleteTime     datetime       NULL,
        CONSTRAINT PK_hr_emp_leave_pay_cmd PRIMARY KEY (Id)
    );
    CREATE INDEX IX_hr_emp_leave_pay_cmd_emp ON dbo.hr_emp_leave_pay_cmd (emp_id, pay_period) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_emp_leave_pay_cmd_leave ON dbo.hr_emp_leave_pay_cmd (leave_id) WHERE IsDelete = 0;
END
