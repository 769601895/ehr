# 设计方案：用户管辖部门（Usercc）

> 状态：设计稿（实施前依据）  
> 日期：2026-10-12（修订：结构/编码变**不改写** scope；表名 `Usercc`）  
> 所属：组织架构二期（Roadmap P6）  
> 关联：`domains/01-domain-hr-org.md`、`ehr.dbo.cc_mstr`

---

## 1. 问题与约束

| 项 | 说明 |
|----|------|
| 目标 | 为用户配置「可管辖的部门范围」，并据此过滤部门树、员工及相关 HR 数据 |
| 部门规模 | 约 80 万，且可能继续增长 |
| 部门主档 | `ehr.dbo.cc_mstr`：`cc_code`（内码，稳定业务主键）、`cc_desc`、`cc_level`（层级码，可随调动重算）等 |
| 层级规则 | `cc_level` 每层固定 2 位：如 `01` → `0101` → `010101` → `01010101` |
| 组织变动 | 部门会持续增删改（含停用、合并、调动）；**`Usercc` 在结构/编码变更时不改写** |
| 与 SysOrg | **零关联**；不使用框架 `SysOrg` / `SysRoleOrg` / `GetUserOrgIdList` 展开 Id 列表 |

**禁止**：把用户可见部门展开成数十万行写入授权表或登录缓存。

---

## 2. 设计原则

1. **授权挂稳定部门内码，不挂会变的层级码**  
   表中只存「管辖根」的 `CcCode`（+ 是否含下级）。  
   `cc_level` **仅在查询时**从当前 `cc_mstr` 解析，用于前缀匹配。

2. **用「当前」`cc_level` 前缀匹配做范围**  
   用户授了部门 `CcCode=X` 且含下级：先取 `X` 的**当前** `cc_level`（如调动后已是 `0201`），再 `LIKE 当前level%`。  
   新增落在该前缀下的部门**自动纳入**。

3. **结构/编码变更不改写 Usercc**  
   调动、子树重算 `cc_level`、改父级等：**不 UPDATE / DELETE `Usercc`**。  
   权限语义是「仍管辖这些部门节点（及当时勾选的含下级规则）」；节点跟着组织树走，范围跟着**当前树形**走。

4. **失效靠主档，不靠级联改授权表**  
   合并停用、删除导致 `CcCode` 在主档不存在或不可用时：该行保留亦可，**查询时 JOIN 不到有效部门则该根不生效**；由管理员在授权页清理或改授。  
   组织服务**不**自动改挂、不自动删 `Usercc`。

5. **与菜单权限分离**  
   菜单/按钮仍走现有角色权限；本方案只解决 **HR 数据范围（部门维度）**。

6. **产品叙事按新系统编写**  
   不写 Legacy / 兼容 PB / SyncFrom 等迁移型痕迹。旧库 `hr_sys_usercc` 仅溯源，非实现契约。

---

## 3. 数据模型

### 3.1 表：`ehr.dbo.Usercc`

| 列 | 类型 | 说明 |
|----|------|------|
| `Id` | `BIGINT` IDENTITY | 主键 |
| `UserId` | `BIGINT` | 系统用户 Id（`SysUser.Id`） |
| `CcCode` | `NVARCHAR(50)` | **管辖根部门内码**（授权主键，稳定） |
| `IncludeChild` | `BIT` | `1`=本部门及以下（默认）；`0`=仅本部门 |
| `CreateTime` | `DATETIME2` | 可选 |
| `CreateUserId` | `BIGINT` | 可选 |

约束与索引：

- `UNIQUE (UserId, CcCode)`
- `INDEX IX_Usercc_UserId (UserId)`
- `INDEX IX_Usercc_CcCode (CcCode)`（授权页反查、巡检）
- `cc_mstr` 侧：`PK(cc_code)`、`IX_cc_mstr_cc_level`；员工侧：`h_emp_mstr.cc_code`

**不存 `CcLevel`。** 避免与主档不一致；过滤一律现查。

### 3.2 语义（解析后）

设根部门当前行为 `root`（`cc_mstr.cc_code = Usercc.CcCode`）：

| `IncludeChild` | 含义 |
|----------------|------|
| `1` | 目标部门 `cc_level` 等于 `root.cc_level`，或以 `root.cc_level` 为前缀 |
| `0` | 目标部门 `cc_level` 等于 `root.cc_level`（或等价：`目标.cc_code = 根.CcCode`） |

根在主档不存在 / 已停用（按产品是否允许管停用部门）：该行**跳过**，不参与 OR 条件。

### 3.3 可选扩展（二期以后）

- 角色级管辖表（若需要）：最终范围 = 用户 ∪ 角色。  
- 排除子树：一期不做。

### 3.4 能力分层（产品）

