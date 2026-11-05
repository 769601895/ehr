/*
 * 补 EntityBase.DeleteTime（软删过滤会引用该列）
 * 前置：已执行 10-员工资料导入建表.sql
 * 库：HR
 * 幂等：列不存在才加
 */

IF OBJECT_ID(N'dbo.hr_emp_import_tpl', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_import_tpl', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_import_tpl ADD DeleteTime DATETIME NULL;
GO

IF OBJECT_ID(N'dbo.hr_emp_import_tpl_field', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_import_tpl_field', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_import_tpl_field ADD DeleteTime DATETIME NULL;
GO

IF OBJECT_ID(N'dbo.hr_emp_import_batch', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_import_batch', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_import_batch ADD DeleteTime DATETIME NULL;
GO

IF OBJECT_ID(N'dbo.hr_emp_import_batch_row', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_import_batch_row', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_import_batch_row ADD DeleteTime DATETIME NULL;
GO
