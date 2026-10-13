# 方案：基于 `cc_mstr` 提取与 `SysOrg` 同形视图

> 日期：2026-10-12（修订：Id/Pid=`ID`/`ParentID`；DeleteTime=`cc_dele_date`；IsDelete=`cc_enable`；不加 `sys_id`/`sys_pid`/`delete_time`/`is_delete`）  
> 范围：`ehr.dbo.cc_mstr` → 视图（字段与 `ehr.dbo.SysOrg` 对齐）  
> 原则：能映射的用现有列；映射不上的在 `cc_mstr` **新增物理列**，再由视图以 `SysOrg` 列名暴露。  
> 说明：本方案是**读形状适配**（视图层同形），**不**双写框架表 `SysOrg`，也**不**替代人事主档 `cc_mstr`。

---

## 1. 目标与边界

### 1.1 目标

1. 在 `ehr` 库创建视图（正式名：`dbo.V_SysOrg`），列名、类型与 `SysOrg` **一致**（见第 3 节对照表）。  
2. 数据源唯一为 `dbo.cc_mstr`（可自连接解析父节点）。  
3. 凡 `cc_mstr` 无法稳定映射到 `SysOrg` 的字段，先对 `cc_mstr` 做 DDL 增列，再纳入视图。

### 1.2 非目标

| 不做 | 原因 |
|------|------|
| 往 `SysOrg` 表插入/同步 | 主档仍是 `cc_mstr`；避免双源 |
| 替换 `HrOrgService` 写路径 | 增删改仍写 `cc_mstr`；视图只读 |
| 把 `SysOrg` 实体直接挂到视图上做写操作 | 视图不可更新（含计算列/CASE） |
| 新增 `sys_id` / `sys_pid` | **已定**：`ID`→`Id`，`ParentID`→`Pid` |
| 新增 `delete_time` | **已定**：`cc_dele_date`→`DeleteTime` |
| 新增 `is_delete` | **已定**：`cc_enable`→`IsDelete`（0→0，1→1） |

### 1.3 推荐用途

- 需要「机构树同形」的只读消费方（报表、选人机构树适配、跨模块只读联查）。  
- 评估框架机构 UI/权限与人事部门数据的字段差距。

---

## 2. 现状快照（MCP 实测 2026-10-12）

### 2.1 `SysOrg`（21 列）

| 列 | 类型 | 说明 |
|----|------|------|
| Id | bigint PK | 雪花 Id（本视图用 `cc_mstr.ID` 提升为 bigint） |
| Pid | bigint | 父 Id，根为 0（本视图用 `ParentID`） |
| Name | nvarchar(64/128) | 名称 |
| Code | nvarchar(64/128) | 编码 |
| Level | int | 级别 |
| Type | nvarchar | 机构类型（字典，如 101/201/301） |
| DirectorId | bigint | 负责人用户 Id |
| Longitude / Latitude | decimal(18,4) | 经纬度，默认 0 |
| OrderNo | int | 排序 |
| Status | int | `StatusEnum`：1 启用 / 2 停用 |
| Remark | nvarchar | 备注 |
| TenantId | bigint | 租户 |
| CreateTime / UpdateTime | datetime | 审计时间 |
| CreateUserId / UpdateUserId | bigint | 审计用户 Id |
| CreateUserName / UpdateUserName | nvarchar | 审计用户名 |
| IsDelete | bit | 软删 |
| DeleteTime | datetime | 软删时间 |

### 2.2 `cc_mstr`（32 列，无 SysOrg 专用列）

业务主键 `cc_code`；树靠 `cc_level` / `ParentID` / `cc_code_parent`；停用靠 `cc_enable`（0 启用 / 1 停用）；辅助键 `ID`（identity）/ `ParentID`。

---

## 3. 字段对照与处置

图例：

- **直接映射**：现有列可 1:1 或简单 CAST  
- **派生映射**：可用表达式计算，可不增列  
- **必须增列**：无可靠来源，或类型/语义无法用表达式稳定表达  

### 3.1 对照总表

