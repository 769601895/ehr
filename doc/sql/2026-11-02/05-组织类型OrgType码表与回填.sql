/*
 * 变更说明：组织架构增加组织类型码表 OrgType（1部门/2公司），回填 cc_mstr.org_type
 * 日期：2026-11-02
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

IF COL_LENGTH(N'dbo.cc_mstr', N'org_type') IS NULL
BEGIN
    ALTER TABLE dbo.cc_mstr ADD org_type NVARCHAR(50) NULL;
END
GO

-- 码表
DELETE FROM dbo.code_mstr_lang WHERE code_field = N'OrgType';
DELETE FROM dbo.code_mstr WHERE code_field = N'OrgType';

INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'OrgType', N'1', N'部门', N'10');
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'OrgType', N'2', N'公司', N'20');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'1', N'zh-CN', N'部门');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'2', N'zh-CN', N'公司');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'1', N'en', N'Department');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'2', N'en', N'Company');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'1', N'zh-TW', N'部門');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'2', N'zh-TW', N'公司');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'1', N'zh-HK', N'部門');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OrgType', N'2', N'zh-HK', N'公司');

-- 旧码 101/201 视作公司；其余空值默认部门
UPDATE dbo.cc_mstr SET org_type = N'2' WHERE org_type IN (N'101', N'201');
UPDATE dbo.cc_mstr SET org_type = N'1' WHERE org_type IS NULL OR LTRIM(RTRIM(org_type)) = N'' OR org_type NOT IN (N'1', N'2');
GO
