# HRMS PowerBuilder（src）项目框架

> 来源：`src/` 工程文件 + `hr_2025.sys_module_mstr`  
> 技术栈：PowerBuilder / Appeon Runtime 22.2  
> 应用名：`hrms`（人力资源管理系统 Ver 4.0）

---

## 1. 工程入口

| 文件 | 作用 |
|------|------|
| `src/hrms.pbw` | Workspace，默认 Target：`hrms.pbt` |
| `src/hrms.pbt` | 主应用 Target：`appname=hrms`，`applib=main.pbl` |
| `src/hrms.exe` | 运行时主程序 |
| `src/hrms.xml` | Runtime 路径：Appeon PB Runtime 22.2.0.3391 |
| `src/hrms.ini` | 数据库、语言、系统信息等运行配置 |
| `src/config.ini` | 界面/语言等本地配置 |
| `src/hrms.pbr` | 资源库引用 |

### 附属 Target

| Target | 应用 | 入口库 | 用途 |
|--------|------|--------|------|
| `autoexec.pbt` / `autoexec.pbw` | `autoexec` | `autoexec.pbl` | 自动执行任务 |
| `auto_time_class.pbt` | `auto_time_class` | `auto_time_class.pbl` | 考勤/时间类后台 |

---

## 2. 目录结构

```
src/
├── hrms.pbw / hrms.pbt / hrms.exe / hrms.ini / hrms.xml
├── autoexec.* / auto_time_class.*          # 附属应用
├── *.pbl / *.pbd                           # 源库 / 编译库
├── language/                               # 多语言包
│   ├── en.TXT / en3.TXT
│   ├── big5.TXT
│   └── other.TXT
├── res/ / respic/                          # 桌面图标、菜单图、UI 资源
├── config.ini
└── *.dll / *.ocx                           # PB Runtime 与第三方组件
```

约定：

- **`.pbl`**：PowerBuilder 源对象库（开发编辑）
- **`.pbd`**：编译后动态库（运行分发）
- 菜单打开的窗口名写在库表 `sys_module_mstr.module_window`

---

## 3. 库分层（LibList）

`hrms.pbt` 的 `LibList` 按职责可归为：

### 3.1 内核 / 公共

| PBL | 职责 |
|-----|------|
| `main.pbl` | 应用入口 Application、主框架 |
| `hro.pbl` / `hrf.pbl` / `hrs.pbl` / `hrt.pbl` | 公共对象、框架、共享逻辑 |
| `pub.pbl` / `pat.pbl` | 公共工具 / 模式 |
| `sys.pbl` / `sys_mail.pbl` | 系统管理、用户角色权限、邮件 |
| `dw_tools.pbl` / `dw2xls.pbl` / `dw2xls_new.pbl` | DataWindow 工具、导出 Excel |
| `report.pbl` / `reportupdate.pbl` | 报表与报表更新 |

### 3.2 业务域 PBL（与菜单模块对应）

