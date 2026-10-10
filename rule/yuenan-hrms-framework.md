# HRMS PowerBuilder（yuenan）项目架构

> 核对时间：2026-10-10（再次提取）  
> 来源：`yuenan/` 工程树（`hrms.pbt` / `hrms.pbw` / `*.pbl`）+ MCP 库 `HR_2025`  
> 技术栈：Appeon PowerBuilder Runtime **22.2.0.3391**  
> 产品：厦门易维「人力资源管理系统」Ver 4.0（越南现场 / 越语客户线）  
> 业务库 MCP：`hr2025-sqlserver数据库自动操作` → 库 `HR_2025`  
> 关联：菜单与模块细节亦见 `rule/src-hrms-framework.md`（路径已迁入 `yuenan/`，以本文件为准）

---

## 0. 定位（最高优先级）

- **`yuenan/` 是当前仓库中唯一的 PowerBuilder 开发 + 可运行工程**，不是「仅部署包」。
- 本机实测：约 **101** 个 `.pbl`、**1** 个 `.pbd`（`efw.pbd`）、`hrms.pbt` / `hrms.pbw`、`hrms.exe`、INI/语言包/资源/Runtime DLL。
- 仓库根下 **无** 独立 `src/` 目录；历史文档中的 `src/*` 路径一律对应为 **`yuenan/*`**。
- `.gitignore` 仍忽略 `/yuenan`，工程默认不进 Git；对照与开发以本机目录 + MCP 库为准。
- 主 Target：`hrms.pbt`（`appname=hrms`，`applib=main.pbl`，LibList **94** 项，与磁盘文件一一对应）。

---

## 1. 工程入口

| 文件 | 作用 |
|------|------|
| `yuenan/hrms.pbw` | Workspace；Targets：`hrms.pbt`、`auto_time_class.pbt`；默认 `hrms.pbt` |
| `yuenan/hrms.pbt` | 主应用 Target：`appname=hrms`，`applib=main.pbl`，完整 LibList |
| `yuenan/hrms.exe` | 主客户端 |
| `yuenan/hrms.xml` | RuntimePath → Appeon PB Runtime 22.2.0.3391 |
| `yuenan/hrms.ini` | 库连接、语言、系统品牌等 |
| `yuenan/config.ini` | 界面/语言等本地偏好 |
| `yuenan/hrms.pbr` | 资源引用 |
| `yuenan/err_info.txt` | 客户端错误日志（含 `f_getinfo_from_text_yenan` 等越语线索） |

### 附属 Target

| Target | 应用 | 入口库 | 发布 exe |
|--------|------|--------|----------|
| `auto_time_class.pbt` | `auto_time_class` | `auto_time_class.pbl` | `auto_time_class.exe`（有） |
| `autoexec.pbt` / `autoexec.pbw` | `autoexec` | `autoexec.pbl` | **无**独立 exe（未随主发布） |

---

## 2. 目录结构（实测）

```
yuenan/                          # PB 开发 + 运行一体（~793 文件 / ~453 MB）
├── hrms.pbw / hrms.pbt / hrms.exe / hrms.xml / hrms.ini / hrms.pbr
├── auto_time_class.* / autoexec.*
├── *.pbl                        # 101 个源库（主 Target 用 93 + 附属/未进表）
├── efw.pbd                      # 唯一编译库（在 LibList 中）
├── language/                    # 界面文案（en / en3 / big5；无 other.TXT）
├── res/                         # 菜单/桌面图标等（~289 文件）
├── respic/                      # 模块墙纸等（~126 文件）
└── *.dll / *.ocx / Appeon.DB.*  # Runtime 与数据访问组件（含历史 PBVM 残留）
```

约定：

| 类型 | 含义 |
|------|------|
| `.pbl` | 源对象库（IDE 编辑） |
| `.pbd` | 编译动态库；本树仅 `efw.pbd` 进主 LibList |
| LibList 顺序 | 对象查找路径；以 `hrms.pbt` 为准 |

