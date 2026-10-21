/*
 * 变更说明：人员资料常用代码类的多语言描述（列表/下拉按当前文化取 code_desc）
 * 日期：2026-10-19
 * 影响库：ehr
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.code_mstr_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.code_mstr_lang
    (
        code_field NVARCHAR(50)  NOT NULL,
        code_value NVARCHAR(50)  NOT NULL,
        culture    NVARCHAR(16)  NOT NULL,
        code_desc  NVARCHAR(256) NULL,
        CONSTRAINT PK_code_mstr_lang PRIMARY KEY CLUSTERED (code_field, code_value, culture)
    );
END;

MERGE dbo.code_mstr_lang AS t
USING (VALUES
    (N'emp_sex', N'男', N'en', N'Male'),
    (N'emp_sex', N'男', N'zh-TW', N'男'),
    (N'emp_sex', N'男', N'th', N'ชาย'),
    (N'emp_sex', N'男', N'vi', N'Nam'),
    (N'emp_sex', N'女', N'en', N'Female'),
    (N'emp_sex', N'女', N'zh-TW', N'女'),
    (N'emp_sex', N'女', N'th', N'หญิง'),
    (N'emp_sex', N'女', N'vi', N'Nữ'),

    (N'emp_type', N'01', N'en', N'Regular'),
    (N'emp_type', N'01', N'zh-TW', N'正式工'),
    (N'emp_type', N'01', N'th', N'พนักงานประจำ'),
    (N'emp_type', N'01', N'vi', N'Chính thức'),
    (N'emp_type', N'02', N'en', N'Guard'),
    (N'emp_type', N'02', N'zh-TW', N'門衛'),
    (N'emp_type', N'02', N'th', N'รปภ.'),
    (N'emp_type', N'02', N'vi', N'Bảo vệ'),
    (N'emp_type', N'03', N'en', N'Temporary'),
    (N'emp_type', N'03', N'zh-TW', N'臨時工'),
    (N'emp_type', N'03', N'th', N'พนักงานชั่วคราว'),
    (N'emp_type', N'03', N'vi', N'Thời vụ'),

    (N'emp_rgt', N'01', N'en', N'Non-local'),
    (N'emp_rgt', N'01', N'zh-TW', N'外地'),
    (N'emp_rgt', N'01', N'th', N'ต่างจังหวัด'),
    (N'emp_rgt', N'01', N'vi', N'Ngoại tỉnh'),
    (N'emp_rgt', N'02', N'en', N'Local'),
    (N'emp_rgt', N'02', N'zh-TW', N'本地'),
    (N'emp_rgt', N'02', N'th', N'ท้องถิ่น'),
    (N'emp_rgt', N'02', N'vi', N'Địa phương'),
    (N'emp_rgt', N'03', N'en', N'Foreign'),
    (N'emp_rgt', N'03', N'zh-TW', N'外國籍'),
    (N'emp_rgt', N'03', N'th', N'ชาวต่างชาติ'),
    (N'emp_rgt', N'03', N'vi', N'Nước ngoài'),

    (N'emp_marry', N'已婚', N'en', N'Married'),
    (N'emp_marry', N'已婚', N'zh-TW', N'已婚'),
    (N'emp_marry', N'已婚', N'th', N'แต่งงานแล้ว'),
    (N'emp_marry', N'已婚', N'vi', N'Đã kết hôn'),
    (N'emp_marry', N'未婚', N'en', N'Single'),
    (N'emp_marry', N'未婚', N'zh-TW', N'未婚'),
    (N'emp_marry', N'未婚', N'th', N'โสด'),
    (N'emp_marry', N'未婚', N'vi', N'Chưa kết hôn'),
    (N'emp_marry', N'丧偶', N'en', N'Widowed'),
    (N'emp_marry', N'丧偶', N'zh-TW', N'喪偶'),
    (N'emp_marry', N'丧偶', N'th', N'หม้าย'),
    (N'emp_marry', N'丧偶', N'vi', N'Góa'),
    (N'emp_marry', N'离异', N'en', N'Divorced'),
    (N'emp_marry', N'离异', N'zh-TW', N'離異'),
    (N'emp_marry', N'离异', N'th', N'หย่า'),
    (N'emp_marry', N'离异', N'vi', N'Ly hôn'),
    (N'emp_marry', N'离婚', N'en', N'Divorced'),
    (N'emp_marry', N'离婚', N'zh-TW', N'離婚'),
    (N'emp_marry', N'离婚', N'th', N'หย่าร้าง'),
    (N'emp_marry', N'离婚', N'vi', N'Ly hôn')
) AS s(code_field, code_value, culture, code_desc)
ON t.code_field = s.code_field AND t.code_value = s.code_value AND t.culture = s.culture
WHEN MATCHED THEN
    UPDATE SET code_desc = s.code_desc
WHEN NOT MATCHED THEN
    INSERT (code_field, code_value, culture, code_desc)
    VALUES (s.code_field, s.code_value, s.culture, s.code_desc);
