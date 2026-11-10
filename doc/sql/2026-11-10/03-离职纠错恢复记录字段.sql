/*
 * 变更说明：hr_emp_leave_restore 增加关联离职单字段，用于纠错恢复审计记录
 * 日期：2026-11-10
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'leave_id') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD leave_id bigint NULL;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'leave_no') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD leave_no nvarchar(50) NULL;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'operate_user_id') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD operate_user_id bigint NULL;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'operate_user_name') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD operate_user_name nvarchar(64) NULL;

IF COL_LENGTH(N'dbo.hr_emp_leave_restore', N'operate_time') IS NULL
    ALTER TABLE dbo.hr_emp_leave_restore ADD operate_time datetime NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_hr_emp_leave_restore_leave' AND object_id = OBJECT_ID(N'dbo.hr_emp_leave_restore'))
    CREATE INDEX IX_hr_emp_leave_restore_leave ON dbo.hr_emp_leave_restore (leave_id) WHERE IsDelete = 0;
