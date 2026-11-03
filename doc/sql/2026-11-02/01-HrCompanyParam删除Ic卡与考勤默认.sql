/*
 * 变更说明：删除 IC卡取值方式(IcCardMode)、考勤方式默认值(AttMethodDefault)
 * 模块：hrCompanyParam
 * 日期：2026-11-02
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_HrCompanyParam_IcCardMode')
    ALTER TABLE dbo.HrCompanyParam DROP CONSTRAINT DF_HrCompanyParam_IcCardMode;
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_HrCompanyParam_AttMethodDefault')
    ALTER TABLE dbo.HrCompanyParam DROP CONSTRAINT DF_HrCompanyParam_AttMethodDefault;

IF COL_LENGTH(N'dbo.HrCompanyParam', N'IcCardMode') IS NOT NULL
    ALTER TABLE dbo.HrCompanyParam DROP COLUMN IcCardMode;
IF COL_LENGTH(N'dbo.HrCompanyParam', N'AttMethodDefault') IS NOT NULL
    ALTER TABLE dbo.HrCompanyParam DROP COLUMN AttMethodDefault;

DELETE FROM dbo.code_mstr_lang WHERE code_field IN (N'IcCardMode', N'AttMethodDefault');
DELETE FROM dbo.code_mstr WHERE code_field IN (N'IcCardMode', N'AttMethodDefault');
GO
