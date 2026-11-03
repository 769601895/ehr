/*
 * 变更说明：系统管理下挂「公司参数设置」，与账号管理平级且排在其前（OrderNo=90 < 100）
 * 模块：hrCompanyParam
 * 日期：2026-10-30
 * 影响库：ehrnew（SysMenu）
 * 是否幂等：是
 * 前置：目录 1310000000101（系统管理）已存在
 * 说明：Title 使用 NCHAR 拼接，避免 sqlcmd UTF-8 乱码
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000101)
BEGIN
    RAISERROR(N'SysMenu 1310000000101 (system) missing.', 16, 1);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000102)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1310000000102, 1310000000101, 2, N'hrCompanyParam', N'/system/companyParam', N'/hrms/companyParam/index',
             NCHAR(0x516C)+NCHAR(0x53F8)+NCHAR(0x53C2)+NCHAR(0x6570)+NCHAR(0x8BBE)+NCHAR(0x7F6E), N'ele-Setting',
             0, 0, 1, 0, 90, 1, 0, GETDATE());
    ELSE
        UPDATE [SysMenu]
        SET [Pid] = 1310000000101, [Type] = 2, [Name] = N'hrCompanyParam',
            [Path] = N'/system/companyParam', [Component] = N'/hrms/companyParam/index',
            [Title] = NCHAR(0x516C)+NCHAR(0x53F8)+NCHAR(0x53C2)+NCHAR(0x6570)+NCHAR(0x8BBE)+NCHAR(0x7F6E),
            [Icon] = N'ele-Setting', [IsDelete] = 0, [Status] = 1, [OrderNo] = 90
        WHERE [Id] = 1310000000102;

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000103)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000103, 1310000000102, 3, N'hrCompanyParamPage',
            NCHAR(0x67E5)+NCHAR(0x8BE2), N'hrCompanyParam/page', 0, 0, 1, 0, 100, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000104)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000104, 1310000000102, 3, N'hrCompanyParamAdd',
            NCHAR(0x65B0)+NCHAR(0x589E), N'hrCompanyParam/add', 0, 0, 1, 0, 110, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000105)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000105, 1310000000102, 3, N'hrCompanyParamUpdate',
            NCHAR(0x4FDD)+NCHAR(0x5B58), N'hrCompanyParam/update', 0, 0, 1, 0, 120, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000106)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000106, 1310000000102, 3, N'hrCompanyParamDelete',
            NCHAR(0x5220)+NCHAR(0x9664), N'hrCompanyParam/delete', 0, 0, 1, 0, 130, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000107)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000107, 1310000000102, 3, N'hrCompanyParamLangList',
            NCHAR(0x8BD1)+NCHAR(0x6587)+NCHAR(0x67E5)+NCHAR(0x8BE2), N'hrCompanyParam/langList', 0, 0, 1, 0, 140, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000108)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000108, 1310000000102, 3, N'hrCompanyParamSaveLang',
            NCHAR(0x8BD1)+NCHAR(0x6587)+NCHAR(0x4FDD)+NCHAR(0x5B58), N'hrCompanyParam/saveLang', 0, 0, 1, 0, 150, 1, 0, GETDATE());
END
GO
