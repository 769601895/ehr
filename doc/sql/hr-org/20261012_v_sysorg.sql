/*
 * 变更说明：cc_mstr 补齐 SysOrg 同形列，并创建视图 dbo.V_SysOrg
 * 模块：组织视图
 * 日期：2026-10-12
 * 关联：doc/分析文档/组织视图/cc_mstr_to_sysorg_view_plan.md
 * 影响库：ehr
 * 是否幂等：是
 *
 * 约定：
 *   Id ← ID；Pid ← ParentID
 *   IsDelete ← cc_enable（0→0，1→1）
 *   DeleteTime ← cc_dele_date
 *   Status ← cc_enable（0→1，1→2）
 *   不加 sys_id / sys_pid / is_delete / delete_time
 */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

IF OBJECT_ID(N'dbo.cc_mstr', N'U') IS NULL
    THROW 50001, N'cc_mstr 不存在', 1;

IF COL_LENGTH(N'dbo.cc_mstr', N'org_type') IS NULL
    ALTER TABLE dbo.cc_mstr ADD org_type nvarchar(64) NULL
        CONSTRAINT DF_cc_mstr_org_type DEFAULT (N'201');
IF COL_LENGTH(N'dbo.cc_mstr', N'director_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD director_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'longitude') IS NULL
    ALTER TABLE dbo.cc_mstr ADD longitude decimal(18,4) NOT NULL
        CONSTRAINT DF_cc_mstr_longitude DEFAULT (0);
IF COL_LENGTH(N'dbo.cc_mstr', N'latitude') IS NULL
    ALTER TABLE dbo.cc_mstr ADD latitude decimal(18,4) NOT NULL
        CONSTRAINT DF_cc_mstr_latitude DEFAULT (0);
IF COL_LENGTH(N'dbo.cc_mstr', N'order_no') IS NULL
    ALTER TABLE dbo.cc_mstr ADD order_no int NOT NULL
        CONSTRAINT DF_cc_mstr_order_no DEFAULT (100);
IF COL_LENGTH(N'dbo.cc_mstr', N'remark') IS NULL
    ALTER TABLE dbo.cc_mstr ADD remark nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'tenant_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD tenant_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'create_user_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD create_user_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'update_user_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD update_user_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'update_user_name') IS NULL
    ALTER TABLE dbo.cc_mstr ADD update_user_name nvarchar(64) NULL;

COMMIT TRAN;
GO

CREATE OR ALTER VIEW dbo.V_SysOrg
AS
SELECT
    CAST(c.ID AS bigint)                                        AS Id,
    CAST(ISNULL(c.ParentID, 0) AS bigint)                       AS Pid,
    CAST(LEFT(c.cc_desc, 64) AS nvarchar(64))                   AS [Name],
    CAST(c.cc_code AS nvarchar(64))                             AS Code,
    CAST(LEN(c.cc_level) / 2 AS int)                            AS [Level],
    CAST(ISNULL(c.org_type, N'201') AS nvarchar(64))            AS [Type],
    c.director_id                                               AS DirectorId,
    ISNULL(c.longitude, CAST(0 AS decimal(18,4)))               AS Longitude,
    ISNULL(c.latitude,  CAST(0 AS decimal(18,4)))               AS Latitude,
    ISNULL(c.order_no, 100)                                     AS OrderNo,
    CASE WHEN ISNULL(c.cc_enable, 0) = 0 THEN 1 ELSE 2 END      AS [Status],
    CAST(c.remark AS nvarchar(128))                             AS Remark,
    c.tenant_id                                                 AS TenantId,
    c.cc_create_date                                            AS CreateTime,
    c.cc_date                                                   AS UpdateTime,
    c.create_user_id                                            AS CreateUserId,
    CAST(c.cc_user AS nvarchar(64))                             AS CreateUserName,
    c.update_user_id                                            AS UpdateUserId,
    CAST(ISNULL(c.update_user_name, c.cc_user) AS nvarchar(64)) AS UpdateUserName,
    CAST(ISNULL(c.cc_enable, 0) AS bit)                         AS IsDelete,
    c.cc_dele_date                                              AS DeleteTime
FROM dbo.cc_mstr AS c;
GO

/*
 * ROLLBACK：
 *
 * DROP VIEW IF EXISTS dbo.V_SysOrg;
 * -- 新列是否回滚按环境决定，此处不自动 DROP COLUMN
 */
