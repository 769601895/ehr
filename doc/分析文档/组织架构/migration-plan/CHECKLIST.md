# 建设验收清单（组织架构）

实施完成后逐项勾选。依据：`00-roadmap.md`、`domains/01-domain-hr-org.md`。

## 数据

- [x] `ehr.dbo.cc_mstr` 已按 DDL 创建，列为约定 32 列，无 `OrgId`
- [x] `ehr.dbo.h_emp_mstr` 已具备组织依赖列，部门字段名为 **`cc_code`**
- [x] 不存在对本模块使用表名 `employee` 的依赖

## 后端

- [x] `HrOrgService`（或等价服务）**不**注入 / 引用 `SysOrg`、`SysOrgService`
- [x] 提供：树/列表、增改、软停用、合并、调动
- [x] **不**提供：`SyncFrom*`、`ImportFrom*`、`Migrate*` 等迁移型 API
- [x] 删除 = `cc_enable=1` + `cc_dele_*`（含子树规则）；有在职/停职人员则拒绝
- [x] 合并：更新 `h_emp_mstr.cc_code`（`emp_state <> 3`），源部门停用
- [x] 实体/DTO/注释无 `Legacy`、`FromPb`、`半迁移`、`兼容旧系统` 等字样

## 前端

- [x] 组织页数据源仅为本模块 API，不调 `sysOrg`（`Web/src/views/hrms/org`）
- [x] 无「人员部门归属」批量页签
- [x] 界面文案无「从旧系统导入 / 同步遗留」类表述
- [x] 系统管理→机构管理已恢复为 `SysOrgApi`，与人事组织解耦

## 权限

- [x] 菜单与按钮权限已挂（`SysMenu` 1300000000211–0216）；未用 `SysRoleOrg` 绑人事树数据范围
- [x] `sys_admin` 角色已授权人事目录 + 组织架构菜单/按钮（`20261010_hrOrg_seed_roleMenu.sql`）；重新登录后生效

## 文档边界

- [x] `specs/` 未当作实现说明书抄进代码命名
- [x] 若有历史数据灌入，仅为运维脚本，未进正式菜单

## P5 禁止项评审（2026-10-10）

- [x] `Service/Hr`、`views/hrms/org`、`api/hrms/hrOrg.ts`、`Entity/CcMstr` 无 `SysOrg`/`OrgId`/`SyncFrom*`/`ImportFrom*`/`Legacy`/`半迁移`
- [x] 接口冒烟：`GET /api/hrOrg/list`、`GET /api/hrOrg/codeList`、`POST /api/hrOrg/add` 均 200（样例子部门 `000002`）
