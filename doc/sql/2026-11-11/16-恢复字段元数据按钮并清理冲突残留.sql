/*
 * 变更说明：
 * 1) 高级查询方案误占 0261~0264（原为字段元数据按钮）→ 字段元数据按钮改用 0540~0543 重建
 * 2) 清理挂在业务界面配置(0500)下、已软删的人事月报残留按钮
 * 日期：2026-11-11
 * 影响库：主库 SysMenu / SysRoleMenu
 * 是否幂等：是
 *
 * 说明：0261~0264 现已作为高级查询方案权限使用，本次不回迁，避免再冲掉人员资料高级查询。
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @fieldDefId BIGINT = 1300000000260;
DECLARE @roleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @fieldDefId AND IsDelete = 0)
BEGIN
    RAISERROR(N'字段元数据菜单 1300000000260 不存在或已删除，请先执行 07/15 恢复。', 16, 1);
    RETURN;
END

;WITH btns AS (
    SELECT * FROM (VALUES
        (1300000000540, N'hrUiFieldDefPage', N'查询', N'hrUiFieldDef/page', 100),
        (1300000000541, N'hrUiFieldDefUpdate', N'编辑', N'hrUiFieldDef/update', 110),
        (1300000000542, N'hrUiFieldDefSync', N'同步', N'hrUiFieldDef/syncFromDb', 120),
        (1300000000543, N'hrUiFieldDefLang', N'译文', N'hrUiFieldDef/saveLang', 130)
    ) AS v(Id, Name, Title, Permission, OrderNo)
)
MERGE dbo.SysMenu AS t
USING btns AS s ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET
    Pid = @fieldDefId, Type = 3, Name = s.Name, Title = s.Title, Permission = s.Permission,
    OrderNo = s.OrderNo, IsDelete = 0, Status = 1, IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0,
    Path = NULL, Component = NULL, UpdateTime = @now
WHEN NOT MATCHED THEN INSERT (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (s.Id, @fieldDefId, 3, s.Name, s.Title, s.Permission, 0, 0, 1, 0, s.OrderNo, 1, 0, @now);

;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000005400, 1300000000540),
        (1300000005401, 1300000000541),
        (1300000005402, 1300000000542),
        (1300000005403, 1300000000543)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @roleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @roleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

/* 业务界面配置下误挂的人事月报软删残留：断开父子，避免配置页展开看到垃圾 */
UPDATE dbo.SysMenu
SET Pid = 0, UpdateTime = @now
WHERE Pid = 1300000000500
  AND IsDelete = 1
  AND (Name LIKE N'hrEmpMonth%' OR Permission LIKE N'hrEmpMonth/%');

SELECT Id, Pid, Type, Name, Title, Permission, IsDelete, Status
FROM dbo.SysMenu
WHERE Id = @fieldDefId OR Pid = @fieldDefId
ORDER BY OrderNo, Id;
GO
