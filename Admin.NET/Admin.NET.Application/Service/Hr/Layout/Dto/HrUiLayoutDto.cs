namespace Admin.NET.Application;

public class HrUiLayoutQueryInput
{
    [Required(ErrorMessage = "模块码不能为空")]
    public string ModuleCode { get; set; } = "hrEmp";

    /// <summary>list / form</summary>
    [Required(ErrorMessage = "场景不能为空")]
    public string Scene { get; set; }
}

public class HrUiLayoutFieldOutput
{
    public long Id { get; set; }
    public string FieldKey { get; set; }
    public string? DbColumn { get; set; }
    public string Label { get; set; }
    public int SortNo { get; set; }
    public bool Visible { get; set; }
    public int? WidthPx { get; set; }
    public int? SpanSlots { get; set; }
    public string? GroupCode { get; set; }
    public string ControlType { get; set; }
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public bool Sortable { get; set; }
    /// <summary>字段池中不可删除（如工号/姓名）</summary>
    public bool Locked { get; set; }
}

public class HrUiLayoutOutput
{
    public long Id { get; set; }
    public string ModuleCode { get; set; }
    public string Scene { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public long? OwnerUserId { get; set; }
    public int Version { get; set; }
    /// <summary>system / user / builtin</summary>
    public string Source { get; set; }
    public List<HrUiLayoutFieldOutput> Fields { get; set; } = [];
}

public class HrUiLayoutFieldInput
{
    [Required]
    public string FieldKey { get; set; }
    public string? DbColumn { get; set; }
    [Required]
    public string Label { get; set; }
    public int SortNo { get; set; }
    public bool Visible { get; set; } = true;
    public int? WidthPx { get; set; }
    public int? SpanSlots { get; set; }
    public string? GroupCode { get; set; }
    public string ControlType { get; set; } = "text";
    public bool Required { get; set; }
    public bool Readonly { get; set; }
}

public class HrUiLayoutSaveInput
{
    [Required]
    public string ModuleCode { get; set; } = "hrEmp";

    [Required]
    public string Scene { get; set; }

    public string? Name { get; set; }

    [Required]
    public List<HrUiLayoutFieldInput> Fields { get; set; } = [];
}

public class HrUiFieldPoolItemOutput
{
    public string FieldKey { get; set; }
    public string? DbColumn { get; set; }
    public string Label { get; set; }
    public string ControlType { get; set; }
    public string? GroupCode { get; set; }
    public int DefaultSpanSlots { get; set; } = 1;
    public int? DefaultWidthPx { get; set; }
    public bool Sortable { get; set; } = true;
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public bool Locked { get; set; }
    public string TableName { get; set; } = "h_emp_mstr";
}
