# 代码类设置 — 多语言显示方案分析

> 日期：2026-10-13  
> 范围：人事代码（`code_mstr`）、系统字典（`SysDictData`）及与语言切换联动的显示层  
> 目标：切换语言环境时，代码类设置项的**显示文本**同步切换；业务存值（Value/Code）不变

---

## 1. 结论

当前系统**切换语言只影响 UI 文案**（vue-i18n / `Lang.*.json`）。  
**人事代码 `code_mstr`、系统字典 `SysDictData` 的显示名都是单语字段**，切换语言后下拉/列表文字不会变。

要让「代码类设置」随语言切换，必须为业务数据增加**多语言解析层**（库表翻译或前端 i18n 覆盖），不能仅依赖现有 UI 资源包。

---

## 2. 现状

### 2.1 分层能力

| 层级 | 机制 | 切换语言后 |
|------|------|------------|
| 前端 UI | `vue-i18n` + `themeConfig.globalI18n`，请求带 `Accept-Language` | ✅ 页面文案变 |
| 后端消息/菜单 | `Lang.*.json` + `SetCulture` + `I18nConverter` | ✅ 固定中文 key 有翻译则变 |
| 系统字典 | DB 单字段 `Label`；前端有未启用的钩子 `message.dictType.{typeCode}_{value}` | ❌ 实际仍显示 DB 中文 |
| 人事代码 | `code_mstr.code_desc` 单字段；`GetCodeList` 直接拼 Label | ❌ 始终原语言 |

支持语言：`zh-CN` / `zh-TW` / `en` / `th` / `vi`（见 `App.json` LocalizationSettings、`Web/src/i18n` languageList）。

### 2.2 语言切换链路（已有）

1. 用户选择语言 → 写入 `themeConfig.globalI18n` 并 Local 持久化  
2. `locale.value = lang`（vue-i18n）  
3. 刷新当前页签  
4. `POST /api/sysCommon/setCulture?culture=...`  
5. 后续 API 请求头携带 `Accept-Language`（`axios-utils`）

入口：`Web/src/layout/navBars/topBar/user.vue` → `onLanguageChange`。

### 2.3 人事代码（「人事代码设置」）

| 项 | 说明 |
|----|------|
| 表 | `ehr.dbo.code_mstr` |
| 字段 | `code_field`（类别）、`code_value`（码）、`code_desc`（描述，单语）、`code_sort` |
| 实体 | `Admin.NET.Application/Entity/CodeMstr.cs` |
| 下拉 | `HrOrgService.GetCodeList`：`Label = code_value + " " + code_desc` |
| 菜单 | 「人事代码设置」`/hr/code`（种子存在，管理页尚未齐备） |
| 多语言列/翻译表 | **无** |

种子与表定义参考：`doc/sql/hr-org/20261010_hr_org_lookup_tables.sql`。

### 2.4 系统字典

| 项 | 说明 |
|----|------|
| 类型 | `SysDictType`：`Name` + `Code` |
| 项 | `SysDictData`：单字段 `Label` + `Value` |
| 前端覆盖钩子 | `userInfo.ts` → `setDictLangMessageAsync`，key = `message.dictType.{typeCode}_{value}` |
| 实际翻译包 | `Web/src/i18n` 下**几乎无** `dictType` 词条 → 始终回退 DB `label` |
| 缓存 | 登录后装入 `dictList`；语言切换后若未重载，仍可能显示旧 Label |

### 2.5 已有可复用模式（业务数据）

| 模式 | 位置 | 可否复用 |
|------|------|----------|
| UI 资源包 | vue-i18n / `Lang.*.json` | 仅 UI |
| `I18nConverter` + 中文 key | `SysMenu.Title` | 适合固定标题，不适合运营可编辑目录 |
| 字典前端 key 覆盖 | `message.dictType.*` | **适合固定系统字典**；需补词条 + 切换后重载 |
| `NameEn` 多列 | 仅个别插件 DTO | **非**项目通用模式 |
| 翻译表 / JSON 多语言列 | Application HR 实体中未发现 | 需新建 |

---

## 3. 差距（Gap）

1. **`code_mstr`**：仅一列 `code_desc`；`GetCodeList` 不读 culture → 语言切换无效。  
2. **人事代码设置**管理 UI 未齐备 → 尚无多语言维护入口。  
3. **SysDict**：DB 单语；前端覆盖模式存在但未使用；切换语言后可能需重载 `setDictList()`。  
4. **菜单**「人事代码设置」需同时出现在前端 `menu/*.ts` 与后端 `Lang.*.json`，否则菜单标题仍可能中文。  
5. **无统一的业务多语言模型**可直接套用。

---

## 4. 方案对比

### 方案 A：前端 i18n 包覆盖（适合系统字典、固定枚举）

**做法**

- 在 `Web/src/i18n` 增加 `message.dictType.{typeCode}_{value}`  
- 沿用 `setDictLangMessageAsync`  
- 语言切换后**重新执行** `setDictList()`（或依赖整页刷新）

