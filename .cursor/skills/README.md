# HCM 项目级专家群

基于根目录 `HR系统专家分析-市面产品视角.md`。路径：`.cursor/skills/hcm-*/SKILL.md`。

**不需要单独安装插件**；打开本仓库即可。`@` 纠错规则：`.cursor/rules/hcm-expert-invoke.mdc`。

---

## 如何调用

对话点名 **简称**、**全称**、Skill 目录名，或 Cursor `@` 引用专家 Skill，例如：

- `总控拆一下这个需求`
- `人事看档案与雇佣关系`
- `@hcm-payroll` / `薪酬` 评估多次发薪

跨模块先走 **总控**。  
**仅当**你用 `@` 点专家且名称与下表不一致时，Agent 才提示候选让你选；普通聊天未 `@` 不打断纠错。

---

## 简称 ↔ 全称 ↔ Skill（权威对照）

| 简称 | 全称 | Skill 目录 | 产品包 |
|------|------|------------|--------|
| **总控** | HCM 产品总控 | `hcm-product-router` | Insights & Risk / 总控 |
| **组织** | 组织与编制专家 | `hcm-org-position` | Core HR |
| **岗职** | 岗位职级体系专家 | `hcm-job-grade` | Core HR |
| **人事** | 人员主数据专家 | `hcm-employee-master` | Core HR |
| **入职** | 入职与预入职专家 | `hcm-onboarding` | Core HR |
| **异动** | 异动与任职专家 | `hcm-job-change` | Core HR |
| **离职** | 离职与再雇佣专家 | `hcm-offboarding` | Core HR |
| **合同** | 合同与用工关系专家 | `hcm-contract` | Core HR |
| **考勤** | 考勤排班专家 | `hcm-time-attendance` | Time |
| **假期** | 假期与休假专家 | `hcm-leave-absence` | Time |
| **薪酬** | 薪酬专家 | `hcm-payroll` | Pay & Benefits |
| **社保** | 福利与社保公积金专家 | `hcm-benefits` | Pay & Benefits |
| **绩效** | 绩效专家 | `hcm-performance` | Talent |
| **招聘** | 招聘专家 | `hcm-recruiting` | Talent |
| **学习** | 学习发展专家 | `hcm-learning` | Talent |
| **自助** | 员工自助与经理自助专家 | `hcm-ess-mss` | Experience |
| **审批** | 流程审批专家 | `hcm-workflow` | Experience |
| **多语** | 主数据与多语言专家 | `hcm-foundation-i18n` | Platform |
| **权限** | 权限与数据安全专家 | `hcm-security` | Platform |
| **报表** | 报表分析与劳动力洞察专家 | `hcm-analytics` | Insights & Risk |
| **生效** | 规则引擎与生效专家 | `hcm-effective-dating` | Platform |
| **集成** | 集成与生态专家 | `hcm-integration` | Platform |
| **实施** | 实施配置与迁移专家 | `hcm-implementation` | Platform |
| **合规** | 合规隐私专家 | `hcm-compliance` | Insights & Risk |

说明：人员主数据简称是 **人事**，不用「主档」。

---

## 维护说明

- 批量重生（覆盖各 `SKILL.md`）：`python .cursor/skills/_gen-hcm-experts.py`
- 改简称时：同步改本对照表、生成脚本、各 Skill 页眉、以及 `.cursor/rules/hcm-expert-invoke.mdc` 速查行
- 与 `.cursor/rules` 其它规则：Rules 含工程强约束；本专家群为产品域。实现本仓库功能时两者都要遵守
