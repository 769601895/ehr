# Admin.NET（EHR 新系统）项目规则

> 技术栈：.NET 8 / .NET 10 + Furion + SqlSugar  
> 入口：`Admin.NET/Admin.NET.sln`  
> 老系统（仅供对照参考，不在其内开发）：`NHR.NClient/`（.NET Framework 4.0 WinForms）、`src/`（PowerBuilder）

---

## 0. 开发边界（最高优先级）

- **本仓库后续所有新功能开发，一律在 `Admin.NET/` 下进行**，包括实体、服务、接口、配置、种子数据。
- `NHR.NClient/`（.NET 4.0 三层 WinForms）与 `src/`（PowerBuilder）属于**遗留系统**：只读参考，用于核对旧业务逻辑/旧表结构，**不得新增或修改其业务代码**（除非用户明确要求维护老系统）。
- 迁移做法：先读懂老系统的业务与表，再按本规则在 `Admin.NET.Application` 中重新实现，**不复制老代码风格**。

---

## 1. 解决方案结构

`Admin.NET/Admin.NET.sln` 共 13 个项目，依赖链单向：

```
Admin.NET.Web.Entry  →  Admin.NET.Web.Core  →  Admin.NET.Application  →  Admin.NET.Core
                                                        ↑
                                             Plugins/Admin.NET.Plugin.*
```

