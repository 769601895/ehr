# 界面报表定义 — 字段显示多语言方案分析

> 日期：2026-10-13  
> 范围：HrUiLayout 字段池默认名、布局自定义 Label（列表列标题 / 表单项标签）；与语言切换联动的显示层  
> 目标：切换语言环境时，界面/报表定义中的**字段显示文本**同步切换；业务存值（FieldKey / Value）不变  
> 相关：`doc/分析文档/代码类设置多语言分析/`（业务码显示）；`doc/分析文档/通用表单设计/01-通用表单设计.md`（布局与字段池）

---

## 1. 结论

当前系统**切换语言只影响 UI 壳文案**（vue-i18n / `Lang.*.json`）。  
**HrUiLayout 字段池与布局明细的 Label 都是单语中文字符串**，切换语言后列标题、表单项标签不会变。

界面字段显示属于**业务元数据多语言**，与「代码类设置」同类问题：必须增加**多语言解析层**（库表翻译或等价结构），不能仅依赖现有 UI 资源包。

**推荐**：复用代码类方案 B 的**翻译表 + culture 解析 + 主语言回退**；字段池默认名与布局自定义覆盖分层存储，API 按 `Accept-Language` 返回已解析 Label。

---

## 2. 分层职责（先划清）

| 层 | 内容 | 现有/规划方案 | 本分析是否覆盖 |
|----|------|---------------|----------------|
| UI 壳 | 按钮、菜单、固定页面文案 | vue-i18n + `Lang.*.json` | 仅约定边界；HR 页面壳可另迁 i18n |
| 业务码显示 | 性别/工种等下拉文字 | 建议 `code_mstr_lang`（见代码类分析） | 不重复实现，消费端用解析后 Label |
| **字段元数据** | 列标题、表单项 Label（工号/姓名…） | 目前单语 `Label` | **本文重点** |
| 业务数据值 | 员工姓名、备注内容等 | 一般不翻译 | 不做 |

约定：

| 场景 | 约定 |
|------|------|
| 存库、关联、权限、过滤、布局绑定 | 一律使用 **FieldKey**（及业务 Value/Code） |
| 界面展示 | 一律使用 **当前语言 Label** |
| 缺翻译 | 回退主语言（建议固定 zh-CN），不抛错、不空白 |
| 语言切换 | 已保存的字段集合与 FieldKey **不变**，只变显示文字 |

---

## 3. 现状

### 3.1 布局与字段 Label

| 项 | 说明 |
|----|------|
| 方案表 | `hr_ui_layout`（模块 + 场景 list/form + 默认/个人方案） |
| 明细表 | `hr_ui_layout_field`：`FieldKey` + 单字段 `Label` + 顺序/宽/占位等 |
| 字段池 | `HrUiLayoutService.GetPool`：Label 为 C# 中文字面量（如「工号」「姓名」） |
| 前端消费 | 列表等用 API 返回的 `f.label` 作列 `title`；HRMS 页几乎无 `$t` |
| 布局方案文档 | `通用表单设计/01-通用表单设计.md`：已落地布局一期为中文显示名；多语言按翻译表分期 |
| 多语言列 / 翻译表 | **无** |

### 3.2 语言切换链路（已有，可复用）

1. 用户选择语言 → `themeConfig.globalI18n` + Local 持久化  
2. `locale.value = lang`（vue-i18n）  
3. 刷新当前页签  
4. `POST /api/sysCommon/setCulture?culture=...`  
5. 后续 API 请求头携带 `Accept-Language`

入口：`Web/src/layout/navBars/topBar/user.vue` → `onLanguageChange`。

支持语言：`zh-CN` / `zh-TW` / `en` / `th` / `vi`（与 `App.json` LocalizationSettings、前端 `languageList` 一致）。

### 3.3 与代码类多语言的关系

| 模式 | 代码类分析结论 | 界面字段可否复用 |
|------|----------------|------------------|
| 前端 i18n 包覆盖 | 适合固定系统字典 | 仅适合不可配置的固定列；**不适合**布局自定义 Label |
| 翻译表（推荐） | `code_mstr_lang` | **同构可复用** |
| JSON 多语言列 | 折中、长期不如翻译表 | 仅可作试点 |
| `I18nConverter` + 中文 key | 适合菜单等固定标题 | 不适合运营可编辑显示名 |

---

