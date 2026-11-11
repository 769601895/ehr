/*
 * hrEmpChg：createTime 补 form 场景适用
 * 原因：字段已入池但仅挂 list；表单定义新增提示「字段池已全部添加」
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @mod NVARCHAR(64) = N'hrEmpChg';

IF NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene
    WHERE module_code = @mod AND field_key = N'createTime' AND scene = N'form'
)
BEGIN
    INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
    VALUES (@mod, N'createTime', N'form');
END

-- 入池开关兜底（截图已勾选，幂等）
UPDATE dbo.hr_ui_field_def
SET is_in_pool = 1, enabled = 1, UpdateTime = GETDATE()
WHERE module_code = @mod AND field_key = N'createTime' AND IsDelete = 0
  AND (is_in_pool = 0 OR enabled = 0);

SELECT field_key, scene
FROM dbo.hr_ui_field_scene
WHERE module_code = @mod AND field_key = N'createTime'
ORDER BY scene;
