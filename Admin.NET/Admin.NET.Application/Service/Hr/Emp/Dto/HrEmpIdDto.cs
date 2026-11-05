namespace Admin.NET.Application;

public class HrEmpIdPreviewInput
{
    /// <summary>任职部门；空则尝试管辖同属兜底</summary>
    public string? CcCode { get; set; }
}

public class HrEmpIdPreviewOutput
{
    public string SiteCode { get; set; }
    public int EmpIdMode { get; set; }
    public int EmpIdLength { get; set; }
    public string? EmpIdFixedPrefix { get; set; }
    /// <summary>是否手工维护工号</summary>
    public bool Manual { get; set; }
    /// <summary>预览工号（不占号；手工模式为空）</summary>
    public string? EmpId { get; set; }
}
