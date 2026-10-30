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
    public string CodeField { get; set; }
    public string? LookupSource { get; set; }
    public string Origin { get; set; }
    public string DataType { get; set; }
    /// <summary>池中不存在、未启用，或不在该场景适用表</summary>
    public bool IsStale { get; set; }
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
    /// <summary>按语言稀疏覆盖。空文案删除该语言行。</summary>
    public List<HrUiLayoutLabelInput> Labels { get; set; }
}

public class HrUiLayoutLabelInput
{
    public string Culture { get; set; }
    public string Label { get; set; }
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
    public string CodeField { get; set; }
    public string? LookupSource { get; set; }
    public string Origin { get; set; }
    public string DataType { get; set; }
    /// <summary>池主语言，不随当前文化变化。布局主表 label 写这个值。</summary>
    public string MainLabel { get; set; }
}
