/*
 * 变更说明：hr_lookup_def 查找数据源；hr_post_lang 岗位译文；hr_ui_field_def.lookup_source
 * 模块：hrLookupDef / hrPost / hrUiFieldDef
 * 日期：2026-10-29
 * 影响库：ehrnew
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_lookup_def', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_lookup_def
    (
        Id                    BIGINT         NOT NULL,
        code                  NVARCHAR(64)   NOT NULL,
        name                  NVARCHAR(100)  NOT NULL,
        table_name            NVARCHAR(128)  NOT NULL,
        value_column          NVARCHAR(64)   NOT NULL,
        label_column          NVARCHAR(64)   NOT NULL,
        order_column          NVARCHAR(64)   NULL,
        filter_use_is_delete  BIT            NOT NULL CONSTRAINT DF_hr_lookup_def_is_delete DEFAULT (1),
        filter_enabled_column NVARCHAR(64)   NULL,
        filter_enabled_value  NVARCHAR(64)   NULL,
        filter_equal_column   NVARCHAR(64)   NULL,
        filter_equal_value    NVARCHAR(128)  NULL,
        lang_table            NVARCHAR(128)  NULL,
        lang_value_column     NVARCHAR(64)   NULL,
        lang_culture_column   NVARCHAR(64)   NULL,
        lang_label_column     NVARCHAR(64)   NULL,
        maintain_path         NVARCHAR(256)  NULL,
        enabled               BIT            NOT NULL CONSTRAINT DF_hr_lookup_def_enabled DEFAULT (1),
        remark                NVARCHAR(256)  NULL,
        CreateTime            DATETIME       NULL,
        UpdateTime            DATETIME       NULL,
        CreateUserId          BIGINT         NULL,
        CreateUserName        NVARCHAR(64)   NULL,
        UpdateUserId          BIGINT         NULL,
        UpdateUserName        NVARCHAR(64)   NULL,
        IsDelete              BIT            NOT NULL CONSTRAINT DF_hr_lookup_def_IsDelete DEFAULT (0),
        DeleteTime            DATETIME       NULL,
        CONSTRAINT PK_hr_lookup_def PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_lookup_def_code ON dbo.hr_lookup_def (code) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_lookup_def_enabled ON dbo.hr_lookup_def (enabled);
END
GO

IF OBJECT_ID(N'dbo.hr_post_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_post_lang
    (
        post_code NVARCHAR(50)  NOT NULL,
        culture   NVARCHAR(16)  NOT NULL,
        post_name NVARCHAR(100) NULL,
        CONSTRAINT PK_hr_post_lang PRIMARY KEY CLUSTERED (post_code, culture)
    );
END
GO

IF COL_LENGTH(N'dbo.hr_ui_field_def', N'lookup_source') IS NULL
BEGIN
    ALTER TABLE dbo.hr_ui_field_def ADD lookup_source NVARCHAR(64) NULL;
END
GO

PRINT N'hr_lookup_def / hr_post_lang / lookup_source 已就绪';
GO
