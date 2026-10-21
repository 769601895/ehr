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
| 补充信息 | 家庭成员 / 教育经历 / 工作履历 / 职称情况（每人多行） |
| 字典下拉 | 职务/岗位/性别/婚姻/户籍/人员类别/薪酬类别一期只加字段，数据源后续补 |
| 入司前工龄 | `emp_longmon`（月） |

## 实现入口

| 层 | 路径 |
|----|------|
| 实体 | `Admin.NET.Application/Entity/HrEmployee.cs`、`HrEmpChild.cs` |
| 服务 | `HrEmpService.cs`、`HrEmpChildService.cs` |
| 前端 | `Web/src/views/hrms/emp/`（`editEmp` 分节 + `empChildGrid`） |
| API | `Web/src/api/hrms/hrEmp.ts`、`hrEmpChild.ts` |
| 建表 | `doc/sql/2026-10-19/02-人员补充信息四表.sql` |
| 菜单权限 | `doc/sql/2026-10-19/03-人员补充信息菜单权限.sql` |
| 菜单种子 | `doc/sql/seed/20261012_hrEmp_seed_menu.sql` |
| 角色授权 | `doc/sql/seed/20261012_hrEmp_seed_roleMenu.sql` |

## 编辑页结构

顶栏身份（照片/工号/姓名/部门/状态）+ 左侧目录（人员资料、四张补充表）+ 右侧当前节。人员资料节内基本信息与任职信息同页，仍走表单布局自定义（分组/占格/显隐）。主档点「保存主档」；补充表按行增删改、拖拽排序。

## 目录

| 文档 | 说明 |
|------|------|
| `field-mapping-mvp.md` | 一期界面字段 ↔ `h_emp_mstr` 列对照（已定稿项） |
| `编辑人员界面原型.html` | 分节交互原型 |

相关：自定义列表/表单与字段池方案见 [`../通用表单设计/01-通用表单设计.md`](../通用表单设计/01-通用表单设计.md)。
