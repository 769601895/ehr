# 人员资料一期字段对照（MVP）

主表：`ehr.dbo.h_emp_mstr`  
界面：基本信息 + 任职信息（截图范围）  
更新日期：2026-10-12

## 1. 基本信息

| 界面 | 列 | 类型（参考） | 说明 |
|------|-----|--------------|------|
| 工号 | `emp_id` | nvarchar(50) | PK，新增必填、不可改 |
| 姓名 | `emp_name` | nvarchar(100) | |
| 身份证号 | `emp_card` | nvarchar(50) | 建议可空唯一 |
| 身份证到期日 | `emp_card_overdate` | datetime | |
| 发证日期 | `emp_card_date` | datetime | |
| 期限 | `emp_card_limit` | int | 单位待旧数据抽样后定（常见为月）；一期只存数值 |
| 性别 | `emp_sex` | nvarchar(20) | 一期纯文本输入；下拉数据源后续补 |
| 出生日期 | `emp_birth` | datetime | |
| 年龄 | — | 计算 | 由 `emp_birth` 计算，**不落库** |
| 户籍类型 | `emp_rgt` | nvarchar(4) | 一期纯文本；下拉后续补 |
| 民族 | **`emp_nation`** | nvarchar(50) | **已确认** |
| 户口所在地 | `emp_residence` | nvarchar(80) | 可选另展示 `emp_province` |
| 婚姻状况 | `emp_marry` | nvarchar(20) | 一期纯文本；下拉后续补 |
| 现住址 | `emp_addr` | nvarchar(100) | |
| 家庭住址 | `emp_home` | nvarchar(100) | |
| 联系电话 | **`emp_addrter`** | nvarchar(50) | **已确认** |
| 照片 | **`emp_pic`** | nvarchar(255) | 存上传后的文件 URL（SysFile） |

## 2. 任职信息

| 界面 | 列 | 类型（参考） | 说明 |
|------|-----|--------------|------|
| 部门 | `cc_code` | nvarchar(50) | = `cc_mstr.cc_code`，选启用部门 |
| 职务 | `duty_id` | nvarchar(50) | 一期只加字段；下拉数据源后续补 |
| 岗位 | `post_id` | nvarchar(50) | 一期只加字段；下拉数据源后续补 |
| 参加工作时间 | `emp_work_date` | datetime | |
| 集团入职日 | `emp_in_date_group` | datetime | |
| 公司入职日 | `emp_in_date` | datetime | |
| 试用起始日 | **`emp_beg_date`** | datetime | **已确认** |
| 试用到期日 | `emp_try_date` | datetime | |
| 转正日 | **`emp_from_date`** | datetime | **已确认** |
| 入司前工龄 | **`emp_longmon`** | int | **已确认**，单位：月；界面可换算年+月展示 |
| 考勤开始日 | `emp_kq_indate` | datetime | |
| 考勤结束日 | `emp_kq_lea_date` | datetime | |
| 人员类别 | **`emp_type`** | nvarchar(50) | **已确认**；一期纯文本，下拉后续补 |
| 薪酬类别 | **`emp_sal`** | nvarchar(2) | **已确认**；一期纯文本，下拉后续补 |

## 3. 已确认映射（定稿，勿改）

| 界面 | 列 |
|------|-----|
| 试用起始日 | `emp_beg_date` |
| 转正日 | `emp_from_date` |
| 民族 | `emp_nation` |
| 联系电话 | `emp_addrter` |
| 人员类别 | `emp_type` |
| 薪酬类别 | `emp_sal` |
| 入司前工龄 | `emp_longmon`（月） |

说明：`emp_beg_date` ≠ `emp_from_date`；试用起始与转正不得混用。

## 4. 一期实现约定（已拍板）

| 项 | 约定 |
|----|------|
| 入司前工龄 | 只用 `emp_longmon`（月）；不用 `emp_long`；界面可换算年+月 |
| 身份证期限 `emp_card_limit` | 一期只存 int；单位待旧数据抽样后定（常见为月），抽样前 UI 可不标单位或暂标「期限」 |
| 职务 / 岗位 / 性别 / 婚姻 / 户籍 / 人员类别 / 薪酬类别 | **只加字段，不加数据源**（无 `code_mstr` 种子、无下拉接口）；表单用输入框；后续再补齐字典/下拉 |
| 部门 `cc_code` | 例外：继续用组织树/部门选择器（已有） |

## 5. 后端实体一期应映射的列清单

组织已有：`emp_id`, `cc_code`, `emp_name`, `emp_state`, `emp_free`, `bs_id`

本期新增（截图）：

```
emp_card, emp_card_overdate, emp_card_date, emp_card_limit,
emp_sex, emp_birth, emp_rgt, emp_nation, emp_residence, emp_province,
emp_marry, emp_addr, emp_home, emp_addrter,
duty_id, post_id, emp_work_date, emp_in_date_group, emp_in_date,
emp_beg_date, emp_try_date, emp_from_date, emp_longmon,
emp_kq_indate, emp_kq_lea_date, emp_type, emp_sal, emp_pic
```

年龄：仅输出 DTO 计算属性 `Age`，不写库。  
不映射：`emp_long`（入司前工龄走 `emp_longmon`）。