| 项目 | 职责 | 写什么 |
|------|------|--------|
| `Admin.NET.Core` | 框架内核：实体基类、系统服务（Sys*）、枚举/常量/特性、SqlSugar 封装、种子数据 | **一般不动**；框架能力缺失时才改 |
| `Admin.NET.Application` | **业务层**：业务实体、业务服务、DTO、Configuration/*.json、SeedData | **业务代码主战场** |
| `Admin.NET.Web.Core` | Web 中间件：JWT、Swagger、CORS、限流、调度、`JwtHandler` | 仅改基础设施 |
| `Admin.NET.Web.Entry` | 启动入口 exe：`Program.cs`、`Controllers/`、wwwroot | 仅页面 Controller / 启动项 |
| `Admin.NET.Test` | 单元测试 | 测试 |
| `Plugins/Admin.NET.Plugin.*` | GoView、DingTalk、ReZero、K3Cloud、PaddleOCR、WorkWeixin、Ai、DataApproval | 插件式扩展 |

命名空间：`Admin.NET.Core` / `Admin.NET.Core.Service` / `Admin.NET.Application` / `Admin.NET.Web.Core` / `Admin.NET.Plugin.Xxx`。

---

## 2. 数据库与 MCP 连接

| 用途 | MCP 配置 | 默认库 | 说明 |
|------|----------|--------|------|
| **新系统（本项目）** | `mssql-ehr` | `ehr` | 对应 `Database.json` 的两个连接（主库 + `hr_2025`） |
| **老系统（对照）** | `ohr` | `tpk_hr` | 对应 `NHR.NClient` / `hr_2025` 遗留业务库 |

规则：

- 查新系统表结构、数据、验证结果 → 用 **mssql-ehr**（库 `ehr`）。
- 查老系统遗留表/业务含义、迁移前的数据核对 → 用 **ohr**（库 `tpk_hr`）。
- 需要表结构时先用 MCP 的 `describe_table` / `summarize_schema` 确认，**不要凭猜测写实体字段**。
- 若 MCP 连接失败（主机不可达/TLS 报错），先排查网络或 VPN；**不要擅自修改 `Database.json` 连接串**去"修"MCP 问题。

多库配置：`Admin.NET.Application/Configuration/Database.json`

- 第 1 个连接 = 默认主库（框架 `Sys*` 表，`ConfigId` 保持默认不写）。
- 第 2 个连接 `ConfigId: "hr_2025"` = 人事/遗留业务库（目前与主库同实例 `ehr`，仅逻辑分库）。
- 新增业务库：`Database.json` 加连接节点 + 在 `Admin.NET.Application/Const/ApplicationConst.cs` 加 `XxxDbConfigId` 常量，实体用 `[Tenant(常量)]` 引用，**禁止硬编码字符串**。

CodeFirst 开关（`DbSettings` / `TableSettings` / `SeedSettings`）当前全部为 `false`：遗留表绝不能被框架自动建/改。

---

## 3. 实体约定

基类全部在 `Admin.NET.Core/Entity/EntityBase.cs`：

| 基类 | 用途 |
|------|------|
| `EntityBaseId` | 仅 `Id`（long 雪花Id，主键，非自增） |
| `EntityBase` | + `CreateTime/UpdateTime/CreateUserId/CreateUserName/UpdateUserId/UpdateUserName` + `IsDelete`(软删除) + `DeleteTime` |
| `EntityBaseData` | + `CreateOrgId/CreateOrgName`（数据权限 `IOrgIdFilter`） |
| `EntityTenant` | `EntityBase` + `TenantId`（多租户） |

规则：

1. **新建业务表** → 继承 `EntityBase`（默认含软删除）；需要数据权限用 `EntityBaseData`。
2. 每个字段必须写 `[SugarColumn(ColumnDescription = "中文说明")]`，`ColumnDescription` 即数据库字段注释。
3. 表名：`[SugarTable("表名", "中文名")]`。
4. 文件位置：`Admin.NET.Application/Entity/`，文件名 PascalCase。
5. **遗留表对接**必须三件套（见第 6 节）。

---

## 4. 服务 / 接口约定

位置：`Admin.NET.Application/Service/<业务域>/`，DTO 放同级 `Dto/` 目录。

标准写法（参照 `Admin.NET.Application/Service/Hr/Org/HrOrgService.cs`）：

```csharp
namespace Admin.NET.Application;

/// <summary>
/// 人事组织（SysOrg 结构 + cc_mstr 扩展）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrOrg", Order = 471, Description = "人事组织")]
public class HrOrgService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CcMstr> _ccRep;

    public HrOrgService(SqlSugarRepository<CcMstr> ccRep) => _ccRep = ccRep;

    /// <summary>
    /// 增加机构并写入 cc_mstr
    /// </summary>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加人事机构")]
    public async Task<long> Add(HrOrgSaveInput input) { ... }
}
```

规则：

- 服务类实现 `IDynamicApiController, ITransient`；简单 CRUD 可继承 `BaseService<TEntity>`（`Admin.NET.Core/Service/BaseService.cs`，已提供 `GetDetail/GetList/Add/Update/Delete`）。
- 仓储：`SqlSugarRepository<T>` 构造注入，字段命名 `_xxxRep`。
- 方法命名：`Page` / `GetList` / `GetDetail` / `Add` / `Update` / `Delete`；写操作加 `[ApiDescriptionSettings(Name = "Add"), HttpPost]`。
- 每个对外方法必加 `[DisplayName("中文名")]`——它会自动注册为菜单按钮权限。
- 分页：入参继承 `BasePageInput`（`Page/PageSize/Field/Order`），返回 `SqlSugarPagedList<T>`；查询用 `_rep.AsQueryable().WhereIF(...)`。
- 业务异常统一 `throw Oops.Oh("提示语")` 或 `throw Oops.Oh(ErrorCodeEnum.D1002)`；不要用 `throw new Exception`。
- 跨表写入用 `[UnitOfWork]` 保证事务。
- 鉴权：已开启全局鉴权（`Web.Core/Startup.cs` 中 `enableGlobalAuthorize: true`），**默认所有接口需登录**；免登录接口显式加 `[AllowAnonymous]`。
- 绕过统一返回包装用 `[NonUnify]`。

---

## 5. 路由与统一返回值

- 动态 API 路由：`api/{小驼峰服务名}/{方法名}`，例：`/api/hrOrg/add`、`/api/sysUser/page`（`App.json`：`AsLowerCamelCase: true`、`SplitCamelCase: false`）。
- 统一返回体（`AdminNETResultProvider`）：

```json
{ "Code": 200, "Type": "success", "Message": "", "Result": {}, "Extras": {}, "Time": "" }
```

- 401 → "登录已过期，请重新登录"；403 → "禁止访问，没有权限"。
- 服务端口 `http://*:5005`（`Configuration/App.json`）；Swagger 由 `UseInject` 挂载，另有 Scalar UI `/sapi`。

---

## 6. 遗留表（hr_2025）接入模式 ⭐

现有业务模式是「**框架表存结构 + 遗留表作扩展，双写同步**」，新模块必须沿用：

```csharp
namespace Admin.NET.Application;

[Tenant(ApplicationConst.HrDbConfigId)]   // 指向 Database.json 的 hr_2025
[SugarTable("cc_mstr", "人事部门主档")]
[IgnoreTable]                              // 关键：不参与 CodeFirst 建表/改表
public class CcMstr { ... }                 // 遗留主键为 string，故不继承 EntityBase
```

- 遗留表一律：`[Tenant(ApplicationConst.HrDbConfigId)]` + `[SugarTable]` + `[IgnoreTable]`。
- 遗留表主键多为字符串（如 `cc_code`、`emp_id`），**不继承 `EntityBase`**，字段逐个按真实表结构手写。
- 已接入的遗留实体：`Entity/CcMstr.cs`（部门主档）、`Entity/CodeMstr.cs`（代码表）、`Entity/HrEmployee.cs`（员工 `employee`）。
- 已落地模块：`Service/Hr/Org/`（`HrOrgService` + `Dto/HrOrgDto.cs`），策略 = `SysOrg` 存结构树，`cc_mstr` 一对一扩展，`OrgId` 字段做双向映射。
- 后续模块（员工、考勤、薪酬、合同、保险…）统一放 `Admin.NET.Application/Service/Hr/<模块>/`，分组用 `ApplicationConst.GroupName`，`Order` 从 470+ 递增避免与框架冲突。

---

## 7. 配置与启动

- 入口 `Admin.NET.Web.Entry/appsettings.json` 只声明 `ConfigurationScanDirectories`，Furion 自动合并 `Admin.NET.Application/Configuration/` 下全部 json（`App.json`、`Database.json`、`JWT.json`、`Cache.json`、`Logging.json`、`Swagger.json`、`Upload.json` …，共 24 个）。
- 新配置项：在 `Configuration/` 加 json + 在 `Admin.NET.Application/Option/` 加对应 `XxxOptions.cs`，**不要塞进 `appsettings.json`**。
- 环境差异用 `Xxx.Development.json`。
- 启动（见根目录 `启动程序步骤`）：

```powershell
cd Admin.NET
dotnet run --project Admin.NET.Web.Entry/Admin.NET.Web.Entry.csproj --framework net10.0
# 前端：
cd Web && npm run dev
```

- 目标框架双目标 `net8.0;net10.0`；`dot_net_8.bat` / `dot_net_10.bat` 用于批量切换 csproj 目标框架（仅 .NET 8 SDK 环境时先跑 `dot_net_8.bat`），**日常不要手改 csproj**。

---

## 8. 老系统对照速查

| 老系统 | 形态 | 对照用途 |
|--------|------|----------|
| `NHR.NClient/` | .NET Framework 4.0 WinForms，`NHR.Model` / `NHR.DAL` / `NHR.BLL` + 业务模块（Employee、Attendance、Payroll、Contract、Insurance、HousingFund、Recruit、Staffing、Security、SelfInquiry…） | 查业务规则、SQL、报表逻辑（`.rdlc`）；库 `tpk_hr`（MCP `ohr`） |
| `src/` | PowerBuilder HRMS 4.0，菜单驱动，库 `hr_2025` | 查菜单/模块锚点 `sys_module_mstr.module_window`，详见 `rule/src-hrms-framework.md` |

迁移时的锚点：老系统模块 → 老表 → 在 `Admin.NET.Application` 建 `[IgnoreTable]` 实体 + 新服务。

---

## 9. 开发流程 Checklist

1. 用 **mssql-ehr** 确认目标表结构（字段、主键、类型、可空）。
2. 若是遗留表：查 **ohr**（`tpk_hr`）或老系统代码确认业务含义与旧逻辑。
3. 在 `Admin.NET.Application/Entity/` 建实体（遗留表三件套；新表继承 `EntityBase`）。
4. 在 `Admin.NET.Application/Service/Hr/<模块>/` 建服务 + `Dto/`。
5. 需要新库连接：`Database.json` + `ApplicationConst` 常量。
6. 需要菜单/权限：服务方法打 `[DisplayName(L.Get("中文菜单名"))]`，配合种子数据/菜单配置注册（文案走多语言，见第 11 节）。
7. 编译：`dotnet build`（或先跑 `dot_net_8.bat` 切换框架）。
8. 用 **mssql-ehr** 复核落库数据是否正确。
9. **多语言**：所有用户可见文案改 `L.Get("key")`，并在五语资源文件（`zh-CN`/`zh-TW`/`en`/`th`/`vi`）同步新增 key（详见第 11 节）。
10. **数据库变更必须出脚本**：任何建表/改表结构/改表记录的操作，都要生成 SQL 脚本并归档到 `doc/sql/`（命名、幂等等见第 12 节），禁止仅改实体而不落脚本。

---

## 10. 禁止事项

- ❌ 在 `NHR.NClient/`、`src/`、`Web/`、`GoView/` 中做本项目的功能开发。
- ❌ 对遗留表开启 `EnableInitTable` / `EnableDiffLog`，或去掉 `[IgnoreTable]`（会破坏生产表结构）。
- ❌ 在 `Admin.NET.Core` 里写业务代码（那里只放框架能力）。
- ❌ 硬编码数据库连接串或 `ConfigId` 字符串。
- ❌ 用 `throw new Exception` 抛业务错误、裸写 `try/catch` 吞异常。
- ❌ 在面向用户的提示/菜单/权限文案里写死中英文等字面量（`Oops.Oh("xxx")`、裸字符串 `DisplayName`）而不走 `L.Get`，导致新模块缺多语言。
- ❌ 未经确认擅自修改 `Admin.NET.Application/Configuration/` 下已有配置的开关值。

---

## 11. 多语言（i18n）强制要求 ⭐

EHR 面向泰国、越南等多国分支机构，**所有新模块、新功能（含给实体/DTO 新增字段、扩展表结构）都必须内置多语言**，不得写死中文/英文文案。框架已用 Furion 本地化（`L.Get` / `L.Text`），无需自建机制。

### 11.1 必须覆盖的五种语言

| 文化码 | 语言 | 资源文件 |
|--------|------|----------|
| `zh-CN` | 简体中文（默认） | `Admin.NET.Core/Resources/Lang.zh-CN.json`（已存在） |
| `zh-TW` | 繁体中文 | `Admin.NET.Core/Resources/Lang.zh-TW.json`（已创建，待译） |
| `en` | 英语 | `Admin.NET.Core/Resources/Lang.en.json`（已存在） |
| `th` | 泰语 | `Admin.NET.Core/Resources/Lang.th.json`（已创建，待译） |
| `vi` | 越南语 | `Admin.NET.Core/Resources/Lang.vi.json`（已创建，待译） |

- 支持语言在 `Admin.NET.Application/Configuration/App.json` 的 `LocalizationSettings.SupportedCultures` 中登记（已含上述五语）。
- 新增第六种语言：在 `SupportedCultures` 加文化码 + 在 `Admin.NET.Core/Resources/` 新建 `Lang.{culture}.json` + 补齐该文件的全部 key。**不在此清单外的语言上做硬编码文案**。
- 客户端通过 `Accept-Language` 头或 `?culture=xx` 切换；`L.SetCulture("th")` 可强制设定。

### 11.2 必须走多语言的文案范围

凡面向用户/前端的字符串，**一律用 key 取词，禁止字符串字面量**：

1. 业务异常提示：`throw Oops.Oh(L.Get("员工不存在"));`（不要 `Oops.Oh("员工不存在")`）。
2. 服务/接口/按钮名：`[DisplayName(L.Get("增加人事机构"))]`；菜单、权限点名同此。
3. 返回给前端的 `Message`、状态描述、校验失败文案。
4. 枚举 `Description`、字典显示名等可本地化字段。
5. 种子数据（菜单、字典）中的展示文案，用 key 而非原文。
6. **新增字段（实体/DTO 加列）**：字段在界面上的列名、表单标签、占位符、下拉项、校验失败提示（如"xxx 不能为空""xxx 格式不正确"）均属用户可见文案，必须用 `L.Get("key")`，并在五语文件同步加 key。`[SugarColumn(ColumnDescription = "中文说明")]` 仅作数据库注释（中文即可），**不等于 UI 显示名**，UI 显示名仍需独立本地化。

### 11.3 资源文件维护规则

- key 统一用英文/中文语义串，**五语文件必须同步新增同一 key**，缺译会回退默认文化（`zh-CN`），但不能漏建 key。
- 带参文案保持占位符一致：`"员工{0}不存在"` → 各语言 `"Employee {0} not found"` / `"พนักงาน{0}ไม่พบ"` / `"Nhân viên {0} không tồn tại"`。
- 新增 key 后**五份 json 都要加**；可在 `Admin.NET.Application/Service/Test/TestService.cs` 的 `GetCulture` 用 `L.Text["key"]` 自测取值。
- 资源文件位于 `Admin.NET.Core/Resources/`，**不要在 `Admin.NET.Application` 或别处另建本地化目录**。

### 11.4 Checklist 增补（新模块交付前必须完成）

- 所有用户可见文案（含新增字段的列名/表单标签/校验提示）已改为 `L.Get("key")`。
- `Lang.zh-CN.json` / `Lang.zh-TW.json` / `Lang.en.json` / `Lang.th.json` / `Lang.vi.json` 五份均已新增对应 key，且占位符一致。
- 用 `?culture=th`、`?culture=vi`、`?culture=zh-TW` 各验证一次关键接口文案正确性（重点核对新增字段的标签与校验提示）。

---

## 12. 数据库变更 SQL 脚本管理规则 ⭐

**设计/开发过程中，凡是涉及数据库表结构的变更或表记录的写操作，都必须生成可执行的 SQL 脚本并归档**，不得仅停留在代码层（CodeFirst 当前全 `false`，实体不会自动落库）。

适用场景（满足任一即需生成脚本）：

- **新增表**（新业务表 / 遗留对接表）：`CREATE TABLE` + 索引 + 字段注释。
- **修改表结构**：`ALTER TABLE` 增加/修改/删除字段、改类型/可空/默认值、加索引、改注释。
- **修改表记录**：`INSERT`/`UPDATE`/`DELETE` 业务数据、字典/种子数据、配置记录、迁徙/回填数据。
- **其他 DDL/DCL**：建视图、存储过程、函数、约束、权限等。

### 12.1 脚本存放位置

| 项 | 路径 |
|----|------|
| 脚本根目录 | `d:/project/ehr/ehr/doc/sql/`（已存在，按子目录分类） |

- 按业务域分子目录（如 `doc/sql/hr/`、`doc/sql/sys/`、`doc/sql/seed/`），无明确域时放根目录。
- 严禁把 SQL 直接写进 issue 评论或聊天记录而不归档文件。

### 12.2 文件命名规范

```
{YYYYMMDD}_{模块}_{动作}[_{序号}].sql
```

- `YYYYMMDD`：生成日期；`模块`：业务域（如 `hrOrg`、`employee`、`payroll`）；`动作`：`create` / `addColumn` / `alterColumn` / `dropColumn` / `seed` / `update` / `delete` / `view` 等。
- 同日同模块多段变更加 `_01`/`_02` 序号。
- 例：`20261009_hrOrg_addColumn_status.sql`、`20261009_hr_seed_dictionary_01.sql`。

### 12.3 脚本内容要求

1. 脚本头部必须带注释头，便于追溯：

   ```sql
   /*
    * 变更说明：人事机构表 cc_mstr 增加状态字段
    * 模块：hrOrg
    * 日期：2026-10-09
    * 关联需求/Issue：#123
    * 影响库：ehr（hr_2025）
    * 是否幂等：是
    */
   ```

2. **幂等优先**：尽量用 `IF NOT EXISTS` / `IF OBJECT_ID(...) IS NULL` / `IF COL_LENGTH(...) IS NULL` 包裹，保证可重复执行不报错（尤其种子数据先查后插/存在则更新）。
3. **可回滚**：复杂/不可逆变更（删字段、删表、改类型）建议在脚本末尾附 `-- ROLLBACK:` 反向 SQL 注释，或单独提供 `*_rollback.sql`。
4. **可执行**：纯 SQL，不依赖 IDE 专属语法；字段类型、长度、可空、默认值、注释与实体 `[SugarColumn]` 保持一致。
5. **跨库标注**：涉及 `hr_2025` 的语句显式带库名前缀 `ehr.hr_2025.dbo.xxx`，避免执行时选错库。
6. 生产敏感操作（清数、改主键、大量 UPDATE）需单独醒目注释 `[WARNING]` 并提示需人工复核。

### 12.4 执行与校验流程

1. 生成脚本 → 放入 `doc/sql/`（按 12.1/12.2 命名）。
2. 用 **mssql-ehr**（库 `ehr`）在测试环境先执行，确认无报错、影响行数符合预期。
3. 用 `describe_table` / `read_data` 复核表结构与数据。
4. 实体/服务代码与脚本保持同步：改了实体字段就必须补对应 `ALTER` 脚本，反之亦然。
5. 严禁跳过脚本直接手改线上库结构；上线以归档脚本为准。

### 12.5 Checklist 增补

- 本次变更涉及表结构/记录 → 已生成 SQL 脚本并存于 `doc/sql/`，命名合规。
- 脚本含注释头、幂等判断、跨库前缀、可回滚说明。
- 已在测试库用 mssql-ehr 实际执行并复核通过。
- 实体 `[SugarColumn]` 与脚本字段定义一致。
