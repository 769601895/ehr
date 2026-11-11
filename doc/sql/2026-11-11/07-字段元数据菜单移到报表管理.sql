/*
 * 字段元数据：从「人事管理」挪到「报表管理」下（与业务界面配置同级）
 * 子权限按钮 Pid 仍指向本菜单 Id，无需改动
 * 日期：2026-11-11  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @menuId BIGINT = 1300000000260;
DECLARE @reportManagePid BIGINT = 1300000000207; -- 报表管理

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @reportManagePid AND IsDelete = 0)
BEGIN
    RAISERROR(N'未找到菜单「报表管理」(Id=1300000000207)，请先确认菜单数据。', 16, 1);
    RETURN;
END

UPDATE dbo.SysMenu
SET Pid = @reportManagePid,
    Type = 2,
    Name = N'hrUiFieldDef',
    Path = N'/reportManage/fieldDef',
    Component = N'/hrms/fieldDef/index',
    Title = N'字段元数据',
    Icon = N'ele-Tickets',
    IsHide = 0,
    Status = 1,
    IsDelete = 0,
    OrderNo = 90,
    UpdateTime = @now
WHERE Id = @menuId;

SELECT Id, Pid, Type, Name, Title, Path, Component, OrderNo
FROM dbo.SysMenu
WHERE Pid = @reportManagePid OR Id = @reportManagePid
ORDER BY CASE WHEN Id = @reportManagePid THEN 0 ELSE 1 END, OrderNo;