磁盘有、**未**进主 Target LibList 的库（8）：  
`abc`、`auto_time_class`、`autoexec`、`dddw`、`Dw2Xlsnew`、`hr_kq_card_new`、`hr_test`、`pbt`。

---

## 3. 库分层（LibList → PBL/PBD）

`hrms.pbt` LibList 共 **94** 项（顺序即加载路径）。

### 3.1 内核 / 公共

| 库 | 职责 |
|----|------|
| `main.pbl` | Application、登录、主框架 |
| `hro` / `hrf` / `hrs` / `hrt` | 公共对象、框架粘合、共享 HR 服务 |
| `pub` / `pat` | 公共工具 / UI 模式（`w_pattern_*`） |
| `sys` / `sys_mail` | 系统管理、用户模块、邮件 |
| `efo` / `efl` / `eff` / `efc` / `efw.pbd` | Eway 框架 / 流程相关 |
| `dw_tools` / `dw2xls` / `dw2xls_new` | DataWindow 工具与 Excel 导出 |
| `report` / `reportupdate` | 报表与更新 |

### 3.2 业务域

| 业务域 | 典型库 | 菜单根（module_id） |
|--------|--------|---------------------|
| 人事 | `hr_rs_*` | `rs` PEM → `flow_rs` |
| 考勤 | `hr_kq_*` `uoday` `at_*` | `kq` ADM → `flow_kq` |
| 薪资 | `hr_xz_*` | `xz` PAM → `flow_xz` |
| 合同 | `hr_ht_new` | `HT` COM → `flow_ht` |
| 保险 | `hr_bx` | `BX` INM → `flow_bx` |
| 公积金 | `hr_house` | `HM` HOM → `flow_house` |
| 培训 | `hr_train_*` | `ETM` TRM → `flow_etm` |
| 绩效/KPI | `hr_per_*` `hr_jx` `hr_kh_kpi` `hr_sp_assess` | `KPI` → `w_rs_check_query` |
| 编制 | `hr_bz` | `WAM` POM → `flow_bz` |
| 招聘 | `hr_zp` | `zp` REM → `flow_zp` |
| 计件 | `hr_prm` | `PRM` → `flow_pr` |
| 后勤/门禁 | `hr_book` `hr_lodging` `hr_din` `hr_one_card` `hr_mj` `hr_door_basic` `hr_pw` `hr_assets` `hr_resources` | `XZM` 等 |
| 报表 | `hr_report_*` `hr_report_user_*` | `query` → `flow_report` |
| 接口 | `hr_sap` `hr_pipe` `hr_mobile_sms` `hr_auto_service` `hr_dataout` | `ERP` 等 |
| 综合/客户化 | `hr_cs_*` | `KCS` → `flow_wom` |
| 代码表 | `hr_code_cn` `hr_code_en` `hr_useritem_ddw` | 公共 |
| 考勤机前后端 | `at_backend` `at_basic` `at_basic_new` `at_empinfo` `at_frontend` | 附属 |

菜单树与 `module_window` 映射以库表 `sys_module_mstr` 为准（MCP 查询）。

### 3.3 LibList 完整顺序（1–94）

`hro` → `hrf` → `main` → `hrs` → `hr_code_cn/en` → `hr_rs_*`(7) → `hr_kq_*` + `uoday` + `hr_kq_diary_new` → `hr_xz_*`(7) → `hr_ht_new` → `hr_bx` → `hr_house` → `hr_train_*` → `hr_report_*` → `hr_useritem_ddw` → `hr_sp_assess` → `hr_bz` → `hr_jx` → `hr_kh_kpi` → `hr_zp` → 后勤诸库 → `hr_per_*` / `hr_resources` / `hr_assets` / `hr_out_basic` → `reportupdate` → `dw_tools` → `sys` / `sys_mail` → `pub` / `pat` → `ef*` → 接口诸库 → **`efw.pbd`** → `hr_cs_*` → `report` → `hr_report_user_*` → `hrt` → `hr_report_user` → `hr_prm` → `dw2xls*` → `at_*`。

---

## 4. 分层架构（UI / 业务 / 数据）

