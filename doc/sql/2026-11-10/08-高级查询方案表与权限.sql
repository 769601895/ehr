/*
 * 高级查询方案表 + 人员资料页方案权限
 * 日期：2026-11-10  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.hr_adv_query_scheme', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.hr_adv_query_scheme
    (
        Id             bigint         NOT NULL,
        module_code    nvarchar(64)   NOT NULL,
        name           nvarchar(100)  NOT NULL,
        filter_json    nvarchar(max)  NOT NULL,
        owner_user_id  bigint         NOT NULL,
        is_shared      bit            NOT NULL CONSTRAINT DF_hr_adv_query_scheme_share DEFAULT (0),
        share_role_ids nvarchar(1000) NULL,
        CreateTime     datetime       NULL,
        UpdateTime     datetime       NULL,
        CreateUserId   bigint         NULL,
        CreateUserName nvarchar(64)   NULL,
        UpdateUserId   bigint         NULL,
        UpdateUserName nvarchar(64)   NULL,
        IsDelete       bit            NOT NULL CONSTRAINT DF_hr_adv_query_scheme_del DEFAULT (0),
        DeleteTime     datetime       NULL,
        CONSTRAINT PK_hr_adv_query_scheme PRIMARY KEY (Id)
    );
END
ELSE IF COL_LENGTH(N'dbo.hr_adv_query_scheme', N'DeleteTime') IS NULL
    ALTER TABLE dbo.hr_adv_query_scheme ADD DeleteTime datetime NULL;

IF OBJECT_ID(N'dbo.hr_adv_query_scheme', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_hr_adv_query_scheme_mod_owner' AND object_id = OBJECT_ID(N'dbo.hr_adv_query_scheme'))
    CREATE INDEX IX_hr_adv_query_scheme_mod_owner ON dbo.hr_adv_query_scheme (module_code, owner_user_id) WHERE IsDelete = 0;

-- 权限挂在人员资料管理下（Pid=1300000000221）
DECLARE @now DATETIME = GETDATE();
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @pid BIGINT = 1300000000221;

-- 注意：勿占用 1300000000241（人事资料定义目录）及 0244/0245；方案权限用 0261~0264
;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000261, @pid, N'hrAdvQuerySchemeList', N'高级查询方案列表', N'hrAdvQueryScheme/list', 200),
        (1300000000262, @pid, N'hrAdvQuerySchemeSave', N'保存高级查询方案', N'hrAdvQueryScheme/save', 210),
        (1300000000263, @pid, N'hrAdvQuerySchemeDelete', N'删除高级查询方案', N'hrAdvQueryScheme/delete', 220),
        (1300000000264, @pid, N'hrAdvQuerySchemeDetail', N'高级查询方案详情', N'hrAdvQueryScheme/detail', 230)
    ) AS v(Id, Pid, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, s.Pid, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now)
WHEN MATCHED THEN
    UPDATE SET Pid = s.Pid, Type = 3, Permission = s.Permission, Title = s.Title, Name = s.Name, IsDelete = 0, Status = 1, OrderNo = s.OrderNo, Path = NULL, Component = NULL, UpdateTime = @now;

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000002261, 1300000000261),
        (1300000002262, 1300000000262),
        (1300000002263, 1300000000263),
        (1300000002264, 1300000000264)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);
