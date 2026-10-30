namespace Admin.NET.Application;

public class HrWfStartInput
{
    [Required]
    [MaxLength(50)]
    public string BizType { get; set; }

    /// <summary>表单数据（字典；标量/嵌套对象均可，兼容 Newtonsoft 绑定）</summary>
    [Required]
    public Dictionary<string, object?>? FormData { get; set; }

    /// <summary>
    /// 单据部门内码（可选）。为空时按当事人工号所属部门自动解析。
    /// </summary>
    [MaxLength(64)]
    public string? SubjectCcCode { get; set; }

    [MaxLength(64)]
    public string? BizId { get; set; }

    /// <summary>当事人（单据主体）工号；用于汇报路径与单据部门自动解析</summary>
    [Required(ErrorMessage = "当事人工号不能为空")]
    [MaxLength(50)]
    public string SubjectEmpId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    public long? ApplicantUserId { get; set; }
}

public class HrWfStartOutput
{
    public long InstId { get; set; }
    public string BizId { get; set; }
    public string Status { get; set; }
    public string? CurrentNodeKey { get; set; }
}

public class HrWfApproveInput
{
    [Required]
    public long TaskId { get; set; }

    [MaxLength(500)]
    public string? Opinion { get; set; }

    public Dictionary<string, object?>? FormDataPatch { get; set; }
}

public class HrWfRejectInput
{
    [Required]
    public long TaskId { get; set; }

    [Required(ErrorMessage = "驳回意见必填")]
    [MaxLength(500)]
    public string Opinion { get; set; }

    public Dictionary<string, object?>? FormDataPatch { get; set; }
}

public class HrWfPatchFormInput
{
    [Required]
    public long InstId { get; set; }

    [Required]
    public Dictionary<string, object?> FormDataPatch { get; set; }
}

public class HrWfInstDetailOutput
{
    public long InstId { get; set; }
    public string BizType { get; set; }
    public string BizId { get; set; }
    public string Status { get; set; }
    public string? Title { get; set; }
    public long ApplicantUserId { get; set; }
    public string SubjectCcCode { get; set; }
    public string? CurrentNodeKey { get; set; }
    public DateTime StartedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public bool AllowEditWhileRunning { get; set; }
    public bool EditByApplicant { get; set; }
    public bool EditByAssignee { get; set; }
    public HrWfSchemaSnapshot? FormSchema { get; set; }
    public Dictionary<string, object?>? FormData { get; set; }
    public HrWfNodesSnapshot? DefNodes { get; set; }
    public List<HrWfTaskItemOutput> Tasks { get; set; } = [];
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanEditForm { get; set; }
    public long? MyPendingTaskId { get; set; }
}

public class HrWfTaskItemOutput
{
    public long TaskId { get; set; }
    public string NodeKey { get; set; }
    public long AssigneeId { get; set; }
    public string Status { get; set; }
    public string? Opinion { get; set; }
    public DateTime? FinishedTime { get; set; }
    public bool FromEscalate { get; set; }
}

public class HrWfTodoPageInput : BasePageInput
{
    public string? Keyword { get; set; }
    public string? BizType { get; set; }
}

public class HrWfTodoOutput
{
    public long InstId { get; set; }
    public long? TaskId { get; set; }
    public string BizType { get; set; }
    public string BizId { get; set; }
    public string? Title { get; set; }
    public string Status { get; set; }
    public string? CurrentNodeKey { get; set; }
    public string? NodeKey { get; set; }
    public DateTime StartedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
}

public class HrWfAssigneeRule
{
    public string Type { get; set; }
    public long? LevelId { get; set; }
    public List<long>? UserIds { get; set; }
    /// <summary>人事岗位编码（RulePost），与 HrEmployee.PostId 一致</summary>
    public string? PostId { get; set; }
}

public class HrWfSchemaSnapshot
{
    public long FormId { get; set; }
    public string BizType { get; set; }
    public string FormMode { get; set; }
    public int Version { get; set; }
    public List<HrWfFormFieldItem> Fields { get; set; } = [];
}

public class HrWfNodesSnapshot
{
    public long DefId { get; set; }
    public int Version { get; set; }
    public bool AllowEditWhileRunning { get; set; }
    public bool EditByApplicant { get; set; }
    public bool EditByAssignee { get; set; }
    /// <summary>LegacyLevel | ReportPath；缺省按 LegacyLevel 兼容在途</summary>
    public string? AssigneeResolveMode { get; set; }
    /// <summary>汇报路径角色 Id（父→…→根），仅 ReportPath</summary>
    public List<long>? ReportPathRoleIds { get; set; }
    public List<HrWfDefNodeItem> Nodes { get; set; } = [];
}
