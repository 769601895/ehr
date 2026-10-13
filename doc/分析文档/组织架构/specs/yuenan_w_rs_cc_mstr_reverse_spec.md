# Reverse-Engineered Specification: 组织架构管理（yuenan · `w_rs_cc_mstr`）

> 提取时间：2026-10-10  
> 工程：`yuenan/`（Appeon PB 22.2 / `hrms.pbt`）  
> 主对象：`w_rs_cc_mstr` ∈ `yuenan/hr_rs_basic.pbl`（LibList 第 7 位）  
> 对照库：`HR_2025.dbo.cc_mstr`  
> 原始脚本片段：`../spec-extract/src_w_rs_cc_mstr.txt`  
> 语言包：`yuenan/language/big5.TXT` 段 `[w_rs_cc_mstr]`  
> 关联：`rule/yuenan-hrms-framework.md`；对照/缺口见同目录 `hr12_org_cc_mstr_*`  
>
> **文档角色（2026-10-10）**：本文件为**逆向分析 / 只读参考**，不是实现说明书。  
> 实施以 `../migration-plan/` 为准：按**新系统逻辑**编写，产品与代码**不得**出现「从旧系统迁移」痕迹。  
> `tabpage_2`「人员部门归属」本模块不做；OBS-011/012 仅存档。

---

## Overview

越南线 HRMS（`yuenan`）的**组织架构 / 部门主档维护**入口窗口为 **`w_rs_cc_mstr`**，基类 `w_pattern_hr`（左组织树 `uo_2` + 右侧多页签）。核心表 **`cc_mstr`**，层级用 **`cc_level`（每层 2 位）** 表达；删除/合并均为**软删**（`cc_enable=1`）。

二进制核对：`hr_rs_basic.pbl` 内 `w_rs_cc_mstr` 以 **UTF-16LE** 出现约 15 次；`hrms.pbt` LibList 含 `hr_rs_basic.pbl`。

---

## Architecture Summary

### Technology Stack

| 层 | 实现 |
|----|------|
| 客户端 | Appeon PowerBuilder 22.2 · `hrms.exe` |
| 入口库 | `yuenan/hr_rs_basic.pbl` |
| 主窗 | `w_rs_cc_mstr` ← `w_pattern_hr` |
| 数据访问 | DataWindow + 嵌入 SQL（`SQLCA` / 部分 `SQLCA_oleconnect`） |
| 主表 | `cc_mstr`；员工 `h_emp_mstr`；厂别 `h_site_set`；单号 `h_bill_mstr`（`bill_type='部门内码'`） |

### Module Structure

```
yuenan/
├── hrms.pbt                     # LibList 含 hr_rs_basic.pbl
├── hr_rs_basic.pbl              # ★ 本模块
│   ├── w_rs_cc_mstr             # 主窗
│   ├── d_rs_cc_mstr             # 部门维护
│   ├── d_rs_cc_mstr_delds       # 删除前员工校验
│   ├── d_rs_cc_changdw1         # 调动表单
│   ├── d_rs_cc_changdw2         # 合并表单
│   ├── d_rs_cc_detail           # 部门明细 / 调动历史镜像
│   ├── d_rs_empcc_batcedit*     # 人员部门归属批量
│   └── w_rs_ccsele_pop          # 部门选择弹窗
└── language/big5.TXT            # [w_rs_cc_mstr] 控件文案
```

### Data Flow

```
登录 → sys_module_mstr 打开 w_rs_cc_mstr
        │
        ▼
uo_2 组织树选中 → is_level
        │
        ├─ tab1 部门维护：retrieve(is_level, is_level+'zz', len+2)
        │     新增 / 保存 / 软删 / Visio 展开
        ├─ tab2 人员部门归属（PB 默认 hidden；**新系统不迁入**）
        ├─ tab3 部门异动：合并 / 调动
        └─ tab4 部门明细：列表 + Excel 导出
```

---

## UI 页签与控件（观测）

