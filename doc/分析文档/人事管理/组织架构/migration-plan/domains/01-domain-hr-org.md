# Domain Plan：HrOrg（组织架构管理）

## 0. 实现立场

本模块是 Admin.NET 上的**人事组织功能**，按本章业务规则编写。

- `../specs/` 只用于理解字段与规则来源，**不得**把「对照旧窗 / 迁移」写进服务、DTO、前端、菜单。  
- 禁止迁移型命名与接口（详见 `00-roadmap.md`「实现原则」「禁止清单」）。

## 1. 背景（分析备忘，非产品文案）

| 项 | 说明 |
|----|------|
| 业务参考（只读） | `../specs/yuenan_w_rs_cc_mstr_reverse_spec.md` |
| 仓库旧代码 | `HrOrgService` 若仍双写 SysOrg — **整模块按本章重写** |

## 2. Target State

### 2.1 架构原则

1. 表名：`dbo.cc_mstr`、`dbo.h_emp_mstr`。  
2. 字段一期不加列（含不加 `OrgId`）。  
3. 主库 `ehr` DDL 建表。  
4. 停用：`cc_enable=1` + `cc_dele_date` / `cc_dele_remark`。  
5. 层级：`cc_level`（每层 2 位）+ `ParentID` / `cc_code_parent`。  
6. **切断 SysOrg**。  
7. **不做**「人员部门归属」批量改部门。  
8. **按新系统逻辑编写，无老系统迁移痕迹**。

```
[Vue /hrms/org]
        │
        ▼
 HrOrgService ──► ehr.dbo.cc_mstr
        │              ▲
        │              │ cc_code
        └──────── ehr.dbo.h_emp_mstr
        │
        ✕ SysOrg / 迁移型 API
```

### 2.2 表清单

| 表名 | 说明 |
|------|------|
| `cc_mstr` | 部门主档 |
| `h_emp_mstr` | 人员资料；部门字段 `cc_code` |

DDL：  
- `../../../sql/hr-org/20261010_hr_org_greenfield_tables.sql`  
- `../../../sql/hr-org/20261010_h_emp_mstr_org_dep.sql`  

### 2.3 `cc_mstr` 字段（32 列）

`cc_code`, `cc_code_parent`, `cc_dept`, `cc_desc`, `cc_level`, `cc_group`, `cc_indet`, `cc_sub`, `cc_site`, `cc_state`, `cc_cost`, `cc_wage_rate`, `cc_stat_basic`, `cc_enable`, `cc_dele_date`, `cc_count_group`, `cc_user`, `cc_date`, `cc_kqgroup`, `cc_woca`, `cc_bz_unit`, `cc_dele_remark`, `cc_desc_en`, `cc_level_censor`, `cc_xz_group`, `cc_create_date`, `machine_floor`, `kq_new_sysuser`, `kq_new_sysdate`, `ID`, `ParentID`, `cc_code2`。

不建：`OrgId` 及原库没有的 `cc_kq_*` 扩展列。

### 2.3.1 `h_emp_mstr`（组织依赖）

- 部门字段 **`cc_code`** = `cc_mstr.cc_code`（逻辑关联，一期不建 FK）。  
- 删除部门：下属仍有在职/停职人员则拒绝。  
- 合并：将非离职人员（`emp_state <> 3`）的 `cc_code` 改到目标部门。  
- 不做批量「人员改部门」页。  
- 组织脚本只建依赖列；员工模块补全其余列，**不得改名 `cc_code`**。

### 2.4 异动备注

一期不另建变更表；合并/调动说明写入 `cc_dele_remark`。

### 2.5 API（本系统能力，无迁移接口）

| API | 行为 |
|-----|------|
| GetList / GetTree | 按 `cc_level` / `ParentID`；默认可只查 `cc_enable=0` |
| Add / Update | 写 `cc_mstr`；生成 `cc_code`/`cc_level`；继承上级厂别/工时等 |
| Delete | 软停用：占用校验 → 子树 `cc_enable=1` + `cc_dele_*` |
| Merge | 无启用子部门；人员迁码；源部门停用并写备注 |
| Transfer | 禁止调入子孙；重算子树 `cc_level` |

**禁止提供**：`SyncFrom*`、`ImportFrom*`、`Migrate*`、任何 SysOrg CRUD。

权限：菜单/按钮 API 即可。

### 2.6 `cc_level`

每层 2 位；一期同级数字 `01..99`。

### 2.7 前端

- 树 / 列表 / 编辑 / 合并 / 调动。  
- 不调用 `sysOrg`；无「人员部门归属」页签；文案不提旧系统。

## 3. 建设步骤

| # | 步骤 | 产出 |
|---|------|------|
| 1 | 建表 | SQL |
| 2 | 新建/重写实体（无 Ignore 补丁叙事、无 OrgId） | Entity |
| 3 | 全新编写 `HrOrgService` | Service/Dto |
| 4 | 前端对接 | org 页 |
| 5 | 菜单种子 | SysMenu |
| 6 | 评审禁止清单 | checklist |

若需历史数据灌入：仅运维脚本，**不进**产品功能列表。

## 4. 验收

| 用例 | 期望 |
|------|------|
| 树与 CRUD | 只碰 `cc_mstr` |
| 停用 | `cc_enable=1`，有人员则拒绝 |
| 合并 / 调动 | 人员与层级正确 |
| 命名与 API | 无 Legacy / SyncFrom / ImportFrom / SysOrg |
| 注释与 UI | 无「兼容旧系统 / 从 PB 迁移」 |

## 5. Success Metrics

- 主档仅为 `ehr.dbo.cc_mstr`，人员挂 `h_emp_mstr.cc_code`。  
- 与 `SysOrg` 零耦合。  
- 代码与产品面读起来是**本系统模块**，不是迁移工程残留。
