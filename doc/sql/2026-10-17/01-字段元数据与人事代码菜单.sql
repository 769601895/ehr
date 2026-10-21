/*
 * 变更说明：人事管理下增加「字段元数据」「人事代码设置」
 * 日期：2026-10-17
 * 影响库：ehr（SysMenu）
 * 是否幂等：是
 * 前置：菜单 1300000000201（人事管理）已存在
 */

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)
BEGIN
    RAISERROR(N'菜单 1300000000201（人事管理）不存在。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000260)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000260, 1300000000201, 2, N'hrUiFieldDef', N'/hr/fieldDef', N'/hrms/fieldDef/index',
             N'字段元数据', N'ele-Tickets',
             0, 0, 1, 0, 90, 1, 0, GETDATE());
    ELSE
        UPDATE [SysMenu]
        SET [Pid] = 1300000000201, [Type] = 2, [Name] = N'hrUiFieldDef',
            [Path] = N'/hr/fieldDef', [Component] = N'/hrms/fieldDef/index',
            [Title] = N'字段元数据', [IsDelete] = 0, [Status] = 1, [OrderNo] = 90
        WHERE [Id] = 1300000000260;

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000261)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000261, 1300000000260, 3, N'hrUiFieldDefPage', N'查询', N'hrUiFieldDef/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000262)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000262, 1300000000260, 3, N'hrUiFieldDefUpdate', N'编辑', N'hrUiFieldDef/update',
            0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000263)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000263, 1300000000260, 3, N'hrUiFieldDefSync', N'同步', N'hrUiFieldDef/syncFromDb',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000264)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000264, 1300000000260, 3, N'hrUiFieldDefLang', N'译文', N'hrUiFieldDef/saveLang',
            0, 0, 1, 0, 130, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000270)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000270, 1300000000201, 2, N'hrCodeMstr', N'/hr/codeMstr', N'/hrms/codeMstr/index',
             N'人事代码设置', N'ele-Collection',
             0, 0, 1, 0, 120, 1, 0, GETDATE());
    ELSE
        UPDATE [SysMenu]
        SET [Pid] = 1300000000201, [Type] = 2, [Name] = N'hrCodeMstr',
            [Path] = N'/hr/codeMstr', [Component] = N'/hrms/codeMstr/index',
            [Title] = N'人事代码设置', [IsDelete] = 0, [Status] = 1, [OrderNo] = 120
        WHERE [Id] = 1300000000270;

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000271)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000271, 1300000000270, 3, N'hrCodeMstrPage', N'查询', N'hrCodeMstr/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000272)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000272, 1300000000270, 3, N'hrCodeMstrSave', N'保存', N'hrCodeMstr/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000273)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000273, 1300000000270, 3, N'hrCodeMstrDelete', N'删除', N'hrCodeMstr/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000274)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000274, 1300000000270, 3, N'hrCodeMstrLang', N'译文', N'hrCodeMstr/saveLang',
            0, 0, 1, 0, 130, 1, 0, GETDATE());
END;
