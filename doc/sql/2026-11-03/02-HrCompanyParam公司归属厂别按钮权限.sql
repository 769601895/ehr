/*
 * 变更说明：公司参数设置增加「公司组织分页 / 保存归属厂别 / 厂别下拉」按钮权限
 * 模块：hrCompanyParam
 * 日期：2026-11-03
 * 影响库：ehrnew（SysMenu）
 * 是否幂等：是
 * 前置：菜单 1310000000102 已存在
 * 说明：Title 使用 NCHAR 拼接；厂别下拉 Id 用 1310000000122（避开账号管理 0111）
 */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000102)
BEGIN
    RAISERROR(N'SysMenu 1310000000102 (hrCompanyParam) missing.', 16, 1);
END
ELSE
BEGIN
    -- 公司组织分页
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000109)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000109, 1310000000102, 3, N'hrCompanyParamCompanyOrgPage',
            NCHAR(0x516C)+NCHAR(0x53F8)+NCHAR(0x7EC4)+NCHAR(0x7EC7)+NCHAR(0x5206)+NCHAR(0x9875),
            N'hrCompanyParam/companyOrgPage', 0, 0, 1, 0, 160, 1, 0, GETDATE());

    -- 保存归属厂别
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000110)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000110, 1310000000102, 3, N'hrCompanyParamSaveCompanyOrgSite',
            NCHAR(0x4FDD)+NCHAR(0x5B58)+NCHAR(0x5F52)+NCHAR(0x5C5E)+NCHAR(0x5382)+NCHAR(0x522B),
            N'hrCompanyParam/saveCompanyOrgSite', 0, 0, 1, 0, 170, 1, 0, GETDATE());

    -- 厂别下拉（避开 1310000000111=sysUser）
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000122)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1310000000122, 1310000000102, 3, N'hrCompanyParamSiteOptions',
            NCHAR(0x5382)+NCHAR(0x522B)+NCHAR(0x4E0B)+NCHAR(0x62C9),
            N'hrCompanyParam/siteOptions', 0, 0, 1, 0, 180, 1, 0, GETDATE());
    ELSE IF EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1310000000122 AND [Permission] = N'hrCompanyParam/siteOptions')
        UPDATE [SysMenu]
        SET [Pid] = 1310000000102, [Type] = 3, [Name] = N'hrCompanyParamSiteOptions',
            [IsDelete] = 0, [Status] = 1, [OrderNo] = 180
        WHERE [Id] = 1310000000122;
END
GO