```
┌─────────────────────────────────────────────────────────┐
│  UI                                                      │
│  w_* · m_* · uo_* · flow_* · w_pattern_* / uo_pattern_hr│
│  壳：Application(main.pbl) → 登录 → 主框架窗口            │
├─────────────────────────────────────────────────────────┤
│  业务 / 服务                                              │
│  f_* · 窗口事件逻辑 · n_cst_*（PFC 风格 NVO）            │
│  域库：hr_rs_* / hr_kq_* / hr_xz_* / …                  │
├─────────────────────────────────────────────────────────┤
│  数据                                                    │
│  d_* / dw_*（内嵌 SQL）· SQLCA · ProfileString(hrms.ini) │
│  表前缀：h_* 业务 · sys_* 安全/元数据 · code* 等         │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
              SQL Server（MSOLEDBSQL19 / SQLNCLI）
```

数据访问以 **DataWindow / 脚本内嵌 SQL** 为主；库侧存储过程较少（见 §11）。

---

## 5. 运行配置框架

### 5.1 数据库（`yuenan/hrms.ini`）

- 可选库列表：`[database_list]`（易维、宸鸿、测试等显示名）
- 连接段：`[database]` `[database2]` `[database3]` `[database22]` `[database_OLE]` 等
- DBMS：`MSOLEDBSQL SQL Server` 或 `SNC SQL Native Client(OLE DB)`
- Provider：`MSOLEDBSQL19` / `SQLNCLI*`
- 配置中出现的库名示例：`hr_2025`、`EHR`（以现场实际启用段为准）
- `Lock=RU`
- **注意**：INI 含明文账号口令，勿提交 Git / 外传；改连接只改本地 INI，勿写入本规则文档

### 5.1.1 数据库与 MCP（分析 / 对照必用）

| 用途 | MCP 配置名 | Cursor serverId | 默认库 |
|------|------------|-----------------|--------|
| **本系统（HRMS / yuenan）** | `hr2025-sqlserver数据库自动操作` | `user-hr2025-sqlserver数据库自动操作` | **`HR_2025`** |

规则：

- 查遗留业务表、`sys_module_mstr`、权限表、`h_*`、过程/视图 → 用上述 MCP。
- 常用能力：`list_tables` / `describe_table` / `execute_query` / `list_procedures` / `describe_procedure` / `sample_table` 等；先读工具 schema 再调用。
- 需要表结构时先 `describe_table` / `list_tables`，**不要凭猜测写字段**。
- MCP 连不上时先查网络/VPN；**不要为“修 MCP”去改 `hrms.ini` 或把口令写进本文件**。
- 与其它 MCP 区分：
  - `ehr-sqlserver…` → 新系统库 `ehr`
  - `tpkhr-sqlserver…` → 另一套遗留库
  - **本文件描述的 PB / 越南线 → 只用 `hr2025-sqlserver…` → `HR_2025`**

### 5.2 系统信息（`[SYSTEM]`）

| 项 | 值 |
|----|-----|
| 产品名 | 人力资源管理系统 |
| 版本 | Ver 4.0 |
| 厂商 | 厦门易维 / Eway |
| 越语标题 | `modulenameother`（越南语产品名） |

### 5.3 语言（越南现场关键）

- 注释约定：`lanage=cn|en|big5|other`（拼写为 `lanage`）
- 文案目录：`yuenan/language/` 现有 `en.TXT` / `en3.TXT` / `big5.TXT`
- **`other.TXT`（越语包）本树缺失**；现场若启用 `lanage=other` 需补齐越语文案
- 错误日志线索：`f_getinfo_from_text_yenan`

### 5.4 其他段

- `[pic]`：图片共享路径 / 临时目录
- `[Mail]`：邮件服务器
- `[report]`：报表共享路径
- `[Ver]`：强制更新版本号
- `[cmd]`：网络映射等运维命令

---

## 6. 运行时调用链

