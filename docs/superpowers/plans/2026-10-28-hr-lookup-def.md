# 查找数据源可配与多语言 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 字段池可通过后台配置的查找数据源绑定 HR 主档表下拉，并支持岗位等多语言 Label。

**Architecture:** 新增 `hr_lookup_def` 描述表/列/固定过滤/可选翻译表；统一 `GetOptions` 在 HR 库安全拼查询并用 `HrUiI18nResolver.ResolveCodeDesc` 解析；`hr_ui_field_def.lookup_source` 与 `code_field` 互斥；岗位侧新增 `hr_post_lang` 与代码类同构的多语维护。

**Tech Stack:** Admin.NET / SqlSugar / Vue3 + vxe-grid；HR 库 `ehrnew`；SQL 归档 `doc/sql/yyyy-MM-dd/`。

**Spec:** `doc/分析文档/通用表单设计/02-查找数据源可配与多语言.md`

## Global Constraints

- 表名/列名仅允许 `^[A-Za-z_][A-Za-z0-9_]*$`，只查 `ApplicationConst.HrDbConfigId`
- 列表页必须服务端排序（`OrderBuilder` + `field`/`order`/`descStr`）
- 分析/SQL 新稿：中文分析名；SQL 按日目录 `doc/sql/{yyyy-MM-dd}/`
- 异常对用户使用中文/`ErrorCodeItemMetadata`，不直接抛裸键
- 过滤第一期仅：IsDelete、启用列=值、等值一列；不做任意 SQL

## Review Focus

- 未配翻译表时 GetOptions 仍返回主列 Label，不报错
- 改 post_code 时译文行同步迁移，避免孤儿 culture 行
- lookup_source 指向停用/不存在编码：选项空列表 + 字段保存可提示
- 标识符注入：非法表名被拒绝，不进 SQL
- 表单同时误配 code_field 与 lookup_source：保存失败

---

### Task 1: DDL + 实体（lookup_def / post_lang / field_def 列）

**Files:**
- Create: `doc/sql/{当天}/07-查找数据源与岗位译文.sql`
- Create: `Admin.NET/Admin.NET.Application/Entity/HrLookupDef.cs`
- Create: `Admin.NET/Admin.NET.Application/Entity/HrPostLang.cs`
- Modify: `Admin.NET/Admin.NET.Application/Entity/HrUiFieldDef.cs` — 增加 `LookupSource`
- Test: 手工执行 SQL 后 `describe`/`SELECT` 验证列存在

**Interfaces:**
- Produces: `HrLookupDef`（含过滤与 lang_* 字段）、`HrPostLang(PostCode,Culture,PostName)`、`HrUiFieldDef.LookupSource`

- [ ] **Step 1: 编写幂等 DDL**（`hr_lookup_def`、`hr_post_lang`、`hr_ui_field_def.lookup_source`）并执行到 `ehrnew`
- [ ] **Step 2: 编写实体与 Sugar 映射（Tenant=HrDb）**
- [ ] **Step 3: 验证表结构与编译通过**

---

### Task 2: GetOptions 安全查询 + 单元测试

**Files:**
- Create: `Admin.NET/Admin.NET.Application/Service/Hr/Lookup/HrLookupSqlGuard.cs`
- Create: `Admin.NET/Admin.NET.Application/Service/Hr/Lookup/HrLookupOptionsBuilder.cs`（或同目录服务内 private）
- Create: `Admin.NET/Admin.NET.Application.Tests/HrLookup/HrLookupSqlGuardTests.cs`
- Create: `Admin.NET/Admin.NET.Application.Tests/HrLookup/HrLookupResolveTests.cs`

**Interfaces:**
- Produces: `HrLookupSqlGuard.IsSafeIdent(string) -> bool`
- Produces: 解析结果使用 `HrUiI18nResolver.ResolveCodeDesc(value, main, culture, langMap)`

- [ ] **Step 1: 写失败测试** — 非法标识符返回 false；合法 `post_code` true
- [ ] **Step 2: 实现 Guard；测试通过**
- [ ] **Step 3: 写测试** — 有译文用译文，无则回退主描述（直接测 ResolveCodeDesc 调用约定即可）
- [ ] **Step 4: Commit**（若用户要求提交时再提交）

---

### Task 3: HrLookupDefService（CRUD + listEnabled + getOptions）

**Files:**
- Create: `Admin.NET/Admin.NET.Application/Service/Hr/Lookup/Dto/HrLookupDefDto.cs`
- Create: `Admin.NET/Admin.NET.Application/Service/Hr/Lookup/HrLookupDefService.cs`
- Modify: 如需 `HrWfErrorCodeEnum` 风格，可新增少量中文 Oops 文案或直接中文 `Oops.Oh`

**Interfaces:**
- Produces: `Page` / `Save` / `Delete` / `ListEnabled` / `GetOptions(string source) -> List<{Value,Label,Text}>`
- Consumes: Task1 实体、Task2 Guard、`HrUiCulture.Current()`、SqlSugar Ado 或动态查询 HR 库

