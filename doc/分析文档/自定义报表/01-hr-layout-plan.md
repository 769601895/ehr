# 人员资料 — 自定义列表/表单布局可落地方案

模块试点：`hrEmp`（人员资料管理）  
主表：`ehr.dbo.h_emp_mstr`  
字段池：`doc/分析文档/人员资料/field-mapping-mvp.md`  
更新日期：2026-10-12

---

## 1. 问题与目标

### 1.1 现状

- 列表列、新增/编辑表单字段均写死在前端（`Web/src/views/hrms/emp/`）。
- 调整显示列、占位宽度需改代码发版。

### 1.2 目标（对齐旧「报表定义」弹窗）

| 能力 | 描述 |
|------|------|
| 定义显示字段 | 从字段池勾选/增删行，设中文显示名 |
| 占位长度 | 表单：一行 6 格，字段占 1～6 格；列表：列宽(px) |
| 弹窗配置 | 左：表/字段区；右：预览区；保存 / 返回 |
| 方案记忆 | 系统默认 + 可选用户覆盖 |

### 1.3 非目标（本期不做）

- 多表 JOIN 自由报表、任意 SQL。
- 运行时从库表反射全部 200+ 列进表单（仅字段池内可选）。
- 替代代码生成器 / `reportCustomQuery` 全站能力。

---

## 2. 概念模型

### 2.1 方案（Layout Scheme）

| 字段 | 说明 |
|------|------|
| `ModuleCode` | 模块码，固定如 `hrEmp` |
| `Scene` | `list` = 列表列；`form` = 表单布局 |
| `Name` | 方案名称，如「默认列表」 |
| `IsDefault` | 是否系统默认（同 Module+Scene 仅一条默认） |
| `OwnerUserId` | 空=系统方案；有值=用户个人方案 |
| `Version` | 兼容升级用，初始 1 |

### 2.2 明细（Layout Field）

| 字段 | 列表 `list` | 表单 `form` |
|------|-------------|-------------|
| `FieldKey` | 属性名 camelCase，如 `empId` | 同左 |
| `DbColumn` | 可选，如 `emp_id`（展示/导出用） | 同左 |
| `Label` | 列标题 | 表单项标签 |
| `SortNo` | 列顺序 | 表单项顺序 |
| `Visible` | 是否显示 | 是否显示 |
| `WidthPx` | 列宽(px)，必填建议 | 可空 |
| `SpanSlots` | 不用（或固定 1） | **占位格数 1～6**，`el-col :span = SpanSlots * 4` |
| `GroupCode` | 可空 | `basic` / `job`（基本信息/任职信息） |
| `ControlType` | `text`（展示） | `text` / `date` / `number` / `select` / `cascader` / `photo` / `computed` |
| `Required` | 不用 | 前端校验提示（后端仍以 API 为准） |
| `Readonly` | 可空 | 如工号编辑时只读、年龄只读 |

### 2.3 栅格约定（表单）

- 一行 **6 格**（Element 24 栅格：`span = SpanSlots × 4`）。
- 普通字段默认 `SpanSlots=1`；长字段（地址、身份证号、部门）默认 `2`。
- 照片：建议 `SpanSlots=1`，`ControlType=photo`，单独渲染上传区。
- 同一行累计 `SpanSlots` 超过 6 时自动换行。

### 2.4 字段池（白名单）

一期仅允许 MVP 对照表中的字段 + 计算列 `age` + 展示列 `ccDesc`：

```
empId, empName, empCard, empCardOverdate, empCardDate, empCardLimit,
empSex, empBirth, age, empRgt, empNation, empProvince, empResidence,
empMarry, empAddr, empHome, empAddrter, empPic,
ccCode, ccDesc, dutyId, postId, empWorkDate, empInDateGroup, empInDate,
empBegDate, empTryDate, empFromDate, empLongmon, empKqIndate, empKqLeaDate,
empType, empSal, empState
```

保存方案时服务端校验：`FieldKey` ∈ 白名单；`SpanSlots` ∈ [1,6]；`list` 场景忽略非法 Span。

---

## 3. 存储设计

### 3.1 表：`hr_ui_layout`（方案头）

| 列 | 类型 | 说明 |
|----|------|------|
| `id` | bigint PK | 雪花/自增按项目惯例 |
| `module_code` | nvarchar(50) | `hrEmp` |
| `scene` | nvarchar(20) | `list` / `form` |
| `name` | nvarchar(100) | |
| `is_default` | bit | |
| `owner_user_id` | bigint NULL | |
| `version` | int | 默认 1 |
| `create_time` / `update_time` | datetime | |
| `is_delete` | bit | 软删 |

