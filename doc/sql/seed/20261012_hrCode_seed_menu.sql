/*

 * 变更说明：在「人事管理」下、组织架构管理前新增「人事代码设置」空目录（后续挂子模块）

 * 模块：hrCode

 * 日期：2026-10-12

 * 影响库：ehr（主库 SysMenu）

 * 是否幂等：是（IF NOT EXISTS）

 *

 * 说明：

 *   1. MenuTypeEnum：Dir=1、Menu=2、Button=3。

 *   2. IsIframe / IsHide / IsKeepAlive / IsAffix / IsDelete 为 NOT NULL，需显式给值。

 *   3. OrderNo=90，位于组织架构(100)之前。

 *   4. 执行后需重新登录前端；角色需授权本菜单。

 */



IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)

BEGIN

    RAISERROR('父菜单 1300000000201（人事管理）不存在，请先初始化顶级目录。', 16, 1);

END

ELSE

BEGIN

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000231)

    BEGIN

        INSERT INTO [SysMenu]

            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],

             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])

        VALUES

            (1300000000231, 1300000000201, 1, 'hrCode', '/hr/code', 'Layout',

             '人事代码设置', 'ele-List',

             0, 0, 1, 0, 90, 1, 0, GETDATE());

    END

END;



SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [OrderNo], [Status]

FROM [SysMenu]

WHERE [Id] = 1300000000231;


