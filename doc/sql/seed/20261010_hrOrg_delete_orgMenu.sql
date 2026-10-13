/*
 * 变更说明：移除“人事管理”下“组织架构管理”子菜单（Id=1300000000211）
 *            同步清理角色菜单、用户收藏菜单关联
 * 模块：hrOrg
 * 日期：2026-10-10
 * 关联需求/Issue：移除组织架构管理模块
 * 影响库：ehr（主库 SysMenu / SysRoleMenu / SysUserMenu）
 * 是否幂等：是（不存在则跳过）
 *
 * [DEPRECATED] 2026-10-10 起请勿再执行本脚本。
 *              组织架构管理已恢复：前端 Web/src/views/hrms/org + 菜单种子
 *              doc/sql/seed/20261010_hrOrg_seed_menu.sql
 *
 * 说明：
 *   1. 本脚本仅作历史回滚参考。
 *   2. 若误执行，请再跑 20261010_hrOrg_seed_menu.sql 恢复菜单。
 */

-- 清理角色菜单关联
DELETE FROM [SysRoleMenu] WHERE [MenuId] = 1300000000211;

-- 清理用户收藏菜单关联
DELETE FROM [SysUserMenu] WHERE [MenuId] = 1300000000211;

-- 删除子按钮（若有）及菜单本身
DELETE FROM [SysMenu] WHERE [Pid] = 1300000000211 OR [Id] = 1300000000211;

-- 校验：应无结果
SELECT [Id], [Pid], [Name], [Title], [Path], [Component]
FROM [SysMenu]
WHERE [Id] = 1300000000211 OR [Pid] = 1300000000211;

-- ROLLBACK:
-- INSERT INTO [SysMenu]
--     ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
--      [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
-- VALUES
--     (1300000000211, 1300000000201, 2, 'hrOrg', '/hr/org', '/hrms/org/index',
--      '组织架构管理', 'ele-Share',
--      0, 0, 1, 0, 100, 1, 0, GETDATE());