- [ ] **Step 1: 实现 Page（OrderBuilder）与 Save 校验（编码唯一、标识符安全、启用列与值成对）**
- [ ] **Step 2: 实现 GetOptions：读 def → 拼参数化 SQL → 批量 lang → ResolveCodeDesc**
- [ ] **Step 3: 手工或集成验证 source=`hrPost`（种子前可先插一条 def）**

---

### Task 4: 岗位多语 API

**Files:**
- Modify: `Admin.NET/Admin.NET.Application/Service/Hr/Post/HrPostService.cs`
- Modify: `Admin.NET/Admin.NET.Application/Service/Hr/Post/Dto/HrPostDto.cs`

**Interfaces:**
- Produces: `LangList(postCode)` / `SaveLang(postCode, items)`；Save 改码时迁移 `hr_post_lang`
- Consumes: `HrPostLang`；文化列表与 `hrCodeMstr` 一致（前端 `hrUiCultures`）

- [ ] **Step 1: 参照 HrCodeMstrService 的 LangList/SaveLang 实现岗位译文**
- [ ] **Step 2: Save 变更 post_code 时更新 lang 表键**
- [ ] **Step 3: 验证改码后译文仍挂在新码上**

---

### Task 5: 字段池 lookup_source 互斥

**Files:**
- Modify: `Admin.NET/Admin.NET.Application/Service/Hr/FieldDef/HrUiFieldDefService.cs`（及 DTO/Effective 透出）
- Modify: 布局 Effective 输出类型（若有独立 DTO）含 `lookupSource`

**Interfaces:**
- Consumes: `LookupSource` 列
- Produces: Save 时二者皆非空 → `Oops.Oh("代码类与查找数据源不能同时设置")`；Effective/GetPool 返回 `lookupSource`

- [ ] **Step 1: DTO + Save 校验 + 查询 Select 带上 LookupSource**
- [ ] **Step 2: 确认 emp Effective 响应含 lookupSource**

---

### Task 6: 种子 SQL（def + postId + 菜单）

**Files:**
- Create: `doc/sql/{当天}/08-查找数据源种子与菜单.sql`
- Modify: `Admin.NET/Admin.NET.Application/SeedData/SysMenuSeedData.cs`（菜单 Id 段避开冲突，参考岗位 `130000000031x`）

**Interfaces:**
- Produces: `hr_lookup_def` 行 `hrPost`；`hr_ui_field_def` 中 `hrEmp`+`postId` 设 `lookup_source=hrPost` 并清空 `code_field`；菜单「查找数据源设置」+ RoleMenu

- [ ] **Step 1: 写幂等 MERGE/UPDATE 种子并执行**
- [ ] **Step 2: 同步 SysMenuSeedData**
- [ ] **Step 3: 验证 GetOptions(hrPost) 返回已有岗位**

---

### Task 7: 前端 — 数据源设置页 + 字段元数据 + 岗位多语 + 消费

**Files:**
- Create: `Web/src/api/hrms/hrLookupDef.ts`
- Create: `Web/src/views/hrms/lookupDef/index.vue`
- Modify: `Web/src/views/hrms/fieldDef/index.vue` — 数据源下拉与代码类互斥、跳转 maintainPath
- Modify: `Web/src/views/hrms/post/index.vue` — 多语弹窗
- Modify: `Web/src/api/hrms/hrPost.ts` — lang API
- Modify: `Web/src/views/hrms/emp/component/editEmp.vue`、`empDynamicForm.vue`、`emp/index.vue` — lookupSource 批拉 options / 列表翻译
- Modify: `Web/src/i18n/pages/hrms/zh-CN.ts`（及 en 等）文案

**Interfaces:**
- Consumes: Task3/4/5 API；选项形状与 codeList 相同 `{ value, label, text }`

- [ ] **Step 1: lookupDef 列表页（排序+CRUD）**
- [ ] **Step 2: fieldDef 绑定 UI**
- [ ] **Step 3: post 多语 UI**
- [ ] **Step 4: emp 表单/列表改走 getOptions(lookupSource)**
- [ ] **Step 5: 切语言验证岗位 Label；验收清单勾选 Spec §8**

---

## 自检（对照 Spec）

| Spec 节 | Task |
|---------|------|
| §3.1 hr_lookup_def | 1, 3, 6 |
| §3.2 hr_post_lang | 1, 4, 7 |
| §3.3 lookup_source | 1, 5, 6, 7 |
| §4 GetOptions / 多语解析 | 2, 3 |
| §5 前端 | 7 |
| §6 种子 | 6 |
| §7 非目标 | 未排期复杂条件 |

---

**推荐执行方式：** Native（本会话连续实现）——任务接口依赖链清晰，拆 subagent 收益有限。若要更严审查用 Subagent-driven。
