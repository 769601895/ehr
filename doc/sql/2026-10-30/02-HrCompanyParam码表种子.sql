/*
 * 变更说明：公司参数下拉码表种子；code_field = 列名（PascalCase，无下划线）
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 影响库：ehrnew
 * 是否幂等：是
 */
SET NOCOUNT ON;
GO

-- EmpIdMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'0', N'自动流水号', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'自动流水号', code_sort=N'10'
    WHERE code_field=N'EmpIdMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'0', N'zh-CN', N'自动流水号');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'自动流水号'
    WHERE code_field=N'EmpIdMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'0', N'en', N'Auto serial');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Auto serial'
    WHERE code_field=N'EmpIdMode' AND code_value=N'0' AND culture=N'en';
GO

-- EmpIdMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'1', N'手工维护', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'手工维护', code_sort=N'20'
    WHERE code_field=N'EmpIdMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'1', N'zh-CN', N'手工维护');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'手工维护'
    WHERE code_field=N'EmpIdMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'1', N'en', N'Manual');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Manual'
    WHERE code_field=N'EmpIdMode' AND code_value=N'1' AND culture=N'en';
GO

-- EmpIdMode=4
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'4')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'4', N'固定号+流水号', N'50');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'固定号+流水号', code_sort=N'50'
    WHERE code_field=N'EmpIdMode' AND code_value=N'4';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'4' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'4', N'zh-CN', N'固定号+流水号');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'固定号+流水号'
    WHERE code_field=N'EmpIdMode' AND code_value=N'4' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'4' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'4', N'en', N'Fixed prefix + serial');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Fixed prefix + serial'
    WHERE code_field=N'EmpIdMode' AND code_value=N'4' AND culture=N'en';
GO

-- EmpIdMode=5
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'5')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'5', N'4位年+月+流水号', N'60');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'4位年+月+流水号', code_sort=N'60'
    WHERE code_field=N'EmpIdMode' AND code_value=N'5';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'5' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'5', N'zh-CN', N'4位年+月+流水号');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'4位年+月+流水号'
    WHERE code_field=N'EmpIdMode' AND code_value=N'5' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'5' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'5', N'en', N'4-digit year+month+serial');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'4-digit year+month+serial'
    WHERE code_field=N'EmpIdMode' AND code_value=N'5' AND culture=N'en';
GO

-- EmpIdMode=8
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'8')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'8', N'固定号+4位年+2位月+特殊符号+流水号', N'70');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'固定号+4位年+2位月+特殊符号+流水号', code_sort=N'70'
    WHERE code_field=N'EmpIdMode' AND code_value=N'8';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'8' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'8', N'zh-CN', N'固定号+4位年+2位月+特殊符号+流水号');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'固定号+4位年+2位月+特殊符号+流水号'
    WHERE code_field=N'EmpIdMode' AND code_value=N'8' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'8' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'8', N'en', N'Fixed+yyyy+mm+symbol+serial');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Fixed+yyyy+mm+symbol+serial'
    WHERE code_field=N'EmpIdMode' AND code_value=N'8' AND culture=N'en';
GO

-- EmpIdMode=9
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'EmpIdMode' AND code_value=N'9')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'EmpIdMode', N'9', N'固定号+4位年+2位月+流水号', N'80');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'固定号+4位年+2位月+流水号', code_sort=N'80'
    WHERE code_field=N'EmpIdMode' AND code_value=N'9';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'9' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'9', N'zh-CN', N'固定号+4位年+2位月+流水号');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'固定号+4位年+2位月+流水号'
    WHERE code_field=N'EmpIdMode' AND code_value=N'9' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'EmpIdMode' AND code_value=N'9' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'EmpIdMode', N'9', N'en', N'Fixed+yyyy+mm+serial');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Fixed+yyyy+mm+serial'
    WHERE code_field=N'EmpIdMode' AND code_value=N'9' AND culture=N'en';
GO

-- ProbationOnHire=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'ProbationOnHire' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'ProbationOnHire', N'0', N'不试用', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'不试用', code_sort=N'10'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationOnHire' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationOnHire', N'0', N'zh-CN', N'不试用');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'不试用'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationOnHire' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationOnHire', N'0', N'en', N'No probation');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No probation'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'0' AND culture=N'en';
GO

