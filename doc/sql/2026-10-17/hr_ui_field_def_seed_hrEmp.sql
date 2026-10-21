/*
 * 变更说明：hrEmp 字段池种子，与原 GetPool 并集一致
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empId' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001001, N'hrEmp', N'empId', N'physical', N'emp_id', N'h_emp_mstr', N'工号', N'nvarchar', 0, N'text', 100, 1, N'basic', 1, 1, 1, 0, 1, 1, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empName' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001002, N'hrEmp', N'empName', N'physical', N'emp_name', N'h_emp_mstr', N'姓名', N'nvarchar', 0, N'text', 100, 1, N'basic', 1, 1, 1, 0, 1, 2, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empSex' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001003, N'hrEmp', N'empSex', N'physical', N'emp_sex', N'h_emp_mstr', N'性别', N'nvarchar', 1, N'text', 70, 1, N'basic', 1, 1, 0, 0, 0, 3, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empBirth' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001004, N'hrEmp', N'empBirth', N'physical', N'emp_birth', N'h_emp_mstr', N'出生日期', N'datetime', 1, N'date', 120, 1, N'basic', 1, 1, 0, 0, 0, 4, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'age' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001005, N'hrEmp', N'age', N'computed', NULL, NULL, N'年龄', N'int', 1, N'computed', 70, 1, N'basic', 1, 0, 0, 1, 0, 5, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empPic' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001006, N'hrEmp', N'empPic', N'physical', N'emp_pic', N'h_emp_mstr', N'照片', N'nvarchar', 1, N'photo', 80, 1, N'basic', 1, 0, 0, 0, 0, 6, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCard' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001007, N'hrEmp', N'empCard', N'physical', N'emp_card', N'h_emp_mstr', N'身份证号', N'nvarchar', 1, N'text', 160, 2, N'basic', 1, 1, 0, 0, 0, 7, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCardOverdate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001008, N'hrEmp', N'empCardOverdate', N'physical', N'emp_card_overdate', N'h_emp_mstr', N'身份证到期日', N'datetime', 1, N'date', 120, 1, N'basic', 1, 1, 0, 0, 0, 8, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCardDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001009, N'hrEmp', N'empCardDate', N'physical', N'emp_card_date', N'h_emp_mstr', N'发证日期', N'datetime', 1, N'date', 120, 1, N'basic', 1, 1, 0, 0, 0, 9, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCardLimit' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001010, N'hrEmp', N'empCardLimit', N'physical', N'emp_card_limit', N'h_emp_mstr', N'期限', N'decimal', 1, N'number', 80, 1, N'basic', 1, 1, 0, 0, 0, 10, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empAddrter' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001011, N'hrEmp', N'empAddrter', N'physical', N'emp_addrter', N'h_emp_mstr', N'联系电话', N'nvarchar', 1, N'text', 120, 1, N'basic', 1, 1, 0, 0, 0, 11, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empRgt' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001012, N'hrEmp', N'empRgt', N'physical', N'emp_rgt', N'h_emp_mstr', N'户籍类型', N'nvarchar', 1, N'text', 90, 1, N'basic', 1, 1, 0, 0, 0, 12, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empNation' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001013, N'hrEmp', N'empNation', N'physical', N'emp_nation', N'h_emp_mstr', N'民族', N'nvarchar', 1, N'text', 80, 1, N'basic', 1, 1, 0, 0, 0, 13, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empProvince' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001014, N'hrEmp', N'empProvince', N'physical', N'emp_province', N'h_emp_mstr', N'省份', N'nvarchar', 1, N'text', 90, 1, N'basic', 1, 1, 0, 0, 0, 14, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empMarry' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001015, N'hrEmp', N'empMarry', N'physical', N'emp_marry', N'h_emp_mstr', N'婚姻状况', N'nvarchar', 1, N'text', 90, 1, N'basic', 1, 1, 0, 0, 0, 15, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empResidence' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001016, N'hrEmp', N'empResidence', N'physical', N'emp_residence', N'h_emp_mstr', N'户口所在地', N'nvarchar', 1, N'text', 160, 2, N'basic', 1, 1, 0, 0, 0, 16, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empAddr' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001017, N'hrEmp', N'empAddr', N'physical', N'emp_addr', N'h_emp_mstr', N'现住址', N'nvarchar', 1, N'text', 160, 2, N'basic', 1, 1, 0, 0, 0, 17, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empHome' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001018, N'hrEmp', N'empHome', N'physical', N'emp_home', N'h_emp_mstr', N'家庭住址', N'nvarchar', 1, N'text', 160, 2, N'basic', 1, 1, 0, 0, 0, 18, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'ccCode' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001019, N'hrEmp', N'ccCode', N'physical', N'cc_code', N'h_emp_mstr', N'部门', N'nvarchar', 1, N'cascader', 140, 2, N'job', 1, 0, 0, 0, 0, 19, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'ccDesc' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001020, N'hrEmp', N'ccDesc', N'lookup', NULL, NULL, N'部门', N'nvarchar', 1, N'text', 140, 1, N'job', 1, 0, 0, 0, 0, 20, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'dutyId' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001021, N'hrEmp', N'dutyId', N'physical', N'duty_id', N'h_emp_mstr', N'职务', N'nvarchar', 1, N'text', 90, 1, N'job', 1, 1, 0, 0, 0, 21, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'postId' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001022, N'hrEmp', N'postId', N'physical', N'post_id', N'h_emp_mstr', N'岗位', N'nvarchar', 1, N'text', 90, 1, N'job', 1, 1, 0, 0, 0, 22, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empType' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001023, N'hrEmp', N'empType', N'physical', N'emp_type', N'h_emp_mstr', N'人员类别', N'nvarchar', 1, N'text', 100, 1, N'job', 1, 1, 0, 0, 0, 23, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empSal' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001024, N'hrEmp', N'empSal', N'physical', N'emp_sal', N'h_emp_mstr', N'薪酬类别', N'nvarchar', 1, N'text', 90, 1, N'job', 1, 1, 0, 0, 0, 24, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empWorkDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001025, N'hrEmp', N'empWorkDate', N'physical', N'emp_work_date', N'h_emp_mstr', N'参加工作时间', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 25, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empInDateGroup' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001026, N'hrEmp', N'empInDateGroup', N'physical', N'emp_in_date_group', N'h_emp_mstr', N'集团入职日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 26, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empInDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001027, N'hrEmp', N'empInDate', N'physical', N'emp_in_date', N'h_emp_mstr', N'公司入职日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 27, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empBegDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001028, N'hrEmp', N'empBegDate', N'physical', N'emp_beg_date', N'h_emp_mstr', N'试用起始日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 28, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empTryDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001029, N'hrEmp', N'empTryDate', N'physical', N'emp_try_date', N'h_emp_mstr', N'试用到期日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 29, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empFromDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001030, N'hrEmp', N'empFromDate', N'physical', N'emp_from_date', N'h_emp_mstr', N'转正日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 30, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empLongmon' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001031, N'hrEmp', N'empLongmon', N'physical', N'emp_longmon', N'h_emp_mstr', N'入司前工龄(月)', N'decimal', 1, N'number', 110, 1, N'job', 1, 1, 0, 0, 0, 31, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empKqIndate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001032, N'hrEmp', N'empKqIndate', N'physical', N'emp_kq_indate', N'h_emp_mstr', N'考勤开始日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 32, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empKqLeaDate' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001033, N'hrEmp', N'empKqLeaDate', N'physical', N'emp_kq_lea_date', N'h_emp_mstr', N'考勤结束日', N'datetime', 1, N'date', 120, 1, N'job', 1, 1, 0, 0, 0, 33, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empState' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type, default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime)
VALUES (1400000001034, N'hrEmp', N'empState', N'physical', N'emp_state', N'h_emp_mstr', N'人员状态', N'int', 1, N'select', 80, 1, N'job', 1, 1, 0, 0, 0, 34, 1, 0, GETDATE());
