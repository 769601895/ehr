/*
 * 变更说明：代码类元数据表 code_field / code_field_lang（左类右值说明与软删）
 * 日期：2026-11-11
 * 影响库：与 code_mstr 同库
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.code_field', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.code_field (
        code_field       NVARCHAR(50)  NOT NULL,
        field_name       NVARCHAR(100) NOT NULL,
        usage_desc       NVARCHAR(256) NULL,
        order_no         INT           NOT NULL CONSTRAINT DF_code_field_order_no DEFAULT (100),
        is_delete        BIT           NOT NULL CONSTRAINT DF_code_field_is_delete DEFAULT (0),
        delete_time      DATETIME      NULL,
        delete_user_id   BIGINT        NULL,
        delete_user_name NVARCHAR(64)  NULL,
        create_time      DATETIME      NULL,
        update_time      DATETIME      NULL,
        CONSTRAINT PK_code_field PRIMARY KEY CLUSTERED (code_field)
    );
END
GO

IF OBJECT_ID(N'dbo.code_field_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.code_field_lang (
        code_field NVARCHAR(50)  NOT NULL,
        culture    NVARCHAR(16)  NOT NULL,
        field_name NVARCHAR(100) NULL,
        usage_desc NVARCHAR(256) NULL,
        CONSTRAINT PK_code_field_lang PRIMARY KEY CLUSTERED (code_field, culture)
    );
END
GO
