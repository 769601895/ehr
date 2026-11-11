using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Admin.NET.Application;

/// <summary>
/// 高级查询安全应用：白名单字段 + 正确区分条件组/叶子（有子 Filters 才是组）。
/// </summary>
public static class HrAdvancedFilter
{
    public sealed class FieldMap
    {
        /// <summary>实体属性名（PascalCase）；跨表处理器可空</summary>
        public string? PropertyName { get; init; }

        /// <summary>跨表/特殊条件；返回追加后的查询</summary>
        public Func<ISugarQueryable<HrEmployee>, FilterOperatorEnum, object?, ISugarQueryable<HrEmployee>>? ApplyEmp { get; init; }
    }

    /// <summary>人员资料可筛字段（fieldKey camelCase）</summary>
    public static IReadOnlyDictionary<string, FieldMap> HrEmpRegistry { get; } = BuildHrEmpRegistry();

    /// <summary>异动管理可筛字段（主表直映射）</summary>
    public static IReadOnlyDictionary<string, FieldMap> HrEmpChgRegistry { get; } = BuildHrEmpChgRegistry();

    public static ISugarQueryable<HrEmployee> ApplyEmp(ISugarQueryable<HrEmployee> query, Filter? filter)
    {
        if (filter is null) return query;
        return ApplyEmpNode(query, filter, HrEmpRegistry);
    }

    public static ISugarQueryable<HrEmpChg> ApplyEmpChg(ISugarQueryable<HrEmpChg> query, Filter? filter)
    {
        if (filter is null) return query;
        return ApplyEmpChgNode(query, filter, HrEmpChgRegistry);
    }

    private static ISugarQueryable<HrEmpChg> ApplyEmpChgNode(
        ISugarQueryable<HrEmpChg> query,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        var children = filter.Filters?.Where(f => f != null).ToList();
        if (children is { Count: > 0 })
        {
            if (filter.Logic == FilterLogicEnum.Or)
            {
                Expression? orExpr = null;
                var param = Expression.Parameter(typeof(HrEmpChg), "u");
                foreach (var child in children)
                {
                    var leafChildren = child.Filters?.Where(f => f != null).ToList();
                    if (leafChildren is { Count: > 0 })
                        throw Oops.Oh("高级查询 Or 组内暂不支持再嵌套组，请调整条件");
                    var piece = BuildEmpChgLeafExpression(param, child, registry);
                    orExpr = orExpr is null ? piece : Expression.OrElse(orExpr, piece);
                }
                if (orExpr is null) return query;
                var lambda = Expression.Lambda<Func<HrEmpChg, bool>>(orExpr, param);
                return query.Where(lambda);
            }

            foreach (var child in children)
                query = ApplyEmpChgNode(query, child, registry);
            return query;
        }

        return ApplyEmpChgLeaf(query, filter, registry);
    }

    private static ISugarQueryable<HrEmpChg> ApplyEmpChgLeaf(
        ISugarQueryable<HrEmpChg> query,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        if (string.IsNullOrWhiteSpace(filter.Field))
            throw Oops.Oh("高级查询条件缺少字段");

        var key = NormalizeKey(filter.Field);
        if (!registry.TryGetValue(key, out var map) || string.IsNullOrWhiteSpace(map.PropertyName))
            throw Oops.Oh($"高级查询字段不可用或已失效：{filter.Field}");

        var param = Expression.Parameter(typeof(HrEmpChg), "u");
        var body = BuildPropertyCompare(param, map.PropertyName!, filter.Operator, filter.Value);
        var lambda = Expression.Lambda<Func<HrEmpChg, bool>>(body, param);
        return query.Where(lambda);
    }

    private static Expression BuildEmpChgLeafExpression(
        ParameterExpression param,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        if (string.IsNullOrWhiteSpace(filter.Field))
            throw Oops.Oh("高级查询条件缺少字段");

        var key = NormalizeKey(filter.Field);
        if (!registry.TryGetValue(key, out var map) || string.IsNullOrWhiteSpace(map.PropertyName))
            throw Oops.Oh($"高级查询字段不可用或已失效：{filter.Field}");

        return BuildPropertyCompare(param, map.PropertyName!, filter.Operator, filter.Value);
    }

