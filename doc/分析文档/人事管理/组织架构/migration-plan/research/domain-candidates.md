# Domain Candidates — 人事组织

## Bounded Context：HrOrg

| 能力包 | 本系统归属 |
|--------|------------|
| 机构树 CRUD | `ehr.dbo.cc_mstr` |
| 停用 | `cc_enable` / `cc_dele_*` |
| 合并 / 调动 | `HrOrgService` → `cc_mstr` + `h_emp_mstr.cc_code` |
| 明细导出 | 列表导出 |
| 人员批量改部门 | **不做** |
| 管辖范围（二期） | 另表挂 `cc_code`（仍不碰 SysOrg） |

## 邻接域

| 域 | 耦合点 |
|----|--------|
| 人员资料 `h_emp_mstr` | **`cc_code`**；删前占用；合并迁部门 |
| 厂别 / 工时 / 编制 | `cc_site` / `cc_woca` / `cc_bz_unit` |
| 考勤 / 成本 | `cc_cost` / `cc_kqgroup` 等已有列 |

## 非目标（硬性）

- 与 `SysOrg` 零关联。  
- 不新建 `HrOrgExt` 替代主档。  
- 不改表名、一期不改字段集。  
- 不做批量人员改部门。  
- **实现无老系统迁移痕迹**（无 Legacy / SyncFrom / ImportFrom 产品能力）。
