# 人员资料管理 — 分析文档索引

根路径：`doc/分析文档/人员资料/`

## 约定

| 项 | 约定 |
|----|------|
| 菜单位置 | 人事管理 → 人员资料管理 |
| 主表 | `ehr.dbo.h_emp_mstr` |
| 部门字段 | **`cc_code`**（= `cc_mstr.cc_code`，禁止改名） |
| 与组织模块 | 共用同一实体/表；组织占用校验与合并仍写本表 |
| 一期范围 | 截图「基本信息 + 任职信息」；其余列不进 DTO |
| 字典下拉 | 职务/岗位/性别/婚姻/户籍/人员类别/薪酬类别一期只加字段，数据源后续补 |
| 入司前工龄 | `emp_longmon`（月） |

## 实现入口

| 层 | 路径 |
|----|------|
| 实体 | `Admin.NET.Application/Entity/HrEmployee.cs` |
| 服务 | `Admin.NET.Application/Service/Hr/Emp/HrEmpService.cs` |
| 前端 | `Web/src/views/hrms/emp/`、`Web/src/api/hrms/hrEmp.ts` |
| 菜单种子 | `doc/sql/seed/20261012_hrEmp_seed_menu.sql` |
| 角色授权 | `doc/sql/seed/20261012_hrEmp_seed_roleMenu.sql` |

## 目录

| 文档 | 说明 |
|------|------|
| `field-mapping-mvp.md` | 一期界面字段 ↔ `h_emp_mstr` 列对照（已定稿项） |

相关：自定义列表/表单布局方案见 [`../自定义报表/`](../自定义报表/README.md)。
