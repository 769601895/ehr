# Dependency Map — HrOrg

```
ehr.dbo.cc_mstr                 # 部门主档
    │
    ├─ cc_code / cc_level / ParentID / cc_enable …
    │
    └── h_emp_mstr.cc_code      # 人员所属部门

菜单 SysMenu → /hrms/org → hrOrg/*
序列规则 → cc_mstr.cc_code

✕ SysOrg / SysOrgService / SysRoleOrg
✕ 表名 employee（人员表为 h_emp_mstr）
✕ 迁移型 API（ImportFrom* / SyncFrom*）
```

## 入向

| 消费者 | 字段 | 策略 |
|--------|------|------|
| `h_emp_mstr` | **`cc_code`** | = `cc_mstr.cc_code` |
| 考勤/薪酬 | `cc_cost` / `cc_kqgroup` / `cc_woca`… | 读 `cc_mstr` |

## 出向

| 依赖 | 用途 | 一期 |
|------|------|------|
| `cc_mstr` | 主档 | 必须 |
| `h_emp_mstr` | 占用校验；合并迁 `cc_code` | 必须 |
| 单号 | `cc_code` 生成 | 必须 |
| SysOrg | — | **禁止** |

## `h_emp_mstr` 组织相关列

| 列 | 组织用途 |
|----|----------|
| `emp_id` | 人员键 |
| **`cc_code`** | 所属部门 |
| `emp_state` | 删前校验；合并仅迁 `<>3` |
| `emp_name` | 提示（可选） |
| `bs_id` | 自增辅助键 |

## 文件锚点

- DDL：`doc/sql/hr-org/*.sql`  
- 方案：`../domains/01-domain-hr-org.md`  
- 逆向参考（只读）：`../specs/yuenan_w_rs_cc_mstr_reverse_spec.md`
