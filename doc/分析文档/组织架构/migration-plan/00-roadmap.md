# Roadmap：组织架构管理（Admin.NET 全新建设）

## Direction

在 Admin.NET + Vue3 上**新建**人事组织模块。表结构沿用业务命名（`cc_mstr` / `h_emp_mstr`），行为按**本系统规则**实现。

分析目录中的逆向规格仅用于还原业务含义；**实现阶段不得出现「从旧系统迁移而来」的产品与代码痕迹。**

**User-confirmed constraints**：

- 主库 `ehr` DDL 建表；表名/字段与业务字典一致，一期不加列。  
- **与 `SysOrg` 零关联**。  
- **无**人员批量改部门页签。  
- **按新系统逻辑编写**（见下节）。

---

## 实现原则（硬性）

1. **产品叙事**：模块即「人事组织管理」，不是「旧组织迁移工具」。  
2. **代码叙事**：实体/服务/DTO/前端注释使用业务词（部门、层级、停用、合并、调动）；禁止 `Legacy` / `FromPb` / `SyncOld` / `MigrateFrom*` / `半迁移` 等命名与文案。  
3. **无迁移型 API**：不提供「从旧库同步 / 导入遗留部门」类接口；若运维需要灌数，用库外一次性脚本，不进正式菜单。  
4. **无兼容补丁**：禁止运行时 `ALTER`、双写、对照外库、Ensure 补列。  
5. **替换而非修补**：现有 `HrOrgService` 若含 SysOrg 双写，按本方案**重写**为目标态，不在旧双写上打补丁并保留旧路径。  
6. **规则表述**：验收与注释写「本系统：停用=`cc_enable=1`」，不写「对齐 PB / 兼容 yuenan」。

---

## Target Architecture

```
ehr.dbo.cc_mstr           # 部门主档
        │
        │  h_emp_mstr.cc_code = cc_mstr.cc_code
ehr.dbo.h_emp_mstr        # 人员资料（占用校验 / 合并调部门）
        ▲
HrOrgService  ←→  Web /hrms/org
        │
        ✕ SysOrg
```

---

## Phase Plan

| Phase | 内容 | 主要产出 |
|-------|------|----------|
| **P0** | 冻结规则（SysOrg 切断、无 tab2、无迁移痕迹） | 本目录文档 |
| **P1** | 建表 `cc_mstr` + 组织依赖 `h_emp_mstr` | `doc/sql/hr-org/*.sql` |
| **P2** | 全新编写后端（软删 / 合并 / 调动） | `Service/Hr/Org/*`、`Entity/*` |
| **P3** | 前端组织页（树 / 维护 / 异动） | `Web/src/views/hrms/org` |
| **P4** | 菜单与按钮权限 | seed |
| **P5** | 评审：无 SysOrg、无 Legacy/Sync 命名、无迁移型 API | checklist |
| **P6** | 二期（管辖等，仍不含批量人员改部门） | `design/01-user-cc-scope.md` + 增量代码 |

详细步骤：`domains/01-domain-hr-org.md`。  
管辖设计：`design/01-user-cc-scope.md`（表 `Usercc`；存稳定 `CcCode` 根；查询取当前 `cc_level` 前缀；结构/编码变不改写 `Usercc`）。

---

## 禁止清单（代码评审必查）

| 禁止 | 说明 |
|------|------|
| `SysOrg` / `SysOrgService` / `OrgId` | 本模块零关联 |
| `SyncFromCcMstr`、`ImportFromLegacy`、`Migrate*` | 迁移型能力 |
| 注释/UI「兼容旧系统」「从 PB 导入」 | 产品痕迹 |
| 表名 `employee` | 人员表用 `h_emp_mstr` |
| 物理删除部门行 | 使用 `cc_enable` 停用 |

---

## Deliverables Index

| 路径 | 说明 |
|------|------|
| `README.md` | 索引 + 实施铁律 |
| `migration-plan/00-roadmap.md` | 本文件 |
| `migration-plan/domains/01-domain-hr-org.md` | 域方案 |
| `migration-plan/research/*` | 研究备忘 |
| `specs/*` | **只读**逆向参考 |
| `doc/sql/hr-org/*.sql` | 建表脚本 |

---

## Next Action

1. 执行建表脚本。  
2. 按本系统规则全新编写实体与 `HrOrgService`（勿在旧双写上演进）。  
3. 前端只调用本模块 API。

（本目录为**方案**；`specs/` 不指导命名与 API 形态。）
