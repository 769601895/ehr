/*
 * 变更说明：种子 hrPost 数据源；postId 绑 lookup_source；查找数据源菜单+角色授权；岗位多语按钮
 * 日期：2026-10-29
 * 影响库：ehrnew
 * 是否幂等：是
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @now DATETIME = GETDATE();

-- 1) hr_lookup_def: hrPost
IF NOT EXISTS (SELECT 1 FROM dbo.hr_lookup_def WHERE code = N'hrPost' AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_lookup_def
    (
        Id, code, name, table_name, value_column, label_column, order_column,
        filter_use_is_delete, filter_enabled_column, filter_enabled_value,
        lang_table, lang_value_column, lang_culture_column, lang_label_column,
        maintain_path, enabled, CreateTime, IsDelete
    )
    VALUES
    (
        1300000003301, N'hrPost', N'岗位代码', N'hr_post', N'post_code', N'post_name', N'post_code',
        1, N'status', N'Active',
        N'hr_post_lang', N'post_code', N'culture', N'post_name',
        N'/sysCode/hrPost', 1, @now, 0
    );
END
ELSE
BEGIN
    UPDATE dbo.hr_lookup_def
    SET name = N'岗位代码',
        table_name = N'hr_post',
        value_column = N'post_code',
        label_column = N'post_name',
        order_column = N'post_code',
        filter_use_is_delete = 1,
        filter_enabled_column = N'status',
        filter_enabled_value = N'Active',
        lang_table = N'hr_post_lang',
        lang_value_column = N'post_code',
        lang_culture_column = N'culture',
        lang_label_column = N'post_name',
        maintain_path = N'/sysCode/hrPost',
        enabled = 1,
        UpdateTime = @now
    WHERE code = N'hrPost' AND IsDelete = 0;
END

-- 2) hrEmp.postId -> lookup_source
UPDATE dbo.hr_ui_field_def
SET lookup_source = N'hrPost',
    code_field = NULL,
    control_type = N'select',
    UpdateTime = @now
WHERE module_code = N'hrEmp' AND field_key = N'postId' AND IsDelete = 0;

-- 3) 菜单：查找数据源设置
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000320)
    INSERT INTO dbo.SysMenu
        (Id, Pid, Type, Name, Path, Component, Title, Icon,
         IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES
        (1300000000320, 1300000000206, 2, N'hrLookupDef', N'/sysCode/hrLookupDef', N'/hrms/lookupDef/index',
         N'查找数据源设置', N'ele-Connection',
         0, 0, 1, 0, 20, 1, 0, @now);
ELSE
    UPDATE dbo.SysMenu
    SET Pid = 1300000000206, Type = 2, Name = N'hrLookupDef',
        Path = N'/sysCode/hrLookupDef', Component = N'/hrms/lookupDef/index',
        Title = N'查找数据源设置', Icon = N'ele-Connection',
        IsDelete = 0, Status = 1, OrderNo = 20
    WHERE Id = 1300000000320;

IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000321)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000321, 1300000000320, 3, N'hrLookupDefPage', N'查询', N'hrLookupDef/page', 0, 0, 1, 0, 100, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000322)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000322, 1300000000320, 3, N'hrLookupDefSave', N'保存', N'hrLookupDef/save', 0, 0, 1, 0, 110, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000323)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000323, 1300000000320, 3, N'hrLookupDefDelete', N'删除', N'hrLookupDef/delete', 0, 0, 1, 0, 120, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000324)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000324, 1300000000320, 3, N'hrLookupDefOptions', N'选项', N'hrLookupDef/getOptions', 0, 0, 1, 0, 130, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000325)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000325, 1300000000320, 3, N'hrLookupDefList', N'启用列表', N'hrLookupDef/listEnabled', 0, 0, 1, 0, 140, 1, 0, @now);

-- 岗位多语按钮
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000314)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000314, 1300000000310, 3, N'hrPostLangList', N'译文查询', N'hrPost/langList', 0, 0, 1, 0, 130, 1, 0, @now);
IF NOT EXISTS (SELECT 1 FROM dbo.SysMenu WHERE Id = 1300000000315)
    INSERT INTO dbo.SysMenu (Id, Pid, Type, Name, Title, Permission, IsIframe, IsHide, IsKeepAlive, IsAffix, OrderNo, Status, IsDelete, CreateTime)
    VALUES (1300000000315, 1300000000310, 3, N'hrPostSaveLang', N'译文保存', N'hrPost/saveLang', 0, 0, 1, 0, 140, 1, 0, @now);

-- 4) RoleMenu sys_admin
DECLARE @RoleId BIGINT = 1300000000101;
;WITH grants AS (
    SELECT * FROM (VALUES
        (1300000003190, 1300000000320),
        (1300000003191, 1300000000321),
        (1300000003192, 1300000000322),
        (1300000003193, 1300000000323),
        (1300000003194, 1300000000324),
        (1300000003195, 1300000000325),
        (1300000003196, 1300000000314),
        (1300000003197, 1300000000315)
    ) AS v(Id, MenuId)
)
INSERT INTO dbo.SysRoleMenu (Id, RoleId, MenuId)
SELECT g.Id, @RoleId, g.MenuId
FROM grants g
WHERE NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.RoleId = @RoleId AND rm.MenuId = g.MenuId)
  AND NOT EXISTS (SELECT 1 FROM dbo.SysRoleMenu rm WHERE rm.Id = g.Id);

PRINT N'查找数据源种子与菜单已同步';
GO
