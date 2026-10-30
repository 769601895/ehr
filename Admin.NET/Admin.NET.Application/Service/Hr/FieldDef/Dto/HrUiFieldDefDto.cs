namespace Admin.NET.Application;

public class HrUiFieldDefPageInput : BasePageInput
{
    [Required(ErrorMessage = "模块码不能为空")]
    public string ModuleCode { get; set; }

    public bool? Enabled { get; set; }
    public bool? IsInPool { get; set; }
    public string ControlType { get; set; }
}

public class HrUiFieldDefOutput
{
    public long Id { get; set; }
    public string ModuleCode { get; set; }
    public string FieldKey { get; set; }
    public string Origin { get; set; }
    public string DbColumn { get; set; }
    public string TableName { get; set; }
    public string Label { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public string ControlType { get; set; }
    public string CodeField { get; set; }
    public string? LookupSource { get; set; }
    public int? DefaultWidthPx { get; set; }
    public int DefaultSpanSlots { get; set; }
    public string GroupCode { get; set; }
    public bool IsInPool { get; set; }
    public bool Sortable { get; set; }
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public bool Locked { get; set; }
    public int SortNo { get; set; }
    public bool Enabled { get; set; }
}

public class HrUiFieldDefUpdateInput
{
    [Required]
    public long Id { get; set; }

    [Required(ErrorMessage = "显示名不能为空")]
    [MaxLength(256)]
    public string Label { get; set; }

    public string DataType { get; set; }
    public string ControlType { get; set; }
    public string CodeField { get; set; }
    public string? LookupSource { get; set; }
    public string Origin { get; set; }
    public int? DefaultWidthPx { get; set; }
    public int DefaultSpanSlots { get; set; } = 1;
    public string GroupCode { get; set; }
    public bool IsInPool { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public bool Locked { get; set; }
    public int SortNo { get; set; }
    public bool Enabled { get; set; } = true;
    public bool IsNullable { get; set; } = true;
    public string DbColumn { get; set; }
    public string TableName { get; set; }
}

public class HrUiFieldSyncInput
{
    [Required]
    public string ModuleCode { get; set; }
}

public class HrUiFieldSyncOutput
{
    public int Inserted { get; set; }
    public int StructuralUpdated { get; set; }
    public int Disabled { get; set; }
    public int Reenabled { get; set; }
    public List<string> Warnings { get; set; } = [];
}

public class HrUiLangItemInput
{
    public string Culture { get; set; }
    public string Label { get; set; }
}

public class HrUiFieldLangSaveInput
{
    [Required]
    public string ModuleCode { get; set; }

    [Required]
    public string FieldKey { get; set; }

    public List<HrUiLangItemInput> Labels { get; set; } = [];
}

public class HrUiFieldLangOutput
{
    public string Culture { get; set; }
    public string Label { get; set; }
}