## 4. 差距（Gap）

1. **字段池**：默认 Label 硬编码中文，无 culture 维度。  
2. **布局明细**：`hr_ui_layout_field.Label` 单语；保存/读取均不解析语言。  
3. **API**：`GetPool` / `GetEffective` 等直接返回单语 Label，未读 `Accept-Language` / `L.GetCulture()`。  
4. **管理端**：报表定义编辑器仅维护一个显示名，无多语言 Tab/行。  
5. **HR 页面壳**：`Web/src/views/hrms/**` 未接 vue-i18n（与字段 Label 分离；壳文案可后续迁 `i18n/.../hrms/`）。  
6. **无统一业务多语言解析组件**可同时服务代码类与字段 Label（建议落地时抽共用解析约定）。

---

## 5. 可借鉴做法（其他系统）

| 来源 | 做法 | 对本项目 |
|------|------|----------|
| SAP / 用友 / 金蝶类 ERP | 数据字典 + 语言表（主描述 + culture 译文） | ✅ **主推**，与 `code_mstr_lang` 同构 |
| Workday / 现代 SaaS | 元数据 API 按 locale 下发已解析文案 | ✅ 学接口形态：后端解析后返回 Label |
| 老 PowerBuilder HR | `language_key` → 语言包 | △ 可作字段池**默认名**补充，撑不住布局自定义改名 |
| 纯前端 i18n 产品 | 全进 locale 文件 | ❌ 布局可配置后难维护、与运营脱节 |
| JSON 多语言列 | 单列存 `{"en":"...","vi":"..."}` | △ 试点可，长期维护/查询弱于翻译表 |

---

## 6. 方案对比

### 方案 A：前端 i18n Key（`message.field.{module}.{fieldKey}`）

**做法**：字段池默认名写入 vue-i18n；布局若未改名则前端 `$t`，改名则用 DB Label。

**优点**：与现有 UI 包一致，无新表。  
**缺点**：业务人员无法在后台维护翻译；自定义 Label 仍单语；条目多时成本高。

**适用**：极少数固定、永不改名的系统列。  
**不推荐**作为报表定义主路径。

---

### 方案 B：库表多语言 — 翻译表（推荐）

**表结构建议**

```text
-- 字段池默认名（按模块可扩展；一期可与布局字段池同源维护）
hr_ui_field_def              — ModuleCode + FieldKey + Label（主语言）
hr_ui_field_def_lang         — 新增
  ModuleCode
  FieldKey
  Culture     (zh-CN / en / th / vi / zh-TW)
  Label
  PK (ModuleCode, FieldKey, Culture)

-- 布局自定义显示名（用户/管理员改过才需要多语言行）
hr_ui_layout_field           — 保持：FieldKey + Label（主语言覆盖）
hr_ui_layout_field_lang      — 新增
  LayoutFieldId
  Culture
  Label
  PK (LayoutFieldId, Culture)
```

**解析规则（优先级从高到低）**

1. 当前 culture 下，布局字段有 `layout_field_lang` → 用该译文  
2. 否则，若布局 `Label` 与字段池主语言默认名**不同**（视为自定义）→ 用布局主 `Label`（或再查自定义译；缺则回退该主 Label）  
3. 否则用字段池 `field_def_lang` 当前 culture  
4. 再否则回退字段池 / 布局主语言 `Label`  
5. 绝不因缺译返回空白或抛错

简化落地（一期可接受）：

- 未改名：只维护字段池多语言；布局 Label 存主语言默认值，读取时按 FieldKey 走字段池解析。  
- 已改名：写入 `layout_field_lang`（或至少保证主语言自定义名 + 各语言行）。

**管理端**：定义页「显示名」改为「主语言 + 各语言 Tab/行」，交互对齐人事代码多语言维护。

**优点**：后台可配、扩语言只加 culture 行、与代码类方案一致。  
**缺点**：需改表、CRUD、Pool/Effective 接口与编辑器。

---

### 方案 C：JSON 多语言列（折中）

在 `hr_ui_layout_field`（及字段池来源）增加 `label_i18n`（如 `{"en":"Emp No","vi":"..."}`），运行时按 culture 取 key。

**优点**：无新表。  
**缺点**：查询/索引弱、后台编辑体验差，长期不如方案 B。

**适用**：小范围试点验证解析链路；正式推广仍建议迁方案 B。