| 模式 | 存储 | 适用 |
|------|------|------|
| 全部数据 | 超管/特权角色 | 系统管理员 |
| 指定根及以下 | 若干行，`IncludeChild=1` | 主路径 |
| 仅本部门 | `IncludeChild=0` | 精确授权 |
| 仅本人 | 不按部门 scope | 员工自助 |

---

## 4. 写入规则：按当前树压缩（仅保存授权时）

仅在**保存用户管辖**时压缩；组织 Transfer/Merge **不触发**压缩、不改写 `Usercc`。

压缩依据：各 `CcCode` 在**当前** `cc_mstr` 上的 `cc_level` 祖先关系。

1. 已有祖先根 A，再选子孙 B → 丢弃 B。  
2. 已有多个子孙，再选共同祖先 → 删子孙，只留祖先。  
3. 同一 `CcCode` 只一行。  
4. 保存时若某 `CcCode` 主档不存在 → 拒绝或剔除并提示。

组织调动后，可能暂时出现「父、子根同时存在」的冗余行：允许；下次保存该用户授权时再压缩，或不做也可（查询结果仍正确，仅多几个 OR）。

---

## 5. 运行时过滤

### 5.1 解析当前根 level（每次过滤的前置）

```text
roots = Usercc 行
effective =
  FOR EACH root IN roots
    JOIN cc_mstr ON cc_code = root.CcCode
    [AND 启用条件，按产品]
    → { ccCode, ccLevel: 当前值, includeChild }
```

### 5.2 会话缓存

可缓存：

```text
KeyUserCc:{userId} → [{ ccCode, includeChild }, ...]   -- 仅授权表内容
```

**不建议**长期缓存「已解析的 ccLevel」而不失效：调动后 level 会变。可选策略：

| 策略 | 说明 |
|------|------|
| A. 不缓存 level | 每次查询 JOIN `cc_mstr` 取当前 level（推荐默认） |
| B. 短 TTL 缓存 level | 组织结构变更后**清全部**解析缓存（只清缓存，不改 `Usercc`） |

**禁止**缓存展开后的全量部门列表。

### 5.3 查询模式

```sql
WHERE EXISTS (
  SELECT 1
  FROM Usercc s
  INNER JOIN cc_mstr root ON root.cc_code = s.CcCode
    -- AND root.cc_enable = 0   -- 按产品
  WHERE s.UserId = @uid
    AND (
      (s.IncludeChild = 1 AND c.cc_level LIKE root.cc_level + N'%')
      OR (s.IncludeChild = 0 AND c.cc_level = root.cc_level)
    )
)
```

员工：`h_emp_mstr` → `cc_mstr`（员工部门）再与上式同一套 root 解析。

单点校验：用目标部门当前 `cc_level`，对 `effective` 根做前缀/相等判断。

### 5.4 与框架 DataScope

不使用 `GetUserOrgIdList` 展开；HR 走 `Usercc` + 当前 `cc_level` 前缀。

---

## 6. 部门变动与 Usercc 的关系

### 6.1 总原则（已确认）

| 动作 | 是否改写 `Usercc` |
|------|-------------------|
| 改名称/属性 | **否** |
| 新增下级 | **否**（自动纳入） |
| 停用 / 软删 | **否**（查询侧决定是否仍生效） |
| **调动 / 改父级 / 子树重算 `cc_level`** | **否** |
| **合并** | **否**（源停用后对应根自然不生效，直至管理员改授） |
| **物理删除部门**（若发生） | **否**（行可残留；查询 JOIN 不到则无效；授权页/巡检清理） |

组织 API 路径：

```text
Add / Update / Disable / Transfer / Merge
         │
         ├─ 写 cc_mstr（及 Merge 时人员 cc_code）
         ├─ ✕ 不写 Usercc
         └─ 可选：失效「已解析 level」类缓存（若采用策略 B）
```

### 6.2 调动为何仍正确（不改 Usercc）

```text
授权：User → CcCode=生产部（内码固定）
调动前：生产部 cc_level=0101 → 可见 0101%
调动后：生产部 cc_level=0201（Usercc 行未改）
查询：JOIN 得当前 0201 → 可见 0201%
```

语义：**管辖的是「这个部门节点及其（规则下的）下级」**，不是「永远管辖字符串 0101 这片编码」。

### 6.3 合并 / 停用 / 删除后的产品行为

| 场景 | 行为 |
|------|------|
| 授了 A，A 合并进 B 且 A 停用 | 仍指向 A；过滤时 A 无效 → **失去该根权限**；需管理员为用户改授 B（或其它根） |
| 授了 A 的下级 L，仅 A 合并、L 仍在 | 仍按 L 的当前 level 生效 |
| 停用授权根 | 默认不参与过滤；授权页标「主档停用/无效」 |
| 残留悬空 `CcCode` | 不生效；巡检列出，支持批量删除无效行 |

