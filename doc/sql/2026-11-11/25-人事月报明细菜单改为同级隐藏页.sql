/*
 * 变更说明：人事月报明细改为挂「人事管理」同级隐藏页（勿挂在列表页下）
 * 原因：明细若作为列表页的子路由，列表页无 <router-view>，页签切换后明细空白
 * 日期：2026-11-11
 * 是否幂等：是（仅更新 Name=hrEmpMonthDetail）
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @hrPid BIGINT = 1300000000201;          -- 人事管理
DECLARE @detailId BIGINT = 1300000000532;       -- 人事月报明细
DECLARE @now DATETIME = GETDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @hrPid)
BEGIN
    RAISERROR(N'未找到菜单「人事管理」(Id=1300000000201)', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @detailId AND Name = N'hrEmpMonthDetail')
BEGIN
    RAISERROR(N'菜单 1300000000532 不是 hrEmpMonthDetail，禁止覆盖。', 16, 1);
    RETURN;
END

UPDATE dbo.SysMenu
SET Pid = @hrPid,
    Type = 2,
    Path = N'/hr/empMonth/detail',
    Component = N'/hrms/empMonth/detail',
    Title = N'人事月报明细',
    Icon = N'ele-Document',
    IsHide = 1,
    IsKeepAlive = 1,
    IsDelete = 0,
    Status = 1,
    OrderNo = 251,
    UpdateTime = @now
WHERE Id = @detailId AND Name = N'hrEmpMonthDetail';

/* 执行后请：退出重新登录（刷新登录菜单树）→ 关掉旧「人事月报/明细」页签 → 从列表再点开明细 */
SELECT Id, Pid, Name, Title, Path, IsHide
FROM dbo.SysMenu
WHERE Id IN (1300000000201, 1300000000520, 1300000000532)
ORDER BY Id;
GO
