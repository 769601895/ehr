/*
 * hrEmpChg 字段池 + list 场景 + 系统默认列表布局
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @mod NVARCHAR(64) = N'hrEmpChg';
DECLARE @tbl NVARCHAR(128) = N'hr_emp_chg';
DECLARE @layoutId BIGINT = 1400000004001;
DECLARE @formLayoutId BIGINT = 1400000004002;

-- ========== 字段池 ==========
;WITH rows AS (
    SELECT * FROM (VALUES
        (1400000002001, N'chgNo', N'physical', N'chg_no', N'异动单号', N'nvarchar', 0, N'text', 150, 1, 1, 1, 1),
        (1400000002002, N'empId', N'physical', N'emp_id', N'工号', N'nvarchar', 0, N'text', 100, 1, 1, 1, 2),
        (1400000002003, N'empName', N'physical', N'emp_name', N'姓名', N'nvarchar', 1, N'text', 100, 1, 1, 0, 3),
        (1400000002004, N'chgDate', N'physical', N'chg_date', N'异动日期', N'datetime', 0, N'date', 110, 1, 1, 0, 4),
        (1400000002005, N'chgType', N'physical', N'chg_type', N'异动类别', N'nvarchar', 0, N'select', 100, 1, 1, 0, 5),
        (1400000002006, N'chgTypeText', N'lookup', NULL, N'类别', N'nvarchar', 1, N'text', 100, 0, 1, 0, 6),
        (1400000002007, N'occText', N'lookup', NULL, N'原部门', N'nvarchar', 1, N'text', 120, 0, 1, 0, 7),
        (1400000002008, N'nccText', N'lookup', NULL, N'新部门', N'nvarchar', 1, N'cascader', 120, 0, 1, 0, 8),
        (1400000002009, N'status', N'physical', N'status', N'状态', N'nvarchar', 0, N'select', 100, 1, 1, 0, 9),
        (1400000002010, N'createUserName', N'lookup', NULL, N'创建人', N'nvarchar', 1, N'text', 90, 0, 1, 0, 10),
        (1400000002011, N'createTime', N'physical', N'CreateTime', N'创建时间', N'datetime', 1, N'datetime', 150, 1, 1, 0, 11),
        (1400000002012, N'chgReason', N'physical', N'chg_reason', N'异动原因', N'nvarchar', 1, N'text', 200, 0, 0, 0, 12),
        (1400000002013, N'remark', N'physical', N'remark', N'备注', N'nvarchar', 1, N'textarea', 200, 0, 0, 0, 13),
        (1400000002014, N'opostText', N'lookup', NULL, N'原岗位', N'nvarchar', 1, N'text', 120, 0, 0, 0, 14),
        (1400000002015, N'npostText', N'lookup', NULL, N'新岗位', N'nvarchar', 1, N'select', 120, 0, 0, 0, 15)
    ) AS v(Id, FieldKey, Origin, DbColumn, Label, DataType, IsNullable, ControlType, WidthPx, Sortable, Locked, Required, SortNo)
)
MERGE dbo.hr_ui_field_def AS t
USING rows AS s ON t.module_code = @mod AND t.field_key = s.FieldKey AND t.IsDelete = 0
WHEN NOT MATCHED THEN
    INSERT (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, code_field,
            default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
    VALUES (s.Id, @mod, s.FieldKey, s.Origin, s.DbColumn, CASE WHEN s.Origin = N'physical' THEN @tbl ELSE NULL END,
            s.Label, s.DataType, s.IsNullable, s.ControlType,
            CASE WHEN s.FieldKey = N'chgType' THEN N'EmpChgType' ELSE NULL END,
            s.WidthPx,
            CASE WHEN s.FieldKey IN (N'chgReason', N'remark') THEN 2 ELSE 1 END,
            CASE WHEN s.FieldKey IN (N'occText', N'nccText', N'opostText', N'npostText') THEN N'job' ELSE N'basic' END,
            1, s.Sortable, s.Required,
            CASE WHEN s.FieldKey IN (N'occText', N'opostText', N'chgNo', N'empName', N'status') THEN 1 ELSE 0 END,
            s.Locked, s.SortNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET label = s.Label, enabled = 1, is_in_pool = 1, UpdateTime = @now;

-- chgType 补 CodeField（若已存在行）
UPDATE dbo.hr_ui_field_def
SET code_field = N'EmpChgType', control_type = N'select', UpdateTime = @now
WHERE module_code = @mod AND field_key = N'chgType' AND IsDelete = 0;

-- ========== 场景适用 list ==========
;WITH sc AS (
    SELECT FieldKey FROM (VALUES
        (N'chgNo'),(N'empId'),(N'empName'),(N'chgDate'),(N'chgType'),(N'chgTypeText'),
        (N'occText'),(N'nccText'),(N'status'),(N'createUserName'),(N'createTime')
    ) AS v(FieldKey)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT @mod, s.FieldKey, N'list'
FROM sc s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene x
    WHERE x.module_code = @mod AND x.field_key = s.FieldKey AND x.scene = N'list'
);

-- ========== 场景适用 form（含原/新部门岗位）==========
;WITH scForm AS (
    SELECT FieldKey FROM (VALUES
        (N'chgNo'),(N'empId'),(N'empName'),(N'chgDate'),(N'chgType'),
        (N'status'),(N'chgReason'),(N'remark'),
        (N'occText'),(N'nccText'),(N'opostText'),(N'npostText'),
        (N'createTime')
    ) AS v(FieldKey)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT @mod, s.FieldKey, N'form'
FROM scForm s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene x
    WHERE x.module_code = @mod AND x.field_key = s.FieldKey AND x.scene = N'form'
);

-- ========== 系统默认列表布局 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout WHERE Id = @layoutId)
    INSERT INTO dbo.hr_ui_layout (Id, module_code, scene, name, is_default, owner_user_id, version, CreateTime, IsDelete)
    VALUES (@layoutId, @mod, N'list', N'异动管理默认列表', 1, NULL, 1, @now, 0);
ELSE
    UPDATE dbo.hr_ui_layout
    SET module_code = @mod, scene = N'list', name = N'异动管理默认列表', is_default = 1,
        owner_user_id = NULL, IsDelete = 0, UpdateTime = @now
    WHERE Id = @layoutId;

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @layoutId AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_ui_layout_field
        (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
    VALUES
        (1400000004101, @layoutId, N'chgNo', N'chg_no', N'异动单号', 1, 1, 150, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004102, @layoutId, N'empId', N'emp_id', N'工号', 2, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004103, @layoutId, N'empName', N'emp_name', N'姓名', 3, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004104, @layoutId, N'chgDate', N'chg_date', N'异动日期', 4, 1, 110, NULL, NULL, N'date', 0, 0, @now, 0),
        (1400000004105, @layoutId, N'chgTypeText', NULL, N'类别', 5, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004106, @layoutId, N'occText', NULL, N'原部门', 6, 1, 120, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004107, @layoutId, N'nccText', NULL, N'新部门', 7, 1, 120, NULL, NULL, N'text', 0, 0, @now, 0),
        (1400000004108, @layoutId, N'status', N'status', N'状态', 8, 1, 100, NULL, NULL, N'select', 0, 0, @now, 0),
        (1400000004109, @layoutId, N'createUserName', NULL, N'创建人', 9, 1, 90, NULL, NULL, N'text', 0, 0, @now, 0);
END

-- ========== 系统默认表单布局 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout WHERE Id = @formLayoutId)
    INSERT INTO dbo.hr_ui_layout (Id, module_code, scene, name, is_default, owner_user_id, version, CreateTime, IsDelete)
    VALUES (@formLayoutId, @mod, N'form', N'异动管理默认表单', 1, NULL, 1, @now, 0);
ELSE
    UPDATE dbo.hr_ui_layout
    SET module_code = @mod, scene = N'form', name = N'异动管理默认表单', is_default = 1,
        owner_user_id = NULL, IsDelete = 0, UpdateTime = @now
    WHERE Id = @formLayoutId;

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @formLayoutId AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_ui_layout_field
        (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
    VALUES
        (1400000004201, @formLayoutId, N'chgNo', N'chg_no', N'异动单号', 1, 1, NULL, 1, N'basic', N'text', 0, 1, @now, 0),
        (1400000004202, @formLayoutId, N'status', N'status', N'状态', 2, 1, NULL, 1, N'basic', N'select', 0, 1, @now, 0),
        (1400000004203, @formLayoutId, N'empId', N'emp_id', N'工号', 3, 1, NULL, 1, N'basic', N'text', 1, 0, @now, 0),
        (1400000004204, @formLayoutId, N'empName', N'emp_name', N'姓名', 4, 1, NULL, 1, N'basic', N'text', 0, 1, @now, 0),
        (1400000004205, @formLayoutId, N'chgDate', N'chg_date', N'异动日期', 5, 1, NULL, 1, N'basic', N'date', 1, 0, @now, 0),
        (1400000004206, @formLayoutId, N'chgType', N'chg_type', N'异动类别', 6, 1, NULL, 1, N'basic', N'select', 1, 0, @now, 0),
        (1400000004207, @formLayoutId, N'chgReason', N'chg_reason', N'异动原因', 7, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),
        (1400000004208, @formLayoutId, N'remark', N'remark', N'备注', 8, 1, NULL, 2, N'basic', N'textarea', 0, 0, @now, 0),
        (1400000004209, @formLayoutId, N'occText', NULL, N'原部门', 9, 1, NULL, 1, N'job', N'text', 0, 1, @now, 0),
        (1400000004210, @formLayoutId, N'opostText', NULL, N'原岗位', 10, 1, NULL, 1, N'job', N'text', 0, 1, @now, 0),
        (1400000004211, @formLayoutId, N'nccText', NULL, N'新部门', 11, 1, NULL, 1, N'job', N'cascader', 1, 0, @now, 0),
        (1400000004212, @formLayoutId, N'npostText', NULL, N'新岗位', 12, 1, NULL, 1, N'job', N'select', 0, 0, @now, 0);
END

SELECT module_code, field_key, origin, label FROM dbo.hr_ui_field_def WHERE module_code = @mod AND IsDelete = 0 ORDER BY sort_no;
SELECT Id, module_code, scene, name FROM dbo.hr_ui_layout WHERE Id IN (@layoutId, @formLayoutId);
SELECT scene, COUNT(*) cnt FROM dbo.hr_ui_field_scene WHERE module_code = @mod GROUP BY scene;
