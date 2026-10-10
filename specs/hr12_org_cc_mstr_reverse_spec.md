# Reverse-Engineered Specification: 组织架构管理（hr12 ↔ w_rs_cc_mstr）

## Overview

从 `hr12` 提取「组织架构管理」功能规格，并对照新一代 HRMS 窗口 `w_rs_cc_mstr`（`src/hr_rs_basic.pbl`）以及已落地的 Web/Admin.NET 实现。

**关键映射（务必先读）：**

| 代际 | 工程 | 菜单名 | 主窗口 | 所在库 |
|------|------|--------|--------|--------|
| hr12（旧） | `hr12/` | 组织架构管理 | **`w_deptmanger`** | `deptmanger.pbl` / `.pbd` |
| 新 PB（yuenan/src） | `src/` | 部门维护类菜单 | **`w_rs_cc_mstr`** | `hr_rs_basic.pbl` |
| 新 Web | `Web` + `Admin.NET` | 人事管理 → 组织架构管理 | `/hrms/org/index` | `HrOrgService` |

> hr12 **没有** `w_rs_cc_mstr` 对象。用户所说「对应程序位置在 w_rs_cc_mstr」指的是 **src/新系统侧等价入口**，不是 hr12 源码里的窗口名。

核心数据表始终是 **`cc_mstr`**（部门主档），层级用 **`cc_level`（每层 2 位字符）** 表达。

---

## Architecture Summary

### Technology Stack

| 层 | hr12 | src `w_rs_cc_mstr` | 新 Web |
|----|------|-------------------|--------|
| 客户端 | PowerBuilder（`hr.exe` / `hr.pbt`） | Appeon PB 22（`hrms`） | Vue3 + vxe-grid |
| 基类窗口 | `w_module_hr` | `w_pattern_hr` | — |
| 后端 | 直连 SQLCA / 嵌入 SQL | 直连 SQLCA | Admin.NET `HrOrgService` |
| 库表 | `cc_mstr` + `v_usercc` + `h_emp_mstr` | `cc_mstr` + employee | `SysOrg`（结构）+ `cc_mstr`（扩展，双写） |

### Module Structure（hr12）

```
hr12/
├── hr.pbt / HR.pbw          # Target；LibList 含 deptmanger.pbl
├── deptmanger.pbl/.pbd      # ★ 组织架构主库
│   ├── w_deptmanger         # 组织架构管理主窗
│   ├── w_companyset         # 集团/公司/事业部维护
│   ├── w_dept_select        # 部门选择（单选）
│   ├── w_dept_multipleslect # 部门多选
│   ├── uf_getnew_level      # 层级码递增（00..99..A0..ZZ）
│   ├── uf_updateparentid    # 回写 parentid / dept_parentlevel
│   ├── d_deptmanager        # 部门维护 DW
│   ├── d_companymanager     # 公司维护 DW
│   ├── d_department_group   # 部门变更/合并操作 DW
│   └── d_depthistroy 等
├── System.ini               # 使用统计：组织架构管理 → w_deptmanger
└── Language/                # 文案「组织机构设置」「部门管理」
```

### Module Structure（src / w_rs_cc_mstr）

```
src/hr_rs_basic.pbl
├── w_rs_cc_mstr             # 主窗（基类 w_pattern_hr）
├── d_rs_cc_mstr             # 部门维护
├── d_rs_cc_mstr_delds       # 删除前员工校验
├── d_rs_cc_changdw1/2       # 异动：合并/调动表单
├── d_rs_cc_detail           # 部门明细
├── d_rs_empcc_batcedit*     # 人员部门归属批量
└── w_rs_ccsele_pop 等
```

### Data Flow

```
[树节点选中 is_level]
        │
        ▼
  子部门列表检索
  cc_level LIKE is_level+'%' AND LEN(cc_level)=LEN(is_level)+2 AND cc_enable=0
        │
        ├── 新增：Max(cc_code)+1；Max 同级 cc_level → uf_getnew_level → 拼接
        ├── 删除：软删 cc_enable=1（含子树）；在职员工则拒绝
        ├── 变更/调动：重算子树 cc_level，写 parentid
        └── 合并：员工迁入新部门，旧部门软删
```

新 Web 在此基础上：**SysOrg 存树结构，cc_mstr 一对一扩展（OrgId 映射）**。

---

## 程序位置对照

### hr12 入口

