namespace Admin.NET.Application;

/// <summary>人事月报常量</summary>
public static class HrEmpMonthConst
{
    public const string ModuleCode = "hrEmpMonth";
    public const string MonthSnapScene = "monthSnap";

    public const string StatusDraft = "Draft";
    public const string StatusGenerated = "Generated";
    public const string StatusFrozen = "Frozen";
    public const string StatusReopened = "Reopened";
    public const string StatusConsumed = "Consumed";

    public const string IncludeInclude = "Include";
    public const string IncludeExclude = "Exclude";

    public const string WarnChgPending = "CHG_PENDING";
    public const string WarnChgSnapMismatch = "CHG_SNAP_MISMATCH";
    public const string WarnChgNotInMonthSnap = "CHG_NOT_IN_MONTH_SNAP";
    public const string WarnFieldNoMap = "FIELD_NO_MAP";
    public const string WarnMonthSnapEmpty = "MONTH_SNAP_EMPTY";
    public const string WarnHasChgManualCheck = "HAS_CHG_MANUAL_CHECK";

    public const string ErrDupEmpId = "DUP_EMP_ID";
    public const string ErrEmptyEmpId = "EMPTY_EMP_ID";
    public const string ErrScopeResidual = "SCOPE_RESIDUAL";

    /// <summary>固有列 FieldKey，不写入 EmpFieldSnapshot（方案 §9.5）</summary>
    public static readonly HashSet<string> CoreFieldKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "empId", "empName", "empState", "empSal", "ccCode", "postId", "dutyId",
        "empInDate", "empLeaDate", "empKqIndate", "empKqLeaDate", "ccName"
    };

    public static readonly string[] EditableStatuses =
    [
        StatusDraft, StatusGenerated, StatusReopened
    ];

    public static readonly string[] GeneratableStatuses =
    [
        StatusDraft, StatusGenerated, StatusReopened
    ];
}
