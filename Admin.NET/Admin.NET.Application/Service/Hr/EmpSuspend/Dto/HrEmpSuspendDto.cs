namespace Admin.NET.Application;

public class HrEmpSuspendPageInput : BasePageInput
{
    public new string? Keyword { get; set; }
    public string? Status { get; set; }
    public List<string>? StatusList { get; set; }
    public DateTime? SuspendDateBegin { get; set; }
    public DateTime? SuspendDateEnd { get; set; }
}

public class HrEmpSuspendOutput
{
    public long Id { get; set; }
    public string SuspendNo { get; set; }
    public string EmpId { get; set; }
    public string? EmpName { get; set; }
    public DateTime SuspendDate { get; set; }
    public DateTime PlanEffectDate { get; set; }
    public string? SuspendReason { get; set; }
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

public class HrEmpSuspendSaveDraftInput
{
    public long? Id { get; set; }
    public string EmpId { get; set; }
    public DateTime SuspendDate { get; set; }
    public string? SuspendReason { get; set; }
    public string? Remark { get; set; }
}

public class HrEmpSuspendIdInput
{
    public long Id { get; set; }
}

public class HrEmpSuspendRejectInput
{
    public long Id { get; set; }
    public string? RejectReason { get; set; }
}

public class HrEmpSuspendVoidInput
{
    public long Id { get; set; }
    public string? VoidReason { get; set; }
}

public class HrEmpSuspendSubmitOutput
{
    public bool AutoApproved { get; set; }
    public string Status { get; set; }
}