索引：`(module_code, scene, owner_user_id)`；默认方案唯一可用过滤 `is_default=1 AND owner_user_id IS NULL`。

### 3.2 表：`hr_ui_layout_field`（明细）

| 列 | 类型 | 说明 |
|----|------|------|
| `id` | bigint PK | |
| `layout_id` | bigint | FK → hr_ui_layout |
| `field_key` | nvarchar(50) | |
| `db_column` | nvarchar(50) NULL | |
| `label` | nvarchar(50) | |
| `sort_no` | int | |
| `visible` | bit | |
| `width_px` | int NULL | |
| `span_slots` | int NULL | form 用，默认 1 |
| `group_code` | nvarchar(20) NULL | |
| `control_type` | nvarchar(20) | |
| `required` | bit | |
| `readonly` | bit | |

索引：`(layout_id, sort_no)`。

### 3.3 解析优先级

1. 当前用户个人方案（`owner_user_id = 当前用户`，同 module+scene）  
2. 系统默认方案（`is_default=1` 且 `owner_user_id IS NULL`）  
3. 前端内置兜底（与现网写死列一致，防止无种子时空白）

### 3.4 种子

- SQL：`doc/sql/hr-layout/YYYYMMDD_hr_ui_layout_seed.sql`（幂等）  
- 预置两套默认：`hrEmp`+`list`、`hrEmp`+`form`，明细对齐当前页面字段顺序与占位。

---

## 4. 后端 API（建议）

服务名：`hrUiLayout`（`Admin.NET.Application/Service/Hr/Layout/`）

| API | 方法 | 说明 |
|-----|------|------|
| `/api/hrUiLayout/getEffective` | GET | `moduleCode`+`scene` → 当前生效方案+明细 |
| `/api/hrUiLayout/getDefault` | GET | 系统默认（只读预览） |
| `/api/hrUiLayout/saveMy` | POST | 保存/覆盖当前用户个人方案（整单替换明细） |
| `/api/hrUiLayout/resetMy` | POST | 删除个人方案，回退默认 |
| `/api/hrUiLayout/saveDefault` | POST | 管理员维护系统默认（权限：`hrUiLayout/saveDefault`） |
| `/api/hrUiLayout/fieldPool` | GET | 返回模块字段池元数据（key/label/controlType/默认 span） |

权限建议挂在人员资料菜单下按钮，或独立「界面布局」按钮。

**安全：**

- `save*` 校验字段白名单与 Span 范围。  
- 禁止客户端传入非池字段。  
- 列表导出若后续做，列集以生效方案为准，仍走现有 `page` 权限。

---

## 5. 前端改造

### 5.1 配置弹窗（对齐截图二）

路径建议：`Web/src/views/hrms/emp/component/layoutDefineDialog.vue`

| 区域 | 行为 |
|------|------|
| 左：字段区 | 表格行：增/删、序号、表名、中文显示、字段名(下拉自字段池)、宽度(list)、占位格数(form)、上移下移 |
| 右：显示区 | `list`：表头预览；`form`：按 Span 渲染简易表单项骨架 |
| 顶栏 | 保存、重置为默认、返回 |

入口：

- 列表页工具栏：「列设置」→ `scene=list`  
- 编辑弹窗标题旁或列表页：「表单布局」→ `scene=form`（二期）

### 5.2 列表页（一期必做）

文件：`Web/src/views/hrms/emp/index.vue`

1. `onMounted` 调 `getEffective(hrEmp, list)`。  
2. `useVxeTable` 的 `columns` 由配置生成（保留操作列固定右侧）。  
3. 排序字段仍用 `field_key`；不可排序列（如 `age`、`ccDesc`）配置里可加 `sortable:false`（字段池元数据带出）。  
4. 本地 `localPageParam:hrEmp` 的 defaultSort 若指向已隐藏列，回退 `empId`。

### 5.3 表单页（二期）

文件：`Web/src/views/hrms/emp/component/editEmp.vue`

1. 按 `group_code` 分区块 + `v-for` 渲染。  
2. `:span="spanSlots * 4"`。  
3. 按 `control_type` 映射控件（日期/数字/级联部门/照片/只读年龄）。  
4. 工号、姓名在字段池标记 `required` 且不可从方案中删除（保存时服务端强制补回）。

### 5.4 共享

- `Web/src/api/hrms/hrUiLayout.ts`  
- 可选：`composables/useHrUiLayout.ts`（拉配置、生成 columns/form schema）

---

## 6. 分期计划

