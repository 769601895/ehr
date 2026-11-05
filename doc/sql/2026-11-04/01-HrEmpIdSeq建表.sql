/*
 * 变更说明：工号流水号段表（按厂别+作用域发号）
 * 模块：hrEmp / 公司参数工号规则
 * 日期：2026-11-04
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.HrEmpIdSeq', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HrEmpIdSeq
    (
        Id         BIGINT       NOT NULL,
        SiteCode   NVARCHAR(32) NOT NULL,
        ScopeKey   NVARCHAR(32) NOT NULL,
        NextVal    BIGINT       NOT NULL CONSTRAINT DF_HrEmpIdSeq_NextVal DEFAULT (0),
        UpdateTime DATETIME     NULL,
        CONSTRAINT PK_HrEmpIdSeq PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_HrEmpIdSeq_Site_Scope' AND object_id = OBJECT_ID(N'dbo.HrEmpIdSeq'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_HrEmpIdSeq_Site_Scope
        ON dbo.HrEmpIdSeq (SiteCode, ScopeKey);
END
GO
