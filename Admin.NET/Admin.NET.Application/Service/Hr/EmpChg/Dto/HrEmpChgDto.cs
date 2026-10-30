namespace Admin.NET.Application;

/// <summary>异动分页查询</summary>
public class HrEmpChgPageInput : BasePageInput
{
    /// <summary>工号或姓名模糊</summary>
    public new string? Keyword { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>异动类别</summary>
    public string? ChgType { get; set; }

    /// <summary>异动日期起</summary>
    public DateTime? ChgDateBegin { get; set; }

    /// <summary>异动日期止</summary>
    public DateTime? ChgDateEnd { get; set; }
}

/// <summary>异动明细分页/详情行</summary>
public class HrEmpChgLineOutput
{
    public long Id { get; set; }
    public string FieldKey { get; set; }
    public string DbColumn { get; set; }
    public string? Label { get; set; }
    public string? ValueBefore { get; set; }
    public string? ValueAfter { get; set; }
    public string? ValueBeforeText { get; set; }
    public string? ValueAfterText { get; set; }
    public int SortNo { get; set; }
}

/// <summary>异动列表/详情输出</summary>
public class HrEmpChgOutput
{
    public long Id { get; set; }
    public string ChgNo { get; set; }
    public string EmpId { get; set; }
    public string? EmpName { get; set; }
    public DateTime ChgDate { get; set; }
    public string ChgType { get; set; }
    public string? ChgTypeText { get; set; }
    public string? ChgReason { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; }
    public string? StatusText { get; set; }
    public string? OccCode { get; set; }
    public string? NccCode { get; set; }
    public string? OccText { get; set; }
    public string? NccText { get; set; }
    public DateTime? SubmitTime { get; set; }
    public DateTime? ApproveTime { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? EffectiveTime { get; set; }
    public DateTime? VoidTime { get; set; }
    public string? VoidReason { get; set; }
    public DateTime CreateTime { get; set; }
    public string? CreateUserName { get; set; }
    public List<HrEmpChgLineOutput>? Lines { get; set; }
}

/// <summary>明细保存行</summary>
public class HrEmpChgLineInput
{
    /// <summary>字段键 ccCode / postId</summary>
    public string FieldKey { get; set; }

    /// <summary>异动后；部门必填</summary>
    public string? ValueAfter { get; set; }
}

/// <summary>保存草稿</summary>
public class HrEmpChgSaveDraftInput
{
    public long? Id { get; set; }

    /// <summary>工号（必填）</summary>
    public string EmpId { get; set; }

    /// <summary>异动日期（必填）</summary>
    public DateTime ChgDate { get; set; }

    /// <summary>异动类别</summary>
    public string? ChgType { get; set; }

    public string? ChgReason { get; set; }
    public string? Remark { get; set; }

    /// <summary>为 true 时按当前主档重载明细前后默认值</summary>
    public bool ReloadFromEmp { get; set; }

    public List<HrEmpChgLineInput>? Lines { get; set; }
}

public class HrEmpChgIdInput
{
    public long Id { get; set; }
}

public class HrEmpChgRejectInput
{
    public long Id { get; set; }
    public string? RejectReason { get; set; }
}

public class HrEmpChgVoidInput
{
    public long Id { get; set; }
    public string? VoidReason { get; set; }
}

/// <summary>提交结果</summary>
public class HrEmpChgSubmitOutput
{
    /// <summary>未配置启用审批流程时为 true（已自动通过）</summary>
    public bool AutoApproved { get; set; }

    /// <summary>提交后状态</summary>
    public string Status { get; set; }
}
