namespace Admin.NET.Application;

/// <summary>离职/复职模块常量</summary>
public static class HrEmpLeaveConst
{
    public const string LeaveNoPrefix = "LV";
    public const string RestoreNoPrefix = "FZ";
    /// <summary>离职纠错审计单号前缀（由离职撤销写入 restore 表）</summary>
    public const string CorrectNoPrefix = "LR";

    /// <summary>同工号复职单</summary>
    public const string KindReinstate = "Reinstate";
    /// <summary>离职纠错审计（非复职业务）</summary>
    public const string KindCorrect = "Correct";

    public const string CodeLeaveType = "LeaveType";
    public const string CodeLeaveReason = "LeaveReason";
    public const string CodeSalarySettle = "LeaveSalarySettle";
    public const string DefaultSalarySettle = "NormalPay";

    public const string WfBizType = "hrEmpLeave";

    public const string StatusDraft = "Draft";
    public const string StatusInReview = "InReview";
    public const string StatusApproved = "Approved";
    public const string StatusEffective = "Effective";
    public const string StatusRejected = "Rejected";
    public const string StatusVoided = "Voided";

    /// <summary>在途拦截（不含已生效；已生效靠人员状态约束）</summary>
    public static readonly string[] OpenStatuses =
    [
        StatusDraft, StatusInReview, StatusApproved
    ];

    public const string PayCmdActive = "Active";
    public const string PayCmdConsumed = "Consumed";
    public const string PayCmdVoided = "Voided";

    public const int EmpStateActive = 1;
    public const int EmpStateSuspend = 2;
    public const int EmpStateLeave = 3;
}