| 阶段 | 内容 | 产出 | 预估 |
|------|------|------|------|
| **P0** | 表结构 + 默认种子 + `fieldPool`/`getEffective` | SQL + 只读 API | 0.5～1 天 |
| **P1** | 列表列定义弹窗 + 列表按配置渲染 + `saveMy`/`resetMy` | 可演示「列设置」 | 1.5～2 天 |
| **P2** | 表单布局弹窗 + `editEmp` 动态渲染 | 可调占位/显隐 | 2～3 天 |
| **P3** | 管理员改默认方案、导入导出方案 JSON | 运维友好 | 0.5～1 天 |

建议：**P0→P1 先上线**；P2 在占位/日期显示稳定后再做。

---

## 7. 默认方案草案（与现网对齐）

### 7.1 `list` 默认列（顺序）

| sort | fieldKey | label | widthPx |
|------|----------|-------|---------|
| 1 | empId | 工号 | 100 |
| 2 | empName | 姓名 | 100 |
| 3 | ccDesc | 部门 | 140 |
| 4 | empSex | 性别 | 70 |
| 5 | age | 年龄 | 70 |
| 6 | empAddrter | 联系电话 | 120 |
| 7 | dutyId | 职务 | 90 |
| 8 | postId | 岗位 | 90 |
| 9 | empInDate | 公司入职日 | 120 |
| 10 | empType | 人员类别 | 100 |
| 11 | empSal | 薪酬类别 | 90 |
| 12 | empState | 状态 | 80 |

操作列不进配置表，代码固定。

### 7.2 `form` 默认占位（节选）

| fieldKey | group | spanSlots | controlType |
|----------|-------|-----------|-------------|
| empId | basic | 1 | text |
| empName | basic | 1 | text |
| empSex | basic | 1 | text |
| empBirth | basic | 1 | date |
| age | basic | 1 | computed |
| empPic | basic | 1 | photo |
| empCard | basic | 2 | text |
| empResidence | basic | 2 | text |
| empAddr | basic | 2 | text |
| empHome | basic | 2 | text |
| ccCode | job | 2 | cascader |
| 其余日期/短文本 | job/basic | 1 | date/text/number |

完整种子以落库 SQL 为准，与 `editEmp.vue` 现网顺序保持一致后冻结一版。

---

## 8. 验收清单

### P1 列表

- [ ] 打开「列设置」可增删字段、改标题/宽度/顺序并预览表头  
- [ ] 保存后刷新列表列集与顺序一致  
- [ ] 重置后恢复系统默认  
- [ ] 非法字段保存被后端拒绝  
- [ ] 列头排序：`field`/`order`/`descStr` 仍生效（可见且可排序列）

### P2 表单

- [ ] 改占位格数后，一行累计按 6 格换行，长字段占 2 格  
- [ ] 隐藏非强制字段后，新增/编辑不再展示，提交不报错  
- [ ] 工号/姓名无法从方案删除  
- [ ] 照片、部门级联仍可用  

---

## 9. 风险与对策

| 风险 | 对策 |
|------|------|
| 日期列过窄显示不全 | list 用 widthPx 下限；form 长标签字段默认 span≥1 且弹窗保持加宽 |
| 配置被清空导致白屏 | 前端内置兜底 columns/schema |
| 与后端 DTO 不一致 | 字段池与 `HrEmpOutput`/`HrEmpSaveInput` 同步维护 |
| 用户方案过多 | 每用户每 scene 仅保留一套（save 覆盖） |

---

## 10. 实现文件清单（已落地）

| 类型 | 路径 |
|------|------|
| SQL 表/种子 | `doc/sql/hr-layout/20261013_hr_ui_layout_tables.sql`、`*_seed_default.sql` |
| 菜单 | `doc/sql/seed/20261013_hrReportDef_children_menu.sql`、`*_children_roleMenu.sql` |
| 实体 | `Admin.NET.Application/Entity/HrUiLayout.cs`、`HrUiLayoutField.cs` |
| 服务 | `Admin.NET.Application/Service/Hr/Layout/HrUiLayoutService.cs` |
| API | `Web/src/api/hrms/hrUiLayout.ts` |
| 定义页 | `Web/src/views/hrms/reportDef/listDef.vue`、`formDef.vue`、`component/layoutEditor.vue` |
| 列表消费 | `Web/src/views/hrms/emp/index.vue`（P1） |
| 表单消费 | `Web/src/views/hrms/emp/component/editEmp.vue`（P2，定义页已就绪、编辑页待接） |

---

## 11. 决策记录

| 项 | 结论 |
|----|------|
| 首期场景 | **先 list，后 form** |
| 占位模型 | 表单用格数 1～6；列表用宽度 px |
| 存储 | 独立 `hr_ui_layout*`，不复用 CodeGen 表 |
| 个人方案 | 每用户每 scene 一套，可重置 |
| 字段来源 | MVP 白名单，不开放全表反射 |
