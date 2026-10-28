/*
 * 变更说明：初始化岗位代码设置（hr_post）样例数据
 * 模块：hrPost
 * 日期：2026-10-28
 * 影响库：ehrnew
 * 是否幂等：是（按 post_code MERGE）
 * 前置：02-岗位代码建表.sql；部门：总经办=000002，管理部=000003，人力资源部=000017
 *
 * 说明：Id 使用 1300000003201~3208
 * 执行：sqlcmd -S jack\sql2014 -U sa -P *** -d ehrnew -f 65001 -i 本文件
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_post', N'U') IS NULL
    THROW 50001, N'hr_post 不存在，请先执行 02-岗位代码建表.sql', 1;

DECLARE @now DATETIME = GETDATE();

MERGE dbo.hr_post AS t
USING (
    SELECT *
    FROM (VALUES
        -- Id, post_code, post_name, cc_code（部门）
        (CAST(1300000003201 AS BIGINT), N'000001', N'总经理',         N'000002'), -- 总经办
        (CAST(1300000003202 AS BIGINT), N'000002', N'管理部经理',     N'000003'), -- 管理部
        (CAST(1300000003203 AS BIGINT), N'000003', N'管理部课长',     N'000003'), -- 管理部
        (CAST(1300000003204 AS BIGINT), N'000004', N'管理部职员',     N'000003'), -- 管理部
        (CAST(1300000003205 AS BIGINT), N'000005', N'人事专员',       N'000017'), -- 人力资源部
        (CAST(1300000003206 AS BIGINT), N'000006', N'人力资源部经理', N'000017'), -- 人力资源部
        (CAST(1300000003207 AS BIGINT), N'000007', N'人力资源部课长', N'000017'), -- 人力资源部
        (CAST(1300000003208 AS BIGINT), N'000008', N'人力资源部总监', N'000017')  -- 人力资源部
    ) AS v(Id, post_code, post_name, cc_code)
) AS s
ON t.post_code = s.post_code AND ISNULL(t.IsDelete, 0) = 0
WHEN MATCHED THEN
    UPDATE SET
        post_name = s.post_name,
        cc_code = s.cc_code,
        status = N'Active',
        need_audit = 1,
        UpdateTime = @now
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, post_code, post_name, grade, status, cc_code, need_audit, remark, CreateTime, IsDelete)
    VALUES (s.Id, s.post_code, s.post_name, 0, N'Active', s.cc_code, 1, NULL, @now, 0);

SELECT p.post_code, p.post_name, p.cc_code, c.cc_desc AS dept_name, p.status
FROM dbo.hr_post p
LEFT JOIN dbo.cc_mstr c ON c.cc_code = p.cc_code
WHERE p.IsDelete = 0
  AND p.post_code IN (N'000001', N'000002', N'000003', N'000004', N'000005', N'000006', N'000007', N'000008')
ORDER BY p.post_code;

PRINT N'hr_post 岗位代码种子已同步';
GO
