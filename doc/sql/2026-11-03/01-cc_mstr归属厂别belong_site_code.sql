/*
 * 变更说明：cc_mstr 增加归属公司厂别代码 belong_site_code（关联 HrCompanyParam.SiteCode，可空）
 * 模块：hrCompanyParam / 公司归属厂别设置
 * 日期：2026-11-03
 * 影响库：ehrnew（cc_mstr）
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.cc_mstr', N'belong_site_code') IS NULL
BEGIN
    ALTER TABLE dbo.cc_mstr ADD belong_site_code nvarchar(32) NULL;
    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description', @value = N'归属公司厂别代码',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE',  @level1name = N'cc_mstr',
        @level2type = N'COLUMN', @level2name = N'belong_site_code';
END
GO
