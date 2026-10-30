namespace Admin.NET.Application;

public class HrLookupDefPageInput : BasePageInput
{
    public new string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}

public class HrLookupDefOutput
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string TableName { get; set; }
    public string ValueColumn { get; set; }
    public string LabelColumn { get; set; }
    public string? OrderColumn { get; set; }
    public bool FilterUseIsDelete { get; set; }
    public string? FilterEnabledColumn { get; set; }
    public string? FilterEnabledValue { get; set; }
    public string? FilterEqualColumn { get; set; }
    public string? FilterEqualValue { get; set; }
    public string? LangTable { get; set; }
    public string? LangValueColumn { get; set; }
    public string? LangCultureColumn { get; set; }
    public string? LangLabelColumn { get; set; }
    public string? MaintainPath { get; set; }
    public bool Enabled { get; set; }
    public string? Remark { get; set; }
}

public class HrLookupDefSaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "编码不能为空")]
    [MaxLength(64)]
    public string Code { get; set; }

    [Required(ErrorMessage = "名称不能为空")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "表名不能为空")]
    [MaxLength(128)]
    public string TableName { get; set; }

    [Required(ErrorMessage = "值列不能为空")]
    [MaxLength(64)]
    public string ValueColumn { get; set; }

    [Required(ErrorMessage = "显示列不能为空")]
    [MaxLength(64)]
    public string LabelColumn { get; set; }

    [MaxLength(64)]
    public string? OrderColumn { get; set; }

    public bool FilterUseIsDelete { get; set; } = true;

    [MaxLength(64)]
    public string? FilterEnabledColumn { get; set; }

    [MaxLength(64)]
    public string? FilterEnabledValue { get; set; }

    [MaxLength(64)]
    public string? FilterEqualColumn { get; set; }

    [MaxLength(128)]
    public string? FilterEqualValue { get; set; }

    [MaxLength(128)]
    public string? LangTable { get; set; }

    [MaxLength(64)]
    public string? LangValueColumn { get; set; }

    [MaxLength(64)]
    public string? LangCultureColumn { get; set; }

    [MaxLength(64)]
    public string? LangLabelColumn { get; set; }

    [MaxLength(256)]
    public string? MaintainPath { get; set; }

    public bool Enabled { get; set; } = true;

    [MaxLength(256)]
    public string? Remark { get; set; }
}

public class HrLookupDefDeleteInput
{
    [Required]
    public long Id { get; set; }
}

public class HrLookupOptionRow
{
    public string? Value { get; set; }
    public string? Label { get; set; }
}
