namespace Admin.NET.Application;

public class HrEmpMonthBatchPageInput : BasePageInput
{
    public string? SiteCode { get; set; }
    public string? PayPeriod { get; set; }
    public string? Status { get; set; }
}

public class HrEmpMonthCreateInput
{
    public string SiteCode { get; set; }
    public string PayPeriod { get; set; }
    public string? Remark { get; set; }
}

public class HrEmpMonthGenerateInput
{
    public long BatchId { get; set; }
    public bool FullReset { get; set; }
}

public class HrEmpMonthFreezeInput
{
    public long BatchId { get; set; }
    /// <summary>Warn 二次确认</summary>
    public bool ConfirmWarn { get; set; }
}

public class HrEmpMonthUnfreezeInput
{
    public long BatchId { get; set; }
    public string Reason { get; set; }
}

public class HrEmpMonthLinePageInput : BasePageInput
{
    public long BatchId { get; set; }
    public string? CcCode { get; set; }
    public string? IncludeFlag { get; set; }
    public bool? HasWarn { get; set; }
}

public class HrEmpMonthAdjustLineInput
{
    public long BatchId { get; set; }
    /// <summary>排除/改备注时必填；手工纳入时可空（新建行）</summary>
    public long? LineId { get; set; }
    public string? EmpId { get; set; }
    /// <summary>Include / Exclude</summary>
    public string IncludeFlag { get; set; }
    public string? ExcludeReason { get; set; }
    public string? IncludeReason { get; set; }
    public string? Remark { get; set; }
}

public class HrEmpMonthPeriodPreviewInput
{
    public string SiteCode { get; set; }
    public string PayPeriod { get; set; }
}

public class HrEmpMonthFrozenQueryInput : BasePageInput
{
    public string SiteCode { get; set; }
    public string PayPeriod { get; set; }
}

public class HrEmpMonthMarkConsumedInput
{
    public string SiteCode { get; set; }
    public string PayPeriod { get; set; }
}

public class HrEmpMonthBatchOutput
{
    public long Id { get; set; }
    public string SiteCode { get; set; }
    public string? SiteName { get; set; }
    public string PayPeriod { get; set; }
    public string AttMonth { get; set; }
    public DateTime AttFrom { get; set; }
    public DateTime AttTo { get; set; }
    public string Status { get; set; }
    public string? StatusText { get; set; }
    public string? ParamSnapshot { get; set; }
    public string? FieldSchemaSnapshot { get; set; }
    public int FreezeVersion { get; set; }
    public DateTime? FrozenTime { get; set; }
    public long? FrozenUserId { get; set; }
    public string? UnfreezeReason { get; set; }
    public DateTime? ConsumedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreateTime { get; set; }
    public int? LineCount { get; set; }
}

public class HrEmpMonthPeriodPreviewOutput
{
    public string PayPeriod { get; set; }
    public string AttMonth { get; set; }
    public DateTime AttFrom { get; set; }
    public DateTime AttTo { get; set; }
    public int AttCloseDay { get; set; }
    public int PayrollAttMonth { get; set; }
}

public class HrEmpMonthLineOutput
{
    public long Id { get; set; }
    public long BatchId { get; set; }
    public string EmpId { get; set; }
    public long IEmpId { get; set; }
    public string? EmpName { get; set; }
    public string IncludeFlag { get; set; }
    public bool ManualLock { get; set; }
    public int? EmpState { get; set; }
    public string? EmpSal { get; set; }
    public string? CcCode { get; set; }
    public string? CcName { get; set; }
    public string? PostId { get; set; }
    public string? DutyId { get; set; }
    public DateTime? EmpInDate { get; set; }
    public DateTime? EmpLeaDate { get; set; }
    public DateTime? EmpKqIndate { get; set; }
    public DateTime? EmpKqLeaDate { get; set; }
    public bool NeedAttendance { get; set; }
    public bool IsNewHire { get; set; }
    public bool IsLeaver { get; set; }
    public bool IsSuspend { get; set; }
    public bool HasChgInPeriod { get; set; }
    public string? LeaveSettleCode { get; set; }
    public string? LeavePayCmdStatus { get; set; }
    public long? PayCmdId { get; set; }
    public string? CcCodeAtStart { get; set; }
    public string? CcCodeAtEnd { get; set; }
    public string? ChgSummary { get; set; }
    public string? ChgFieldSnapshot { get; set; }
    public string? CostCcCode { get; set; }
    public string? ExcludeReason { get; set; }
    public string? IncludeReason { get; set; }
    public string? Remark { get; set; }
    public string? EmpFieldSnapshot { get; set; }
    public string? WarnCodes { get; set; }
    public string? ErrorCodes { get; set; }
    public Dictionary<string, string?>? EmpSnapMap { get; set; }
}

public class HrEmpMonthValidateOutput
{
    public int ErrorCount { get; set; }
    public int WarnCount { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warns { get; set; } = [];
    public bool CanFreeze { get; set; }
}

public class MonthSnapFieldOutput
{
    public string FieldKey { get; set; }
    public string? DbColumn { get; set; }
    public string Label { get; set; }
    public string? DataType { get; set; }
    public bool InCoreColumns { get; set; }
}
