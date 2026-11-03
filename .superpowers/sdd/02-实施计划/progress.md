# SDD ledger — plan: doc/分析文档/公司参数设置/02-实施计划.md
Pre-flight: Ruling: menu Pid=1310000000101(系统管理), OrderNo=90 before 账号管理(100); Path=/system/companyParam — user request overrides plan's 系统代码设置 placement.
Pre-flight: shared interfaces T1→T3 table/cols, T2→T4 code_field=col names, T3→T4 entity, T4→T6/T7 API — aligned with SPEC.
Task 1: Ruling: filtered unique index needs SET QUOTED_IDENTIFIER ON; seed via PowerShell SqlClient (sqlcmd UTF-8 broke Chinese/$params) — cost if wrong: re-run seed script differently on other envs.
Task 1: complete (tables HrCompanyParam + Lang + UX index)
Task 2: complete (81 code rows; EmpIdMode=8)
Task 3: complete (Entity + Defaults + Consts)
Task 4: complete (Service API; build 0 errors)
Task 5: complete (menu under 系统管理 OrderNo=90; sys_admin role grants)
Task 6: complete (siteList.vue)
Task 7: complete (detailForm.vue)
Task 8: complete (langDialog + index + i18n; Keyword warning fixed by removing shadow prop)
Final review: self-review (no fresh subagent) — build succeeded 0 errors; menu/DB verified.
