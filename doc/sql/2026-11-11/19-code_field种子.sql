/*
 * 变更说明：代码类元数据种子（名称 + 用途说明）；并从 code_mstr 补齐未登记类
 * 日期：2026-11-11
 * 前置：18-code_field建表.sql
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();

;WITH seed AS (
    SELECT * FROM (VALUES
        (N'EmpIdMode', N'工号模式', N'用于公司参数：工号生成规则', 10),
        (N'IdCardV2Enabled', N'二代证校验', N'用于公司参数：是否启用二代证校验', 20),
        (N'HireDayAttMode', N'入职日考勤', N'用于公司参数：入职当天考勤处理方式', 30),
        (N'LeaveDayAttMode', N'离职日考勤', N'用于公司参数：离职当天考勤处理方式', 40),
        (N'CardMakeupAudit', N'补卡审核', N'用于公司参数：补卡是否需审核', 50),
        (N'LeaveAudit', N'请假审核', N'用于公司参数：请假是否需审核', 60),
        (N'OtAuditMode', N'加班审核', N'用于公司参数：加班是否需审核', 70),
        (N'DimissionAudit', N'离职审核', N'用于公司参数：离职是否需审核', 80),
        (N'TransferAudit', N'异动审核', N'用于公司参数：异动是否需审核', 90),
        (N'RegularizeAudit', N'转正审核', N'用于公司参数：转正是否需审核', 100),
        (N'InsMonthReportMode', N'参保月报模式', N'用于公司参数：社保月报生成时机', 110),
        (N'InsSameMonthReport', N'同月参保月报', N'用于公司参数：同月参保是否生成月报', 120),
        (N'InsFeeToPayroll', N'保险费进薪资', N'用于公司参数：保险费用是否进入薪资', 130),
        (N'ProbationOnHire', N'入职试用', N'用于公司参数：入职是否进入试用期', 140),
        (N'ProbationDayInclusive', N'试用天数含首日', N'用于公司参数：试用期天数是否含入职日', 150),
        (N'OtCrossdayMode', N'跨天加班', N'用于公司参数：跨天加班处理方式', 160),
        (N'PayrollAttMonth', N'薪资考勤月', N'用于公司参数：薪资关联考勤月规则', 170),
        (N'RequiredAttDayMode', N'应出勤天模式', N'用于公司参数：应出勤天数计算方式', 180),
        (N'OrgType', N'组织类型', N'用于组织架构：节点组织类型', 200),
        (N'cc_site', N'厂别/站点', N'用于组织或参数：厂别代码', 210),
        (N'cc_costtype', N'成本类型', N'用于组织：成本类型', 220),
        (N'cc_countbasic', N'计人头基数', N'用于组织：人数统计基数', 230),
        (N'emp_country', N'国籍', N'用于人员资料：国籍下拉', 300),
        (N'LeaveReason', N'离职原因', N'用于离职管理：离职原因选项', 310),
        (N'LeaveType', N'离职类型', N'用于离职管理：离职类型选项', 320),
        (N'LeaveSalarySettle', N'离职薪资结算', N'用于离职管理：薪资结算方式', 330),
        (N'EmpChgType', N'异动类别', N'用于异动管理：异动类型', 340),
        (N'EmpMonthExcludeReason', N'月报排除原因', N'用于人事月报：排除人员原因', 350),
        (N'EmpMonthIncludeReason', N'月报纳入原因', N'用于人事月报：人工纳入原因', 360),
        (N'hr_wf_biz', N'审批业务类型', N'用于审批流程：业务类型代码', 400)
    ) AS v(code_field, field_name, usage_desc, order_no)
)
MERGE dbo.code_field AS t
USING seed AS s ON t.code_field = s.code_field
WHEN NOT MATCHED BY TARGET THEN
    INSERT (code_field, field_name, usage_desc, order_no, is_delete, create_time, update_time)
    VALUES (s.code_field, s.field_name, s.usage_desc, s.order_no, 0, @now, @now)
WHEN MATCHED AND t.is_delete = 0 AND (t.field_name = t.code_field OR t.usage_desc LIKE N'（自动同步%') THEN
    UPDATE SET
        field_name = s.field_name,
        usage_desc = s.usage_desc,
        order_no = s.order_no,
        update_time = @now;

-- 从已有码值补齐未登记类（名称暂用编码，用途提示补充）
INSERT INTO dbo.code_field (code_field, field_name, usage_desc, order_no, is_delete, create_time, update_time)
SELECT DISTINCT c.code_field, c.code_field, N'（自动同步，请补充用途说明）', 900, 0, @now, @now
FROM dbo.code_mstr c
WHERE NOT EXISTS (SELECT 1 FROM dbo.code_field f WHERE f.code_field = c.code_field);

;WITH langSeed AS (
    SELECT * FROM (VALUES
        (N'EmpIdMode', N'en', N'Employee ID mode', N'Company param: employee ID generation rule'),
        (N'EmpIdMode', N'zh-TW', N'工號模式', N'用於公司參數：工號產生規則'),
        (N'EmpIdMode', N'th', N'รูปแบบรหัสพนักงาน', N'พารามิเตอร์บริษัท: กติกาสร้างรหัสพนักงาน'),
        (N'EmpIdMode', N'vi', N'Chế độ mã NV', N'Tham số công ty: quy tắc sinh mã nhân viên'),
        (N'LeaveReason', N'en', N'Leave reason', N'Employee leave: reason options'),
        (N'LeaveReason', N'zh-TW', N'離職原因', N'用於離職管理：離職原因選項'),
        (N'LeaveType', N'en', N'Leave type', N'Employee leave: type options'),
        (N'LeaveType', N'zh-TW', N'離職類型', N'用於離職管理：離職類型選項'),
        (N'EmpChgType', N'en', N'Change type', N'Employee change: change category'),
        (N'EmpChgType', N'zh-TW', N'異動類別', N'用於異動管理：異動類型'),
        (N'OrgType', N'en', N'Org type', N'Organization: node type'),
        (N'OrgType', N'zh-TW', N'組織類型', N'用於組織架構：節點組織類型')
    ) AS v(code_field, culture, field_name, usage_desc)
)
MERGE dbo.code_field_lang AS t
USING langSeed AS s ON t.code_field = s.code_field AND t.culture = s.culture
WHEN NOT MATCHED BY TARGET THEN
    INSERT (code_field, culture, field_name, usage_desc)
    VALUES (s.code_field, s.culture, s.field_name, s.usage_desc)
WHEN MATCHED THEN
    UPDATE SET field_name = s.field_name, usage_desc = s.usage_desc;
GO