| SysOrg 列 | 类型期望 | cc_mstr 来源 | 处置 | 视图表达式（草案） |
|-----------|----------|--------------|------|-------------------|
| Id | bigint | `ID` (int identity) | **直接映射** | `CAST(ID AS bigint)` |
| Pid | bigint | `ParentID` (int) | **直接映射** | `CAST(ISNULL(ParentID, 0) AS bigint)` |
| Name | nvarchar | `cc_desc` | 直接映射 | `LEFT(cc_desc, 64)` 或扩长后原样 |
| Code | nvarchar | `cc_code` | 直接映射 | `cc_code` |
| Level | int | `cc_level` | 派生映射 | `LEN(cc_level) / 2` |
| Type | nvarchar | 无 | **必须增列** `org_type` | `org_type` |
| DirectorId | bigint | 无 | **必须增列** `director_id` | `director_id` |
| Longitude | decimal | 无 | **必须增列** `longitude` | `ISNULL(longitude, 0)` |
| Latitude | decimal | 无 | **必须增列** `latitude` | `ISNULL(latitude, 0)` |
| OrderNo | int | 无（勿用 `cc_group`） | **必须增列** `order_no` | `ISNULL(order_no, 100)` |
| Status | int | `cc_enable` | 派生映射 | `CASE WHEN ISNULL(cc_enable,0)=0 THEN 1 ELSE 2 END` |
| Remark | nvarchar | `cc_dele_remark` 语义偏异动 | **必须增列** `remark` | `remark`（异动备注仍留 `cc_dele_remark`） |
| TenantId | bigint | 无 | **必须增列** `tenant_id` | `tenant_id` |
| CreateTime | datetime | `cc_create_date` | 直接映射 | `cc_create_date` |
| UpdateTime | datetime | `cc_date` | 直接映射 | `cc_date` |
| CreateUserId | bigint | 无（`cc_user` 为人名） | **必须增列** `create_user_id` | `create_user_id` |
| CreateUserName | nvarchar | `cc_user` | 直接映射 | `cc_user`（创建时写入；更新可另记） |
| UpdateUserId | bigint | 无 | **必须增列** `update_user_id` | `update_user_id` |
| UpdateUserName | nvarchar | `cc_user`（维护人） | 派生 + 增列可选 | 优先 `update_user_name`；空则回退 `cc_user` |
| IsDelete | bit | `cc_enable` | **直接映射** | `CAST(ISNULL(cc_enable, 0) AS bit)`（0 启用→0，1 停用→1；**不加** `is_delete`） |
| DeleteTime | datetime | `cc_dele_date` | **直接映射** | `cc_dele_date`（**不加** `delete_time`） |

### 3.2 Id / Pid 约定（已定）

| SysOrg | cc_mstr | 说明 |
|--------|---------|------|
| Id | `ID` | 视图 `CAST(ID AS bigint)`；**不加** `sys_id` |
| Pid | `ParentID` | 视图 `CAST(ISNULL(ParentID, 0) AS bigint)`；根为 `0`；**不加** `sys_pid` |

树父子关系沿用现有 `ParentID` → 父行 `ID`，与 `SysOrg.Pid` → `SysOrg.Id` 同构。

注意：`ID`/`ParentID` 为 int，视图提升为 bigint 仅满足列类型对齐；消费方勿与框架表 `SysOrg` 的雪花 Id 混用、互为 FK。

### 3.3 Status / IsDelete / DeleteTime（已定）

`cc_enable` 同时派生 `Status` 与 `IsDelete`；**不加** `is_delete` 列。

| 业务动作 | `cc_enable` | `Status`（视图） | `IsDelete`（视图） | `DeleteTime`（=`cc_dele_date`） |
|----------|-------------|------------------|-------------------|--------------------------------|
| 启用 | 0 | 1 (Enable) | 0 | NULL |
| 停用 / 软删 | 1 | 2 (Disable) | 1 | 写入 `cc_dele_date` |

约定：`IsDelete` ↔ `cc_enable`（0→false，1→true）；`Status` 仍由同一列派生；`DeleteTime` ↔ `cc_dele_date`。