**不在** Merge/Disable/Transfer 事务内自动改挂或删除 `Usercc`。

### 6.4 缓存失效（仅缓存，非表）

| 事件 | 动作 |
|------|------|
| 修改某用户授权 | 删该用户授权缓存 |
| Transfer / 大批量改 `cc_level` | 若缓存了解析后的 level → 清空解析缓存；**不改 `Usercc`** |
| 仅新增下级 / 改名 | 可不清授权缓存 |

### 6.5 巡检（治理，非级联写）

```sql
-- 悬空：主档无此部门
SELECT s.*
FROM Usercc s
LEFT JOIN cc_mstr c ON c.cc_code = s.CcCode
WHERE c.cc_code IS NULL;

-- 主档已停用（若产品视停用为无效根）
SELECT s.*
FROM Usercc s
JOIN cc_mstr c ON c.cc_code = s.CcCode
WHERE c.cc_enable = 1;
```

可选：授权页提供「清理无效管辖」；**不**在组织异动 API 里静默改 `Usercc`。

---

## 7. 前端（授权维护）

| 要求 | 说明 |
|------|------|
| 懒加载树 | 按 `ParentID` / 层级一次一层；80 万不可整树加载 |
| 只勾管辖根 | 存 `CcCode`；默认含下级 |
| 已选列表 | 展示当前名称/level（现查主档）；无效根标红可删 |
| 搜索 | 按编码/名称设为根 |
| 保存 | 后端按**当前** level 做祖先压缩 |
| 组织已调动 | 无需改 `Usercc`；刷新后名称/路径按主档显示 |

**禁止**：展开写入全部子孙。

---

## 8. 建议 API 面

| API | 行为 |
|-----|------|
| 获取用户管辖根 | 读 `Usercc` + JOIN 主档补全名称/当前 level/是否有效 |
| 保存用户管辖根 | 校验 `CcCode` 存在 → 按当前树压缩 → 替换写入 → 清缓存 |
| 是否可访问某部门 | 解析用户有效根当前 level 后判断 |
| 清理无效管辖 | 可选；删悬空/停用根对应行 |

组织 Transfer/Merge/Disable：**无** `Usercc` 同步钩子。

---

## 9. 验收清单

### 9.1 规模与查询

- [ ] 授权表仅存 `CcCode` 根，无叶子百万行  
- [ ] 过滤为 `Usercc JOIN cc_mstr(root) + 当前 level 前缀`，无巨型 `IN`  
- [ ] 不缓存全量可见部门 Id  

### 9.2 自动纳入

- [ ] 授了某课（含下级）后，其下新增组 → 立即可见  

### 9.3 结构/编码变（不改 Usercc）

- [ ] 授了部门 X，将 X 整树从 `0101` 调到 `0201` → **`Usercc` 行不变**，用户仍管 X 及新前缀下下级  
- [ ] 授了子孙 L，仅祖先调动且 L 随子树改 level → 仍指向 L 的 `CcCode`，按 L **当前** level 过滤正确  
- [ ] Transfer/Merge **事务内无**对 `Usercc` 的写操作  
- [ ] A 合并停用后，仅授 A 的用户失去该根效力；行可仍在，授权页可见无效  

### 9.4 治理

- [ ] 巡检列出悬空/停用根  
- [ ] 与 SysOrg 数据范围无耦合  

---

## 10. 实施顺序建议

| 步骤 | 内容 |
|------|------|
| 1 | 钉死：停用根是否仍算有效；合并后是否仅提示改授（已定：不自动改挂） |
| 2 | DDL：`Usercc(UserId, CcCode, IncludeChild, …)` |
| 3 | 授权 CRUD + 按当前 level 压缩 + 查询 helper（JOIN 取当前 level） |
| 4 | 业务列表/组织维护接入过滤（**不**改 HrOrg Transfer/Merge 写 `Usercc`） |
| 5 | 授权维护前端 |
| 6 | 巡检与「清理无效管辖」 |

---

## 11. 文档关系

| 文档 | 关系 |
|------|------|
| `../00-roadmap.md` | P6 入口 |
| `../domains/01-domain-hr-org.md` | 部门主档与异动；异动**不**维护本表 |
| `../../specs/*` | 旧 usercc 溯源（只读） |
| `../CHECKLIST.md` | 落地后增补勾选 |

---

## 12. 一句话

> 管辖表 `Usercc` 只存稳定 `CcCode` 根；过滤时 JOIN 主档取**当前** `cc_level` 做前缀匹配。  
> **结构/编码变更不改写 `Usercc`**——权限跟部门节点走；节点停用或消失则该根自然失效，由管理员改授或清理。
