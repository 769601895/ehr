# -*- coding: utf-8 -*-
"""Generate code_mstr seed for HrCompanyParam dropdowns. code_field = column name."""
from pathlib import Path

# code_field -> list of (value, zh, en, sort)
DATA = {
    "EmpIdMode": [
        ("0", "自动流水号", "Auto serial", "10"),
        ("1", "手工维护", "Manual", "20"),
        ("2", "2位年+流水号", "2-digit year + serial", "30"),
        ("3", "4位年+流水号", "4-digit year + serial", "40"),
        ("4", "固定号+流水号", "Fixed prefix + serial", "50"),
        ("5", "4位年+月+流水号", "4-digit year+month+serial", "60"),
        ("6", "1开始流水号", "Serial from 1", "70"),
        ("7", "2位年+月+流水号", "2-digit year+month+serial", "80"),
    ],
    "ProbationOnHire": [("0", "不试用", "No probation", "10"), ("1", "试用", "Probation", "20")],
    "ProbationLinkType": [
        ("0", "固定值", "Fixed value", "10"),
        ("1", "与职务关联", "Linked to duty", "20"),
        ("2", "与岗位关联", "Linked to post", "30"),
        ("3", "与人员类别关联", "Linked to emp type", "40"),
    ],
    "ProbationUnit": [("0", "月", "Month", "10"), ("1", "天", "Day", "20")],
    "ProbationDayInclusive": [("1", "是", "Yes", "10"), ("0", "否", "No", "20")],
    "SalaryTypeMaint": [
        ("0", "人事维护", "HR maintain", "10"),
        ("1", "薪资维护", "Payroll maintain", "20"),
        ("2", "保密", "Confidential", "30"),
    ],
    "PayCategoryMaint": [("0", "人事维护", "HR maintain", "10"), ("1", "薪资维护", "Payroll maintain", "20")],
    "DimissionAudit": [("0", "否", "No", "10"), ("1", "是", "Yes", "20")],
    "TransferAudit": [("0", "否", "No", "10"), ("1", "是", "Yes", "20")],
    "RegularizeAudit": [("0", "否", "No", "10"), ("1", "是", "Yes", "20")],
    "IdCardV2Enabled": [("1", "是", "Yes", "10"), ("0", "否", "No", "20")],
    "IcCardMode": [
        ("0", "与工号一致(完全)", "Same as emp id (full)", "10"),
        ("2", "与工号一致(截零)", "Same as emp id (trim zero)", "20"),
        ("1", "手工维护", "Manual", "30"),
    ],
    "AttMethodDefault": [
        ("0", "正常考勤", "Normal attendance", "10"),
        ("1", "手工考勤", "Manual attendance", "20"),
        ("2", "免刷卡", "No swipe", "30"),
    ],
    "HireDayAttMode": [
        ("0", "是", "Yes", "10"),
        ("1", "第1天免考勤", "Exempt day 1", "20"),
        ("2", "前两天免考勤", "Exempt first 2 days", "30"),
    ],
    "LeaveDayAttMode": [
        ("0", "无薪不考勤", "Unpaid no attendance", "10"),
        ("1", "有薪需刷卡", "Paid require swipe", "20"),
        ("2", "有薪不考勤", "Paid no attendance", "30"),
    ],
    "RequiredAttDayMode": [
        ("0", "正常工作日", "Normal workdays", "10"),
        ("1", "全月天数", "Full month days", "20"),
        ("2", "正常工作日(含节日)", "Workdays incl. holidays", "30"),
    ],
    "PayrollAttMonth": [("0", "当月", "Current month", "10"), ("1", "上月", "Previous month", "20")],
    "OtAuditMode": [
        ("0", "否(按时间申请)", "No (by time)", "10"),
        ("2", "否(按小时申请)", "No (by hour)", "20"),
        ("1", "是(按时间申请)", "Yes (by time)", "30"),
        ("3", "是(按小时申请)", "Yes (by hour)", "40"),
    ],
    "LeaveAudit": [("0", "否", "No", "10"), ("1", "是", "Yes", "20")],
    "LateAsException": [("0", "是", "Yes", "10"), ("1", "否", "No", "20")],
    "OtCrossdayMode": [("0", "当日计", "Same day", "10"), ("1", "次日计", "Next day", "20")],
    "DailyLeaveUnit": [("0", "小时", "Hour", "10"), ("1", "天", "Day", "20")],
    "MissSwipeMode": [("0", "计漏刷卡", "Missed swipe", "10"), ("1", "计旷工", "Absent", "20")],
    "OtCalcStandard": [
        ("0", "以实际时数", "Actual hours", "10"),
        ("1", "以半小时取整", "Round to 0.5 hour", "20"),
        ("2", "以加班刷卡时间取整", "Round by OT swipe", "30"),
    ],
    "CardMakeupAudit": [("0", "否", "No", "10"), ("1", "是", "Yes", "20")],
    "RewardCalcMode": [("0", "按日薪比例", "By daily rate", "10"), ("1", "奖惩对应金额", "Fixed amount", "20")],
    "BankAccountMode": [("1", "单一帐户", "Single account", "10"), ("2", "多帐户", "Multi account", "20")],
    "ApprovedSalaryCalc": [("0", "壹段式计算", "One-stage", "10"), ("1", "两段式计算", "Two-stage", "20")],
    "RewardAmountMaint": [("0", "与奖惩类型关联", "Linked to type", "10"), ("1", "手工维护", "Manual", "20")],
    "InsMonthReportMode": [("0", "当月生成月报", "Current month report", "10"), ("1", "次月生成月报", "Next month report", "20")],
    "InsFeeToPayroll": [("1", "保费提交当月薪资", "Fee to current payroll", "10"), ("2", "保费提交上月薪资", "Fee to previous payroll", "20")],
    "InsSameMonthReport": [("0", "生成", "Generate", "10"), ("1", "不生成", "Do not generate", "20")],
}


