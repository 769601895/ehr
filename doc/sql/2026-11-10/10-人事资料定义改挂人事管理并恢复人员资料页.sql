/*
 * 将「人事资料定义」从人员资料管理(0221)下挪回人事管理(0201)同级，
 * 避免 Type=2 的人员资料页因挂了 Type=1 子目录而变成空菜单。
 * 日期：2026-11-10  幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now DATETIME = GETDATE();
DECLARE @hrPid BIGINT = 1300000000201;          -- 人事管理
DECLARE @reportDefDir BIGINT = 1300000000241;   -- 人事资料定义目录

UPDATE dbo.SysMenu
SET Pid = @hrPid,
    Type = 1,
    Name = N'hrReportDef',
    Path = N'/hr/reportDef',
    Component = N'Layout',
    Permission = NULL,
    Title = N'人事资料定义',
    Icon = N'ele-Document',
    IsIframe = 0,
    IsHide = 0,
    IsKeepAlive = 1,
    IsAffix = 0,
    OrderNo = 95,
    Status = 1,
    IsDelete = 0,
    UpdateTime = @now
WHERE Id = @reportDefDir;

UPDATE dbo.SysMenu
SET Pid = @reportDefDir,
    Type = 2,
    IsDelete = 0,
    Status = 1,
    UpdateTime = @now
WHERE Id IN (1300000000244, 1300000000245);

-- 人员资料管理保持为可点开的页面菜单（仅挂权限按钮）
UPDATE dbo.SysMenu
SET Type = 2,
    Path = N'/hr/emp',
    Component = N'/hrms/emp/index',
    Permission = NULL,
    Title = N'人员资料管理',
    IsHide = 0,
    Status = 1,
    IsDelete = 0,
    UpdateTime = @now
WHERE Id = 1300000000221;

-- 校验：定义目录应挂在人事管理下；人员资料下不应再有 Type=1/2 子菜单
SELECT Id, Pid, Type, Name, Title, Path, Component, IsDelete
FROM dbo.SysMenu
WHERE Id IN (1300000000201, 1300000000221, 1300000000241, 1300000000244, 1300000000245)
   OR Pid IN (1300000000221, 1300000000241)
ORDER BY Pid, OrderNo, Id;
