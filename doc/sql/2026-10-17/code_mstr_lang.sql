/*
 * 变更说明：人事代码描述译文
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.code_mstr_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.code_mstr_lang
    (
        code_field NVARCHAR(50)  NOT NULL,
        code_value NVARCHAR(50)  NOT NULL,
        culture    NVARCHAR(16)  NOT NULL,
        code_desc  NVARCHAR(256) NULL,
        CONSTRAINT PK_code_mstr_lang PRIMARY KEY CLUSTERED (code_field, code_value, culture)
    );
END;
