/*
 * 变更说明：字段场景适用。主键 module_code + field_key + scene
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.hr_ui_field_scene', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_ui_field_scene
    (
        module_code NVARCHAR(64) NOT NULL,
        field_key   NVARCHAR(64) NOT NULL,
        scene       NVARCHAR(20) NOT NULL,
        CONSTRAINT PK_hr_ui_field_scene PRIMARY KEY CLUSTERED (module_code, field_key, scene)
    );
END;
