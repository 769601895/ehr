/*
 * 业务界面配置：挂在「报表管理」目录下（子菜单）
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @menuId BIGINT = 1300000000500;
DECLARE @reportManagePid BIGINT = 1300000000207; -- 报表管理

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @reportManagePid AND IsDelete = 0)
BEGIN
    RAISERROR(N'未找到菜单「报表管理」(Id=1300000000207)，请先确认菜单数据。', 16, 1);
    RETURN;
END

UPDATE dbo.SysMenu
SET Pid = @reportManagePid,
    Type = 2,
    Name = N'hrUiConfig',
    Path = N'/reportManage/uiConfig',
    Component = N'/hrms/uiConfig/index',
    Title = N'业务界面配置',
    Icon = N'ele-SetUp',
    IsHide = 0,
    Status = 1,
    IsDelete = 0,
    OrderNo = 100,
    UpdateTime = @now
WHERE Id = @menuId;

SELECT Id, Pid, Type, Name, Title, Path, Component, OrderNo
FROM dbo.SysMenu
WHERE Id IN (@reportManagePid, @menuId);
