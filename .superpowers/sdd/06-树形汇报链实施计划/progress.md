# SDD ledger — plan: doc/分析文档/审批流程引擎/06-树形汇报链实施计划.md

Ruling: work in place on main (not worktree) — existing untracked P0 WF sources must stay with implementation; cost if wrong: pollutes main until user commits/branches.

Pre-flight:
- T1→T5: ReportPath / ConditionEval / Resolver produce → Runtime consumes
- T1→T2: AssertUniqueLevelsOnPath / cycle → Role Save validation
- T2→T6: Tree/Page API → frontend
- T4→T5: ResolveUsersForRole/Post → Dispatch
- Shared interfaces OK; no conflicts found.

Task 1: complete (tests: HrWfReportPathTests → 5/5 pass; commit deferred per plan)

Task 2: complete (tests: HrWfRoleTree → 7/5 pass; Tree/Page/Save validation)
Task 3: complete (tests: HrWfConditionEvalTests → 7 pass)
Task 4: complete (ResolveUsersForRole/Post + path find tests)
Task 5: complete (Runtime ReportPath dual-track; DispatchGate; suite 44/44)
Task 6: complete (role left-tree right-table; def Level/FixedUser/Post; start SubjectEmpId)
Task 7: complete (doc/sql/2026-10-27/hr_wf_role_parent_reset.sql + README)

Ruling: Dispatch all-skipped uses hadCandidate — empty remaining nodes after last approve → Approved; candidates all skipped → Wf.NoAssigneeOnPath — cost if wrong: last-node approve would fail.
Ruling: ResolveUsersForPost/Role IO not integration-tested (no DB harness); covered by ScopeCovers + contract tests — cost if wrong: runtime bugs in SQL filters.

Final review: self-review (no committed SHAs for branch diff; suite 44/44 green). Deferred minors: ResolveUsers* 缺 DB 集成测；菜单文案未改。
