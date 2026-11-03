/*
 * 变更说明：新建公司参数主表/译文表（全新设计，不沿用 hr_sys_mstr）
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 * 字段范围：以最新界面截图为准
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.HrCompanyParam', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HrCompanyParam
    (
        Id                   BIGINT         NOT NULL,
        SiteCode             NVARCHAR(32)   NOT NULL,
        SiteName             NVARCHAR(100)  NOT NULL,
        EmpIdMode            INT            NOT NULL CONSTRAINT DF_HrCompanyParam_EmpIdMode DEFAULT (1),
        EmpIdLength          INT            NOT NULL CONSTRAINT DF_HrCompanyParam_EmpIdLength DEFAULT (8),
        EmpIdFixedPrefix     NVARCHAR(32)   NULL,
        ProbationOnHire      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_ProbationOnHire DEFAULT (1),
        ProbationHireDays    INT            NOT NULL CONSTRAINT DF_HrCompanyParam_ProbationHireDays DEFAULT (2),
        ProbationDayInclusive INT           NOT NULL CONSTRAINT DF_HrCompanyParam_ProbationDayInclusive DEFAULT (1),
        DimissionAudit       INT            NOT NULL CONSTRAINT DF_HrCompanyParam_DimissionAudit DEFAULT (0),
        TransferAudit        INT            NOT NULL CONSTRAINT DF_HrCompanyParam_TransferAudit DEFAULT (0),
        RegularizeAudit      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_RegularizeAudit DEFAULT (0),
        IdCardV2Enabled      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_IdCardV2Enabled DEFAULT (1),
        HireDayAttMode       INT            NOT NULL CONSTRAINT DF_HrCompanyParam_HireDayAttMode DEFAULT (0),
        LeaveDayAttMode      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_LeaveDayAttMode DEFAULT (1),
        AttCloseDay          INT            NOT NULL CONSTRAINT DF_HrCompanyParam_AttCloseDay DEFAULT (31),
        RequiredAttDayMode   INT            NOT NULL CONSTRAINT DF_HrCompanyParam_RequiredAttDayMode DEFAULT (0),
        PayrollAttMonth      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_PayrollAttMonth DEFAULT (0),
        OtAuditMode          INT            NOT NULL CONSTRAINT DF_HrCompanyParam_OtAuditMode DEFAULT (0),
        LeaveAudit           INT            NOT NULL CONSTRAINT DF_HrCompanyParam_LeaveAudit DEFAULT (0),
        OtCrossdayMode       INT            NOT NULL CONSTRAINT DF_HrCompanyParam_OtCrossdayMode DEFAULT (0),
        CardMakeupAudit      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_CardMakeupAudit DEFAULT (0),
        OtMinUnitHour        DECIMAL(18,2)  NOT NULL CONSTRAINT DF_HrCompanyParam_OtMinUnitHour DEFAULT (0.50),
        InsMonthReportMode   INT            NOT NULL CONSTRAINT DF_HrCompanyParam_InsMonthReportMode DEFAULT (0),
        InsFeeToPayroll      INT            NOT NULL CONSTRAINT DF_HrCompanyParam_InsFeeToPayroll DEFAULT (1),
        InsSameMonthReport   INT            NOT NULL CONSTRAINT DF_HrCompanyParam_InsSameMonthReport DEFAULT (1),
        CreateTime           DATETIME       NULL,
        UpdateTime           DATETIME       NULL,
        CreateUserId         BIGINT         NULL,
        CreateUserName       NVARCHAR(64)   NULL,
        UpdateUserId         BIGINT         NULL,
        UpdateUserName       NVARCHAR(64)   NULL,
        IsDelete             BIT            NOT NULL CONSTRAINT DF_HrCompanyParam_IsDelete DEFAULT (0),
        DeleteTime           DATETIME       NULL,
        CONSTRAINT PK_HrCompanyParam PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_HrCompanyParam_SiteCode' AND object_id = OBJECT_ID(N'dbo.HrCompanyParam'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_HrCompanyParam_SiteCode
        ON dbo.HrCompanyParam (SiteCode)
        WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.HrCompanyParamLang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HrCompanyParamLang
    (
        ParamId   BIGINT        NOT NULL,
        Culture   NVARCHAR(16)  NOT NULL,
        SiteName  NVARCHAR(100) NULL,
        CONSTRAINT PK_HrCompanyParamLang PRIMARY KEY CLUSTERED (ParamId, Culture)
    );
END
GO
