/*

 * 变更说明：在“人事管理”菜单（Id=1300000000201）下新增“人员资料管理”子菜单及按钮权限

 *            组件指向 Web/src/views/hrms/emp/index.vue，对应后端 hrEmp 接口

 * 模块：hrEmp

 * 日期：2026-10-12

 * 影响库：ehr（主库 SysMenu）

 * 是否幂等：是（IF NOT EXISTS）

 *

 * 说明：

 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。

 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。

 *   3. 执行后需重新登录前端，侧栏方可显示；角色需授权本菜单及按钮。

 */



IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)

BEGIN

    RAISERROR('父菜单 1300000000201（人事管理）不存在，请先初始化顶级目录。', 16, 1);

END

ELSE

BEGIN

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000221)

    BEGIN

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000221, 1300000000201, 2, 'hrEmp', '/hr/emp', '/hrms/emp/index',

             '人员资料管理', 'ele-User',

             0, 0, 1, 0, 110, 1, 0, GETDATE());

    END



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000222)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Title], [Permission],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000222, 1300000000221, 3, 'hrEmpPage', '分页查询', 'hrEmp/page',

             0, 0, 1, 0, 90, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000223)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Title], [Permission],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000223, 1300000000221, 3, 'hrEmpDetail', '详情', 'hrEmp/detail',

             0, 0, 1, 0, 95, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000224)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Title], [Permission],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000224, 1300000000221, 3, 'hrEmpAdd', '新增', 'hrEmp/add',

             0, 0, 1, 0, 100, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000225)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Title], [Permission],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000225, 1300000000221, 3, 'hrEmpUpdate', '编辑', 'hrEmp/update',

             0, 0, 1, 0, 110, 1, 0, GETDATE());



    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000226)

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Title], [Permission],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000226, 1300000000221, 3, 'hrEmpDelete', '离职', 'hrEmp/delete',

             0, 0, 1, 0, 120, 1, 0, GETDATE());

END



SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Permission], [OrderNo], [Status]

FROM [SysMenu]

WHERE [Id] = 1300000000221 OR [Pid] = 1300000000221

ORDER BY [OrderNo], [Id];

