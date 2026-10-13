/*
 * 变更说明：ehr 主库创建人事部门主档 cc_mstr
 * 模块：hrOrg
 * 日期：2026-10-10
 * 关联：doc/分析文档/组织架构/migration-plan/domains/01-domain-hr-org.md
 * 影响库：ehr
 * 是否幂等：是（IF NOT EXISTS）
 *
 * 约定：
 *   1. 表名 dbo.cc_mstr；字段为业务字典约定列（32 列）。
 *   2. 不建 OrgId；不含 cc_kq_wocachange / cc_kq_code / cc_kq_control /
 *      cc_kq_edituser / cc_kq_editdate。
 *   3. 与框架 SysOrg 无关。
 *   4. 业务主键 cc_code；ID 为自增辅助键。
 *   5. 停用：cc_enable（0 启用 / 1 停用）。
 *   6. 树：cc_level / ParentID / cc_code_parent。
 *   7. 本脚本为新系统建表，非「迁旧库」工具。
 */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

IF OBJECT_ID(N'dbo.cc_mstr', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.cc_mstr
    (
        cc_code          NVARCHAR(50)   NOT NULL,       -- 部门编码（业务主键）
        cc_code_parent   NVARCHAR(50)   NULL,           -- 父部门编码
        cc_dept          NVARCHAR(50)   NULL,           -- 部门代号
        cc_desc          NVARCHAR(122)  NOT NULL,       -- 部门名称
        cc_level         NVARCHAR(50)   NOT NULL,       -- 层级码（每层 2 位）
        cc_group         INT            NULL,           -- 部门组
        cc_indet         NVARCHAR(50)   NULL,           -- 缩进/标识
        cc_sub           FLOAT          NULL,           -- 下属标志
        cc_site          NVARCHAR(50)   NULL,           -- 厂区
        cc_state         INT            NULL,           -- 状态码
        cc_cost          NVARCHAR(50)   NULL,           -- 成本类型
        cc_wage_rate     FLOAT          NULL,           -- 工资率
        cc_stat_basic    NVARCHAR(50)   NULL,           -- 统计分类
        cc_enable        INT            NULL
            CONSTRAINT DF_cc_mstr_cc_enable DEFAULT (0), -- 0启用/1停用
        cc_dele_date     DATETIME       NULL,           -- 删除日期
        cc_count_group   NVARCHAR(60)   NULL,           -- 统计组
        cc_user          NVARCHAR(50)   NULL,           -- 维护人
        cc_date          DATETIME       NULL,           -- 维护时间
        cc_kqgroup       INT            NULL,           -- 考勤组
        cc_woca          NVARCHAR(50)   NULL,           -- 工时类别
        cc_bz_unit       NVARCHAR(50)   NULL,           -- 编制单位
        cc_dele_remark   NVARCHAR(120)  NULL,           -- 删除备注
        cc_desc_en       NVARCHAR(150)  NULL,           -- 英文名称
        cc_level_censor  NVARCHAR(50)   NULL,           -- 审核级别
        cc_xz_group      NVARCHAR(50)   NULL,           -- 薪酬组
        cc_create_date   DATETIME       NULL,           -- 创建日期
        machine_floor    NVARCHAR(10)   NULL,           -- 楼层
        kq_new_sysuser   NVARCHAR(25)   NULL,           -- 考勤同步人
        kq_new_sysdate   DATETIME       NULL,           -- 考勤同步时间
        ID               INT            NOT NULL IDENTITY(1, 1), -- 自增辅助键
        ParentID         INT            NULL,           -- 父节点辅助 Id
        cc_code2         NVARCHAR(50)   NULL,           -- 辅助编码
        CONSTRAINT PK_cc_mstr PRIMARY KEY CLUSTERED (cc_code)
    );

    CREATE UNIQUE INDEX UX_cc_mstr_ID ON dbo.cc_mstr (ID);
    CREATE INDEX IX_cc_mstr_cc_level ON dbo.cc_mstr (cc_level);
    CREATE INDEX IX_cc_mstr_ParentID ON dbo.cc_mstr (ParentID);
    CREATE INDEX IX_cc_mstr_cc_enable ON dbo.cc_mstr (cc_enable);
    CREATE INDEX IX_cc_mstr_cc_code_parent ON dbo.cc_mstr (cc_code_parent);
END;

COMMIT TRAN;

-- 校验
SELECT c.column_id, c.name, ty.name AS type_name, c.max_length, c.is_nullable
FROM sys.columns c
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.cc_mstr')
ORDER BY c.column_id;

/*
 * ROLLBACK：
 *
 * BEGIN TRAN;
 * DROP TABLE IF EXISTS dbo.cc_mstr;
 * COMMIT TRAN;
 */
