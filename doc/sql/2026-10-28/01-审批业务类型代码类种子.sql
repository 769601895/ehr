/*
 * 变更说明：审批引擎业务类型改为人事代码类维护（code_field = hr_wf_biz）
 * 模块：hrWf
 * 幂等：MERGE，可重复执行
 */
SET NOCOUNT ON;

MERGE dbo.code_mstr AS t
USING (
    SELECT *
    FROM (VALUES
        (N'hr_wf_biz', N'hrWfGeneral', N'通用申请', 10),
        (N'hr_wf_biz', N'hrEmh',       N'员工异动管理', 20)
    ) AS v(code_field, code_value, code_desc, code_sort)
) AS s
ON t.code_field = s.code_field AND t.code_value = s.code_value
WHEN MATCHED THEN
    UPDATE SET code_desc = s.code_desc, code_sort = s.code_sort
WHEN NOT MATCHED THEN
    INSERT (code_field, code_value, code_desc, code_sort)
    VALUES (s.code_field, s.code_value, s.code_desc, s.code_sort);

PRINT N'hr_wf_biz 业务类型代码已同步';
