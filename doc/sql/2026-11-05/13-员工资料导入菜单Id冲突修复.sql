/*
 * 修复：11-员工资料导入菜单权限.sql 误用了 1300000000370（已被「市译文保存」占用）
 * 影响：主库 SysMenu / SysRoleMenu
 * 做法：
 *   1) 若 0370 被改成员工资料导入菜单，则恢复为 hrCity/saveLang 按钮
 *   2) 用新 Id 段 0390~0400 重建员工资料导入菜单与按钮，并授权系统管理员
 */

DECLARE @now DATETIME = GETDATE();
DECLARE @hrPid BIGINT = 1300000000201;
DECLARE @provinceMenu BIGINT = 1300000000360;
DECLARE @adminRole BIGINT = 1300000000101;
DECLARE @newMenuId BIGINT = 1300000000390;

-- 1) 恢复被覆盖的「市译文保存」按钮（若仍被改成导入菜单）
IF EXISTS (
    SELECT 1 FROM SysMenu
    WHERE Id = 1300000000370 AND (Name = N'hrEmpImport' OR Title = N'员工资料导入' OR Permission IS NULL OR Type = 2)
)
BEGIN
    UPDATE SysMenu
    SET Pid = @provinceMenu,
        Type = 3,
        Name = N'hrCitySaveLang',
        Path = NULL,
        Component = NULL,
        Title = N'市译文保存',
        Icon = NULL,
        Permission = N'hrCity/saveLang',
        IsIframe = 0, IsHide = 0, IsKeepAlive = 1, IsAffix = 0,
        OrderNo = 240, Status = 1, IsDelete = 0
    WHERE Id = 1300000000370;
END
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000370)
BEGIN
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000370, @provinceMenu, 3, N'hrCitySaveLang', N'市译文保存', N'hrCity/saveLang', 0, 0, 1, 0, 240, 1, 0, @now);
END

-- 2) 新建/纠正员工资料导入菜单（0390）
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = @newMenuId)
BEGIN
    INSERT INTO SysMenu
        ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
         [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES
        (@newMenuId, @hrPid, 2, N'hrEmpImport', N'/hr/empImport', N'/hrms/empImport/index',
         N'员工资料导入', N'ele-Upload',
         0, 0, 1, 0, 115, 1, 0, @now);
END
ELSE
BEGIN
    UPDATE SysMenu
    SET Pid = @hrPid, Type = 2, Name = N'hrEmpImport',
        Path = N'/hr/empImport', Component = N'/hrms/empImport/index',
        Title = N'员工资料导入', Icon = N'ele-Upload', Permission = NULL,
        IsHide = 0, IsKeepAlive = 1, OrderNo = 115, Status = 1, IsDelete = 0
    WHERE Id = @newMenuId;
END

-- 3) 按钮：优先把旧错误 Pid(0370) 下的导入按钮改挂到 0390；没有则插入 0391~0400
-- fieldPool
IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000371 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/fieldPool', Title = N'导入字段池', Type = 3, IsDelete = 0 WHERE Id = 1300000000371;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000391)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000391, @newMenuId, 3, N'hrEmpImportFieldPool', N'导入字段池', N'hrEmpImport/fieldPool', 0, 0, 1, 0, 100, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000372 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/tplPage', Title = N'模板分页', Type = 3, IsDelete = 0 WHERE Id = 1300000000372;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000392)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000392, @newMenuId, 3, N'hrEmpImportTplPage', N'模板分页', N'hrEmpImport/tplPage', 0, 0, 1, 0, 110, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000373 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/tplDetail', Title = N'模板详情', Type = 3, IsDelete = 0 WHERE Id = 1300000000373;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000393)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000393, @newMenuId, 3, N'hrEmpImportTplDetail', N'模板详情', N'hrEmpImport/tplDetail', 0, 0, 1, 0, 120, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000374 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/tplSave', Title = N'保存模板', Type = 3, IsDelete = 0 WHERE Id = 1300000000374;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000394)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000394, @newMenuId, 3, N'hrEmpImportTplSave', N'保存模板', N'hrEmpImport/tplSave', 0, 0, 1, 0, 130, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000375 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/tplDelete', Title = N'删除模板', Type = 3, IsDelete = 0 WHERE Id = 1300000000375;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000395)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000395, @newMenuId, 3, N'hrEmpImportTplDelete', N'删除模板', N'hrEmpImport/tplDelete', 0, 0, 1, 0, 140, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000376 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/downloadTemplate', Title = N'下载模板', Type = 3, IsDelete = 0 WHERE Id = 1300000000376;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000396)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000396, @newMenuId, 3, N'hrEmpImportDownload', N'下载模板', N'hrEmpImport/downloadTemplate', 0, 0, 1, 0, 150, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000377 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/preview', Title = N'预览校验', Type = 3, IsDelete = 0 WHERE Id = 1300000000377;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000397)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000397, @newMenuId, 3, N'hrEmpImportPreview', N'预览校验', N'hrEmpImport/preview', 0, 0, 1, 0, 160, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000378 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/commit', Title = N'确认导入', Type = 3, IsDelete = 0 WHERE Id = 1300000000378;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000398)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000398, @newMenuId, 3, N'hrEmpImportCommit', N'确认导入', N'hrEmpImport/commit', 0, 0, 1, 0, 170, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000379 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/batchPage', Title = N'导入历史', Type = 3, IsDelete = 0 WHERE Id = 1300000000379;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000399)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000399, @newMenuId, 3, N'hrEmpImportBatchPage', N'导入历史', N'hrEmpImport/batchPage', 0, 0, 1, 0, 180, 1, 0, @now);