```
Appeon PB 22.2 Runtime
        │
   hrms.exe（main.pbl Application）
        │
   读 hrms.ini → SQLCA 连接 SQL Server（默认对照库 HR_2025）
        │
   登录（sys_user_mstr 等）
        │
   按权限加载 sys_module_mstr 菜单
        │
   Open(module_window) [+ module_arguments]
        │
   域 PBL 内窗口 / DW 读写 h_* · sys_*
```

附属：`auto_time_class.exe` → `auto_time_class.pbl`（LibList 与主应用大体同套）。

---

## 7. 命名约定

| 前缀/模式 | 含义 |
|-----------|------|
| `w_*` | Window |
| `flow_*` | 域导航 / 流程壳窗口 |
| `d_*` / `dw_*` | DataWindow |
| `uo_*` / `u_*` | 可视 UserObject |
| `n_cst_*` / `n_*` | PFC 风格 NonVisualObject |
| `m_*` | Menu |
| `f_*` | Global / 共享 Function |
| `hr_<域>[_子].pbl` | 按业务域拆分的对象库 |
| `*_user.pbl` | 客户化 / 用户扩展库 |
| `w_pattern_*` / `uo_pattern_hr` | 共用 CRUD/列表模式 |

---

## 8. 与历史「src / 纯部署包」的差异

| 方面 | 本次实测（yuenan） | 旧文档描述 |
|------|-------------------|------------|
| 角色 | **开发工程 + 可运行客户端** | 曾拆为 `src` 开发 / `yuenan` 仅部署 |
| `.pbl` | **101** | 旧：yuenan=0，src≈103 |
| `.pbd` | **1**（`efw.pbd`） | 旧：yuenan≈94 全量 PBD |
| `.pbt` / `.pbw` | **有** | 旧：仅 src 有 |
| 独立 `src/` | **不存在** | 旧：源码在 `src/` |
| Git | `/yuenan` 仍 ignore | 同 |

**结论**：改窗口/脚本/库列表 → 直接改 `yuenan/*.pbl` 与 `yuenan/hrms.pbt`；查现场库/语言 → `yuenan/hrms.ini` + MCP `HR_2025`。

---

## 9. 开发 / 对照定位速查

| 要做什么 | 去哪里 |
|----------|--------|
| 改窗口/脚本/对象 | `yuenan/*.pbl`（PB IDE / ORCA） |
| 库加载顺序 / Target | `yuenan/hrms.pbt` |
| Workspace | `yuenan/hrms.pbw` |
| 数据库、语言 | `yuenan/hrms.ini` |
| Runtime 版本 | `yuenan/hrms.xml` |
| **查库表/菜单/过程/样例数据** | **MCP `hr2025-sqlserver…`（库 `HR_2025`）** |
| 菜单 → 窗口映射 | `sys_module_mstr.module_window` |
| 越语文案 | 需补 `language/other.TXT`（当前缺失） |
| 新系统迁移实现 | `Admin.NET/`（见 `rule/admin-net-framework.md`） |

---

## 10. 与仓库其他部分的关系

```
ehr/
├── yuenan/     ← 本文件：PB 开发/运行工程（越南线）
├── Admin.NET/  ← 新后端
├── Web/ App/ GoView/ docker/ …
├── hr12/ NHR.NClient/ …
└── rule/       ← 架构与开发规则文档
```

迁移或对照：功能锚点 = `sys_module_mstr.module_window` + 对应 `hr_*.pbl`。

---

## 11. 业务库架构（MCP `HR_2025` 实测）

| 项 | 值 |
|----|-----|
| 库名 / 状态 | `HR_2025` / ONLINE |
| 排序规则 | `Chinese_PRC_CI_AS` |
| 兼容级别 | 100 |
| 数据/日志约 | 319 MB / 112 MB |
| 表 / 视图 / 存储过程 | **616** / **2** / **20** |

### 11.1 表前缀分布

| 前缀 | 数量 | 角色 |
|------|------|------|
| `h_*` | 471 | 业务主数据与事务 |
| `sys_*` | 58 | 安全、菜单、邮件、任务、元数据 |
| `code*` | 5 | 代码表相关 |
| 其他 | 82 | 杂项/历史/辅助 |

