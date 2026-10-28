/*
 * 变更说明：清空现有审核等级，按旧系统「审核流程级别定义」重建
 * 模块：hrWf
 * 日期：2026-10-24
 * 影响库：ehrnew（HR 库）
 * 是否幂等：是（目标 code upsert；其余软删；节点引用可重复执行）
 * 说明：
 *   - 非目标等级（如试点 deptLead/divMgr）软删
 *   - 同步把试点流程节点 levelId 映射到 1030/1025
 */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @now DATETIME = GETDATE();

DECLARE @levels TABLE
(
    Id            BIGINT         NOT NULL,
    code          NVARCHAR(50)   NOT NULL,
    name          NVARCHAR(100)  NOT NULL,
    audit_order   INT            NOT NULL,
    is_functional BIT            NOT NULL,
    order_no      INT            NOT NULL
);

INSERT INTO @levels (Id, code, name, audit_order, is_functional, order_no) VALUES
    (13000000051010, N'1010', N'总经理',   6,  0, 60),
    (13000000051012, N'1012', N'营运经理', 5,  0, 50),
    (13000000051015, N'1015', N'总监',     4,  0, 40),
    (13000000051020, N'1020', N'部门经理', 3,  0, 30),
    (13000000051025, N'1025', N'分部经理', 2,  0, 20),
    (13000000051030, N'1030', N'部门主管', 1,  0, 10),
    (13000000051035, N'1035', N'考勤专员', 99, 1, 99),
    (13000000051040, N'1040', N'职员',     0,  0, 0),
    (13000000051050, N'1050', N'HR经理',   99, 1, 100);

/* ---------- 软删非目标等级 ---------- */
UPDATE L
SET L.IsDelete = 1, L.DeleteTime = @now, L.UpdateTime = @now
FROM dbo.hr_wf_level AS L
WHERE L.IsDelete = 0
  AND NOT EXISTS (SELECT 1 FROM @levels AS T WHERE T.code = L.code);

/* ---------- 目标等级 upsert ---------- */
UPDATE L
SET
    L.name = T.name,
    L.audit_order = T.audit_order,
    L.is_functional = T.is_functional,
    L.order_no = T.order_no,
    L.enabled = 1,
    L.IsDelete = 0,
    L.DeleteTime = NULL,
    L.UpdateTime = @now
FROM dbo.hr_wf_level AS L
INNER JOIN @levels AS T ON T.code = L.code;

INSERT INTO dbo.hr_wf_level
    (Id, code, name, audit_order, is_functional, order_no, enabled, remark, CreateTime, IsDelete)
SELECT
    T.Id, T.code, T.name, T.audit_order, T.is_functional, T.order_no, 1, NULL, @now, 0
FROM @levels AS T
WHERE NOT EXISTS (SELECT 1 FROM dbo.hr_wf_level AS L WHERE L.code = T.code);

/* ---------- 试点流程节点 levelId 映射 ---------- */
UPDATE dbo.hr_wf_def_node
SET assignee_rule_json = N'{"type":"Level","levelId":13000000051030}', UpdateTime = @now
WHERE Id = 1300000004021 AND IsDelete = 0;

UPDATE dbo.hr_wf_def_node
SET assignee_rule_json = N'{"type":"Level","levelId":13000000051025}', UpdateTime = @now
WHERE Id = 1300000004022 AND IsDelete = 0;

COMMIT;

SELECT Id, code, name, audit_order, is_functional, order_no, enabled
FROM dbo.hr_wf_level
WHERE IsDelete = 0
ORDER BY audit_order, code;
