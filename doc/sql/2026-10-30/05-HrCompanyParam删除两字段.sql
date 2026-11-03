/*
 * 变更说明：删除人事字段 工作周期日(WorkCycleDay)、试用期关联对象(ProbationLinkType)
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 影响库：ehrnew
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_HrCompanyParam_WorkCycleDay')
    ALTER TABLE dbo.HrCompanyParam DROP CONSTRAINT DF_HrCompanyParam_WorkCycleDay;
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_HrCompanyParam_ProbationLinkType')
    ALTER TABLE dbo.HrCompanyParam DROP CONSTRAINT DF_HrCompanyParam_ProbationLinkType;

IF COL_LENGTH(N'dbo.HrCompanyParam', N'WorkCycleDay') IS NOT NULL
    ALTER TABLE dbo.HrCompanyParam DROP COLUMN WorkCycleDay;
IF COL_LENGTH(N'dbo.HrCompanyParam', N'ProbationLinkType') IS NOT NULL
    ALTER TABLE dbo.HrCompanyParam DROP COLUMN ProbationLinkType;

DELETE FROM dbo.code_mstr_lang WHERE code_field = N'ProbationLinkType';
DELETE FROM dbo.code_mstr WHERE code_field = N'ProbationLinkType';
GO
