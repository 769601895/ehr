/*
 * 变更说明：系统代码设置下挂「岗位代码设置」菜单与按钮权限
 * 模块：hrPost
 * 日期：2026-10-28
 * 影响库：ehr（SysMenu）
 * 是否幂等：是
 * 前置：目录 1300000000206（系统代码设置）已存在
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000206)
BEGIN
    RAISERROR(N'菜单 1300000000206（系统代码设置）不存在，请先执行 sysCode 种子。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000310)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000310, 1300000000206, 2, N'hrPost', N'/sysCode/hrPost', N'/hrms/post/index',
             N'岗位代码设置', N'ele-Briefcase',
             0, 0, 1, 0, 10, 1, 0, GETDATE());
    ELSE
        UPDATE [SysMenu]
        SET [Pid] = 1300000000206, [Type] = 2, [Name] = N'hrPost',
            [Path] = N'/sysCode/hrPost', [Component] = N'/hrms/post/index',
            [Title] = N'岗位代码设置', [Icon] = N'ele-Briefcase',
            [IsDelete] = 0, [Status] = 1, [OrderNo] = 10
        WHERE [Id] = 1300000000310;

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000311)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000311, 1300000000310, 3, N'hrPostPage', N'查询', N'hrPost/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000312)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000312, 1300000000310, 3, N'hrPostSave', N'保存', N'hrPost/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000313)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000313, 1300000000310, 3, N'hrPostDelete', N'删除', N'hrPost/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());
END
GO
