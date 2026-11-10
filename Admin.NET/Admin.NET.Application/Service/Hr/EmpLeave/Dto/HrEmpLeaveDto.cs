namespace Admin.NET.Application;

public class HrEmpLeavePageInput : BasePageInput
{
    public new string? Keyword { get; set; }
    public string? Status { get; set; }
    /// <summary>多状态过滤（页签：在途 / 生效 / 作废失效）；与 Status 同时传时优先 StatusList</summary>
    public List<string>? StatusList { get; set; }
    public string? LeaveType { get; set; }
    public DateTime? LeaveDateBegin { get; set; }
    public DateTime? LeaveDateEnd { get; set; }
}

public class HrEmpLeaveOutput
{
    public long Id { get; set; }
    public string LeaveNo { get; set; }
    public string EmpId { get; set; }
    public string? EmpName { get; set; }
    public DateTime LeaveDate { get; set; }
    public DateTime PlanEffectDate { get; set; }
    public string LeaveType { get; set; }
    public string? LeaveTypeText { get; set; }
    public string LeaveReason { get; set; }
    public string? LeaveReasonText { get; set; }
    public string SalarySettle { get; set; }
    public string? SalarySettleText { get; set; }
    public bool IsBlacklist { get; set; }
    public string? CcCodeSnap { get; set; }
    public string? CcDescSnap { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; }
    public string? StatusText { get; set; }
    public DateTime? SubmitTime { get; set; }
    public DateTime? ApproveTime { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? EffectiveTime { get; set; }
    public DateTime? VoidTime { get; set; }
    public string? VoidReason { get; set; }
    public DateTime CreateTime { get; set; }
    public string? CreateUserName { get; set; }
}

public class HrEmpLeaveSaveDraftInput
{
    public long? Id { get; set; }
    public string EmpId { get; set; }
    public DateTime LeaveDate { get; set; }
    public string LeaveType { get; set; }
    public string LeaveReason { get; set; }
    public string? SalarySettle { get; set; }
    public bool IsBlacklist { get; set; }
    public string? Remark { get; set; }
}

public class HrEmpLeaveIdInput
{
    public long Id { get; set; }
}

public class HrEmpLeaveRejectInput
{
    public long Id { get; set; }
    public string? RejectReason { get; set; }
}

public class HrEmpLeaveVoidInput
{
    public long Id { get; set; }
    public string? VoidReason { get; set; }
}

public class HrEmpLeaveSubmitOutput
{
    public bool AutoApproved { get; set; }
    public string Status { get; set; }
}

public class HrEmpLeaveRestorePageInput : BasePageInput
{
    public new string? Keyword { get; set; }
    public string? Status { get; set; }
    public List<string>? StatusList { get; set; }
    /// <summary>默认仅 Reinstate；传 Correct 可查纠错审计</summary>
    public string? Kind { get; set; }
    public DateTime? RestoreDateBegin { get; set; }
    public DateTime? RestoreDateEnd { get; set; }
}

public class HrEmpLeaveRestoreOutput
{
    public long Id { get; set; }
    public string RestoreNo { get; set; }
    public string? Kind { get; set; }
    public long? LeaveId { get; set; }
    public string? LeaveNo { get; set; }
    public string EmpId { get; set; }
    public string? EmpName { get; set; }
    public DateTime RestoreDate { get; set; }
    public DateTime PlanEffectDate { get; set; }
    public bool ClearBlacklist { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; }
    public string? StatusText { get; set; }
    public DateTime? SubmitTime { get; set; }
    public DateTime? ApproveTime { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? EffectiveTime { get; set; }
    public DateTime? VoidTime { get; set; }
    public string? VoidReason { get; set; }
    public long? OperateUserId { get; set; }
    public string? OperateUserName { get; set; }
    public DateTime? OperateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public string? CreateUserName { get; set; }
}

public class HrEmpLeaveRestoreSaveDraftInput
{
    public long? Id { get; set; }
    public string EmpId { get; set; }
    public DateTime RestoreDate { get; set; }
    public bool ClearBlacklist { get; set; }
    public string? Remark { get; set; }
}
