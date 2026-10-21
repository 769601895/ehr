/*
 * 变更说明：人事 UI 字段池
 * 模块：hrUiFieldDef
 * 日期：2026-10-17
 * 影响库：ehr
 * 是否幂等：是
 */

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.hr_ui_field_def', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_ui_field_def
    (
        Id                BIGINT         NOT NULL,
        module_code       NVARCHAR(64)   NOT NULL,
        field_key         NVARCHAR(64)   NOT NULL,
        origin            NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_ui_field_def_origin DEFAULT (N'physical'),
        db_column         NVARCHAR(64)   NULL,
        table_name        NVARCHAR(128)  NULL,
        label             NVARCHAR(256)  NOT NULL,
        data_type         NVARCHAR(50)   NULL,
        is_nullable       BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_nullable DEFAULT (1),
        control_type      NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_ui_field_def_ctrl DEFAULT (N'text'),
        code_field        NVARCHAR(50)   NULL,
        default_width_px  INT            NULL,
        default_span_slots INT           NOT NULL CONSTRAINT DF_hr_ui_field_def_span DEFAULT (1),
        group_code        NVARCHAR(20)   NULL,
        is_in_pool        BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_pool DEFAULT (1),
        sortable          BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_sort DEFAULT (1),
        required          BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_req DEFAULT (0),
        [readonly]        BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_ro DEFAULT (0),
        locked            BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_lock DEFAULT (0),
        sort_no           INT            NOT NULL CONSTRAINT DF_hr_ui_field_def_sortno DEFAULT (0),
        enabled           BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_enabled DEFAULT (1),
        CreateTime        DATETIME       NULL,
        UpdateTime        DATETIME       NULL,
        CreateUserId      BIGINT         NULL,
        CreateUserName    NVARCHAR(64)   NULL,
        UpdateUserId      BIGINT         NULL,
        UpdateUserName    NVARCHAR(64)   NULL,
        IsDelete          BIT            NOT NULL CONSTRAINT DF_hr_ui_field_def_IsDelete DEFAULT (0),
        DeleteTime        DATETIME       NULL,
        CONSTRAINT PK_hr_ui_field_def PRIMARY KEY CLUSTERED (Id)
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_hr_ui_field_def_key' AND object_id = OBJECT_ID(N'dbo.hr_ui_field_def')
)
    CREATE UNIQUE INDEX UX_hr_ui_field_def_key
        ON dbo.hr_ui_field_def (module_code, field_key)
        WHERE IsDelete = 0;