    private static Dictionary<string, FieldMap> BuildHrEmpChgRegistry()
    {
        var dict = new Dictionary<string, FieldMap>(StringComparer.OrdinalIgnoreCase);
        void Map(string key, string prop) => dict[key] = new FieldMap { PropertyName = prop };
        Map("chgNo", nameof(HrEmpChg.ChgNo));
        Map("empId", nameof(HrEmpChg.EmpId));
        Map("empName", nameof(HrEmpChg.EmpName));
        Map("chgDate", nameof(HrEmpChg.ChgDate));
        Map("chgType", nameof(HrEmpChg.ChgType));
        Map("status", nameof(HrEmpChg.Status));
        Map("createTime", nameof(HrEmpChg.CreateTime));
        return dict;
    }

    private static ISugarQueryable<HrEmployee> ApplyEmpNode(
        ISugarQueryable<HrEmployee> query,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        var children = filter.Filters?.Where(f => f != null).ToList();
        if (children is { Count: > 0 })
        {
            // 组：And 可顺序叠加；Or 需合并为一条表达式
            if (filter.Logic == FilterLogicEnum.Or)
            {
                Expression? orExpr = null;
                var param = Expression.Parameter(typeof(HrEmployee), "u");
                foreach (var child in children)
                {
                    var leafChildren = child.Filters?.Where(f => f != null).ToList();
                    if (leafChildren is { Count: > 0 })
                        throw Oops.Oh("高级查询 Or 组内暂不支持再嵌套组，请调整条件");

                    var piece = BuildEmpLeafExpression(param, child, registry);
                    orExpr = orExpr is null ? piece : Expression.OrElse(orExpr, piece);
                }
                if (orExpr is null) return query;
                var lambda = Expression.Lambda<Func<HrEmployee, bool>>(orExpr, param);
                return query.Where(lambda);
            }

            foreach (var child in children)
                query = ApplyEmpNode(query, child, registry);
            return query;
        }

        return ApplyEmpLeaf(query, filter, registry);
    }

    private static ISugarQueryable<HrEmployee> ApplyEmpLeaf(
        ISugarQueryable<HrEmployee> query,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        if (string.IsNullOrWhiteSpace(filter.Field))
            throw Oops.Oh("高级查询条件缺少字段");

        var key = NormalizeKey(filter.Field);
        if (!registry.TryGetValue(key, out var map))
            throw Oops.Oh($"高级查询字段不可用或已失效：{filter.Field}");

        if (map.ApplyEmp != null)
            return map.ApplyEmp(query, filter.Operator, filter.Value);

        if (string.IsNullOrWhiteSpace(map.PropertyName))
            throw Oops.Oh($"高级查询字段未配置：{filter.Field}");

        var param = Expression.Parameter(typeof(HrEmployee), "u");
        var body = BuildPropertyCompare(param, map.PropertyName!, filter.Operator, filter.Value);
        var lambda = Expression.Lambda<Func<HrEmployee, bool>>(body, param);
        return query.Where(lambda);
    }

    private static Expression BuildEmpLeafExpression(
        ParameterExpression param,
        Filter filter,
        IReadOnlyDictionary<string, FieldMap> registry)
    {
        if (string.IsNullOrWhiteSpace(filter.Field))
            throw Oops.Oh("高级查询条件缺少字段");

        var key = NormalizeKey(filter.Field);
        if (!registry.TryGetValue(key, out var map))
            throw Oops.Oh($"高级查询字段不可用或已失效：{filter.Field}");

        if (map.ApplyEmp != null)
            throw Oops.Oh($"跨表字段「{filter.Field}」不能放在 Or 组内，请放到并且条件中");

        if (string.IsNullOrWhiteSpace(map.PropertyName))
            throw Oops.Oh($"高级查询字段未配置：{filter.Field}");

        return BuildPropertyCompare(param, map.PropertyName!, filter.Operator, filter.Value);
    }

