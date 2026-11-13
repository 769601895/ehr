/*
 * 变更说明：人事月报表补 EntityBase.DeleteTime（开批 Insert 会写该列）
 * 日期：2026-11-11
 * 前置：11-hr_emp_month建表.sql
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_emp_month_batch', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_month_batch', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_month_batch ADD DeleteTime DATETIME NULL;
GO

IF OBJECT_ID(N'dbo.hr_emp_month_line', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.hr_emp_month_line', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_emp_month_line ADD DeleteTime DATETIME NULL;
GO