| 业务域 | 典型 PBL | 菜单根（module_id） |
|--------|----------|---------------------|
| 人事 | `hr_rs_basic` `hr_rs_change` `hr_rs_jc` `hr_rs_dim` `hr_rs_count` `hr_rs_computer` `hr_rs_user` | `rs` PEM |
| 考勤 | `hr_kq_basic` `hr_kq_apply` `hr_kq_diary` `hr_kq_diary_new` `hr_kq_formula` `hr_kq_user` `hr_kq_card` `uoday` | `kq` ADM |
| 薪酬 | `hr_xz` `hr_xz_kb` `hr_xz_formula` `hr_xz_transfer` `hr_xz_count` `hr_xz_computer` `hr_xz_user` | `xz` PAM |
| 合同 | `hr_ht_new` | `HT` COM |
| 保险 | `hr_bx` | `BX` INM |
| 公积金 | `hr_house` | `HM` HOM |
| 培训 | `hr_train_new` `hr_train_user` | `ETM` TRM |
| 考核/KPI | `hr_kh_kpi` `hr_jx` `hr_sp_assess` | `KPI` |
| 编制 | `hr_bz` | `WAM` POM |
| 招聘 | `hr_zp` | `zp` REM |
| 计件 | `hr_prm` | `PRM` |
| 综合/客户化 | `hr_cs_mstr` `hr_cs_report*` | `KCS` |
| 行政后勤 | `hr_book` `hr_lodging` `hr_din` `hr_one_card` `hr_mj` `hr_pw` `hr_assets` `hr_resources` | `XZM` 等 |
| 报表 | `hr_report_*` `hr_report_user_*` | `query` QUM |
| 接口/服务 | `hr_sap` `hr_pipe` `hr_dataout` `hr_auto_service` `hr_mobile_sms` `hr_door_basic` | `ERP` 等 |
| 代码表 | `hr_code_cn` `hr_code_en` `hr_useritem_ddw` | 公共 |
| 人事档案扩展 | `hr_per_basic` `hr_per_user` `hr_out_basic` | — |
| 流程/表单引擎 | `efo` `efl` `eff` `efc` `efw` | `FLOW` |
| 考勤机/前后端 | `at_backend` `at_basic` `at_basic_new` `at_empinfo` `at_frontend` | 附属 |

### 3.3 构建产物工程

`hrms.pbt` 内定义多个 `p_hrms_exe*` 工程节点，均以 `main.pbl` 为入口，用于分版本/分批编译发布。

---

## 4. 运行配置框架

### 4.1 数据库（`hrms.ini`）

- 主连接段：`[database]`
- DBMS：`MSOLEDBSQL SQL Server`（亦有 `SNC` / OLE DB 历史配置）
- 默认库：**`hr_2025`**
- 关键段：`[database2]` `[database3]` `[database22]` 等，用于多环境切换
- `[database_list]`：登录界面可选库列表

### 4.2 系统信息（`[SYSTEM]`）

- 产品名：人力资源管理系统
- 版本：Ver 4.0
- 厂商：易维 / Eway

### 4.3 语言

- `lanage=cn|en|big5|other`
- 文案文件：`src/language/*.TXT`
- 窗口控件翻译按 `[窗口名]` 分段（如 `[w_sys_roleuser_m_ls]`）

### 4.4 其他

- `[pic]`：图片共享路径 / 临时目录
- `[Mail]`：邮件服务器
- `[report]`：报表共享路径
- `[Ver]`：强制更新版本号

---

## 5. 菜单 / 模块框架（数据库驱动）

表：**`dbo.sys_module_mstr`**

| 关键字段 | 含义 |
|----------|------|
| `module_id` | 模块编号（树节点） |
| `module_name` / `module_name_en` | 中英文名称 |
| `module_parent_id` | 父模块（根为 `0`） |
| `module_window` | **打开的 PB 窗口对象名**（核心映射） |
| `module_arguments` | 打开参数 |
| `module_type` | `CS` 客户端 / `BS` Web |
| `module_sort` | 排序 |
| `module_help` / `module_ico` | 帮助、图标 |

### 5.1 顶级业务模块

| module_id | 名称 | 导航窗口 |
|-----------|------|----------|
| `rs` | PEM 人事管理 | `flow_rs` |
| `kq` | ADM 考勤管理 | `flow_kq` |
| `xz` | PAM 薪酬管理 | `flow_xz` |
| `HT` | COM 合同管理 | `flow_ht` |
| `BX` | INM 保险管理 | `flow_bx` |
| `HM` | HOM 公积金管理 | `flow_house` |
| `ETM` | TRM 培训管理 | `flow_etm` |
| `KPI` | KPI 考核管理 | `w_rs_check_query`（库实测；非 `flow_kh`） |
| `WAM` | POM 编制管理 | `flow_bz` |
| `zp` | REM 招聘管理 | `flow_zp` |
| `PRM` | PRM 计件管理 | `flow_pr` |
| `KCS` | KCS 综合管理 | `flow_wom` |
| `FLOW` | FOM 流程管理 | `flow_fom` |
| `XZM` | XZM 行政后勤 | `flow_xzm` |
| `query` | QUM 自助查询 | `flow_report` |
| `DMM` | DMM 决策分析 | `flow_jcfx` |
| `SYS` | SYS 系统管理 | `flow_sys` |