---

## 7. 推荐落地路径

```text
用户切换语言
  → 前端 globalI18n + Accept-Language
  → 后端 SetCulture
  → 展示分流：
       UI/菜单              → vue-i18n / Lang.json
       系统字典(固定项)     → message.dictType.*（见代码类分析）
       人事代码(可维护)     → code_mstr_lang
       字段 Label(报表定义) → field_def_lang + layout_field_lang（方案 B）
```

### 7.1 实施顺序建议

| 阶段 | 内容 | 方案 |
|------|------|------|
| P1 | 字段池：建默认名翻译（表或种子）；`GetPool` / `GetEffective` 按 culture 解析 Label | B |
| P2 | 布局自定义 Label：`layout_field_lang` + 定义页多语言录入；改名后各语言可配 | B |
| P3 | 与代码类共用「按 culture 取文案、缺省回退」工具方法，避免两套解析逻辑 | 共用 |
| P4 | HR 页面壳文案逐步迁入 `Web/src/i18n/.../hrms/`（与字段 Label 分离） | UI 包 |

### 7.2 接口约定

- **优先改接口返回的 Label**，前端列/表单继续绑 `label`，无需按语言硬编码映射。  
- 配置保存仍提交 FieldKey + 各语言 Label（或主 Label + i18n map）；关联永不提交「当前界面语言的中文」当键。  
- 错误信息若需展示字段名，使用解析后 Label 或 FieldKey，避免半截多语言。

### 7.3 不建议

- 仅靠前端硬编码 FieldKey → 各语言 Map（难维护、难扩展）  
- 把可配置布局 Label 全部塞进 `Lang.*.json` / vue-i18n（与运营配置脱节）  
- 用「中文 Label」做关联键或布局唯一键（语言一切换关联即断）  
- 与代码类采用两套完全不同的存储模型（增加认知与维护成本）

---

## 8. 验收要点

- [ ] 切换到 `en` / `th` / `vi` / `zh-TW`：人员列表列标题、（表单消费后）表单项标签随语言切换  
- [ ] 字段池默认名：有译文用译文；缺译回退主语言，不空白、不报错  
- [ ] 布局自定义显示名：各语言可维护；未维护的语言回退合理（自定义主名或字段池译）  
- [ ] 已保存布局的 FieldKey 集合、顺序、宽度/占位不因语言切换改变  
- [ ] 定义页保存后再切换语言，生效布局显示正确  
- [ ] 与人事代码下拉同时切换语言时，**码值不变、显示名变**（两端约定一致）

---

## 9. 关键文件索引

| 角色 | 路径 |
|------|------|
| 布局方案（能力） | `doc/分析文档/通用表单设计/01-通用表单设计.md` |
| 代码类多语言（可对齐） | `doc/分析文档/代码类设置多语言分析/01-multilang-analysis.md` |
| 布局实体 | `Admin.NET/Admin.NET.Application/Entity/HrUiLayout.cs`、`HrUiLayoutField.cs` |
| 布局服务 | `Admin.NET/Admin.NET.Application/Service/Hr/Layout/HrUiLayoutService.cs` |
| 前端 API | `Web/src/api/hrms/hrUiLayout.ts` |
| 定义页 | `Web/src/views/hrms/reportDef/` |
| 列表消费 | `Web/src/views/hrms/emp/index.vue` |
| 表结构 / 种子 | `doc/sql/hr-layout/` |
| 语言切换 UI | `Web/src/layout/navBars/topBar/user.vue` |
| Accept-Language | `Web/src/utils/axios-utils.ts` |
| 后端 SetCulture | `Admin.NET/Admin.NET.Core/Service/Common/SysCommonService.cs` |
| 前端 i18n | `Web/src/i18n/index.ts` |

---

## 10. 后续实现备注

- 本分析不包含具体 SQL/API 实现稿；落地时另开实施任务与迁移脚本。  
- 布局能力以 `通用表单设计/01-通用表单设计.md` 为准；字段 Label 多语言仍在本目录跟踪。  
- 若字段池长期以代码白名单维护、暂不建 `hr_ui_field_def` 物理表，P1 可用「种子翻译表 / 资源表按 FieldKey+Culture」等价实现，解析规则不变。  
- 实现时与代码类 P1（`code_mstr_lang`）可并行，共享 culture 读取与回退工具即可。
