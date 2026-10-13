/*
 * 变更说明：组织架构左树演示数据（对齐图1部门层级与人数）
 * 模块：hrOrg
 * 日期：2026-10-10
 * 影响库：ehr
 * 是否幂等：是（按约定编码清理后重插）
 * 注意：仅开发/联调演示，非正式业务灌数。
 */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

-- 清理本演示批次（及早期测试脏数据）
DELETE FROM dbo.h_emp_mstr WHERE emp_id LIKE N'DEMO%';
DELETE FROM dbo.cc_mstr WHERE cc_code IN (
    N'000001', N'000002', N'000003', N'000004', N'000005',
    N'000006', N'000007', N'000008', N'000009', N'000010',
    N'000011', N'000012', N'000013', N'000014', N'000015'
);

DECLARE @now DATETIME = GETDATE();

-- 根：易维
INSERT INTO dbo.cc_mstr (cc_code, cc_code_parent, cc_dept, cc_desc, cc_level, cc_enable, cc_user, cc_date, cc_create_date, ParentID)
VALUES (N'000001', NULL, N'YW', N'易维', N'01', 0, N'demo', @now, @now, 0);

DECLARE @rootId INT = (SELECT ID FROM dbo.cc_mstr WHERE cc_code = N'000001');

-- 一级部门（代号对齐图1右侧）
INSERT INTO dbo.cc_mstr (cc_code, cc_code_parent, cc_dept, cc_desc, cc_level, cc_enable, cc_user, cc_date, cc_create_date, ParentID) VALUES
(N'000002', N'000001', N'001', N'总经办',       N'0101', 0, N'demo', @now, @now, @rootId),
(N'000003', N'000001', N'002', N'管理部',       N'0102', 0, N'demo', @now, @now, @rootId),
(N'000004', N'000001', N'003', N'资材部',       N'0103', 0, N'demo', @now, @now, @rootId),
(N'000005', N'000001', N'004', N'品管部',       N'0104', 0, N'demo', @now, @now, @rootId),
(N'000006', N'000001', N'005', N'工艺工程部',   N'0105', 0, N'demo', @now, @now, @rootId),
(N'000007', N'000001', N'006', N'财务部',       N'0106', 0, N'demo', @now, @now, @rootId),
(N'000008', N'000001', N'009', N'制造部',       N'0107', 0, N'demo', @now, @now, @rootId),
(N'000009', N'000001', N'010', N'关务部',       N'0108', 0, N'demo', @now, @now, @rootId),
(N'000010', N'000001', N'011', N'仓库部',       N'0109', 0, N'demo', @now, @now, @rootId);

DECLARE @zcId INT = (SELECT ID FROM dbo.cc_mstr WHERE cc_code = N'000004');
DECLARE @zzId INT = (SELECT ID FROM dbo.cc_mstr WHERE cc_code = N'000008');

-- 资材部 / 制造部 下级（便于树展开）
INSERT INTO dbo.cc_mstr (cc_code, cc_code_parent, cc_dept, cc_desc, cc_level, cc_enable, cc_user, cc_date, cc_create_date, ParentID) VALUES
(N'000011', N'000004', N'031', N'采购课', N'010301', 0, N'demo', @now, @now, @zcId),
(N'000012', N'000004', N'032', N'物控课', N'010302', 0, N'demo', @now, @now, @zcId),
(N'000013', N'000008', N'091', N'一车间', N'010701', 0, N'demo', @now, @now, @zzId),
(N'000014', N'000008', N'092', N'二车间', N'010702', 0, N'demo', @now, @now, @zzId),
(N'000015', N'000008', N'093', N'三车间', N'010703', 0, N'demo', @now, @now, @zzId);

-- 按图1人数生成在职人员（含子部门拆分后汇总对齐）
;WITH cnt(cc_code, n) AS (
    SELECT N'000002', 2  UNION ALL -- 总经办
    SELECT N'000003', 8  UNION ALL -- 管理部
    SELECT N'000004', 4  UNION ALL -- 资材部本级
    SELECT N'000011', 5  UNION ALL -- 采购课
    SELECT N'000012', 3  UNION ALL -- 物控课  => 资材部合计 12
    SELECT N'000005', 11 UNION ALL -- 品管部
    SELECT N'000006', 5  UNION ALL -- 工艺工程部
    SELECT N'000007', 3  UNION ALL -- 财务部
    SELECT N'000008', 15 UNION ALL -- 制造部本级
    SELECT N'000013', 80 UNION ALL -- 一车间
    SELECT N'000014', 70 UNION ALL -- 二车间
    SELECT N'000015', 50 UNION ALL -- 三车间 => 制造部合计 215
    SELECT N'000009', 3  UNION ALL -- 关务部
    SELECT N'000010', 3            -- 仓库部
),
nums AS (
    SELECT TOP (300) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
),
rows AS (
    SELECT c.cc_code, n.i,
           ROW_NUMBER() OVER (ORDER BY c.cc_code, n.i) AS rn
    FROM cnt c
    INNER JOIN nums n ON n.i <= c.n
)
INSERT INTO dbo.h_emp_mstr (iempid, emp_id, cc_code, emp_name, emp_state, emp_free)
SELECT
    900000 + rn,
    N'DEMO' + RIGHT(N'0000' + CAST(rn AS NVARCHAR(10)), 4),
    cc_code,
    N'演示' + CAST(i AS NVARCHAR(10)),
    1,
    0
FROM rows;

COMMIT TRAN;

-- 校验：一级部门直接+下级人数
SELECT
    p.cc_dept AS dept_code,
    p.cc_desc,
    (
        SELECT COUNT(*)
        FROM dbo.h_emp_mstr e
        INNER JOIN dbo.cc_mstr d ON d.cc_code = e.cc_code
        WHERE d.cc_level LIKE p.cc_level + N'%'
          AND ISNULL(e.emp_state, 1) = 1
    ) AS emp_cnt
FROM dbo.cc_mstr p
WHERE p.cc_code_parent = N'000001'
ORDER BY p.cc_level;
