# SDD ledger — plan: docs/superpowers/plans/2026-10-28-hr-lookup-def.md

Pre-flight: Task3 消费 Task1 实体 + Task2 Guard；Task4/5/7 消费 LookupSource / GetOptions / Lang API — 接口与 Spec 一致，无冲突。
Ruling: 在当前 main 工作区继续实现（用户已选 Native 并说继续）— 未另开 worktree — 若需隔离分支可事后 cherry-pick。
Ruling: 不自动 git commit（用户未要求提交）。

Task 1: complete — DDL 已执行；实体 HrLookupDef/HrPostLang/LookupSource 已加
Task 2: complete — HrLookup 单测 14 passed
Task 3: complete — HrLookupDefService Page/Save/GetOptions
Task 4: complete — HrPost LangList/SaveLang + 改码迁移
Task 5: complete — 字段池互斥 + Effective 透出 LookupSource
Task 6: complete — 种子 hrPost + postId + 菜单
Task 7: complete — lookupDef 页、fieldDef、post 多语、emp 消费
Ruling: getOptions 权限挂在查找数据源菜单下，使用人员表单的角色需授权 `hrLookupDef/getOptions`（或超级管理员）— 否则下拉 403。
