/*
 * 变更说明：离职原因设置 = code_mstr(code_field=LeaveReason) + 菜单按钮权限（复用 hrCodeMstr API）
 * 日期：2026-11-09
 * 影响库：主库 code_mstr / code_mstr_lang / SysMenu / SysRoleMenu
 * 是否幂等：是
 * 前置：02-离职管理子模块空菜单.sql（菜单 1300000000411）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @menuId BIGINT = 1300000000411; -- 离职原因设置
DECLARE @roleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
BEGIN
    RAISERROR(N'菜单 1300000000411（离职原因设置）不存在，请先执行 02-离职管理子模块空菜单.sql', 16, 1);
    RETURN;
END

/* ---------- 按钮权限（Permission 与人事代码 API 一致） ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000414)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000414, @menuId, 3, N'hrLeaveReasonPage', N'查询', N'hrCodeMstr/page', 0, 0, 1, 0, 100, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @menuId, Permission = N'hrCodeMstr/page', Title = N'查询', IsDelete = 0, Status = 1 WHERE Id = 1300000000414;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000415)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000415, @menuId, 3, N'hrLeaveReasonSave', N'保存', N'hrCodeMstr/save', 0, 0, 1, 0, 110, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @menuId, Permission = N'hrCodeMstr/save', Title = N'保存', IsDelete = 0, Status = 1 WHERE Id = 1300000000415;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000416)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000416, @menuId, 3, N'hrLeaveReasonDelete', N'删除', N'hrCodeMstr/delete', 0, 0, 1, 0, 120, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @menuId, Permission = N'hrCodeMstr/delete', Title = N'删除', IsDelete = 0, Status = 1 WHERE Id = 1300000000416;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000417)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000417, @menuId, 3, N'hrLeaveReasonSaveLang', N'译文', N'hrCodeMstr/saveLang', 0, 0, 1, 0, 130, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @menuId, Permission = N'hrCodeMstr/saveLang', Title = N'译文', IsDelete = 0, Status = 1 WHERE Id = 1300000000417;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000418)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000418, @menuId, 3, N'hrLeaveReasonLangList', N'译文查询', N'hrCodeMstr/langList', 0, 0, 1, 0, 140, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu SET Pid = @menuId, Permission = N'hrCodeMstr/langList', Title = N'译文查询', IsDelete = 0, Status = 1 WHERE Id = 1300000000418;

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000004114, 1300000000414),
        (1300000004115, 1300000000415),
        (1300000004116, 1300000000416),
        (1300000004117, 1300000000417),
        (1300000004118, 1300000000418)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

/* ---------- LeaveReason 码表种子（仅插入缺失项） ---------- */
;WITH seed AS (
    SELECT * FROM (VALUES
        (N'Voluntary', N'自愿离职',   N'10'),
        (N'Resign',    N'辞职',       N'20'),
        (N'Dismiss',   N'辞退',       N'30'),
        (N'Contract',  N'合同到期',   N'40'),
        (N'Retire',    N'退休',       N'50'),
        (N'Mutual',    N'协商解除',   N'60'),
        (N'Other',     N'其他',       N'90')
    ) AS v(code_value, code_desc, code_sort)
)
INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
SELECT N'LeaveReason', s.code_value, s.code_desc, s.code_sort
FROM seed s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.code_mstr c
    WHERE c.code_field = N'LeaveReason' AND c.code_value = s.code_value
);

/* 译文：zh-TW / en / th / vi / other（有则跳过） */
;WITH langSeed AS (
    SELECT * FROM (VALUES
        (N'Voluntary', N'zh-TW', N'自願離職'),
        (N'Voluntary', N'en',    N'Voluntary resignation'),
        (N'Voluntary', N'th',    N'ลาออกโดยสมัครใจ'),
        (N'Voluntary', N'vi',    N'Tự nguyện nghỉ việc'),
        (N'Voluntary', N'other', N'Voluntary resignation'),
        (N'Resign',    N'zh-TW', N'辭職'),
        (N'Resign',    N'en',    N'Resignation'),
        (N'Resign',    N'th',    N'ลาออก'),
        (N'Resign',    N'vi',    N'Đơn phương nghỉ việc'),
        (N'Resign',    N'other', N'Resignation'),
        (N'Dismiss',   N'zh-TW', N'辭退'),
        (N'Dismiss',   N'en',    N'Dismissal'),
        (N'Dismiss',   N'th',    N'เลิกจ้าง'),
        (N'Dismiss',   N'vi',    N'Sa thải'),
        (N'Dismiss',   N'other', N'Dismissal'),
        (N'Contract',  N'zh-TW', N'合約到期'),
        (N'Contract',  N'en',    N'Contract expired'),
        (N'Contract',  N'th',    N'สัญญาหมดอายุ'),
        (N'Contract',  N'vi',    N'Hết hạn hợp đồng'),
        (N'Contract',  N'other', N'Contract expired'),
        (N'Retire',    N'zh-TW', N'退休'),
        (N'Retire',    N'en',    N'Retirement'),
        (N'Retire',    N'th',    N'เกษียณ'),
        (N'Retire',    N'vi',    N'Về hưu'),
        (N'Retire',    N'other', N'Retirement'),
        (N'Mutual',    N'zh-TW', N'協商解除'),
        (N'Mutual',    N'en',    N'Mutual termination'),
        (N'Mutual',    N'th',    N'ยกเลิกโดยความตกลง'),
        (N'Mutual',    N'vi',    N'Thỏa thuận chấm dứt'),
        (N'Mutual',    N'other', N'Mutual termination'),
        (N'Other',     N'zh-TW', N'其他'),
        (N'Other',     N'en',    N'Other'),
        (N'Other',     N'th',    N'อื่นๆ'),
        (N'Other',     N'vi',    N'Khác'),
        (N'Other',     N'other', N'Other')
    ) AS v(code_value, culture, code_desc)
)
INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)
SELECT N'LeaveReason', s.code_value, s.culture, s.code_desc
FROM langSeed s
WHERE EXISTS (SELECT 1 FROM dbo.code_mstr c WHERE c.code_field = N'LeaveReason' AND c.code_value = s.code_value)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.code_mstr_lang l
      WHERE l.code_field = N'LeaveReason' AND l.code_value = s.code_value AND l.culture = s.culture
  );

SELECT code_value, code_desc, code_sort FROM dbo.code_mstr WHERE code_field = N'LeaveReason' ORDER BY code_sort, code_value;
SELECT Id, Title, Permission FROM dbo.SysMenu WHERE Pid = @menuId OR Id = @menuId ORDER BY OrderNo, Id;
