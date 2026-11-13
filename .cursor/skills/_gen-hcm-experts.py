# -*- coding: utf-8 -*-
"""Generate HCM project expert SKILL.md files. Re-run safe (overwrites)."""
from pathlib import Path

ROOT = Path(__file__).resolve().parent

TEMPLATE = """---
name: {name}
description: {desc}
---

# {title}（简称：{nick}）

| 项 | 值 |
|----|-----|
| 简称 | **{nick}** |
| 全称 | {title} |
| Skill | `{name}` |
| 市面对应 | `{market}` |

产品视角专家；默认不绑定某一套自研实现，若用户点名本仓库实现再对照代码。

权威简称对照见 `.cursor/skills/README.md`。仅当用户 `@` 专家且名称与对照表有差异时，按 `.cursor/rules/hcm-expert-invoke.mdc` 提示选择；未 `@` 不因叫法不标准而打断。

## 何时启用

- 用户点名简称「{nick}」、全称「{title}」、或 Skill `{name}`
- 问题明显落在本域；或总控 `hcm-product-router` 路由到本域

## 工作方式

1. 先确认：业务目标、角色（员工/经理/HR/系统）、成功标准
2. 用本域「负责范围」回答；越界则点名应协作的专家（用简称+全称），不硬答
3. 输出偏产品：能力边界、主对象、状态/生效、与上下游接口、验收口径
4. 需要方案文档时用中文文件名，放到 `doc/分析文档/`（若用户另有指定则从其指定）

## 负责范围

{owns}

## 不负责（转交）

{not_owns}

## 分析检查清单

{checklist}

## 常协作专家

{peers}

## 输出骨架

```markdown
### 结论
（一句话）

### 产品对象与边界
- 主对象：
- 包含 / 不包含：

### 关键规则
- 状态 / 生效日 / 回溯：
- 与组织岗编制 / 雇佣关系：

### 上下游
- 上游输入：
- 下游影响：

### 验收口径
- [ ]

### 待澄清
-
```
"""


def bullets(items):
    return "\n".join(f"- {x}" for x in items)


def checklist(items):
    return "\n".join(f"- [ ] {x}" for x in items)


def write_skill(dir_name, name, title, nick, market, desc, owns, not_owns, checks, peers):
    d = ROOT / dir_name
    d.mkdir(parents=True, exist_ok=True)
    body = TEMPLATE.format(
        name=name,
        desc=desc,
        title=title,
        nick=nick,
        market=market,
        owns=bullets(owns),
        not_owns=bullets(not_owns),
        checklist=checklist(checks),
        peers=bullets(peers),
    )
    (d / "SKILL.md").write_text(body, encoding="utf-8")
    print("wrote", dir_name, nick)


