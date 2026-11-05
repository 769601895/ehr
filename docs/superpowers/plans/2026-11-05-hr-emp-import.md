# 员工资料导入 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 落地与「人员资料管理」同级的员工资料导入模块（个人动态模板、新增/修改分模板、整批事务、导入历史、字段权限预留）。

**Architecture:** 独立 `hrEmpImport` API + 三 Tab 前端页；字段池取自 `HrUiFieldDef`（剔除 EmpState）；写库复用 `HrEmpService.Add/Update`（调整 Add：有填工号则用填的）；Excel 用 MiniExcel 读写标准 `.xlsx`（兼容 WPS）。

**Tech Stack:** Admin.NET / SqlSugar / MiniExcel / Vue3 + Element Plus + vxe-table / vue-i18n

**Spec:** `doc/分析文档/员工资料导入/01-设计方案.md`

## Global Constraints

- SQL 按日归档 `doc/sql/2026-11-05/`
- 列表历史须 `OrderBuilder` + 前端 sortable
- UI 五语 i18n；模板名个人私有无 lang 表
- 工号遵循 emp-id-assign / company-param-resolve
- 单批上限 **1000** 行；文件 **5MB**；历史仅本人；表头严格匹配

## Review Focus

- 修改空单元格不覆盖
- 新增有工号用填、无则发号；预览不占号
- EmpState 不可导入
- Lookup 码/名都认、歧义失败
- 整批失败仍落 Failed 历史 + 行错误

---

### Task 1: SQL + Entities

**Files:**
- Create: `doc/sql/2026-11-05/10-员工资料导入建表.sql`
- Create: `doc/sql/2026-11-05/11-员工资料导入菜单权限.sql`
- Create: `Admin.NET/.../Entity/HrEmpImportTpl.cs` 等 4 实体

- [ ] 建表 tpl / tpl_field / batch / batch_row
- [ ] 菜单 1300000000370 挂 Pid=1300000000201，按钮权限 hrEmpImport/*
- [ ] 管理员角色授权

### Task 2: Backend Service

**Files:**
- Create: `Service/Hr/EmpImport/*`
- Modify: `HrEmpService.Add` 优先用手填工号

- [ ] `IHrEmpImportFieldFilter` + 默认恒等实现
- [ ] FieldPool / Tpl CRUD / Download / Preview / Commit / Batch Page+FailRows
- [ ] Lookup 解析（码|名）；禁止 empState

### Task 3: Frontend

**Files:**
- Create: `Web/src/views/hrms/empImport/index.vue` + components
- Create: `Web/src/api/hrms/hrEmpImport.ts`
- Modify: `Web/src/i18n/pages/hrms/{zh-CN,zh-TW,en,th,vi}.ts` + menu

- [ ] 三 Tab：导入 / 我的模板 / 历史
- [ ] accept=.xlsx；整批预览后提交

### Task 4: Verify

- [ ] 编译后端；前端无阻塞 lint
- [ ] 对照验收清单