| 页签 | 文案（源码 / big5） | 主要控件 | DataWindow |
|------|---------------------|----------|------------|
| tabpage_1 | 部门维护 / 部門維護 | `dw_1`；工具栏查询/保存 | `d_rs_cc_mstr` |
| tabpage_2 | 人员部门归属 | `dw_2` + `dw_5` +「批 量」；PB `visible=false`；**新系统排除** | `d_rs_empcc_batcedit` / `_sele` |
| tabpage_3 | 部门异动 / 部門异動 | 「合 并」「调 动」 | `d_rs_cc_changdw2` / `d_rs_cc_changdw1` |
| tabpage_4 | 部门明细 / 部門明細 | `dw_6` + 导出按钮 | （明细 DW，调动时用 `d_rs_cc_detail`） |
| tabpage_5 / 6 | 有 create/resize 注册 | `dw_7` 等 | **提取片段在 tab4 后噪声截断，待 ORCA 补齐** |

语言包 `[w_rs_cc_mstr]`（`../spec-extract/yuenan_lang_w_rs_cc_mstr.txt`）：

- `tabpage_1` 部門維護 · `tabpage_3` 部門异動 · `tabpage_4` 部門明細  
- `cb_1` 批 量 · `cb_2` 調 動 · `cb_4` 合 并 · `st_3` 變更部門 · `st_4` 選擇  

工具栏（`open`）：隐藏「存增」；显示「查询」；新增/删除菜单有历史注释开关。

---

## Observed Functional Requirements（EARS）

### 树与列表

**OBS-YN-ORG-001**：When 用户在 `uo_2` 树上选中节点（`is_level`），the system shall 触发 `dw_1.pfc_retrieve`，检索参数为 `(is_level, is_level+'zz', len(is_level)+2)`（直接下级一层）。

**OBS-YN-ORG-002**：While 部门维护页打开，the system shall 默认将 `dw_1` 设为 `readonly=true`；新增时解除只读。

### 新增

**OBS-YN-ORG-003**：When 新增部门，the system shall：  
1）用 `max(cast(substring(cc_level,len(is_level)+1,2) as int))+1` 生成同级序号（`SQLCA_oleconnect`），拼 `is_level + string(i_tmp,'00')`；  
2）要求已维护 `h_site_set` 厂别，否则提示并中止；  
3）调用 `uf_site_regiser('empadd')`，失败则中止；  
4）非根层继承上级 `cc_site / cc_woca / cc_bz_unit / cc_stat_basic`；根层取 `h_woca_set.woca_ok=1`；  
5）写入 `cc_date`、`cc_user`，焦点落到 `cc_dept`。

**OBS-YN-ORG-004**：When 保存新行且 `cc_code` 为空，the system shall 从 `h_bill_mstr`（`bill_type='部门内码'`）取号递增（默认 6 位），并与 `max(cc_code)` 取较大值后回写单号表。

**OBS-YN-ORG-005**：When 保存前校验，the system shall 要求每行 `cc_dept`、`cc_desc` 非空，否则提示并 `return -1`。

### 删除（软删除）

**OBS-YN-ORG-006**：When 删除已落库部门，the system shall **不硬删**：置 `cc_enable=1`，写 `cc_dele_date`、`cc_dele_remark='删除撤消'`，并对 `cc_level like 当前+'%'` 且仍启用的子树一并软删。

**OBS-YN-ORG-007**：When `d_rs_cc_mstr_delds` 检出在职（参数 1）或停职（参数 2）员工，the system shall 拒绝删除并提示先调离。

**OBS-YN-ORG-008**：When 删除/保存成功且曾新增过行，the system shall 可选提示「是否重新刷新组织架」并重建 `uo_2.tv`。

### 调动

**OBS-YN-ORG-009**：When 在「部门异动」点「调 动」，the system shall：  
- 校验调动部门、归属部门已选且 `cc_level` 不同；  
- 若目标层级是源的子孙（`pos(ls_level2,ls_level1)>0`）则静默拒绝；  
- 在目标下生成新同级码 `ls_level3`；  
- `update cc_mstr set cc_level = ls_level3 + right(...)` 整棵源子树改前缀；  
- 用 `d_rs_cc_detail` 对原节点写软删镜像（新 `cc_code`、备注「被调至…」）。

