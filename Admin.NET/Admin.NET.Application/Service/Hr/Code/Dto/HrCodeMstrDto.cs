namespace Admin.NET.Application;

public class HrCodeMstrPageInput : BasePageInput
{
    public string CodeField { get; set; }
}

public class HrCodeMstrOutput
{
    public string CodeField { get; set; }
    public string CodeValue { get; set; }
    public string CodeDesc { get; set; }
    public string CodeSort { get; set; }
}

public class HrCodeMstrSaveInput
{
    [Required]
    public string CodeField { get; set; }

    [Required]
    public string CodeValue { get; set; }

    /// <summary>修改码值时传旧值，同一事务搬走译文。</summary>
    public string OldCodeValue { get; set; }

    public string CodeDesc { get; set; }
    public string CodeSort { get; set; }
}

public class HrCodeMstrDeleteInput
{
    [Required]
    public string CodeField { get; set; }

    [Required]
    public string CodeValue { get; set; }
}

public class HrCodeLangSaveInput
{
    [Required]
    public string CodeField { get; set; }

    [Required]
    public string CodeValue { get; set; }

    public List<HrUiLangItemInput> Labels { get; set; } = [];
}

public class HrCodeLangOutput
{
    public string Culture { get; set; }
    public string Label { get; set; }
}
