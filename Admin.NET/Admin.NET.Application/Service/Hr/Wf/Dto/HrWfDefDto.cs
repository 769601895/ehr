namespace Admin.NET.Application;

public class HrWfDefPageInput : BasePageInput
{
    public string? BizType { get; set; }
    public string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}

public class HrWfDefNodeItem
{
    public long? Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string NodeKey { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public int OrderNo { get; set; }

    [MaxLength(20)]
    public string NodeMode { get; set; } = HrWfConsts.NodeOr;

    /// <summary>审批人规则 JSON 或对象序列化后字符串</summary>
    [Required]
    public string AssigneeRuleJson { get; set; }

    public string? ConditionJson { get; set; }
    public int? TimeoutHours { get; set; }
    public bool Enabled { get; set; } = true;
}

public class HrWfDefOutput
{
    public long Id { get; set; }
    public string BizType { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public long FormId { get; set; }
    public string? FormName { get; set; }
    public int Version { get; set; }
    public bool Enabled { get; set; }
    public bool AllowEditWhileRunning { get; set; }
    public bool EditByApplicant { get; set; }
    public bool EditByAssignee { get; set; }
    public int DefaultTimeoutHours { get; set; }
    public string? Remark { get; set; }
    public List<HrWfDefNodeItem> Nodes { get; set; } = [];
}

public class HrWfDefSaveInput
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

    [Required]
    public long FormId { get; set; }

    public bool Enabled { get; set; } = true;
    public bool AllowEditWhileRunning { get; set; }
    public bool EditByApplicant { get; set; }
    public bool EditByAssignee { get; set; }
    public int DefaultTimeoutHours { get; set; }
    [MaxLength(256)]
    public string? Remark { get; set; }

    public List<HrWfDefNodeItem> Nodes { get; set; } = [];
}

public class HrWfDefDeleteInput
{
    [Required]
    public long Id { get; set; }
}