| 项 | 证据 |
|----|------|
| 菜单功能名 | `System.ini`：`组织架构管理,w_deptmanger` |
| PBL | `hr12/deptmanger.pbl`（LibList 第 5 位） |
| 主窗 | `global type w_deptmanger from w_module_hr` |
| 左树 | `tv_cc`（`ue_depttreeselectchaged` → 刷新 dw_1） |
| 权限视图 | `v_usercc`（新增前校验管辖权限） |

### 等价：src `w_rs_cc_mstr`

| 项 | 证据 |
|----|------|
| PBL | `src/hr_rs_basic.pbl` |
| 主窗 | `global type w_rs_cc_mstr from w_pattern_hr` |
| 左树 | `uo_2::uf_tv_selechange` → `dw_1.pfc_retrieve` |
| 语言包 | `src/language/*/…` 段 `[w_rs_cc_mstr]` |

### 等价：新 Web（已实现）

| 项 | 路径 |
|----|------|
| 前端页 | `Web/src/views/hrms/org/index.vue` |
| 树/编辑/批量 | `orgTree.vue` / `editOrg.vue` / `batchMaintenance.vue` |
| API | `Admin.NET.../Service/Hr/Org/HrOrgService.cs`（`hrOrg`） |
| 实体 | `Entity/CcMstr.cs` |
| 菜单种子 | `doc/sql/seed/20261009_hrOrg_seed_orgMenu.sql` |

---

## 页签功能对照

| 能力 | hr12 `w_deptmanger` | src `w_rs_cc_mstr` | 新 Web `hrOrg` |
|------|---------------------|-------------------|----------------|
| 部门 CRUD | tab「部门维护」`d_deptmanager` | tab「部门维护」`d_rs_cc_mstr` | 树表 + Add/Update/Delete |
| 部门调动/变更 | tab「部门变更」`d_department_group` | tab「部门异动」→「调 动」`d_rs_cc_changdw*` | `TransferDept` |
| 部门合并 | 同页变更区（合并按钮逻辑） | tab「部门异动」→「合 并」 | `MergeDept` |
| 人员批量改部门 | （员工模块侧为主） | tab「人员部门归属」批量 | 部分能力在员工模块；组织页有批量维护成本/考勤 |
| 部门明细 | — | tab「部门明细」`d_rs_cc_detail` | 编辑表单字段展开 |
| 公司/集团层 | 独立窗 `w_companyset` | 同表不同层级维护 | 同一机构树 |
| 成本中心/考勤批改 | — | 新版页签扩展（`w_rs_cc_mstr_new` 注释见 Dto） | `BatchUpdateCostKq` / `BatchUpdateKqCode` |

语言包 `[w_rs_cc_mstr]` 可见控件文案：

- `tabpage_1` = 部门维护  
- `tabpage_3` = 部门异动  
- `tabpage_4` = 部门明细  
- `cb_1` = 批量，`cb_2` = 调动，`cb_4` = 合并  

---

## Observed Functional Requirements（EARS）

### 树与列表

**OBS-ORG-001**：When 用户在组织树上选中节点（`is_level`），the system shall 仅检索「直接下级」启用部门：  
`cc_level LIKE is_level+'%' AND LEN(cc_level)=LEN(is_level)+2 AND cc_enable=0`。  
（hr12：`dw_1::sbo_ue_refresh`；src：`retrieve(is_level, is_level+'zz', len(is_level)+2)`）

**OBS-ORG-002**：While 列表展示，the system shall 按 `cc_level` / `cc_sort` 排序。

### 新增

**OBS-ORG-003**：When 新增部门，the system shall：  
1）校验用户对当前 `is_level` 有管辖权限（hr12：`v_usercc`）；  
2）`cc_code = Max(cc_code)+1`（10 位零填充，空则 `0000000001`）；  
3）同级最大 `cc_level` 经 `uf_getnew_level` 得到后两位，拼到父级 `is_level` 后写入。

**OBS-ORG-004**：The system shall 用每层 2 位字符编码层级，序列为 `00..09..99` 后进入 `A0..ZZ`；溢出返回 `'err'`。

### 删除（软删除）

**OBS-ORG-005**：When 删除部门，the system shall **禁止硬删**；设置 `cc_enable=1`，并写入 `cc_dele_date` / 删除人 / `cc_dele_remark`（如「删除」「删除撤消」）。

