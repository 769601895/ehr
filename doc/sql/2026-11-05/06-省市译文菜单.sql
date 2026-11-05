/*
 * 变更说明：省市设置增加译文查询/保存按钮权限
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 前置：02-省市菜单.sql（菜单 1300000000360）
 * 是否幂等：是
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000360)
BEGIN
    RAISERROR(N'菜单 1300000000360（省市设置）不存在。', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000367)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000367, 1300000000360, 3, N'hrProvinceLangList', N'省译文查询', N'hrProvince/langList',
            0, 0, 1, 0, 130, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000368)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000368, 1300000000360, 3, N'hrProvinceSaveLang', N'省译文保存', N'hrProvince/saveLang',
            0, 0, 1, 0, 140, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000369)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000369, 1300000000360, 3, N'hrCityLangList', N'市译文查询', N'hrCity/langList',
            0, 0, 1, 0, 230, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000370)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000370, 1300000000360, 3, N'hrCitySaveLang', N'市译文保存', N'hrCity/saveLang',
            0, 0, 1, 0, 240, 1, 0, GETDATE());
END
GO

DECLARE @RoleId BIGINT = 1300000000101;
;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000003217, 1300000000367),
        (1300000003218, 1300000000368),
        (1300000003219, 1300000000369),
        (1300000003220, 1300000000370)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @RoleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @RoleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);
GO
