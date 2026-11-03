/*
 * 变更说明：sys_admin 授权公司参数设置菜单与按钮
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 影响库：ehrnew（SysRoleMenu）
 * 是否幂等：是
 * 前置：已执行 03-HrCompanyParam菜单.sql
 */
SET NOCOUNT ON;

DECLARE @RoleId BIGINT = 1300000000101;

;WITH grants AS (
    SELECT *
    FROM (VALUES
        (1310000005102, 1310000000102), -- 公司参数设置
        (1310000005103, 1310000000103), -- 查询
        (1310000005104, 1310000000104), -- 新增
        (1310000005105, 1310000000105), -- 保存
        (1310000005106, 1310000000106), -- 删除
        (1310000005107, 1310000000107), -- 译文查询
        (1310000005108, 1310000000108)  -- 译文保存
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
GO
