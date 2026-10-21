# 代码类设置多语言分析 — 文档索引

根路径：`doc/分析文档/代码类设置多语言分析/`

## 背景

系统为多语言环境（`zh-CN` / `zh-TW` / `en` / `th` / `vi`）。切换语言后，UI 文案可切换，但**人事代码、系统字典等业务显示名**目前为单语字段，不会随语言变化。本文档给出现状、差距与推荐方案。

## 目录

| 文档 | 说明 |
|------|------|
| [01-multilang-analysis.md](./01-multilang-analysis.md) | 现状、方案对比、推荐落地路径与验收要点 |

## 相关代码入口（速查）

| 类型 | 路径 |
|------|------|
| 人事代码实体 | `Admin.NET/Admin.NET.Application/Entity/CodeMstr.cs` |
| 人事代码下拉 | `HrOrgService.GetCodeList` |
| 系统字典 | `SysDictType` / `SysDictData`；前端 `Web/src/stores/userInfo.ts` |
| 前端语言切换 | `Web/src/layout/navBars/topBar/user.vue` → `SetCulture` + `Accept-Language` |
| 后端资源 | `Admin.NET/Admin.NET.Core/Resources/Lang.*.json` |
| 菜单种子 | `doc/sql/seed/20261012_hrCode_seed_menu.sql`（人事代码设置） |
