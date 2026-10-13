# 组织架构管理 — 分析文档索引

根路径：`doc/分析文档/组织架构/`

## 实施铁律（2026-10-10 确认）

**新系统按 Admin.NET / 本模块自身业务逻辑编写，代码与产品面不得留「老系统迁到新系统」痕迹。**

| 允许 | 禁止 |
|------|------|
| 分析目录保留逆向规格（只读参考） | 产品 API / 页面 / 注释写「兼容 PB / 从旧库同步 / Legacy」 |
| 表名字段沿用业务字典（`cc_mstr`、`cc_code`…） | `ImportFromLegacy`、`SyncFromCcMstr`、`Legacy*`、Ensure 式补丁列 |
| 业务规则用新系统语言表述（软删、合并、调动） | 运行时对照外库、双写、迁移进度状态机 |
| 初始化数据用一次性运维脚本（库外） | 把「迁旧数」做成正式菜单/接口能力 |

实现入口以 `migration-plan/` 为准；`specs/` 仅需求溯源，**不是**实现说明书。

## 与 SysOrg 的关系（已确认）

**本方案与框架表 `SysOrg` 没有任何关系。**

| 项 | 约定 |
|----|------|
| 部门主档 | `ehr.dbo.cc_mstr` |
| 人员资料 | `ehr.dbo.h_emp_mstr`（部门字段 **`cc_code`**） |
| 树结构 | `cc_level` / `ParentID` / `cc_code_parent` |
| SysOrg | **不读写、不同步、不映射、不加 OrgId** |
| 权限 | 一期：菜单/按钮 API；二期数据范围见管辖设计（不走 SysOrg） |

**排除范围：** 「人员部门归属」批量改部门（原 tab2）本模块不做。

## 目录结构

```
组织架构/
├── README.md                          # 本索引
├── migration-plan/                    # 新系统建设方案（实施依据）
│   ├── 00-roadmap.md
│   ├── CHECKLIST.md                   # 建设验收清单
│   ├── domains/01-domain-hr-org.md
│   ├── design/
│   │   └── 01-user-cc-scope.md        # 用户管辖部门（二期）
│   └── research/
├── specs/                             # 逆向（只读参考，非实现契约）
│   ├── yuenan_w_rs_cc_mstr_reverse_spec.md   # 主业务参考
│   ├── hr12_org_cc_mstr_reverse_spec.md      # 归档对照
│   └── hr12_org_cc_mstr_gap_analysis.md      # 归档缺口（含旧 SysOrg 史实）
└── spec-extract/                      # 源码摘录（只读）
```

## 相关但不在本目录

| 路径 | 说明 |
|------|------|
| `doc/sql/hr-org/20261010_hr_org_greenfield_tables.sql` | `cc_mstr` 建表 |
| `doc/sql/hr-org/20261010_h_emp_mstr_org_dep.sql` | `h_emp_mstr` 组织依赖列 |
| `doc/sql/hr-org/20261010_hr_org_lookup_tables.sql` | `code_mstr` / `h_cost` / `h_shift_set` |
| `doc/sql/seed/20261010_hrOrg_seed_menu.sql` | 菜单与按钮 |
| `doc/sql/seed/20261010_hrOrg_seed_roleMenu.sql` | `sys_admin` 角色授权 |
| `Admin.NET/.../Service/Hr/Org/` | `HrOrgService`（已按本方案编写） |
| `Web/src/views/hrms/org/` | 组织架构前端页 |

## 阅读顺序

1. 本 README「实施铁律」  
2. `migration-plan/00-roadmap.md`  
3. `migration-plan/domains/01-domain-hr-org.md`  
4. 二期管辖：`migration-plan/design/01-user-cc-scope.md`  
5. 实施完成后勾 `migration-plan/CHECKLIST.md`  
6. 需要溯源业务细节时再查 `specs/`（勿把对照史实写进代码）