-- ProbationOnHire=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'ProbationOnHire' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'ProbationOnHire', N'1', N'试用', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'试用', code_sort=N'20'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationOnHire' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationOnHire', N'1', N'zh-CN', N'试用');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'试用'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationOnHire' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationOnHire', N'1', N'en', N'Probation');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Probation'
    WHERE code_field=N'ProbationOnHire' AND code_value=N'1' AND culture=N'en';
GO

-- ProbationDayInclusive=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'ProbationDayInclusive', N'1', N'是', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'10'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationDayInclusive', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationDayInclusive', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'1' AND culture=N'en';
GO

-- ProbationDayInclusive=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'ProbationDayInclusive', N'0', N'否', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'20'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationDayInclusive', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'ProbationDayInclusive', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'ProbationDayInclusive' AND code_value=N'0' AND culture=N'en';
GO

-- DimissionAudit=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'DimissionAudit' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'DimissionAudit', N'0', N'否', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'10'
    WHERE code_field=N'DimissionAudit' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'DimissionAudit' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'DimissionAudit', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'DimissionAudit' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'DimissionAudit' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'DimissionAudit', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'DimissionAudit' AND code_value=N'0' AND culture=N'en';
GO

-- DimissionAudit=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'DimissionAudit' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'DimissionAudit', N'1', N'是', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'20'
    WHERE code_field=N'DimissionAudit' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'DimissionAudit' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'DimissionAudit', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'DimissionAudit' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'DimissionAudit' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'DimissionAudit', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'DimissionAudit' AND code_value=N'1' AND culture=N'en';
GO

-- TransferAudit=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'TransferAudit' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'TransferAudit', N'0', N'否', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'10'
    WHERE code_field=N'TransferAudit' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'TransferAudit' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'TransferAudit', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'TransferAudit' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'TransferAudit' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'TransferAudit', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'TransferAudit' AND code_value=N'0' AND culture=N'en';
GO

-- TransferAudit=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'TransferAudit' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'TransferAudit', N'1', N'是', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'20'
    WHERE code_field=N'TransferAudit' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'TransferAudit' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'TransferAudit', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'TransferAudit' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'TransferAudit' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'TransferAudit', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'TransferAudit' AND code_value=N'1' AND culture=N'en';
GO

-- RegularizeAudit=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'RegularizeAudit' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'RegularizeAudit', N'0', N'否', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'10'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RegularizeAudit' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RegularizeAudit', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RegularizeAudit' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RegularizeAudit', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'0' AND culture=N'en';
GO

-- RegularizeAudit=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'RegularizeAudit' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'RegularizeAudit', N'1', N'是', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'20'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RegularizeAudit' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RegularizeAudit', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RegularizeAudit' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RegularizeAudit', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'RegularizeAudit' AND code_value=N'1' AND culture=N'en';
GO

-- IdCardV2Enabled=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'IdCardV2Enabled', N'1', N'是', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'10'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'IdCardV2Enabled', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'IdCardV2Enabled', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'1' AND culture=N'en';
GO

-- IdCardV2Enabled=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'IdCardV2Enabled', N'0', N'否', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'20'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'IdCardV2Enabled', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'IdCardV2Enabled', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'IdCardV2Enabled' AND code_value=N'0' AND culture=N'en';
GO

-- HireDayAttMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'HireDayAttMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'HireDayAttMode', N'1', N'是', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'10'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'HireDayAttMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'HireDayAttMode', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'HireDayAttMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'HireDayAttMode', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'1' AND culture=N'en';
GO

-- HireDayAttMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'HireDayAttMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'HireDayAttMode', N'0', N'否', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'20'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'HireDayAttMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'HireDayAttMode', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'HireDayAttMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'HireDayAttMode', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'HireDayAttMode' AND code_value=N'0' AND culture=N'en';
GO

-- LeaveDayAttMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'LeaveDayAttMode', N'1', N'是', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'10'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveDayAttMode', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveDayAttMode', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'1' AND culture=N'en';
GO

-- LeaveDayAttMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'LeaveDayAttMode', N'0', N'否', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'20'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveDayAttMode', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveDayAttMode', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'LeaveDayAttMode' AND code_value=N'0' AND culture=N'en';
GO

-- RequiredAttDayMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'RequiredAttDayMode', N'0', N'正常工作日', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'正常工作日', code_sort=N'10'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'0', N'zh-CN', N'正常工作日');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'正常工作日'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'0', N'en', N'Normal workdays');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Normal workdays'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'0' AND culture=N'en';
GO

-- RequiredAttDayMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'RequiredAttDayMode', N'1', N'全月天数', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'全月天数', code_sort=N'20'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'1', N'zh-CN', N'全月天数');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'全月天数'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'1', N'en', N'Full month days');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Full month days'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'1' AND culture=N'en';
GO

-- RequiredAttDayMode=2
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'RequiredAttDayMode', N'2', N'正常工作日(含节日)', N'30');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'正常工作日(含节日)', code_sort=N'30'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'2', N'zh-CN', N'正常工作日(含节日)');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'正常工作日(含节日)'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'RequiredAttDayMode', N'2', N'en', N'Workdays incl. holidays');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Workdays incl. holidays'
    WHERE code_field=N'RequiredAttDayMode' AND code_value=N'2' AND culture=N'en';
GO

-- PayrollAttMonth=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'PayrollAttMonth' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'PayrollAttMonth', N'0', N'当月', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'当月', code_sort=N'10'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'PayrollAttMonth' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'PayrollAttMonth', N'0', N'zh-CN', N'当月');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'当月'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'PayrollAttMonth' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'PayrollAttMonth', N'0', N'en', N'Current month');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Current month'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'0' AND culture=N'en';
GO

-- PayrollAttMonth=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'PayrollAttMonth' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'PayrollAttMonth', N'1', N'上月', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'上月', code_sort=N'20'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'PayrollAttMonth' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'PayrollAttMonth', N'1', N'zh-CN', N'上月');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'上月'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'PayrollAttMonth' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'PayrollAttMonth', N'1', N'en', N'Previous month');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Previous month'
    WHERE code_field=N'PayrollAttMonth' AND code_value=N'1' AND culture=N'en';
GO

-- OtAuditMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'OtAuditMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'OtAuditMode', N'1', N'是', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'10'
    WHERE code_field=N'OtAuditMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtAuditMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtAuditMode', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'OtAuditMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtAuditMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtAuditMode', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'OtAuditMode' AND code_value=N'1' AND culture=N'en';
GO

-- OtAuditMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'OtAuditMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'OtAuditMode', N'0', N'否', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'20'
    WHERE code_field=N'OtAuditMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtAuditMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtAuditMode', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'OtAuditMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtAuditMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtAuditMode', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'OtAuditMode' AND code_value=N'0' AND culture=N'en';
GO

-- LeaveAudit=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'LeaveAudit' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'LeaveAudit', N'0', N'否', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'10'
    WHERE code_field=N'LeaveAudit' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveAudit' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveAudit', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'LeaveAudit' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveAudit' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveAudit', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'LeaveAudit' AND code_value=N'0' AND culture=N'en';
GO

-- LeaveAudit=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'LeaveAudit' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'LeaveAudit', N'1', N'是', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'20'
    WHERE code_field=N'LeaveAudit' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveAudit' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveAudit', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'LeaveAudit' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'LeaveAudit' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'LeaveAudit', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'LeaveAudit' AND code_value=N'1' AND culture=N'en';
GO

-- OtCrossdayMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'OtCrossdayMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'OtCrossdayMode', N'0', N'当日计', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'当日计', code_sort=N'10'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtCrossdayMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtCrossdayMode', N'0', N'zh-CN', N'当日计');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'当日计'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtCrossdayMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtCrossdayMode', N'0', N'en', N'Same day');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Same day'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'0' AND culture=N'en';
GO

-- OtCrossdayMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'OtCrossdayMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'OtCrossdayMode', N'1', N'次日计', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'次日计', code_sort=N'20'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtCrossdayMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtCrossdayMode', N'1', N'zh-CN', N'次日计');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'次日计'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'OtCrossdayMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'OtCrossdayMode', N'1', N'en', N'Next day');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Next day'
    WHERE code_field=N'OtCrossdayMode' AND code_value=N'1' AND culture=N'en';