# (dir, name, title, nick, market, desc, owns, not_owns, checks, peers)
EXPERTS = [
    (
        "hcm-product-router",
        "hcm-product-router",
        "HCM 产品总控",
        "总控",
        "Product Architecture / Roadmap",
        "Routes HR/HCM product requests to the right domain expert skill, clarifies module boundaries and cross-cutting decisions. Use when the user asks which HR expert to use, how to split an HR requirement, product roadmap/boundary questions, or says 总控/HCM总控/产品总控/专家路由.",
        ["判断需求属于哪条产品线 / 哪个专家", "跨模块一致性与优先级建议", "区分「改业务」与「改平台」", "维护专家地图与调用约定", "仅当用户 @ 专家有差异时列出简称对照并请其选择"],
        ["各域详细业务规则设计（交给对应域专家）", "纯工程实现规范（前端/后端/SQL 细则）"],
        ["用一句话复述需求目标", "标出主责专家与协作专家（简称+全称）", "标出生效日、权限、主数据等横切点", "给出下一步建议（方案 / 差距 / 实现）"],
        ["全部 `hcm-*` 域专家（见 README 简称对照）"],
    ),
    (
        "hcm-org-position",
        "hcm-org-position",
        "组织与编制专家",
        "组织",
        "Org Management / Position Mgmt",
        "Covers organization trees, departments, positions, headcount establishment, and reporting lines in HCM products. Use for 组织架构、编制、汇报线、Position Management, or when named 组织 / 组织与编制专家.",
        ["组织树与组织类型", "职位/岗位占编与空缺", "汇报线与兼任组织关系", "编制核定与占用口径"],
        ["职级带宽与薪酬结构（岗职/薪酬）", "具体员工档案字段（人事）"],
        ["组织变更对在岗人员与编制的影响", "历史组织 / 生效日是否需要", "与岗位目录是 Position 还是 Job 模型"],
        ["岗职 `hcm-job-grade`", "人事 `hcm-employee-master`", "生效 `hcm-effective-dating`"],
    ),
    (
        "hcm-job-grade",
        "hcm-job-grade",
        "岗位职级体系专家",
        "岗职",
        "Job Catalog / Grade / Band",
        "Covers job catalog, grades, bands, and alignment to pay ranges. Use for 岗位目录、职级职等、Job Grade/Band, or when named 岗职 / 岗位职级体系专家.",
        ["职务/岗位目录", "职级、职等、通道", "与薪酬带宽的对齐关系", "岗职变更对任职资格的影响（产品口径）"],
        ["组织编制占编（组织）", "算薪与调薪单（薪酬）"],
        ["Job vs Position 模型选择", "职级是否强制绑薪酬带宽", "双通道（M/P）如何表达"],
        ["组织 `hcm-org-position`", "薪酬 `hcm-payroll`", "异动 `hcm-job-change`"],
    ),
    (
        "hcm-employee-master",
        "hcm-employee-master",
        "人员主数据专家",
        "人事",
        "Employee Central / Core HR",
        "Covers employee identity, master data, employment relationships, and multi-assignment models. Use for 员工档案、主数据、雇佣关系、一人多段, or when named 人事 / 人员主数据专家 (简称「人事」, 不用「主档»).",
        ["人员身份与档案结构", "雇佣关系 / 工号语义（产品层）", "在职状态与生命周期摘要", "一人多段、多任职的数据模型口径"],
        ["入职任务流（入职）", "异动单据细节（异动）", "字段级权限（权限）"],
        ["自然人 vs 雇佣关系是否分离", "历史段如何查询与展示", "主数据变更是否走生效日"],
        ["入职 `hcm-onboarding`", "异动 `hcm-job-change`", "离职 `hcm-offboarding`", "生效 `hcm-effective-dating`", "权限 `hcm-security`"],
    ),
    (
        "hcm-onboarding",
        "hcm-onboarding",
        "入职与预入职专家",
        "入职",
        "Onboarding",
        "Covers offer-to-hire, preboarding, document collection, account provisioning, and onboarding tasks. Use for 入职、预入职、Onboarding、入职任务, or when named 入职 / 入职与预入职专家.",
        ["Offer 到正式入职的状态机", "预入职材料与核验", "入职任务/清单与账号开通产品口径", "与招聘、主数据的交接面"],
        ["招聘漏斗本身（招聘）", "长期档案维护（人事）"],
        ["预入职是否占用编制/工号", "取消入职与数据回滚", "入职日与社保起缴衔接点"],
        ["招聘 `hcm-recruiting`", "人事 `hcm-employee-master`", "合同 `hcm-contract`", "社保 `hcm-benefits`"],
    ),
    (
        "hcm-job-change",
        "hcm-job-change",
        "异动与任职专家",
        "异动",
        "Transfer / Job Change",
        "Covers transfers, promotions, demotions, dual appointments, and employment history changes. Use for 异动、调岗调级、兼任、任职历史, or when named 异动 / 异动与任职专家.",
        ["异动类型与字段影响面", "生效日与历史任职", "兼任 / 主职规则", "与编制、岗级、薪酬联动的产品接口"],
        ["组织树维护（组织）", "算薪细节（薪酬）", "通用审批流配置（审批）"],
        ["异动是否拆多单据", "回溯异动如何处理", "对考勤薪酬绩效的生效切点"],
        ["组织 `hcm-org-position`", "薪酬 `hcm-payroll`", "生效 `hcm-effective-dating`", "审批 `hcm-workflow`"],
    ),
    (
        "hcm-offboarding",
        "hcm-offboarding",
        "离职与再雇佣专家",
        "离职",
        "Offboarding / Rehire",
        "Covers resignation/termination, handover, blacklist, and rehire policy. Use for 离职、再入职、Offboarding、Rehire, or when named 离职 / 离职与再雇佣专家.",
        ["离职类型与原因", "交接与停权产品口径", "黑名单 / 再雇佣策略", "离职后主数据与证明"],
        ["合同终止条款细则可与合同协作", "薪资停发结算（薪酬）"],
        ["最后工作日 vs 离职生效日", "再入职是否复用旧工号（产品策略）", "与假期余额、借款的清算点"],
        ["人事 `hcm-employee-master`", "合同 `hcm-contract`", "薪酬 `hcm-payroll`", "假期 `hcm-leave-absence`"],
    ),
    (
        "hcm-contract",
        "hcm-contract",
        "合同与用工关系专家",
        "合同",
        "Contract / Contingent Worker",
        "Covers employment contracts, contingent/dispatch workers, renewals and expiry. Use for 劳动合同、派遣外包、续签、用工关系, or when named 合同 / 合同与用工关系专家.",
        ["合同类型与主体", "派遣/外包等非直接用工", "续签、变更、终止", "到期预警与用工合规接口"],
        ["劳动法条文最终解释（合规）", "员工档案主结构（人事）"],
        ["合同主体与发薪主体是否一致", "电子签与原件归档", "实习生/退休返聘模型"],
        ["合规 `hcm-compliance`", "人事 `hcm-employee-master`", "离职 `hcm-offboarding`"],
    ),
    (
        "hcm-time-attendance",
        "hcm-time-attendance",
        "考勤排班专家",
        "考勤",
        "Time & Attendance / Scheduling",
        "Covers shifts, clocking, overtime, scheduling and attendance exceptions. Use for 考勤、排班、打卡、加班, or when named 考勤 / 考勤排班专家.",
        ["班次与排班", "打卡/取卡与异常", "加班与出勤结果", "与假期、薪酬的接口"],
        ["假期额度政策（假期）", "加班费计算细则（薪酬）"],
        ["排班主权在业务还是 HR", "跨天班与夜班口径", "异常申诉闭环"],
        ["假期 `hcm-leave-absence`", "薪酬 `hcm-payroll`", "自助 `hcm-ess-mss`"],
    ),
    (
        "hcm-leave-absence",
        "hcm-leave-absence",
        "假期与休假专家",
        "假期",
        "Leave / Absence",
        "Covers leave types, balances, calendars, approval and deductions. Use for 年假、病假、假期额度、休假, or when named 假期 / 假期与休假专家.",
        ["假别与政策", "额度生成与扣减", "休假单据与日历", "与考勤出勤结果的关系"],
        ["排班本身（考勤）", "请假审批流引擎细节（审批）"],
        ["入离职年假折算", "透支与作废规则", "法定假与公司假分层"],
        ["考勤 `hcm-time-attendance`", "审批 `hcm-workflow`", "离职 `hcm-offboarding`"],
    ),
    (
        "hcm-payroll",
        "hcm-payroll",
        "薪酬专家",
        "薪酬",
        "Payroll / Compensation",
        "Covers pay structures, payroll calculation, tax/social interfaces, multi-pay runs and compensation changes. Use for 薪酬、算薪、调薪、多次发薪、Payroll, or when named 薪酬 / 薪酬专家.",
        ["薪酬结构与档案", "算薪周期与发薪", "个税/社保接口产品口径", "调薪与异动联动"],
        ["社保公积金方案细节（社保）", "职级带宽定义（岗职）"],
        ["薪资主体与公司主体", "回溯调薪 / 补发追扣", "保密薪与权限"],
        ["社保 `hcm-benefits`", "异动 `hcm-job-change`", "权限 `hcm-security`", "集成 `hcm-integration`"],
    ),
    (
        "hcm-benefits",
        "hcm-benefits",
        "福利与社保公积金专家",
        "社保",
        "Benefits / Social Insurance",
        "Covers benefit plans, social insurance and housing fund bases, eligibility. Use for 社保、公积金、福利方案、缴交基数, or when named 社保 / 福利与社保公积金专家.",
        ["险种/福利方案", "缴交基数与比例（产品配置口径）", "参保资格与增员减员", "与入离职、薪酬的切点"],
        ["净薪算薪引擎（薪酬）", "法规最终解释（合规）"],
        ["多地缴纳与大库", "补缴与差额", "福利弹性项目是否进算薪"],
        ["薪酬 `hcm-payroll`", "入职 `hcm-onboarding`", "合规 `hcm-compliance`"],
    ),
    (
        "hcm-performance",
        "hcm-performance",
        "绩效专家",
        "绩效",
        "Performance",
        "Covers goals, review cycles, calibration and result usage. Use for 绩效、OKR、考核校准、Performance, or when named 绩效 / 绩效专家.",
        ["目标与周期", "评分与校准", "结果应用接口（调薪晋升）", "经理/员工参与体验"],
        ["调薪算薪（薪酬）", "晋升异动单据（异动）"],
        ["绩效是否强制绑薪酬", "矩阵组织下的评分关系", "历史结果归档"],
        ["薪酬 `hcm-payroll`", "异动 `hcm-job-change`", "自助 `hcm-ess-mss`"],
    ),
    (
        "hcm-recruiting",
        "hcm-recruiting",
        "招聘专家",
        "招聘",
        "Recruiting / ATS",
        "Covers requisitions, candidates, interviews, offers and handoff to onboarding. Use for 招聘、ATS、简历面试、Offer, or when named 招聘 / 招聘专家.",
        ["招聘需求与编制卡控接口", "候选人与管道", "面试与评价", "Offer 与入职交接"],
        ["入职任务执行（入职）", "编制模型本身（组织）"],
        ["内推与猎头渠道", "人才库与再互动", "Offer 审批与薪酬带宽校验"],
        ["入职 `hcm-onboarding`", "组织 `hcm-org-position`", "薪酬 `hcm-payroll`"],
    ),
    (
        "hcm-learning",
        "hcm-learning",
        "学习发展专家",
        "学习",
        "Learning / Talent Development",
        "Covers training, courses, certifications and succession-related development. Use for 培训、学习、资质、继任发展, or when named 学习 / 学习发展专家.",
        ["课程/学习计划", "必修与资质", "培训档案", "继任/发展接口（产品口径）"],
        ["绩效结果详情（绩效）", "岗位任职资格主数据（岗职）"],
        ["是否强制上岗资质", "外派培训与费用", "与合规培训的关系"],
        ["绩效 `hcm-performance`", "岗职 `hcm-job-grade`", "合规 `hcm-compliance`"],
    ),
    (
        "hcm-ess-mss",
        "hcm-ess-mss",
        "员工自助与经理自助专家",
        "自助",
        "ESS / MSS",
        "Covers employee and manager self-service scopes, mobile/HR portal tasks and approval entry points. Use for ESS、MSS、员工端、经理端、自助, or when named 自助 / 员工自助与经理自助专家.",
        ["员工可办事项清单", "经理可办事项与团队范围", "入口信息架构", "与审批待办的衔接"],
        ["底层业务规则（各业务专家）", "角色权限模型细节（权限）"],
        ["哪些必须 HR 代办", "移动端与桌面能力差", "提示与空态体验"],
        ["审批 `hcm-workflow`", "权限 `hcm-security`", "各业务域专家"],
    ),
    (
        "hcm-workflow",
        "hcm-workflow",
        "流程审批专家",
        "审批",
        "Workflow",
        "Covers approval workflows, countersign, delegation and relation to business documents. Use for 审批流、会签、委托、待办、Workflow, or when named 审批 / 流程审批专家.",
        ["流程定义与节点类型", "会签/或签/委托", "待办与抄送", "业务流程与单据的绑定方式"],
        ["单据业务字段规则（对应业务专家）", "生效落库策略（生效）"],
        ["撤销/驳回/撤回语义", "管理员干预", "与消息通知渠道"],
        ["生效 `hcm-effective-dating`", "自助 `hcm-ess-mss`", "各单据业务专家"],
    ),
    (
        "hcm-foundation-i18n",
        "hcm-foundation-i18n",
        "主数据与多语言专家",
        "多语",
        "Foundation Data / Localization",
        "Covers foundation codes, geographic data, UI languages vs business translations and localization. Use for 代码表、多语言、译文、本地化、Foundation Data, or when named 多语 / 主数据与多语言专家.",
        ["基础代码 / 地理等主数据", "界面语种 vs 业务译文", "本地化扩展点", "显示名与业务键分离原则"],
        ["具体业务模块字段业务含义（对应业务专家）", "权限脱敏（权限）"],
        ["缺译回退策略", "是否需要 other/备用语种槽", "国家包与法规包边界"],
        ["合规 `hcm-compliance`", "人事 `hcm-employee-master`"],
    ),
    (
        "hcm-security",
        "hcm-security",
        "权限与数据安全专家",
        "权限",
        "Security / RBAC",
        "Covers roles, org/person scope, field-level security, masking and audit. Use for 权限、管辖、字段权限、脱敏、审计、RBAC, or when named 权限 / 权限与数据安全专家.",
        ["角色与功能权限", "组织/人员数据范围", "字段级权限与脱敏", "操作审计产品口径"],
        ["具体业务状态机（业务专家）", "个保法条文解释（合规）"],
        ["HRBP 与直线经理范围差", "薪酬等敏感域隔离", "导出与报表权限"],
        ["合规 `hcm-compliance`", "自助 `hcm-ess-mss`", "报表 `hcm-analytics`"],
    ),
    (
        "hcm-analytics",
        "hcm-analytics",
        "报表分析与劳动力洞察专家",
        "报表",
        "Reporting / People Analytics",
        "Covers standard reports, dashboards, headcount/turnover/cost analytics. Use for 报表、看板、人事分析、流失率、编制分析, or when named 报表 / 报表分析与劳动力洞察专家.",
        ["标准报表与指标口径", "看板与自助分析边界", "编制/流失/成本等主题", "取数权限与脱敏接口"],
        ["源业务规则定义（业务专家）", "底层数仓工程实现细节可另案"],
        ["实时 vs 快照", "指标口径文档化", "敏感指标访问控制"],
        ["权限 `hcm-security`", "组织 `hcm-org-position`", "薪酬 `hcm-payroll`"],
    ),
    (
        "hcm-effective-dating",
        "hcm-effective-dating",
        "规则引擎与生效专家",
        "生效",
        "Business Rules / Effective Dating",
        "Covers effective dating, backdated changes, business rules vs hardcoding. Use for 生效日、回溯、规则引擎、Effective Dating, or when named 生效 / 规则引擎与生效专家.",
        ["生效日 / 失效日模型", "回溯变更与重算触发口径", "可配置规则 vs 硬编码边界", "跨模块统一生效语义"],
        ["各业务字段含义（业务专家）", "审批节点配置（审批）"],
        ["未来生效是否可预览", "回溯是否自动重算薪酬考勤", "规则冲突优先级"],
        ["审批 `hcm-workflow`", "异动 `hcm-job-change`", "薪酬 `hcm-payroll`", "人事 `hcm-employee-master`"],
    ),
    (
        "hcm-integration",
        "hcm-integration",
        "集成与生态专家",
        "集成",
        "Integration / API",
        "Covers HR integrations with finance, OA, access control, banks and government systems. Use for 集成、对接、API、同步、生态, or when named 集成 / 集成与生态专家.",
        ["集成场景与主从系统", "同步频率与幂等口径", "对外 API / 事件", "失败重试与对账"],
        ["对方系统内部业务规则", "本域主数据模型（人事/业务专家）"],
        ["谁是 Master", "近实时 vs 日批", "敏感字段出站控制"],
        ["权限 `hcm-security`", "人事 `hcm-employee-master`", "薪酬 `hcm-payroll`"],
    ),
    (
        "hcm-implementation",
        "hcm-implementation",
        "实施配置与迁移专家",
        "实施",
        "Implementation / Data Migration",
        "Covers parameterized go-live, historical data migration, cutover and acceptance. Use for 实施、上线、迁移、切量、历史数据, or when named 实施 / 实施配置与迁移专家.",
        ["配置项清单与环境策略", "历史数据迁移与校验", "切量窗口与回退", "验收与培训口径"],
        ["产品能力是否该做（总控/业务专家）", "法规解释（合规）"],
        ["并行期双写还是冻结", "主数据清洗标准", "上线后超管变更管控"],
        ["总控 `hcm-product-router`", "人事 `hcm-employee-master`", "多语 `hcm-foundation-i18n`"],
    ),
    (
        "hcm-compliance",
        "hcm-compliance",
        "合规隐私专家",
        "合规",
        "Compliance / Privacy",
        "Covers labor-law related product constraints, personal data protection, cross-border and certificate/archive rules. Use for 合规、个保法、隐私、审计留存、证明开具政策, or when named 合规 / 合规隐私专家.",
        ["用工与档案的合规约束（产品层）", "个人信息最小化与同意口径", "证明/存档/留存周期", "跨境与本地化合规接口"],
        ["具体权限模型实现（权限）", "各地社保缴费参数配置（社保）"],
        ["敏感字段清单", "员工查阅与删除请求", "日志与证据链"],
        ["权限 `hcm-security`", "合同 `hcm-contract`", "多语 `hcm-foundation-i18n`"],
    ),
]


def main():
    for row in EXPERTS:
        write_skill(*row)
    print("ALL_DONE", len(EXPERTS))


if __name__ == "__main__":
    main()
