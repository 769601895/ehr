/*
 * 员工资料导入菜单 + 按钮权限（与人员资料管理同级，Pid=人事管理 1300000000201）
 * 菜单 Id：1300000000390~0400（避开 0360~0370 省市/译文）
 * 影响库：主库 SysMenu / SysRoleMenu
 * 幂等：IF NOT EXISTS
 *
 * 若曾执行过旧版（误用 0370），请再执行 13-员工资料导入菜单Id冲突修复.sql
 */

DECLARE @now DATETIME = GETDATE();
DECLARE @pid BIGINT = 1300000000201;
DECLARE @menuId BIGINT = 1300000000390;
DECLARE @adminRole BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = @pid)
BEGIN
    RAISERROR(N'父菜单 1300000000201（人事管理）不存在。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = @menuId)
BEGIN
    INSERT INTO [SysMenu]
        ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
         [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES
        (@menuId, @pid, 2, N'hrEmpImport', N'/hr/empImport', N'/hrms/empImport/index',
         N'员工资料导入', N'ele-Upload',
         0, 0, 1, 0, 115, 1, 0, @now);
END
ELSE
BEGIN
    UPDATE [SysMenu]
    SET Pid = @pid, Type = 2, Name = N'hrEmpImport',
        Path = N'/hr/empImport', Component = N'/hrms/empImport/index',
        Title = N'员工资料导入', Icon = N'ele-Upload', Permission = NULL,
        IsHide = 0, IsKeepAlive = 1, OrderNo = 115, Status = 1, IsDelete = 0
    WHERE Id = @menuId;
END

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000391)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000391, @menuId, 3, N'hrEmpImportFieldPool', N'导入字段池', N'hrEmpImport/fieldPool', 0, 0, 1, 0, 100, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000392)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000392, @menuId, 3, N'hrEmpImportTplPage', N'模板分页', N'hrEmpImport/tplPage', 0, 0, 1, 0, 110, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000393)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000393, @menuId, 3, N'hrEmpImportTplDetail', N'模板详情', N'hrEmpImport/tplDetail', 0, 0, 1, 0, 120, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000401)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000401, @menuId, 3, N'hrEmpImportTplByMode', N'按类型取模板', N'hrEmpImport/tplByMode', 0, 0, 1, 0, 125, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000394)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000394, @menuId, 3, N'hrEmpImportTplSave', N'保存模板', N'hrEmpImport/tplSave', 0, 0, 1, 0, 130, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000395)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000395, @menuId, 3, N'hrEmpImportTplDelete', N'删除模板', N'hrEmpImport/tplDelete', 0, 0, 1, 0, 140, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000396)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000396, @menuId, 3, N'hrEmpImportDownload', N'下载模板', N'hrEmpImport/downloadTemplate', 0, 0, 1, 0, 150, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000397)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000397, @menuId, 3, N'hrEmpImportPreview', N'预览校验', N'hrEmpImport/preview', 0, 0, 1, 0, 160, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000398)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000398, @menuId, 3, N'hrEmpImportCommit', N'确认导入', N'hrEmpImport/commit', 0, 0, 1, 0, 170, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000399)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000399, @menuId, 3, N'hrEmpImportBatchPage', N'导入历史', N'hrEmpImport/batchPage', 0, 0, 1, 0, 180, 1, 0, @now);

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000400)
    INSERT INTO [SysMenu] ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000400, @menuId, 3, N'hrEmpImportFailRows', N'失败明细', N'hrEmpImport/failRows', 0, 0, 1, 0, 190, 1, 0, @now);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001760, @adminRole, @menuId
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = @menuId);

INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001761, @adminRole, 1300000000391 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000391);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001762, @adminRole, 1300000000392 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000392);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001763, @adminRole, 1300000000393 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000393);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001771, @adminRole, 1300000000401 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000401);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001764, @adminRole, 1300000000394 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000394);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001765, @adminRole, 1300000000395 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000395);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001766, @adminRole, 1300000000396 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000396);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001767, @adminRole, 1300000000397 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000397);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001768, @adminRole, 1300000000398 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000398);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001769, @adminRole, 1300000000399 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000399);
INSERT INTO SysRoleMenu (Id, RoleId, MenuId) SELECT 1300000001770, @adminRole, 1300000000400 WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @adminRole AND MenuId = 1300000000400);
GO
