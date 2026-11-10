/*
 * 变更说明：hr_emp_leave_restore 增加单据种类；菜单改名为复职管理
 * kind: Reinstate=同工号复职；Correct=离职纠错审计（由离职撤销写入）
 * 日期：2026-11-10
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'kind') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD kind nvarchar(20) NULL;

-- 历史纠错审计（有 leave_id）标为 Correct；其余标为 Reinstate
UPDATE dbo.hr_emp_leave_restore
SET kind = N'Correct'
WHERE leave_id IS NOT NULL AND (kind IS NULL OR kind = N'');

UPDATE dbo.hr_emp_leave_restore
SET kind = N'Reinstate'
WHERE leave_id IS NULL AND (kind IS NULL OR kind = N'');

-- 菜单：员工离职恢复 → 复职管理（保持 Id/权限码不变，避免改授权）
UPDATE dbo.SysMenu
SET Title = N'复职管理', UpdateTime = GETDATE()
WHERE Id = 1300000000413;

UPDATE dbo.SysMenu
SET Title = N'复职管理', UpdateTime = GETDATE()
WHERE Title = N'员工离职恢复' OR Title = N'离职纠错恢复';
