-- ============================================================
-- 新增业务模块目录菜单（与"工作台"平级）
-- 说明：四个顶级目录，挂在根节点(Pid=0)，排序位于"工作台"(0)之后、"系统管理"(10000)之前
-- 注意：SysMenu 中 IsIframe / IsHide / IsDelete 为 NOT NULL，需显式给值
-- 执行前请确认当前数据库为 ehr
-- ============================================================

-- 人事管理
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000201)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000201, 0, 1, 'hr', '/hr', 'Layout', '人事管理', 'ele-User', 0, 0, 1, 0, 100, 1, 0, GETDATE());
END;

-- 考勤管理
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000202)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000202, 0, 1, 'attendance', '/attendance', 'Layout', '考勤管理', 'ele-Calendar', 0, 0, 1, 0, 200, 1, 0, GETDATE());
END;

-- 保险公积金管理
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000203)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000203, 0, 1, 'insurance', '/insurance', 'Layout', '保险公积金管理', 'ele-OfficeBuilding', 0, 0, 1, 0, 300, 1, 0, GETDATE());
END;

-- 薪酬管理
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000204)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000204, 0, 1, 'salary', '/salary', 'Layout', '薪酬管理', 'ele-Money', 0, 0, 1, 0, 400, 1, 0, GETDATE());
END;

-- 合同管理
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000205)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000205, 0, 1, 'contract', '/contract', 'Layout', '合同管理', 'ele-Notebook', 0, 0, 1, 0, 500, 1, 0, GETDATE());
END;

-- 系统代码设置（空目录，后续挂子模块；亦可单独执行 doc/sql/seed/20261012_sysCode_seed_menu.sql）
IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000206)
BEGIN
    INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon], [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000206, 0, 1, 'sysCode', '/sysCode', 'Layout', '系统代码设置', 'ele-Collection', 0, 0, 1, 0, 600, 1, 0, GETDATE());
END;
