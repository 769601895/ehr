/*
 * 变更说明：删除 日报请假显示/漏刷卡处理/加班计算标准
 * 删除列：DailyLeaveUnit, MissSwipeMode, OtCalcStandard
 * 日期：2026-11-02
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @cols TABLE(name sysname, df sysname);
INSERT INTO @cols(name, df) VALUES
 (N'DailyLeaveUnit', N'DF_HrCompanyParam_DailyLeaveUnit'),
 (N'MissSwipeMode', N'DF_HrCompanyParam_MissSwipeMode'),
 (N'OtCalcStandard', N'DF_HrCompanyParam_OtCalcStandard');

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

DELETE FROM dbo.code_mstr_lang WHERE code_field IN (N'DailyLeaveUnit', N'MissSwipeMode', N'OtCalcStandard');
DELETE FROM dbo.code_mstr WHERE code_field IN (N'DailyLeaveUnit', N'MissSwipeMode', N'OtCalcStandard');
GO
