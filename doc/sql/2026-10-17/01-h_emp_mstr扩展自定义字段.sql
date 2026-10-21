/*
 * 变更说明：h_emp_mstr 扩展自定义字段
 *   - emp_char19 ~ emp_char100  → nvarchar(50) NULL（已有 1~18）
 *   - emp_date4  ~ emp_date10   → datetime NULL（已有 1~3）
 *   - emp_item4  ~ emp_item10   → decimal(9,2) NULL（已有 1~3）
 * 目标库：ehr
 * 说明：默认为空（NULL，无 DEFAULT）；与既有列一致。
 * 幂等：可重复执行，已存在列跳过。
 */
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.h_emp_mstr', N'U') IS NULL
BEGIN
    RAISERROR(N'dbo.h_emp_mstr 不存在，终止。', 16, 1);
    RETURN;
END;

DECLARE @sql nvarchar(max) = N'';
DECLARE @i int;
DECLARE @col sysname;

-- emp_char19 ~ emp_char100
SET @i = 19;
WHILE @i <= 100
BEGIN
    SET @col = N'emp_char' + CAST(@i AS nvarchar(10));
    IF COL_LENGTH(N'dbo.h_emp_mstr', @col) IS NULL
        SET @sql += N'ALTER TABLE dbo.h_emp_mstr ADD ' + QUOTENAME(@col) + N' nvarchar(50) NULL;' + CHAR(13);
    SET @i += 1;
END;

-- emp_date4 ~ emp_date10
SET @i = 4;
WHILE @i <= 10
BEGIN
    SET @col = N'emp_date' + CAST(@i AS nvarchar(10));
    IF COL_LENGTH(N'dbo.h_emp_mstr', @col) IS NULL
        SET @sql += N'ALTER TABLE dbo.h_emp_mstr ADD ' + QUOTENAME(@col) + N' datetime NULL;' + CHAR(13);
    SET @i += 1;
END;

-- emp_item4 ~ emp_item10
SET @i = 4;
WHILE @i <= 10
BEGIN
    SET @col = N'emp_item' + CAST(@i AS nvarchar(10));
    IF COL_LENGTH(N'dbo.h_emp_mstr', @col) IS NULL
        SET @sql += N'ALTER TABLE dbo.h_emp_mstr ADD ' + QUOTENAME(@col) + N' decimal(9,2) NULL;' + CHAR(13);
    SET @i += 1;
END;

IF LEN(@sql) > 0
BEGIN
    EXEC sys.sp_executesql @sql;
    PRINT N'已补充缺失列。';
END
ELSE
    PRINT N'无需变更：目标列均已存在。';

-- 校验期望：emp_char=100, emp_date=10, emp_item=10
SELECT
    SUM(CASE WHEN name LIKE N'emp_char[0-9]%' THEN 1 ELSE 0 END) AS emp_char_cnt,
    SUM(CASE WHEN name LIKE N'emp_date[0-9]%' THEN 1 ELSE 0 END) AS emp_date_cnt,
    SUM(CASE WHEN name LIKE N'emp_item[0-9]%' THEN 1 ELSE 0 END) AS emp_item_cnt
FROM sys.columns
WHERE object_id = OBJECT_ID(N'dbo.h_emp_mstr')
  AND (
      name LIKE N'emp_char[0-9]%'
      OR name LIKE N'emp_date[0-9]%'
      OR name LIKE N'emp_item[0-9]%'
  );
