/*
 * 变更说明：sys_admin 授权岗位代码设置菜单与按钮
 * 模块：hrPost
 * 日期：2026-10-28
 * 影响库：ehr（SysRoleMenu）
 * 是否幂等：是
 * 前置：已执行 03-岗位代码菜单.sql
 *
 * 说明：RoleMenu.Id 使用 130000000318x（勿再用 1750，已与其它种子冲突）
 */
SET NOCOUNT ON;

DECLARE @RoleId BIGINT = 1300000000101;

;WITH grants AS (
    SELECT *
    FROM (VALUES
        (1300000003180, 1300000000310), -- 岗位代码设置
        (1300000003181, 1300000000311), -- 查询
        (1300000003182, 1300000000312), -- 保存
        (1300000003183, 1300000000313)  -- 删除
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @RoleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (
        SELECT 1 FROM dbo.SysRoleMenu rm
        WHERE rm.RoleId = @RoleId AND rm.MenuId = g.MenuId
    )
  AND NOT EXISTS (
        SELECT 1 FROM dbo.SysRoleMenu rm
        WHERE rm.Id = g.Id
    );

SELECT rm.Id, rm.RoleId, rm.MenuId, m.Title, m.Permission
FROM dbo.SysRoleMenu rm
INNER JOIN dbo.SysMenu m ON m.Id = rm.MenuId
WHERE rm.RoleId = @RoleId
  AND rm.MenuId IN (1300000000310, 1300000000311, 1300000000312, 1300000000313)
ORDER BY rm.MenuId;
GO
