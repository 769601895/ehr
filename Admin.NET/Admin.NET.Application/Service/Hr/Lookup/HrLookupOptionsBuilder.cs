namespace Admin.NET.Application;

/// <summary>根据数据源定义拼主表/译文查询（标识符已白名单校验）。</summary>
public static class HrLookupOptionsBuilder
{
    public sealed record MainQuery(string Sql, List<SugarParameter> Parameters);

    public sealed record LangQuery(string Sql, List<SugarParameter> Parameters);

    public static MainQuery BuildMainQuery(HrLookupDef def)
    {
        var table = HrLookupSqlGuard.Quote(def.TableName);
        var valueCol = HrLookupSqlGuard.Quote(def.ValueColumn);
        var labelCol = HrLookupSqlGuard.Quote(def.LabelColumn);
        var orderCol = string.IsNullOrWhiteSpace(def.OrderColumn)
            ? valueCol
            : HrLookupSqlGuard.Quote(def.OrderColumn);

        var where = new List<string>();
        var ps = new List<SugarParameter>();

        if (def.FilterUseIsDelete)
            where.Add($"{HrLookupSqlGuard.Quote("IsDelete")}=0");

        if (!string.IsNullOrWhiteSpace(def.FilterEnabledColumn))
        {
            if (string.IsNullOrWhiteSpace(def.FilterEnabledValue))
                throw Oops.Oh("启用列与启用值须成对配置");
            var col = HrLookupSqlGuard.Quote(def.FilterEnabledColumn);
            where.Add($"{col}=@enabledValue");
            ps.Add(new SugarParameter("@enabledValue", def.FilterEnabledValue.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(def.FilterEqualColumn))
        {
            var col = HrLookupSqlGuard.Quote(def.FilterEqualColumn);
            where.Add($"{col}=@equalValue");
            ps.Add(new SugarParameter("@equalValue", def.FilterEqualValue?.Trim() ?? ""));
        }

        var sql = $"SELECT {valueCol} AS [Value], {labelCol} AS [Label] FROM {table}";
        if (where.Count > 0)
            sql += " WHERE " + string.Join(" AND ", where);
        sql += $" ORDER BY {orderCol}";
        return new MainQuery(sql, ps);
    }

    public static LangQuery? BuildLangQuery(HrLookupDef def, string culture, IReadOnlyList<string> values)
    {
        if (string.IsNullOrWhiteSpace(def.LangTable) || values.Count == 0)
            return null;

        var table = HrLookupSqlGuard.Quote(def.LangTable);
        var valueColName = string.IsNullOrWhiteSpace(def.LangValueColumn) ? def.ValueColumn : def.LangValueColumn!;
        var cultureColName = string.IsNullOrWhiteSpace(def.LangCultureColumn) ? "culture" : def.LangCultureColumn!;
        var labelColName = string.IsNullOrWhiteSpace(def.LangLabelColumn) ? def.LabelColumn : def.LangLabelColumn!;

        var valueCol = HrLookupSqlGuard.Quote(valueColName);
        var cultureCol = HrLookupSqlGuard.Quote(cultureColName);
        var labelCol = HrLookupSqlGuard.Quote(labelColName);

        var ps = new List<SugarParameter> { new("@culture", culture) };
        var inParts = new List<string>();
        for (var i = 0; i < values.Count; i++)
        {
            var p = "@v" + i;
            inParts.Add(p);
            ps.Add(new SugarParameter(p, values[i]));
        }

        var sql = $"SELECT {valueCol} AS [Value], {labelCol} AS [Label] FROM {table} WHERE {cultureCol}=@culture AND {valueCol} IN ({string.Join(",", inParts)})";
        return new LangQuery(sql, ps);
    }
}
