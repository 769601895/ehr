namespace Admin.NET.Application;

public class HrPostPageInput : BasePageInput
{
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public string? CcCode { get; set; }
    public bool? NeedAudit { get; set; }
}

public class HrPostOutput
{
    public long Id { get; set; }
    public string PostCode { get; set; }
    public string PostName { get; set; }
    public int Grade { get; set; }
    public string Status { get; set; }
    public string? CcCode { get; set; }
    public string? CcName { get; set; }
    public bool NeedAudit { get; set; }
    public string? Remark { get; set; }
}

public class HrPostSaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "岗位代码不能为空")]
    [MaxLength(50)]
    public string PostCode { get; set; }

    [Required(ErrorMessage = "岗位名称不能为空")]
    [MaxLength(100)]
    public string PostName { get; set; }

    public int Grade { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = HrPostConsts.StatusActive;

    [MaxLength(64)]
    public string? CcCode { get; set; }

    public bool NeedAudit { get; set; } = true;

    [MaxLength(256)]
    public string? Remark { get; set; }
}

public class HrPostDeleteInput
{
    [Required]
    public long Id { get; set; }
}
