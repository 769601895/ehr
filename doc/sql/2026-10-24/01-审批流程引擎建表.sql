/*
 * 变更说明：审批流程引擎 hr_wf_* 建表（P0）
 * 模块：hrWf
 * 日期：2026-10-24
 * 影响库：ehr（HR 库）
 * 是否幂等：是
 * 说明：Running 唯一用过滤索引；无过滤索引环境可依赖应用层校验（见契约 §3.9）
 */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

/* ---------- 公共审计列模板已内联到各表 ---------- */

IF OBJECT_ID(N'dbo.hr_wf_level', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_level
    (
        Id             BIGINT         NOT NULL,
        code           NVARCHAR(50)   NOT NULL,
        name           NVARCHAR(100)  NOT NULL,
        audit_order    INT            NOT NULL CONSTRAINT DF_hr_wf_level_audit_order DEFAULT (0),
        is_functional  BIT            NOT NULL CONSTRAINT DF_hr_wf_level_is_functional DEFAULT (0),
        order_no       INT            NOT NULL CONSTRAINT DF_hr_wf_level_order_no DEFAULT (100),
        enabled        BIT            NOT NULL CONSTRAINT DF_hr_wf_level_enabled DEFAULT (1),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_level_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_level PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_level_audit_order ON dbo.hr_wf_level (audit_order);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_role', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_role
    (
        Id             BIGINT         NOT NULL,
        level_id       BIGINT         NOT NULL,
        code           NVARCHAR(50)   NOT NULL,
        name           NVARCHAR(100)  NOT NULL,
        parent_id      BIGINT         NULL,
        order_no       INT            NOT NULL CONSTRAINT DF_hr_wf_role_order_no DEFAULT (100),
        enabled        BIT            NOT NULL CONSTRAINT DF_hr_wf_role_enabled DEFAULT (1),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_role_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_role PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_role_level ON dbo.hr_wf_role (level_id);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_role_scope', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_role_scope
    (
        Id             BIGINT         NOT NULL,
        role_id        BIGINT         NOT NULL,
        cc_code        NVARCHAR(64)   NOT NULL,
        include_child  BIT            NOT NULL CONSTRAINT DF_hr_wf_role_scope_include_child DEFAULT (1),
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_role_scope_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_role_scope PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_role_scope_role_cc ON dbo.hr_wf_role_scope (role_id, cc_code) WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.hr_wf_role_post', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_role_post
    (
        Id             BIGINT         NOT NULL,
        role_id        BIGINT         NOT NULL,
        post_id        NVARCHAR(50)   NOT NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_role_post_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_role_post PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_role_post ON dbo.hr_wf_role_post (role_id, post_id) WHERE IsDelete = 0;
END
GO

IF OBJECT_ID(N'dbo.hr_wf_form', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_form
    (
        Id             BIGINT         NOT NULL,
        biz_type       NVARCHAR(50)   NOT NULL,
        code           NVARCHAR(50)   NOT NULL,
        name           NVARCHAR(100)  NOT NULL,
        form_mode      NVARCHAR(10)   NOT NULL CONSTRAINT DF_hr_wf_form_mode DEFAULT (N'B'),
        version        INT            NOT NULL CONSTRAINT DF_hr_wf_form_version DEFAULT (1),
        enabled        BIT            NOT NULL CONSTRAINT DF_hr_wf_form_enabled DEFAULT (1),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_form_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_form PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_form_biz ON dbo.hr_wf_form (biz_type);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_form_field', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_form_field
    (
        Id             BIGINT         NOT NULL,
        form_id        BIGINT         NOT NULL,
        field_key      NVARCHAR(50)   NOT NULL,
        label          NVARCHAR(100)  NOT NULL,
        control_type   NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_wf_form_field_control DEFAULT (N'text'),
        field_source   NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_wf_form_field_source DEFAULT (N'Custom'),
        pool_field_key NVARCHAR(50)   NULL,
        required       BIT            NOT NULL CONSTRAINT DF_hr_wf_form_field_required DEFAULT (0),
        [readonly]     BIT            NOT NULL CONSTRAINT DF_hr_wf_form_field_readonly DEFAULT (0),
        sort_no        INT            NOT NULL CONSTRAINT DF_hr_wf_form_field_sort DEFAULT (0),
        group_code     NVARCHAR(20)   NULL,
        span_slots     INT            NULL,
        code_field     NVARCHAR(50)   NULL,
        default_value  NVARCHAR(256)  NULL,
        placeholder    NVARCHAR(100)  NULL,
        extra_json     NVARCHAR(MAX)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_form_field_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_form_field PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_form_field ON dbo.hr_wf_form_field (form_id, field_key) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_wf_form_field_sort ON dbo.hr_wf_form_field (form_id, sort_no);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_def', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_def
    (
        Id                        BIGINT         NOT NULL,
        biz_type                  NVARCHAR(50)   NOT NULL,
        code                      NVARCHAR(50)   NOT NULL,
        name                      NVARCHAR(100)  NOT NULL,
        form_id                   BIGINT         NOT NULL,
        version                   INT            NOT NULL CONSTRAINT DF_hr_wf_def_version DEFAULT (1),
        enabled                   BIT            NOT NULL CONSTRAINT DF_hr_wf_def_enabled DEFAULT (1),
        allow_edit_while_running  BIT            NOT NULL CONSTRAINT DF_hr_wf_def_edit_run DEFAULT (0),
        edit_by_applicant         BIT            NOT NULL CONSTRAINT DF_hr_wf_def_edit_app DEFAULT (0),
        edit_by_assignee          BIT            NOT NULL CONSTRAINT DF_hr_wf_def_edit_asg DEFAULT (0),
        default_timeout_hours     INT            NOT NULL CONSTRAINT DF_hr_wf_def_timeout DEFAULT (0),
        remark                    NVARCHAR(256)  NULL,
        CreateTime                DATETIME       NULL,
        UpdateTime                DATETIME       NULL,
        CreateUserId              BIGINT         NULL,
        CreateUserName            NVARCHAR(64)   NULL,
        UpdateUserId              BIGINT         NULL,
        UpdateUserName            NVARCHAR(64)   NULL,
        IsDelete                  BIT            NOT NULL CONSTRAINT DF_hr_wf_def_IsDelete DEFAULT (0),
        DeleteTime                DATETIME       NULL,
        CONSTRAINT PK_hr_wf_def PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_def_biz ON dbo.hr_wf_def (biz_type);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_def_node', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_def_node
    (
        Id                  BIGINT         NOT NULL,
        def_id              BIGINT         NOT NULL,
        node_key            NVARCHAR(50)   NOT NULL,
        name                NVARCHAR(100)  NOT NULL,
        order_no            INT            NOT NULL,
        node_mode           NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_wf_def_node_mode DEFAULT (N'Or'),
        assignee_rule_json  NVARCHAR(MAX)  NOT NULL,
        condition_json      NVARCHAR(MAX)  NULL,
        timeout_hours       INT            NULL,
        enabled             BIT            NOT NULL CONSTRAINT DF_hr_wf_def_node_enabled DEFAULT (1),
        CreateTime          DATETIME       NULL,
        UpdateTime          DATETIME       NULL,
        CreateUserId        BIGINT         NULL,
        CreateUserName      NVARCHAR(64)   NULL,
        UpdateUserId        BIGINT         NULL,
        UpdateUserName      NVARCHAR(64)   NULL,
        IsDelete            BIT            NOT NULL CONSTRAINT DF_hr_wf_def_node_IsDelete DEFAULT (0),
        DeleteTime          DATETIME       NULL,
        CONSTRAINT PK_hr_wf_def_node PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_wf_def_node_key ON dbo.hr_wf_def_node (def_id, node_key) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_wf_def_node_order ON dbo.hr_wf_def_node (def_id, order_no);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_inst', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_inst
    (
        Id                        BIGINT         NOT NULL,
        biz_type                  NVARCHAR(50)   NOT NULL,
        biz_id                    NVARCHAR(64)   NOT NULL,
        def_id                    BIGINT         NOT NULL,
        form_id                   BIGINT         NOT NULL,
        status                    NVARCHAR(20)   NOT NULL,
        title                     NVARCHAR(200)  NULL,
        applicant_user_id         BIGINT         NOT NULL,
        subject_emp_id            NVARCHAR(50)   NULL,
        subject_cc_code           NVARCHAR(64)   NOT NULL,
        form_data_json            NVARCHAR(MAX)  NOT NULL,
        form_schema_snapshot      NVARCHAR(MAX)  NOT NULL,
        def_nodes_snapshot        NVARCHAR(MAX)  NOT NULL,
        current_node_key          NVARCHAR(50)   NULL,
        allow_edit_while_running  BIT            NOT NULL CONSTRAINT DF_hr_wf_inst_edit_run DEFAULT (0),
        edit_by_applicant         BIT            NOT NULL CONSTRAINT DF_hr_wf_inst_edit_app DEFAULT (0),
        edit_by_assignee          BIT            NOT NULL CONSTRAINT DF_hr_wf_inst_edit_asg DEFAULT (0),
        started_time              DATETIME       NOT NULL,
        finished_time             DATETIME       NULL,
        CreateTime                DATETIME       NULL,
        UpdateTime                DATETIME       NULL,
        CreateUserId              BIGINT         NULL,
        CreateUserName            NVARCHAR(64)   NULL,
        UpdateUserId              BIGINT         NULL,
        UpdateUserName            NVARCHAR(64)   NULL,
        IsDelete                  BIT            NOT NULL CONSTRAINT DF_hr_wf_inst_IsDelete DEFAULT (0),
        DeleteTime                DATETIME       NULL,
        CONSTRAINT PK_hr_wf_inst PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_inst_biz ON dbo.hr_wf_inst (biz_type, biz_id);
    CREATE UNIQUE INDEX UX_hr_wf_inst_running ON dbo.hr_wf_inst (biz_type, biz_id) WHERE status = N'Running' AND IsDelete = 0;
    CREATE INDEX IX_hr_wf_inst_applicant ON dbo.hr_wf_inst (applicant_user_id, status);
    CREATE INDEX IX_hr_wf_inst_status ON dbo.hr_wf_inst (status, started_time);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_task', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_task
    (
        Id             BIGINT         NOT NULL,
        inst_id        BIGINT         NOT NULL,
        node_key       NVARCHAR(50)   NOT NULL,
        assignee_id    BIGINT         NOT NULL,
        status         NVARCHAR(20)   NOT NULL,
        opinion        NVARCHAR(500)  NULL,
        claimed_time   DATETIME       NULL,
        finished_time  DATETIME       NULL,
        from_escalate  BIT            NOT NULL CONSTRAINT DF_hr_wf_task_escalate DEFAULT (0),
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_task_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_task PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_task_todo ON dbo.hr_wf_task (assignee_id, status);
    CREATE INDEX IX_hr_wf_task_inst ON dbo.hr_wf_task (inst_id, node_key);
END
GO

IF OBJECT_ID(N'dbo.hr_wf_action_log', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_wf_action_log
    (
        Id             BIGINT         NOT NULL,
        inst_id        BIGINT         NOT NULL,
        task_id        BIGINT         NULL,
        action_type    NVARCHAR(30)   NOT NULL,
        actor_user_id  BIGINT         NULL,
        node_key       NVARCHAR(50)   NULL,
        from_level_id  BIGINT         NULL,
        to_level_id    BIGINT         NULL,
        opinion        NVARCHAR(500)  NULL,
        detail_json    NVARCHAR(MAX)  NULL,
        action_time    DATETIME       NOT NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_wf_action_log_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_wf_action_log PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX IX_hr_wf_action_log_inst ON dbo.hr_wf_action_log (inst_id, action_time);
END
GO
