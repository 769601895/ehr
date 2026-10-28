-- 审核角色汇报汇报链上线：可选清父子关系，供管理员在新「左树右表」页面重建全局汇报树。
-- 说明：不导入旧体系；在途实例仍走开单时快照（LegacyLevel）。仅新开单使用 ReportPath。
-- 执行前请确认环境环境，并评估是否有仍依赖脏 parent_id 的手工数据。

-- 可选：将既有 parent_id 全部置空（全部变为根），再由管理员重建汇报上下级
UPDATE hr_wf_role
SET parent_id = NULL
WHERE parent_id IS NOT NULL
  AND IsDelete = 0;  -- EntityBase.IsDelete；若库中为 is_delete 请改列名

-- 校验
-- SELECT Id, code, name, parent_id, level_id, order_no FROM hr_wf_role WHERE IsDelete = 0 ORDER BY order_no, Id;