业务表粗分（表名启发式）：绩效/KPI ~57、计件/PR ~44、人事组织 ~26、考勤 ~20、培训 ~20、薪资 ~19、保险 ~14、合同 ~10、招聘 ~8、其余 `h_*` 约 253。

### 11.2 菜单驱动（`sys_module_mstr`）

顶级模块（`module_parent_id = 0`，MCP 实测；含少量杂项根节点）：

| module_id | 名称 | module_window |
|-----------|------|---------------|
| `rs` | PEM 人事管理 | `flow_rs` |
| `kq` | ADM 考勤管理 | `flow_kq` |
| `xz` | PAM 薪酬管理 | `flow_xz` |
| `HT` | COM 合同管理 | `flow_ht` |
| `BX` | INM 保险管理 | `flow_bx` |
| `HM` | HOM 公积金 | `flow_house` |
| `ETM` | TRM 培训 | `flow_etm` |
| `KPI` | KPI 考核 | `w_rs_check_query` |
| `WAM` | POM 编制 | `flow_bz` |
| `zp` | REM 招聘 | `flow_zp` |
| `ERP` | ERP 接口 | （无窗口） |
| `PRM` | PRM 计件 | `flow_pr` |
| `KCS` | KCS 综合 | `flow_wom` |
| `FLOW` | FOM 流程 | `flow_fom` |
| `SAT` | SAT 问卷 | （无窗口） |
| `XZM` | XZM 行政后勤 | `flow_xzm` |
| `query` | QUM 自助查询 | `flow_report` |
| `DMM` | DMM 决策分析 | `flow_jcfx` |
| `SYS` | SYS 系统管理 | `flow_sys` |

菜单规模：共 **495** 节点；**437** 有 `module_window`（`flow_*` 20 / `w_*` 354）。

权限相关表：`sys_user_mstr`、`sys_role_mstr`、`sys_umodule_mstr`、`sys_user_mdept`、`sys_user_column`、`sys_purview` 等。

### 11.3 存储过程（20）

以 DataWindow SQL 为主。与越南/考勤相关：

| 过程 | 备注 |
|------|------|
| `hr_kq_leave_year_yenan` | **越南年假**（2025-10） |
| `hr_kq_leave_year` / `hr_kq_leave_errtest` | 年假通用 |
| `hr_kq_month_computer` / `hr_kq_shift_mstr` | 月考勤计算 / 班次 |
| `usp_login_emp` / `usp_login_cc` | 登录 |
| `hr_xz_user`、`ufp_dw_*`、`usp_update_h_month_mon_ovdate` 等 | 薪资/DW 权限/加班日期 |

完整列表：MCP `list_procedures` → `describe_procedure`。

### 11.4 工程一致性核对

| 核对项 | 结果 |
|--------|------|
| `hrms.pbt` LibList | **94** |
| LibList 文件均在磁盘 | **是**（93 `.pbl` + `efw.pbd`） |
| 磁盘 `.pbl` 总数 | **101**（多出 8 个未进主 Target） |
| Runtime | `hrms.xml` → Appeon **22.2.0.3391** |
| 越语文案 `other.TXT` | **缺失** |
| 独立 `src/` | **不存在** |

---

## 12. 待澄清项

1. 无 `.sra` 文本导出时，脚本级细节需 PB IDE / ORCA 打开 `.pbl`。
2. 生产实际启用的 `[database*]` 段以现场为准；**分析对照默认 MCP `HR_2025`**。
3. `language/other.TXT` 缺失；启用越语 UI 前需补齐。
4. `ef*` / `hro`/`hrf`/`hrs`/`hrt` 对象级职责以 IDE 对象列表为准。
5. 目录中残留的旧版 `pbvm90` / `PBVM125` 等 DLL 可能是历史遗留，以 `hrms.xml` 指向的 22.2 为准。
6. `KPI` 根节点窗口为 `w_rs_check_query`（非 `flow_kh`），以 MCP 为准。
7. `rule/src-hrms-framework.md` 仍写 `src/` 路径，内容层面对齐菜单/命名；**路径与工程入口以本文件为准**。
)
