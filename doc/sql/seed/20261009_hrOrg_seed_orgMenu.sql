/*
 * 变更说明：在“人事管理”菜单（Id=1300000000201）下新增“组织架构管理”子菜单（Id=1300000000211）
 *            组件指向前端 hrms 模块组织架构管理页（/hrms/org/index），对应后端 hrOrg 接口分组
 * 模块：hrOrg
 * 日期：2026-10-09
 * 关联需求/Issue：组织架构管理模块接入（人事管理 → 组织架构管理）
 * 影响库：ehr（主库 SysMenu）
 * 是否幂等：是（IF NOT EXISTS 包裹，可重复执行不报错）
 *
 * 说明：
 *   1. SysMenu 继承 EntityBase，无 TenantId 列，无需处理租户。
 *   2. MenuTypeEnum：Dir=1、Menu=2、Button=3 -> 子菜单 Type=2。
 *   3. 父菜单“人事管理”(1300000000201) 由 doc/sql/2026-10-09.sql 初始化，本脚本前置校验其存在。
 *   4. IsIframe / IsHide / IsDelete 为 NOT NULL，需显式给值（与既有种子脚本列集保持一致）。
 *   5. 执行后需重启后端（注册 hrOrg 接口权限 SysApi）并重新登录前端，菜单方可显示。
 *   6. 执行前请确认当前数据库为 ehr（主库）。
 */

-- 前置校验：父菜单“人事管理”必须已存在
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)
BEGIN
    RAISERROR('父菜单 1300000000201（人事管理）不存在，请先执行 doc/sql/2026-10-09.sql 初始化顶级目录。', 16, 1);
END
ELSE
BEGIN
    -- 幂等插入：子菜单不存在才写入
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000211)
    BEGIN
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000211, 1300000000201, 2, 'hrOrg', '/hr/org', '/hrms/org/index',
             '组织架构管理', 'ele-Share',
             0, 0, 1, 0, 100, 1, 0, GETDATE());
    END
END

-- 校验落库
SELECT [Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [OrderNo], [Status], [IsDelete]
FROM [SysMenu]
WHERE [Id] = 1300000000211;

-- ROLLBACK:
-- DELETE FROM [SysMenu] WHERE [Id] = 1300000000211;
