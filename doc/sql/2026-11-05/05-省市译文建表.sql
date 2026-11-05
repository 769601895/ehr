/*
 * 变更说明：省市名称译文表 hr_province_lang / hr_city_lang
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_province_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_province_lang
    (
        province_code NVARCHAR(12)  NOT NULL,
        culture       NVARCHAR(16)  NOT NULL,
        province_name NVARCHAR(50)  NULL,
        CONSTRAINT PK_hr_province_lang PRIMARY KEY CLUSTERED (province_code, culture)
    );
END
GO

IF OBJECT_ID(N'dbo.hr_city_lang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_city_lang
    (
        city_code NVARCHAR(12)  NOT NULL,
        culture   NVARCHAR(16)  NOT NULL,
        city_name NVARCHAR(50)  NULL,
        CONSTRAINT PK_hr_city_lang PRIMARY KEY CLUSTERED (city_code, culture)
    );
END
GO

PRINT N'hr_province_lang / hr_city_lang 已就绪';
GO
