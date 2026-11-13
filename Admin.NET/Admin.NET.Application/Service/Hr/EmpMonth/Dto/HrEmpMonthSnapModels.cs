namespace Admin.NET.Application;

public sealed class MonthSnapFieldMeta
{
    public string FieldKey { get; set; } = "";
    public string? DbColumn { get; set; }
    public string Label { get; set; } = "";
    public string? DataType { get; set; }
    public bool InCoreColumns { get; set; }
}

public sealed class EmpFieldSnapshotDto
{
    public int SchemaVersion { get; set; } = 1;
    public List<EmpFieldSnapshotItemDto> Fields { get; set; } = [];
}

public sealed class EmpFieldSnapshotItemDto
{
    public string FieldKey { get; set; } = "";
    public string? DbColumn { get; set; }
    public string Label { get; set; } = "";
    public string? DataType { get; set; }
    public string? Value { get; set; }
}

public sealed class ChgFieldSnapshotDto
{
    public int SchemaVersion { get; set; } = 1;
    public List<ChgFieldSnapshotItemDto> Items { get; set; } = [];
}

public sealed class ChgFieldSnapshotItemDto
{
    public string FieldKey { get; set; } = "";
    public string? DbColumn { get; set; }
    public string Label { get; set; } = "";
    public string? ValueAtStart { get; set; }
    public string? ValueAtEnd { get; set; }
    public List<ChgFieldChangeDto> Changes { get; set; } = [];
}

public sealed class ChgFieldChangeDto
{
    public string ChgNo { get; set; } = "";
    public string ChgDate { get; set; } = "";
    public string? ValueBefore { get; set; }
    public string? ValueAfter { get; set; }
}

public sealed class FieldSchemaSnapshotDto
{
    public int SchemaVersion { get; set; } = 1;
    public List<string> MonthSnapFieldKeys { get; set; } = [];
    public List<string> ChgEnabledFieldKeys { get; set; } = [];
    public DateTime CapturedAt { get; set; }
}
