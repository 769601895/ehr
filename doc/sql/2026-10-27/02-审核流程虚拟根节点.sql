/*
 * 变更说明：hr_wf_role.level_id 改为可空；种子组织虚拟根「审核流程」
 * 模块：hrWf 汇报树
 * 日期：2026-10-27
 * 影响库：ehrnew
 * 是否幂等：是
 * 说明：虚拟根无审核等级、不绑岗位/管辖，仅便于挂子节点；不参与派单
 * 执行：sqlcmd -S jack\sql2014 -U sa -P *** -d ehrnew -f 65001 -i 本文件
 */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF EXISTS (
    SELECT 1
    FROM sys.columns c
    JOIN sys.tables t ON c.object_id = t.object_id
    WHERE t.name = N'hr_wf_role' AND c.name = N'level_id' AND c.is_nullable = 0
)
BEGIN
    ALTER TABLE dbo.hr_wf_role ALTER COLUMN level_id BIGINT NULL;
END
GO

DECLARE @now DATETIME = GETDATE();
IF NOT EXISTS (SELECT 1 FROM dbo.hr_wf_role WHERE code = N'WF_TREE_ROOT' AND IsDelete = 0)
BEGIN
    INSERT INTO dbo.hr_wf_role
        (Id, level_id, code, name, parent_id, order_no, enabled, remark, CreateTime, IsDelete)
    VALUES
        (13000000050001, NULL, N'WF_TREE_ROOT', N'审核流程', NULL, 0, 1,
         N'组织虚拟根，不参与审核，仅挂子节点', @now, 0);
END
GO

SELECT Id, code, name, parent_id, level_id, order_no, enabled
FROM dbo.hr_wf_role
WHERE IsDelete = 0 AND code = N'WF_TREE_ROOT';
GO
