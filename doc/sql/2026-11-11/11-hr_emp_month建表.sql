/*
 * 变更说明：人事月报批次/行表
 * 日期：2026-11-11
 * 影响库：HR 库
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_emp_month_batch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_month_batch (
        Id                  BIGINT          NOT NULL,
        site_code           NVARCHAR(32)    NOT NULL,
        site_name           NVARCHAR(100)   NULL,
        pay_period          CHAR(6)         NOT NULL,
        att_month           CHAR(6)         NOT NULL,
        att_from            DATE            NOT NULL,
        att_to              DATE            NOT NULL,
        status              VARCHAR(20)     NOT NULL,
        param_snapshot      NVARCHAR(MAX)   NULL,
        field_schema_snapshot NVARCHAR(MAX) NULL,
        freeze_version      INT             NOT NULL CONSTRAINT DF_hr_emp_month_batch_fv DEFAULT (0),
        frozen_time         DATETIME        NULL,
        frozen_user_id      BIGINT          NULL,
        unfreeze_reason     NVARCHAR(500)   NULL,
        consumed_time       DATETIME        NULL,
        remark              NVARCHAR(500)   NULL,
        CreateTime          DATETIME        NULL,
        UpdateTime          DATETIME        NULL,
        CreateUserId        BIGINT          NULL,
        CreateUserName      NVARCHAR(64)    NULL,
        UpdateUserId        BIGINT          NULL,
        UpdateUserName      NVARCHAR(64)    NULL,
        IsDelete            BIT             NOT NULL CONSTRAINT DF_hr_emp_month_batch_del DEFAULT (0),
        DeleteTime          DATETIME        NULL,
        CONSTRAINT PK_hr_emp_month_batch PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX ux_hr_emp_month_batch_site_period
        ON dbo.hr_emp_month_batch (site_code, pay_period) WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.hr_emp_month_line', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_month_line (
        Id                  BIGINT          NOT NULL,
        batch_id            BIGINT          NOT NULL,
        emp_id              NVARCHAR(50)    NOT NULL,
        iempid              BIGINT          NOT NULL,
        emp_name            NVARCHAR(100)   NULL,
        include_flag        VARCHAR(20)     NOT NULL,
        manual_lock         BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_ml DEFAULT (0),
        emp_state           INT             NULL,
        emp_sal             NVARCHAR(2)     NULL,
        cc_code             NVARCHAR(50)    NULL,
        cc_name             NVARCHAR(100)   NULL,
        post_id             NVARCHAR(50)    NULL,
        duty_id             NVARCHAR(50)    NULL,
        emp_in_date         DATE            NULL,
        emp_lea_date        DATE            NULL,
        emp_kq_indate       DATE            NULL,
        emp_kq_lea_date     DATE            NULL,
        need_attendance     BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_na DEFAULT (0),
        is_new_hire         BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_nh DEFAULT (0),
        is_leaver           BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_lv DEFAULT (0),
        is_suspend          BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_sp DEFAULT (0),
        has_chg_in_period   BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_hc DEFAULT (0),
        leave_settle_code   NVARCHAR(50)    NULL,
        leave_pay_cmd_status NVARCHAR(20)   NULL,
        pay_cmd_id          BIGINT          NULL,
        cc_code_at_start    NVARCHAR(50)    NULL,
        cc_code_at_end      NVARCHAR(50)    NULL,
        chg_summary         NVARCHAR(500)   NULL,
        chg_field_snapshot  NVARCHAR(MAX)   NULL,
        cost_cc_code        NVARCHAR(50)    NULL,
        exclude_reason      NVARCHAR(50)    NULL,
        include_reason      NVARCHAR(50)    NULL,
        remark              NVARCHAR(500)   NULL,
        emp_field_snapshot  NVARCHAR(MAX)   NULL,
        warn_codes          NVARCHAR(500)   NULL,
        error_codes         NVARCHAR(500)   NULL,
        CreateTime          DATETIME        NULL,
        UpdateTime          DATETIME        NULL,
        CreateUserId        BIGINT          NULL,
        CreateUserName      NVARCHAR(64)    NULL,
        UpdateUserId        BIGINT          NULL,
        UpdateUserName      NVARCHAR(64)    NULL,
        IsDelete            BIT             NOT NULL CONSTRAINT DF_hr_emp_month_line_del DEFAULT (0),
        DeleteTime          DATETIME        NULL,
        CONSTRAINT PK_hr_emp_month_line PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX ux_hr_emp_month_line_batch_emp
        ON dbo.hr_emp_month_line (batch_id, emp_id) WHERE IsDelete = 0;
    CREATE INDEX ix_hr_emp_month_line_batch ON dbo.hr_emp_month_line (batch_id) WHERE IsDelete = 0;
END
GO

/* 中文扩展属性（有则跳过） */
DECLARE @props TABLE (t SYSNAME, c SYSNAME, d NVARCHAR(200));
INSERT INTO @props (t, c, d) VALUES
(N'hr_emp_month_batch', N'site_code', N'厂别代码'),
(N'hr_emp_month_batch', N'pay_period', N'薪酬月'),
(N'hr_emp_month_batch', N'att_month', N'出勤归属月'),
(N'hr_emp_month_batch', N'att_from', N'考勤开始日'),
(N'hr_emp_month_batch', N'att_to', N'考勤结束日'),
(N'hr_emp_month_batch', N'status', N'批次状态'),
(N'hr_emp_month_batch', N'param_snapshot', N'参数快照'),
(N'hr_emp_month_batch', N'field_schema_snapshot', N'字段方案快照'),
(N'hr_emp_month_line', N'batch_id', N'批次Id'),
(N'hr_emp_month_line', N'emp_id', N'工号'),
(N'hr_emp_month_line', N'include_flag', N'纳入标志'),
(N'hr_emp_month_line', N'need_attendance', N'是否需考勤'),
(N'hr_emp_month_line', N'emp_field_snapshot', N'人事字段快照'),
(N'hr_emp_month_line', N'chg_field_snapshot', N'异动字段快照');

DECLARE @t SYSNAME, @c SYSNAME, @d NVARCHAR(200);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT t, c, d FROM @props;
OPEN cur;
FETCH NEXT FROM cur INTO @t, @c, @d;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF COL_LENGTH(N'dbo.' + @t, @c) IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM sys.extended_properties ep
           INNER JOIN sys.columns col ON ep.major_id = col.object_id AND ep.minor_id = col.column_id
           INNER JOIN sys.tables tb ON tb.object_id = col.object_id
           WHERE ep.name = N'MS_Description' AND tb.name = @t AND col.name = @c)
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=@d,
            @level0type=N'SCHEMA', @level0name=N'dbo',
            @level1type=N'TABLE',  @level1name=@t,
            @level2type=N'COLUMN', @level2name=@c;
    FETCH NEXT FROM cur INTO @t, @c, @d;
END
CLOSE cur; DEALLOCATE cur;
GO
