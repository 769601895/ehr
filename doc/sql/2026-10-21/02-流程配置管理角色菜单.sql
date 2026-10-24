/*
 * 变更说明：为系统管理员角色（sys_admin / 1300000000101）授权「流程配置管理」目录
 * 模块：moduleFlowConfig
 * 日期：2026-10-21
 * 影响库：ehrnew（SysRoleMenu）
 * 是否幂等：是（INSERT…WHERE NOT EXISTS）
 *
 * 前置：已执行 01-流程配置管理菜单.sql
 */

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001750, 1300000000101, 1300000000208
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = 1300000000101 AND MenuId = 1300000000208);

SELECT rm.Id, rm.RoleId, rm.MenuId, m.Title, m.Name
FROM SysRoleMenu rm
INNER JOIN SysMenu m ON m.Id = rm.MenuId
WHERE rm.RoleId = 1300000000101
  AND rm.MenuId = 1300000000208;
