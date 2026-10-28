/*
 * 变更说明：新建岗位代码表 hr_post（系统代码设置 · 岗位代码设置）
 * 模块：hrPost
 * 日期：2026-10-28
 * 影响库：ehr（HR 库，与 hr_wf_* 同库）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_post', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_post
    (
        Id             BIGINT         NOT NULL,
        post_code      NVARCHAR(50)   NOT NULL,
        post_name      NVARCHAR(100)  NOT NULL,
        grade          INT            NOT NULL CONSTRAINT DF_hr_post_grade DEFAULT (0),
        status         NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_post_status DEFAULT (N'Active'),
        cc_code        NVARCHAR(64)   NULL,
        need_audit     BIT            NOT NULL CONSTRAINT DF_hr_post_need_audit DEFAULT (1),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_post_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_post PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_post_code ON dbo.hr_post (post_code) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_post_status ON dbo.hr_post (status);
    CREATE INDEX IX_hr_post_cc ON dbo.hr_post (cc_code);
END
GO

PRINT N'hr_post 表已就绪';
