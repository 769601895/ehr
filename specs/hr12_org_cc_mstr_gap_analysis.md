# Gap Analysis: 组织架构管理（hr12 / w_rs_cc_mstr → Web）

> 对照基准：`specs/hr12_org_cc_mstr_reverse_spec.md`  
> 现场库：`HR_2025.dbo.cc_mstr`（MCP 实测 2026-10-10）  
> 目标态：Admin.NET `HrOrgService` + `Web/src/views/hrms/org`

---

## 1. 总览

| 维度 | 覆盖度 | 说明 |
|------|--------|------|
| 核心 CRUD + 树 | 高 | SysOrg 双写已落地 |
| 合并 / 调动 | 高 | API + 前端对话框已有 |
| 批量成本/考勤 | **中低** | UI/API 有，但库缺列，运行会炸 |
| 软删语义 | **低** | PB 软删；Web 硬删 |
| 层级码字母扩展 | **中** | 现网无字母后缀；算法未对齐 `uf_getnew_level` |
| 管辖权限 `usercc` | **未做** | 库有 `hr_sys_usercc`，Web 未用 |
| 人员部门归属批量 | **未做** | 属 `w_rs_cc_mstr` 页签2，应落员工模块 |

---

## 2. 能力缺口（Capability）

| ID | 期望（PB） | 现状（Web） | 优先级 | 建议 |
|----|------------|-------------|--------|------|
| G1 | 删除=软删 `cc_enable=1`，可带删子树 | `Delete` 物理删除 SysOrg + cc_mstr | **P0** | 改为停用：写 `cc_enable/cc_dele_*`，`SysOrg.Status=Disable`；可选级联子树 |
| G2 | 批量维护 `cc_kq_*` | 实体/API 写了 `cc_kq_wocachange` 等 | **P0** | 库无这些列 → 先 DDL 或从 UpdateColumns 去掉不存在列 |
| G3 | `OrgId` 映射 SysOrg | 运行时 `EnsureOrgIdColumn` 动态加列 | P1 | 正式迁移脚本加 `OrgId`，勿依赖首次请求 DDL |
| G4 | `uf_getnew_level` 支持 `00..99..A0..ZZ` | `BuildCcLevel` 仅 `int.Parse` 两位数字 | P1 | 同级>99 时会错码/撞码；移植 PB 算法 |
| G5 | 新增前校验 `v_usercc` 管辖 | 无部门范围校验 | P2 | 对接 `hr_sys_usercc`（或框架数据权限） |
| G6 | 人员部门归属批量改 `cc_code` | 组织页无此页签 | P2 | 放到员工模块批量调岗 |
| G7 | 删前区分在职/停职员工 | `HrEmployee` 仅 `emp_id/cc_code`，任意占用即禁删 | P2 | 加 `emp_state`，对齐「仅在职阻断」 |
| G8 | 删后可选重建树 / 刷新 usercc | 仅前端 reload | P3 | 按需 |
| G9 | `of_cchist_insert` 部门历史 | PB 已 stub；Web 无 | P3 | 暂可忽略 |
| G10 | `cc_code_parent` 维护 | 双写时写入；现网大量为空 | P2 | Sync/Update 时回填 |

---

## 3. 数据缺口（Data / Schema）

### 3.1 `HR_2025.dbo.cc_mstr` 实测列（32 列）

与实体 `CcMstr.cs` 对比：

| 实体属性 | 库列 | 状态 |
|----------|------|------|
| 核心部门字段（code/desc/level/…） | 有 | OK |
| `CcKqgroup` / `CcWoca` / … | 有 | OK |
| `CcKqWocachange` | **无** `cc_kq_wocachange` | **缺口** |
| `CcKqCode` | **无** `cc_kq_code` | **缺口** |
| `CcKqControl` | **无** | **缺口** |
| `CcKqEdituser` / `CcKqEditdate` | **无** | **缺口** |
| `OrgId` | **无**（靠运行时 ALTER） | **缺口** |

`BatchUpdateCostKq` / `BatchUpdateKqCode` / `GetCcList` 映射这些字段时，**读可能静默空，写会 SQL 异常**。

### 3.2 现网数据特征（37 行）

