/*
 * hr_emp_suspend 补 EntityBase.DeleteTime（与离职/复职表一致）
 * 日期：2026-11-10  幂等：是
 */
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.hr_emp_suspend', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_suspend', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_suspend ADD DeleteTime datetime NULL;
