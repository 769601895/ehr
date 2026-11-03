$ErrorActionPreference = "Stop"
$cs = "Server=jack\sql2014;Database=ehrnew;User Id=sa;Password=qingfeng;Encrypt=False;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000102)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Path,Component,Title,Icon,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000102,1310000000101,2,N'hrCompanyParam',N'/system/companyParam',N'/hrms/companyParam/index',N'公司参数设置',N'ele-Setting',0,0,1,0,90,1,0,GETDATE());
ELSE
  UPDATE SysMenu SET Pid=1310000000101,Type=2,Name=N'hrCompanyParam',Path=N'/system/companyParam',Component=N'/hrms/companyParam/index',Title=N'公司参数设置',Icon=N'ele-Setting',IsDelete=0,Status=1,OrderNo=90 WHERE Id=1310000000102;

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000103)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000103,1310000000102,3,N'hrCompanyParamPage',N'查询',N'hrCompanyParam/page',0,0,1,0,100,1,0,GETDATE());
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000104)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000104,1310000000102,3,N'hrCompanyParamAdd',N'新增',N'hrCompanyParam/add',0,0,1,0,110,1,0,GETDATE());
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000105)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000105,1310000000102,3,N'hrCompanyParamUpdate',N'保存',N'hrCompanyParam/update',0,0,1,0,120,1,0,GETDATE());
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000106)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000106,1310000000102,3,N'hrCompanyParamDelete',N'删除',N'hrCompanyParam/delete',0,0,1,0,130,1,0,GETDATE());
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000107)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000107,1310000000102,3,N'hrCompanyParamLangList',N'译文查询',N'hrCompanyParam/langList',0,0,1,0,140,1,0,GETDATE());
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id=1310000000108)
  INSERT INTO SysMenu (Id,Pid,Type,Name,Title,Permission,IsIframe,IsHide,IsKeepAlive,IsAffix,OrderNo,Status,IsDelete,CreateTime)
  VALUES (1310000000108,1310000000102,3,N'hrCompanyParamSaveLang',N'译文保存',N'hrCompanyParam/saveLang',0,0,1,0,150,1,0,GETDATE());

DECLARE @RoleId BIGINT = 1300000000101;
;WITH grants AS (
  SELECT * FROM (VALUES
    (1310000005102, 1310000000102),(1310000005103, 1310000000103),(1310000005104, 1310000000104),
    (1310000005105, 1310000000105),(1310000005106, 1310000000106),(1310000005107, 1310000000107),
    (1310000005108, 1310000000108)
  ) v(Id, MenuId)
)
INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @RoleId, g.MenuId FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu rm WHERE rm.RoleId=@RoleId AND rm.MenuId=g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM SysRoleMenu rm WHERE rm.Id=g.Id);
"@
[void]$cmd.ExecuteNonQuery()
$cmd.CommandText = "SELECT Id,Pid,Title,OrderNo,Path FROM SysMenu WHERE Id IN (1310000000102,1310000000111) ORDER BY OrderNo"
$r = $cmd.ExecuteReader()
while ($r.Read()) { Write-Host ($r.GetValue(0).ToString() + " | " + $r.GetValue(2) + " | OrderNo=" + $r.GetValue(3) + " | " + $r.GetValue(4)) }
$r.Close(); $conn.Close()
Write-Host "menu ok"
