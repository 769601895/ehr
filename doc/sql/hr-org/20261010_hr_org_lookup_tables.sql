/*
 * 变更说明：ehr 主库创建组织模块下拉依赖表
 * 模块：hrOrg
 * 日期：2026-10-10
 * 影响库：ehr
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 表：
 *   code_mstr   — 人事代码（厂区 / 成本类型 / 统计分类等）
 *   h_cost      — 成本中心
 *   h_shift_set — 考勤组别
 */

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.code_mstr', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.code_mstr
    (
        code_field NVARCHAR(50) NOT NULL,
        code_value NVARCHAR(50) NOT NULL,
        code_desc  NVARCHAR(50) NULL,
        code_sort  NVARCHAR(50) NULL,
        CONSTRAINT PK_code_mstr PRIMARY KEY CLUSTERED (code_field, code_value)
    );
END;

IF OBJECT_ID(N'dbo.h_cost', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_cost
    (
        cost_id    NVARCHAR(50)  NOT NULL,
        cost_name  NVARCHAR(100) NULL,
        cost_site  NVARCHAR(50)  NULL,
        cost_state NVARCHAR(10)  NULL CONSTRAINT DF_h_cost_state DEFAULT (N'Y'),
        CONSTRAINT PK_h_cost PRIMARY KEY CLUSTERED (cost_id)
    );
END;

IF OBJECT_ID(N'dbo.h_shift_set', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_shift_set
    (
        shift_id      NVARCHAR(50)  NOT NULL,
        shift_desc    NVARCHAR(100) NULL,
        shift_site    NVARCHAR(50)  NULL,
        shift_tpksite NVARCHAR(50)  NULL,
        CONSTRAINT PK_h_shift_set PRIMARY KEY CLUSTERED (shift_id)
    );
END;

-- 基础代码（幂等）
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_site' AND code_value = N'VN')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_site', N'VN', N'越南', N'01');
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_site' AND code_value = N'CN')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_site', N'CN', N'中国', N'02');
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_costtype' AND code_value = N'DL')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_costtype', N'DL', N'直接人工', N'01');
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_costtype' AND code_value = N'IDL')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_costtype', N'IDL', N'间接人工', N'02');
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_countbasic' AND code_value = N'A')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_countbasic', N'A', N'生产', N'01');
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field = N'cc_countbasic' AND code_value = N'B')
    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)
    VALUES (N'cc_countbasic', N'B', N'管理', N'02');

SELECT 'code_mstr' AS tbl, COUNT(*) AS cnt FROM dbo.code_mstr
UNION ALL SELECT 'h_cost', COUNT(*) FROM dbo.h_cost
UNION ALL SELECT 'h_shift_set', COUNT(*) FROM dbo.h_shift_set;
