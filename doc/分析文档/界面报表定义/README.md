# 界面报表定义 — 多语言分析文档索引

根路径：`doc/分析文档/界面报表定义/`

## 背景

系统为多语言环境（`zh-CN` / `zh-TW` / `en` / `th` / `vi`）。切换语言后，UI 壳文案可切换，但**界面/报表定义中的字段显示名（列标题、表单项 Label）**目前为单语字段，不会随语言变化。

本文档给出界面字段显示多语言的现状、差距、可借鉴方案与推荐落地路径；与「代码类设置」多语言分析共用同一套业务显示约定。

## 目录

| 文档 | 说明 |
|------|------|
| [01-field-label-multilang-analysis.md](./01-field-label-multilang-analysis.md) | 界面字段 Label 多语言：现状、方案对比、推荐路径与验收 |

## 相关文档

| 文档 | 说明 |
|------|------|
| [../通用表单设计/01-通用表单设计.md](../通用表单设计/01-通用表单设计.md) | 布局、字段池、代码绑定与多语言的统一方案 |
| [../代码类设置多语言分析/01-multilang-analysis.md](../代码类设置多语言分析/01-multilang-analysis.md) | 人事代码 / 字典多语言（翻译表模式，可复用） |

## 相关代码入口（速查）

| 类型 | 路径 |
|------|------|
| 布局实体 | `Admin.NET/Admin.NET.Application/Entity/HrUiLayout.cs`、`HrUiLayoutField.cs` |
| 布局服务 / 字段池 | `HrUiLayoutService`（含 `GetPool`） |
| 前端 API | `Web/src/api/hrms/hrUiLayout.ts` |
| 定义页 | `Web/src/views/hrms/reportDef/` |
| 列表消费 | `Web/src/views/hrms/emp/index.vue` |
| 语言切换 | `Web/src/layout/navBars/topBar/user.vue` → `SetCulture` + `Accept-Language` |
| 前端 i18n | `Web/src/i18n/index.ts` |
| 后端 Lang | `Admin.NET/Admin.NET.Core/Resources/Lang.*.json` |
