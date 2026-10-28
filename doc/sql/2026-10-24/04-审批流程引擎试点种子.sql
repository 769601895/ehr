/*
 * 变更说明：试点 hrWfGeneral 样例等级/表单/流程（不含角色管辖与岗位，需按环境手工补）
 * 模块：hrWf
 * 日期：2026-10-24
 * 影响库：ehrnew（HR 库表）
 * 是否幂等：是（按 code 判断）
 * 说明：角色管辖/岗位依赖真实 CcCode 与 post_id，请在前端「审批角色」页配置后联调
 */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();

/* ---------- 审核等级（与旧系统级别定义一致，完整重建见 07-*.sql） ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_level WHERE code = N'1030' AND IsDelete = 0)
    INSERT INTO dbo.hr_wf_level (Id, code, name, audit_order, is_functional, order_no, enabled, remark, CreateTime, IsDelete)
    VALUES (13000000051030, N'1030', N'部门主管', 1, 0, 10, 1, N'试点', @now, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_level WHERE code = N'1025' AND IsDelete = 0)
    INSERT INTO dbo.hr_wf_level (Id, code, name, audit_order, is_functional, order_no, enabled, remark, CreateTime, IsDelete)
    VALUES (13000000051025, N'1025', N'分部经理', 2, 0, 20, 1, N'试点', @now, 0);

/* ---------- 表单方案 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_form WHERE biz_type = N'hrWfGeneral' AND code = N'generalDefault' AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_wf_form (Id, biz_type, code, name, form_mode, version, enabled, remark, CreateTime, IsDelete)
    VALUES (1300000004010, N'hrWfGeneral', N'generalDefault', N'通用申请表单', N'B', 1, 1, N'试点', @now, 0);

    INSERT INTO dbo.hr_wf_form_field
        (Id, form_id, field_key, label, control_type, field_source, required, [readonly], sort_no, group_code, span_slots, CreateTime, IsDelete)
    VALUES
        (1300000004011, 1300000004010, N'subject', N'申请主题', N'text', N'Custom', 1, 0, 10, N'main', 24, @now, 0),
        (1300000004012, 1300000004010, N'amount', N'金额', N'number', N'Custom', 0, 0, 20, N'main', 12, @now, 0),
        (1300000004013, 1300000004010, N'needDate', N'需用日期', N'date', N'Custom', 0, 0, 30, N'main', 12, @now, 0),
        (1300000004014, 1300000004010, N'reason', N'事由说明', N'textarea', N'Custom', 1, 0, 40, N'main', 24, @now, 0),
        (1300000004015, 1300000004010, N'deptCcCode', N'申请部门', N'dept', N'Custom', 1, 0, 50, N'main', 12, @now, 0);
END

/* ---------- 流程定义（2 级 Level） ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_def WHERE biz_type = N'hrWfGeneral' AND code = N'generalFlow' AND IsDelete = 0)
   AND EXISTS (SELECT 1 FROM dbo.hr_wf_form WHERE Id = 1300000004010 AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_wf_def
        (Id, biz_type, code, name, form_id, version, enabled,
         allow_edit_while_running, edit_by_applicant, edit_by_assignee, default_timeout_hours,
         remark, CreateTime, IsDelete)
    VALUES
        (1300000004020, N'hrWfGeneral', N'generalFlow', N'通用申请流程', 1300000004010, 1, 1,
         0, 0, 0, 0, N'试点', @now, 0);

    INSERT INTO dbo.hr_wf_def_node
        (Id, def_id, node_key, name, order_no, node_mode, assignee_rule_json, enabled, CreateTime, IsDelete)
    VALUES
        (1300000004021, 1300000004020, N'n1', N'部门主管', 10, N'Or',
         N'{"type":"Level","levelId":13000000051030}', 1, @now, 0),
        (1300000004022, 1300000004020, N'n2', N'分部经理', 20, N'Or',
         N'{"type":"Level","levelId":13000000051025}', 1, @now, 0);
END

SELECT N'level' AS kind, Id, code, name FROM dbo.hr_wf_level WHERE Id IN (13000000051030, 13000000051025)
UNION ALL
SELECT N'form', Id, code, name FROM dbo.hr_wf_form WHERE Id = 1300000004010
UNION ALL
SELECT N'def', Id, code, name FROM dbo.hr_wf_def WHERE Id = 1300000004020;
