-- ============================================================
-- 修复：ehrnew.dbo.cc_mstr.ID 非 IDENTITY，导致 SqlSugar 新增时省略 ID，
--       SQL Server 报「不能将值 NULL 插入列 'ID'」。
-- 实体 CcMstr.CcIdentityId 已标记 IsIdentity=true，与绿库设计一致。
-- 本脚本：重建表使 ID 为 IDENTITY，保留原 ID/ParentID 及全部数据；重建 SysOrg 视图。
-- ============================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.cc_mstr', N'U') IS NULL
    THROW 50001, N'cc_mstr 不存在', 1;

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.cc_mstr')
      AND name = N'ID'
      AND is_identity = 1
)
BEGIN
    PRINT N'cc_mstr.ID 已是 IDENTITY，无需处理。';
    RETURN;
END;

BEGIN TRAN;

IF OBJECT_ID(N'dbo.SysOrg', N'V') IS NOT NULL
    DROP VIEW dbo.SysOrg;

IF OBJECT_ID(N'dbo.cc_mstr__bak_id', N'U') IS NOT NULL
    DROP TABLE dbo.cc_mstr__bak_id;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'UX_cc_mstr_ID')
    DROP INDEX UX_cc_mstr_ID ON dbo.cc_mstr;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'IX_cc_mstr_cc_enable')
    DROP INDEX IX_cc_mstr_cc_enable ON dbo.cc_mstr;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'IX_cc_mstr_cc_code_parent')
    DROP INDEX IX_cc_mstr_cc_code_parent ON dbo.cc_mstr;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'IX_cc_mstr_ParentID')
    DROP INDEX IX_cc_mstr_ParentID ON dbo.cc_mstr;
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'IX_cc_mstr_cc_level')
    DROP INDEX IX_cc_mstr_cc_level ON dbo.cc_mstr;
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'PK_cc_mstr')
    ALTER TABLE dbo.cc_mstr DROP CONSTRAINT PK_cc_mstr;

EXEC sp_rename N'dbo.cc_mstr', N'cc_mstr__bak_id';

-- 新表默认约束用不冲突的名称（旧 DF_cc_mstr_* 仍挂在 bak 表上）
CREATE TABLE dbo.cc_mstr (
    ID               bigint         NOT NULL IDENTITY(1, 1),
    cc_code          nvarchar(50)   NOT NULL,
    ParentID         bigint         NULL,
    cc_code_parent   nvarchar(50)   NULL,
    cc_dept          nvarchar(50)   NULL,
    cc_desc          nvarchar(122)  NOT NULL,
    cc_level         nvarchar(50)   NOT NULL,
    cc_group         int            NULL,
    cc_indet         nvarchar(50)   NULL,
    cc_sub           float          NULL,
    cc_site          nvarchar(50)   NULL,
    cc_state         int            NULL,
    cc_cost          nvarchar(50)   NULL,
    cc_wage_rate     float          NULL,
    cc_stat_basic    nvarchar(50)   NULL,
    cc_enable        int            NULL CONSTRAINT DF_cc_mstr_cc_enable_v2 DEFAULT (0),
    cc_dele_date     datetime       NULL,
    cc_count_group   nvarchar(60)   NULL,
    cc_user          nvarchar(50)   NULL,
    cc_date          datetime       NULL,
    cc_kqgroup       int            NULL,
    cc_woca          nvarchar(50)   NULL,
    cc_bz_unit       nvarchar(50)   NULL,
    cc_dele_remark   nvarchar(120)  NULL,
    cc_desc_en       nvarchar(150)  NULL,
    cc_level_censor  nvarchar(50)   NULL,
    cc_xz_group      nvarchar(50)   NULL,
    cc_create_date   datetime       NULL,
    machine_floor    nvarchar(10)   NULL,
    kq_new_sysuser   nvarchar(25)   NULL,
    kq_new_sysdate   datetime       NULL,
    cc_code2         nvarchar(50)   NULL,
    org_type         nvarchar(64)   NULL CONSTRAINT DF_cc_mstr_org_type_v2 DEFAULT (N'201'),
    director_id      bigint         NULL,
    longitude        decimal(18, 4) NOT NULL CONSTRAINT DF_cc_mstr_longitude_v2 DEFAULT (0),
    latitude         decimal(18, 4) NOT NULL CONSTRAINT DF_cc_mstr_latitude_v2 DEFAULT (0),
    order_no         int            NOT NULL CONSTRAINT DF_cc_mstr_order_no_v2 DEFAULT (100),
    remark           nvarchar(128)  NULL,
    tenant_id        bigint         NULL,
    create_user_id   bigint         NULL,
    update_user_id   bigint         NULL,
    update_user_name nvarchar(64)   NULL,
    CONSTRAINT PK_cc_mstr PRIMARY KEY CLUSTERED (cc_code)
);

SET IDENTITY_INSERT dbo.cc_mstr ON;