### 3.4 不建议硬映射的列

| 诱惑映射 | 为何不做 |
|----------|----------|
| `OrderNo` ← `cc_group` | `cc_group` 是业务「部门组」，非排序 |
| `Type` ← `cc_level` 长度 | 长度是树深，不是机构类型字典 |
| `Remark` ← `cc_dele_remark` | 多为合并/删除说明，污染日常备注 |
| `CreateUserId` ← 解析 `cc_user` | 登录名/姓名不稳定，无法可靠反查用户 Id |

---

## 4. 对 `cc_mstr` 的增列清单（DDL）

### 4.1 新增列定义（10 列）

| 新列名 | 类型 | 可空 | 默认 | 对应 SysOrg |
|--------|------|------|------|-------------|
| org_type | nvarchar(64) | NULL | N'201' | Type |
| director_id | bigint | NULL | NULL | DirectorId |
| longitude | decimal(18,4) | NOT NULL | 0 | Longitude |
| latitude | decimal(18,4) | NOT NULL | 0 | Latitude |
| order_no | int | NOT NULL | 100 | OrderNo |
| remark | nvarchar(128) | NULL | NULL | Remark |
| tenant_id | bigint | NULL | 当前租户 | TenantId |
| create_user_id | bigint | NULL | NULL | CreateUserId |
| update_user_id | bigint | NULL | NULL | UpdateUserId |
| update_user_name | nvarchar(64) | NULL | NULL | UpdateUserName |

> **不加** `sys_id` / `sys_pid` / `delete_time` / `is_delete`。主键/父键沿用 `ID` / `ParentID`；删除标记/时间沿用 `cc_enable` / `cc_dele_date`。

### 4.2 建议 DDL（幂等草案）

```sql
/*
 * 模块：组织视图
 * 说明：为 V_SysOrg 补齐 SysOrg 同形所需物理列
 * （不含 Id/Pid/IsDelete/DeleteTime：沿用 ID/ParentID/cc_enable/cc_dele_date）
 * 库：ehr
 */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @t INT = OBJECT_ID(N'dbo.cc_mstr');
IF @t IS NULL THROW 50001, N'cc_mstr 不存在', 1;

IF COL_LENGTH(N'dbo.cc_mstr', N'org_type') IS NULL
    ALTER TABLE dbo.cc_mstr ADD org_type nvarchar(64) NULL
        CONSTRAINT DF_cc_mstr_org_type DEFAULT (N'201');
IF COL_LENGTH(N'dbo.cc_mstr', N'director_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD director_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'longitude') IS NULL
    ALTER TABLE dbo.cc_mstr ADD longitude decimal(18,4) NOT NULL
        CONSTRAINT DF_cc_mstr_longitude DEFAULT (0);
IF COL_LENGTH(N'dbo.cc_mstr', N'latitude') IS NULL
    ALTER TABLE dbo.cc_mstr ADD latitude decimal(18,4) NOT NULL
        CONSTRAINT DF_cc_mstr_latitude DEFAULT (0);
IF COL_LENGTH(N'dbo.cc_mstr', N'order_no') IS NULL
    ALTER TABLE dbo.cc_mstr ADD order_no int NOT NULL
        CONSTRAINT DF_cc_mstr_order_no DEFAULT (100);
IF COL_LENGTH(N'dbo.cc_mstr', N'remark') IS NULL
    ALTER TABLE dbo.cc_mstr ADD remark nvarchar(128) NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'tenant_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD tenant_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'create_user_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD create_user_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'update_user_id') IS NULL
    ALTER TABLE dbo.cc_mstr ADD update_user_id bigint NULL;
IF COL_LENGTH(N'dbo.cc_mstr', N'update_user_name') IS NULL
    ALTER TABLE dbo.cc_mstr ADD update_user_name nvarchar(64) NULL;

COMMIT TRAN;
```

### 4.3 回填策略

