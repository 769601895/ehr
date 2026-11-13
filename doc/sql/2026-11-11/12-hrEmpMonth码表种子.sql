/*
 * 变更说明：人事月报纳入/排除原因码表
 * 日期：2026-11-11
 * 影响库：主库 code_mstr / code_mstr_lang
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

;WITH seed AS (
    SELECT * FROM (VALUES
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'不在本月计薪范围', N'10'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'停职不计薪', N'20'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'重复人员', N'30'),
        (N'EmpMonthExcludeReason', N'DataError', N'资料错误', N'40'),
        (N'EmpMonthExcludeReason', N'Other', N'其他', N'90'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'人工补入', N'10'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'月内复职', N'20'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'特殊计薪', N'30'),
        (N'EmpMonthIncludeReason', N'Correction', N'纠错纳入', N'40'),
        (N'EmpMonthIncludeReason', N'Other', N'其他', N'90')
    ) AS v(code_field, code_value, code_desc, code_sort)
)
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
SELECT s.code_field, s.code_value, s.code_desc, s.code_sort
FROM seed s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.code_mstr c
    WHERE c.code_field = s.code_field AND c.code_value = s.code_value
);

;WITH langSeed AS (
    SELECT * FROM (VALUES
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'zh-TW', N'不在本月計薪範圍'),
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'en', N'Out of pay period'),
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'th', N'นอกช่วงจ่ายเงินเดือน'),
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'vi', N'Ngoài kỳ lương'),
        (N'EmpMonthExcludeReason', N'NotInPeriod', N'other', N'Out of pay period'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'zh-TW', N'停職不計薪'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'en', N'Suspension unpaid'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'th', N'พักงานไม่จ่าย'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'vi', N'Tạm đình chỉ không lương'),
        (N'EmpMonthExcludeReason', N'SuspendNoPay', N'other', N'Suspension unpaid'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'zh-TW', N'重複人員'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'en', N'Duplicate'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'th', N'ซ้ำ'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'vi', N'Trùng'),
        (N'EmpMonthExcludeReason', N'Duplicate', N'other', N'Duplicate'),
        (N'EmpMonthExcludeReason', N'DataError', N'zh-TW', N'資料錯誤'),
        (N'EmpMonthExcludeReason', N'DataError', N'en', N'Data error'),
        (N'EmpMonthExcludeReason', N'DataError', N'th', N'ข้อมูลผิด'),
        (N'EmpMonthExcludeReason', N'DataError', N'vi', N'Lỗi dữ liệu'),
        (N'EmpMonthExcludeReason', N'DataError', N'other', N'Data error'),
        (N'EmpMonthExcludeReason', N'Other', N'zh-TW', N'其他'),
        (N'EmpMonthExcludeReason', N'Other', N'en', N'Other'),
        (N'EmpMonthExcludeReason', N'Other', N'th', N'อื่นๆ'),
        (N'EmpMonthExcludeReason', N'Other', N'vi', N'Khác'),
        (N'EmpMonthExcludeReason', N'Other', N'other', N'Other'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'zh-TW', N'人工補入'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'en', N'Manual include'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'th', N'เพิ่มด้วยมือ'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'vi', N'Bổ sung thủ công'),
        (N'EmpMonthIncludeReason', N'ManualAdd', N'other', N'Manual include'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'zh-TW', N'月內復職'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'en', N'Rehire in month'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'th', N'Rehire ในเดือน'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'vi', N'Tái tuyển trong tháng'),
        (N'EmpMonthIncludeReason', N'RehireInMonth', N'other', N'Rehire in month'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'zh-TW', N'特殊計薪'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'en', N'Special pay'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'th', N'จ่ายพิเศษ'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'vi', N'Tính lương đặc biệt'),
        (N'EmpMonthIncludeReason', N'SpecialPay', N'other', N'Special pay'),
        (N'EmpMonthIncludeReason', N'Correction', N'zh-TW', N'糾錯納入'),
        (N'EmpMonthIncludeReason', N'Correction', N'en', N'Correction include'),
        (N'EmpMonthIncludeReason', N'Correction', N'th', N'แก้ไขเพิ่ม'),
        (N'EmpMonthIncludeReason', N'Correction', N'vi', N'Bổ sung sửa sai'),
        (N'EmpMonthIncludeReason', N'Correction', N'other', N'Correction include'),
        (N'EmpMonthIncludeReason', N'Other', N'zh-TW', N'其他'),
        (N'EmpMonthIncludeReason', N'Other', N'en', N'Other'),
        (N'EmpMonthIncludeReason', N'Other', N'th', N'อื่นๆ'),
        (N'EmpMonthIncludeReason', N'Other', N'vi', N'Khác'),
        (N'EmpMonthIncludeReason', N'Other', N'other', N'Other')
    ) AS v(code_field, code_value, culture, code_desc)
)
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
SELECT s.code_field, s.code_value, s.culture, s.code_desc
FROM langSeed s
WHERE EXISTS (SELECT 1 FROM dbo.code_mstr c WHERE c.code_field = s.code_field AND c.code_value = s.code_value)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.code_mstr_lang l
      WHERE l.code_field = s.code_field AND l.code_value = s.code_value AND l.culture = s.culture
  );
GO
