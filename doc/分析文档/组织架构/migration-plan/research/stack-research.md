# Stack Research — 人事组织（Admin.NET）

## 目标栈

| 项 | 约定 |
|----|------|
| 框架 | Admin.NET（Furion + SqlSugar） |
| 主库 | `ehr`（SqlServer） |
| 部门主档 | `ehr.dbo.cc_mstr` |
| 人员资料 | `ehr.dbo.h_emp_mstr`，部门字段 **`cc_code`** |
| 树模型 | `cc_level` / `ParentID` / `cc_code_parent`（不用 `SysOrg.Pid`） |
| 服务 | 全新编写 `HrOrgService`；不注入 `SysOrgService` |
| 菜单权限 | `SysMenu` + API 按钮权限 |
| DDL | `doc/sql/hr-org/*.sql` |

> 框架自带的 `SysOrg` 供系统机构等使用；**本模块不使用、不关联**。

## 实现立场

- 按本系统业务规则编写（见 `00-roadmap.md` 实现原则）。  
- `specs/` 逆向材料仅作需求分析参考，**不**决定 API 命名与产品文案。  
- 禁止迁移型接口与 Legacy 命名。

## 仓库中待替换实现（备忘）

| 项 | 说明 |
|----|------|
| 现状 | `HrOrgService` 曾双写 SysOrg |
| 处置 | **整模块按本方案重写**，不在旧路径上打补丁 |

## 结论

1. 主库建 `cc_mstr` + 组织依赖的 `h_emp_mstr`。  
2. 与 `SysOrg` 切断。  
3. 代码与产品面无「老系统迁到新系统」痕迹。
