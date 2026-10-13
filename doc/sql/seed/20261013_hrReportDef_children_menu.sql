/*

 * 变更说明：将「人事资料定义」改为目录，下挂「人事资料列表定义」「人事资料表单定义」

 * 模块：hrReportDef / hrUiLayout

 * 日期：2026-10-13

 * 影响库：ehr（SysMenu）

 * 是否幂等：是

 *

 * 结构：

 *   1300000000241  Dir  人事资料定义

 *     1300000000244 Menu 人事资料列表定义（一期 list）

 *     1300000000245 Menu 人事资料表单定义（二期 form）

 *   原按钮 0242/0243 软删（权限改挂子菜单按钮）

 */



IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000241)

BEGIN

    RAISERROR(N'菜单 1300000000241（人事资料定义）不存在，请先执行 20261013_hrReportDef_seed_menu.sql。', 16, 1);

END

ELSE

BEGIN

    -- 父级改为目录

    UPDATE [SysMenu]

    SET [Type] = 1,

        [Name] = N'hrReportDef',

        [Path] = N'/hr/reportDef',

        [Component] = N'Layout',

        [Title] = N'人事资料定义',

        [Icon] = N'ele-Document',

        [IsIframe] = 0,

        [IsHide] = 0,

        [IsKeepAlive] = 1,

        [IsAffix] = 0,

        [OrderNo] = 95,

        [Status] = 1,

        [IsDelete] = 0

    WHERE [Id] = 1300000000241;



    -- 原挂在目录上的查询/保存按钮作废

    UPDATE [SysMenu] SET [IsDelete] = 1, [Status] = 2

    WHERE [Id] IN (1300000000242, 1300000000243) AND [IsDelete] = 0;



    -- 列表定义（一期）

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000244)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000244, 1300000000241, 2, 'hrEmpListDef', '/hr/reportDef/list', '/hrms/reportDef/listDef',

             N'人事资料列表定义', N'ele-Grid',

             0, 0, 1, 0, 100, 1, 0, GETDATE());

    ELSE

        UPDATE [SysMenu]

        SET [Pid] = 1300000000241, [Type] = 2, [Name] = N'hrEmpListDef',

            [Path] = N'/hr/reportDef/list', [Component] = N'/hrms/reportDef/listDef',

            [Title] = N'人事资料列表定义', [IsDelete] = 0, [Status] = 1, [OrderNo] = 100

        WHERE [Id] = 1300000000244;



    -- 表单定义（二期）

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000245)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000245, 1300000000241, 2, 'hrEmpFormDef', '/hr/reportDef/form', '/hrms/reportDef/formDef',

             N'人事资料表单定义', N'ele-EditPen',

             0, 0, 1, 0, 110, 1, 0, GETDATE());

    ELSE

        UPDATE [SysMenu]

        SET [Pid] = 1300000000241, [Type] = 2, [Name] = N'hrEmpFormDef',

            [Path] = N'/hr/reportDef/form', [Component] = N'/hrms/reportDef/formDef',

            [Title] = N'人事资料表单定义', [IsDelete] = 0, [Status] = 1, [OrderNo] = 110

        WHERE [Id] = 1300000000245;



    -- 列表定义按钮

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000246)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000246, 1300000000244, 3, 'hrUiLayoutListGet', N'查询', N'hrUiLayout/getEffective',

            0, 0, 1, 0, 90, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000247)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000247, 1300000000244, 3, 'hrUiLayoutListSave', N'保存默认', N'hrUiLayout/saveDefault',

            0, 0, 1, 0, 100, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000248)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000248, 1300000000244, 3, 'hrUiLayoutListFieldPool', N'字段池', N'hrUiLayout/fieldPool',

            0, 0, 1, 0, 110, 1, 0, GETDATE());



    -- 表单定义按钮

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000249)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000249, 1300000000245, 3, 'hrUiLayoutFormGet', N'查询', N'hrUiLayout/getEffective',

            0, 0, 1, 0, 90, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000250)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000250, 1300000000245, 3, 'hrUiLayoutFormSave', N'保存默认', N'hrUiLayout/saveDefault',

            0, 0, 1, 0, 100, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000251)

        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],

            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES (1300000000251, 1300000000245, 3, 'hrUiLayoutFormFieldPool', N'字段池', N'hrUiLayout/fieldPool',

            0, 0, 1, 0, 110, 1, 0, GETDATE());

END



SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Permission], [OrderNo], [Status], [IsDelete]

FROM [SysMenu]

WHERE [Id] = 1300000000241 OR [Pid] = 1300000000241

   OR [Pid] IN (1300000000244, 1300000000245)

ORDER BY [Pid], [OrderNo], [Id];

