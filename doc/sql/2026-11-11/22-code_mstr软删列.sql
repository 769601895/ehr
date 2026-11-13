/*
 * 变更说明：code_mstr 明细软删（进回收站；业务仍可按旧码值解析）
 * 日期：2026-11-11
 * 影响库：与 code_mstr 同库
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF COL_LENGTH(N'dbo.code_mstr', N'is_delete') IS NULL
BEGIN
    ALTER TABLE dbo.code_mstr ADD is_delete BIT NOT NULL
        CONSTRAINT DF_code_mstr_is_delete DEFAULT (0);
END
GO

IF COL_LENGTH(N'dbo.code_mstr', N'delete_time') IS NULL
    ALTER TABLE dbo.code_mstr ADD delete_time DATETIME NULL;
GO

IF COL_LENGTH(N'dbo.code_mstr', N'delete_user_id') IS NULL
    ALTER TABLE dbo.code_mstr ADD delete_user_id BIGINT NULL;
GO

IF COL_LENGTH(N'dbo.code_mstr', N'delete_user_name') IS NULL
    ALTER TABLE dbo.code_mstr ADD delete_user_name NVARCHAR(64) NULL;
GO

/* 历史行兜底 */
UPDATE dbo.code_mstr SET is_delete = 0 WHERE is_delete IS NULL;
GO
