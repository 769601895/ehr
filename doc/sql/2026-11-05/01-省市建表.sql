/*
 * 变更说明：新建省市主数据表 hr_province / hr_city（系统代码设置 · 省市设置）
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 影响库：ehr（HR 库，与 hr_post 同库）
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_province', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_province
    (
        Id             BIGINT         NOT NULL,
        province_code  NVARCHAR(12)   NOT NULL,
        province_name  NVARCHAR(50)   NOT NULL,
        order_no       INT            NOT NULL CONSTRAINT DF_hr_province_order_no DEFAULT (100),
        status         NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_province_status DEFAULT (N'Active'),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_province_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_province PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_province_code ON dbo.hr_province (province_code) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_province_status ON dbo.hr_province (status);
END
GO

IF OBJECT_ID(N'dbo.hr_city', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_city
    (
        Id             BIGINT         NOT NULL,
        province_code  NVARCHAR(12)   NOT NULL,
        city_code      NVARCHAR(12)   NOT NULL,
        city_name      NVARCHAR(50)   NOT NULL,
        order_no       INT            NOT NULL CONSTRAINT DF_hr_city_order_no DEFAULT (100),
        status         NVARCHAR(20)   NOT NULL CONSTRAINT DF_hr_city_status DEFAULT (N'Active'),
        remark         NVARCHAR(256)  NULL,
        CreateTime     DATETIME       NULL,
        UpdateTime     DATETIME       NULL,
        CreateUserId   BIGINT         NULL,
        CreateUserName NVARCHAR(64)   NULL,
        UpdateUserId   BIGINT         NULL,
        UpdateUserName NVARCHAR(64)   NULL,
        IsDelete       BIT            NOT NULL CONSTRAINT DF_hr_city_IsDelete DEFAULT (0),
        DeleteTime     DATETIME       NULL,
        CONSTRAINT PK_hr_city PRIMARY KEY CLUSTERED (Id)
    );
    CREATE UNIQUE INDEX UX_hr_city_code ON dbo.hr_city (city_code) WHERE IsDelete = 0;
    CREATE INDEX IX_hr_city_province ON dbo.hr_city (province_code);
    CREATE INDEX IX_hr_city_status ON dbo.hr_city (status);
END
GO

PRINT N'hr_province / hr_city 表已就绪';
GO
