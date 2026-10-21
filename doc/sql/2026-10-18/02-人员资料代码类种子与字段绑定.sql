-- 人员资料常用代码类（自旧库对齐）+ 字段元数据绑定为下拉
SET NOCOUNT ON;

-- ========== code_mstr 种子（幂等）==========
MERGE dbo.code_mstr AS t
USING (VALUES
    -- 人员类别 emp_type
    (N'emp_type',  N'01', N'正式工', N'01'),
    (N'emp_type',  N'02', N'门卫',   N'02'),
    (N'emp_type',  N'03', N'临时工', N'03'),
    -- 户籍类型 emp_rgt
    (N'emp_rgt',   N'01', N'外地',   N'01'),
    (N'emp_rgt',   N'02', N'本地',   N'02'),
    (N'emp_rgt',   N'03', N'外国籍', N'03'),
    -- 性别 emp_sex（与旧库实存值一致）
    (N'emp_sex',   N'男',  N'男',    N'01'),
    (N'emp_sex',   N'女',  N'女',    N'02'),
    -- 婚姻状况 emp_marry
    (N'emp_marry', N'已婚', N'已婚',  N'01'),
    (N'emp_marry', N'未婚', N'未婚',  N'02'),
    (N'emp_marry', N'丧偶', N'丧偶',  N'03'),
    (N'emp_marry', N'离异', N'离异',  N'04'),
    (N'emp_marry', N'离婚', N'离婚',  N'05')
) AS s(code_field, code_value, code_desc, code_sort)
ON t.code_field = s.code_field AND t.code_value = s.code_value
WHEN MATCHED THEN
    UPDATE SET code_desc = s.code_desc, code_sort = s.code_sort
WHEN NOT MATCHED THEN
    INSERT (code_field, code_value, code_desc, code_sort)
    VALUES (s.code_field, s.code_value, s.code_desc, s.code_sort);

-- ========== 字段池：绑代码类 + 控件 select ==========
UPDATE d
SET code_field = v.code_field,
    control_type = N'select',
    UpdateTime = GETDATE()
FROM dbo.hr_ui_field_def d
INNER JOIN (VALUES
    (N'empType',  N'emp_type'),
    (N'empSex',   N'emp_sex'),
    (N'empMarry', N'emp_marry'),
    (N'empRgt',   N'emp_rgt')
) AS v(field_key, code_field) ON d.field_key = v.field_key
WHERE d.module_code = N'hrEmp'
  AND ISNULL(d.IsDelete, 0) = 0;

-- 布局行控件同步为 select
UPDATE lf
SET control_type = N'select',
    UpdateTime = GETDATE()
FROM dbo.hr_ui_layout_field lf
INNER JOIN dbo.hr_ui_layout l ON l.Id = lf.layout_id AND ISNULL(l.IsDelete, 0) = 0
INNER JOIN dbo.hr_ui_field_def d
    ON d.module_code = l.module_code AND d.field_key = lf.field_key AND ISNULL(d.IsDelete, 0) = 0
WHERE l.module_code = N'hrEmp'
  AND d.field_key IN (N'empType', N'empSex', N'empMarry', N'empRgt')
  AND ISNULL(lf.IsDelete, 0) = 0
  AND ISNULL(lf.control_type, N'') <> N'select';
