namespace Admin.NET.Application;

public class HrWfEffectPolicyOutput
{
    public long Id { get; set; }
    public string BizType { get; set; }
    public bool Enabled { get; set; }
    public string BaseDateSource { get; set; }
    public string BaseDateField { get; set; }
    public int DayOffset { get; set; }
    public string ApplyMode { get; set; }
    public string TargetEntity { get; set; }
    public string? Remark { get; set; }
}

public class HrWfEffectPolicySaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "业务类型不能为空")]
    [MaxLength(50)]
    public string BizType { get; set; }

    public bool Enabled { get; set; } = true;

    [MaxLength(20)]
    public string BaseDateSource { get; set; } = HrWfConsts.EffectBaseFromForm;

    [Required(ErrorMessage = "基准日字段不能为空")]
    [MaxLength(64)]
    public string BaseDateField { get; set; }

    public int DayOffset { get; set; }

    [MaxLength(20)]
    public string ApplyMode { get; set; } = HrWfConsts.EffectModeImmediateIfDue;

    [MaxLength(50)]
    public string TargetEntity { get; set; } = "HrEmployee";

    [MaxLength(200)]
    public string? Remark { get; set; }
}

public class HrWfEffectFieldMapOutput
{
    public long Id { get; set; }
    public string BizType { get; set; }
    public string FormFieldKey { get; set; }
    public string Role { get; set; }
    public string TargetEntity { get; set; }
    public string? DbColumn { get; set; }
    public string? PairGroup { get; set; }
    public bool Enabled { get; set; }
    public int SortNo { get; set; }
    public string? Remark { get; set; }
}

public class HrWfEffectFieldMapSaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "业务类型不能为空")]
    [MaxLength(50)]
    public string BizType { get; set; }

    [Required(ErrorMessage = "表单字段键不能为空")]
    [MaxLength(64)]
    public string FormFieldKey { get; set; }

    [Required(ErrorMessage = "角色不能为空")]
    [MaxLength(20)]
    public string Role { get; set; }

    [MaxLength(50)]
    public string TargetEntity { get; set; } = "HrEmployee";

    [MaxLength(64)]
    public string? DbColumn { get; set; }

    [MaxLength(64)]
    public string? PairGroup { get; set; }

    public bool Enabled { get; set; } = true;
    public int SortNo { get; set; } = 100;

    [MaxLength(200)]
    public string? Remark { get; set; }
}

public class HrWfEffectApplyResult
{
    public bool Applied { get; set; }
    public string? Message { get; set; }
    public DateTime? PlanEffectDate { get; set; }
    public int? HistId { get; set; }
}

public class HrWfEffectVoidInput
{
    [Required]
    [MaxLength(50)]
    public string BizType { get; set; }

    [Required]
    [MaxLength(64)]
    public string BizId { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }
}

/// <summary>待生效 payload</summary>
public class HrWfEffectPayload
{
    public Dictionary<string, string?> Before { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string?> After { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>表单原始键值（写 h_emh_hist 用）</summary>
    public Dictionary<string, string?> Form { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int? HistId { get; set; }
}
