/*
 * 变更说明：补充运行时/表单查询等按钮权限并授权管理员
 * 日期：2026-10-24
 * 是否幂等：是
 */

SET NOCOUNT ON;

DECLARE @RoleId BIGINT = 1300000000101;

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000302)
    INSERT INTO SysMenu ([Id], [Pid], [Type], [Name], [Title], [Permission],
        [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000302, 1300000000298, 3, N'hrWfTodoDone', N'已办', N'hrWfTodo/pageMyDone',
        0, 0, 1, 0, 130, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000303)
    INSERT INTO SysMenu ([Id], [Pid], [Type], [Name], [Title], [Permission],
        [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000303, 1300000000298, 3, N'hrWfTodoApplied', N'我发起的', N'hrWfTodo/pageMyApplied',
        0, 0, 1, 0, 140, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000304)
    INSERT INTO SysMenu ([Id], [Pid], [Type], [Name], [Title], [Permission],
        [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000304, 1300000000298, 3, N'hrWfRuntimeDetail', N'详情', N'hrWfRuntime/getDetail',
        0, 0, 1, 0, 150, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000305)
    INSERT INTO SysMenu ([Id], [Pid], [Type], [Name], [Title], [Permission],
        [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000305, 1300000000288, 3, N'hrWfFormByBiz', N'按业务取表单', N'hrWfForm/getByBizType',
        0, 0, 1, 0, 130, 1, 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM SysMenu WHERE Id = 1300000000306)
    INSERT INTO SysMenu ([Id], [Pid], [Type], [Name], [Title], [Permission],
        [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
    VALUES (1300000000306, 1300000000280, 3, N'hrWfLevelList', N'启用列表', N'hrWfLevel/listEnabled',
        0, 0, 1, 0, 130, 1, 0, GETDATE());

DECLARE @Mid BIGINT;
DECLARE @Seq INT = 0;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT Id FROM SysMenu WHERE Id BETWEEN 1300000000302 AND 1300000000306;
OPEN c;
FETCH NEXT FROM c INTO @Mid;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM SysRoleMenu WHERE RoleId = @RoleId AND MenuId = @Mid)
        INSERT INTO SysRoleMenu (Id, RoleId, MenuId)
        VALUES (1300000001822 + @Seq, @RoleId, @Mid);
    SET @Seq = @Seq + 1;
    FETCH NEXT FROM c INTO @Mid;
END
CLOSE c;
DEALLOCATE c;