1. **Id / Pid**：无需回填；视图直接读 `ID` / `ParentID`。确保根节点 `ParentID` 为 `NULL` 或 `0`（视图统一成 `0`）。  
2. **tenant_id**：统一写入当前租户 Id。  
3. **org_type / order_no**：按默认值即可；后续在组织编辑页维护。

```sql
-- 可选：统一根 ParentID
UPDATE dbo.cc_mstr
SET ParentID = 0
WHERE ParentID IS NULL;
```

---

## 5. 视图定义

### 5.1 正式视图（依赖增列完成后）

```sql
CREATE OR ALTER VIEW dbo.V_SysOrg
AS
SELECT
    CAST(c.ID AS bigint)                                        AS Id,
    CAST(ISNULL(c.ParentID, 0) AS bigint)                       AS Pid,
    CAST(LEFT(c.cc_desc, 64) AS nvarchar(64))                   AS Name,
    CAST(c.cc_code AS nvarchar(64))                             AS Code,
    CAST(LEN(c.cc_level) / 2 AS int)                            AS Level,
    CAST(ISNULL(c.org_type, N'201') AS nvarchar(64))            AS Type,
    c.director_id                                               AS DirectorId,
    ISNULL(c.longitude, CAST(0 AS decimal(18,4)))               AS Longitude,
    ISNULL(c.latitude,  CAST(0 AS decimal(18,4)))               AS Latitude,
    ISNULL(c.order_no, 100)                                     AS OrderNo,
    CASE WHEN ISNULL(c.cc_enable, 0) = 0 THEN 1 ELSE 2 END      AS Status,
    CAST(c.remark AS nvarchar(128))                             AS Remark,
    c.tenant_id                                                 AS TenantId,
    c.cc_create_date                                            AS CreateTime,
    c.cc_date                                                   AS UpdateTime,
    c.create_user_id                                            AS CreateUserId,
    CAST(c.cc_user AS nvarchar(64))                             AS CreateUserName,
    c.update_user_id                                            AS UpdateUserId,
    CAST(ISNULL(c.update_user_name, c.cc_user) AS nvarchar(64)) AS UpdateUserName,
    CAST(ISNULL(c.cc_enable, 0) AS bit)                         AS IsDelete,
    c.cc_dele_date                                              AS DeleteTime
FROM dbo.cc_mstr AS c;
GO
```

### 5.2 形状校验 SQL

```sql
-- 列名集合应与 SysOrg 一致（忽略列序可用 EXCEPT 双向）
SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.SysOrg')
EXCEPT
SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.V_SysOrg');

SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.V_SysOrg')
EXCEPT
SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.SysOrg');
-- 两边均应 0 行
```

---

## 6. 应用层落地步骤

| 步骤 | 内容 | 产出 |
|------|------|------|
| 1 | 评审本方案（Id=`ID`、Pid=`ParentID`、IsDelete=`cc_enable`） | 确认 |
| 2 | 执行增列 DDL + 回填 `tenant_id` 等默认值 | SQL 脚本入 `doc/sql/hr-org/` |
| 3 | 创建 `V_SysOrg` | 同上 |
| 4 | 实体 `CcMstr` 增加对应属性；`HrOrgService` Add/Update 维护 `order_no` 等 | 后端 |
| 5 | （可选）只读仓储映射视图，供需要 SysOrg 同形的查询使用 | 后端只读 |
| 6 | 验收：形状校验 + 树 `Pid`←`ParentID` 可拼 + Status 与启用停用一致 | 清单 |

### 6.1 写路径约定（增列后）

| 操作 | 额外维护 |
|------|----------|
| Add | identity 生成 `ID`；`ParentID`=父 `ID` 或 0；默认 `org_type/order_no/tenant_id`；写 `create_user_id` + `cc_user` |
| Update | 写 `update_user_id` / `update_user_name` / `cc_date`；可改 `remark`/`director_id`/`order_no`/`org_type` |
| Delete（停用） | `cc_enable=1` + `cc_dele_*`（视图即 `IsDelete=1`、`DeleteTime=cc_dele_date`） |
| Transfer / Merge | 子树重挂时同步更新各行 `ParentID`（沿用现有树逻辑即可） |

