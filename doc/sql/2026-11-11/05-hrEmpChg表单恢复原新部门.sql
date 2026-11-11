/*
 * hrEmpChg：按业务要求，表单场景恢复「原部门/新部门」，并补「原岗位/新岗位」
 * 分组 job=任职信息；新部门/新岗位控件标 cascader/select，原侧只读 text
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @mod NVARCHAR(64) = N'hrEmpChg';
DECLARE @formLayoutId BIGINT = 1400000004002;

-- ========== 字段池：原/新部门 + 原/新岗位 ==========
;WITH rows AS (
    SELECT * FROM (VALUES
        (1400000002007, N'occText', N'lookup', NULL, N'原部门', N'nvarchar', 1, N'text', 120, 0, 0, 0, 1, 7),
        (1400000002008, N'nccText', N'lookup', NULL, N'新部门', N'nvarchar', 1, N'cascader', 120, 0, 0, 1, 0, 8),
        (1400000002014, N'opostText', N'lookup', NULL, N'原岗位', N'nvarchar', 1, N'text', 120, 0, 0, 0, 1, 14),
        (1400000002015, N'npostText', N'lookup', NULL, N'新岗位', N'nvarchar', 1, N'select', 120, 0, 0, 0, 0, 15)
    ) AS v(Id, FieldKey, Origin, DbColumn, Label, DataType, IsNullable, ControlType, WidthPx, Sortable, Locked, Required, Readonly, SortNo)
)
MERGE dbo.hr_ui_field_def AS t
USING rows AS s ON t.module_code = @mod AND t.field_key = s.FieldKey AND t.IsDelete = 0
WHEN NOT MATCHED THEN
    INSERT (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, code_field,
            default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
    VALUES (s.Id, @mod, s.FieldKey, s.Origin, s.DbColumn, NULL,
            s.Label, s.DataType, s.IsNullable, s.ControlType, NULL,
            s.WidthPx, 1, N'job', 1, s.Sortable, s.Required, s.Readonly, s.Locked, s.SortNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET label = s.Label, enabled = 1, is_in_pool = 1, control_type = s.ControlType,
               group_code = N'job', [readonly] = s.Readonly, required = s.Required,
               default_span_slots = 1, UpdateTime = @now;

-- ========== form 场景 ==========
;WITH sc AS (
    SELECT FieldKey FROM (VALUES
        (N'chgNo'),(N'empId'),(N'empName'),(N'chgDate'),(N'chgType'),
        (N'status'),(N'chgReason'),(N'remark'),
        (N'occText'),(N'nccText'),(N'opostText'),(N'npostText')
    ) AS v(FieldKey)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT @mod, s.FieldKey, N'form'
FROM sc s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene x
    WHERE x.module_code = @mod AND x.field_key = s.FieldKey AND x.scene = N'form'
);

-- list 场景也挂岗位展示（可选）
;WITH scList AS (
    SELECT FieldKey FROM (VALUES (N'opostText'),(N'npostText')) AS v(FieldKey)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT @mod, s.FieldKey, N'list'
FROM scList s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene x
    WHERE x.module_code = @mod AND x.field_key = s.FieldKey AND x.scene = N'list'
);

-- ========== 重建默认表单布局 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout WHERE Id = @formLayoutId AND IsDelete = 0)
    INSERT INTO dbo.hr_ui_layout (Id, module_code, scene, name, is_default, owner_user_id, version, CreateTime, IsDelete)
    VALUES (@formLayoutId, @mod, N'form', N'异动管理默认表单', 1, NULL, 1, @now, 0);
ELSE
    UPDATE dbo.hr_ui_layout
    SET name = N'异动管理默认表单', is_default = 1, scene = N'form', IsDelete = 0, UpdateTime = @now
    WHERE Id = @formLayoutId;

UPDATE dbo.hr_ui_layout_field SET IsDelete = 1, UpdateTime = @now
WHERE layout_id = @formLayoutId AND IsDelete = 0;

INSERT INTO dbo.hr_ui_layout_field
    (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
VALUES
    (1400000004221, @formLayoutId, N'chgNo', N'chg_no', N'异动单号', 1, 1, NULL, 1, N'basic', N'text', 0, 1, @now, 0),
    (1400000004222, @formLayoutId, N'status', N'status', N'状态', 2, 1, NULL, 1, N'basic', N'select', 0, 1, @now, 0),
    (1400000004223, @formLayoutId, N'empId', N'emp_id', N'工号', 3, 1, NULL, 1, N'basic', N'text', 1, 0, @now, 0),
    (1400000004224, @formLayoutId, N'empName', N'emp_name', N'姓名', 4, 1, NULL, 1, N'basic', N'text', 0, 1, @now, 0),
    (1400000004225, @formLayoutId, N'chgDate', N'chg_date', N'异动日期', 5, 1, NULL, 1, N'basic', N'date', 1, 0, @now, 0),
    (1400000004226, @formLayoutId, N'chgType', N'chg_type', N'异动类别', 6, 1, NULL, 1, N'basic', N'select', 1, 0, @now, 0),
    (1400000004227, @formLayoutId, N'chgReason', N'chg_reason', N'异动原因', 7, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),
    (1400000004228, @formLayoutId, N'remark', N'remark', N'备注', 8, 1, NULL, 2, N'basic', N'textarea', 0, 0, @now, 0),
    (1400000004229, @formLayoutId, N'occText', NULL, N'原部门', 9, 1, NULL, 1, N'job', N'text', 0, 1, @now, 0),
    (1400000004230, @formLayoutId, N'opostText', NULL, N'原岗位', 10, 1, NULL, 1, N'job', N'text', 0, 1, @now, 0),
    (1400000004231, @formLayoutId, N'nccText', NULL, N'新部门', 11, 1, NULL, 1, N'job', N'cascader', 1, 0, @now, 0),
    (1400000004232, @formLayoutId, N'npostText', NULL, N'新岗位', 12, 1, NULL, 1, N'job', N'select', 0, 0, @now, 0);

-- 若有个人 form 布局曾软删部门字段，恢复可见需用户自行「加载默认」；此处只保证系统默认正确

SELECT field_key, scene FROM dbo.hr_ui_field_scene WHERE module_code = @mod AND scene = N'form' ORDER BY field_key;
SELECT field_key, label, group_code, control_type, span_slots, sort_no
FROM dbo.hr_ui_layout_field WHERE layout_id = @formLayoutId AND IsDelete = 0 ORDER BY sort_no;