**OBS-ORG-006**：When 部门（含子树）下仍有在职员工，the system shall 拒绝删除。  
（hr12：`h_emp_mstr.empstate=0`；src：`d_rs_cc_mstr_delds` 区分在职/停职）

**OBS-ORG-007**：When 存在启用中的下级部门，the system shall 提示是否同步软删整棵子树（`cc_level LIKE 当前level+'%'`）。

### 保存后副作用

**OBS-ORG-008**：When 组织有增改（`li_deptchange=1`），the system shall 可触发 `uf_ini_usercc_online` 刷新在线用户管辖部门，并可选重建组织树。

**OBS-ORG-009**：When 需要维护父子关系，the system shall 调用 `uf_updateparentid`：  
`dept_parentlevel = LEFT(cc_level, LEN-2)`，`parentid = 父级 cc_level 对应 id`。

### 调动

**OBS-ORG-010**：When 将部门 A 调往归属部门 B，the system shall：  
- 拒绝 A、B 为空或相同；  
- 在 B 下生成新的同级 `cc_level` 前缀；  
- 整棵子树 `cc_level` 替换前缀：`newPrefix + Mid(oldLevel, Len(oldPrefix)+1, …)`；  
- 写调动说明 / 历史（hr12 变更页）；src 侧有成功提示「部门调动成功」。

**OBS-ORG-011**：Where 层级深度受限，the system shall 拒绝过深调动（新 Web：`cc_level` 长度 > 20）。

### 合并

**OBS-ORG-012**：When 合并部门一、二到新部门，the system shall：  
- 要求三者均已选择且一≠二；  
- 若合并前部门仍有启用子部门则拒绝；  
- 将员工 `cc_code` 迁到新部门；  
- 旧部门 `cc_enable=1`，备注类似「与××合并为××」。

### 人员部门归属（src 侧强化）

**OBS-ORG-013**：Where 使用 `w_rs_cc_mstr` 页签「人员部门归属」，the system shall 支持按条件检索人员并批量变更所属部门（`d_rs_empcc_batcedit*` +「批 量」按钮）。

---

## 关键对象清单（hr12 `deptmanger.pbl`）

| 对象 | 类型 | 职责 |
|------|------|------|
| `w_deptmanger` | Window | 组织架构主界面 |
| `w_companyset` | Window | 集团/公司/事业部维护 |
| `w_dept_select` | Window | 部门单选弹窗 |
| `w_dept_multipleslect` | Window | 部门多选 |
| `uf_getnew_level` | Function | 层级码下一位 |
| `uf_updateparentid` | Function | 回写 parentid |
| `d_deptmanager` | DataWindow | 部门维护网格 |
| `d_companymanager` / `old` | DataWindow | 公司维护 |
| `d_department_group` | DataWindow | 变更/合并操作区 |
| `d_depthistroy` | DataWindow | 部门历史 |
| `d_cc_*` / `s_dept*` | DW/结构 | 辅助检索与结构体 |
| `u_depttree` | UserObject | 部门树（相关） |
| `f_ini_usercc_*` | Function | 用户部门权限初始化 |

## 关键对象清单（src `w_rs_cc_mstr`）

| 对象 | 职责 |
|------|------|
| `w_rs_cc_mstr` | 主窗 |
| `d_rs_cc_mstr` | 部门维护 |
| `d_rs_cc_mstr_delds` | 删除员工校验 |
| `d_rs_cc_changdw1` / `d_rs_cc_changdw2` | 合并/调动表单 |
| `d_rs_cc_detail` | 部门明细 |
| `d_rs_empcc_batcedit` / `_sele` | 人员批量改部门 |
| `d_rs_cc_countbase_set` | 统计单位相关 |
| `of_cchist_insert` | 部门历史插入（当前实现为空 stub） |

---

## 数据模型要点（`cc_mstr`）

两端共用字段语义（新实体见 `CcMstr.cs`）：

| 字段 | 含义 |
|------|------|
| `cc_code` | 部门编码（主键） |
| `cc_desc` / `cc_desc_en` | 中/英名称 |
| `cc_level` | 层级码（2 位一层） |
| `cc_code_parent` / `ParentID` | 父部门 |
| `cc_enable` | 0=启用，1=停用（软删） |
| `cc_dele_date` / `cc_dele_remark` | 删除信息 |
| `cc_site` / `cc_cost` / `cc_kq*` | 厂区、成本、考勤扩展 |
| `OrgId` | **仅新 Web**：映射 `SysOrg.Id` |

