/*

 * 变更说明：人事 UI 布局方案表（列表列 / 表单占位）

 * 模块：hrUiLayout

 * 日期：2026-10-13

 * 影响库：ehr

 * 是否幂等：是

 */



SET NOCOUNT ON;



IF OBJECT_ID(N'dbo.hr_ui_layout', N'U') IS NULL

BEGIN

    CREATE TABLE dbo.hr_ui_layout

    (

        Id             BIGINT         NOT NULL,

        module_code    NVARCHAR(50)   NOT NULL,

        scene          NVARCHAR(20)   NOT NULL,

        name           NVARCHAR(100)  NOT NULL,

        is_default     BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_is_default DEFAULT (0),

        owner_user_id  BIGINT         NULL,

        version        INT            NOT NULL CONSTRAINT DF_hr_ui_layout_version DEFAULT (1),

        CreateTime     DATETIME       NULL,

        UpdateTime     DATETIME       NULL,

        CreateUserId   BIGINT         NULL,

        CreateUserName NVARCHAR(64)   NULL,

        UpdateUserId   BIGINT         NULL,

        UpdateUserName NVARCHAR(64)   NULL,

        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_IsDelete DEFAULT (0),

        DeleteTime     DATETIME       NULL,

        CONSTRAINT PK_hr_ui_layout PRIMARY KEY CLUSTERED (Id)

    );

    CREATE INDEX IX_hr_ui_layout_module_scene ON dbo.hr_ui_layout (module_code, scene, owner_user_id);

END;



IF OBJECT_ID(N'dbo.hr_ui_layout_field', N'U') IS NULL

BEGIN

    CREATE TABLE dbo.hr_ui_layout_field

    (

        Id             BIGINT         NOT NULL,

        layout_id      BIGINT         NOT NULL,

        field_key      NVARCHAR(50)   NOT NULL,

        db_column      NVARCHAR(50)   NULL,

        label          NVARCHAR(50)   NOT NULL,

        sort_no        INT            NOT NULL CONSTRAINT DF_hr_ui_layout_field_sort DEFAULT (0),

        visible        BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_field_visible DEFAULT (1),

        width_px       INT            NULL,

        span_slots     INT            NULL,

        group_code     NVARCHAR(20)   NULL,

        control_type   NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_ui_layout_field_ctrl DEFAULT (N'text'),

        required       BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_field_req DEFAULT (0),

        [readonly]     BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_field_ro DEFAULT (0),

        CreateTime     DATETIME       NULL,

        UpdateTime     DATETIME       NULL,

        CreateUserId   BIGINT         NULL,

        CreateUserName NVARCHAR(64)   NULL,

        UpdateUserId   BIGINT         NULL,

        UpdateUserName NVARCHAR(64)   NULL,

        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_ui_layout_field_IsDelete DEFAULT (0),

        DeleteTime     DATETIME       NULL,

        CONSTRAINT PK_hr_ui_layout_field PRIMARY KEY CLUSTERED (Id)

    );

    CREATE INDEX IX_hr_ui_layout_field_layout ON dbo.hr_ui_layout_field (layout_id, sort_no);

END;



SELECT 'hr_ui_layout' AS tbl, COUNT(*) AS cnt FROM dbo.hr_ui_layout

UNION ALL SELECT 'hr_ui_layout_field', COUNT(*) FROM dbo.hr_ui_layout_field;

