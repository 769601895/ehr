-- 用户人事管辖部门（Usercc）— 在人事库（与 cc_mstr 同库）执行
-- 只存管辖根 CcCode；查询时按当前 cc_level 前缀匹配
-- 日期：2026-11-05
-- 数据迁移：保存账号时自动同步；也可调用接口 POST /api/hrUserCcScope/syncAllFromExtOrg（需管理员）

IF OBJECT_ID(N'dbo.Usercc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usercc
    (
        Id            BIGINT         NOT NULL,
        UserId        BIGINT         NOT NULL,
        CcCode        NVARCHAR(50)   NOT NULL,
        IncludeChild  BIT            NOT NULL CONSTRAINT DF_Usercc_IncludeChild DEFAULT (1),
        CreateTime    DATETIME2(7)   NULL,
        CreateUserId  BIGINT         NULL,
        CONSTRAINT PK_Usercc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UK_Usercc_User_Cc UNIQUE (UserId, CcCode)
    );

    CREATE INDEX IX_Usercc_UserId ON dbo.Usercc (UserId);
    CREATE INDEX IX_Usercc_CcCode ON dbo.Usercc (CcCode);
END
GO
