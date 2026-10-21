/*
 * 变更说明：为系统管理员角色（sys_admin / 1300000000101）授权「报表管理」目录
 * 模块：reportManage
 * 日期：2026-10-14
 * 影响库：ehr（SysRoleMenu）
 * 是否幂等：是（INSERT…WHERE NOT EXISTS）
 *
 * 前置：已执行 20261014_reportManage_seed_menu.sql
 */

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001730, 1300000000101, 1300000000207
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000207);

SELECT rm.Id, rm.RoleId, rm.MenuId, m.Title, m.Name
FROM SysRoleMenu rm
INNER JOIN SysMenu m ON m.Id = rm.MenuId
WHERE rm.RoleId = 1300000000101
  AND rm.MenuId = 1300000000207;
