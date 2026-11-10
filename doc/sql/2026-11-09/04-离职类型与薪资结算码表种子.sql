/*
 * 变更说明：离职类型 LeaveType、薪资结算 LeaveSalarySettle 码表种子（含多语）
 * 日期：2026-11-09
 * 影响库：主库 code_mstr / code_mstr_lang
 * 是否幂等：是（仅插入缺失项）
 * 维护入口：离职管理 → 离职原因设置（页签）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

/* ---------- LeaveType 离职类型 ---------- */
;WITH seed AS (
    SELECT * FROM (VALUES
        (N'Resign',     N'辞职',       N'10'),
        (N'Dismiss',    N'辞退',       N'20'),
        (N'Contract',   N'合同到期',   N'30'),
        (N'Retire',     N'退休',       N'40'),
        (N'Mutual',     N'协商解除',   N'50'),
        (N'Other',      N'其他',       N'90')
    ) AS v(code_value, code_desc, code_sort)
)
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
SELECT N'LeaveType', s.code_value, s.code_desc, s.code_sort
FROM seed s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.code_mstr c
    WHERE c.code_field = N'LeaveType' AND c.code_value = s.code_value
);

;WITH langSeed AS (
    SELECT * FROM (VALUES
        (N'Resign',   N'zh-TW', N'辭職'),
        (N'Resign',   N'en',    N'Resignation'),
        (N'Resign',   N'th',    N'ลาออก'),
        (N'Resign',   N'vi',    N'Nghỉ việc'),
        (N'Resign',   N'other', N'Resignation'),
        (N'Dismiss',  N'zh-TW', N'辭退'),
        (N'Dismiss',  N'en',    N'Dismissal'),
        (N'Dismiss',  N'th',    N'เลิกจ้าง'),
        (N'Dismiss',  N'vi',    N'Sa thải'),
        (N'Dismiss',  N'other', N'Dismissal'),
        (N'Contract', N'zh-TW', N'合約到期'),
        (N'Contract', N'en',    N'Contract expired'),
        (N'Contract', N'th',    N'สัญญาหมดอายุ'),
        (N'Contract', N'vi',    N'Hết hạn hợp đồng'),
        (N'Contract', N'other', N'Contract expired'),
        (N'Retire',   N'zh-TW', N'退休'),
        (N'Retire',   N'en',    N'Retirement'),
        (N'Retire',   N'th',    N'เกษียณ'),
        (N'Retire',   N'vi',    N'Về hưu'),
        (N'Retire',   N'other', N'Retirement'),
        (N'Mutual',   N'zh-TW', N'協商解除'),
        (N'Mutual',   N'en',    N'Mutual termination'),
        (N'Mutual',   N'th',    N'ยกเลิกโดยความตกลง'),
        (N'Mutual',   N'vi',    N'Thỏa thuận chấm dứt'),
        (N'Mutual',   N'other', N'Mutual termination'),
        (N'Other',    N'zh-TW', N'其他'),
        (N'Other',    N'en',    N'Other'),
        (N'Other',    N'th',    N'อื่นๆ'),
        (N'Other',    N'vi',    N'Khác'),
        (N'Other',    N'other', N'Other')
    ) AS v(code_value, culture, code_desc)
)
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
SELECT N'LeaveType', s.code_value, s.culture, s.code_desc
FROM langSeed s
WHERE EXISTS (SELECT 1 FROM dbo.code_mstr c WHERE c.code_field = N'LeaveType' AND c.code_value = s.code_value)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.code_mstr_lang l
      WHERE l.code_field = N'LeaveType' AND l.code_value = s.code_value AND l.culture = s.culture
  );

/* ---------- LeaveSalarySettle 薪资结算 ---------- */
;WITH seed AS (
    SELECT * FROM (VALUES
        (N'NormalPay',  N'手续正常发薪',   N'10'),
        (N'StopPay',    N'手续未办停薪',   N'20'),
        (N'AutoNoPay',  N'自动离职无薪',   N'30')
    ) AS v(code_value, code_desc, code_sort)
)
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
SELECT N'LeaveSalarySettle', s.code_value, s.code_desc, s.code_sort
FROM seed s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.code_mstr c
    WHERE c.code_field = N'LeaveSalarySettle' AND c.code_value = s.code_value
);

;WITH langSeed AS (
    SELECT * FROM (VALUES
        (N'NormalPay', N'zh-TW', N'手續正常發薪'),
        (N'NormalPay', N'en',    N'Normal payroll'),
        (N'NormalPay', N'th',    N'จ่ายเงินเดือนปกติ'),
        (N'NormalPay', N'vi',    N'Trả lương bình thường'),
        (N'NormalPay', N'other', N'Normal payroll'),
        (N'StopPay',   N'zh-TW', N'手續未辦停薪'),
        (N'StopPay',   N'en',    N'Suspend pay (pending formalities)'),
        (N'StopPay',   N'th',    N'ระงับเงินเดือน'),
        (N'StopPay',   N'vi',    N'Dừng trả lương (thiếu thủ tục)'),
        (N'StopPay',   N'other', N'Suspend pay'),
        (N'AutoNoPay', N'zh-TW', N'自動離職無薪'),
        (N'AutoNoPay', N'en',    N'Auto leave without pay'),
        (N'AutoNoPay', N'th',    N'ลาออกอัตโนมัติไม่จ่าย'),
        (N'AutoNoPay', N'vi',    N'Tự động nghỉ không lương'),
        (N'AutoNoPay', N'other', N'Auto leave without pay')
    ) AS v(code_value, culture, code_desc)
)
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
SELECT N'LeaveSalarySettle', s.code_value, s.culture, s.code_desc
FROM langSeed s
WHERE EXISTS (SELECT 1 FROM dbo.code_mstr c WHERE c.code_field = N'LeaveSalarySettle' AND c.code_value = s.code_value)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.code_mstr_lang l
      WHERE l.code_field = N'LeaveSalarySettle' AND l.code_value = s.code_value AND l.culture = s.culture
  );

SELECT code_field, code_value, code_desc, code_sort
FROM dbo.code_mstr
WHERE code_field IN (N'LeaveType', N'LeaveSalarySettle')
ORDER BY code_field, code_sort, code_value;
