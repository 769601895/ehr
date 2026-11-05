namespace Admin.NET.Application;

public class HrEmpImportTplPageInput : BasePageInput
{
    /// <summary>Add / Update；空=全部</summary>
    public string? ImportMode { get; set; }
}

public class HrEmpImportTplFieldInput
{
    [Required]
    public string FieldKey { get; set; }

    public int SortNo { get; set; } = 100;
}

public class HrEmpImportTplSaveInput
{
    /// <summary>可空；按 ImportMode 对本账号唯一模板做 upsert</summary>
    public long? Id { get; set; }

    [Required]
    public string ImportMode { get; set; }

    /// <summary>已废弃：每账号每作业类型固定一份模板，名称由服务端生成</summary>
    public string? Name { get; set; }

    public string? Remark { get; set; }

    [Required]
    public List<HrEmpImportTplFieldInput> Fields { get; set; } = [];
}

public class HrEmpImportTplDeleteInput
{
    [Required]
    public long Id { get; set; }
}

public class HrEmpImportTplFieldOutput
{
    public string FieldKey { get; set; }
    public string? Label { get; set; }
    public int SortNo { get; set; }
    public bool IsLocked { get; set; }
    public bool Required { get; set; }
}

public class HrEmpImportTplOutput
{
    public long Id { get; set; }
    public string ImportMode { get; set; }
    public string Name { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public List<HrEmpImportTplFieldOutput> Fields { get; set; } = [];
}

public class HrEmpImportRowErrorOutput
{
    public int RowNo { get; set; }
    public string? EmpId { get; set; }
    public string ErrorMessage { get; set; }
}

public class HrEmpImportHeaderMismatchOutput
{
    /// <summary>模板期望表头（按顺序）</summary>
    public List<string> Expected { get; set; } = [];
    /// <summary>文件实际表头（按顺序）</summary>
    public List<string> Actual { get; set; } = [];
    /// <summary>模板有、文件缺</summary>
    public List<string> Missing { get; set; } = [];
    /// <summary>文件有、模板无</summary>
    public List<string> Extra { get; set; } = [];
    /// <summary>同列位置顺序不一致说明，如「第2列：期望「姓名」实际「工号」」</summary>
    public List<string> OrderDiffs { get; set; } = [];
}

public class HrEmpImportPreviewColumnOutput
{
    public string FieldKey { get; set; }
    public string Label { get; set; }
}

public class HrEmpImportPreviewRowOutput
{
    public int RowNo { get; set; }
    /// <summary>按模板字段：fieldKey → 单元格原文</summary>
    public Dictionary<string, string?> Cells { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>该行全部异常，多条以「；」拼接；无异常为空</summary>
    public string? ErrorMessage { get; set; }
}

public class HrEmpImportPreviewOutput
{
    public int TotalCount { get; set; }
    public bool Ok { get; set; }
    public List<HrEmpImportRowErrorOutput> Errors { get; set; } = [];
    /// <summary>表头不一致明细（有值时优先展示）</summary>
    public HrEmpImportHeaderMismatchOutput? HeaderMismatch { get; set; }
    /// <summary>预览列（模板字段顺序）</summary>
    public List<HrEmpImportPreviewColumnOutput> Columns { get; set; } = [];
    /// <summary>全部数据行（含异常列）</summary>
    public List<HrEmpImportPreviewRowOutput> Rows { get; set; } = [];
}

public class HrEmpImportCommitOutput
{
    public long BatchId { get; set; }
    public string BatchNo { get; set; }
    public string Status { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<HrEmpImportRowErrorOutput> Errors { get; set; } = [];
}

public class HrEmpImportBatchPageInput : BasePageInput
{
    public string? ImportMode { get; set; }
    public string? Status { get; set; }
}

public class HrEmpImportBatchOutput
{
    public long Id { get; set; }
    public string BatchNo { get; set; }
    public string ImportMode { get; set; }
    public long? TplId { get; set; }
    public string? TplNameSnapshot { get; set; }
    public string? FileName { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public string Status { get; set; }
    public string? ErrorSummary { get; set; }
    public DateTime? CreateTime { get; set; }
}

public class HrEmpImportFailRowsInput : BasePageInput
{
    [Required]
    public long BatchId { get; set; }
}