**优点**：改动小，与现有字典 store 一致。  
**缺点**：业务人员无法在后台维护翻译；条目多时成本高。

**适用**：系统级、条目少、变更少的字典。

---

### 方案 B：库表多语言 — 翻译表（推荐用于人事代码）

**表结构建议**

```text
code_mstr              — 保持不变：code_field / code_value / code_desc（默认/主语言）
code_mstr_lang         — 新增
  code_field
  code_value
  culture     (zh-CN / en / th / vi / zh-TW)
  code_desc
  PK (code_field, code_value, culture)
```

**解析规则**

1. 读当前 culture（`Accept-Language` / `L.GetCulture()`）  
2. 有对应翻译 → 用翻译  
3. 否则回退 `code_mstr.code_desc`（主语言，建议固定为 zh-CN）  
4. `GetCodeList`（及列表展示）的 `Label` 使用解析后的描述

**管理端**：代码设置页按「主描述 + 各语言 Tab/行」维护。

**优点**：后台可配、扩展语言只需加 culture 行。  
**缺点**：需改表、CRUD、下拉/列表接口。

---

### 方案 C：JSON 多语言列（折中）

在 `code_mstr` 增加 `code_desc_i18n`（如 `{"en":"Plant","vi":"..."}`），运行时按 culture 取 key。

**优点**：无新表。  
**缺点**：查询/索引弱、后台编辑体验差，长期不如方案 B。

---

## 5. 推荐落地路径

```text
用户切换语言
  → 前端 globalI18n + Accept-Language
  → 后端 SetCulture
  → 展示分流：
       UI/菜单          → vue-i18n / Lang.json
       系统字典(固定项) → message.dictType.* 覆盖 + 重载 dictList
       人事代码(可维护) → code_mstr_lang 按 culture 解析 Label
```

### 5.1 实施顺序建议

| 阶段 | 内容 | 方案 |
|------|------|------|
| P1 | 人事代码：建 `code_mstr_lang`；`GetCodeList` 按 culture 解析；设置页支持多语言录入 | B |
| P2 | 系统字典固定项：补 `message.dictType.*`；切换语言后重载字典 | A |
| P3 | 菜单/页面壳：补「人事代码设置」等菜单多语言词条 | UI 包 |

### 5.2 统一约定（避免混用）

| 场景 | 约定 |
|------|------|
| 存库、关联、权限、报表过滤 | 一律使用 **Value / Code** |
| 界面展示 | 一律使用 **当前语言 Label** |
| 缺翻译 | 回退主语言，不抛错、不空白 |
| 语言切换 | 已选中的 value **不变**，只变显示文字 |

### 5.3 不建议

- 仅靠拉长 `code_desc` 或前端硬编码映射（难维护、难扩展）  
- 把可维护业务码全部塞进 `Lang.*.json` / vue-i18n（与运营配置脱节）  
- 用「中文 Label」做关联键（语言一切换关联即断）

---

## 6. 验收要点

- [ ] 切换到 `en` / `th` / `vi` / `zh-TW`：人事代码下拉与列表描述切换  
- [ ] 缺某语言翻译：显示主语言 `code_desc`，不空白、不报错  
- [ ] 已选中的 `code_value` 不因语言切换而改变  
- [ ] 系统字典（若采用方案 A）：切换语言后 Label 更新（含 store 重载）  
- [ ] 菜单「人事代码设置」标题随语言切换  

---

## 7. 关键文件索引

| 角色 | 路径 |
|------|------|
| 人事代码实体 | `Admin.NET/Admin.NET.Application/Entity/CodeMstr.cs` |
| 人事代码下拉 | `Admin.NET/Admin.NET.Application/Service/Hr/Org/HrOrgService.cs` → `GetCodeList` |
| 表种子 | `doc/sql/hr-org/20261010_hr_org_lookup_tables.sql` |
| 菜单种子 | `doc/sql/seed/20261012_hrCode_seed_menu.sql` |
| 字典实体 | `Admin.NET/Admin.NET.Core/Entity/SysDictData.cs` |
| 字典前端 i18n 钩子 | `Web/src/stores/userInfo.ts` → `setDictLangMessageAsync` |
| 语言切换 UI | `Web/src/layout/navBars/topBar/user.vue` |
| Accept-Language | `Web/src/utils/axios-utils.ts` |
| 后端 SetCulture | `Admin.NET/Admin.NET.Core/Service/Common/SysCommonService.cs` |
| 后端 Lang 资源 | `Admin.NET/Admin.NET.Core/Resources/Lang.{zh-CN,zh-TW,en,th,vi}.json` |
| 前端 i18n 入口 | `Web/src/i18n/index.ts` |

---

## 8. 后续实现备注

- 实现时优先改 **接口返回的 Label**，前端下拉组件无需按语言硬编码。  
- 若同时需要「系统字典后台可编辑多语言」，再评估为 `SysDictData` 增加同类翻译表（勿与方案 A 混用于同一批可运营条目）。  
- 本分析不包含具体 SQL/API 实现稿；落地时另开实施任务与迁移脚本。
