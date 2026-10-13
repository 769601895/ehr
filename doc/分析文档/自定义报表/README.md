# 自定义报表 / 布局 — 分析文档索引

根路径：`doc/分析文档/自定义报表/`

## 目标

让业务界面（优先人事人员资料）支持**自定义显示字段**与**占位长度/列宽**，交互对齐旧系统「报表定义」：左侧配置字段、右侧预览，可保存方案。

## 范围分层

| 能力 | 场景 | 一期 | 说明 |
|------|------|------|------|
| **A. 列表列定义** | 人员列表 / 报表表头 | ✅ 优先 | 菜单「人事资料列表定义」 |
| **B. 表单布局定义** | 人员新增/编辑弹窗 | 二期 | 菜单「人事资料表单定义」；字段显隐、顺序、占位格数(1–4) |
| **C. 通用报表引擎** | 多表自由查、导出 | 后续 | 不与人员模块一期绑死 |

## 菜单结构（已落地）

```
人事管理
└─ 人事资料定义（Dir，Id=…0241）
   ├─ 人事资料列表定义（Menu …0244 → /hrms/reportDef/listDef）  ← 一期
   └─ 人事资料表单定义（Menu …0245 → /hrms/reportDef/formDef）  ← 二期
```

| 项 | 路径 |
|----|------|
| 子菜单种子 | `doc/sql/seed/20261013_hrReportDef_children_menu.sql` |
| 角色授权 | `doc/sql/seed/20261013_hrReportDef_children_roleMenu.sql` |
| 表结构 | `doc/sql/hr-layout/20261013_hr_ui_layout_tables.sql` |
| 默认布局种子 | `doc/sql/hr-layout/20261013_hr_ui_layout_seed_default.sql`（list 12 列 / form 33 字段） |
| 定义页 | `Web/src/views/hrms/reportDef/listDef.vue`、`formDef.vue`、`component/layoutEditor.vue` |
| API | `Web/src/api/hrms/hrUiLayout.ts` → `HrUiLayoutService` |
| 列表消费 | `Web/src/views/hrms/emp/index.vue`（`getEffective` scene=list） |

## 与现有模块关系

| 模块 | 关系 |
|------|------|
| 菜单 `人事资料定义` | 目录入口（OrderNo=95）；原单页按钮 0242/0243 已软删，权限改挂子菜单 |
| 人员资料 `hrms/emp` | 首个落地试点（列表 A 已接生效布局；表单 B 定义页已就绪，编辑页消费待二期） |
| 字段对照 | `../人员资料/field-mapping-mvp.md`（字段池来源） |
| 系统代码生成列配置 | 仅作参考，**不复用**其表结构做人事布局 |
| `reportCustomQuery` | 通用查询原型可参考交互，人事布局独立存储 |

## 目录

| 文档 | 说明 |
|------|------|
| `01-hr-layout-plan.md` | 人员模块可落地方案（配置模型、表结构、API、前端改造、分期验收） |

## 实施铁律

- 配置只控制**展示与布局**；后端校验、白名单、业务主键规则不变。
- 字段名必须落在模块字段池（白名单）内，禁止任意拼 SQL 列名。
- 默认方案可运维种子；用户个人方案可覆盖，删除个人方案后回退默认。
