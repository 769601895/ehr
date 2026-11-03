/*
 * 变更说明：删除薪资参数 奖惩计算方式/奖惩金额维护方式/核定薪资计算
 * 删除列：RewardCalcMode, RewardAmountMaint, ApprovedSalaryCalc
 * 日期：2026-11-02
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @cols TABLE(name sysname, df sysname);
INSERT INTO @cols(name, df) VALUES
 (N'RewardCalcMode', N'DF_HrCompanyParam_RewardCalcMode'),
 (N'RewardAmountMaint', N'DF_HrCompanyParam_RewardAmountMaint'),
 (N'ApprovedSalaryCalc', N'DF_HrCompanyParam_ApprovedSalaryCalc');

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

DELETE FROM dbo.code_mstr_lang WHERE code_field IN (N'RewardCalcMode', N'RewardAmountMaint', N'ApprovedSalaryCalc');
DELETE FROM dbo.code_mstr WHERE code_field IN (N'RewardCalcMode', N'RewardAmountMaint', N'ApprovedSalaryCalc');
GO