def esc(s: str) -> str:
    return s.replace("'", "''")


lines = [
    "/*",
    " * 变更说明：公司参数下拉码表种子；code_field = 列名（PascalCase，无下划线）",
    " * 模块：hrCompanyParam",
    " * 日期：2026-10-30",
    " * 影响库：ehrnew",
    " * 是否幂等：是",
    " */",
    "SET NOCOUNT ON;",
    "GO",
    "",
]

for field, rows in DATA.items():
    for val, zh, en, sort in rows:
        lines.append(f"-- {field}={val}")
        lines.append(
            f"IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=N'{field}' AND code_value=N'{val}')\n"
            f"    INSERT INTO dbo.code_mstr (code_field, code_value, code_desc, code_sort)\n"
            f"    VALUES (N'{field}', N'{val}', N'{esc(zh)}', N'{sort}');\n"
            f"ELSE\n"
            f"    UPDATE dbo.code_mstr SET code_desc=N'{esc(zh)}', code_sort=N'{sort}'\n"
            f"    WHERE code_field=N'{field}' AND code_value=N'{val}';"
        )
        for culture, desc in (("zh-CN", zh), ("en", en)):
            lines.append(
                f"IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=N'{field}' AND code_value=N'{val}' AND culture=N'{culture}')\n"
                f"    INSERT INTO dbo.code_mstr_lang (code_field, code_value, culture, code_desc)\n"
                f"    VALUES (N'{field}', N'{val}', N'{culture}', N'{esc(desc)}');\n"
                f"ELSE\n"
                f"    UPDATE dbo.code_mstr_lang SET code_desc=N'{esc(desc)}'\n"
                f"    WHERE code_field=N'{field}' AND code_value=N'{val}' AND culture=N'{culture}';"
            )
        lines.append("GO")
        lines.append("")

out = Path(r"e:\ehr2026\ehr\doc\sql\2026-10-30\02-HrCompanyParam码表种子.sql")
out.write_text("\n".join(lines) + "\n", encoding="utf-8")
print("wrote", out, "bytes", out.stat().st_size)
