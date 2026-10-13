# Admin.NET / ehr Database Schema

> Source: SQL Server database `ehr` @ 10.1.2.14
> Extracted: 2026-10-12 13:22:20
> Tables: 88 | Columns: 1674 | Foreign Keys: 0

## TOC

- **02-Plugin-DataApproval** (4)
  - [ApprovalForm](#approvalform) - 审批流表单
  - [ApprovalFormRecord](#approvalformrecord) - 审批流表单记录
  - [DataApproval](#dataapproval) - 数据审批信息表
  - [DataApprovalRecord](#dataapprovalrecord) - 审批流流程记录
- **06-Business-HR-Org** (6)
  - [cc_mstr](#cc_mstr)
  - [cc_mstrback](#cc_mstrback)
  - [code_mstr](#code_mstr)
  - [h_cost](#h_cost)
  - [h_emp_mstr](#h_emp_mstr)
  - [h_shift_set](#h_shift_set)
- **03-Plugin-DingTalk** (5)
  - [ding_talk_dept](#ding_talk_dept) - 钉钉部门表
  - [ding_talk_wokerflow_config](#ding_talk_wokerflow_config) - 审批配置表
  - [ding_talk_wokerflow_log](#ding_talk_wokerflow_log) - 钉钉审批日志
  - [DingTalkRoleUser](#dingtalkroleuser) - 钉钉角色表
  - [DingTalkUser](#dingtalkuser) - 钉钉用户表
- **04-Plugin-GoView** (2)
  - [GoViewPro](#goviewpro) - GoView 项目表
  - [GoViewProData](#goviewprodata) - GoView 项目数据表
- **05-Plugin-AI** (2)
  - [LLMChatHistory](#llmchathistory) - AI聊天历史表
  - [LLMChatSummaryHistory](#llmchatsummaryhistory) - AI聊天摘要历史表
- **01-Admin.NET-Core-Sys** (55)
  - [SysAlipayAuthInfo](#sysalipayauthinfo) - 支付宝授权记录表
  - [SysAlipayTransaction](#sysalipaytransaction) - 支付宝交易记录表
  - [SysCodeGen](#syscodegen) - 代码生成表
  - [SysCodeGenColumn](#syscodegencolumn) - 代码生成表字段配置表
  - [SysCodeGenTable](#syscodegentable) - 代码生成关联表
  - [SysColumnCustom](#syscolumncustom) - Vxe表格列配置
  - [SysConfig](#sysconfig) - 系统配置参数表
  - [SysDictData](#sysdictdata) - 系统字典值表
  - [SysDictType](#sysdicttype) - 系统字典类型表
  - [SysFile](#sysfile) - 系统文件表
  - [SysFileContent](#sysfilecontent) - 系统文件内容表
  - [SysJobCluster](#sysjobcluster) - 系统作业集群表
  - [SysJobDetail](#sysjobdetail) - 系统作业信息表
  - [SysJobTrigger](#sysjobtrigger) - 系统作业触发器表
  - [SysJobTriggerRecord](#sysjobtriggerrecord) - 系统作业触发器运行记录表
  - [SysLdap](#sysldap) - 系统域登录信息配置表
  - [SysLogDiff](#syslogdiff) - 系统差异日志表
  - [SysLogEvent](#syslogevent) - 系统事件日志表
  - [SysLogEx](#syslogex) - 系统异常日志表
  - [SysLogHttp](#sysloghttp) - Http请求日志表
  - [SysLogMsg](#syslogmsg) - 系统消息日志表
  - [SysLogOp](#syslogop) - 系统操作日志表
  - [SysLogVis](#syslogvis) - 系统访问日志表
  - [SysMenu](#sysmenu) - 系统菜单表
  - [SysNotice](#sysnotice) - 系统通知公告表
  - [SysNoticeUser](#sysnoticeuser) - 系统通知公告用户表
  - [SysOAuthUser](#sysoauthuser) - 系统三方用户表
  - [SysOpenAccess](#sysopenaccess) - 开放接口身份表
  - [SysOrg](#sysorg) - 系统机构表
  - [SysPlugin](#sysplugin) - 系统动态插件表
  - [SysPos](#syspos) - 系统职位表
  - [SysPrint](#sysprint) - 系统打印模板表
  - [SysRegion](#sysregion) - 系统行政地区表
  - [SysReportConfig](#sysreportconfig) - 系统报表配置表
  - [SysReportDataSource](#sysreportdatasource) - 系统报表数据源表
  - [SysReportGroup](#sysreportgroup) - 系统报表分组表
  - [SysRole](#sysrole) - 系统角色表
  - [SysRoleApi](#sysroleapi) - 系统角色接口黑名单表
  - [SysRoleMenu](#sysrolemenu) - 系统角色菜单表
  - [SysRoleOrg](#sysroleorg) - 系统角色机构表
  - [SysRoleTable](#sysroletable) - 系统角色表格表
  - [SysSchedule](#sysschedule) - 系统日程表
  - [SysSerial](#sysserial) - 系统流水号表
  - [SysTenant](#systenant) - 系统租户表
  - [SysUpgrade](#sysupgrade) - 系统更新日志表
  - [SysUpgradeUser](#sysupgradeuser) - 系统更新日志用户表
  - [SysUser](#sysuser) - 系统用户表
  - [SysUserExtOrg](#sysuserextorg) - 系统用户扩展机构表
  - [SysUserLdap](#sysuserldap) - 系统用户域配置表
  - [SysUserMenu](#sysusermenu) - 系统用户菜单快捷导航表
  - [SysUserPasswordRecord](#sysuserpasswordrecord) - 系统用户密码记录表
  - [SysUserRegWay](#sysuserregway) - 系统用户注册方案表
  - [SysUserRole](#sysuserrole) - 系统用户角色表
  - [SysWechatPay](#syswechatpay) - 系统微信支付表
  - [SysWechatRefund](#syswechatrefund) - 系统微信退款表
- **07-Demo-Test** (2)
  - [TestCodeGenDemoMaster](#testcodegendemomaster) - 代码自动生成DEMO主表
  - [TestCodeGenDemoSlave](#testcodegendemoslave) - 代码自动生成DEMO子表
- **08-ZeroCode** (12)
  - [zero_database_info](#zero_database_info)
  - [zero_entity_column_info](#zero_entity_column_info)
  - [zero_entity_info](#zero_entity_info)
  - [zero_interface_category](#zero_interface_category)
  - [zero_interface_list](#zero_interface_list)
  - [zero_jwt_token_management](#zero_jwt_token_management)
  - [zero_permission_info](#zero_permission_info)
  - [zero_permission_mapping](#zero_permission_mapping)
  - [zero_sys_setting](#zero_sys_setting)
  - [zero_template](#zero_template)
  - [zero_template_type](#zero_template_type)
  - [zero_user_info](#zero_user_info)

## Table Catalog

| Category | Table | Description | Rows | Columns |
|----------|-------|-------------|------|---------|
| 02-Plugin-DataApproval | `ApprovalForm` | 审批流表单 | 0 | 19 |
| 02-Plugin-DataApproval | `ApprovalFormRecord` | 审批流表单记录 | 0 | 18 |
| 02-Plugin-DataApproval | `DataApproval` | 数据审批信息表 | 0 | 17 |
| 02-Plugin-DataApproval | `DataApprovalRecord` | 审批流流程记录 | 0 | 17 |
| 06-Business-HR-Org | `cc_mstr` |  | 16 | 32 |
| 06-Business-HR-Org | `cc_mstrback` |  | 15 | 32 |
| 06-Business-HR-Org | `code_mstr` |  | 6 | 4 |
| 06-Business-HR-Org | `h_cost` |  | 0 | 4 |
| 06-Business-HR-Org | `h_emp_mstr` |  | 262 | 216 |
| 06-Business-HR-Org | `h_shift_set` |  | 0 | 4 |
| 03-Plugin-DingTalk | `ding_talk_dept` | 钉钉部门表 | 0 | 5 |
| 03-Plugin-DingTalk | `ding_talk_wokerflow_config` | 审批配置表 | 0 | 2 |
| 03-Plugin-DingTalk | `ding_talk_wokerflow_log` | 钉钉审批日志 | 0 | 12 |
| 03-Plugin-DingTalk | `DingTalkRoleUser` | 钉钉角色表 | 0 | 14 |
| 03-Plugin-DingTalk | `DingTalkUser` | 钉钉用户表 | 0 | 20 |
| 04-Plugin-GoView | `GoViewPro` | GoView 项目表 | 0 | 14 |
| 04-Plugin-GoView | `GoViewProData` | GoView 项目数据表 | 0 | 13 |
| 05-Plugin-AI | `LLMChatHistory` | AI聊天历史表 | 0 | 6 |
| 05-Plugin-AI | `LLMChatSummaryHistory` | AI聊天摘要历史表 | 0 | 5 |
| 01-Admin.NET-Core-Sys | `SysAlipayAuthInfo` | 支付宝授权记录表 | 0 | 31 |
| 01-Admin.NET-Core-Sys | `SysAlipayTransaction` | 支付宝交易记录表 | 0 | 24 |
| 01-Admin.NET-Core-Sys | `SysCodeGen` | 代码生成表 | 0 | 25 |
| 01-Admin.NET-Core-Sys | `SysCodeGenColumn` | 代码生成表字段配置表 | 0 | 32 |
| 01-Admin.NET-Core-Sys | `SysCodeGenTable` | 代码生成关联表 | 0 | 17 |
| 01-Admin.NET-Core-Sys | `SysColumnCustom` | Vxe表格列配置 | 0 | 8 |
| 01-Admin.NET-Core-Sys | `SysConfig` | 系统配置参数表 | 27 | 16 |
| 01-Admin.NET-Core-Sys | `SysDictData` | 系统字典值表 | 628 | 21 |
| 01-Admin.NET-Core-Sys | `SysDictType` | 系统字典类型表 | 62 | 16 |
| 01-Admin.NET-Core-Sys | `SysFile` | 系统文件表 | 0 | 25 |
| 01-Admin.NET-Core-Sys | `SysFileContent` | 系统文件内容表 | 0 | 11 |
| 01-Admin.NET-Core-Sys | `SysJobCluster` | 系统作业集群表 | 0 | 5 |
| 01-Admin.NET-Core-Sys | `SysJobDetail` | 系统作业信息表 | 5 | 12 |
| 01-Admin.NET-Core-Sys | `SysJobTrigger` | 系统作业触发器表 | 4 | 22 |
| 01-Admin.NET-Core-Sys | `SysJobTriggerRecord` | 系统作业触发器运行记录表 | 4 | 10 |
| 01-Admin.NET-Core-Sys | `SysLdap` | 系统域登录信息配置表 | 0 | 21 |
| 01-Admin.NET-Core-Sys | `SysLogDiff` | 系统差异日志表 | 0 | 12 |
| 01-Admin.NET-Core-Sys | `SysLogEvent` | 系统事件日志表 | 0 | 23 |
| 01-Admin.NET-Core-Sys | `SysLogEx` | 系统异常日志表 | 21 | 29 |
| 01-Admin.NET-Core-Sys | `SysLogHttp` | Http请求日志表 | 0 | 22 |
| 01-Admin.NET-Core-Sys | `SysLogMsg` | 系统消息日志表 | 0 | 22 |
| 01-Admin.NET-Core-Sys | `SysLogOp` | 系统操作日志表 | 591 | 29 |
| 01-Admin.NET-Core-Sys | `SysLogVis` | 系统访问日志表 | 23 | 20 |
| 01-Admin.NET-Core-Sys | `SysMenu` | 系统菜单表 | 212 | 26 |
| 01-Admin.NET-Core-Sys | `SysNotice` | 系统通知公告表 | 1 | 20 |
| 01-Admin.NET-Core-Sys | `SysNoticeUser` | 系统通知公告用户表 | 0 | 5 |
| 01-Admin.NET-Core-Sys | `SysOAuthUser` | 系统三方用户表 | 0 | 30 |
| 01-Admin.NET-Core-Sys | `SysOpenAccess` | 开放接口身份表 | 0 | 13 |
| 01-Admin.NET-Core-Sys | `SysOrg` | 系统机构表 | 5 | 21 |
| 01-Admin.NET-Core-Sys | `SysPlugin` | 系统动态插件表 | 0 | 16 |
| 01-Admin.NET-Core-Sys | `SysPos` | 系统职位表 | 4 | 16 |
| 01-Admin.NET-Core-Sys | `SysPrint` | 系统打印模板表 | 0 | 19 |
| 01-Admin.NET-Core-Sys | `SysRegion` | 系统行政地区表 | 0 | 15 |
| 01-Admin.NET-Core-Sys | `SysReportConfig` | 系统报表配置表 | 0 | 20 |
| 01-Admin.NET-Core-Sys | `SysReportDataSource` | 系统报表数据源表 | 0 | 13 |
| 01-Admin.NET-Core-Sys | `SysReportGroup` | 系统报表分组表 | 0 | 11 |
| 01-Admin.NET-Core-Sys | `SysRole` | 系统角色表 | 5 | 17 |
| 01-Admin.NET-Core-Sys | `SysRoleApi` | 系统角色接口黑名单表 | 0 | 3 |
| 01-Admin.NET-Core-Sys | `SysRoleMenu` | 系统角色菜单表 | 211 | 3 |
| 01-Admin.NET-Core-Sys | `SysRoleOrg` | 系统角色机构表 | 0 | 3 |
| 01-Admin.NET-Core-Sys | `SysRoleTable` | 系统角色表格表 | 0 | 4 |
| 01-Admin.NET-Core-Sys | `SysSchedule` | 系统日程表 | 0 | 16 |
| 01-Admin.NET-Core-Sys | `SysSerial` | 系统流水号表 | 1 | 20 |
| 01-Admin.NET-Core-Sys | `SysTenant` | 系统租户表 | 1 | 36 |
| 01-Admin.NET-Core-Sys | `SysUpgrade` | 系统更新日志表 | 0 | 10 |
| 01-Admin.NET-Core-Sys | `SysUpgradeUser` | 系统更新日志用户表 | 0 | 4 |
| 01-Admin.NET-Core-Sys | `SysUser` | 系统用户表 | 5 | 53 |
| 01-Admin.NET-Core-Sys | `SysUserExtOrg` | 系统用户扩展机构表 | 1 | 7 |
| 01-Admin.NET-Core-Sys | `SysUserLdap` | 系统用户域配置表 | 0 | 20 |
| 01-Admin.NET-Core-Sys | `SysUserMenu` | 系统用户菜单快捷导航表 | 0 | 3 |
| 01-Admin.NET-Core-Sys | `SysUserPasswordRecord` | 系统用户密码记录表 | 0 | 3 |
| 01-Admin.NET-Core-Sys | `SysUserRegWay` | 系统用户注册方案表 | 0 | 17 |
| 01-Admin.NET-Core-Sys | `SysUserRole` | 系统用户角色表 | 4 | 3 |
| 01-Admin.NET-Core-Sys | `SysWechatPay` | 系统微信支付表 | 0 | 37 |
| 01-Admin.NET-Core-Sys | `SysWechatRefund` | 系统微信退款表 | 0 | 28 |
| 07-Demo-Test | `TestCodeGenDemoMaster` | 代码自动生成DEMO主表 | 0 | 14 |
| 07-Demo-Test | `TestCodeGenDemoSlave` | 代码自动生成DEMO子表 | 0 | 23 |
| 08-ZeroCode | `zero_database_info` |  | 1 | 16 |
| 08-ZeroCode | `zero_entity_column_info` |  | 172 | 29 |
| 08-ZeroCode | `zero_entity_info` |  | 9 | 18 |
| 08-ZeroCode | `zero_interface_category` |  | 15 | 18 |
| 08-ZeroCode | `zero_interface_list` |  | 59 | 23 |
| 08-ZeroCode | `zero_jwt_token_management` |  | 0 | 17 |
| 08-ZeroCode | `zero_permission_info` |  | 0 | 14 |
| 08-ZeroCode | `zero_permission_mapping` |  | 0 | 16 |
| 08-ZeroCode | `zero_sys_setting` |  | 2 | 17 |
| 08-ZeroCode | `zero_template` |  | 2 | 18 |
| 08-ZeroCode | `zero_template_type` |  | 0 | 14 |
| 08-ZeroCode | `zero_user_info` |  | 1 | 18 |

## Table Details

### 02-Plugin-DataApproval

#### ApprovalForm (审批流表单)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Code` | `nvarchar(32)` | YES |  |  |  | 编号 |
| 3 | `Name` | `nvarchar(32)` | YES |  |  |  | 名称 |
| 4 | `FormName` | `nvarchar(32)` | YES |  |  |  | 表单名称 |
| 5 | `FormType` | `nvarchar(32)` | YES |  |  |  | 表单属性 |
| 6 | `FormStatus` | `int` | YES |  |  |  | 表单状态 |
| 7 | `FormResult` | `nvarchar(MAX)` | YES |  |  |  | 表单结果 |
| 8 | `Status` | `int` | YES |  |  |  | 状态 |
| 9 | `Remark` | `nvarchar(255)` | YES |  |  |  | 备注 |
| 10 | `CreateOrgId` | `bigint` | YES |  |  |  | 创建者机构Id |
| 11 | `CreateOrgName` | `nvarchar(64)` | YES |  |  |  | 创建者机构名称 |
| 12 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 13 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 14 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 15 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 16 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 17 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 18 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 19 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_ApprovalForm_Id` | CLUSTERED |  |  | `Id` |

#### ApprovalFormRecord (审批流表单记录)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `FlowId` | `bigint` | YES |  |  |  | 流程Id |
| 3 | `FormName` | `nvarchar(32)` | YES |  |  |  | 表单名称 |
| 4 | `FormType` | `nvarchar(32)` | YES |  |  |  | 表单类型 |
| 5 | `FormStatus` | `nvarchar(11)` | YES |  |  |  | 表单状态 |
| 6 | `FormBefore` | `nvarchar(MAX)` | YES |  |  |  | 修改前 |
| 7 | `FormAfter` | `nvarchar(MAX)` | YES |  |  |  | 修改后 |
| 8 | `FormResult` | `nvarchar(MAX)` | YES |  |  |  | 表单结果 |
| 9 | `CreateOrgId` | `bigint` | YES |  |  |  | 创建者机构Id |
| 10 | `CreateOrgName` | `nvarchar(64)` | YES |  |  |  | 创建者机构名称 |
| 11 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 12 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 13 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 14 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 15 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 16 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 17 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 18 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_ApprovalFormRecord_Id` | CLUSTERED |  |  | `Id` |

#### DataApproval (数据审批信息表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Code` | `nvarchar(32)` | YES |  |  |  | 编号 |
| 3 | `Name` | `nvarchar(32)` | NO |  |  |  | 名称 |
| 4 | `FormJson` | `nvarchar(MAX)` | YES |  |  |  | 表单 |
| 5 | `FlowJson` | `nvarchar(MAX)` | YES |  |  |  | 流程 |
| 6 | `Status` | `int` | YES |  |  |  | 状态 |
| 7 | `Remark` | `nvarchar(256)` | YES |  |  |  | 备注 |
| 8 | `CreateOrgId` | `bigint` | YES |  |  |  | 创建者机构Id |
| 9 | `CreateOrgName` | `nvarchar(64)` | YES |  |  |  | 创建者机构名称 |
| 10 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 11 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 12 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 13 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 14 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 15 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 16 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 17 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_DataApproval_Id` | CLUSTERED |  |  | `Id` |

#### DataApprovalRecord (审批流流程记录)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `FormName` | `nvarchar(255)` | YES |  |  |  | 表单名称 |
| 3 | `FormStatus` | `nvarchar(32)` | YES |  |  |  | 表单状态 |
| 4 | `FormJson` | `nvarchar(MAX)` | YES |  |  |  | 表单触发 |
| 5 | `FormResult` | `nvarchar(MAX)` | YES |  |  |  | 表单结果 |
| 6 | `FlowJson` | `nvarchar(MAX)` | YES |  |  |  | 流程结构 |
| 7 | `FlowResult` | `nvarchar(MAX)` | YES |  |  |  | 流程结果 |
| 8 | `CreateOrgId` | `bigint` | YES |  |  |  | 创建者机构Id |
| 9 | `CreateOrgName` | `nvarchar(64)` | YES |  |  |  | 创建者机构名称 |
| 10 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 11 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 12 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 13 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 14 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 15 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 16 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 17 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_DataApprovalRecord_Id` | CLUSTERED |  |  | `Id` |

### 06-Business-HR-Org

#### cc_mstr

- Schema: `dbo`
- Rows: 16

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `cc_code` | `nvarchar(50)` | NO | Y |  |  |  |
| 2 | `cc_code_parent` | `nvarchar(50)` | YES |  |  |  |  |
| 3 | `cc_dept` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `cc_desc` | `nvarchar(122)` | NO |  |  |  |  |
| 5 | `cc_level` | `nvarchar(50)` | NO |  |  |  |  |
| 6 | `cc_group` | `int` | YES |  |  |  |  |
| 7 | `cc_indet` | `nvarchar(50)` | YES |  |  |  |  |
| 8 | `cc_sub` | `float` | YES |  |  |  |  |
| 9 | `cc_site` | `nvarchar(50)` | YES |  |  |  |  |
| 10 | `cc_state` | `int` | YES |  |  |  |  |
| 11 | `cc_cost` | `nvarchar(50)` | YES |  |  |  |  |
| 12 | `cc_wage_rate` | `float` | YES |  |  |  |  |
| 13 | `cc_stat_basic` | `nvarchar(50)` | YES |  |  |  |  |
| 14 | `cc_enable` | `int` | YES |  |  | ((0)) |  |
| 15 | `cc_dele_date` | `datetime` | YES |  |  |  |  |
| 16 | `cc_count_group` | `nvarchar(60)` | YES |  |  |  |  |
| 17 | `cc_user` | `nvarchar(50)` | YES |  |  |  |  |
| 18 | `cc_date` | `datetime` | YES |  |  |  |  |
| 19 | `cc_kqgroup` | `int` | YES |  |  |  |  |
| 20 | `cc_woca` | `nvarchar(50)` | YES |  |  |  |  |
| 21 | `cc_bz_unit` | `nvarchar(50)` | YES |  |  |  |  |
| 22 | `cc_dele_remark` | `nvarchar(120)` | YES |  |  |  |  |
| 23 | `cc_desc_en` | `nvarchar(150)` | YES |  |  |  |  |
| 24 | `cc_level_censor` | `nvarchar(50)` | YES |  |  |  |  |
| 25 | `cc_xz_group` | `nvarchar(50)` | YES |  |  |  |  |
| 26 | `cc_create_date` | `datetime` | YES |  |  |  |  |
| 27 | `machine_floor` | `nvarchar(10)` | YES |  |  |  |  |
| 28 | `kq_new_sysuser` | `nvarchar(25)` | YES |  |  |  |  |
| 29 | `kq_new_sysdate` | `datetime` | YES |  |  |  |  |
| 30 | `ID` | `int` | NO |  | Y |  |  |
| 31 | `ParentID` | `int` | YES |  |  |  |  |
| 32 | `cc_code2` | `nvarchar(50)` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `IX_cc_mstr_cc_code_parent` | NONCLUSTERED |  |  | `cc_code_parent` |
| `IX_cc_mstr_cc_enable` | NONCLUSTERED |  |  | `cc_enable` |
| `IX_cc_mstr_cc_level` | NONCLUSTERED |  |  | `cc_level` |
| `IX_cc_mstr_ParentID` | NONCLUSTERED |  |  | `ParentID` |
| `PK_cc_mstr` | CLUSTERED |  |  | `cc_code` |
| `UX_cc_mstr_ID` | NONCLUSTERED |  |  | `ID` |

#### cc_mstrback

- Schema: `dbo`
- Rows: 15

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `cc_code` | `nvarchar(50)` | NO |  |  |  |  |
| 2 | `cc_code_parent` | `nvarchar(50)` | YES |  |  |  |  |
| 3 | `cc_dept` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `cc_desc` | `nvarchar(122)` | NO |  |  |  |  |
| 5 | `cc_level` | `nvarchar(50)` | NO |  |  |  |  |
| 6 | `cc_group` | `int` | YES |  |  |  |  |
| 7 | `cc_indet` | `nvarchar(50)` | YES |  |  |  |  |
| 8 | `cc_sub` | `float` | YES |  |  |  |  |
| 9 | `cc_site` | `nvarchar(50)` | YES |  |  |  |  |
| 10 | `cc_state` | `int` | YES |  |  |  |  |
| 11 | `cc_cost` | `nvarchar(50)` | YES |  |  |  |  |
| 12 | `cc_wage_rate` | `float` | YES |  |  |  |  |
| 13 | `cc_stat_basic` | `nvarchar(50)` | YES |  |  |  |  |
| 14 | `cc_enable` | `int` | YES |  |  |  |  |
| 15 | `cc_dele_date` | `datetime` | YES |  |  |  |  |
| 16 | `cc_count_group` | `nvarchar(60)` | YES |  |  |  |  |
| 17 | `cc_user` | `nvarchar(50)` | YES |  |  |  |  |
| 18 | `cc_date` | `datetime` | YES |  |  |  |  |
| 19 | `cc_kqgroup` | `int` | YES |  |  |  |  |
| 20 | `cc_woca` | `nvarchar(50)` | YES |  |  |  |  |
| 21 | `cc_bz_unit` | `nvarchar(50)` | YES |  |  |  |  |
| 22 | `cc_dele_remark` | `nvarchar(120)` | YES |  |  |  |  |
| 23 | `cc_desc_en` | `nvarchar(150)` | YES |  |  |  |  |
| 24 | `cc_level_censor` | `nvarchar(50)` | YES |  |  |  |  |
| 25 | `cc_xz_group` | `nvarchar(50)` | YES |  |  |  |  |
| 26 | `cc_create_date` | `datetime` | YES |  |  |  |  |
| 27 | `machine_floor` | `nvarchar(10)` | YES |  |  |  |  |
| 28 | `kq_new_sysuser` | `nvarchar(25)` | YES |  |  |  |  |
| 29 | `kq_new_sysdate` | `datetime` | YES |  |  |  |  |
| 30 | `ID` | `int` | NO |  | Y |  |  |
| 31 | `ParentID` | `int` | YES |  |  |  |  |
| 32 | `cc_code2` | `nvarchar(50)` | YES |  |  |  |  |

#### code_mstr

- Schema: `dbo`
- Rows: 6

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `code_field` | `nvarchar(50)` | NO | Y |  |  |  |
| 2 | `code_value` | `nvarchar(50)` | NO | Y |  |  |  |
| 3 | `code_desc` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `code_sort` | `nvarchar(50)` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_code_mstr` | CLUSTERED |  |  | `code_field, code_value` |

#### h_cost

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `cost_id` | `nvarchar(50)` | NO | Y |  |  |  |
| 2 | `cost_name` | `nvarchar(100)` | YES |  |  |  |  |
| 3 | `cost_site` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `cost_state` | `nvarchar(10)` | YES |  |  | (N'Y') |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_h_cost` | CLUSTERED |  |  | `cost_id` |

#### h_emp_mstr

- Schema: `dbo`
- Rows: 262

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `iempid` | `bigint` | NO | Y |  |  |  |
| 2 | `emp_id` | `nvarchar(50)` | NO | Y |  |  |  |
| 3 | `emp_old_id` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `cc_code` | `nvarchar(50)` | YES |  |  |  |  |
| 5 | `duty_id` | `nvarchar(50)` | YES |  |  |  |  |
| 6 | `post_id` | `nvarchar(50)` | YES |  |  |  |  |
| 7 | `emp_name` | `nvarchar(100)` | YES |  |  |  |  |
| 8 | `emp_name_en` | `nvarchar(100)` | YES |  |  |  |  |
| 9 | `emp_name_py` | `nvarchar(100)` | YES |  |  |  |  |
| 10 | `emp_ic` | `nvarchar(50)` | YES |  |  |  |  |
| 11 | `emp_ic_old` | `nvarchar(50)` | YES |  |  |  |  |
| 12 | `emp_pass` | `nvarchar(50)` | YES |  |  |  |  |
| 13 | `emp_state` | `int` | YES |  |  | ((1)) |  |
| 14 | `emp_free` | `tinyint` | YES |  |  | ((0)) |  |
| 15 | `emp_class` | `int` | YES |  |  | ((0)) |  |
| 16 | `emp_ov` | `int` | YES |  |  | ((0)) |  |
| 17 | `emp_in_date` | `datetime` | YES |  |  |  |  |
| 18 | `emp_beg_date` | `datetime` | YES |  |  |  |  |
| 19 | `emp_duty_date` | `datetime` | YES |  |  |  |  |
| 20 | `emp_stay_date` | `datetime` | YES |  |  |  |  |
| 21 | `emp_back_date` | `datetime` | YES |  |  |  |  |
| 22 | `emp_lea_date` | `datetime` | YES |  |  |  |  |
| 23 | `emp_lea_xz` | `int` | YES |  |  | ((0)) |  |
| 24 | `emp_dim_type` | `nvarchar(50)` | YES |  |  |  |  |
| 25 | `emp_try` | `nvarchar(2)` | YES |  |  |  |  |
| 26 | `emp_try_long` | `smallint` | YES |  |  |  |  |
| 27 | `emp_try_date` | `datetime` | YES |  |  |  |  |
| 28 | `emp_from_date` | `datetime` | YES |  |  |  |  |
| 29 | `emp_change_oldcc` | `nvarchar(50)` | YES |  |  |  |  |
| 30 | `emp_basic` | `nvarchar(6)` | YES |  |  |  |  |
| 31 | `emp_grade` | `int` | YES |  |  |  |  |
| 32 | `emp_role` | `int` | YES |  |  |  |  |
| 33 | `emp_sal` | `nvarchar(2)` | YES |  |  | ('%') |  |
| 34 | `emp_sal_type` | `nvarchar(4)` | YES |  |  |  |  |
| 35 | `emp_cost` | `nvarchar(50)` | YES |  |  |  |  |
| 36 | `emp_zj` | `nvarchar(4)` | YES |  |  |  |  |
| 37 | `emp_check` | `nvarchar(2)` | YES |  |  |  |  |
| 38 | `emp_con_beg` | `datetime` | YES |  |  |  |  |
| 39 | `emp_con_end` | `datetime` | YES |  |  |  |  |
| 40 | `emp_ht_state` | `int` | YES |  |  | ((0)) |  |
| 41 | `emp_ht_userdate` | `datetime` | YES |  |  |  |  |
| 42 | `emp_assure` | `nvarchar(50)` | YES |  |  |  |  |
| 43 | `emp_assure_refer` | `nvarchar(50)` | YES |  |  |  |  |
| 44 | `emp_sex` | `nvarchar(20)` | YES |  |  |  |  |
| 45 | `emp_card` | `nvarchar(50)` | YES |  |  |  |  |
| 46 | `emp_birth` | `datetime` | YES |  |  |  |  |
| 47 | `emp_rgt` | `nvarchar(4)` | YES |  |  |  |  |
| 48 | `emp_province` | `nvarchar(50)` | YES |  |  |  |  |
| 49 | `emp_residence` | `nvarchar(80)` | YES |  |  |  |  |
| 50 | `emp_face` | `nvarchar(50)` | YES |  |  |  |  |
| 51 | `emp_nation` | `nvarchar(50)` | YES |  |  |  |  |
| 52 | `emp_marry` | `nvarchar(20)` | YES |  |  |  |  |
| 53 | `emp_union` | `nvarchar(2)` | YES |  |  |  |  |
| 54 | `emp_height` | `real` | YES |  |  |  |  |
| 55 | `emp_weight` | `real` | YES |  |  |  |  |
| 56 | `emp_blood` | `nvarchar(2)` | YES |  |  |  |  |
| 57 | `emp_home` | `nvarchar(100)` | YES |  |  |  |  |
| 58 | `emp_addr` | `nvarchar(100)` | YES |  |  |  |  |
| 59 | `emp_hometel` | `nvarchar(50)` | YES |  |  |  |  |
| 60 | `emp_addrter` | `nvarchar(50)` | YES |  |  |  |  |
| 61 | `emp_email` | `nvarchar(50)` | YES |  |  |  |  |
| 62 | `emp_school` | `nvarchar(80)` | YES |  |  |  |  |
| 63 | `emp_school2` | `nvarchar(80)` | YES |  |  |  |  |
| 64 | `emp_grad_time` | `datetime` | YES |  |  |  |  |
| 65 | `emp_grad_time2` | `datetime` | YES |  |  |  |  |
| 66 | `emp_level` | `nvarchar(50)` | YES |  |  |  |  |
| 67 | `emp_level2` | `nvarchar(50)` | YES |  |  |  |  |
| 68 | `emp_major` | `nvarchar(50)` | YES |  |  |  |  |
| 69 | `emp_major2` | `nvarchar(50)` | YES |  |  |  |  |
| 70 | `emp_language` | `nvarchar(50)` | YES |  |  |  |  |
| 71 | `emp_lang_level` | `nvarchar(50)` | YES |  |  |  |  |
| 72 | `emp_special` | `nvarchar(50)` | YES |  |  |  |  |
| 73 | `emp_computer` | `nvarchar(4)` | YES |  |  |  |  |
| 74 | `emp_nz` | `int` | YES |  |  |  |  |
| 75 | `emp_long` | `nvarchar(50)` | YES |  |  |  |  |
| 76 | `emp_longmon` | `int` | YES |  |  |  |  |
| 77 | `emp_longtype` | `int` | YES |  |  |  |  |
| 78 | `emp_old` | `int` | YES |  |  |  |  |
| 79 | `emp_oldtype` | `nvarchar(50)` | YES |  |  |  |  |
| 80 | `emp_dorm` | `nvarchar(2)` | YES |  |  |  |  |
| 81 | `emp_dormid` | `nvarchar(50)` | YES |  |  |  |  |
| 82 | `emp_stransp` | `nvarchar(2)` | YES |  |  |  |  |
| 83 | `emp_jycode` | `nvarchar(50)` | YES |  |  |  |  |
| 84 | `emp_zzcode` | `nvarchar(50)` | YES |  |  |  |  |
| 85 | `emp_sjcode` | `nvarchar(50)` | YES |  |  |  |  |
| 86 | `emp_by` | `nvarchar(2)` | YES |  |  |  |  |
| 87 | `emp_tjb` | `nvarchar(2)` | YES |  |  |  |  |
| 88 | `emp_sinsu` | `nvarchar(50)` | YES |  |  |  |  |
| 89 | `emp_einsu` | `nvarchar(50)` | YES |  |  |  |  |
| 90 | `emp_jy_date` | `datetime` | YES |  |  |  |  |
| 91 | `emp_zz_date` | `datetime` | YES |  |  |  |  |
| 92 | `emp_sj_date` | `datetime` | YES |  |  |  |  |
| 93 | `emp_flod_date` | `datetime` | YES |  |  |  |  |
| 94 | `emp_con_state` | `nvarchar(50)` | YES |  |  |  |  |
| 95 | `emp_back` | `int` | YES |  |  |  |  |
| 96 | `emp_work_hist` | `text(2147483647)` | YES |  |  |  |  |
| 97 | `emp_user` | `nvarchar(50)` | YES |  |  |  |  |
| 98 | `emp_time` | `datetime` | YES |  |  |  |  |
| 99 | `emp_text` | `nvarchar(50)` | YES |  |  |  |  |
| 100 | `emp_dec` | `decimal(8,2)` | YES |  |  |  |  |
| 101 | `emp_date` | `datetime` | YES |  |  |  |  |
| 102 | `emp_repaytemp` | `decimal(10,2)` | YES |  |  |  |  |
| 103 | `emp_inport_cc` | `nvarchar(50)` | YES |  |  |  |  |
| 104 | `emp_bx` | `int` | YES |  |  |  |  |
| 105 | `emp_dina` | `nvarchar(2)` | YES |  |  |  |  |
| 106 | `emp_con_man` | `nvarchar(50)` | YES |  |  |  |  |
| 107 | `emp_con_relative` | `nvarchar(50)` | YES |  |  |  |  |
| 108 | `emp_con_tel` | `nvarchar(50)` | YES |  |  |  |  |
| 109 | `emp_intro_man` | `nvarchar(50)` | YES |  |  |  |  |
| 110 | `emp_hire_type` | `nvarchar(4)` | YES |  |  |  |  |
| 111 | `emp_dorm_in` | `datetime` | YES |  |  |  |  |
| 112 | `emp_dorm_out` | `datetime` | YES |  |  |  |  |
| 113 | `emp_dorm_money` | `decimal(10,2)` | YES |  |  | ((0)) |  |
| 114 | `emp_dorm_remark` | `nvarchar(50)` | YES |  |  |  |  |
| 115 | `emp_marri_date` | `datetime` | YES |  |  |  |  |
| 116 | `emp_bear_date1` | `datetime` | YES |  |  |  |  |
| 117 | `emp_bear_sex1` | `nvarchar(20)` | YES |  |  |  |  |
| 118 | `emp_bear_date2` | `datetime` | YES |  |  |  |  |
| 119 | `emp_bear_sex2` | `nvarchar(20)` | YES |  |  |  |  |
| 120 | `emp_unbear_way` | `nvarchar(50)` | YES |  |  |  |  |
| 121 | `emp_bear_rmk` | `nvarchar(255)` | YES |  |  |  |  |
| 122 | `emp_bonus_dot` | `int` | YES |  |  |  |  |
| 123 | `emp_bonus_type` | `nvarchar(4)` | YES |  |  |  |  |
| 124 | `emp_hour_id` | `nvarchar(50)` | YES |  |  |  |  |
| 125 | `emp_piece_id` | `nvarchar(50)` | YES |  |  |  |  |
| 126 | `emp_salary_type` | `nvarchar(50)` | YES |  |  |  |  |
| 127 | `emp_kq_lea_date` | `datetime` | YES |  |  |  |  |
| 128 | `emp_kq_indate` | `datetime` | YES |  |  |  |  |
| 129 | `emp_remark` | `nvarchar(255)` | YES |  |  |  |  |
| 130 | `emp_create_date` | `datetime` | YES |  |  |  |  |
| 131 | `emp_create_user` | `nvarchar(50)` | YES |  |  |  |  |
| 132 | `emp_leaxz_stopmon` | `nvarchar(6)` | YES |  |  |  |  |
| 133 | `emp_leaxz_endmon` | `nvarchar(6)` | YES |  |  |  |  |
| 134 | `emp_card_date` | `datetime` | YES |  |  |  |  |
| 135 | `emp_card_limit` | `int` | YES |  |  |  |  |
| 136 | `emp_ss` | `nvarchar(50)` | YES |  |  |  |  |
| 137 | `emp_lzbz` | `nvarchar(50)` | YES |  |  |  |  |
| 138 | `emp_assure_tel` | `nvarchar(50)` | YES |  |  |  |  |
| 139 | `emp_rmk` | `nvarchar(255)` | YES |  |  |  |  |
| 140 | `emp_stat_basic` | `nvarchar(50)` | YES |  |  |  |  |
| 141 | `emp_ht_flg` | `int` | YES |  |  | ((1)) |  |
| 142 | `emp_type` | `nvarchar(50)` | YES |  |  |  |  |
| 143 | `emp_rc_type` | `nvarchar(50)` | YES |  |  |  |  |
| 144 | `emp_zc_type` | `nvarchar(50)` | YES |  |  |  |  |
| 145 | `emp_kq_autoclass` | `int` | YES |  |  |  |  |
| 146 | `emp_censor_enable` | `int` | YES |  |  | ((0)) |  |
| 147 | `emp_pic` | `nvarchar(255)` | YES |  |  |  |  |
| 148 | `emp_dim_flg` | `int` | YES |  |  |  |  |
| 149 | `emp_dim_time` | `int` | YES |  |  |  |  |
| 150 | `emp_officetel` | `nvarchar(50)` | YES |  |  |  |  |
| 151 | `emp_image` | `image(2147483647)` | YES |  |  |  |  |
| 152 | `emp_bx_flag` | `nvarchar(1)` | YES |  |  | ((1)) |  |
| 153 | `emp_house_flag` | `nvarchar(1)` | YES |  |  | ((1)) |  |
| 154 | `emp_ht_type` | `nvarchar(50)` | YES |  |  |  |  |
| 155 | `emp_ht_enabled` | `int` | YES |  |  |  |  |
| 156 | `emp_ht_overdate` | `datetime` | YES |  |  |  |  |
| 157 | `emp_jyhave` | `nvarchar(50)` | YES |  |  | ((0)) |  |
| 158 | `emp_card_overdate` | `datetime` | YES |  |  |  |  |
| 159 | `emp_censorgflag` | `int` | YES |  |  |  |  |
| 160 | `emp_otflag` | `int` | YES |  |  |  |  |
| 161 | `emp_class_censor` | `int` | YES |  |  |  |  |
| 162 | `emp_ic_zm200id` | `int` | YES |  |  | ((0)) |  |
| 163 | `emp_bx_month` | `nchar(10)` | YES |  |  |  |  |
| 164 | `emp_work_date` | `datetime` | YES |  |  |  |  |
| 165 | `emp_diary_maxdate` | `datetime` | YES |  |  |  |  |
| 166 | `emp_diary_abnday` | `int` | YES |  |  |  |  |
| 167 | `emp_diary_abndate` | `datetime` | YES |  |  |  |  |
| 168 | `emp_finger_pass` | `nvarchar(50)` | YES |  |  | ('') |  |
| 169 | `emp_finger_privilege` | `int` | YES |  |  | ((0)) |  |
| 170 | `emp_kqmc_group` | `nvarchar(50)` | YES |  |  | ('A') |  |
| 171 | `emp_kaohe` | `tinyint` | YES |  |  | ((1)) |  |
| 172 | `emp_lea_cc` | `nvarchar(50)` | YES |  |  |  |  |
| 173 | `emp_ovtype` | `int` | YES |  |  |  |  |
| 174 | `emp_char1` | `nvarchar(50)` | YES |  |  |  |  |
| 175 | `emp_char2` | `nvarchar(50)` | YES |  |  |  |  |
| 176 | `emp_char3` | `nvarchar(50)` | YES |  |  |  |  |
| 177 | `emp_char4` | `nvarchar(50)` | YES |  |  |  |  |
| 178 | `emp_char5` | `nvarchar(50)` | YES |  |  |  |  |
| 179 | `emp_date1` | `datetime` | YES |  |  |  |  |
| 180 | `emp_date2` | `datetime` | YES |  |  |  |  |
| 181 | `emp_date3` | `datetime` | YES |  |  |  |  |
| 182 | `emp_item1` | `decimal(9,2)` | YES |  |  |  |  |
| 183 | `emp_item2` | `decimal(9,2)` | YES |  |  |  |  |
| 184 | `emp_item3` | `decimal(9,2)` | YES |  |  |  |  |
| 185 | `emp_valid` | `datetime` | YES |  |  |  |  |
| 186 | `emp_stop_sysuser` | `nvarchar(50)` | YES |  |  |  |  |
| 187 | `emp_stop_sysdate` | `datetime` | YES |  |  |  |  |
| 188 | `emp_char6` | `nvarchar(50)` | YES |  |  |  |  |
| 189 | `emp_in_date_team` | `datetime` | YES |  |  |  |  |
| 190 | `emp_char7` | `nvarchar(50)` | YES |  |  |  |  |
| 191 | `emp_woca` | `nvarchar(10)` | YES |  |  |  |  |
| 192 | `bs_id` | `int` | NO |  | Y |  |  |
| 193 | `DeptID` | `int` | YES |  |  |  |  |
| 194 | `PostID` | `int` | YES |  |  |  |  |
| 195 | `emp_char8` | `nvarchar(50)` | YES |  |  |  |  |
| 196 | `emp_char9` | `nvarchar(50)` | YES |  |  |  |  |
| 197 | `emp_char10` | `nvarchar(50)` | YES |  |  |  |  |
| 198 | `emp_char11` | `nvarchar(50)` | YES |  |  |  |  |
| 199 | `emp_char12` | `nvarchar(50)` | YES |  |  |  |  |
| 200 | `emp_char13` | `nvarchar(50)` | YES |  |  |  |  |
| 201 | `emp_char14` | `nvarchar(50)` | YES |  |  |  |  |
| 202 | `emp_char15` | `nvarchar(50)` | YES |  |  |  |  |
| 203 | `emp_char16` | `nvarchar(50)` | YES |  |  |  |  |
| 204 | `emp_char17` | `nvarchar(50)` | YES |  |  |  |  |
| 205 | `emp_char18` | `nvarchar(50)` | YES |  |  |  |  |
| 206 | `emp_spouse` | `nvarchar(12)` | YES |  |  |  |  |
| 207 | `emp_post_work` | `nvarchar(50)` | YES |  |  |  |  |
| 208 | `emp_ic_zm200id_new` | `int` | YES |  |  |  |  |
| 209 | `at_status` | `int` | YES |  |  |  |  |
| 210 | `emp_ic_at` | `nvarchar(12)` | YES |  |  |  |  |
| 211 | `emh_black` | `int` | YES |  |  |  |  |
| 212 | `emp_black` | `int` | YES |  |  |  |  |
| 213 | `emp_in_date_group` | `datetime` | YES |  |  |  |  |
| 214 | `emp_long_add` | `int` | YES |  |  |  |  |
| 215 | `emp_cert_no1` | `nvarchar(50)` | YES |  |  |  |  |
| 216 | `emp_cert_no2` | `nvarchar(50)` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `emp_id_key` | CLUSTERED |  |  | `iempid, emp_id` |

#### h_shift_set

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `shift_id` | `nvarchar(50)` | NO | Y |  |  |  |
| 2 | `shift_desc` | `nvarchar(100)` | YES |  |  |  |  |
| 3 | `shift_site` | `nvarchar(50)` | YES |  |  |  |  |
| 4 | `shift_tpksite` | `nvarchar(50)` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_h_shift_set` | CLUSTERED |  |  | `shift_id` |

### 03-Plugin-DingTalk

#### ding_talk_dept (钉钉部门表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 部门id |
| 2 | `parent_id` | `bigint` | NO |  |  |  | 上级部门id |
| 3 | `name` | `nvarchar(64)` | YES |  |  |  | 部门名 |
| 4 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 5 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_ding_talk_dept_Id` | CLUSTERED |  |  | `Id` |

#### ding_talk_wokerflow_config (审批配置表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `WorkflowName` | `nvarchar(255)` | NO | Y |  |  | 审批名 |
| 2 | `ProcessCode` | `nvarchar(255)` | NO |  |  |  | 审批单Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_ding_talk_wokerflow_config_WorkflowName` | CLUSTERED |  |  | `WorkflowName` |

#### ding_talk_wokerflow_log (钉钉审批日志)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `instanceId` | `nvarchar(255)` | NO | Y |  |  | 审批实例ID |
| 2 | `WorkflowId` | `nvarchar(255)` | YES |  |  |  | 审批单号 |
| 3 | `SourceDocument` | `nvarchar(255)` | NO |  |  |  | 来源单据 |
| 4 | `EndTime` | `datetime` | YES |  |  |  | 审批完成时间 |
| 5 | `other_info` | `varchar(4000)` | YES |  |  |  | 其他信息 |
| 6 | `isReturn` | `bit` | YES |  |  |  | 是否回传结果 |
| 7 | `Status` | `nvarchar(255)` | NO |  |  |  | 审批状态 |
| 8 | `taskId` | `bigint` | YES |  |  |  | 任务ID |
| 9 | `Result` | `nvarchar(255)` | YES |  |  |  | 审批结果 |
| 10 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 11 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 12 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_ding_talk_wokerflow_log_instanceId` | CLUSTERED |  |  | `instanceId` |

#### DingTalkRoleUser (钉钉角色表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `DingTalkUserId` | `nvarchar(64)` | YES |  |  |  | 钉钉用户id |
| 3 | `groupId` | `bigint` | NO |  |  |  | 角色组id |
| 4 | `groupName` | `nvarchar(64)` | YES |  |  |  | 角色组名 |
| 5 | `roleId` | `bigint` | NO |  |  |  | 角色id |
| 6 | `roleName` | `nvarchar(64)` | YES |  |  |  | 角色名 |
| 7 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 8 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 9 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 10 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 11 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 12 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 13 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 14 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_DingTalkRoleUser_Id` | CLUSTERED |  |  | `Id` |

#### DingTalkUser (钉钉用户表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `SysUserId` | `bigint` | NO |  |  |  | 系统用户Id |
| 3 | `DingTalkUserId` | `nvarchar(64)` | YES |  |  |  | 钉钉用户id |
| 4 | `UnionId` | `nvarchar(64)` | YES |  |  |  | UnionId |
| 5 | `Name` | `nvarchar(64)` | YES |  |  |  | 用户名 |
| 6 | `Mobile` | `nvarchar(16)` | YES |  |  |  | 手机号码 |
| 7 | `Sex` | `int` | YES |  |  |  | 性别 |
| 8 | `Avatar` | `nvarchar(256)` | YES |  |  |  | 头像 |
| 9 | `JobNumber` | `nvarchar(16)` | YES |  |  |  | 工号 |
| 10 | `DeptId` | `nvarchar(16)` | YES |  |  |  | 主部门Id |
| 11 | `Dept` | `nvarchar(16)` | YES |  |  |  | 主部门 |
| 12 | `Position` | `nvarchar(16)` | YES |  |  |  | 职位 |
| 13 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 14 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 15 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 16 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 17 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 18 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 19 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 20 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_DingTalkUser_Id` | CLUSTERED |  |  | `Id` |

### 04-Plugin-GoView

#### GoViewPro (GoView 项目表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `ProjectName` | `nvarchar(64)` | NO |  |  |  | 项目名称 |
| 3 | `StateEnum` | `int` | NO |  |  |  | 项目状态 |
| 4 | `IndexImage` | `nvarchar(1024)` | YES |  |  |  | 预览图片Url |
| 5 | `Remarks` | `nvarchar(512)` | YES |  |  |  | 项目备注 |
| 6 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 7 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 8 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 9 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 10 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 11 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 12 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 13 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 14 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_GoViewPro_Id` | CLUSTERED |  |  | `Id` |

#### GoViewProData (GoView 项目数据表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Content` | `nvarchar(MAX)` | YES |  |  |  | 项目内容 |
| 3 | `IndexImageData` | `nvarchar(MAX)` | YES |  |  |  | 预览图片 |
| 4 | `BackGroundImageData` | `nvarchar(MAX)` | YES |  |  |  | 背景图片 |
| 5 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 6 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 7 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 8 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 9 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 10 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 11 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 12 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 13 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_GoViewProData_Id` | CLUSTERED |  |  | `Id` |

### 05-Plugin-AI

#### LLMChatHistory (AI聊天历史表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `SummaryId` | `bigint` | NO |  |  |  | 摘要ID |
| 3 | `UserId` | `bigint` | NO |  |  |  | 用户ID |
| 4 | `Role` | `nvarchar(20)` | NO |  |  |  | 角色 |
| 5 | `Content` | `nvarchar(MAX)` | YES |  |  |  | 内容 |
| 6 | `UtcCreateTime` | `bigint` | NO |  |  |  | 创建时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_LLMChatHistory_s` | NONCLUSTERED |  |  | `SummaryId` |
| `i_LLMChatHistory_u` | NONCLUSTERED |  |  | `UserId` |
| `PK_LLMChatHistory_Id` | CLUSTERED |  |  | `Id` |

#### LLMChatSummaryHistory (AI聊天摘要历史表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户ID |
| 3 | `UniqueToken` | `nvarchar(255)` | NO |  |  |  | 唯一标识 |
| 4 | `Summary` | `nvarchar(1024)` | YES |  |  |  | 摘要 |
| 5 | `UtcCreateTime` | `bigint` | NO |  |  |  | 创建时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_LLMChatSummaryHistory_un` | NONCLUSTERED |  |  | `UniqueToken` |
| `i_LLMChatSummaryHistory_us` | NONCLUSTERED |  |  | `UserId` |
| `PK_LLMChatSummaryHistory_Id` | CLUSTERED |  |  | `Id` |

### 01-Admin.NET-Core-Sys

#### SysAlipayAuthInfo (支付宝授权记录表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `AppId` | `nvarchar(64)` | YES |  |  |  | 商户AppId |
| 3 | `OpenId` | `nvarchar(64)` | YES |  |  |  | 开放ID |
| 4 | `UserId` | `nvarchar(64)` | YES |  |  |  | 用户ID |
| 5 | `Gender` | `int` | NO |  |  |  | 性别 |
| 6 | `Age` | `int` | NO |  |  |  | 年龄 |
| 7 | `Mobile` | `nvarchar(32)` | NO |  |  |  | 手机号 |
| 8 | `DisplayName` | `nvarchar(128)` | NO |  |  |  | 显示名称 |
| 9 | `NickName` | `nvarchar(64)` | NO |  |  |  | 昵称 |
| 10 | `UserName` | `nvarchar(64)` | NO |  |  |  | 用户名 |
| 11 | `Avatar` | `nvarchar(512)` | YES |  |  |  | 头像 |
| 12 | `Email` | `nvarchar(128)` | YES |  |  |  | 邮箱 |
| 13 | `UserNation` | `nvarchar(32)` | YES |  |  |  | 用户民族 |
| 14 | `TaobaoId` | `nvarchar(64)` | YES |  |  |  | 淘宝ID |
| 15 | `Phone` | `nvarchar(32)` | YES |  |  |  | 电话 |
| 16 | `PersonBirthday` | `nvarchar(32)` | YES |  |  |  | 生日 |
| 17 | `Profession` | `nvarchar(64)` | YES |  |  |  | 职业 |
| 18 | `Province` | `nvarchar(64)` | YES |  |  |  | 省份 |
| 19 | `UserStatus` | `nvarchar(32)` | YES |  |  |  | 用户状态 |
| 20 | `Degree` | `nvarchar(32)` | YES |  |  |  | 学历 |
| 21 | `UserType` | `nvarchar(32)` | YES |  |  |  | 用户类型 |
| 22 | `Zip` | `nvarchar(16)` | YES |  |  |  | 邮编 |
| 23 | `Address` | `nvarchar(256)` | YES |  |  |  | 地址 |
| 24 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 25 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 26 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 27 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 28 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 29 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 30 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 31 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysAlipayAuthInfo_o` | NONCLUSTERED |  |  | `OpenId` |
| `i_SysAlipayAuthInfo_u` | NONCLUSTERED |  |  | `UserId` |
| `PK_SysAlipayAuthInfo_Id` | CLUSTERED |  |  | `Id` |

#### SysAlipayTransaction (支付宝交易记录表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `TradeNo` | `nvarchar(64)` | YES |  |  |  | 交易号 |
| 4 | `OutTradeNo` | `nvarchar(64)` | NO |  |  |  | 商户订单号 |
| 5 | `TotalAmount` | `decimal(20,0)` | NO |  |  |  | 交易金额 |
| 6 | `TradeStatus` | `nvarchar(32)` | NO |  |  |  | 交易状态 |
| 7 | `FinishTime` | `datetime` | YES |  |  |  | 交易完成时间 |
| 8 | `Subject` | `nvarchar(256)` | NO |  |  |  | 交易标题 |
| 9 | `Body` | `nvarchar(512)` | YES |  |  |  | 交易描述 |
| 10 | `BuyerLogonId` | `nvarchar(128)` | YES |  |  |  | 买家支付宝账号 |
| 11 | `BuyerUserId` | `nvarchar(32)` | YES |  |  |  | 买家支付宝用户ID |
| 12 | `SellerUserId` | `nvarchar(32)` | YES |  |  |  | 卖家支付宝用户ID |
| 13 | `AppId` | `nvarchar(64)` | YES |  |  |  | 商户AppId |
| 14 | `ExtendInfo` | `nvarchar(1024)` | YES |  |  |  | 交易扩展信息 |
| 15 | `ErrorInfo` | `nvarchar(1024)` | YES |  |  |  | 交易扩展信息 |
| 16 | `Remark` | `nvarchar(512)` | YES |  |  |  | 交易备注 |
| 17 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 18 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 19 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 20 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 21 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 22 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 23 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 24 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysAlipayTransaction_o` | NONCLUSTERED |  |  | `OutTradeNo` |
| `i_SysAlipayTransaction_t` | NONCLUSTERED |  |  | `TradeNo` |
| `i_SysAlipayTransaction_u` | NONCLUSTERED |  |  | `UserId` |
| `PK_SysAlipayTransaction_Id` | CLUSTERED |  |  | `Id` |

#### SysCodeGen (代码生成表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `AuthorName` | `nvarchar(32)` | YES |  |  |  | 作者姓名 |
| 3 | `Email` | `nvarchar(32)` | YES |  |  |  | 作者邮箱 |
| 4 | `GenerateMethod` | `int` | NO |  |  |  | 生成方式 |
| 5 | `Scene` | `int` | NO |  |  |  | 生成场景 |
| 6 | `TreeConfig` | `nvarchar(MAX)` | YES |  |  |  | 树组件配置 |
| 7 | `NameSpace` | `nvarchar(128)` | NO |  |  |  | 命名空间 |
| 8 | `BusName` | `nvarchar(128)` | NO |  |  |  | 业务名 |
| 9 | `ModuleName` | `nvarchar(32)` | NO |  |  |  | 模块名称 |
| 10 | `IsHorizontal` | `bit` | NO |  |  |  | 是否水平布局 |
| 11 | `GenerateMenu` | `bit` | NO |  |  |  | 是否生成菜单 |
| 12 | `MenuIcon` | `nvarchar(32)` | NO |  |  |  | 菜单图标 |
| 13 | `MenuPid` | `bigint` | NO |  |  |  | 菜单编码 |
| 14 | `PagePath` | `nvarchar(32)` | NO |  |  |  | 页面目录 |
| 15 | `PrintType` | `int` | NO |  |  |  | 支持打印类型 |
| 16 | `PrintName` | `nvarchar(32)` | YES |  |  |  | 打印模版名称 |
| 17 | `IsApiService` | `bit` | NO |  |  |  | 是否使用 Api Service |
| 18 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 19 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 20 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 21 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 22 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 23 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 24 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 25 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `index_SysCodeGen_b` | NONCLUSTERED |  |  | `BusName` |
| `PK_SysCodeGen_Id` | CLUSTERED |  |  | `Id` |

#### SysCodeGenColumn (代码生成表字段配置表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `CodeGenTableId` | `bigint` | NO |  |  |  | 表Id |
| 3 | `ColumnName` | `nvarchar(128)` | NO |  |  |  | 字段名称 |
| 4 | `PropertyName` | `nvarchar(128)` | NO |  |  |  | 属性名称 |
| 5 | `NetType` | `nvarchar(64)` | NO |  |  |  | NET数据类型 |
| 6 | `DataType` | `nvarchar(64)` | NO |  |  |  | 数据库中类型 |
| 7 | `ColumnLength` | `int` | YES |  |  |  | 字段数据长度 |
| 8 | `ColumnComment` | `nvarchar(128)` | NO |  |  |  | 字段描述 |
| 9 | `EffectType` | `int` | NO |  |  |  | 控件类型 |
| 10 | `Config` | `nvarchar(MAX)` | YES |  |  |  | 控件配置 |
| 11 | `IsPrimarykey` | `bit` | NO |  |  |  | 主键 |
| 12 | `IsCommon` | `bit` | NO |  |  |  | 是否通用字段 |
| 13 | `IsRequired` | `bit` | NO |  |  |  | 是否必填 |
| 14 | `IsAddUpdate` | `bit` | NO |  |  |  | 增改 |
| 15 | `IsImport` | `bit` | NO |  |  |  | 导入导出 |
| 16 | `IsSortable` | `bit` | NO |  |  |  | 是否可排序 |
| 17 | `IsStatistical` | `bit` | NO |  |  |  | 是否是统计字段 |
| 18 | `IsQuery` | `bit` | NO |  |  |  | 是否是查询条件 |
| 19 | `QueryType` | `nvarchar(16)` | YES |  |  |  | 查询方式 |
| 20 | `IsTable` | `bit` | NO |  |  |  | 列表显示 |
| 21 | `IsCopy` | `bit` | NO |  |  |  | 内容复制 |
| 22 | `DefaultValue` | `nvarchar(256)` | YES |  |  |  | 默认值 |
| 23 | `FromValid` | `int` | YES |  |  |  | 字段验证规则 |
| 24 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 25 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 26 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 27 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 28 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 29 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 30 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 31 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 32 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `index_SysCodeGenColumn_cn` | NONCLUSTERED |  |  | `ColumnName` |
| `index_SysCodeGenColumn_cti` | NONCLUSTERED |  |  | `CodeGenTableId` |
| `PK_SysCodeGenColumn_Id` | CLUSTERED |  |  | `Id` |

#### SysCodeGenTable (代码生成关联表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `CodeGenId` | `bigint` | NO |  |  |  | 代码生成Id |
| 3 | `ConfigId` | `nvarchar(64)` | NO |  |  |  | 数据库配置id |
| 4 | `TableName` | `nvarchar(128)` | NO |  |  |  | 数据库表名 |
| 5 | `EntityName` | `nvarchar(128)` | NO |  |  |  | 表实体名称 |
| 6 | `ModuleName` | `nvarchar(64)` | NO |  |  |  | 模块名称 |
| 7 | `BusName` | `nvarchar(128)` | NO |  |  |  | 业务名 |
| 8 | `LastLinkPropertyName` | `nvarchar(32)` | YES |  |  |  | 上级联表字段 |
| 9 | `NextLinkPropertyName` | `nvarchar(32)` | YES |  |  |  | 下级联表字段 |
| 10 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 11 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 12 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 13 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 14 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 15 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 16 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 17 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `index_SysCodeGenTable_cgi` | NONCLUSTERED |  |  | `CodeGenId` |
| `index_SysCodeGenTable_tn` | NONCLUSTERED |  |  | `TableName` |
| `PK_SysCodeGenTable_Id` | CLUSTERED |  |  | `Id` |

#### SysColumnCustom (Vxe表格列配置)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `GridId` | `nvarchar(128)` | NO |  |  |  | 表格Id |
| 4 | `FixedData` | `nvarchar(MAX)` | YES |  |  |  | 冻结列状态数据 |
| 5 | `ResizableData` | `nvarchar(MAX)` | YES |  |  |  | 列宽状态数据 |
| 6 | `SortData` | `nvarchar(MAX)` | YES |  |  |  | 列顺序数据 |
| 7 | `VisibleData` | `nvarchar(MAX)` | YES |  |  |  | 显示/隐藏列状态数据 |
| 8 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysColumnCustom_g` | NONCLUSTERED |  |  | `GridId` |
| `i_SysColumnCustom_u` | NONCLUSTERED |  |  | `UserId` |
| `PK_SysColumnCustom_Id` | CLUSTERED |  |  | `Id` |

#### SysConfig (系统配置参数表)

- Schema: `dbo`
- Rows: 27

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `Code` | `nvarchar(64)` | YES |  |  |  | 编码 |
| 4 | `Value` | `nvarchar(512)` | YES |  |  |  | 参数值 |
| 5 | `SysFlag` | `int` | NO |  |  | ('1') | 是否是内置参数 |
| 6 | `GroupCode` | `nvarchar(64)` | YES |  |  |  | 分组编码 |
| 7 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 8 | `Remark` | `nvarchar(256)` | YES |  |  |  | 备注 |
| 9 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 10 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 12 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 13 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 14 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 15 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 16 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysConfig_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysConfig_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysConfig_Id` | CLUSTERED |  |  | `Id` |

#### SysDictData (系统字典值表)

- Schema: `dbo`
- Rows: 628

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `DictTypeId` | `bigint` | NO |  |  |  | 字典类型Id |
| 3 | `Label` | `nvarchar(256)` | NO |  |  |  | 显示文本 |
| 4 | `Value` | `nvarchar(256)` | NO |  |  |  | 值 |
| 5 | `Code` | `nvarchar(256)` | YES |  |  |  | 编码 |
| 6 | `TagType` | `nvarchar(16)` | YES |  |  |  | 显示样式-标签颜色 |
| 7 | `StyleSetting` | `nvarchar(512)` | YES |  |  |  | 显示样式-Style |
| 8 | `ClassSetting` | `nvarchar(512)` | YES |  |  |  | 显示样式-Class |
| 9 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 10 | `Remark` | `nvarchar(2048)` | YES |  |  |  | 备注 |
| 11 | `ExtData` | `nvarchar(MAX)` | YES |  |  |  | 拓展数据(保存业务功能的配置项) |
| 12 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 13 | `IsEnum` | `int` | NO |  |  | ('2') | 是否是枚举转换字典 |
| 14 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 15 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 16 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 17 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 18 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 19 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 20 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 21 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysDictData_c` | NONCLUSTERED |  |  | `Code` |
| `PK_SysDictData_Id` | CLUSTERED |  |  | `Id` |

#### SysDictType (系统字典类型表)

- Schema: `dbo`
- Rows: 62

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `Code` | `nvarchar(64)` | NO |  |  |  | 编码 |
| 4 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 5 | `Remark` | `nvarchar(256)` | YES |  |  |  | 备注 |
| 6 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 7 | `SysFlag` | `int` | NO |  |  | ('1') | 是否是内置字典 |
| 8 | `IsEnum` | `int` | NO |  |  | ('2') | 是否是枚举转换字典 |
| 9 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 10 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 12 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 13 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 14 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 15 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 16 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysDictType_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysDictType_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysDictType_Id` | CLUSTERED |  |  | `Id` |

#### SysFile (系统文件表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Provider` | `nvarchar(128)` | YES |  |  |  | 提供者 |
| 3 | `BucketName` | `nvarchar(128)` | YES |  |  |  | 仓储名称 |
| 4 | `FileName` | `nvarchar(128)` | YES |  |  |  | 文件名称 |
| 5 | `Suffix` | `nvarchar(16)` | YES |  |  |  | 文件后缀 |
| 6 | `ContentType` | `nvarchar(128)` | YES |  |  |  | MIME类型 |
| 7 | `FilePath` | `nvarchar(512)` | YES |  |  |  | 存储路径 |
| 8 | `SizeKb` | `bigint` | NO |  |  |  | 文件大小KB |
| 9 | `SizeInfo` | `nvarchar(64)` | YES |  |  |  | 文件大小信息 |
| 10 | `Url` | `nvarchar(512)` | YES |  |  |  | 外链地址 |
| 11 | `FileMd5` | `nvarchar(128)` | YES |  |  |  | 文件MD5 |
| 12 | `FileType` | `nvarchar(128)` | YES |  |  |  | 文件类别 |
| 13 | `FileAlias` | `nvarchar(128)` | YES |  |  |  | 文件别名 |
| 14 | `DataId` | `bigint` | YES |  |  |  | 业务数据Id |
| 15 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 16 | `CreateOrgId` | `bigint` | YES |  |  |  | 创建者机构Id |
| 17 | `CreateOrgName` | `nvarchar(64)` | YES |  |  |  | 创建者机构名称 |
| 18 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 19 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 20 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 21 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 22 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 23 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 24 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 25 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysFile_f` | NONCLUSTERED |  |  | `FileName` |
| `PK_SysFile_Id` | CLUSTERED |  |  | `Id` |

#### SysFileContent (系统文件内容表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `SysFileId` | `bigint` | NO |  |  |  | SysFile里的Id |
| 3 | `Content` | `varbinary(MAX)` | NO |  |  |  | 文件内容 |
| 4 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 5 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 6 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 7 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 8 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 9 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 10 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 11 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysFileContent_Id` | CLUSTERED |  |  | `Id` |

#### SysJobCluster (系统作业集群表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `ClusterId` | `nvarchar(64)` | NO |  |  |  | 作业集群Id |
| 3 | `Description` | `nvarchar(128)` | YES |  |  |  | 描述信息 |
| 4 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 5 | `UpdatedTime` | `datetime` | YES |  |  |  | 更新时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysJobCluster_Id` | CLUSTERED |  |  | `Id` |

#### SysJobDetail (系统作业信息表)

- Schema: `dbo`
- Rows: 5

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `JobId` | `nvarchar(64)` | NO |  |  |  | 作业Id |
| 3 | `GroupName` | `nvarchar(128)` | YES |  |  |  | 组名称 |
| 4 | `JobType` | `nvarchar(128)` | YES |  |  |  | 作业类型 |
| 5 | `AssemblyName` | `nvarchar(128)` | YES |  |  |  | 程序集 |
| 6 | `Description` | `nvarchar(128)` | YES |  |  |  | 描述信息 |
| 7 | `Concurrent` | `bit` | NO |  |  |  | 是否并行执行 |
| 8 | `annotation` | `bit` | NO |  |  |  | 是否扫描特性触发器 |
| 9 | `Properties` | `nvarchar(MAX)` | YES |  |  |  | 额外数据 |
| 10 | `UpdatedTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateType` | `int` | NO |  |  | ('0') | 作业创建类型 |
| 12 | `ScriptCode` | `nvarchar(MAX)` | YES |  |  |  | 脚本代码 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysJobDetail_j` | NONCLUSTERED |  |  | `JobId` |
| `PK_SysJobDetail_Id` | CLUSTERED |  |  | `Id` |

#### SysJobTrigger (系统作业触发器表)

- Schema: `dbo`
- Rows: 4

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `TriggerId` | `nvarchar(64)` | NO |  |  |  | 触发器Id |
| 3 | `JobId` | `nvarchar(64)` | NO |  |  |  | 作业Id |
| 4 | `TriggerType` | `nvarchar(128)` | YES |  |  |  | 触发器类型 |
| 5 | `AssemblyName` | `nvarchar(128)` | YES |  |  |  | 程序集 |
| 6 | `Args` | `nvarchar(128)` | YES |  |  |  | 参数 |
| 7 | `Description` | `nvarchar(128)` | YES |  |  |  | 描述信息 |
| 8 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 9 | `StartTime` | `datetime` | YES |  |  |  | 起始时间 |
| 10 | `EndTime` | `datetime` | YES |  |  |  | 结束时间 |
| 11 | `LastRunTime` | `datetime` | YES |  |  |  | 最近运行时间 |
| 12 | `NextRunTime` | `datetime` | YES |  |  |  | 下一次运行时间 |
| 13 | `NumberOfRuns` | `bigint` | NO |  |  |  | 触发次数 |
| 14 | `MaxNumberOfRuns` | `bigint` | NO |  |  |  | 最大触发次数 |
| 15 | `NumberOfErrors` | `bigint` | NO |  |  |  | 出错次数 |
| 16 | `MaxNumberOfErrors` | `bigint` | NO |  |  |  | 最大出错次数 |
| 17 | `NumRetries` | `int` | NO |  |  |  | 重试次数 |
| 18 | `RetryTimeout` | `int` | NO |  |  | ('3000') | 重试间隔时间(ms) |
| 19 | `StartNow` | `bit` | NO |  |  |  | 是否立即启动 |
| 20 | `RunOnStart` | `bit` | NO |  |  |  | 是否启动时执行一次 |
| 21 | `ResetOnlyOnce` | `bit` | NO |  |  |  | 是否重置触发次数 |
| 22 | `UpdatedTime` | `datetime` | YES |  |  |  | 更新时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysJobTrigger_Id` | CLUSTERED |  |  | `Id` |

#### SysJobTriggerRecord (系统作业触发器运行记录表)

- Schema: `dbo`
- Rows: 4

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `JobId` | `nvarchar(64)` | NO |  |  |  | 作业Id |
| 3 | `TriggerId` | `nvarchar(64)` | NO |  |  |  | 触发器Id |
| 4 | `NumberOfRuns` | `bigint` | NO |  |  |  | 当前运行次数 |
| 5 | `LastRunTime` | `datetime` | YES |  |  |  | 最近运行时间 |
| 6 | `NextRunTime` | `datetime` | YES |  |  |  | 下一次运行时间 |
| 7 | `Status` | `int` | NO |  |  | ('1') | 触发器状态 |
| 8 | `Result` | `nvarchar(MAX)` | YES |  |  |  | 执行结果 |
| 9 | `ElapsedTime` | `bigint` | NO |  |  |  | 执行耗时 |
| 10 | `CreatedTime` | `datetime` | YES |  |  |  | 创建时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysJobTriggerRecord_Id` | CLUSTERED |  |  | `Id` |

#### SysLdap (系统域登录信息配置表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Host` | `nvarchar(128)` | NO |  |  |  | 主机 |
| 3 | `Port` | `int` | NO |  |  |  | 端口 |
| 4 | `BaseDn` | `nvarchar(128)` | NO |  |  |  | 用户搜索基准 |
| 5 | `BindDn` | `nvarchar(128)` | NO |  |  |  | 绑定DN |
| 6 | `BindPass` | `nvarchar(512)` | NO |  |  |  | 绑定密码 |
| 7 | `AuthFilter` | `nvarchar(128)` | NO |  |  |  | 用户过滤规则 |
| 8 | `Version` | `int` | NO |  |  |  | Ldap版本 |
| 9 | `BindAttrAccount` | `nvarchar(32)` | NO |  |  |  | 绑定域账号字段属性值 |
| 10 | `BindAttrEmployeeId` | `nvarchar(32)` | NO |  |  |  | 绑定用户EmployeeId属性值 |
| 11 | `BindAttrCode` | `nvarchar(64)` | NO |  |  |  | 绑定对象Code属性值 |
| 12 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 13 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 14 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 15 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 16 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 17 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 18 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 19 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 20 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 21 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLdap_Id` | CLUSTERED |  |  | `Id` |

#### SysLogDiff (系统差异日志表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `BeforeData` | `nvarchar(MAX)` | YES |  |  |  | 操作前记录 |
| 3 | `AfterData` | `nvarchar(MAX)` | YES |  |  |  | 操作后记录 |
| 4 | `Sql` | `nvarchar(MAX)` | YES |  |  |  | Sql |
| 5 | `Parameters` | `nvarchar(MAX)` | YES |  |  |  | 参数 |
| 6 | `BusinessData` | `nvarchar(MAX)` | YES |  |  |  | 业务对象 |
| 7 | `DiffType` | `nvarchar(MAX)` | YES |  |  |  | 操作类型 |
| 8 | `Elapsed` | `bigint` | YES |  |  |  | 耗时 |
| 9 | `CreateTime` | `datetime` | NO |  |  |  | 创建时间 |
| 10 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 11 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 12 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLogDiff_Id` | CLUSTERED |  |  | `Id` |

#### SysLogEvent (系统事件日志表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `RunId` | `nvarchar(64)` | NO |  |  |  | 运行ID |
| 3 | `EventId` | `nvarchar(64)` | NO |  |  |  | 事件ID |
| 4 | `Payload` | `nvarchar(MAX)` | YES |  |  |  | 事件参数 |
| 5 | `CreateTimeOfEvent` | `datetime` | NO |  |  |  | 事件创建时间 |
| 6 | `ExecutingTime` | `datetime` | NO |  |  |  | 执行前时间 |
| 7 | `Exception` | `nvarchar(MAX)` | YES |  |  |  | 异常信息 |
| 8 | `Elapsed` | `bigint` | YES |  |  |  | 耗时（毫秒） |
| 9 | `ExecutedTime` | `datetime` | YES |  |  |  | 执行后时间 |
| 10 | `Attribute` | `nvarchar(512)` | YES |  |  |  | 订阅特性 |
| 11 | `HandlerMethod` | `nvarchar(128)` | YES |  |  |  | 触发方法 |
| 12 | `Properties` | `nvarchar(1024)` | YES |  |  |  | 共享上下文数据 |
| 13 | `ConsumeOnce` | `bit` | NO |  |  |  | 消息是否只消费一次 |
| 14 | `RetryRecord` | `nvarchar(1024)` | YES |  |  |  | 重试记录 |
| 15 | `Status` | `int` | NO |  |  |  | 事件状态 |
| 16 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 17 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 18 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 19 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 20 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 21 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 22 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 23 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysLogEvent_e` | NONCLUSTERED |  |  | `EventId` |
| `i_SysLogEvent_r` | NONCLUSTERED |  |  | `RunId` |
| `PK_SysLogEvent_Id` | CLUSTERED |  |  | `Id` |

#### SysLogEx (系统异常日志表)

- Schema: `dbo`
- Rows: 21

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `HttpMethod` | `nvarchar(32)` | YES |  |  |  | 请求方式 |
| 3 | `RequestUrl` | `nvarchar(MAX)` | YES |  |  |  | 请求地址 |
| 4 | `RequestParam` | `nvarchar(MAX)` | YES |  |  |  | 请求参数 |
| 5 | `ReturnResult` | `nvarchar(MAX)` | YES |  |  |  | 返回结果 |
| 6 | `EventId` | `int` | YES |  |  |  | 事件Id |
| 7 | `ThreadId` | `int` | YES |  |  |  | 线程Id |
| 8 | `TraceId` | `nvarchar(128)` | YES |  |  |  | 请求跟踪Id |
| 9 | `Exception` | `nvarchar(MAX)` | YES |  |  |  | 异常信息 |
| 10 | `Message` | `nvarchar(MAX)` | YES |  |  |  | 日志消息Json |
| 11 | `ControllerName` | `nvarchar(256)` | YES |  |  |  | 模块名称 |
| 12 | `ActionName` | `nvarchar(256)` | YES |  |  |  | 方法名称 |
| 13 | `DisplayTitle` | `nvarchar(256)` | YES |  |  |  | 显示名称 |
| 14 | `Status` | `nvarchar(32)` | YES |  |  |  | 执行状态 |
| 15 | `RemoteIp` | `nvarchar(256)` | YES |  |  |  | IP地址 |
| 16 | `Location` | `nvarchar(128)` | YES |  |  |  | 登录地点 |
| 17 | `Longitude` | `float` | YES |  |  |  | 经度 |
| 18 | `Latitude` | `float` | YES |  |  |  | 纬度 |
| 19 | `Browser` | `nvarchar(1024)` | YES |  |  |  | 浏览器 |
| 20 | `Os` | `nvarchar(256)` | YES |  |  |  | 操作系统 |
| 21 | `Elapsed` | `bigint` | YES |  |  |  | 操作用时 |
| 22 | `LogDateTime` | `datetime` | YES |  |  |  | 日志时间 |
| 23 | `LogLevel` | `int` | NO |  |  |  | 日志级别 |
| 24 | `Account` | `nvarchar(32)` | YES |  |  |  | 账号 |
| 25 | `RealName` | `nvarchar(32)` | YES |  |  |  | 真实姓名 |
| 26 | `CreateTime` | `datetime` | NO |  |  |  | 创建时间 |
| 27 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 28 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 29 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLogEx_Id` | CLUSTERED |  |  | `Id` |

#### SysLogHttp (Http请求日志表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `HttpClientName` | `nvarchar(32)` | YES |  |  |  | 客户端名称 |
| 3 | `HttpApiDesc` | `nvarchar(64)` | YES |  |  |  | 接口描述 |
| 4 | `HttpMethod` | `nvarchar(8)` | YES |  |  |  | 请求方式 |
| 5 | `IsSuccessStatusCode` | `int` | YES |  |  |  | 是否成功 |
| 6 | `ActionName` | `nvarchar(64)` | YES |  |  |  | 模块名称 |
| 7 | `RequestUrl` | `nvarchar(MAX)` | YES |  |  |  | 请求地址 |
| 8 | `RequestHeaders` | `nvarchar(MAX)` | YES |  |  |  | 请求头 |
| 9 | `RequestBody` | `nvarchar(MAX)` | YES |  |  |  | 请求体 |
| 10 | `RequestBodyPlaintext` | `nvarchar(MAX)` | YES |  |  |  | 请求体明文 |
| 11 | `StatusCode` | `int` | YES |  |  |  | 响应状态码 |
| 12 | `ResponseHeaders` | `nvarchar(MAX)` | YES |  |  |  | 响应头 |
| 13 | `ResponseBody` | `nvarchar(MAX)` | YES |  |  |  | 响应体 |
| 14 | `ResponseBodyPlaintext` | `nvarchar(MAX)` | YES |  |  |  | 响应体明文 |
| 15 | `Exception` | `nvarchar(MAX)` | YES |  |  |  | 异常信息 |
| 16 | `StartTime` | `datetime` | YES |  |  |  | 开始时间 |
| 17 | `EndTime` | `datetime` | YES |  |  |  | 结束时间 |
| 18 | `Elapsed` | `bigint` | YES |  |  |  | 耗时（毫秒） |
| 19 | `CreateTime` | `datetime` | NO |  |  |  | 创建时间 |
| 20 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 21 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 22 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysLogHttp_n` | NONCLUSTERED |  |  | `HttpClientName` |
| `PK_SysLogHttp_Id` | CLUSTERED |  |  | `Id` |

#### SysLogMsg (系统消息日志表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `MessageType` | `nvarchar(64)` | NO |  |  |  | 消息类型 |
| 3 | `Title` | `nvarchar(64)` | NO |  |  |  | 消息标题 |
| 4 | `Message` | `nvarchar(MAX)` | NO |  |  |  | 消息内容 |
| 5 | `ReceiveUserId` | `bigint` | NO |  |  |  | 接收者Id |
| 6 | `ReceiveUserName` | `nvarchar(32)` | NO |  |  |  | 接收者名称 |
| 7 | `ReceiveUserIds` | `nvarchar(MAX)` | YES |  |  |  | 接收者Id集合 |
| 8 | `ReceiveIp` | `nvarchar(256)` | YES |  |  |  | 接收者IP |
| 9 | `ReceiveBrowser` | `nvarchar(128)` | YES |  |  |  | 接收者浏览器 |
| 10 | `ReceiveOs` | `nvarchar(128)` | YES |  |  |  | 接收者操作系统 |
| 11 | `ReceiveDevice` | `nvarchar(256)` | YES |  |  |  | 接收者设备 |
| 12 | `SendUserId` | `bigint` | NO |  |  |  | 发送者Id |
| 13 | `SendUserName` | `nvarchar(32)` | NO |  |  |  | 发送者名称 |
| 14 | `SendTime` | `datetime` | NO |  |  |  | 发送时间 |
| 15 | `SendIp` | `nvarchar(256)` | YES |  |  |  | 发送者IP |
| 16 | `SendBrowser` | `nvarchar(128)` | YES |  |  |  | 发送者浏览器 |
| 17 | `SendOs` | `nvarchar(128)` | YES |  |  |  | 发送者操作系统 |
| 18 | `SendDevice` | `nvarchar(256)` | YES |  |  |  | 发送者设备 |
| 19 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 20 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 21 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 22 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLogMsg_Id` | CLUSTERED |  |  | `Id` |

#### SysLogOp (系统操作日志表)

- Schema: `dbo`
- Rows: 591

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `HttpMethod` | `nvarchar(32)` | YES |  |  |  | 请求方式 |
| 3 | `RequestUrl` | `nvarchar(MAX)` | YES |  |  |  | 请求地址 |
| 4 | `RequestParam` | `nvarchar(MAX)` | YES |  |  |  | 请求参数 |
| 5 | `ReturnResult` | `nvarchar(MAX)` | YES |  |  |  | 返回结果 |
| 6 | `EventId` | `int` | YES |  |  |  | 事件Id |
| 7 | `ThreadId` | `int` | YES |  |  |  | 线程Id |
| 8 | `TraceId` | `nvarchar(128)` | YES |  |  |  | 请求跟踪Id |
| 9 | `Exception` | `nvarchar(MAX)` | YES |  |  |  | 异常信息 |
| 10 | `Message` | `nvarchar(MAX)` | YES |  |  |  | 日志消息Json |
| 11 | `ControllerName` | `nvarchar(256)` | YES |  |  |  | 模块名称 |
| 12 | `ActionName` | `nvarchar(256)` | YES |  |  |  | 方法名称 |
| 13 | `DisplayTitle` | `nvarchar(256)` | YES |  |  |  | 显示名称 |
| 14 | `Status` | `nvarchar(32)` | YES |  |  |  | 执行状态 |
| 15 | `RemoteIp` | `nvarchar(256)` | YES |  |  |  | IP地址 |
| 16 | `Location` | `nvarchar(128)` | YES |  |  |  | 登录地点 |
| 17 | `Longitude` | `float` | YES |  |  |  | 经度 |
| 18 | `Latitude` | `float` | YES |  |  |  | 纬度 |
| 19 | `Browser` | `nvarchar(1024)` | YES |  |  |  | 浏览器 |
| 20 | `Os` | `nvarchar(256)` | YES |  |  |  | 操作系统 |
| 21 | `Elapsed` | `bigint` | YES |  |  |  | 操作用时 |
| 22 | `LogDateTime` | `datetime` | YES |  |  |  | 日志时间 |
| 23 | `LogLevel` | `int` | NO |  |  |  | 日志级别 |
| 24 | `Account` | `nvarchar(32)` | YES |  |  |  | 账号 |
| 25 | `RealName` | `nvarchar(32)` | YES |  |  |  | 真实姓名 |
| 26 | `CreateTime` | `datetime` | NO |  |  |  | 创建时间 |
| 27 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 28 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 29 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLogOp_Id` | CLUSTERED |  |  | `Id` |

#### SysLogVis (系统访问日志表)

- Schema: `dbo`
- Rows: 23

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `ControllerName` | `nvarchar(256)` | YES |  |  |  | 模块名称 |
| 3 | `ActionName` | `nvarchar(256)` | YES |  |  |  | 方法名称 |
| 4 | `DisplayTitle` | `nvarchar(256)` | YES |  |  |  | 显示名称 |
| 5 | `Status` | `nvarchar(32)` | YES |  |  |  | 执行状态 |
| 6 | `RemoteIp` | `nvarchar(256)` | YES |  |  |  | IP地址 |
| 7 | `Location` | `nvarchar(128)` | YES |  |  |  | 登录地点 |
| 8 | `Longitude` | `float` | YES |  |  |  | 经度 |
| 9 | `Latitude` | `float` | YES |  |  |  | 纬度 |
| 10 | `Browser` | `nvarchar(1024)` | YES |  |  |  | 浏览器 |
| 11 | `Os` | `nvarchar(256)` | YES |  |  |  | 操作系统 |
| 12 | `Elapsed` | `bigint` | YES |  |  |  | 操作用时 |
| 13 | `LogDateTime` | `datetime` | YES |  |  |  | 日志时间 |
| 14 | `LogLevel` | `int` | NO |  |  |  | 日志级别 |
| 15 | `Account` | `nvarchar(32)` | YES |  |  |  | 账号 |
| 16 | `RealName` | `nvarchar(32)` | YES |  |  |  | 真实姓名 |
| 17 | `CreateTime` | `datetime` | NO |  |  |  | 创建时间 |
| 18 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 19 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 20 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysLogVis_Id` | CLUSTERED |  |  | `Id` |

#### SysMenu (系统菜单表)

- Schema: `dbo`
- Rows: 212

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Pid` | `bigint` | NO |  |  |  | 父Id |
| 3 | `Type` | `int` | NO |  |  | ('2') | 菜单类型 |
| 4 | `Name` | `nvarchar(64)` | YES |  |  |  | 路由名称 |
| 5 | `Path` | `nvarchar(128)` | YES |  |  |  | 路由地址 |
| 6 | `Component` | `nvarchar(128)` | YES |  |  |  | 组件路径 |
| 7 | `Redirect` | `nvarchar(128)` | YES |  |  |  | 重定向 |
| 8 | `Permission` | `nvarchar(128)` | YES |  |  |  | 权限标识 |
| 9 | `Title` | `nvarchar(64)` | NO |  |  |  | 菜单名称 |
| 10 | `Icon` | `nvarchar(128)` | YES |  |  |  | 图标 |
| 11 | `IsIframe` | `bit` | NO |  |  |  | 是否内嵌 |
| 12 | `OutLink` | `nvarchar(256)` | YES |  |  |  | 外链链接 |
| 13 | `IsHide` | `bit` | NO |  |  |  | 是否隐藏 |
| 14 | `IsKeepAlive` | `bit` | NO |  |  |  | 是否缓存 |
| 15 | `IsAffix` | `bit` | NO |  |  |  | 是否固定 |
| 16 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 17 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 18 | `Remark` | `nvarchar(256)` | YES |  |  |  | 备注 |
| 19 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 20 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 21 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 22 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 23 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 24 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 25 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 26 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysMenu_t1` | NONCLUSTERED |  |  | `Title` |
| `i_SysMenu_t2` | NONCLUSTERED |  |  | `Type` |
| `PK_SysMenu_Id` | CLUSTERED |  |  | `Id` |

#### SysNotice (系统通知公告表)

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Title` | `nvarchar(256)` | NO |  |  |  | 标题 |
| 3 | `Content` | `nvarchar(MAX)` | NO |  |  |  | 内容 |
| 4 | `Type` | `int` | NO |  |  | ('1') | 类型（1通知 2公告） |
| 5 | `PublicUserId` | `bigint` | NO |  |  |  | 发布人Id |
| 6 | `PublicUserName` | `nvarchar(32)` | YES |  |  |  | 发布人姓名 |
| 7 | `PublicOrgId` | `bigint` | NO |  |  |  | 发布机构Id |
| 8 | `PublicOrgName` | `nvarchar(64)` | YES |  |  |  | 发布机构名称 |
| 9 | `PublicTime` | `datetime` | YES |  |  |  | 发布时间 |
| 10 | `CancelTime` | `datetime` | YES |  |  |  | 撤回时间 |
| 11 | `Status` | `int` | NO |  |  | ('0') | 状态（0草稿 1发布 2撤回 3删除） |
| 12 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 13 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 14 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 15 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 16 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 17 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 18 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 19 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 20 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysNotice_t` | NONCLUSTERED |  |  | `Type` |
| `PK_SysNotice_Id` | CLUSTERED |  |  | `Id` |

#### SysNoticeUser (系统通知公告用户表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `NoticeId` | `bigint` | NO |  |  |  | 通知公告Id |
| 3 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 4 | `ReadTime` | `datetime` | YES |  |  |  | 阅读时间 |
| 5 | `ReadStatus` | `int` | NO |  |  | ('0') | 状态（0未读 1已读） |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysNoticeUser_Id` | CLUSTERED |  |  | `Id` |

#### SysOAuthUser (系统三方用户表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 系统用户Id |
| 3 | `PlatformType` | `int` | NO |  |  | ('1') | 平台类型 |
| 4 | `OpenId` | `nvarchar(64)` | YES |  |  |  | OpenId |
| 5 | `SessionKey` | `nvarchar(256)` | YES |  |  |  | 会话密钥 |
| 6 | `UnionId` | `nvarchar(64)` | YES |  |  |  | UnionId |
| 7 | `Account` | `nvarchar(128)` | YES |  |  |  | 账号 |
| 8 | `NickName` | `nvarchar(64)` | YES |  |  |  | 昵称 |
| 9 | `Avatar` | `nvarchar(256)` | YES |  |  |  | 头像 |
| 10 | `Email` | `nvarchar(128)` | YES |  |  |  | 邮箱 |
| 11 | `Mobile` | `nvarchar(16)` | YES |  |  |  | 手机号码 |
| 12 | `Sex` | `int` | YES |  |  |  | 性别 |
| 13 | `IpAddress` | `nvarchar(256)` | YES |  |  |  | IP 地址 |
| 14 | `Language` | `nvarchar(64)` | YES |  |  |  | 语言 |
| 15 | `City` | `nvarchar(64)` | YES |  |  |  | 城市 |
| 16 | `Province` | `nvarchar(64)` | YES |  |  |  | 省 |
| 17 | `Country` | `nvarchar(64)` | YES |  |  |  | 国家 |
| 18 | `AccessToken` | `nvarchar(MAX)` | YES |  |  |  | AccessToken |
| 19 | `RefreshToken` | `nvarchar(MAX)` | YES |  |  |  | RefreshToken |
| 20 | `ExpiresIn` | `int` | YES |  |  |  | ExpiresIn |
| 21 | `Scope` | `nvarchar(64)` | YES |  |  |  | 授权作用域 |
| 22 | `Remark` | `nvarchar(512)` | YES |  |  |  | 备注 |
| 23 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 24 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 25 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 26 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 27 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 28 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 29 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 30 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysOAuthUser_m` | NONCLUSTERED |  |  | `Mobile` |
| `i_SysOAuthUser_n` | NONCLUSTERED |  |  | `NickName` |
| `PK_SysOAuthUser_Id` | CLUSTERED |  |  | `Id` |

#### SysOpenAccess (开放接口身份表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `AccessKey` | `nvarchar(128)` | NO |  |  |  | 身份标识 |
| 3 | `AccessSecret` | `nvarchar(256)` | NO |  |  |  | 密钥 |
| 4 | `BindTenantId` | `bigint` | NO |  |  |  | 绑定租户Id |
| 5 | `BindUserId` | `bigint` | NO |  |  |  | 绑定用户Id |
| 6 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 7 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 8 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 9 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 10 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 11 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 12 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 13 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysOpenAccess_a` | NONCLUSTERED |  |  | `AccessKey` |
| `PK_SysOpenAccess_Id` | CLUSTERED |  |  | `Id` |

#### SysOrg (系统机构表)

- Schema: `dbo`
- Rows: 5

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Pid` | `bigint` | NO |  |  |  | 父Id |
| 3 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 4 | `Code` | `nvarchar(64)` | YES |  |  |  | 编码 |
| 5 | `Level` | `int` | YES |  |  |  | 级别 |
| 6 | `Type` | `nvarchar(64)` | YES |  |  |  | 机构类型 |
| 7 | `DirectorId` | `bigint` | YES |  |  |  | 负责人Id |
| 8 | `Longitude` | `decimal(18,4)` | NO |  |  | ('0') | 经度 |
| 9 | `Latitude` | `decimal(18,4)` | NO |  |  | ('0') | 纬度 |
| 10 | `OrderNo` | `int` | NO |  |  |  | 排序 |
| 11 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 12 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 13 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 14 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 15 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 16 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 17 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 18 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 19 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 20 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 21 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysOrg_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysOrg_n` | NONCLUSTERED |  |  | `Name` |
| `i_SysOrg_t` | NONCLUSTERED |  |  | `Type` |
| `PK_SysOrg_Id` | CLUSTERED |  |  | `Id` |

#### SysPlugin (系统动态插件表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `CsharpCode` | `nvarchar(MAX)` | NO |  |  |  | C#代码 |
| 4 | `AssemblyName` | `nvarchar(512)` | YES |  |  |  | 程序集名称 |
| 5 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 6 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 7 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 8 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 9 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 10 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 12 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 13 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 14 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 15 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 16 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysPlugin_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysPlugin_Id` | CLUSTERED |  |  | `Id` |

#### SysPos (系统职位表)

- Schema: `dbo`
- Rows: 4

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `Code` | `nvarchar(64)` | YES |  |  |  | 编码 |
| 4 | `Type` | `nvarchar(64)` | YES |  |  |  | 类别 |
| 5 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 6 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 7 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 8 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 9 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 10 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 12 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 13 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 14 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 15 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 16 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysPos_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysPos_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysPos_Id` | CLUSTERED |  |  | `Id` |

#### SysPrint (系统打印模板表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `Template` | `nvarchar(MAX)` | NO |  |  |  | 打印模板 |
| 4 | `PrintType` | `int` | NO |  |  | ('1') | 打印类型 |
| 5 | `ClientServiceAddress` | `nvarchar(128)` | YES |  |  |  | 客户端服务地址 |
| 6 | `PrintParam` | `nvarchar(MAX)` | YES |  |  |  | 打印参数 |
| 7 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 8 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 9 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 10 | `PrintDataDemo` | `nvarchar(MAX)` | YES |  |  |  | 打印预览测试数据 |
| 11 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 12 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 13 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 14 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 15 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 16 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 17 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 18 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 19 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysPrint_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysPrint_Id` | CLUSTERED |  |  | `Id` |

#### SysRegion (系统行政地区表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Pid` | `bigint` | NO |  |  |  | 父Id |
| 3 | `Name` | `nvarchar(128)` | NO |  |  |  | 名称 |
| 4 | `ShortName` | `nvarchar(32)` | YES |  |  |  | 简称 |
| 5 | `MergerName` | `nvarchar(64)` | YES |  |  |  | 组合名 |
| 6 | `Code` | `nvarchar(32)` | YES |  |  |  | 行政代码 |
| 7 | `ZipCode` | `nvarchar(6)` | YES |  |  |  | 邮政编码 |
| 8 | `CityCode` | `nvarchar(6)` | YES |  |  |  | 区号 |
| 9 | `Level` | `int` | NO |  |  |  | 层级 |
| 10 | `Type` | `nvarchar(32)` | YES |  |  |  | 类型 |
| 11 | `PinYin` | `nvarchar(128)` | YES |  |  |  | 拼音 |
| 12 | `Longitude` | `decimal(18,4)` | NO |  |  | ('0') | 经度 |
| 13 | `Latitude` | `decimal(18,4)` | NO |  |  | ('0') | 纬度 |
| 14 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 15 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysRegion_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysRegion_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysRegion_Id` | CLUSTERED |  |  | `Id` |

#### SysReportConfig (系统报表配置表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(254)` | NO |  |  |  | 名称 |
| 3 | `Description` | `nvarchar(512)` | YES |  |  |  | 描述 |
| 4 | `DsType` | `int` | NO |  |  |  | 数据源类型 |
| 5 | `DataSource` | `nvarchar(64)` | YES |  |  |  | 数据源 |
| 6 | `GroupId` | `bigint` | YES |  |  |  | 分组Id |
| 7 | `SqlScript` | `nvarchar(MAX)` | YES |  |  |  | 脚本语句 |
| 8 | `ApiUrl` | `nvarchar(1024)` | YES |  |  |  | 接口地址 |
| 9 | `ApiHttpMethod` | `nvarchar(16)` | YES |  |  |  | 接口请求方式 |
| 10 | `ApiParams` | `nvarchar(1024)` | YES |  |  |  | 接口参数 |
| 11 | `Params` | `nvarchar(MAX)` | YES |  |  |  | 参数 |
| 12 | `Fields` | `nvarchar(MAX)` | YES |  |  |  | 列表字段 |
| 13 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 14 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 15 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 16 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 17 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 18 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 19 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 20 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysReportConfig_Id` | CLUSTERED |  |  | `Id` |

#### SysReportDataSource (系统报表数据源表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(254)` | NO |  |  |  | 名称 |
| 3 | `DbType` | `int` | NO |  |  | ('4') | 数据库类型 |
| 4 | `ConnectionString` | `nvarchar(512)` | NO |  |  |  | 连接字符串 |
| 5 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 6 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 7 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 8 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 9 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 10 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 11 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 12 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 13 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysReportDataSource_Id` | CLUSTERED |  |  | `Id` |

#### SysReportGroup (系统报表分组表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Number` | `nvarchar(64)` | NO |  |  |  | 编码 |
| 3 | `Name` | `nvarchar(254)` | NO |  |  |  | 名称 |
| 4 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 5 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 6 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 7 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 8 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 9 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 10 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 11 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysReportGroup_Id` | CLUSTERED |  |  | `Id` |

#### SysRole (系统角色表)

- Schema: `dbo`
- Rows: 5

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 名称 |
| 3 | `Code` | `nvarchar(64)` | YES |  |  |  | 编码 |
| 4 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 5 | `DataScope` | `int` | NO |  |  | ('4') | 数据范围 |
| 6 | `SysFlag` | `int` | NO |  |  | ('2') | 是否是内置 |
| 7 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 8 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 9 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 10 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 11 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 12 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 13 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 14 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 15 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 16 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 17 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysRole_c` | NONCLUSTERED |  |  | `Code` |
| `i_SysRole_n` | NONCLUSTERED |  |  | `Name` |
| `PK_SysRole_Id` | CLUSTERED |  |  | `Id` |

#### SysRoleApi (系统角色接口黑名单表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |
| 3 | `Route` | `nvarchar(MAX)` | NO |  |  |  | 接口路由 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysRoleApi_Id` | CLUSTERED |  |  | `Id` |

#### SysRoleMenu (系统角色菜单表)

- Schema: `dbo`
- Rows: 211

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |
| 3 | `MenuId` | `bigint` | NO |  |  |  | 菜单Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysRoleMenu_Id` | CLUSTERED |  |  | `Id` |

#### SysRoleOrg (系统角色机构表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |
| 3 | `OrgId` | `bigint` | NO |  |  |  | 机构Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysRoleOrg_Id` | CLUSTERED |  |  | `Id` |

#### SysRoleTable (系统角色表格表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |
| 3 | `TableName` | `nvarchar(64)` | NO |  |  |  | 表名 |
| 4 | `ColumnName` | `nvarchar(64)` | NO |  |  |  | 列名 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysRoleTable_Id` | CLUSTERED |  |  | `Id` |

#### SysSchedule (系统日程表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `ScheduleTime` | `datetime` | YES |  |  |  | 日程日期 |
| 4 | `StartTime` | `nvarchar(10)` | YES |  |  |  | 开始时间 |
| 5 | `EndTime` | `nvarchar(10)` | YES |  |  |  | 结束时间 |
| 6 | `Content` | `nvarchar(256)` | NO |  |  |  | 日程内容 |
| 7 | `Status` | `int` | NO |  |  | ('0') | 完成状态 |
| 8 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 9 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 10 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 11 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 12 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 13 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 14 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 15 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 16 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysSchedule_Id` | CLUSTERED |  |  | `Id` |

#### SysSerial (系统流水号表)

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Seq` | `bigint` | NO |  |  |  | 序列号 |
| 3 | `Expy` | `datetime` | NO |  |  |  | 有效期 |
| 4 | `Type` | `nvarchar(32)` | NO |  |  |  | 使用分类 |
| 5 | `ResetInterval` | `int` | NO |  |  |  | 重置间隔 |
| 6 | `Formater` | `nvarchar(128)` | NO |  |  |  | 格式化表达式 |
| 7 | `Min` | `bigint` | NO |  |  |  | 最小值 |
| 8 | `Max` | `bigint` | NO |  |  |  | 最大值 |
| 9 | `OrderNo` | `int` | NO |  |  |  | 排序 |
| 10 | `Status` | `int` | NO |  |  |  | 状态 |
| 11 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 12 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 13 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 14 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 15 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 16 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 17 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 18 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 19 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 20 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysSerial_Id` | CLUSTERED |  |  | `Id` |
| `u_SysSerial_tt` | NONCLUSTERED |  |  | `Type, TenantId` |

#### SysTenant (系统租户表)

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 租管用户Id |
| 3 | `OrgId` | `bigint` | NO |  |  |  | 机构Id |
| 4 | `Host` | `nvarchar(128)` | YES |  |  |  | 域名 |
| 5 | `TenantType` | `int` | NO |  |  | ('0') | 租户类型 |
| 6 | `DbType` | `int` | NO |  |  | ('4') | 数据库类型 |
| 7 | `Connection` | `nvarchar(512)` | YES |  |  |  | 数据库连接 |
| 8 | `ConfigId` | `nvarchar(64)` | YES |  |  |  | 数据库标识 |
| 9 | `SlaveConnections` | `nvarchar(MAX)` | YES |  |  |  | 从库连接/读写分离 |
| 10 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 11 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 12 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 13 | `ExpirationTime` | `datetime` | YES |  |  |  | 过期时间 |
| 14 | `UserCount` | `int` | YES |  |  | ('0') | 用户数量 |
| 15 | `Logo` | `nvarchar(256)` | YES |  |  |  | 系统图标 |
| 16 | `Title` | `nvarchar(32)` | YES |  |  |  | 主标题 |
| 17 | `ViceTitle` | `nvarchar(64)` | YES |  |  |  | 副标题 |
| 18 | `ViceDesc` | `nvarchar(256)` | YES |  |  |  | 副描述 |
| 19 | `Copyright` | `nvarchar(128)` | YES |  |  |  | 版权信息 |
| 20 | `Icp` | `nvarchar(32)` | YES |  |  |  | ICP备案号 |
| 21 | `IcpUrl` | `nvarchar(32)` | YES |  |  |  | ICP地址 |
| 22 | `Watermark` | `nvarchar(32)` | YES |  |  |  | 水印 |
| 23 | `Version` | `nvarchar(16)` | YES |  |  |  | 版本号 |
| 24 | `ThemeColor` | `nvarchar(16)` | YES |  |  | ('#0F59A4') | 主题颜色 |
| 25 | `Layout` | `nvarchar(32)` | YES |  |  | ('columns') | 布局模式 |
| 26 | `Animation` | `nvarchar(32)` | YES |  |  | ('fadeDown') | 页面动画 |
| 27 | `Captcha` | `bit` | YES |  |  |  | 图形验证码 |
| 28 | `SecondVer` | `bit` | YES |  |  |  | 登录二次验证 |
| 29 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 30 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 31 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 32 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 33 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 34 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 35 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 36 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysTenant_Id` | CLUSTERED |  |  | `Id` |

#### SysUpgrade (系统更新日志表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Content` | `nvarchar(MAX)` | NO |  |  |  | 内容 |
| 3 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 4 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 5 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 6 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 7 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 8 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 9 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 10 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUpgrade_Id` | CLUSTERED |  |  | `Id` |

#### SysUpgradeUser (系统更新日志用户表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UpgradeId` | `bigint` | NO |  |  |  | 更新日志Id |
| 3 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 4 | `ReadTime` | `datetime` | YES |  |  |  | 阅读时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUpgradeUser_Id` | CLUSTERED |  |  | `Id` |

#### SysUser (系统用户表)

- Schema: `dbo`
- Rows: 5

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Account` | `nvarchar(32)` | NO |  |  |  | 账号 |
| 3 | `Password` | `nvarchar(512)` | NO |  |  |  | 密码 |
| 4 | `RealName` | `nvarchar(32)` | NO |  |  |  | 真实姓名 |
| 5 | `NickName` | `nvarchar(32)` | YES |  |  |  | 昵称 |
| 6 | `Avatar` | `nvarchar(512)` | YES |  |  |  | 头像 |
| 7 | `Sex` | `int` | NO |  |  | ('1') | 性别 |
| 8 | `Age` | `int` | NO |  |  |  | 年龄 |
| 9 | `Birthday` | `datetime` | YES |  |  |  | 出生日期 |
| 10 | `Nation` | `nvarchar(32)` | YES |  |  |  | 民族 |
| 11 | `Phone` | `nvarchar(16)` | YES |  |  |  | 手机号码 |
| 12 | `CardType` | `int` | NO |  |  | ('0') | 证件类型 |
| 13 | `IdCardNum` | `nvarchar(32)` | YES |  |  |  | 身份证号 |
| 14 | `Email` | `nvarchar(64)` | YES |  |  |  | 邮箱 |
| 15 | `Address` | `nvarchar(256)` | YES |  |  |  | 地址 |
| 16 | `CultureLevel` | `int` | NO |  |  | ('10') | 文化程度 |
| 17 | `PoliticalOutlook` | `nvarchar(16)` | YES |  |  |  | 政治面貌 |
| 18 | `College` | `nvarchar(128)` | YES |  |  |  | 毕业院校 |
| 19 | `OfficePhone` | `nvarchar(16)` | YES |  |  |  | 办公电话 |
| 20 | `EmergencyContact` | `nvarchar(32)` | YES |  |  |  | 紧急联系人 |
| 21 | `EmergencyPhone` | `nvarchar(16)` | YES |  |  |  | 紧急联系人电话 |
| 22 | `EmergencyAddress` | `nvarchar(256)` | YES |  |  |  | 紧急联系人地址 |
| 23 | `Introduction` | `nvarchar(512)` | YES |  |  |  | 个人简介 |
| 24 | `OrderNo` | `int` | NO |  |  |  | 排序 |
| 25 | `Status` | `int` | NO |  |  | ('1') | 状态 |
| 26 | `Remark` | `nvarchar(256)` | YES |  |  |  | 备注 |
| 27 | `AccountType` | `int` | NO |  |  | ('777') | 账号类型 |
| 28 | `OrgId` | `bigint` | NO |  |  |  | 直属机构Id |
| 29 | `ManagerUserId` | `bigint` | YES |  |  |  | 直属主管Id |
| 30 | `PosId` | `bigint` | NO |  |  |  | 职位Id |
| 31 | `JobNum` | `nvarchar(32)` | YES |  |  |  | 工号 |
| 32 | `PosLevel` | `nvarchar(32)` | YES |  |  |  | 职级 |
| 33 | `PosTitle` | `nvarchar(32)` | YES |  |  |  | 职称 |
| 34 | `Expertise` | `nvarchar(32)` | YES |  |  |  | 擅长领域 |
| 35 | `OfficeZone` | `nvarchar(32)` | YES |  |  |  | 办公区域 |
| 36 | `Office` | `nvarchar(32)` | YES |  |  |  | 办公室 |
| 37 | `JoinDate` | `datetime` | YES |  |  |  | 入职日期 |
| 38 | `LastLoginIp` | `nvarchar(256)` | YES |  |  |  | 最新登录Ip |
| 39 | `LastLoginAddress` | `nvarchar(128)` | YES |  |  |  | 最新登录地点 |
| 40 | `LastLoginTime` | `datetime` | YES |  |  |  | 最新登录时间 |
| 41 | `LastLoginDevice` | `nvarchar(128)` | YES |  |  |  | 最新登录设备 |
| 42 | `Signature` | `nvarchar(512)` | YES |  |  |  | 电子签名 |
| 43 | `TokenVersion` | `int` | NO |  |  | ('1') | Token版本号 |
| 44 | `LastChangePasswordTime` | `datetime` | YES |  |  |  | 最新密码修改时间 |
| 45 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 46 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 47 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 48 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 49 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 50 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 51 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 52 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 53 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysUser_a` | NONCLUSTERED |  |  | `Account` |
| `i_SysUser_p` | NONCLUSTERED |  |  | `Phone` |
| `PK_SysUser_Id` | CLUSTERED |  |  | `Id` |

#### SysUserExtOrg (系统用户扩展机构表)

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `OrgId` | `bigint` | NO |  |  |  | 机构Id |
| 4 | `PosId` | `bigint` | NO |  |  |  | 职位Id |
| 5 | `JobNum` | `nvarchar(32)` | YES |  |  |  | 工号 |
| 6 | `PosLevel` | `nvarchar(32)` | YES |  |  |  | 职级 |
| 7 | `JoinDate` | `datetime` | YES |  |  |  | 入职日期 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUserExtOrg_Id` | CLUSTERED |  |  | `Id` |

#### SysUserLdap (系统用户域配置表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `Account` | `nvarchar(32)` | NO |  |  |  | 域账号 |
| 4 | `UserName` | `nvarchar(32)` | NO |  |  |  | 域用户名 |
| 5 | `EmployeeId` | `nvarchar(32)` | YES |  |  |  | 对应EmployeeId |
| 6 | `DeptCode` | `nvarchar(64)` | YES |  |  |  | 组织代码 |
| 7 | `PwdLastSetTime` | `datetime` | YES |  |  |  | 最后设置密码时间 |
| 8 | `Mail` | `nvarchar(64)` | YES |  |  |  | 组织代码 |
| 9 | `AccountExpiresFlag` | `bit` | NO |  |  |  | 检查账户是否已过期 |
| 10 | `DontExpiresFlag` | `bit` | NO |  |  |  | 密码设置是否永不过期 |
| 11 | `Dn` | `nvarchar(512)` | NO |  |  |  | DN |
| 12 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 13 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 14 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 15 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 16 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 17 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 18 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 19 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 20 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysUserLdap_a` | NONCLUSTERED |  |  | `Account` |
| `i_SysUserLdap_u` | NONCLUSTERED |  |  | `UserId` |
| `PK_SysUserLdap_Id` | CLUSTERED |  |  | `Id` |

#### SysUserMenu (系统用户菜单快捷导航表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `MenuId` | `bigint` | NO |  |  |  | 菜单Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUserMenu_Id` | CLUSTERED |  |  | `Id` |

#### SysUserPasswordRecord (系统用户密码记录表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `Password` | `nvarchar(512)` | NO |  |  |  | 密码 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUserPasswordRecord_Id` | CLUSTERED |  |  | `Id` |

#### SysUserRegWay (系统用户注册方案表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(64)` | NO |  |  |  | 方案名称 |
| 3 | `AccountType` | `int` | NO |  |  |  | 账号类型 |
| 4 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |
| 5 | `OrgId` | `bigint` | NO |  |  |  | 机构Id |
| 6 | `PosId` | `bigint` | NO |  |  |  | 职位Id |
| 7 | `OrderNo` | `int` | NO |  |  | ('100') | 排序 |
| 8 | `Remark` | `nvarchar(128)` | YES |  |  |  | 备注 |
| 9 | `TenantId` | `bigint` | YES |  |  |  | 租户Id |
| 10 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 11 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 12 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 13 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 14 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 15 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 16 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 17 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUserRegWay_Id` | CLUSTERED |  |  | `Id` |

#### SysUserRole (系统用户角色表)

- Schema: `dbo`
- Rows: 4

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `UserId` | `bigint` | NO |  |  |  | 用户Id |
| 3 | `RoleId` | `bigint` | NO |  |  |  | 角色Id |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_SysUserRole_Id` | CLUSTERED |  |  | `Id` |

#### SysWechatPay (系统微信支付表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `OrderId` | `nvarchar(255)` | YES |  |  |  | 关联的商户订单号 |
| 3 | `OrderStatus` | `nvarchar(255)` | YES |  |  |  | 关联的商户订单状态 |
| 4 | `MerchantId` | `nvarchar(255)` | NO |  |  |  | 微信商户号 |
| 5 | `AppId` | `nvarchar(255)` | NO |  |  |  | 服务商AppId |
| 6 | `OutTradeNumber` | `nvarchar(255)` | NO |  |  |  | 商户订单号 |
| 7 | `TransactionId` | `nvarchar(255)` | NO |  |  |  | 支付订单号 |
| 8 | `TradeType` | `nvarchar(255)` | YES |  |  |  | 交易类型 |
| 9 | `TradeState` | `nvarchar(255)` | YES |  |  |  | 交易状态 |
| 10 | `TradeStateDescription` | `nvarchar(255)` | YES |  |  |  | 交易状态描述 |
| 11 | `BankType` | `nvarchar(255)` | YES |  |  |  | 付款银行类型 |
| 12 | `Total` | `int` | NO |  |  |  | 订单总金额 |
| 13 | `PayerTotal` | `int` | YES |  |  |  | 用户支付金额 |
| 14 | `SuccessTime` | `datetime` | YES |  |  |  | 支付完成时间 |
| 15 | `ExpireTime` | `datetime` | YES |  |  |  | 交易结束时间 |
| 16 | `Description` | `nvarchar(255)` | YES |  |  |  | 商品描述 |
| 17 | `Scene` | `nvarchar(255)` | YES |  |  |  | 场景信息 |
| 18 | `Attachment` | `nvarchar(255)` | YES |  |  |  | 附加数据 |
| 19 | `GoodsTag` | `nvarchar(255)` | YES |  |  |  | 优惠标记 |
| 20 | `Settlement` | `nvarchar(255)` | YES |  |  |  | 结算信息 |
| 21 | `NotifyUrl` | `nvarchar(255)` | YES |  |  |  | 回调通知地址 |
| 22 | `Remark` | `nvarchar(MAX)` | YES |  |  |  | 备注 |
| 23 | `OpenId` | `nvarchar(255)` | YES |  |  |  | 微信OpenId标识 |
| 24 | `Tags` | `nvarchar(64)` | YES |  |  |  | 业务标签，用来区分做什么业务 |
| 25 | `BusinessId` | `bigint` | NO |  |  |  | 对应业务的主键 |
| 26 | `QrcodeContent` | `nvarchar(255)` | YES |  |  |  | 付款二维码内容 |
| 27 | `SubMerchantId` | `nvarchar(255)` | YES |  |  |  | 子商户号 |
| 28 | `SubAppId` | `nvarchar(255)` | YES |  |  |  | 子商户AppId |
| 29 | `SubOpenId` | `nvarchar(255)` | YES |  |  |  | 子商户唯一标识 |
| 30 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 31 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 32 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 33 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 34 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 35 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 36 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 37 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysWechatPay_o` | NONCLUSTERED |  |  | `OrderId` |
| `PK_SysWechatPay_Id` | CLUSTERED |  |  | `Id` |

#### SysWechatRefund (系统微信退款表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `TransactionId` | `nvarchar(32)` | NO |  |  |  | 微信支付订单号 |
| 3 | `OutTradeNumber` | `nvarchar(32)` | NO |  |  |  | 商户付款单号 |
| 4 | `OutRefundNumber` | `nvarchar(64)` | NO |  |  |  | 商户退款单号 |
| 5 | `RefundId` | `nvarchar(255)` | NO |  |  |  | 微信接口退款ID |
| 6 | `Reason` | `nvarchar(80)` | NO |  |  |  | 退款原因 |
| 7 | `Refund` | `int` | NO |  |  |  | 退款金额 |
| 8 | `Total` | `int` | NO |  |  |  | 订单总金额 |
| 9 | `NotifyUrl` | `nvarchar(256)` | YES |  |  |  | 退款结果回调url |
| 10 | `FundsAccount` | `nvarchar(32)` | YES |  |  |  | 退款资金来源 |
| 11 | `OrderId` | `nvarchar(256)` | YES |  |  |  | 关联的用户订单号 |
| 12 | `RefundStatus` | `nvarchar(32)` | YES |  |  |  | 关联的商户订单状态 |
| 13 | `SuccessTime` | `datetime` | YES |  |  |  | 完成时间 |
| 14 | `MerchantGoodsId` | `nvarchar(32)` | YES |  |  |  | 关联的商户商品编码 |
| 15 | `GoodsName` | `nvarchar(256)` | YES |  |  |  | 关联的商户商品名称 |
| 16 | `UnitPrice` | `int` | NO |  |  |  | 关联的商户商品单价 |
| 17 | `RefundAmount` | `int` | NO |  |  |  | 关联的商户商品退款金额 |
| 18 | `RefundQuantity` | `int` | NO |  |  |  | 关联的商户商品退货数量 |
| 19 | `Attachment` | `nvarchar(255)` | YES |  |  |  | 附加数据 |
| 20 | `Remark` | `nvarchar(MAX)` | YES |  |  |  | 备注 |
| 21 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 22 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 23 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 24 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 25 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 26 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 27 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 28 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `i_SysWechatRefund_o` | NONCLUSTERED |  |  | `OrderId` |
| `PK_SysWechatRefund_Id` | CLUSTERED |  |  | `Id` |

### 07-Demo-Test

#### TestCodeGenDemoMaster (代码自动生成DEMO主表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `Name` | `nvarchar(20)` | NO |  |  |  | 名称 |
| 3 | `Remark` | `nvarchar(120)` | YES |  |  |  | 备注 |
| 4 | `Dict1` | `nvarchar(255)` | YES |  |  |  | 字典1 |
| 5 | `UploadImageId` | `bigint` | YES |  |  |  | 上传控件(保存主键) |
| 6 | `UploadImage` | `nvarchar(255)` | YES |  |  |  | 上传控件(保存路径) |
| 7 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 8 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 9 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 10 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 11 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 12 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 13 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 14 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_TestCodeGenDemoMaster_Id` | CLUSTERED |  |  | `Id` |

#### TestCodeGenDemoSlave (代码自动生成DEMO子表)

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `Id` | `bigint` | NO | Y |  |  | 主键Id |
| 2 | `MasterId` | `bigint` | NO |  |  | ('0') | 主表记录ID |
| 3 | `SeqNo` | `nvarchar(128)` | YES |  |  |  | 流水号 |
| 4 | `Name` | `nvarchar(128)` | YES |  |  |  | 文本 |
| 5 | `Age` | `int` | YES |  |  |  | 数值 |
| 6 | `Date1` | `datetime` | YES |  |  |  | 时间选择 |
| 7 | `IsOk` | `bit` | YES |  |  |  | 开关 |
| 8 | `UserId` | `bigint` | YES |  |  |  | 外键(sys_user) |
| 9 | `OrgId` | `bigint` | YES |  |  |  | 树选择框1(sys_org) |
| 10 | `MenuId` | `bigint` | YES |  |  |  | 树选择框2(sys_menu) |
| 11 | `Dict1` | `nvarchar(255)` | YES |  |  |  | 字典1 |
| 12 | `Enum1` | `int` | YES |  |  |  | 枚举1 |
| 13 | `Const1` | `int` | YES |  |  |  | 常量1 |
| 14 | `UploadImageId` | `bigint` | YES |  |  |  | 上传控件(保存主键) |
| 15 | `UploadImage` | `nvarchar(255)` | YES |  |  |  | 上传控件(保存路径) |
| 16 | `CreateTime` | `datetime` | YES |  |  |  | 创建时间 |
| 17 | `UpdateTime` | `datetime` | YES |  |  |  | 更新时间 |
| 18 | `CreateUserId` | `bigint` | YES |  |  |  | 创建者Id |
| 19 | `CreateUserName` | `nvarchar(64)` | YES |  |  |  | 创建者姓名 |
| 20 | `UpdateUserId` | `bigint` | YES |  |  |  | 修改者Id |
| 21 | `UpdateUserName` | `nvarchar(64)` | YES |  |  |  | 修改者姓名 |
| 22 | `IsDelete` | `bit` | NO |  |  |  | 软删除 |
| 23 | `DeleteTime` | `datetime` | YES |  |  |  | 软删除时间 |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_TestCodeGenDemoSlave_Id` | CLUSTERED |  |  | `Id` |

### 08-ZeroCode

#### zero_database_info

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `connection` | `nvarchar(255)` | NO |  |  |  |  |
| 4 | `db_type` | `int` | NO |  |  |  |  |
| 5 | `sort_id` | `int` | NO |  |  |  |  |
| 6 | `create_time` | `datetime` | NO |  |  |  |  |
| 7 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 8 | `creator_id` | `bigint` | NO |  |  |  |  |
| 9 | `update_time` | `datetime` | YES |  |  |  |  |
| 10 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 11 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 12 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `is_deleted` | `bit` | NO |  |  |  |  |
| 14 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_initialized` | `bit` | NO |  |  |  |  |
| 16 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_database_info_id` | CLUSTERED |  |  | `id` |

#### zero_entity_column_info

- Schema: `dbo`
- Rows: 172

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `table_id` | `bigint` | NO |  |  |  |  |
| 3 | `db_column_name` | `nvarchar(255)` | NO |  |  |  |  |
| 4 | `property_name` | `nvarchar(255)` | NO |  |  |  |  |
| 5 | `length` | `int` | NO |  |  |  |  |
| 6 | `decimal_digits` | `int` | NO |  |  |  |  |
| 7 | `description` | `nvarchar(255)` | YES |  |  |  |  |
| 8 | `is_identity` | `bit` | NO |  |  |  |  |
| 9 | `is_primarykey` | `bit` | NO |  |  |  |  |
| 10 | `is_array` | `bit` | NO |  |  |  |  |
| 11 | `is_json` | `bit` | NO |  |  |  |  |
| 12 | `is_nullable` | `bit` | NO |  |  |  |  |
| 13 | `scale` | `int` | NO |  |  |  |  |
| 14 | `is_unsigned` | `bit` | NO |  |  |  |  |
| 15 | `property_type` | `int` | NO |  |  |  |  |
| 16 | `data_type` | `nvarchar(255)` | YES |  |  |  |  |
| 17 | `extended_attribute` | `nvarchar(200)` | YES |  |  |  |  |
| 18 | `sort_id` | `int` | NO |  |  |  |  |
| 19 | `create_time` | `datetime` | NO |  |  |  |  |
| 20 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 21 | `creator_id` | `bigint` | NO |  |  |  |  |
| 22 | `update_time` | `datetime` | YES |  |  |  |  |
| 23 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 24 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 25 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 26 | `is_deleted` | `bit` | NO |  |  |  |  |
| 27 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 28 | `is_initialized` | `bit` | NO |  |  |  |  |
| 29 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_entity_column_info_id` | CLUSTERED |  |  | `id` |

#### zero_entity_info

- Schema: `dbo`
- Rows: 9

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `class_name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `db_table_name` | `nvarchar(255)` | NO |  |  |  |  |
| 4 | `column_count` | `int` | YES |  |  |  |  |
| 5 | `data_base_id` | `bigint` | NO |  |  |  |  |
| 6 | `description` | `nvarchar(255)` | YES |  |  |  |  |
| 7 | `sort_id` | `int` | NO |  |  |  |  |
| 8 | `create_time` | `datetime` | NO |  |  |  |  |
| 9 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 10 | `creator_id` | `bigint` | NO |  |  |  |  |
| 11 | `update_time` | `datetime` | YES |  |  |  |  |
| 12 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_deleted` | `bit` | NO |  |  |  |  |
| 16 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 17 | `is_initialized` | `bit` | NO |  |  |  |  |
| 18 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_entity_info_id` | CLUSTERED |  |  | `id` |

#### zero_interface_category

- Schema: `dbo`
- Rows: 15

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `parent_id` | `bigint` | YES |  |  |  |  |
| 4 | `description` | `nvarchar(255)` | YES |  |  |  |  |
| 5 | `url` | `nvarchar(255)` | NO |  |  |  |  |
| 6 | `icon` | `nvarchar(255)` | YES |  |  |  |  |
| 7 | `sort_id` | `int` | NO |  |  |  |  |
| 8 | `create_time` | `datetime` | NO |  |  |  |  |
| 9 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 10 | `creator_id` | `bigint` | NO |  |  |  |  |
| 11 | `update_time` | `datetime` | YES |  |  |  |  |
| 12 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_deleted` | `bit` | NO |  |  |  |  |
| 16 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 17 | `is_initialized` | `bit` | NO |  |  |  |  |
| 18 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_interface_category_id` | CLUSTERED |  |  | `id` |

#### zero_interface_list

- Schema: `dbo`
- Rows: 59

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `url` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `original_url` | `nvarchar(255)` | YES |  |  |  |  |
| 4 | `name` | `nvarchar(255)` | NO |  |  |  |  |
| 5 | `interface_category_id` | `bigint` | NO |  |  |  |  |
| 6 | `custom_result_model` | `nvarchar(MAX)` | YES |  |  |  |  |
| 7 | `description` | `nvarchar(255)` | YES |  |  |  |  |
| 8 | `group_name` | `nvarchar(255)` | NO |  |  |  |  |
| 9 | `http_method` | `nvarchar(255)` | NO |  |  |  |  |
| 10 | `data_model` | `nvarchar(MAX)` | NO |  |  |  |  |
| 11 | `database_id` | `bigint` | YES |  |  |  |  |
| 12 | `sort_id` | `int` | NO |  |  |  |  |
| 13 | `create_time` | `datetime` | NO |  |  |  |  |
| 14 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 15 | `creator_id` | `bigint` | NO |  |  |  |  |
| 16 | `update_time` | `datetime` | YES |  |  |  |  |
| 17 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 18 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 19 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 20 | `is_deleted` | `bit` | NO |  |  |  |  |
| 21 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 22 | `is_initialized` | `bit` | NO |  |  |  |  |
| 23 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_interface_list_id` | CLUSTERED |  |  | `id` |

#### zero_jwt_token_management

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `user_name` | `nvarchar(200)` | NO |  |  |  |  |
| 3 | `description` | `nvarchar(1000)` | NO |  |  |  |  |
| 4 | `expiration` | `datetime` | NO |  |  |  |  |
| 5 | `token` | `nvarchar(800)` | NO |  |  |  |  |
| 6 | `sort_id` | `int` | NO |  |  |  |  |
| 7 | `create_time` | `datetime` | NO |  |  |  |  |
| 8 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 9 | `creator_id` | `bigint` | NO |  |  |  |  |
| 10 | `update_time` | `datetime` | YES |  |  |  |  |
| 11 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 12 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `is_deleted` | `bit` | NO |  |  |  |  |
| 15 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 16 | `is_initialized` | `bit` | NO |  |  |  |  |
| 17 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_jwt_token_management_id` | CLUSTERED |  |  | `id` |

#### zero_permission_info

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `sort_id` | `int` | NO |  |  |  |  |
| 4 | `create_time` | `datetime` | NO |  |  |  |  |
| 5 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 6 | `creator_id` | `bigint` | NO |  |  |  |  |
| 7 | `update_time` | `datetime` | YES |  |  |  |  |
| 8 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 9 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 10 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 11 | `is_deleted` | `bit` | NO |  |  |  |  |
| 12 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `is_initialized` | `bit` | NO |  |  |  |  |
| 14 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_permission_info_id` | CLUSTERED |  |  | `id` |

#### zero_permission_mapping

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `user_name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `interface_id` | `bigint` | NO |  |  |  |  |
| 4 | `permission_info_id` | `bigint` | NO |  |  |  |  |
| 5 | `sort_id` | `int` | NO |  |  |  |  |
| 6 | `create_time` | `datetime` | NO |  |  |  |  |
| 7 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 8 | `creator_id` | `bigint` | NO |  |  |  |  |
| 9 | `update_time` | `datetime` | YES |  |  |  |  |
| 10 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 11 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 12 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `is_deleted` | `bit` | NO |  |  |  |  |
| 14 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_initialized` | `bit` | NO |  |  |  |  |
| 16 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_permission_mapping_id` | CLUSTERED |  |  | `id` |

#### zero_sys_setting

- Schema: `dbo`
- Rows: 2

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `type_id` | `int` | NO |  |  |  |  |
| 3 | `child_type_id` | `int` | NO |  |  |  |  |
| 4 | `bool_value` | `bit` | NO |  |  |  |  |
| 5 | `string_value` | `nvarchar(255)` | YES |  |  |  |  |
| 6 | `sort_id` | `int` | NO |  |  |  |  |
| 7 | `create_time` | `datetime` | NO |  |  |  |  |
| 8 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 9 | `creator_id` | `bigint` | NO |  |  |  |  |
| 10 | `update_time` | `datetime` | YES |  |  |  |  |
| 11 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 12 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `is_deleted` | `bit` | NO |  |  |  |  |
| 15 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 16 | `is_initialized` | `bit` | NO |  |  |  |  |
| 17 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_sys_setting_id` | CLUSTERED |  |  | `id` |

#### zero_template

- Schema: `dbo`
- Rows: 2

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `type_id` | `int` | NO |  |  |  |  |
| 3 | `title` | `nvarchar(255)` | NO |  |  |  |  |
| 4 | `template_content` | `nvarchar(MAX)` | NO |  |  |  |  |
| 5 | `template_content_style` | `nvarchar(255)` | YES |  |  |  |  |
| 6 | `url` | `nvarchar(255)` | YES |  |  |  |  |
| 7 | `sort_id` | `int` | NO |  |  |  |  |
| 8 | `create_time` | `datetime` | NO |  |  |  |  |
| 9 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 10 | `creator_id` | `bigint` | NO |  |  |  |  |
| 11 | `update_time` | `datetime` | YES |  |  |  |  |
| 12 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_deleted` | `bit` | NO |  |  |  |  |
| 16 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 17 | `is_initialized` | `bit` | NO |  |  |  |  |
| 18 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_template_id` | CLUSTERED |  |  | `id` |

#### zero_template_type

- Schema: `dbo`
- Rows: 0

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `sort_id` | `int` | NO |  |  |  |  |
| 4 | `create_time` | `datetime` | NO |  |  |  |  |
| 5 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 6 | `creator_id` | `bigint` | NO |  |  |  |  |
| 7 | `update_time` | `datetime` | YES |  |  |  |  |
| 8 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 9 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 10 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 11 | `is_deleted` | `bit` | NO |  |  |  |  |
| 12 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `is_initialized` | `bit` | NO |  |  |  |  |
| 14 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_template_type_id` | CLUSTERED |  |  | `id` |

#### zero_user_info

- Schema: `dbo`
- Rows: 1

| # | Column | Type | Nullable | PK | Identity | Default | Description |
|---|--------|------|----------|----|----------|---------|-------------|
| 1 | `id` | `bigint` | NO | Y |  |  |  |
| 2 | `user_name` | `nvarchar(255)` | NO |  |  |  |  |
| 3 | `password` | `nvarchar(255)` | NO |  |  |  |  |
| 4 | `is_master_admin` | `bit` | NO |  |  |  |  |
| 5 | `avatar` | `nvarchar(255)` | YES |  |  |  |  |
| 6 | `business_account` | `nvarchar(255)` | YES |  |  |  |  |
| 7 | `sort_id` | `int` | NO |  |  |  |  |
| 8 | `create_time` | `datetime` | NO |  |  |  |  |
| 9 | `creator` | `nvarchar(255)` | NO |  |  |  |  |
| 10 | `creator_id` | `bigint` | NO |  |  |  |  |
| 11 | `update_time` | `datetime` | YES |  |  |  |  |
| 12 | `modifier` | `nvarchar(255)` | YES |  |  |  |  |
| 13 | `modifier_id` | `nvarchar(255)` | YES |  |  |  |  |
| 14 | `language_key` | `nvarchar(255)` | YES |  |  |  |  |
| 15 | `is_deleted` | `bit` | NO |  |  |  |  |
| 16 | `easy_description` | `nvarchar(255)` | YES |  |  |  |  |
| 17 | `is_initialized` | `bit` | NO |  |  |  |  |
| 18 | `is_attribute_method` | `bit` | YES |  |  |  |  |

**Indexes**

| Index | Type | Unique | PK | Columns |
|-------|------|--------|----|---------|
| `PK_zero_user_info_id` | CLUSTERED |  |  | `id` |

