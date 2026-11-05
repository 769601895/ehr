/*
 * 人员资料：国籍 emp_country、籍贯市 emp_city；emp_province 存省码；
 * 码表 emp_country；字段池/场景/译文；省市查找数据源。
 * 幂等。
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();

-- ========== 主档列 ==========
IF COL_LENGTH(N'dbo.h_emp_mstr', N'emp_country') IS NULL
    ALTER TABLE dbo.h_emp_mstr ADD emp_country NVARCHAR(20) NULL;

IF COL_LENGTH(N'dbo.h_emp_mstr', N'emp_city') IS NULL
    ALTER TABLE dbo.h_emp_mstr ADD emp_city NVARCHAR(12) NULL;

-- ========== 国籍码表 ==========
MERGE dbo.code_mstr AS t
USING (VALUES
    (N'emp_country', N'CN', N'中国', N'01'),
    (N'emp_country', N'HK', N'中国香港', N'02'),
    (N'emp_country', N'MO', N'中国澳门', N'03'),
    (N'emp_country', N'TW', N'中国台湾', N'04'),
    (N'emp_country', N'US', N'美国', N'11'),
    (N'emp_country', N'JP', N'日本', N'12'),
    (N'emp_country', N'KR', N'韩国', N'13'),
    (N'emp_country', N'VN', N'越南', N'14'),
    (N'emp_country', N'TH', N'泰国', N'15'),
    (N'emp_country', N'SG', N'新加坡', N'16'),
    (N'emp_country', N'MY', N'马来西亚', N'17'),
    (N'emp_country', N'OTHER', N'其他', N'99')
) AS s(code_field, code_value, code_desc, code_sort)
ON t.code_field = s.code_field AND t.code_value = s.code_value
WHEN MATCHED THEN
    UPDATE SET code_desc = s.code_desc, code_sort = s.code_sort
WHEN NOT MATCHED THEN
    INSERT (code_field, code_value, code_desc, code_sort)
    VALUES (s.code_field, s.code_value, s.code_desc, s.code_sort);

-- 国籍译文（有 code_mstr_lang 时）
IF OBJECT_ID(N'dbo.code_mstr_lang', N'U') IS NOT NULL
BEGIN
    MERGE dbo.code_mstr_lang AS t
    USING (VALUES
        (N'emp_country', N'CN', N'en', N'China'),
        (N'emp_country', N'CN', N'zh-TW', N'中國'),
        (N'emp_country', N'CN', N'th', N'จีน'),
        (N'emp_country', N'CN', N'vi', N'Trung Quốc'),
        (N'emp_country', N'HK', N'en', N'Hong Kong, China'),
        (N'emp_country', N'HK', N'zh-TW', N'中國香港'),
        (N'emp_country', N'MO', N'en', N'Macao, China'),
        (N'emp_country', N'MO', N'zh-TW', N'中國澳門'),
        (N'emp_country', N'TW', N'en', N'Taiwan, China'),
        (N'emp_country', N'TW', N'zh-TW', N'中國台灣'),
        (N'emp_country', N'US', N'en', N'United States'),
        (N'emp_country', N'US', N'zh-TW', N'美國'),
        (N'emp_country', N'JP', N'en', N'Japan'),
        (N'emp_country', N'JP', N'zh-TW', N'日本'),
        (N'emp_country', N'KR', N'en', N'South Korea'),
        (N'emp_country', N'KR', N'zh-TW', N'韓國'),
        (N'emp_country', N'VN', N'en', N'Vietnam'),
        (N'emp_country', N'VN', N'zh-TW', N'越南'),
        (N'emp_country', N'VN', N'vi', N'Việt Nam'),
        (N'emp_country', N'TH', N'en', N'Thailand'),
        (N'emp_country', N'TH', N'zh-TW', N'泰國'),
        (N'emp_country', N'TH', N'th', N'ไทย'),
        (N'emp_country', N'SG', N'en', N'Singapore'),
        (N'emp_country', N'SG', N'zh-TW', N'新加坡'),
        (N'emp_country', N'MY', N'en', N'Malaysia'),
        (N'emp_country', N'MY', N'zh-TW', N'馬來西亞'),
        (N'emp_country', N'OTHER', N'en', N'Other'),
        (N'emp_country', N'OTHER', N'zh-TW', N'其他'),
        (N'emp_country', N'OTHER', N'th', N'อื่นๆ'),
        (N'emp_country', N'OTHER', N'vi', N'Khác')
    ) AS s(code_field, code_value, culture, code_desc)
    ON t.code_field = s.code_field AND t.code_value = s.code_value AND t.culture = s.culture
    WHEN MATCHED THEN UPDATE SET code_desc = s.code_desc
    WHEN NOT MATCHED THEN
        INSERT (code_field, code_value, culture, code_desc)
        VALUES (s.code_field, s.code_value, s.culture, s.code_desc);
END;

-- ========== 查找数据源：省 / 市（列表展示用；表单省市级联走专用接口）==========
IF NOT EXISTS (SELECT 1 FROM dbo.hr_lookup_def WHERE code = N'hrProvince' AND IsDelete = 0)
    INSERT INTO dbo.hr_lookup_def
    (Id, code, name, table_name, value_column, label_column, order_column,
     filter_use_is_delete, filter_enabled_column, filter_enabled_value,
     lang_table, lang_value_column, lang_culture_column, lang_label_column,
     maintain_path, enabled, CreateTime, IsDelete)
    VALUES
    (1300000003302, N'hrProvince', N'省份代码', N'hr_province', N'province_code', N'province_name', N'order_no',
     1, N'status', N'Active',
     N'hr_province_lang', N'province_code', N'culture', N'province_name',
     N'/sysCode/hrProvince', 1, @now, 0);
ELSE
    UPDATE dbo.hr_lookup_def
    SET name = N'省份代码', table_name = N'hr_province', value_column = N'province_code', label_column = N'province_name',
        order_column = N'order_no', filter_use_is_delete = 1, filter_enabled_column = N'status', filter_enabled_value = N'Active',
        lang_table = N'hr_province_lang', lang_value_column = N'province_code', lang_culture_column = N'culture', lang_label_column = N'province_name',
        maintain_path = N'/sysCode/hrProvince', enabled = 1, UpdateTime = @now
    WHERE code = N'hrProvince' AND IsDelete = 0;

IF NOT EXISTS (SELECT 1 FROM dbo.hr_lookup_def WHERE code = N'hrCity' AND IsDelete = 0)
    INSERT INTO dbo.hr_lookup_def
    (Id, code, name, table_name, value_column, label_column, order_column,
     filter_use_is_delete, filter_enabled_column, filter_enabled_value,
     lang_table, lang_value_column, lang_culture_column, lang_label_column,
     maintain_path, enabled, CreateTime, IsDelete)
    VALUES
    (1300000003303, N'hrCity', N'城市代码', N'hr_city', N'city_code', N'city_name', N'order_no',
     1, N'status', N'Active',
     N'hr_city_lang', N'city_code', N'culture', N'city_name',
     N'/sysCode/hrProvince', 1, @now, 0);
ELSE
    UPDATE dbo.hr_lookup_def
    SET name = N'城市代码', table_name = N'hr_city', value_column = N'city_code', label_column = N'city_name',
        order_column = N'order_no', filter_use_is_delete = 1, filter_enabled_column = N'status', filter_enabled_value = N'Active',
        lang_table = N'hr_city_lang', lang_value_column = N'city_code', lang_culture_column = N'culture', lang_label_column = N'city_name',
        maintain_path = N'/sysCode/hrProvince', enabled = 1, UpdateTime = @now
    WHERE code = N'hrCity' AND IsDelete = 0;

-- ========== 字段池 ==========
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCountry' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def
    (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type,
     default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime, code_field)
VALUES
    (1400000001090, N'hrEmp', N'empCountry', N'physical', N'emp_country', N'h_emp_mstr', N'国籍', N'nvarchar', 1, N'select',
     90, 1, N'basic', 1, 1, 0, 0, 0, 12, 1, 0, @now, N'emp_country');

IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_def WHERE module_code = N'hrEmp' AND field_key = N'empCity' AND IsDelete = 0)
INSERT INTO dbo.hr_ui_field_def
    (Id, module_code, field_key, origin, db_column, table_name, label, data_type, is_nullable, control_type,
     default_width_px, default_span_slots, group_code, is_in_pool, sortable, required, [readonly], locked, sort_no, enabled, IsDelete, CreateTime, lookup_source)
VALUES
    (1400000001091, N'hrEmp', N'empCity', N'physical', N'emp_city', N'h_emp_mstr', N'籍贯市', N'nvarchar', 1, N'select',
     100, 1, N'basic', 1, 1, 0, 0, 0, 15, 1, 0, @now, N'hrCity');

UPDATE dbo.hr_ui_field_def
SET label = N'籍贯省',
    control_type = N'select',
    code_field = NULL,
    lookup_source = N'hrProvince',
    UpdateTime = @now
WHERE module_code = N'hrEmp' AND field_key = N'empProvince' AND IsDelete = 0;

UPDATE dbo.hr_ui_field_def
SET code_field = N'emp_country',
    lookup_source = NULL,
    control_type = N'select',
    UpdateTime = @now
WHERE module_code = N'hrEmp' AND field_key = N'empCountry' AND IsDelete = 0;

-- 场景
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCountry' AND scene = N'form')
    INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCountry', N'form');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCountry' AND scene = N'list')
    INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCountry', N'list');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCity' AND scene = N'form')
    INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCity', N'form');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_field_scene WHERE module_code = N'hrEmp' AND field_key = N'empCity' AND scene = N'list')
    INSERT INTO dbo.hr_ui_field_scene (module_code, field_key, scene) VALUES (N'hrEmp', N'empCity', N'list');

-- 字段池译文
IF OBJECT_ID(N'dbo.hr_ui_field_def_lang', N'U') IS NOT NULL
BEGIN
    MERGE dbo.hr_ui_field_def_lang AS t
    USING (VALUES
        (N'hrEmp', N'empCountry', N'en', N'Nationality'),
        (N'hrEmp', N'empCountry', N'zh-TW', N'國籍'),
        (N'hrEmp', N'empCountry', N'th', N'สัญชาติ'),
        (N'hrEmp', N'empCountry', N'vi', N'Quốc tịch'),
        (N'hrEmp', N'empCity', N'en', N'Native City'),
        (N'hrEmp', N'empCity', N'zh-TW', N'籍貫市'),
        (N'hrEmp', N'empCity', N'th', N'เมืองบ้านเกิด'),
        (N'hrEmp', N'empCity', N'vi', N'Thành phố quê quán'),
        (N'hrEmp', N'empProvince', N'en', N'Native Province'),
        (N'hrEmp', N'empProvince', N'zh-TW', N'籍貫省'),
        (N'hrEmp', N'empProvince', N'th', N'จังหวัดบ้านเกิด'),
        (N'hrEmp', N'empProvince', N'vi', N'Tỉnh quê quán')
    ) AS s(module_code, field_key, culture, label)
    ON t.module_code = s.module_code AND t.field_key = s.field_key AND t.culture = s.culture
    WHEN MATCHED THEN UPDATE SET label = s.label
    WHEN NOT MATCHED THEN
        INSERT (module_code, field_key, culture, label) VALUES (s.module_code, s.field_key, s.culture, s.label);
END;

-- 默认表单布局：插入国籍/籍贯市（若布局存在且尚未有该字段）
DECLARE @formId BIGINT =
(
    SELECT TOP 1 Id FROM dbo.hr_ui_layout
    WHERE module_code = N'hrEmp' AND scene = N'form' AND IsDelete = 0
    ORDER BY is_default DESC, Id
);

IF @formId IS NOT NULL
BEGIN
    DECLARE @sortProv INT =
    (
        SELECT TOP 1 sort_no FROM dbo.hr_ui_layout_field
        WHERE layout_id = @formId AND field_key = N'empProvince' AND IsDelete = 0
    );

    IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @formId AND field_key = N'empCountry' AND IsDelete = 0)
    BEGIN
        INSERT INTO dbo.hr_ui_layout_field
            (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
        VALUES
            (1400000002190, @formId, N'empCountry', N'emp_country', N'国籍',
             ISNULL(@sortProv, 100) - 1, 1, NULL, 1, N'basic', N'select', 0, 0, @now, 0);
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.hr_ui_layout_field WHERE layout_id = @formId AND field_key = N'empCity' AND IsDelete = 0)
    BEGIN
        INSERT INTO dbo.hr_ui_layout_field
            (Id, layout_id, field_key, db_column, label, sort_no, visible, width_px, span_slots, group_code, control_type, required, [readonly], CreateTime, IsDelete)
        VALUES
            (1400000002191, @formId, N'empCity', N'emp_city', N'籍贯市',
             ISNULL(@sortProv, 100) + 1, 1, NULL, 1, N'basic', N'select', 0, 0, @now, 0);
    END;

    UPDATE lf
    SET control_type = N'select', label = N'籍贯省', UpdateTime = @now
    FROM dbo.hr_ui_layout_field lf
    WHERE lf.layout_id = @formId AND lf.field_key = N'empProvince' AND lf.IsDelete = 0;
END;

PRINT N'09-人员身份证国籍省市字段 完成';
