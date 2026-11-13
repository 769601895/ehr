/*
 * 变更说明：hrEmp 场景 monthSnap 常用字段种子
 * 日期：2026-11-11
 * 影响库：HR 库 hr_ui_field_scene
 * 是否幂等：是（仅插入池中已存在的 FieldKey）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

;WITH seed AS (
    SELECT * FROM (VALUES
        (N'ccCode'),
        (N'postId'),
        (N'dutyId'),
        (N'empSal'),
        (N'empInDate'),
        (N'empLeaDate'),
        (N'empKqIndate'),
        (N'empKqLeaDate'),
        (N'empType'),
        (N'empState'),
        (N'empName')
    ) AS v(field_key)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT N'hrEmp', s.field_key, N'monthSnap'
FROM seed s
WHERE EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_def d
    WHERE d.module_code = N'hrEmp' AND d.field_key = s.field_key AND d.IsDelete = 0
)
AND NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene sc
    WHERE sc.module_code = N'hrEmp' AND sc.field_key = s.field_key AND sc.scene = N'monthSnap'
);
GO
