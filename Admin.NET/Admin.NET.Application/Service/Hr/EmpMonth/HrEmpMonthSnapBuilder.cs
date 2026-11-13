using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Admin.NET.Application;

/// <summary>人事月报动态快照装配（§7.7 / §7.8 / §18）</summary>
public class HrEmpMonthSnapBuilder : ITransient
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly ConcurrentDictionary<string, PropertyInfo?> EmpPropByDbColumn = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Regex SafeIdent = new(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

    private readonly SqlSugarRepository<HrUiFieldDef> _defRep;
    private readonly SqlSugarRepository<HrUiFieldScene> _sceneRep;
    private readonly SqlSugarRepository<HrEmpChg> _chgRep;
    private readonly SqlSugarRepository<HrEmpChgLine> _chgLineRep;
    private readonly SqlSugarRepository<HrEmpChgField> _chgFieldRep;

    public HrEmpMonthSnapBuilder(
        SqlSugarRepository<HrUiFieldDef> defRep,
        SqlSugarRepository<HrUiFieldScene> sceneRep,
        SqlSugarRepository<HrEmpChg> chgRep,
        SqlSugarRepository<HrEmpChgLine> chgLineRep,
        SqlSugarRepository<HrEmpChgField> chgFieldRep)
    {
        _defRep = defRep;
        _sceneRep = sceneRep;
        _chgRep = chgRep;
        _chgLineRep = chgLineRep;
        _chgFieldRep = chgFieldRep;
    }

    public async Task<MonthSnapFieldMeta[]> LoadMonthSnapFieldsAsync()
    {
        var module = HrUiModuleCatalog.HrEmp;
        var keys = await _sceneRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code && u.Scene == HrEmpMonthConst.MonthSnapScene)
            .Select(u => u.FieldKey)
            .ToListAsync();
        if (keys.Count == 0) return [];

        var defs = await _defRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code && u.Enabled && u.IsInPool && keys.Contains(u.FieldKey))
            .OrderBy(u => u.SortNo)
            .ToListAsync();

        return defs
            .Where(d => string.Equals(d.Origin, "physical", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(d.DbColumn)
                        && SafeIdent.IsMatch(d.DbColumn.Trim()))
            .Select(d => new MonthSnapFieldMeta
            {
                FieldKey = d.FieldKey,
                DbColumn = d.DbColumn?.Trim(),
                Label = d.Label,
                DataType = d.DataType,
                InCoreColumns = HrEmpMonthConst.CoreFieldKeys.Contains(d.FieldKey)
            })
            .ToArray();
    }

    public async Task<string[]> LoadChgEnabledFieldKeysAsync()
    {
        var enabled = await _chgFieldRep.AsQueryable()
            .Where(u => u.Enabled)
            .OrderBy(u => u.SortNo)
            .Select(u => u.FieldKey)
            .ToListAsync();
        if (enabled.Count == 0) return [];

        var module = HrUiModuleCatalog.HrEmp.Code;
        var valid = await _defRep.AsQueryable()
            .Where(u => u.ModuleCode == module && u.Enabled && u.IsInPool && enabled.Contains(u.FieldKey)
                        && u.Origin == "physical" && u.DbColumn != null && u.DbColumn != "")
            .Select(u => u.FieldKey)
            .ToListAsync();
        return valid.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    /// <summary>组装 EmpFieldSnapshot；跳过固有去重集；无映射记 warn。</summary>
    public string BuildEmpFieldSnapshot(HrEmployee emp, MonthSnapFieldMeta[] fields, ICollection<string> warns)
    {
        var snapFields = fields.Where(f => !f.InCoreColumns).ToList();
        if (fields.Length == 0)
            warns.Add(HrEmpMonthConst.WarnMonthSnapEmpty);

        var dto = new EmpFieldSnapshotDto { SchemaVersion = 1 };
        foreach (var f in snapFields)
        {
            var prop = ResolveEmpProperty(f.DbColumn);
            if (prop == null)
            {
                warns.Add(HrEmpMonthConst.WarnFieldNoMap);
                continue;
            }
            var raw = prop.GetValue(emp);
            dto.Fields.Add(new EmpFieldSnapshotItemDto
            {
                FieldKey = f.FieldKey,
                DbColumn = f.DbColumn,
                Label = f.Label,
                DataType = f.DataType,
                Value = FormatValue(raw)
            });
        }
        return JsonSerializer.Serialize(dto, JsonOpts);
    }

    public async Task<(string Json, string? Summary, bool HasChg, List<string> Warns)> BuildChgFieldSnapshotAsync(
        string empId,
        HrEmployee emp,
        DateTime attFrom,
        DateTime attTo,
        IReadOnlyCollection<string> chgEnabledKeys,
        IReadOnlyCollection<string> monthSnapKeys)
    {
        var warns = new List<string>();
        var from = attFrom.Date;
        var to = attTo.Date;

        var bills = await _chgRep.AsQueryable()
            .Where(u => u.EmpId == empId
                        && u.Status == HrEmpChgConst.StatusEffective
                        && u.ChgDate >= from && u.ChgDate <= to)
            .OrderBy(u => u.ChgDate)
            .OrderBy(u => u.ChgNo)
            .ToListAsync();

        var enabled = new HashSet<string>(chgEnabledKeys, StringComparer.OrdinalIgnoreCase);
        var dto = new ChgFieldSnapshotDto { SchemaVersion = 1 };
        if (bills.Count == 0 || enabled.Count == 0)
            return (JsonSerializer.Serialize(dto, JsonOpts), null, false, warns);

        var billIds = bills.Select(b => b.Id).ToList();
        var lines = await _chgLineRep.AsQueryable()
            .Where(u => billIds.Contains(u.ChgId) && enabled.Contains(u.FieldKey))
            .ToListAsync();

        var billMap = bills.ToDictionary(b => b.Id);
        var byKey = lines
            .Where(l => !NormEq(l.ValueBefore, l.ValueAfter))
            .GroupBy(l => l.FieldKey, StringComparer.OrdinalIgnoreCase);

        foreach (var g in byKey)
        {
            var ordered = g
                .Select(l => (Line: l, Bill: billMap[l.ChgId]))
                .OrderBy(x => x.Bill.ChgDate)
                .ThenBy(x => x.Bill.ChgNo)
                .ToList();
            if (ordered.Count == 0) continue;

            var first = ordered[0];
            var last = ordered[^1];
            var item = new ChgFieldSnapshotItemDto
            {
                FieldKey = g.Key,
                DbColumn = first.Line.DbColumn,
                Label = first.Line.Label ?? g.Key,
                ValueAtStart = first.Line.ValueBefore,
                ValueAtEnd = last.Line.ValueAfter,
                Changes = ordered.Select(x => new ChgFieldChangeDto
                {
                    ChgNo = x.Bill.ChgNo,
                    ChgDate = x.Bill.ChgDate.ToString("yyyy-MM-dd"),
                    ValueBefore = x.Line.ValueBefore,
                    ValueAfter = x.Line.ValueAfter
                }).ToList()
            };

            // 与主档期末值核对（能映射时）
            var prop = ResolveEmpProperty(item.DbColumn);
            if (prop != null)
            {
                var master = FormatValue(prop.GetValue(emp));
                if (!NormEq(master, item.ValueAtEnd))
                    warns.Add(HrEmpMonthConst.WarnChgSnapMismatch);
            }

            if (!monthSnapKeys.Contains(g.Key, StringComparer.OrdinalIgnoreCase)
                && !HrEmpMonthConst.CoreFieldKeys.Contains(g.Key))
                warns.Add(HrEmpMonthConst.WarnChgNotInMonthSnap);

            dto.Items.Add(item);
        }

        var hasChg = dto.Items.Count > 0;
        var summary = hasChg
            ? string.Join("；", dto.Items.Select(i =>
            {
                var last = i.Changes.LastOrDefault();
                return $"{i.Label}:{last?.ValueBefore}→{last?.ValueAfter}";
            }))
            : null;
        if (summary is { Length: > 500 })
            summary = summary[..497] + "...";

        return (JsonSerializer.Serialize(dto, JsonOpts), summary, hasChg, warns);
    }

    public static string BuildFieldSchemaSnapshot(IEnumerable<string> monthSnapKeys, IEnumerable<string> chgKeys)
    {
        var dto = new FieldSchemaSnapshotDto
        {
            SchemaVersion = 1,
            MonthSnapFieldKeys = monthSnapKeys.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            ChgEnabledFieldKeys = chgKeys.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            CapturedAt = DateTime.Now
        };
        return JsonSerializer.Serialize(dto, JsonOpts);
    }

    public static string? ResolveCcCodeAtStart(ChgFieldSnapshotDto? snap, string? currentCc)
    {
        var cc = snap?.Items?.FirstOrDefault(i => i.FieldKey.Equals("ccCode", StringComparison.OrdinalIgnoreCase));
        if (cc != null && !string.IsNullOrWhiteSpace(cc.ValueAtStart))
            return cc.ValueAtStart;
        return currentCc;
    }

    public static ChgFieldSnapshotDto? TryParseChg(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<ChgFieldSnapshotDto>(json, JsonOpts); }
        catch { return null; }
    }

    private static PropertyInfo? ResolveEmpProperty(string? dbColumn)
    {
        if (string.IsNullOrWhiteSpace(dbColumn) || !SafeIdent.IsMatch(dbColumn.Trim()))
            return null;
        return EmpPropByDbColumn.GetOrAdd(dbColumn.Trim(), col =>
        {
            foreach (var p in typeof(HrEmployee).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var sugar = p.GetCustomAttribute<SugarColumn>();
                if (sugar != null && !string.IsNullOrWhiteSpace(sugar.ColumnName)
                    && sugar.ColumnName.Equals(col, StringComparison.OrdinalIgnoreCase))
                    return p;
            }
            return null;
        });
    }

    private static string? FormatValue(object? raw)
    {
        if (raw == null) return null;
        return raw switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd"),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd"),
            bool b => b ? "1" : "0",
            _ => Convert.ToString(raw)
        };
    }

    private static bool NormEq(string? a, string? b)
    {
        var x = string.IsNullOrWhiteSpace(a) ? "" : a.Trim();
        var y = string.IsNullOrWhiteSpace(b) ? "" : b.Trim();
        return string.Equals(x, y, StringComparison.Ordinal);
    }
}
