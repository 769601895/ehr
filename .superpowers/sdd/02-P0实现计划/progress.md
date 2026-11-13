# SDD ledger — plan: doc/分析文档/人事管理/人事月报/02-P0实现计划.md

Execution: Native (executing-plans) in-place per user 「直接调整」.
Branch: feat/hr-emp-month (created from current dirty main; only emp-month files staged per commit).

## Pre-flight
| Tasks | Shared interface | Check |
|-------|------------------|-------|
| T1→T6 | Resolve(payPeriod, attCloseDay, payrollAttMonth) → EmpMonthPeriodResult | Create/Preview consume same signature |
| T3→T4 | monthSnap scene + EnsureScene by module Scenes | SnapBuilder loads scene monthSnap |
| T4→T5 | BuildEmp/Chg/FieldSchema | Generate writes snapshots |
| T5→T6 | GenerateAsync(batchId, fullReset) | Service wraps generate |
| T6→T7 | Dynamic API names | Frontend api/empMonth.ts |

Ruling: work in place on feat/hr-emp-month without separate worktree — user asked 直接调整 — cost if wrong: emp-month commits share dirty workspace with unrelated WIP (mitigated by selective staging).


## Progress
Task 1: complete — HrEmpMonthPeriodResolver + 10 tests PASS
Task 2: complete — entities/const/SQL 11-12 (DDL not auto-run; ehr MCP timeout)
Task 3: complete — Catalog monthSnap + EnsureScene(module, scene); SQL 13
Task 4: complete — SnapBuilder + 2 contract tests PASS
Task 5: complete — GenerateService D8
Task 6: complete — HrEmpMonthService APIs; Application net8.0 build OK
Task 7: complete — API/UI/i18n/menu SQL 14 (detail hidden route)
Task 8: complete (code acceptance) — README + ledger; unit tests 12/12 PASS; build OK
  Remaining operator steps: run SQL 11–14 on correct DBs; restart backend; re-login for menu; smoke UI path.

Ruling: ehrnew MCP connect timeout — leave SQL for operator — cost if wrong: tables missing until scripts run.
Ruling: Generate soft-deletes lines then re-inserts (filtered unique index) — matches EntityBase IsDelete pattern.
Ruling: No git commit yet — user rule requires explicit ask — cost if wrong: work stays uncommitted on feat/hr-emp-month.
