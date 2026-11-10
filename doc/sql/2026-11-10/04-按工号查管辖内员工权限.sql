-- 人员资料：按工号精确查管辖内员工（表单工号输入回填）+ 超管授权
-- 父菜单：人员资料管理 hrEmp = 1300000000221

SET NOCOUNT ON;
DECLARE @now DATETIME = GETDATE();
DECLARE @roleId BIGINT = 1300000000101;
DECLARE @menuId BIGINT = 1300000000227;
DECLARE @grantId BIGINT = 1300000002227;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = @menuId)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Title, Permission,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (@menuId, 1300000000221, 3, N'hrEmpLookupByEmpId', N'按工号查管辖内员工', N'hrEmp/lookupByEmpId',
         0, 0, 1, 0, 92, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = 1300000000221, Type = 3, Name = N'hrEmpLookupByEmpId', Title = N'按工号查管辖内员工',
        Permission = N'hrEmp/lookupByEmpId', IsDelete = 0, Status = 1, OrderNo = 92, UpdateTime = @now
    WHERE Id = @menuId;

IF NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE RoleId = @roleId AND MenuId = @menuId)
   AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu WHERE Id = @grantId)
    INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
    VALUES (@grantId, @roleId, @menuId);