### 合并

**OBS-YN-ORG-010**：When 点「合 并」，the system shall：  
- 要求 old1 / old2 / new3 均已选，且 old1≠old2；  
- 若任一侧仍有启用子部门则拒绝；  
- 将两侧员工（`emp_state<>3`）`cc_code` 迁到新部门；  
- 非目标侧旧部门软删，备注形如「与××合并为××」；成功后清空表单并刷新树。

### 人员部门归属（PB 观测 · **新系统不实现**）

**OBS-YN-ORG-011**（仅存档）：Where tabpage_2 可用，PB 按员工条件检索 `d_rs_empcc_batcedit`，支持「全部选择/撤消选择」，并通过「批 量」将勾选人员（`emp_free=0`）的 `h_emp_mstr.cc_code` 改为目标部门后保存。  
**迁移：不做**对应页面与 API。

**OBS-YN-ORG-012**（仅存档，随 tab2 排除）：While `uo_2.tv.ii_usercc=0`，PB 将 tab2 检索层级限制为 `gstr_userinfo.mdept`；多部门权限时 `is_level=''`。

### 明细与其它

**OBS-YN-ORG-013**：Where 部门明细页提供导出，the system shall 经 `GetFileSaveName` 导出 Excel（`n_cst_dw2excel`）。

**OBS-YN-ORG-014**：Where 触发 `ue_graphmode`，the system shall 按用户输入级次调用 `nvo_output_visio.of_output_visio(is_level)` 输出组织图。

**OBS-YN-ORG-015**：The system shall 允许在维护行上通过 `w_rs_ccsele_pop` 选择辅助编码写入 `cc_code2`。

**OBS-YN-ORG-016**：`of_cchist_insert` 当前实现为 **空 stub**（`return ''`），部门历史插入未生效。

---

## 关键对象清单

| 对象 | 类型 | 职责 | 证据 |
|------|------|------|------|
| `w_rs_cc_mstr` | Window | 主界面 | PBL UTF-16 + 脚本头 |
| `d_rs_cc_mstr` | DW | 部门维护网格 | `dw_1.dataobject` |
| `d_rs_cc_mstr_delds` | DW | 删前员工校验 | `pfc_deleterow` |
| `d_rs_cc_changdw1` | DW | 调动：cc_1→cc_2 | tab3 `dw_4` |
| `d_rs_cc_changdw2` | DW | 合并：old1/old2/new3 | tab3 `dw_3` |
| `d_rs_cc_detail` | DW | 明细 / 调动镜像 | 调动脚本 |
| `d_rs_empcc_batcedit` | DW | 人员列表 | tab2 `dw_2` |
| `d_rs_empcc_batcedit_sele` | DW | 目标部门选择 | tab2 `dw_5` |
| `w_rs_ccsele_pop` | Window | 部门选择弹窗 | 多处 `open(...)` |
| `uo_emp_find` | UO | 员工检索 | tab2 `uo_3` |
| `nvo_output_visio` | NVO | Visio 组织图 | `ue_graphmode` |

---

## 数据模型要点（`cc_mstr`）

与 `Admin.NET.../Entity/CcMstr.cs` / 现场库对齐的关键字段：

| 字段 | 含义 |
|------|------|
| `cc_code` | 部门内码（主键；单号表生成） |
| `cc_dept` / `cc_desc` / `cc_desc_en` | 代号 / 名称 / 英文名 |
| `cc_level` | 层级码（2 位一层） |
| `cc_site` / `cc_woca` / `cc_bz_unit` / `cc_stat_basic` | 厂别、工时、编制、统计 |
| `cc_enable` | 0=启用，1=停用（软删） |
| `cc_dele_date` / `cc_dele_remark` | 删除信息 |
| `cc_user` / `cc_date` / `cc_create_date` | 维护审计 |
| `cc_code2` | 辅助编码（弹窗选择） |
| `cc_cost` / `cc_kq*` / `cc_xz_group` 等 | 成本、考勤、薪酬扩展 |

