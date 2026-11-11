/*
 * hrEmpChg：表单场景去掉 occText/nccText（原/新部门为列表展示 lookup；
 * 编辑页左右对比选部门属特殊区块，不进 L1/L2 普通字段池）
 * 同步清理默认表单布局中的这两项；个人布局若仍含有则 Effective 会标 stale
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @mod NVARCHAR(64) = N'hrEmpChg';
DECLARE @formLayoutId BIGINT = 1400000004002;

DELETE FROM dbo.hr_ui_field_scene
WHERE module_code = @mod AND scene = N'form' AND field_key IN (N'occText', N'nccText');

-- 所有异动「表单」布局字段：去掉部门 lookup（含个人布局）
UPDATE f
SET f.IsDelete = 1, f.UpdateTime = GETDATE()
FROM dbo.hr_ui_layout_field f
INNER JOIN dbo.hr_ui_layout l ON l.Id = f.layout_id AND l.IsDelete = 0
WHERE l.module_code = @mod AND l.scene = N'form'
  AND f.field_key IN (N'occText', N'nccText') AND f.IsDelete = 0;

-- 若默认布局被用户改乱，确保基础 8 字段仍在（仅当布局字段已被删空时重建）
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @formLayoutId AND IsDelete = 0)
BEGIN
    DECLARE @now DATETIME = GETDATE();
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
        (1400000004208, @formLayoutId, N'remark', N'remark', N'备注', 8, 1, NULL, 2, N'basic', N'textarea', 0, 0, @now, 0);
END

-- 重建系统默认表单布局字段（顺序/占位复位）
UPDATE dbo.hr_ui_layout_field SET IsDelete = 1, UpdateTime = GETDATE()
WHERE layout_id = @formLayoutId AND IsDelete = 0;

DECLARE @now2 DATETIME = GETDATE();
INSERT INTO dbo.hr_ui_layout_field
    (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
VALUES
    (1400000004211, @formLayoutId, N'chgNo', N'chg_no', N'异动单号', 1, 1, NULL, 1, N'basic', N'text', 0, 1, @now2, 0),
    (1400000004212, @formLayoutId, N'status', N'status', N'状态', 2, 1, NULL, 1, N'basic', N'select', 0, 1, @now2, 0),
    (1400000004213, @formLayoutId, N'empId', N'emp_id', N'工号', 3, 1, NULL, 1, N'basic', N'text', 1, 0, @now2, 0),
    (1400000004214, @formLayoutId, N'empName', N'emp_name', N'姓名', 4, 1, NULL, 1, N'basic', N'text', 0, 1, @now2, 0),
    (1400000004215, @formLayoutId, N'chgDate', N'chg_date', N'异动日期', 5, 1, NULL, 1, N'basic', N'date', 1, 0, @now2, 0),
    (1400000004216, @formLayoutId, N'chgType', N'chg_type', N'异动类别', 6, 1, NULL, 1, N'basic', N'select', 1, 0, @now2, 0),
    (1400000004217, @formLayoutId, N'chgReason', N'chg_reason', N'异动原因', 7, 1, NULL, 2, N'basic', N'text', 0, 0, @now2, 0),
    (1400000004218, @formLayoutId, N'remark', N'remark', N'备注', 8, 1, NULL, 2, N'basic', N'textarea', 0, 0, @now2, 0);

SELECT field_key, scene FROM dbo.hr_ui_field_scene WHERE module_code = @mod AND scene = N'form' ORDER BY field_key;
SELECT field_key, label, span_slots FROM dbo.hr_ui_layout_field WHERE layout_id = @formLayoutId AND IsDelete = 0 ORDER BY sort_no;