hr12 特有/并行字段（从脚本可见）：`dept_parentlevel`、`dept_enable`、`deleteuser`、`cc_sort`、`dept_path`、`deptcount` 等——与 src/新库字段命名不完全一致，迁移时需按现场库实际列对齐。

---

## Observed Non-Functional

### Security
- hr12：增删改受 `v_usercc` 管辖范围约束。  
- 新 Web：接口登录鉴权 + `v-auth="'hrOrg/...'"`；`SyncFromCcMstr` 限超管。

### Consistency
- 删除/合并均为软删，保留历史痕迹。  
- 调动必须整树重算 `cc_level`，否则子孙节点路径断裂。

### Error Handling（观测）
- SQL 失败 → `rollback` + MessageBox。  
- 业务拒绝 → 明确中文提示（在职员工、子部门、权限、层级极限等）。

---

## Inferred Acceptance Criteria

### AC-001 直接下级列表
Given 树上选中层级码 `AB`  
When 打开部门维护  
Then 仅显示 `cc_level` 长度为 4 且前缀为 `AB` 且 `cc_enable=0` 的行。

### AC-002 新增编码
Given 当前最大 `cc_code=0000000123`  
When 新增  
Then 新行 `cc_code=0000000124`，`cc_level` 为父级码 + `uf_getnew_level` 结果。

### AC-003 有员工不可删
Given 部门下存在在职员工  
When 删除  
Then 提示无法删除且 `cc_enable` 保持 0。

### AC-004 合并
Given 部门 A、B 均无子部门，目标为 C  
When 确认合并  
Then A/B 员工 `cc_code` 变为 C，A/B `cc_enable=1` 且备注含合并说明。

### AC-005 调动子树
Given 部门 A（含子孙）调往 B  
When 成功  
Then A 及子孙 `cc_level` 前缀全部替换为 B 下新前缀，父子 `parentid` 一致。

---

## 与已落地 Web 的差距 / 建议

| 已覆盖 | 待确认是否补齐 |
|--------|----------------|
| CRUD、导入 Sync、合并、调动、批量成本/考勤 | hr12「人员部门归属」批量（可能属员工模块） |
| SysOrg 双写 | hr12 `v_usercc` 管辖刷新是否要对齐权限服务 |
| 软删语义（Enable/Disable） | hr12 删除时「同步删子树」交互是否要在 Web 复现 |
| — | `of_cchist_insert` 历史轨迹在 src 已空实现，Web 亦未做 |

---

## Uncertainties and Questions

- [x] HR_2025 `cc_mstr` 已统一为 `cc_enable` / `cc_user` 等标准列（32 列，无 `dept_enable`）。详见 gap 文档。  
- [x] 现网无字母层级后缀；`cc_code` 为 6 位数字。  
- [x] 权限表为 `hr_sys_usercc`（无 `v_usercc` 视图）。  
- [ ] `w_deptmanger` 其余页签需 ORCA 导出再确认。  
- [ ] `w_rs_cc_mstr_new` 与 `w_rs_cc_mstr` 是否为同一窗演化版。  
- [ ] `employee.emp_state` 与 hr12 `empstate=0` 取值约定需业务确认。  

**缺口分析续篇：** `specs/hr12_org_cc_mstr_gap_analysis.md`

---

## 原始提取物

从 PBL 抽取出的可读脚本片段（含噪声，仅供追溯）：

- `doc/spec-extract/hr12_w_deptmanger.txt`
- `doc/spec-extract/hr12_w_companyset.txt`
- `doc/spec-extract/hr12_uf_updateparentid.txt`
- `doc/spec-extract/src_w_rs_cc_mstr.txt`

---

## Recommendations

1. **以本规格为契约**：Web 组织模块继续对齐 `w_rs_cc_mstr` 行为；hr12 仅作业务规则来源。  
2. **层级算法**：`uf_getnew_level` 与 `HrOrgService.BuildCcLevel` 应用同一套 2 位序列，避免混码。  
3. **软删一致**：全链路使用 `cc_enable`，停用后树/列表默认不展示。  
4. **如需完整源码**：用 PowerBuilder ORCA / `PBORCA` 从 `deptmanger.pbl`、`hr_rs_basic.pbl` 正式 Export，替换 `doc/spec-extract` 噪声文本。