层级约定：直接下级 = `cc_level LIKE parent+'%'` 且 `LEN = LEN(parent)+2` 且 `cc_enable=0`。

### 人员资料 `h_emp_mstr`（组织侧）

| 字段 | 含义 |
|------|------|
| `emp_id` | 工号（主键） |
| **`cc_code`** | **所属部门**，对应 `cc_mstr.cc_code` |
| `emp_state` | 人员状态（合并只迁 `<>3`；删前区分在职/停职） |
| `emp_name` | 姓名 |

现场 `HR_2025` 全表约 215 列（含 `bs_id` 自增）。新库组织模块依赖上表关联，不使用 `employee` 表名。

---

## Observed Non-Functional

### Security / 权限
- 树与人员页受 `ii_usercc` / `gstr_userinfo.mdept` 影响。  
- 厂别注册人数 `uf_site_regiser('empadd')` 限制新增。

### Consistency
- 删除、合并、调动历史侧均为软删痕迹。  
- 调动必须整树改写 `cc_level` 前缀。

### Error Handling
- SQL 失败 → `rollback` + MessageBox。  
- 业务拒绝 → 中文提示（在职/停职员工、子部门、未选部门等）。

---

## Inferred Acceptance Criteria

### AC-001 直接下级
Given 树节点 `is_level='AB'`  
When 打开部门维护  
Then 仅列出长度 4、前缀 `AB` 的启用部门。

### AC-002 新增继承
Given 上级已有 `cc_site/cc_woca/...`  
When 在其下新增  
Then 新行带出上级厂别与工时等默认值，且 `cc_level` 为同级下一序号。

### AC-003 有员工不可删
Given 部门（含下级）存在在职或停职员工  
When 删除  
Then 拒绝，且不改 `cc_enable`。

### AC-004 合并
Given A、B 均无启用子部门，目标 C  
When 确认合并  
Then 非离职员工迁入 C，A/B（若≠C）软删并写合并备注。

### AC-005 调动子树
Given A（含子孙）调往 B  
When 成功  
Then A 子树 `cc_level` 前缀变为 B 下新码，并写「被调至」软删镜像行。

---

## Uncertainties and Questions

- [ ] `sys_module_mstr` 中本窗菜单中文名 / `module_id`（本次 MCP 不可用，需补查）。  
- [ ] `tabpage_5` / `tabpage_6` 业务含义（脚本提取在 tab4 后混入其它窗对象）。  
- [ ] `en.TXT` 无 `[w_rs_cc_mstr]` 段；现场越语 `other.TXT` 缺失时的文案回退策略。  
- [ ] 新增层级算法仅用数字 `00..` 递增，与 hr12 `uf_getnew_level`（可至 `A0..ZZ`）是否应对齐。  
- [ ] 是否存在 `w_rs_cc_mstr_new` 演化版（Web Dto 注释曾提及）。

---

## 原始提取物

| 路径 | 说明 |
|------|------|
| `../spec-extract/src_w_rs_cc_mstr.txt` | PBL 可读脚本（含噪声，tab4 后不可靠） |
| `../spec-extract/yuenan_lang_w_rs_cc_mstr.txt` | big5 语言段 UTF-8 摘录 |
| `yuenan/hr_rs_basic.pbl` | 权威二进制源 |
| `yuenan/language/big5.TXT` | 界面文案 |

---

## Recommendations

1. 业务规则（软删、合并、调动、直接下级）写入 `../migration-plan/`，实现时用**本系统语言**表述，勿在代码注释写「对齐本窗」。  
2. 需要更干净源码摘录时可用 PB ORCA 再 Export，仅充实本分析目录。  
3. **tab2「人员部门归属」**：本模块不做。  
4. 禁止把本文件中的 PB 对象名、迁移措辞带进 API / 前端 / 菜单。
