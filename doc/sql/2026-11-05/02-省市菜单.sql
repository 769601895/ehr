/*
 * 变更说明：系统代码设置下挂「省市设置」菜单与按钮权限
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 影响库：ehr（SysMenu）
 * 是否幂等：是
 * 前置：目录 1300000000206（系统代码设置）已存在
 * 菜单 Id：1300000000360~0366（避开 0330~0332 业务生效策略按钮）
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000206)
BEGIN
    RAISERROR(N'菜单 1300000000206（系统代码设置）不存在，请先执行 sysCode 种子。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000360)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000360, 1300000000206, 2, N'hrProvince', N'/sysCode/hrProvince', N'/hrms/province/index',
             N'省市设置', N'ele-Location',
             0, 0, 1, 0, 30, 1, 0, GETDATE());
    ELSE
        UPDATE [SysMenu]
        SET [Pid] = 1300000000206, [Type] = 2, [Name] = N'hrProvince',
            [Path] = N'/sysCode/hrProvince', [Component] = N'/hrms/province/index',
            [Title] = N'省市设置', [Icon] = N'ele-Location', [Permission] = NULL,
            [IsDelete] = 0, [Status] = 1, [OrderNo] = 30
        WHERE [Id] = 1300000000360;

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000361)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000361, 1300000000360, 3, N'hrProvincePage', N'省查询', N'hrProvince/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000362)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000362, 1300000000360, 3, N'hrProvinceSave', N'省保存', N'hrProvince/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000363)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000363, 1300000000360, 3, N'hrProvinceDelete', N'省删除', N'hrProvince/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000364)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000364, 1300000000360, 3, N'hrCityPage', N'市查询', N'hrCity/page',
            0, 0, 1, 0, 200, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000365)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000365, 1300000000360, 3, N'hrCitySave', N'市保存', N'hrCity/save',
            0, 0, 1, 0, 210, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000366)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000366, 1300000000360, 3, N'hrCityDelete', N'市删除', N'hrCity/delete',
            0, 0, 1, 0, 220, 1, 0, GETDATE());
END
GO