---

## 7. 与现行组织方案的关系

| 文档 | 关系 |
|------|------|
| `组织架构/migration-plan` | 人事主档仍是 `cc_mstr`，**不**双写 `SysOrg` 表 |
| 本文 | 在 `cc_mstr` 上补同形列 + **视图**暴露，供只读同形消费；不改变「主档唯一」原则 |

若产品最终决定「框架机构与人事部门彻底隔离、连同形视图也不需要」，则仅保留对照，不执行增列。

---

## 8. 风险与缓解

| 风险 | 等级 | 缓解 |
|------|------|------|
| 视图 Id 与真实 `SysOrg` 雪花 Id 混用 | 高 | 命名区分 `V_SysOrg`；禁止对本视图 Id 建指向 `SysOrg` 的 FK |
| `ParentID` 空值导致树根不一致 | 中 | 视图 `ISNULL(ParentID,0)`；可选回填根为 0 |
| `Name` 截断（`cc_desc` 最长 122） | 中 | 视图 `LEFT(...,64)`；或评估放宽视图/消费方长度 |
| 增列扩大 `cc_mstr` 宽度 | 低 | 列均可空/有默认，业务路径渐进写入 |

---

## 9. 验收清单

- [ ] `cc_mstr` 已含 §4.1 全部 **10** 个新列（无 `sys_id`/`sys_pid`/`delete_time`/`is_delete`）  
- [ ] 视图 `Id`=`ID`，`Pid`=`ParentID`，`IsDelete`=`cc_enable`，`DeleteTime`=`cc_dele_date`  
- [ ] 根 `Pid=0`；非根 `Pid` 等于父行 `Id`（即父 `ID`）  
- [ ] `V_SysOrg` 与 `SysOrg` 列名双向 EXCEPT 为空  
- [ ] `Status`：启用=1，停用=`cc_enable=1`→2；`IsDelete`：启用=0，停用=1  
- [ ] Add/Update/Transfer 后视图树可还原  

---

## 10. 建议文件落地位置（实施时）

| 文件 | 用途 |
|------|------|
| `doc/sql/hr-org/20261012_v_sysorg.sql` | 增列（10 列）+ 正式视图 `V_SysOrg` |
| `doc/分析文档/组织视图/cc_mstr_to_sysorg_view_plan.md` | 本方案（本文件） |

---

## 附录 A. 增列一览（速查）

必须新增（**10** 列）：

`org_type`, `director_id`, `longitude`, `latitude`, `order_no`, `remark`, `tenant_id`, `create_user_id`, `update_user_id`, `update_user_name`

可直接/派生映射（无需增列）：

| SysOrg | cc_mstr |
|--------|---------|
| Id | `ID` |
| Pid | `ParentID` |
| Name | `cc_desc` |
| Code | `cc_code` |
| Level | `LEN(cc_level)/2` |
| Status | `cc_enable`（0→1，1→2） |
| IsDelete | `cc_enable`（0→0，1→1） |
| CreateTime | `cc_create_date` |
| UpdateTime | `cc_date` |
| CreateUserName | `cc_user` |
| DeleteTime | `cc_dele_date` |

## 附录 B. 新增列 ↔ SysOrg 对照

| cc_mstr 新列 | SysOrg 字段 |
|--------------|-------------|
| org_type | Type |
| director_id | DirectorId |
| longitude | Longitude |
| latitude | Latitude |
| order_no | OrderNo |
| remark | Remark |
| tenant_id | TenantId |
| create_user_id | CreateUserId |
| update_user_id | UpdateUserId |
| update_user_name | UpdateUserName |

## 附录 C. 实体参考路径

| 对象 | 路径 |
|------|------|
| SysOrg | `Admin.NET/Admin.NET.Core/Entity/SysOrg.cs` + `EntityBase`/`EntityTenant` |
| CcMstr | `Admin.NET/Admin.NET.Application/Entity/CcMstr.cs` |
| StatusEnum | `Admin.NET/Admin.NET.Core/Enum/StatusEnum.cs`（1/2） |
