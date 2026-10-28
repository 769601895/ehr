/*
 * 变更说明：为系统管理员角色授权审批引擎菜单
 * 模块：hrWf
 * 日期：2026-10-24
 * 影响库：ehrnew（SysRoleMenu）
 * 是否幂等：是
 * 前置：已执行 02-审批流程引擎菜单.sql
 */

SET NOCOUNT ON;

DECLARE @RoleId BIGINT = 1300000000101;
DECLARE @MenuId BIGINT;
DECLARE @IdBase BIGINT = 1300000001800;

DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT Id FROM SysMenu WHERE Id BETWEEN 1300000000280 AND 1300000000301;

OPEN cur;
FETCH NEXT FROM cur INTO @MenuId;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @RoleId AND MenuId = @MenuId)
        INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
        VALUES (@IdBase + (@MenuId - 1300000000280), @RoleId, @MenuId);
    FETCH NEXT FROM cur INTO @MenuId;
END
CLOSE cur;
DEALLOCATE cur;

SELECT rm.Id, rm.RoleId, rm.MenuId, m.Title, m.Name
FROM SysRoleMenu rm
INNER JOIN SysMenu m ON m.Id = rm.MenuId
WHERE rm.RoleId = @RoleId AND rm.MenuId BETWEEN 1300000000280 AND 1300000000301
ORDER BY rm.MenuId;
