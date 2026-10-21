/*
 * 变更说明：布局字段按语言稀疏译文，并迁移已改过的中文名
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.hr_ui_layout_field_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_ui_layout_field_lang
    (
        layout_field_id BIGINT        NOT NULL,
        culture         NVARCHAR(16)  NOT NULL,
        label           NVARCHAR(256) NULL,
        CONSTRAINT PK_hr_ui_layout_field_lang PRIMARY KEY CLUSTERED (layout_field_id, culture)
    );
END;

/* 已有布局字段 Trim 后与池主语言不同的，插入 zh-CN 覆盖行。 */
IF OBJECT_ID(N'dbo.hr_ui_field_def', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.hr_ui_layout_field', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.hr_ui_layout_field_lang (layout_field_id, culture, label)
    SELECT f.Id, N'zh-CN', LTRIM(RTRIM(f.label))
    FROM dbo.hr_ui_layout_field f
    INNER JOIN dbo.hr_ui_layout h ON h.Id = f.layout_id AND h.IsDelete = 0
    INNER JOIN dbo.hr_ui_field_def d
        ON d.module_code = h.module_code
       AND d.field_key = f.field_key
       AND d.IsDelete = 0
    WHERE f.IsDelete = 0
      AND LTRIM(RTRIM(ISNULL(f.label, N''))) <> LTRIM(RTRIM(ISNULL(d.label, N'')))
      AND LTRIM(RTRIM(ISNULL(f.label, N''))) <> N''
      AND NOT EXISTS (
          SELECT 1 FROM dbo.hr_ui_layout_field_lang g
          WHERE g.layout_field_id = f.Id AND g.culture = N'zh-CN'
      );
END;
