namespace Admin.NET.Application;

public class HrWfFormPageInput : BasePageInput
{
    public string? BizType { get; set; }
    public string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}

public class HrWfFormFieldItem
{
    public long? Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FieldKey { get; set; }

    [Required]
    [MaxLength(100)]
    public string Label { get; set; }

    [MaxLength(20)]
    public string ControlType { get; set; } = "text";

    [MaxLength(20)]
    public string FieldSource { get; set; } = HrWfConsts.FieldSourceCustom;

    [MaxLength(50)]
    public string? PoolFieldKey { get; set; }

    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public int SortNo { get; set; }

    [MaxLength(20)]
    public string? GroupCode { get; set; }

    public int? SpanSlots { get; set; }

    [MaxLength(50)]
    public string? CodeField { get; set; }

    [MaxLength(256)]
    public string? DefaultValue { get; set; }

    [MaxLength(100)]
    public string? Placeholder { get; set; }

    public string? ExtraJson { get; set; }
}

public class HrWfFormOutput
{
    public long Id { get; set; }
    public string BizType { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string FormMode { get; set; }
    public int Version { get; set; }
    public bool Enabled { get; set; }
    public string? Remark { get; set; }
    public List<HrWfFormFieldItem> Fields { get; set; } = [];
}

public class HrWfFormSaveInput
{
    public long? Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string BizType { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(10)]
    public string FormMode { get; set; } = HrWfConsts.FormModeB;

    public bool Enabled { get; set; } = true;
    [MaxLength(256)]
    public string? Remark { get; set; }

    public List<HrWfFormFieldItem> Fields { get; set; } = [];
}

public class HrWfFormDeleteInput
{
    [Required]
    public long Id { get; set; }
}
