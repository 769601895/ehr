/*

 * 变更说明：预置 hrEmp 列表/表单系统默认布局

 * 前置：20261013_hr_ui_layout_tables.sql

 * 是否幂等：是（按固定 Id）

 */



SET NOCOUNT ON;



DECLARE @now DATETIME = GETDATE();

DECLARE @listId BIGINT = 1300000003001;

DECLARE @formId BIGINT = 1300000003002;



IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout WHERE Id = @listId)

    INSERT INTO dbo.hr_ui_layout (Id, module_code, scene, name, is_default, owner_user_id, version, CreateTime, IsDelete)

    VALUES (@listId, N'hrEmp', N'list', N'人员资料默认列表', 1, NULL, 1, @now, 0);



IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout WHERE Id = @formId)

    INSERT INTO dbo.hr_ui_layout (Id, module_code, scene, name, is_default, owner_user_id, version, CreateTime, IsDelete)

    VALUES (@formId, N'hrEmp', N'form', N'人员资料默认表单', 1, NULL, 1, @now, 0);



-- 列表明细：若该 layout 尚无字段则插入

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @listId AND IsDelete = 0)

BEGIN

    INSERT INTO dbo.hr_ui_layout_field

        (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)

    VALUES

        (1300000003101, @listId, N'empId', N'emp_id', N'工号', 1, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003102, @listId, N'empName', N'emp_name', N'姓名', 2, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003103, @listId, N'ccDesc', NULL, N'部门', 3, 1, 140, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003104, @listId, N'empSex', N'emp_sex', N'性别', 4, 1, 70, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003105, @listId, N'age', NULL, N'年龄', 5, 1, 70, NULL, NULL, N'computed', 0, 1, @now, 0),

        (1300000003106, @listId, N'empAddrter', N'emp_addrter', N'联系电话', 6, 1, 120, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003107, @listId, N'dutyId', N'duty_id', N'职务', 7, 1, 90, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003108, @listId, N'postId', N'post_id', N'岗位', 8, 1, 90, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003109, @listId, N'empInDate', N'emp_in_date', N'公司入职日', 9, 1, 120, NULL, NULL, N'date', 0, 0, @now, 0),

        (1300000003110, @listId, N'empType', N'emp_type', N'人员类别', 10, 1, 100, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003111, @listId, N'empSal', N'emp_sal', N'薪酬类别', 11, 1, 90, NULL, NULL, N'text', 0, 0, @now, 0),

        (1300000003112, @listId, N'empState', N'emp_state', N'状态', 12, 1, 80, NULL, NULL, N'select', 0, 0, @now, 0);

END;



-- 表单明细

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @formId AND IsDelete = 0)

BEGIN

    INSERT INTO dbo.hr_ui_layout_field

        (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)

    VALUES

        (1300000003201, @formId, N'empId', N'emp_id', N'工号', 1, 1, NULL, 1, N'basic', N'text', 1, 0, @now, 0),

        (1300000003202, @formId, N'empName', N'emp_name', N'姓名', 2, 1, NULL, 1, N'basic', N'text', 1, 0, @now, 0),

        (1300000003203, @formId, N'empSex', N'emp_sex', N'性别', 3, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003204, @formId, N'empBirth', N'emp_birth', N'出生日期', 4, 1, NULL, 1, N'basic', N'date', 0, 0, @now, 0),

        (1300000003205, @formId, N'age', NULL, N'年龄', 5, 1, NULL, 1, N'basic', N'computed', 0, 1, @now, 0),

        (1300000003206, @formId, N'empPic', N'emp_pic', N'照片', 6, 1, NULL, 1, N'basic', N'photo', 0, 0, @now, 0),

        (1300000003207, @formId, N'empCard', N'emp_card', N'身份证号', 7, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),

        (1300000003208, @formId, N'empCardOverdate', N'emp_card_overdate', N'身份证到期日', 8, 1, NULL, 1, N'basic', N'date', 0, 0, @now, 0),

        (1300000003209, @formId, N'empCardDate', N'emp_card_date', N'发证日期', 9, 1, NULL, 1, N'basic', N'date', 0, 0, @now, 0),

        (1300000003210, @formId, N'empCardLimit', N'emp_card_limit', N'期限', 10, 1, NULL, 1, N'basic', N'number', 0, 0, @now, 0),

        (1300000003211, @formId, N'empAddrter', N'emp_addrter', N'联系电话', 11, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003212, @formId, N'empRgt', N'emp_rgt', N'户籍类型', 12, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003213, @formId, N'empNation', N'emp_nation', N'民族', 13, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003214, @formId, N'empProvince', N'emp_province', N'省份', 14, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003215, @formId, N'empMarry', N'emp_marry', N'婚姻状况', 15, 1, NULL, 1, N'basic', N'text', 0, 0, @now, 0),

        (1300000003216, @formId, N'empResidence', N'emp_residence', N'户口所在地', 16, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),

        (1300000003217, @formId, N'empAddr', N'emp_addr', N'现住址', 17, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),

        (1300000003218, @formId, N'empHome', N'emp_home', N'家庭住址', 18, 1, NULL, 2, N'basic', N'text', 0, 0, @now, 0),

        (1300000003219, @formId, N'ccCode', N'cc_code', N'部门', 19, 1, NULL, 2, N'job', N'cascader', 0, 0, @now, 0),

        (1300000003220, @formId, N'dutyId', N'duty_id', N'职务', 20, 1, NULL, 1, N'job', N'text', 0, 0, @now, 0),

        (1300000003221, @formId, N'postId', N'post_id', N'岗位', 21, 1, NULL, 1, N'job', N'text', 0, 0, @now, 0),

        (1300000003222, @formId, N'empType', N'emp_type', N'人员类别', 22, 1, NULL, 1, N'job', N'text', 0, 0, @now, 0),

        (1300000003223, @formId, N'empSal', N'emp_sal', N'薪酬类别', 23, 1, NULL, 1, N'job', N'text', 0, 0, @now, 0),

        (1300000003224, @formId, N'empWorkDate', N'emp_work_date', N'参加工作时间', 24, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003225, @formId, N'empInDateGroup', N'emp_in_date_group', N'集团入职日', 25, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003226, @formId, N'empInDate', N'emp_in_date', N'公司入职日', 26, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003227, @formId, N'empBegDate', N'emp_beg_date', N'试用起始日', 27, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003228, @formId, N'empTryDate', N'emp_try_date', N'试用到期日', 28, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003229, @formId, N'empFromDate', N'emp_from_date', N'转正日', 29, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003230, @formId, N'empLongmon', N'emp_longmon', N'入司前工龄(月)', 30, 1, NULL, 1, N'job', N'number', 0, 0, @now, 0),

        (1300000003231, @formId, N'empKqIndate', N'emp_kq_indate', N'考勤开始日', 31, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003232, @formId, N'empKqLeaDate', N'emp_kq_lea_date', N'考勤结束日', 32, 1, NULL, 1, N'job', N'date', 0, 0, @now, 0),

        (1300000003233, @formId, N'empState', N'emp_state', N'人员状态', 33, 1, NULL, 1, N'job', N'select', 0, 0, @now, 0);

END;



SELECT l.Id, l.module_code, l.scene, l.name, COUNT(f.Id) AS field_cnt

FROM dbo.hr_ui_layout l

LEFT JOIN dbo.hr_ui_layout_field f ON f.layout_id = l.Id AND f.IsDelete = 0

WHERE l.Id IN (@listId, @formId)

GROUP BY l.Id, l.module_code, l.scene, l.name;

