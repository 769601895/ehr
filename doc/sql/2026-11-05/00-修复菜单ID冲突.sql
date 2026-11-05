/*
 * 变更说明：修复误占用的业务生效策略按钮 Id，并清理错误挂接的省市按钮
 * 背景：首次省市菜单脚本误用 1300000000330~0332（已属业务生效策略）
 * 日期：2026-11-05
 * 是否幂等：是
 */
SET NOCOUNT ON;

-- 1) 恢复业务生效策略「查询」按钮 0330
UPDATE dbo.SysMenu
SET Pid = 1300000000305,
    Type = 3,
    Name = N'hrWfEffectPolicyList',
    Path = NULL,
    Component = NULL,
    Title = N'查询',
    Icon = NULL,
    Permission = N'hrWfEffectPolicy/listPolicies',
    OrderNo = 100,
    Status = 1,
    IsDelete = 0
WHERE Id = 1300000000330;

-- 2) 删除误挂在旧父级下的省市按钮（0333~0336），正式菜单改用 0360~
DELETE FROM dbo.SysRoleMenu WHERE MenuId BETWEEN 1300000000333 AND 1300000000336;
DELETE FROM dbo.SysMenu WHERE Id BETWEEN 1300000000333 AND 1300000000336
  AND Name IN (N'hrProvinceDelete', N'hrCityPage', N'hrCitySave', N'hrCityDelete');

-- 3) 若曾把 RoleMenu.Id=3190 挂到错误的 0330，删掉以便 03 脚本重挂到 0360
DELETE FROM dbo.SysRoleMenu
WHERE Id BETWEEN 1300000003190 AND 1300000003196
  AND MenuId BETWEEN 1300000000330 AND 1300000000336;

PRINT N'菜单冲突已修复，请继续执行 02/03/04';
GO
