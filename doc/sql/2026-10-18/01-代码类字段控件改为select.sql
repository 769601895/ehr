-- 绑了代码类的字段：控件统一为 select（与后端 HrUiControlGuess.Resolve 一致）
SET NOCOUNT ON;

UPDATE dbo.hr_ui_field_def
SET control_type = N'select',
    UpdateTime = GETDATE()
WHERE IsDelete = 0
  AND NULLIF(LTRIM(RTRIM(code_field)), N'') IS NOT NULL
  AND ISNULL(control_type, N'') <> N'select';

UPDATE lf
SET control_type = N'select',
    UpdateTime = GETDATE()
FROM dbo.hr_ui_layout_field lf
INNER JOIN dbo.hr_ui_layout l
    ON l.Id = lf.layout_id AND l.IsDelete = 0
INNER JOIN dbo.hr_ui_field_def d
    ON d.module_code = l.module_code
   AND d.field_key = lf.field_key
   AND d.IsDelete = 0
WHERE lf.IsDelete = 0
  AND NULLIF(LTRIM(RTRIM(d.code_field)), N'') IS NOT NULL
  AND ISNULL(lf.control_type, N'') <> N'select';
