-- hrEmh 审批表单补「异动日期」emh_date，供生效策略 base_date_field 使用
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @formId bigint = (
    SELECT TOP 1 id FROM dbo.hr_wf_form WHERE biz_type = N'hrEmh' AND enabled = 1 AND IsDelete = 0
);
IF @formId IS NULL
BEGIN
    RAISERROR(N'未找到启用的 hrEmh 表单', 16, 1);
    RETURN;
END

IF NOT EXISTS (
    SELECT 1 FROM dbo.hr_wf_form_field
    WHERE form_id = @formId AND field_key = N'emh_date' AND IsDelete = 0
)
INSERT INTO dbo.hr_wf_form_field
(
    Id, form_id, field_key, label, control_type, field_source, required, [readonly],
    sort_no, span_slots, code_field, lookup_source, default_value, placeholder, extra_json,
    CreateTime, CreateUserId, CreateUserName, IsDelete
)
VALUES
(
    864500020000001, @formId, N'emh_date', N'异动日期', N'date', N'Custom', 1, 0,
    15, 12, NULL, NULL, NULL, N'异动生效基准日', NULL,
    GETDATE(), 1300000000101, N'超级管理员', 0
);

SELECT field_key, label, control_type, sort_no, required
FROM dbo.hr_wf_form_field
WHERE form_id = @formId AND IsDelete = 0
ORDER BY sort_no;
