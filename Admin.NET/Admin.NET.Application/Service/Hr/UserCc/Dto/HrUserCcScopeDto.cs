namespace Admin.NET.Application;

/// <summary>解析后的有效管辖根（含当前层级码）</summary>
public class HrUserCcEffectiveRoot
{
    public string CcCode { get; set; }
    public string CcLevel { get; set; }
    public bool IncludeChild { get; set; }
}

public class HrUserCcSyncResult
{
    public long UserId { get; set; }
    public int SavedCount { get; set; }
    public List<string> SkippedCodes { get; set; } = [];
}

public class HrUserCcSyncAllResult
{
    public int UserCount { get; set; }
    public int RowCount { get; set; }
    public List<string> Warnings { get; set; } = [];
}
