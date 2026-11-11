# 高级查询通用控件

| 文档 | 说明 |
|------|------|
| [01-设计方案.md](./01-设计方案.md) | 目标、契约、字段失效、方案分享、跨表 |
| [02-实现计划.md](./02-实现计划.md) | 分任务落地清单 |
| [03-新模块接入清单.md](./03-新模块接入清单.md) | 新列表页前后端接入步骤 |

项目规则（强制）：`.cursor/rules/adv-query-required.mdc`  
代码入口：`Web/src/components/advQuery/index.ts`（`AdvQueryDrawer` + `useAdvQuery`）

首接业务：人员资料管理（`hrEmp`）。
