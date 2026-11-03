/*
 * 变更说明：工号编码方式 EmpIdMode 新增两项
 * 8=固定号+4位年+2位月+特殊符号+流水号
 * 9=固定号+4位年+2位月+流水号
 * 日期：2026-10-31
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;

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
