/*
 * 变更说明：sys_admin 授权省市设置菜单与按钮
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 影响库：ehr（SysRoleMenu）
 * 是否幂等：是
 * 前置：已执行 02-省市菜单.sql
 * RoleMenu.Id：1300000003210~3216（避开 318x/319x 已占用段）
 */
SET NOCOUNT ON;

DECLARE @RoleId BIGINT = 1300000000101;

;WITH grants AS (
    SELECT *
    FROM (VALUES
        (1300000003210, 1300000000360), -- 省市设置
        (1300000003211, 1300000000361), -- 省查询
        (1300000003212, 1300000000362), -- 省保存
        (1300000003213, 1300000000363), -- 省删除
        (1300000003214, 1300000000364), -- 市查询
        (1300000003215, 1300000000365), -- 市保存
        (1300000003216, 1300000000366)  -- 市删除
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
  AND rm.MenuId BETWEEN 1300000000360 AND 1300000000366
ORDER BY rm.MenuId;
GO