    private static Expression BuildPropertyCompare(
        ParameterExpression param,
        string propertyName,
        FilterOperatorEnum op,
        object? value)
    {
        var member = Expression.PropertyOrField(param, propertyName);
        var constant = BuildConstant(value, member.Type);
        return op switch
        {
            FilterOperatorEnum.EQ => Expression.Equal(member, constant),
            FilterOperatorEnum.NEQ => Expression.NotEqual(member, constant),
            FilterOperatorEnum.LT => Expression.LessThan(member, constant),
            FilterOperatorEnum.LTE => Expression.LessThanOrEqual(member, constant),
            FilterOperatorEnum.GT => Expression.GreaterThan(member, constant),
            FilterOperatorEnum.GTE => Expression.GreaterThanOrEqual(member, constant),
            FilterOperatorEnum.Contains => CallString(member, nameof(string.Contains), constant, negate: false),
            FilterOperatorEnum.NotContains => CallString(member, nameof(string.Contains), constant, negate: true),
            FilterOperatorEnum.StartsWith => CallString(member, nameof(string.StartsWith), constant, negate: false),
            FilterOperatorEnum.EndsWith => CallString(member, nameof(string.EndsWith), constant, negate: false),
            _ => throw Oops.Oh("不支持的运算符"),
        };
    }

    private static Expression CallString(MemberExpression member, string method, ConstantExpression constant, bool negate)
    {
        // 非字符串（如 int 状态码）先 ToString，便于下拉字段使用包含/左包含等
        Expression strExpr;
        if (member.Type == typeof(string))
        {
            strExpr = member;
        }
        else
        {
            var underlying = Nullable.GetUnderlyingType(member.Type);
            if (underlying != null)
            {
                var hasValue = Expression.Property(member, "HasValue");
                var toStr = Expression.Call(Expression.Property(member, "Value"), "ToString", Type.EmptyTypes);
                strExpr = Expression.Condition(hasValue, toStr, Expression.Constant(null, typeof(string)));
            }
            else
            {
                strExpr = Expression.Call(member, "ToString", Type.EmptyTypes);
            }
        }

        var isNull = Expression.Equal(strExpr, Expression.Constant(null, typeof(string)));
        var arg = Expression.Convert(constant, typeof(string));
        var call = Expression.Call(strExpr, method, Type.EmptyTypes, arg);
        if (negate)
            return Expression.OrElse(isNull, Expression.Not(call));
        return Expression.AndAlso(Expression.Not(isNull), call);
    }

    private static ConstantExpression BuildConstant(object? value, Type propertyType)
    {
        if (value is null) return Expression.Constant(null, propertyType);

        var text = value is JsonElement je
            ? (je.ValueKind == JsonValueKind.String ? je.GetString() : je.GetRawText().Trim('"'))
            : value.ToString();

        var target = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (target == typeof(string))
            return Expression.Constant(text, propertyType);

        if (target == typeof(DateTime))
        {
            if (!DateTime.TryParse(text, out var dt)) throw Oops.Oh($"日期值无效：{text}");
            object boxed = propertyType == typeof(DateTime?) ? (DateTime?)dt : dt;
            return Expression.Constant(boxed, propertyType);
        }

        if (target == typeof(int))
        {
            if (!int.TryParse(text, out var n)) throw Oops.Oh($"数值无效：{text}");
            object boxed = propertyType == typeof(int?) ? (int?)n : n;
            return Expression.Constant(boxed, propertyType);
        }

        if (target == typeof(long))
        {
            if (!long.TryParse(text, out var n)) throw Oops.Oh($"数值无效：{text}");
            object boxed = propertyType == typeof(long?) ? (long?)n : n;
            return Expression.Constant(boxed, propertyType);
        }

        if (target == typeof(byte))
        {
            if (!byte.TryParse(text, out var n)) throw Oops.Oh($"数值无效：{text}");
            object boxed = propertyType == typeof(byte?) ? (byte?)n : n;
            return Expression.Constant(boxed, propertyType);
        }

        var converted = Convert.ChangeType(text, target);
        return Expression.Constant(converted, propertyType);
    }

    private static string NormalizeKey(string field)
    {
        var f = field.Trim();
        if (f.Length == 0) return f;
        // EmpId / empId → empId
        return char.ToLowerInvariant(f[0]) + f[1..];
    }