INSERT INTO dbo.cc_mstr (
    ID, cc_code, ParentID, cc_code_parent, cc_dept, cc_desc, cc_level,
    cc_group, cc_indet, cc_sub, cc_site, cc_state, cc_cost, cc_wage_rate, cc_stat_basic,
    cc_enable, cc_dele_date, cc_count_group, cc_user, cc_date, cc_kqgroup, cc_woca, cc_bz_unit,
    cc_dele_remark, cc_desc_en, cc_level_censor, cc_xz_group, cc_create_date,
    machine_floor, kq_new_sysuser, kq_new_sysdate, cc_code2,
    org_type, director_id, longitude, latitude, order_no, remark,
    tenant_id, create_user_id, update_user_id, update_user_name
)
SELECT
    ID, cc_code, ParentID, cc_code_parent, cc_dept, cc_desc, cc_level,
    cc_group, cc_indet, cc_sub, cc_site, cc_state, cc_cost, cc_wage_rate, cc_stat_basic,
    cc_enable, cc_dele_date, cc_count_group, cc_user, cc_date, cc_kqgroup, cc_woca, cc_bz_unit,
    cc_dele_remark, cc_desc_en, cc_level_censor, cc_xz_group, cc_create_date,
    machine_floor, kq_new_sysuser, kq_new_sysdate, cc_code2,
    org_type, director_id, longitude, latitude, order_no, remark,
    tenant_id, create_user_id, update_user_id, update_user_name
FROM dbo.cc_mstr__bak_id;

SET IDENTITY_INSERT dbo.cc_mstr OFF;

CREATE UNIQUE NONCLUSTERED INDEX UX_cc_mstr_ID ON dbo.cc_mstr (ID);
CREATE NONCLUSTERED INDEX IX_cc_mstr_cc_enable ON dbo.cc_mstr (cc_enable);
CREATE NONCLUSTERED INDEX IX_cc_mstr_cc_code_parent ON dbo.cc_mstr (cc_code_parent);
CREATE NONCLUSTERED INDEX IX_cc_mstr_ParentID ON dbo.cc_mstr (ParentID);
CREATE NONCLUSTERED INDEX IX_cc_mstr_cc_level ON dbo.cc_mstr (cc_level);

DECLARE @maxId bigint;
SELECT @maxId = ISNULL(MAX(ID), 0) FROM dbo.cc_mstr;
DBCC CHECKIDENT (N'dbo.cc_mstr', RESEED, @maxId);

DECLARE @oldCnt int, @newCnt int;
SELECT @oldCnt = COUNT(*) FROM dbo.cc_mstr__bak_id;
SELECT @newCnt = COUNT(*) FROM dbo.cc_mstr;
IF @oldCnt <> @newCnt
BEGIN
    DECLARE @msg nvarchar(200);
    SET @msg = N'行数不一致：bak=' + CAST(@oldCnt AS nvarchar(20)) + N', new=' + CAST(@newCnt AS nvarchar(20));
    THROW 50002, @msg, 1;
END;

DROP TABLE dbo.cc_mstr__bak_id;

EXEC(N'
CREATE VIEW [dbo].[SysOrg] AS
SELECT
    CAST(c.ID AS BIGINT) AS Id,
    CAST(ISNULL(c.ParentID, 0) AS BIGINT) AS Pid,
    CAST(LEFT(c.cc_desc, 64) AS NVARCHAR(64)) AS [Name],
    CAST(c.cc_code AS NVARCHAR(64)) AS Code,
    CAST(LEN(c.cc_level) / 2 AS INT) AS [Level],
    CAST(ISNULL(c.org_type, N''301'') AS NVARCHAR(64)) AS [Type],
    c.director_id AS DirectorId,
    ISNULL(c.longitude, CAST(0 AS DECIMAL(18,4))) AS Longitude,
    ISNULL(c.latitude, CAST(0 AS DECIMAL(18,4))) AS Latitude,
    ISNULL(c.order_no, 100) AS OrderNo,
    CASE WHEN ISNULL(c.cc_enable, 0) = 0 THEN 1 ELSE 2 END AS [Status],
    CAST(c.remark AS NVARCHAR(128)) AS Remark,
    c.tenant_id AS TenantId,
    c.cc_create_date AS CreateTime,
    c.cc_date AS UpdateTime,
    c.create_user_id AS CreateUserId,
    CAST(c.cc_user AS NVARCHAR(64)) AS CreateUserName,
    c.update_user_id AS UpdateUserId,
    CAST(ISNULL(c.update_user_name, c.cc_user) AS NVARCHAR(64)) AS UpdateUserName,
    CAST(ISNULL(c.cc_enable, 0) AS BIT) AS IsDelete,
    c.cc_dele_date AS DeleteTime
FROM dbo.cc_mstr AS c;
');

COMMIT;

PRINT N'完成：cc_mstr.ID 已改为 IDENTITY，现有 ID/ParentID 已保留，SysOrg 已重建。';
SELECT name, is_identity, is_nullable
FROM sys.columns
WHERE object_id = OBJECT_ID(N'dbo.cc_mstr') AND name = N'ID';
