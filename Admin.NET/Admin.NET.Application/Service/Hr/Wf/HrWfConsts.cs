namespace Admin.NET.Application;

/// <summary>审批引擎常量</summary>
public static class HrWfConsts
{
    public const string BizTypeGeneral = "hrWfGeneral";

    /// <summary>业务类型人事代码类 code_mstr.code_field</summary>
    public const string BizTypeCodeField = "hr_wf_biz";

    public const string FormModeB = "B";
    public const string FormModeA = "A";
    public const string FieldSourceCustom = "Custom";

    public const string InstRunning = "Running";
    public const string InstApproved = "Approved";
    public const string InstRejected = "Rejected";
    public const string InstCancelled = "Cancelled";

    public const string TaskPending = "Pending";
    public const string TaskApproved = "Approved";
    public const string TaskRejected = "Rejected";
    public const string TaskCancelled = "Cancelled";

    public const string NodeOr = "Or";
    public const string NodeAnd = "And";
    public const string NodeSerial = "Serial";

    public const string RuleLevel = "Level";
    public const string RuleFixedUser = "FixedUser";
    public const string RulePost = "Post";
    public const string RuleDeptManager = "DeptManager";
    public const string RuleReporter = "Reporter";

    /// <summary>在途旧单：等级全库扫描 + AuditOrder 升级</summary>
    public const string ModeLegacyLevel = "LegacyLevel";
    /// <summary>新开单：按汇报路径解析 Level</summary>
    public const string ModeReportPath = "ReportPath";

    public const string ActionStart = "Start";
    public const string ActionApprove = "Approve";
    public const string ActionReject = "Reject";
    public const string ActionEscalate = "Escalate";
    public const string ActionSkipEmpty = "SkipEmpty";
    public const string ActionSkipCondition = "SkipCondition";
    public const string ActionSkipNotOnPath = "SkipNotOnPath";
    public const string ActionSkipScope = "SkipScope";
    public const string ActionPatchForm = "PatchForm";
}
