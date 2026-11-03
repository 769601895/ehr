/*
 * 变更说明：按界面截图删除多余列
 * 删除：ProbationXferDays, ProbationUnit, SalaryTypeMaint, PayCategoryMaint, LateAsException, BankAccountMode
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 是否幂等：是
 */
SET NOCOUNT ON;

DECLARE @cols TABLE(name sysname, df sysname);
INSERT INTO @cols(name, df) VALUES
 (N'ProbationXferDays', N'DF_HrCompanyParam_ProbationXferDays'),
 (N'ProbationUnit', N'DF_HrCompanyParam_ProbationUnit'),
 (N'SalaryTypeMaint', N'DF_HrCompanyParam_SalaryTypeMaint'),
 (N'PayCategoryMaint', N'DF_HrCompanyParam_PayCategoryMaint'),
 (N'LateAsException', N'DF_HrCompanyParam_LateAsException'),
 (N'BankAccountMode', N'DF_HrCompanyParam_BankAccountMode');

DECLARE @name sysname, @df sysname, @sql nvarchar(400);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT name, df FROM @cols;
OPEN c;
FETCH NEXT FROM c INTO @name, @df;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = @df)
    BEGIN
        SET @sql = N'ALTER TABLE dbo.HrCompanyParam DROP CONSTRAINT ' + QUOTENAME(@df);
        EXEC sp_executesql @sql;
    END
    IF COL_LENGTH(N'dbo.HrCompanyParam', @name) IS NOT NULL
    BEGIN
        SET @sql = N'ALTER TABLE dbo.HrCompanyParam DROP COLUMN ' + QUOTENAME(@name);
        EXEC sp_executesql @sql;
    END
    FETCH NEXT FROM c INTO @name, @df;
END
CLOSE c; DEALLOCATE c;

DELETE FROM dbo.code_mstr_lang WHERE code_field IN (N'ProbationUnit', N'SalaryTypeMaint', N'PayCategoryMaint', N'LateAsException', N'BankAccountMode');
DELETE FROM dbo.code_mstr WHERE code_field IN (N'ProbationUnit', N'SalaryTypeMaint', N'PayCategoryMaint', N'LateAsException', N'BankAccountMode');
GO
