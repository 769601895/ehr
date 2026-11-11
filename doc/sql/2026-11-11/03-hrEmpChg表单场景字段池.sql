/*
 * hrEmpChg：补 form 场景适用字段 + 原因/备注进池 + 默认表单布局
 * 修复：业务界面配置 → 表单定义 → 新增 提示「字段池已全部添加」
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @mod NVARCHAR(64) = N'hrEmpChg';
DECLARE @tbl NVARCHAR(128) = N'hr_emp_chg';
DECLARE @formLayoutId BIGINT = 1400000004002;

-- ========== 补字段池：异动原因 / 备注 ==========
;WITH rows AS (
    SELECT * FROM (VALUES
        (1400000002012, N'chgReason', N'physical', N'chg_reason', N'异动原因', N'nvarchar', 1, N'text', 200, 0, 0, 0, 12),
        (1400000002013, N'remark', N'physical', N'remark', N'备注', N'nvarchar', 1, N'textarea', 200, 0, 0, 0, 13)
    ) AS v(Id, FieldKey, Origin, DbColumn, Label, DataType, IsNullable, ControlType, WidthPx, Sortable, Locked, Required, SortNo)
)
MERGE dbo.hr_ui_field_def AS t
USING rows AS s ON t.module_code = @mod AND t.field_key = s.FieldKey AND t.IsDelete = 0
WHEN NOT MATCHED THEN
    INSERT (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, code_field,
            default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
    VALUES (s.Id, @mod, s.FieldKey, s.Origin, s.DbColumn, @tbl,
            s.Label, s.DataType, s.IsNullable, s.ControlType, NULL,
            s.WidthPx, 2, N'basic', 1, s.Sortable, s.Required, 0, s.Locked, s.SortNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET label = s.Label, enabled = 1, is_in_pool = 1, control_type = s.ControlType,
               default_span_slots = 2, UpdateTime = @now;

-- ========== form 场景适用 ==========
;WITH sc AS (
    SELECT FieldKey FROM (VALUES
        (N'chgNo'),(N'empId'),(N'empName'),(N'chgDate'),(N'chgType'),
        (N'status'),(N'chgReason'),(N'remark'),
        (N'occText'),(N'nccText'),(N'opostText'),(N'npostText'),
        (N'createTime')
    ) AS v(FieldKey)
)
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene)
SELECT @mod, s.FieldKey, N'form'
FROM sc s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.hr_ui_field_scene x
    WHERE x.module_code = @mod AND x.field_key = s.FieldKey AND x.scene = N'form'
);

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

SELECT field_key, scene FROM dbo.hr_ui_field_scene WHERE module_code = @mod AND scene = N'form' ORDER BY field_key;
SELECT Id, module_code, scene, name FROM dbo.hr_ui_layout WHERE Id = @formLayoutId;
