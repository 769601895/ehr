/*
 * 变更说明：ehr 主库创建人员资料表 h_emp_mstr（组织模块依赖列）
 * 模块：hrOrg（占用校验 / 合并调部门）
 * 日期：2026-10-10
 * 关联：doc/分析文档/组织架构/migration-plan/domains/01-domain-hr-org.md
 * 影响库：ehr
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 约定：
 *   1. 表名 dbo.h_emp_mstr（不用 employee）。
 *   2. 部门字段 cc_code，逻辑对应 cc_mstr.cc_code（不建 FK）。
 *   3. 本脚本仅组织依赖列；员工模块补全其余列时禁止改名 cc_code。
 *   4. 列宽：emp_id/cc_code NVARCHAR(50)，emp_name NVARCHAR(100)。
 *   5. 本脚本为新系统建表，非「迁旧库」工具。
 */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

IF OBJECT_ID(N'dbo.h_emp_mstr', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_emp_mstr
    (
        emp_id     NVARCHAR(50)  NOT NULL,              -- 工号（业务主键）
        cc_code    NVARCHAR(50)  NULL,                 -- 所属部门 = cc_mstr.cc_code
        emp_name   NVARCHAR(100) NULL,                 -- 姓名
        emp_state  INT           NULL
            CONSTRAINT DF_h_emp_mstr_emp_state DEFAULT (1), -- 默认在职
        emp_free   TINYINT       NULL
            CONSTRAINT DF_h_emp_mstr_emp_free DEFAULT (0),
        bs_id      INT           NOT NULL IDENTITY(1, 1),
        CONSTRAINT PK_h_emp_mstr PRIMARY KEY CLUSTERED (emp_id)
    );

    CREATE INDEX IX_h_emp_mstr_cc_code ON dbo.h_emp_mstr (cc_code);
    CREATE INDEX IX_h_emp_mstr_emp_state ON dbo.h_emp_mstr (emp_state);
    CREATE UNIQUE INDEX UX_h_emp_mstr_bs_id ON dbo.h_emp_mstr (bs_id);
END;

COMMIT TRAN;

SELECT c.column_id, c.name, ty.name AS type_name, c.max_length, c.is_nullable
FROM sys.columns c
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.h_emp_mstr')
ORDER BY c.column_id;

/*
 * ROLLBACK：
 * BEGIN TRAN;
 * DROP TABLE IF EXISTS dbo.h_emp_mstr;
 * COMMIT TRAN;
 */
