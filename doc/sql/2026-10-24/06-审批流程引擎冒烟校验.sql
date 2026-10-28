/*
 * 冒烟：表存在、试点种子、关键索引
 * 退出码：由调用方根据结果行判断（本脚本不 RAISERROR，输出 FAIL/PASS 列）
 */
SET NOCOUNT ON;

SELECT N'table' AS check_group, t.name AS check_name,
       CASE WHEN OBJECT_ID(N'dbo.' + t.name, N'U') IS NOT NULL THEN N'PASS' ELSE N'FAIL' END AS result
FROM (VALUES
    (N'hr_wf_level'),(N'hr_wf_role'),(N'hr_wf_role_scope'),(N'hr_wf_role_post'),
    (N'hr_wf_form'),(N'hr_wf_form_field'),(N'hr_wf_def'),(N'hr_wf_def_node'),
    (N'hr_wf_inst'),(N'hr_wf_task'),(N'hr_wf_action_log')
) AS t(name);

SELECT N'index' AS check_group, N'UX_hr_wf_inst_running' AS check_name,
       CASE WHEN EXISTS (
           SELECT 1 FROM sys.indexes WHERE name = N'UX_hr_wf_inst_running'
             AND object_id = OBJECT_ID(N'dbo.hr_wf_inst')
       ) THEN N'PASS' ELSE N'FAIL' END AS result;

SELECT N'seed' AS check_group, N'level_1030' AS check_name,
       CASE WHEN EXISTS (SELECT 1 FROM dbo.hr_wf_level WHERE code=N'1030' AND IsDelete=0)
            THEN N'PASS' ELSE N'FAIL' END AS result
UNION ALL
SELECT N'seed', N'level_1025',
       CASE WHEN EXISTS (SELECT 1 FROM dbo.hr_wf_level WHERE code=N'1025' AND IsDelete=0)
            THEN N'PASS' ELSE N'FAIL' END
UNION ALL
SELECT N'seed', N'form_hrWfGeneral',
       CASE WHEN EXISTS (SELECT 1 FROM dbo.hr_wf_form WHERE biz_type=N'hrWfGeneral' AND enabled=1 AND IsDelete=0)
            THEN N'PASS' ELSE N'FAIL' END
UNION ALL
SELECT N'seed', N'def_hrWfGeneral',
       CASE WHEN EXISTS (SELECT 1 FROM dbo.hr_wf_def WHERE biz_type=N'hrWfGeneral' AND enabled=1 AND IsDelete=0)
            THEN N'PASS' ELSE N'FAIL' END
UNION ALL
SELECT N'seed', N'def_nodes_ge_2',
       CASE WHEN (
           SELECT COUNT(1) FROM dbo.hr_wf_def_node n
           INNER JOIN dbo.hr_wf_def d ON d.Id=n.def_id AND d.biz_type=N'hrWfGeneral' AND d.IsDelete=0
           WHERE n.IsDelete=0
       ) >= 2 THEN N'PASS' ELSE N'FAIL' END
UNION ALL
SELECT N'seed', N'form_fields_ge_5',
       CASE WHEN (
           SELECT COUNT(1) FROM dbo.hr_wf_form_field f
           INNER JOIN dbo.hr_wf_form fm ON fm.Id=f.form_id AND fm.biz_type=N'hrWfGeneral' AND fm.IsDelete=0
           WHERE f.IsDelete=0
       ) >= 5 THEN N'PASS' ELSE N'FAIL' END;

SELECT N'menu' AS check_group, N'wf_menus' AS check_name,
       CASE WHEN (SELECT COUNT(1) FROM SysMenu WHERE Id BETWEEN 1300000000280 AND 1300000000306 AND IsDelete=0) >= 20
            THEN N'PASS' ELSE N'FAIL' END AS result;