GO

-- CardMakeupAudit=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'CardMakeupAudit' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'CardMakeupAudit', N'0', N'否', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'否', code_sort=N'10'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'CardMakeupAudit' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'CardMakeupAudit', N'0', N'zh-CN', N'否');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'否'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'CardMakeupAudit' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'CardMakeupAudit', N'0', N'en', N'No');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'No'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'0' AND culture=N'en';
GO

-- CardMakeupAudit=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'CardMakeupAudit' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'CardMakeupAudit', N'1', N'是', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'是', code_sort=N'20'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'CardMakeupAudit' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'CardMakeupAudit', N'1', N'zh-CN', N'是');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'是'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'CardMakeupAudit' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'CardMakeupAudit', N'1', N'en', N'Yes');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Yes'
    WHERE code_field=N'CardMakeupAudit' AND code_value=N'1' AND culture=N'en';
GO

-- InsMonthReportMode=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsMonthReportMode' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsMonthReportMode', N'0', N'当月生成月报', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'当月生成月报', code_sort=N'10'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsMonthReportMode' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsMonthReportMode', N'0', N'zh-CN', N'当月生成月报');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'当月生成月报'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsMonthReportMode' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsMonthReportMode', N'0', N'en', N'Current month report');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Current month report'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'0' AND culture=N'en';
GO

-- InsMonthReportMode=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsMonthReportMode' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsMonthReportMode', N'1', N'次月生成月报', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'次月生成月报', code_sort=N'20'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsMonthReportMode' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsMonthReportMode', N'1', N'zh-CN', N'次月生成月报');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'次月生成月报'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsMonthReportMode' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsMonthReportMode', N'1', N'en', N'Next month report');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Next month report'
    WHERE code_field=N'InsMonthReportMode' AND code_value=N'1' AND culture=N'en';
GO

-- InsFeeToPayroll=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsFeeToPayroll', N'1', N'保费提交当月薪资', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'保费提交当月薪资', code_sort=N'10'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsFeeToPayroll', N'1', N'zh-CN', N'保费提交当月薪资');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'保费提交当月薪资'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsFeeToPayroll', N'1', N'en', N'Fee to current payroll');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Fee to current payroll'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'1' AND culture=N'en';
GO

-- InsFeeToPayroll=2
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsFeeToPayroll', N'2', N'保费提交上月薪资', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'保费提交上月薪资', code_sort=N'20'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsFeeToPayroll', N'2', N'zh-CN', N'保费提交上月薪资');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'保费提交上月薪资'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsFeeToPayroll', N'2', N'en', N'Fee to previous payroll');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Fee to previous payroll'
    WHERE code_field=N'InsFeeToPayroll' AND code_value=N'2' AND culture=N'en';
GO

-- InsSameMonthReport=0
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsSameMonthReport' AND code_value=N'0')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsSameMonthReport', N'0', N'生成', N'10');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'生成', code_sort=N'10'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'0';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsSameMonthReport' AND code_value=N'0' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsSameMonthReport', N'0', N'zh-CN', N'生成');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'生成'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'0' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsSameMonthReport' AND code_value=N'0' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsSameMonthReport', N'0', N'en', N'Generate');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Generate'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'0' AND culture=N'en';
GO

-- InsSameMonthReport=1
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'InsSameMonthReport' AND code_value=N'1')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'InsSameMonthReport', N'1', N'不生成', N'20');
ELSE
    UPDATE dbo.code_mstr SET code_desc=N'不生成', code_sort=N'20'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'1';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsSameMonthReport' AND code_value=N'1' AND culture=N'zh-CN')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsSameMonthReport', N'1', N'zh-CN', N'不生成');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'不生成'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'1' AND culture=N'zh-CN';
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'InsSameMonthReport' AND code_value=N'1' AND culture=N'en')
    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
    VALUES (N'InsSameMonthReport', N'1', N'en', N'Do not generate');
ELSE
    UPDATE dbo.code_mstr_lang SET code_desc=N'Do not generate'
    WHERE code_field=N'InsSameMonthReport' AND code_value=N'1' AND culture=N'en';
GO

