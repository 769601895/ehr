/*
 * 变更说明：hr_wf_form_field 增加 lookup_source（查找数据源，与 code_field 互斥）
 * 模块：hrWfForm
 * 日期：2026-10-29
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.hr_wf_form_field', N'lookup_source') IS NULL
BEGIN
    ALTER TABLE dbo.hr_wf_form_field ADD lookup_source NVARCHAR(64) NULL;
END
GO

PRINT N'hr_wf_form_field.lookup_source 已就绪';
GO
