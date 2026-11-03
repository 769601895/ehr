/*
 * 变更说明：入职日考勤 / 离职日考勤否 / 加班审核 仅保留 是=1、否=0
 * 并回写 HrCompanyParam 存量数据
 * 日期：2026-11-02
 * 是否幂等：是（重复执行会再次翻转 HireDayAttMode，勿对已精简库重跑 UPDATE；码表 DELETE+INSERT 可重跑）
 * 注意：若库已是 1/0 是否码，请跳过三段 UPDATE，只保留 DELETE/INSERT。
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

-- HireDayAttMode（旧：0=是,1/2=免考勤）
IF EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'HireDayAttMode' AND code_value=N'2')
BEGIN
    UPDATE dbo.HrCompanyParam SET HireDayAttMode = CASE WHEN HireDayAttMode = 0 THEN 1 ELSE 0 END
    WHERE HireDayAttMode IN (0, 1, 2);
END
DELETE FROM dbo.code_mstr_lang WHERE code_field = N'HireDayAttMode';
DELETE FROM dbo.code_mstr WHERE code_field = N'HireDayAttMode';
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'HireDayAttMode', N'1', N'是', N'10');
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'HireDayAttMode', N'0', N'否', N'20');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'HireDayAttMode', N'1', N'zh-CN', N'是');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'HireDayAttMode', N'0', N'zh-CN', N'否');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'HireDayAttMode', N'1', N'en', N'Yes');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'HireDayAttMode', N'0', N'en', N'No');

-- LeaveDayAttMode（旧：1=有薪需刷卡≈是）
IF EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'LeaveDayAttMode' AND code_value=N'2')
BEGIN
    UPDATE dbo.HrCompanyParam SET LeaveDayAttMode = CASE WHEN LeaveDayAttMode = 1 THEN 1 ELSE 0 END
    WHERE LeaveDayAttMode IN (0, 1, 2);
END
DELETE FROM dbo.code_mstr_lang WHERE code_field = N'LeaveDayAttMode';
DELETE FROM dbo.code_mstr WHERE code_field = N'LeaveDayAttMode';
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'LeaveDayAttMode', N'1', N'是', N'10');
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'LeaveDayAttMode', N'0', N'否', N'20');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'LeaveDayAttMode', N'1', N'zh-CN', N'是');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'LeaveDayAttMode', N'0', N'zh-CN', N'否');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'LeaveDayAttMode', N'1', N'en', N'Yes');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'LeaveDayAttMode', N'0', N'en', N'No');

-- OtAuditMode（旧：0/2=否，1/3=是）
IF EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'OtAuditMode' AND code_value=N'3')
BEGIN
    UPDATE dbo.HrCompanyParam SET OtAuditMode = CASE WHEN OtAuditMode IN (1, 3) THEN 1 ELSE 0 END
    WHERE OtAuditMode IN (0, 1, 2, 3);
END
DELETE FROM dbo.code_mstr_lang WHERE code_field = N'OtAuditMode';
DELETE FROM dbo.code_mstr WHERE code_field = N'OtAuditMode';
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'OtAuditMode', N'1', N'是', N'10');
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort) VALUES (N'OtAuditMode', N'0', N'否', N'20');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OtAuditMode', N'1', N'zh-CN', N'是');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OtAuditMode', N'0', N'zh-CN', N'否');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OtAuditMode', N'1', N'en', N'Yes');
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc) VALUES (N'OtAuditMode', N'0', N'en', N'No');
GO
