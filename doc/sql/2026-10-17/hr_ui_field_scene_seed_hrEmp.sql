/*
 * 变更说明：hrEmp 场景适用。列表排除证件、地址、照片、ccCode 等；表单排除 ccDesc
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empId' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empId', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empId' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empId', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empName' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empName', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empName' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empName', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empSex' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empSex', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empSex' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empSex', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empBirth' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empBirth', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'age' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'age', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'age' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'age', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empPic' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empPic', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCard' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCard', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCardOverdate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCardOverdate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCardDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCardDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCardLimit' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCardLimit', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empAddrter' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empAddrter', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empAddrter' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empAddrter', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empRgt' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empRgt', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empNation' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empNation', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empProvince' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empProvince', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empMarry' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empMarry', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empResidence' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empResidence', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empAddr' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empAddr', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empHome' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empHome', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'ccCode' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'ccCode', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'ccDesc' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'ccDesc', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'dutyId' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'dutyId', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'dutyId' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'dutyId', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'postId' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'postId', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'postId' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'postId', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empType' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empType', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empType' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empType', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empSal' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empSal', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empSal' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empSal', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empWorkDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empWorkDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empInDateGroup' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empInDateGroup', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empInDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empInDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empInDate' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empInDate', N'list');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empBegDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empBegDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empTryDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empTryDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empFromDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empFromDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empLongmon' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empLongmon', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empKqIndate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empKqIndate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empKqLeaDate' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empKqLeaDate', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empState' AND scene = N'form')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empState', N'form');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empState' AND scene = N'list')
INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empState', N'list');
