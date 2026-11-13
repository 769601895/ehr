namespace Admin.NET.Application;

/// <summary>人事月报高级查询白名单（P0 仅固有列）</summary>
public static class HrEmpMonthFieldRegistry
{
    public static ISugarQueryable<HrEmpMonthBatch> ApplyBatch(
        ISugarQueryable<HrEmpMonthBatch> query, Filter? filter)
    {
        if (filter is null) return query;
        return ApplyBatchNode(query, filter);
    }

    public static ISugarQueryable<HrEmpMonthLine> ApplyLine(
        ISugarQueryable<HrEmpMonthLine> query, Filter? filter)
    {
        if (filter is null) return query;
        return ApplyLineNode(query, filter);
    }

    private static ISugarQueryable<HrEmpMonthBatch> ApplyBatchNode(
        ISugarQueryable<HrEmpMonthBatch> query, Filter filter)
    {
        var children = filter.Filters?.Where(f => f != null).ToList();
        if (children is { Count: > 0 })
        {
            if (filter.Logic == FilterLogicEnum.Or)
                throw Oops.Oh("批次高级查询暂不支持 Or 组，请使用并且条件");
            foreach (var child in children)
                query = ApplyBatchNode(query, child);
            return query;
        }
        return ApplyBatchLeaf(query, filter);
    }

    private static ISugarQueryable<HrEmpMonthBatch> ApplyBatchLeaf(
        ISugarQueryable<HrEmpMonthBatch> query, Filter filter)
    {
        var key = (filter.Field ?? "").Trim();
        if (string.IsNullOrWhiteSpace(key)) throw Oops.Oh("高级查询条件缺少字段");
        var op = filter.Operator;
        var val = filter.Value?.ToString()?.Trim();

        return key.ToLowerInvariant() switch
        {
            "sitecode" => ApplyString(query, op, val, (q, v) => q.Where(u => u.SiteCode.Contains(v!)),
                (q, v) => q.Where(u => u.SiteCode == v)),
            "payperiod" => ApplyString(query, op, val, (q, v) => q.Where(u => u.PayPeriod.Contains(v!)),
                (q, v) => q.Where(u => u.PayPeriod == v)),
            "status" => ApplyString(query, op, val, (q, v) => q.Where(u => u.Status.Contains(v!)),
                (q, v) => q.Where(u => u.Status == v)),
            "sitename" => ApplyString(query, op, val, (q, v) => q.Where(u => u.SiteName != null && u.SiteName.Contains(v!)),
                (q, v) => q.Where(u => u.SiteName == v)),
            "remark" => ApplyString(query, op, val, (q, v) => q.Where(u => u.Remark != null && u.Remark.Contains(v!)),
                (q, v) => q.Where(u => u.Remark == v)),
            _ => throw Oops.Oh($"不支持的高级查询字段: {key}")
        };
    }

    private static ISugarQueryable<HrEmpMonthLine> ApplyLineNode(
        ISugarQueryable<HrEmpMonthLine> query, Filter filter)
    {
        var children = filter.Filters?.Where(f => f != null).ToList();
        if (children is { Count: > 0 })
        {
            if (filter.Logic == FilterLogicEnum.Or)
                throw Oops.Oh("行高级查询暂不支持 Or 组，请使用并且条件");
            foreach (var child in children)
                query = ApplyLineNode(query, child);
            return query;
        }
        return ApplyLineLeaf(query, filter);
    }

    private static ISugarQueryable<HrEmpMonthLine> ApplyLineLeaf(
        ISugarQueryable<HrEmpMonthLine> query, Filter filter)
    {
        var key = (filter.Field ?? "").Trim();
        if (string.IsNullOrWhiteSpace(key)) throw Oops.Oh("高级查询条件缺少字段");
        var op = filter.Operator;
        var val = filter.Value?.ToString()?.Trim();

        return key.ToLowerInvariant() switch
        {
            "empid" => ApplyString(query, op, val, (q, v) => q.Where(u => u.EmpId.Contains(v!)),
                (q, v) => q.Where(u => u.EmpId == v)),
            "empname" => ApplyString(query, op, val, (q, v) => q.Where(u => u.EmpName != null && u.EmpName.Contains(v!)),
                (q, v) => q.Where(u => u.EmpName == v)),
            "cccode" or "cccodeatend" => ApplyString(query, op, val, (q, v) => q.Where(u => u.CcCode != null && u.CcCode.Contains(v!)),
                (q, v) => q.Where(u => u.CcCode == v)),
            "ccname" => ApplyString(query, op, val, (q, v) => q.Where(u => u.CcName != null && u.CcName.Contains(v!)),
                (q, v) => q.Where(u => u.CcName == v)),
            "includeflag" => ApplyString(query, op, val, (q, v) => q.Where(u => u.IncludeFlag.Contains(v!)),
                (q, v) => q.Where(u => u.IncludeFlag == v)),
            "empsal" => ApplyString(query, op, val, (q, v) => q.Where(u => u.EmpSal != null && u.EmpSal.Contains(v!)),
                (q, v) => q.Where(u => u.EmpSal == v)),
            "postid" => ApplyString(query, op, val, (q, v) => q.Where(u => u.PostId != null && u.PostId.Contains(v!)),
                (q, v) => q.Where(u => u.PostId == v)),
            "haschginperiod" => ApplyBool(query, op, val, trueVal => query.Where(u => u.HasChgInPeriod == trueVal)),
            "issupend" or "issuspend" => ApplyBool(query, op, val, trueVal => query.Where(u => u.IsSuspend == trueVal)),
            "isnewhire" => ApplyBool(query, op, val, trueVal => query.Where(u => u.IsNewHire == trueVal)),
            "isleaver" => ApplyBool(query, op, val, trueVal => query.Where(u => u.IsLeaver == trueVal)),
            "needattendance" => ApplyBool(query, op, val, trueVal => query.Where(u => u.NeedAttendance == trueVal)),
            _ => throw Oops.Oh($"不支持的高级查询字段: {key}")
        };
    }

    private static ISugarQueryable<T> ApplyString<T>(
        ISugarQueryable<T> query,
        FilterOperatorEnum op,
        string? val,
        Func<ISugarQueryable<T>, string, ISugarQueryable<T>> contains,
        Func<ISugarQueryable<T>, string, ISugarQueryable<T>> equal)
    {
        if (string.IsNullOrEmpty(val)) return query;
        return op switch
        {
            FilterOperatorEnum.EQ => equal(query, val!),
            FilterOperatorEnum.Contains => contains(query, val!),
            FilterOperatorEnum.StartsWith => contains(query, val!),
            _ => contains(query, val!)
        };
    }

    private static ISugarQueryable<HrEmpMonthLine> ApplyBool(
        ISugarQueryable<HrEmpMonthLine> query,
        FilterOperatorEnum op,
        string? val,
        Func<bool, ISugarQueryable<HrEmpMonthLine>> apply)
    {
        var b = val is "1" or "true" or "True" or "yes";
        return apply(b);
    }
}
