namespace Admin.NET.Application;

public class HrWfLevelPageInput : BasePageInput
{
    public string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}

public class HrWfLevelOutput
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int AuditOrder { get; set; }
    public bool IsFunctional { get; set; }
    public int OrderNo { get; set; }
    public bool Enabled { get; set; }
    public string? Remark { get; set; }
}

public class HrWfLevelSaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "编码不能为空")]
    [MaxLength(50)]
    public string Code { get; set; }

    [Required(ErrorMessage = "名称不能为空")]
    [MaxLength(100)]
    public string Name { get; set; }

    public int AuditOrder { get; set; }
    public bool IsFunctional { get; set; }
    public int OrderNo { get; set; } = 100;
    public bool Enabled { get; set; } = true;
    [MaxLength(256)]
    public string? Remark { get; set; }
}

public class HrWfLevelDeleteInput
{
    [Required]
    public long Id { get; set; }
}
