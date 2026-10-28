/*
 * 变更说明：流程配置管理下挂审批引擎菜单（等级/角色/表单/定义/发起/待办）
 * 模块：hrWf
 * 日期：2026-10-24
 * 影响库：ehrnew（SysMenu）
 * 是否幂等：是
 * 前置：1300000000208 流程配置管理目录已存在
 */

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000208)
BEGIN
    RAISERROR(N'菜单 1300000000208（流程配置管理）不存在。', 16, 1);
END
ELSE
BEGIN
    /* 审核等级 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000280)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000280, 1300000000208, 2, N'hrWfLevel', N'/moduleFlowConfig/wfLevel', N'/hrms/wf/level/index',
             N'审核等级', N'ele-Medal', 0, 0, 1, 0, 10, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000281)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000281, 1300000000280, 3, N'hrWfLevelPage', N'查询', N'hrWfLevel/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000282)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000282, 1300000000280, 3, N'hrWfLevelSave', N'保存', N'hrWfLevel/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000283)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000283, 1300000000280, 3, N'hrWfLevelDelete', N'删除', N'hrWfLevel/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    /* 审批角色 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000284)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000284, 1300000000208, 2, N'hrWfRole', N'/moduleFlowConfig/wfRole', N'/hrms/wf/role/index',
             N'审批角色', N'ele-UserFilled', 0, 0, 1, 0, 20, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000285)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000285, 1300000000284, 3, N'hrWfRolePage', N'查询', N'hrWfRole/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000286)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000286, 1300000000284, 3, N'hrWfRoleSave', N'保存', N'hrWfRole/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000287)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000287, 1300000000284, 3, N'hrWfRoleDelete', N'删除', N'hrWfRole/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    /* 审批表单 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000288)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000288, 1300000000208, 2, N'hrWfForm', N'/moduleFlowConfig/wfForm', N'/hrms/wf/form/index',
             N'审批表单', N'ele-Document', 0, 0, 1, 0, 30, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000289)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000289, 1300000000288, 3, N'hrWfFormPage', N'查询', N'hrWfForm/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000290)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000290, 1300000000288, 3, N'hrWfFormSave', N'保存', N'hrWfForm/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000291)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000291, 1300000000288, 3, N'hrWfFormDelete', N'删除', N'hrWfForm/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    /* 流程定义 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000292)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000292, 1300000000208, 2, N'hrWfDef', N'/moduleFlowConfig/wfDef', N'/hrms/wf/def/index',
             N'流程定义', N'ele-SetUp', 0, 0, 1, 0, 40, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000293)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000293, 1300000000292, 3, N'hrWfDefPage', N'查询', N'hrWfDef/page',
            0, 0, 1, 0, 100, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000294)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000294, 1300000000292, 3, N'hrWfDefSave', N'保存', N'hrWfDef/save',
            0, 0, 1, 0, 110, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000295)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000295, 1300000000292, 3, N'hrWfDefDelete', N'删除', N'hrWfDef/delete',
            0, 0, 1, 0, 120, 1, 0, GETDATE());

    /* 发起申请 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000296)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000296, 1300000000208, 2, N'hrWfStart', N'/moduleFlowConfig/wfStart', N'/hrms/wf/start/index',
             N'发起申请', N'ele-Promotion', 0, 0, 1, 0, 50, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000297)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000297, 1300000000296, 3, N'hrWfRuntimeStart', N'发起', N'hrWfRuntime/start',
            0, 0, 1, 0, 100, 1, 0, GETDATE());

    /* 我的待办 */
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000298)
        INSERT INTO [SysMenu]
            ([Id], [Pid], [Type], [Name], [Path], [Component], [Title], [Icon],
             [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES
            (1300000000298, 1300000000208, 2, N'hrWfTodo', N'/moduleFlowConfig/wfTodo', N'/hrms/wf/todo/index',
             N'我的待办', N'ele-Bell', 0, 0, 1, 0, 60, 1, 0, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000299)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000299, 1300000000298, 3, N'hrWfTodoPage', N'待办', N'hrWfTodo/pageMyTodo',
            0, 0, 1, 0, 100, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000300)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000300, 1300000000298, 3, N'hrWfRuntimeApprove', N'同意', N'hrWfRuntime/approve',
            0, 0, 1, 0, 110, 1, 0, GETDATE());
    IF NOT EXISTS (SELECT 1 FROM [SysMenu] WHERE [Id] = 1300000000301)
        INSERT INTO [SysMenu] ([Id], [Pid], [Type], [Name], [Title], [Permission],
            [IsIframe], [IsHide], [IsKeepAlive], [IsAffix], [OrderNo], [Status], [IsDelete], [CreateTime])
        VALUES (1300000000301, 1300000000298, 3, N'hrWfRuntimeReject', N'驳回', N'hrWfRuntime/reject',
            0, 0, 1, 0, 120, 1, 0, GETDATE());
END

SELECT [Id], [Pid], [Name], [Title], [Path], [Component], [OrderNo]
FROM [SysMenu]
WHERE [Id] BETWEEN 1300000000280 AND 1300000000301 OR [Id] = 1300000000208
ORDER BY [Id];