IF EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000380 AND Name LIKE N'hrEmpImport%')
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/failRows', Title = N'失败明细', Type = 3, IsDelete = 0 WHERE Id = 1300000000380;
ELSE IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000400)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000400, @newMenuId, 3, N'hrEmpImportFailRows', N'失败明细', N'hrEmpImport/failRows', 0, 0, 1, 0, 190, 1, 0, @now);

-- 3b) 按类型获取模板（每账号固定新增/修改各一份）
IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000401)
    INSERT INTO SysMenu ([Id],[Pid],[Type],[Name],[Title],[Permission],[IsIframe],[IsHide],[IsKeepAlive],[IsAffix],[OrderNo],[Status],[IsDelete],[CreateTime])
    VALUES (1300000000401, @newMenuId, 3, N'hrEmpImportTplByMode', N'按类型取模板', N'hrEmpImport/tplByMode', 0, 0, 1, 0, 125, 1, 0, @now);
ELSE
    UPDATE SysMenu SET Pid = @newMenuId, Permission = N'hrEmpImport/tplByMode', Title = N'按类型取模板', Type = 3, IsDelete = 0 WHERE Id = 1300000000401;

-- 4) 系统管理员授权：菜单 + 所有导入按钮（含旧 Id 与新 Id）
;WITH grantMenus AS (
    SELECT MenuId FROM (VALUES
        (@newMenuId),
        (1300000000371),(1300000000372),(1300000000373),(1300000000374),(1300000000375),
        (1300000000376),(1300000000377),(1300000000378),(1300000000379),(1300000000380),
        (1300000000391),(1300000000392),(1300000000393),(1300000000394),(1300000000395),
        (1300000000396),(1300000000397),(1300000000398),(1300000000399),(1300000000400),(1300000000401),
        (1300000000370) -- 恢复后的市译文保存
    ) v(MenuId)
    WHERE EXISTS (SELECT 1 FROM SysMenu m WHERE m.Id = v.MenuId AND m.IsDelete = 0)
)
INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
SELECT 1300000001750 + ROW_NUMBER() OVER (ORDER BY g.MenuId), @adminRole, g.MenuId
FROM grantMenus g
WHERE NOT EXISTS (SELECT 1 FROM SysRoleMenu rm WHERE rm.RoleId = @adminRole AND rm.MenuId = g.MenuId);
GO