| 指标 | 值 | 含义 |
|------|-----|------|
| 启用 / 停用 | 34 / 3 | 已有软删数据 |
| 最大 `cc_level` 长 | 8（4 层） | 远未触 20 上限 |
| 字母后缀数量 | **0** | 短期数字算法可用 |
| `cc_code` | 6 位数字，最大 21573 | 与 Web `NextCcCode` 的 `000000` 一致；**不同于** hr12 的 10 位 |
| `cc_code_parent` | 多样本为空 | 父子主要靠 `ParentID` + `cc_level` |
| `v_usercc` | **不存在** | 权限表为 `hr_sys_usercc(user_id, cc_code, …)` |

样例树：

```
01 乐时集团
└─ 0101 乐时越南
   ├─ 010101 家电厂 → 01010101…01010110
   ├─ 010102 资材部 …
   └─ 0102 注塑厂 …
```

---

## 4. 算法缺口（Technology）

### 4.1 层级码

```
PB uf_getnew_level(后两位):
  00..98 → +1 数字
  99 → A0
  A0..Z9 → 字母进制（含 9→A）
  ZZ → 'err'

Web BuildCcLevel:
  同级取 max(int 后两位)+1 → "00" 格式
  字母后缀 TryParse 失败 → 当 0 → 可能生成重复 "01"
```

**风险：** 一旦出现 `A0` 及以后，或同级超过 99，Web 会生成错误层级。

### 4.2 删除语义

| | PB | Web |
|--|----|-----|
| 动作 | `UPDATE … cc_enable=1` | `DELETE` 行 + 删 SysOrg |
| 可恢复 | 是（改回 enable） | 否 |
| 与现网 3 条停用数据 | 一致 | **不一致** |

### 4.3 编码生成

| | hr12 | HR_2025 现网 | Web |
|--|------|-------------|-----|
| 宽度 | 10 位 `0000000001` | 6 位 `021537` | 6 位 `000000` |
| 结论 | — | — | **与现场一致**，勿改回 10 位 |

---

## 5. 流程缺口（Process）

```
PB: 选树 → 直接下级列表 → 增改删/异动 → 可选重建树 + 刷新 usercc
Web: 左树 + 全树表格 → 增改删/合并/调动/批量 → reload
```

差异：

1. Web 列表是**整棵机构树**，不是「仅直接下级」；交互更现代，但与 PB 过滤条件不同。  
2. Web **合并**要求两源部门均无子部门；与 `w_rs_cc_mstr` 一致。  
3. Web **调动**用 OrgId，并重算子树；逻辑对齐，但依赖 `BuildCcLevel` 正确。

---

## 6. 建议落地顺序

### P0（立刻）

1. **修复批量考勤列**（二选一）  
   - A：DDL 增加 `cc_kq_wocachange/cc_kq_code/cc_kq_control/cc_kq_edituser/cc_kq_editdate`  
   - B：实体与 `BatchUpdate*` 去掉不存在列，批量页只保留 `cc_cost` + `cc_kqgroup`  
2. **删除改软删**：对齐 `cc_enable` + `SysOrg.Status`，禁止物理删有业务引用的部门。  
3. **正式加 `OrgId` 列**（迁移 SQL，而不仅靠 `EnsureOrgIdColumn`）。

### P1

4. 移植 `uf_getnew_level` 到 `BuildCcLevel`（或共享工具类）。  
5. Sync/Update 时回填空的 `cc_code_parent`。

### P2

6. 数据权限：按 `hr_sys_usercc` 过滤可管部门。  
7. `HrEmployee` 增加 `emp_state`，删除校验只拦在职。  
8. 员工模块补「批量变更部门」（对应页签「人员部门归属」）。

---

## 7. 验收清单（针对缺口关闭）

- [ ] 批量维护保存不再报「无效列名」  
- [ ] 删除部门后 `cc_mstr` 仍在且 `cc_enable=1`，列表默认不展示  
- [ ] 同级第 100 个部门生成 `A0` 而非错误数字  
- [ ] `OrgId` 列存在且 SyncFromCcMstr 后双向可查  
- [ ] 有在职员工的部门无法删除；仅停职占用时可按产品规则处理  

---

## 8. 相关代码位置

| 项 | 路径 |
|----|------|
| 规格 | `specs/hr12_org_cc_mstr_reverse_spec.md` |
| 服务 | `Admin.NET/.../Service/Hr/Org/HrOrgService.cs` |
| 实体 | `Admin.NET/.../Entity/CcMstr.cs` |
| 前端 | `Web/src/views/hrms/org/` |
| PB hr12 | `hr12/deptmanger.pbl` → `w_deptmanger` |
| PB src | `src/hr_rs_basic.pbl` → `w_rs_cc_mstr` |