    private static Dictionary<string, FieldMap> BuildHrEmpRegistry()
    {
        var dict = new Dictionary<string, FieldMap>(StringComparer.OrdinalIgnoreCase);
        var props = typeof(HrEmployee).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // 主表 physical：凡实体有同名属性均可筛（与字段池对齐；未在池中的也不妨，白名单仍是实体属性）
        foreach (var name in props)
        {
            // 主键内部字段一般不开放
            if (name is nameof(HrEmployee.IEmpId) or nameof(HrEmployee.BsId) or nameof(HrEmployee.EmpPic))
                continue;
            var key = char.ToLowerInvariant(name[0]) + name[1..];
            dict[key] = new FieldMap { PropertyName = name };
        }

        // —— 存编码、界面显示名称：一律按「名称或编码」筛 ——
        dict["ccDesc"] = new FieldMap { ApplyEmp = ApplyCcDesc };
        dict["postId"] = new FieldMap { ApplyEmp = ApplyPost };
        dict["empProvince"] = new FieldMap { ApplyEmp = ApplyProvince };
        dict["empCity"] = new FieldMap { ApplyEmp = ApplyCity };
        dict["empState"] = new FieldMap { ApplyEmp = ApplyEmpState };

        // code_mstr：性别/户籍/国籍/婚姻/人员类别/薪酬类别/扩展下拉等
        dict["empSex"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_sex", nameof(HrEmployee.EmpSex)) };
        dict["empRgt"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_rgt", nameof(HrEmployee.EmpRgt)) };
        dict["empCountry"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_country", nameof(HrEmployee.EmpCountry)) };
        dict["empMarry"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_marry", nameof(HrEmployee.EmpMarry)) };
        dict["empType"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_type", nameof(HrEmployee.EmpType)) };
        dict["empSal"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_sal", nameof(HrEmployee.EmpSal)) };
        dict["empChar1"] = new FieldMap { ApplyEmp = (q, op, val) => ApplyCodeMstr(q, op, val, "emp_char1", nameof(HrEmployee.EmpChar1)) };

        return dict;
    }

    private static void EnsureTextOp(FilterOperatorEnum op, string fieldLabel)
    {
        if (op is FilterOperatorEnum.EQ or FilterOperatorEnum.NEQ
            or FilterOperatorEnum.Contains or FilterOperatorEnum.NotContains
            or FilterOperatorEnum.StartsWith or FilterOperatorEnum.EndsWith)
            return;
        throw Oops.Oh($"{fieldLabel}仅支持等于/不等于/包含类运算符");
    }

    private static ISugarQueryable<HrEmployee> ApplyCcDesc(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val)
    {
        EnsureTextOp(op, "部门名称");
        var text = val?.ToString()?.Trim() ?? "";
        return op switch
        {
            FilterOperatorEnum.EQ => q.Where(u => SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc == text).Any()),
            FilterOperatorEnum.NEQ => q.Where(u => !SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc == text).Any()),
            FilterOperatorEnum.Contains => q.Where(u => SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc!.Contains(text)).Any()),
            FilterOperatorEnum.NotContains => q.Where(u => !SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc!.Contains(text)).Any()),
            FilterOperatorEnum.StartsWith => q.Where(u => SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc!.StartsWith(text)).Any()),
            FilterOperatorEnum.EndsWith => q.Where(u => SqlFunc.Subqueryable<CcMstr>()
                .Where(c => c.CcCode == u.CcCode && c.CcDesc!.EndsWith(text)).Any()),
            _ => q
        };
    }

    private static ISugarQueryable<HrEmployee> ApplyPost(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val)
    {
        EnsureTextOp(op, "岗位");
        var text = val?.ToString()?.Trim() ?? "";
        return op switch
        {
            FilterOperatorEnum.EQ => q.Where(u => SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName == text || p.PostCode == text)).Any()),
            FilterOperatorEnum.NEQ => q.Where(u => !SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName == text || p.PostCode == text)).Any()),
            FilterOperatorEnum.Contains => q.Where(u => SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName!.Contains(text) || p.PostCode.Contains(text))).Any()),
            FilterOperatorEnum.NotContains => q.Where(u => u.PostId == null || !SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName!.Contains(text) || p.PostCode.Contains(text))).Any()),
            FilterOperatorEnum.StartsWith => q.Where(u => SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName!.StartsWith(text) || p.PostCode.StartsWith(text))).Any()),
            FilterOperatorEnum.EndsWith => q.Where(u => SqlFunc.Subqueryable<HrPost>()
                .Where(p => p.PostCode == u.PostId && (p.PostName!.EndsWith(text) || p.PostCode.EndsWith(text))).Any()),
            _ => q
        };
    }

    private static ISugarQueryable<HrEmployee> ApplyProvince(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val)
    {
        EnsureTextOp(op, "籍贯省");
        var text = val?.ToString()?.Trim() ?? "";
        return op switch
        {
            FilterOperatorEnum.EQ => q.Where(u => SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName == text || p.ProvinceCode == text)).Any()),
            FilterOperatorEnum.NEQ => q.Where(u => !SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName == text || p.ProvinceCode == text)).Any()),
            FilterOperatorEnum.Contains => q.Where(u => SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName!.Contains(text) || p.ProvinceCode.Contains(text))).Any()),
            FilterOperatorEnum.NotContains => q.Where(u => u.EmpProvince == null || !SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName!.Contains(text) || p.ProvinceCode.Contains(text))).Any()),
            FilterOperatorEnum.StartsWith => q.Where(u => SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName!.StartsWith(text) || p.ProvinceCode.StartsWith(text))).Any()),
            FilterOperatorEnum.EndsWith => q.Where(u => SqlFunc.Subqueryable<HrProvince>()
                .Where(p => p.ProvinceCode == u.EmpProvince && (p.ProvinceName!.EndsWith(text) || p.ProvinceCode.EndsWith(text))).Any()),
            _ => q
        };
    }

    private static ISugarQueryable<HrEmployee> ApplyCity(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val)
    {
        EnsureTextOp(op, "籍贯市");
        var text = val?.ToString()?.Trim() ?? "";
        return op switch
        {
            FilterOperatorEnum.EQ => q.Where(u => SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName == text || p.CityCode == text)).Any()),
            FilterOperatorEnum.NEQ => q.Where(u => !SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName == text || p.CityCode == text)).Any()),
            FilterOperatorEnum.Contains => q.Where(u => SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName!.Contains(text) || p.CityCode.Contains(text))).Any()),
            FilterOperatorEnum.NotContains => q.Where(u => u.EmpCity == null || !SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName!.Contains(text) || p.CityCode.Contains(text))).Any()),
            FilterOperatorEnum.StartsWith => q.Where(u => SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName!.StartsWith(text) || p.CityCode.StartsWith(text))).Any()),
            FilterOperatorEnum.EndsWith => q.Where(u => SqlFunc.Subqueryable<HrCity>()
                .Where(p => p.CityCode == u.EmpCity && (p.CityName!.EndsWith(text) || p.CityCode.EndsWith(text))).Any()),
            _ => q
        };
    }

    /// <summary>code_mstr：按描述或码值匹配（可输「中国」或「CN」）</summary>
    private static ISugarQueryable<HrEmployee> ApplyCodeMstr(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val, string codeField, string empProp)
    {
        EnsureTextOp(op, codeField);
        var text = val?.ToString()?.Trim() ?? "";
        // 各属性显式写 Where，保证 SqlSugar 可翻译（禁止 Compile）
        return empProp switch
        {
            nameof(HrEmployee.EmpSex) => ApplyCodeOnSex(q, op, text, codeField),
            nameof(HrEmployee.EmpRgt) => ApplyCodeOnRgt(q, op, text, codeField),
            nameof(HrEmployee.EmpCountry) => ApplyCodeOnCountry(q, op, text, codeField),
            nameof(HrEmployee.EmpMarry) => ApplyCodeOnMarry(q, op, text, codeField),
            nameof(HrEmployee.EmpType) => ApplyCodeOnType(q, op, text, codeField),
            nameof(HrEmployee.EmpSal) => ApplyCodeOnSal(q, op, text, codeField),
            nameof(HrEmployee.EmpChar1) => ApplyCodeOnChar1(q, op, text, codeField),
            _ => throw Oops.Oh($"未登记代码字段：{empProp}")
        };
    }

    private static ISugarQueryable<HrEmployee> ApplyCodeOnSex(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpSex == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpSex != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpSex != null && u.EmpSex.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpSex == null || !u.EmpSex.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpSex != null && u.EmpSex.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpSex != null && u.EmpSex.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSex && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnRgt(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpRgt == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpRgt != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpRgt != null && u.EmpRgt.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpRgt == null || !u.EmpRgt.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpRgt != null && u.EmpRgt.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpRgt != null && u.EmpRgt.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpRgt && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnCountry(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpCountry == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpCountry != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpCountry != null && u.EmpCountry.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpCountry == null || !u.EmpCountry.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpCountry != null && u.EmpCountry.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpCountry != null && u.EmpCountry.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpCountry && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnMarry(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpMarry == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpMarry != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpMarry != null && u.EmpMarry.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpMarry == null || !u.EmpMarry.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpMarry != null && u.EmpMarry.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpMarry != null && u.EmpMarry.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpMarry && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnType(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpType == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpType != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpType != null && u.EmpType.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpType == null || !u.EmpType.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpType != null && u.EmpType.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpType != null && u.EmpType.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpType && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnSal(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpSal == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpSal != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpSal != null && u.EmpSal.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpSal == null || !u.EmpSal.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpSal != null && u.EmpSal.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpSal != null && u.EmpSal.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpSal && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyCodeOnChar1(ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, string text, string cf) => op switch
    {
        FilterOperatorEnum.EQ => q.Where(u => u.EmpChar1 == text || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.NEQ => q.Where(u => u.EmpChar1 != text && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc == text || c.CodeValue == text)).Any()),
        FilterOperatorEnum.Contains => q.Where(u => (u.EmpChar1 != null && u.EmpChar1.Contains(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.NotContains => q.Where(u => (u.EmpChar1 == null || !u.EmpChar1.Contains(text)) && !SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc!.Contains(text) || c.CodeValue.Contains(text))).Any()),
        FilterOperatorEnum.StartsWith => q.Where(u => (u.EmpChar1 != null && u.EmpChar1.StartsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc!.StartsWith(text) || c.CodeValue.StartsWith(text))).Any()),
        FilterOperatorEnum.EndsWith => q.Where(u => (u.EmpChar1 != null && u.EmpChar1.EndsWith(text)) || SqlFunc.Subqueryable<CodeMstr>().Where(c => c.CodeField == cf && c.CodeValue == u.EmpChar1 && (c.CodeDesc!.EndsWith(text) || c.CodeValue.EndsWith(text))).Any()),
        _ => q
    };

    private static ISugarQueryable<HrEmployee> ApplyEmpState(
        ISugarQueryable<HrEmployee> q, FilterOperatorEnum op, object? val)
    {
        EnsureTextOp(op, "人员状态");
        var text = val?.ToString()?.Trim() ?? "";
        var map = new (int Code, string Label)[] { (1, "在职"), (2, "停职"), (3, "离职") };
        bool Hit((int Code, string Label) x) => op switch
        {
            FilterOperatorEnum.EQ or FilterOperatorEnum.NEQ => x.Label == text || x.Code.ToString() == text,
            FilterOperatorEnum.Contains or FilterOperatorEnum.NotContains => x.Label.Contains(text) || x.Code.ToString().Contains(text),
            FilterOperatorEnum.StartsWith => x.Label.StartsWith(text) || x.Code.ToString().StartsWith(text),
            FilterOperatorEnum.EndsWith => x.Label.EndsWith(text) || x.Code.ToString().EndsWith(text),
            _ => false
        };
        var codes = map.Where(Hit).Select(x => (int?)x.Code).ToList();
        if (op is FilterOperatorEnum.NEQ or FilterOperatorEnum.NotContains)
        {
            if (codes.Count == 0) return q;
            return q.Where(u => u.EmpState == null || !codes.Contains(u.EmpState));
        }
        if (codes.Count == 0) return q.Where(u => false);
        return q.Where(u => u.EmpState != null && codes.Contains(u.EmpState));
    }
}
