namespace Admin.NET.Application;

public sealed class HrUiSyncRow
{
    public long Id { get; set; }
    public string FieldKey { get; set; }
    public string Origin { get; set; }
    public string DbColumn { get; set; }
    public bool Enabled { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public string TableName { get; set; }
}

public sealed class HrUiSyncColumn
{
    public string DbColumn { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public string Description { get; set; }
}

public sealed class HrUiSyncInsert
{
    public string FieldKey { get; set; }
    public string DbColumn { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public string TableName { get; set; }
    public string Label { get; set; }
    public string ControlType { get; set; }
}

public sealed class HrUiSyncStructural
{
    public long Id { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
    public string TableName { get; set; }
}

public sealed class HrUiSyncPlan
{
    public List<HrUiSyncInsert> Inserts { get; } = [];
    public List<HrUiSyncStructural> Structural { get; } = [];
    public List<long> DisableIds { get; } = [];
    public List<long> ReenableIds { get; } = [];
    public List<string> Warnings { get; } = [];
}

/// <summary>
/// 只处理 origin=physical 且有库列的行。计算列、关联列以及空库列完全不碰。
/// 已有行的 FieldKey、显示名、控件、代码绑定不在计划里。
/// </summary>
public static class HrUiFieldSyncPlanner
{
    public static HrUiSyncPlan Plan(string tableName, IReadOnlyList<HrUiSyncRow> existing, IReadOnlyList<HrUiSyncColumn> columns)
    {
        var plan = new HrUiSyncPlan();
        var colMap = new Dictionary<string, HrUiSyncColumn>(StringComparer.OrdinalIgnoreCase);
        foreach (var col in columns ?? [])
        {
            if (string.IsNullOrWhiteSpace(col?.DbColumn)) continue;
            colMap[col.DbColumn.Trim()] = col;
        }

        var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in existing ?? [])
        {
            if (!string.IsNullOrWhiteSpace(row.FieldKey))
                usedKeys.Add(row.FieldKey);
        }

        var matched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in existing ?? [])
        {
            if (!string.Equals(row.Origin, "physical", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(row.DbColumn))
                continue;

            if (!colMap.TryGetValue(row.DbColumn.Trim(), out var col))
            {
                if (row.Enabled) plan.DisableIds.Add(row.Id);
                continue;
            }

            matched.Add(row.DbColumn.Trim());
            var dataType = col.DataType ?? "";
            var changed = !string.Equals(row.DataType ?? "", dataType, StringComparison.OrdinalIgnoreCase)
                || row.IsNullable != col.IsNullable
                || !string.Equals(row.TableName ?? "", tableName ?? "", StringComparison.OrdinalIgnoreCase);
            if (changed)
            {
                plan.Structural.Add(new HrUiSyncStructural
                {
                    Id = row.Id,
                    DataType = dataType,
                    IsNullable = col.IsNullable,
                    TableName = tableName
                });
            }
            if (!row.Enabled) plan.ReenableIds.Add(row.Id);
        }

        foreach (var col in colMap.Values)
        {
            if (matched.Contains(col.DbColumn.Trim())) continue;
            var key = HrUiFieldKeyUtil.Unique(HrUiFieldKeyUtil.FromColumn(col.DbColumn), usedKeys);
            usedKeys.Add(key);
            var label = string.IsNullOrWhiteSpace(col.Description) ? key : col.Description.Trim();
            if (label.Length > 256) label = label[..256];
            plan.Inserts.Add(new HrUiSyncInsert
            {
                FieldKey = key,
                DbColumn = col.DbColumn.Trim(),
                DataType = col.DataType,
                IsNullable = col.IsNullable,
                TableName = tableName,
                Label = label,
                ControlType = HrUiControlGuess.FromDataType(col.DataType)
            });
            plan.Warnings.Add($"已插入 {key}（{col.DbColumn.Trim()}），显示名与控件只在插入时默认");
        }

        return plan;
    }
}
