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
    public bool IsDelete { get; set; }
    public DateTime? DeleteTime { get; set; }
    public string DeleteUserName { get; set; }
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

public class HrCodeFieldOutput
{
    public string CodeField { get; set; }
    public string FieldName { get; set; }
    public string UsageDesc { get; set; }
    public int OrderNo { get; set; }
    public bool IsDelete { get; set; }
    public DateTime? DeleteTime { get; set; }
    public string DeleteUserName { get; set; }
}

public class HrCodeFieldSaveInput
{
    /// <summary>true=新建；false=仅改名称/说明/排序（编码不可改）</summary>
    public bool IsNew { get; set; }

    [Required]
    public string CodeField { get; set; }

    [Required]
    public string FieldName { get; set; }

    public string UsageDesc { get; set; }
    public int OrderNo { get; set; } = 100;

    /// <summary>新建/保存时可一并提交多语（不含主语言 zh-CN 亦可）</summary>
    public List<HrCodeFieldLangItemInput> Langs { get; set; } = [];
}

public class HrCodeFieldDeleteInput
{
    [Required]
    public string CodeField { get; set; }
}

public class HrCodeFieldLangItemInput
{
    public string Culture { get; set; }
    public string FieldName { get; set; }
    public string UsageDesc { get; set; }
}

public class HrCodeFieldLangOutput
{
    public string Culture { get; set; }
    public string FieldName { get; set; }
    public string UsageDesc { get; set; }
}

public class HrCodeFieldLangSaveInput
{
    [Required]
    public string CodeField { get; set; }

    public List<HrCodeFieldLangItemInput> Langs { get; set; } = [];
}
