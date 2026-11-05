/*
 * 员工资料导入：模板 / 模板字段 / 批次历史 / 失败行
 * 库：HR（与 h_emp_mstr 同库）
 * 幂等：IF NOT EXISTS
 */

IF OBJECT_ID(N'dbo.hr_emp_import_tpl', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_import_tpl
    (
        Id              BIGINT          NOT NULL,
        owner_user_id   BIGINT          NOT NULL,
        import_mode     NVARCHAR(16)    NOT NULL,   -- Add / Update
        name            NVARCHAR(100)   NOT NULL,
        remark          NVARCHAR(256)   NULL,
        CreateTime      DATETIME        NULL,
        UpdateTime      DATETIME        NULL,
        CreateUserId    BIGINT          NULL,
        CreateUserName  NVARCHAR(64)    NULL,
        UpdateUserId    BIGINT          NULL,
        UpdateUserName  NVARCHAR(64)    NULL,
        IsDelete        BIT             NOT NULL CONSTRAINT DF_hr_emp_import_tpl_IsDelete DEFAULT (0),
        DeleteTime      DATETIME        NULL,
        CONSTRAINT PK_hr_emp_import_tpl PRIMARY KEY (Id)
    );
    CREATE INDEX IX_hr_emp_import_tpl_owner ON dbo.hr_emp_import_tpl (owner_user_id, import_mode, IsDelete);
END
GO

IF OBJECT_ID(N'dbo.hr_emp_import_tpl_field', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_import_tpl_field
    (
        Id              BIGINT          NOT NULL,
        tpl_id          BIGINT          NOT NULL,
        field_key       NVARCHAR(64)    NOT NULL,
        sort_no         INT             NOT NULL CONSTRAINT DF_hr_emp_import_tpl_field_sort DEFAULT (100),
        is_locked       BIT             NOT NULL CONSTRAINT DF_hr_emp_import_tpl_field_lock DEFAULT (0),
        CreateTime      DATETIME        NULL,
        UpdateTime      DATETIME        NULL,
        CreateUserId    BIGINT          NULL,
        CreateUserName  NVARCHAR(64)    NULL,
        UpdateUserId    BIGINT          NULL,
        UpdateUserName  NVARCHAR(64)    NULL,
        IsDelete        BIT             NOT NULL CONSTRAINT DF_hr_emp_import_tpl_field_IsDelete DEFAULT (0),
        DeleteTime      DATETIME        NULL,
        CONSTRAINT PK_hr_emp_import_tpl_field PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX UX_hr_emp_import_tpl_field ON dbo.hr_emp_import_tpl_field (tpl_id, field_key) WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.hr_emp_import_batch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_import_batch
    (
        Id                  BIGINT          NOT NULL,
        batch_no            NVARCHAR(40)    NOT NULL,
        import_mode         NVARCHAR(16)    NOT NULL,
        tpl_id              BIGINT          NULL,
        tpl_name_snapshot   NVARCHAR(100)   NULL,
        field_keys_snapshot NVARCHAR(MAX)   NULL,
        file_name           NVARCHAR(256)   NULL,
        total_count         INT             NOT NULL CONSTRAINT DF_hr_emp_import_batch_total DEFAULT (0),
        success_count       INT             NOT NULL CONSTRAINT DF_hr_emp_import_batch_ok DEFAULT (0),
        fail_count          INT             NOT NULL CONSTRAINT DF_hr_emp_import_batch_fail DEFAULT (0),
        status              NVARCHAR(16)    NOT NULL, -- Success / Failed
        error_summary       NVARCHAR(1000)  NULL,
        operator_user_id    BIGINT          NOT NULL,
        CreateTime          DATETIME        NULL,
        UpdateTime          DATETIME        NULL,
        CreateUserId        BIGINT          NULL,
        CreateUserName      NVARCHAR(64)    NULL,
        UpdateUserId        BIGINT          NULL,
        UpdateUserName      NVARCHAR(64)    NULL,
        IsDelete            BIT             NOT NULL CONSTRAINT DF_hr_emp_import_batch_IsDelete DEFAULT (0),
        DeleteTime          DATETIME        NULL,
        CONSTRAINT PK_hr_emp_import_batch PRIMARY KEY (Id)
    );
    CREATE INDEX IX_hr_emp_import_batch_op ON dbo.hr_emp_import_batch (operator_user_id, CreateTime DESC) WHERE IsDelete = 0;
    CREATE UNIQUE INDEX UX_hr_emp_import_batch_no ON dbo.hr_emp_import_batch (batch_no) WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.hr_emp_import_batch_row', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_emp_import_batch_row
    (
        Id              BIGINT          NOT NULL,
        batch_id        BIGINT          NOT NULL,
        row_no          INT             NOT NULL,
        emp_id          NVARCHAR(40)    NULL,
        error_message   NVARCHAR(1000)  NOT NULL,
        CreateTime      DATETIME        NULL,
        UpdateTime      DATETIME        NULL,
        CreateUserId    BIGINT          NULL,
        CreateUserName  NVARCHAR(64)    NULL,
        UpdateUserId    BIGINT          NULL,
        UpdateUserName  NVARCHAR(64)    NULL,
        IsDelete        BIT             NOT NULL CONSTRAINT DF_hr_emp_import_batch_row_IsDelete DEFAULT (0),
        DeleteTime      DATETIME        NULL,
        CONSTRAINT PK_hr_emp_import_batch_row PRIMARY KEY (Id)
    );
    CREATE INDEX IX_hr_emp_import_batch_row ON dbo.hr_emp_import_batch_row (batch_id, row_no) WHERE IsDelete = 0;
END
GO