### 5.2 系统管理（用户相关）子树

```
SYS 系统管理 → flow_sys
├── SYS.01 系统基本设置
│   ├── SYS.01.01 角色&用户设置        → w_sys_roleuser_m_ls        （标准，主入口）
│   ├── SYS.01.02 系统模块设置          → w_sys_module_m
│   └── SYS.01.03 角色&用户设置(岗位版) → w_sys_roleuser_m_ls_new
└── SYS.02 系统权限管理
    ├── SYS.02.01 角色权限设置          → w_sys_rolepv_p
    ├── SYS.02.04 用户权限设置          → w_sys_userpv_p
    ├── sys.02.07 窗口功能权限设置      → w_sys_userpv_window
    ├── SYS.02.06 用户人员资料查询权限  → w_sys_user_columnrs
    ├── SYS.02.05 按模块查询权限用户    → w_sys_modulepv_p
    ├── SYS.02.08 用户多部门权限设置    → w_hr_deptpv_p_yr
    ├── SYS.02.02 用户权限设置(岗位版)  → w_sys_userpv_p_new2
    └── SYS.02.03 岗位权限设置(岗位版)  → w_sys_userpv_p_new
```

对象库优先查找：**`sys.pbl`**

### 5.3 用户相关数据表

| 表 | 用途 |
|----|------|
| `sys_user_mstr` | 用户主档 |
| `sys_role_mstr` | 角色主档 |
| `sys_umodule_mstr` | 用户模块权限 |
| `sys_user_mdept` | 用户多部门 |
| `sys_user_column` | 用户列权限 |

---

## 6. 运行时调用链（概念）

```
hrms.exe
  → Application (main.pbl)
  → 读 hrms.ini 连接 hr_2025
  → 登录校验 sys_user_mstr
  → 按权限加载 sys_module_mstr 菜单树
  → 点击菜单 → Open(module_window) [+ module_arguments]
  → 业务窗口读写对应业务表 / 存储过程
```

---

## 7. 命名约定

| 前缀/模式 | 含义 |
|-----------|------|
| `w_*` | Window |
| `d_*` / `dw_*` | DataWindow |
| `u_*` | UserObject |
| `n_*` / `nvo_*` | NonVisualObject |
| `m_*` | Menu |
| `f_*` | Function / Global Function |
| `flow_*` | 业务域导航/流程图窗口 |
| `hr_<域>_*.pbl` | 按业务域拆分的对象库 |
| `*_user.pbl` | 客户化/用户扩展库 |
| `SYS.xx.xx` | 系统管理模块编号 |

---

## 8. 开发定位速查

| 要改什么 | 去哪里 |
|----------|--------|
| 菜单是否显示、打开哪个窗口 | `sys_module_mstr` |
| 用户/角色维护界面 | `sys.pbl` → `w_sys_roleuser_m_ls` |
| 权限分配 | `sys.pbl` → `w_sys_userpv_p` / `w_sys_rolepv_p` |
| 人事业务 | `hr_rs_*.pbl` |
| 考勤业务 | `hr_kq_*.pbl` |
| 薪酬业务 | `hr_xz*.pbl` |
| 界面多语言 | `src/language/*.TXT` |
| 数据库连接 | `src/hrms.ini` `[database]` |
| 应用入口/库列表 | `src/hrms.pbt` |

---

## 9. 与仓库其他部分的关系

本仓库同时包含：

- `src/` — 本文件描述的 **PowerBuilder 客户端 HRMS**
- `Admin.NET/` + `Web/` — 新一代后端/前端（见根目录 `启动程序步骤`）
- `App/` `GoView/` `docker/` — 其他产品线组件

迁移或对照时：以 `sys_module_mstr.module_window` + 对应 `.pbl` 为遗留功能锚点。
