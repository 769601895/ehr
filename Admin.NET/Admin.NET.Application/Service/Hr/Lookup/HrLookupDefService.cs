using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 查找数据源设置（字段池下拉绑定主档表）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrLookupDef", Order = 471, Description = "查找数据源设置")]
public class HrLookupDefService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrLookupDef> _rep;

    public HrLookupDefService(SqlSugarRepository<HrLookupDef> rep)
    {
        _rep = rep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("查找数据源分页")]
    public async Task<SqlSugarPagedList<HrLookupDefOutput>> Page(HrLookupDefPageInput input)
    {
        input ??= new HrLookupDefPageInput();
        var keyword = input.Keyword?.Trim();
        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.Code.Contains(keyword!) || u.Name.Contains(keyword!) || u.TableName.Contains(keyword!))
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled);

        NormalizeSort(input);
        return await query
            .OrderBuilder(input, defaultSortField: nameof(HrLookupDef.Code), descSort: false)
            .Select<HrLookupDefOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "ListEnabled"), HttpGet]
    [DisplayName("启用的查找数据源")]
    public async Task<List<HrLookupDefOutput>> ListEnabled()
    {
        return await _rep.AsQueryable()
            .Where(u => u.Enabled)
            .OrderBy(u => u.Code)
            .Select<HrLookupDefOutput>()
            .ToListAsync();
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("查找数据源详情")]
    public async Task<HrLookupDefOutput> Get([FromQuery] long id)
    {
        var row = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("查找数据源不存在");
        return row.Adapt<HrLookupDefOutput>();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存查找数据源")]
    public async Task<long> Save(HrLookupDefSaveInput input)
    {
        ValidateIdents(input);
        var code = input.Code.Trim();

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("查找数据源不存在");
            if (await _rep.IsAnyAsync(u => u.Code == code && u.Id != entity.Id))
                throw Oops.Oh("数据源编码已存在");
            Apply(entity, input, code);
            await _rep.UpdateAsync(entity);
            return entity.Id;
        }

        if (await _rep.IsAnyAsync(u => u.Code == code))
            throw Oops.Oh("数据源编码已存在");
        var inserted = await _rep.InsertReturnEntityAsync(Apply(new HrLookupDef(), input, code));
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除查找数据源")]
    public async Task Delete(HrLookupDefDeleteInput input)
    {
        if (!await _rep.IsAnyAsync(u => u.Id == input.Id))
            throw Oops.Oh("查找数据源不存在");
        await _rep.FakeDeleteAsync(input.Id);
    }

    /// <summary>
    /// 按数据源编码拉取选项（当前文化 Label）。停用或不存在返回空列表。
    /// </summary>
    [ApiDescriptionSettings(Name = "GetOptions"), HttpGet]
    [DisplayName("查找数据源选项")]
    public async Task<List<HrCodeItemOutput>> GetOptions([FromQuery] string source)
    {
        var code = source?.Trim();
        if (string.IsNullOrWhiteSpace(code))
            return [];

        var def = await _rep.GetFirstAsync(u => u.Code == code && u.Enabled);
        if (def == null)
            return [];

        try
        {
            ValidateDefRuntime(def);
        }
        catch
        {
            return [];
        }

        var main = HrLookupOptionsBuilder.BuildMainQuery(def);
        var rows = await _rep.Context.Ado.SqlQueryAsync<HrLookupOptionRow>(main.Sql, main.Parameters);
        if (rows.Count == 0)
            return [];

        var culture = HrUiCulture.Current();
        var values = rows.Select(u => u.Value ?? "").Where(u => u.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var langMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var langQ = HrLookupOptionsBuilder.BuildLangQuery(def, culture, values);
        if (langQ != null)
        {
            var langs = await _rep.Context.Ado.SqlQueryAsync<HrLookupOptionRow>(langQ.Sql, langQ.Parameters);
            foreach (var l in langs)
            {
                if (string.IsNullOrWhiteSpace(l.Value) || string.IsNullOrWhiteSpace(l.Label)) continue;
                langMap[l.Value] = l.Label!;
            }
        }

        return rows
            .Where(u => !string.IsNullOrWhiteSpace(u.Value))
            .Select(u =>
            {
                IReadOnlyDictionary<string, string>? one = null;
                if (langMap.TryGetValue(u.Value!, out var tr) && !string.IsNullOrWhiteSpace(tr))
                    one = new Dictionary<string, string> { [culture] = tr };
                var text = HrUiI18nResolver.ResolveCodeDesc(u.Value!, u.Label ?? "", culture, one);
                return new HrCodeItemOutput { Value = u.Value!, Label = text, Text = text };
            })
            .ToList();
    }

    private static void ValidateIdents(HrLookupDefSaveInput input)
    {
        HrLookupSqlGuard.RequireIdent(input.TableName, "表名");
        HrLookupSqlGuard.RequireIdent(input.ValueColumn, "值列");
        HrLookupSqlGuard.RequireIdent(input.LabelColumn, "显示列");
        if (!string.IsNullOrWhiteSpace(input.OrderColumn))
            HrLookupSqlGuard.RequireIdent(input.OrderColumn, "排序列");
        if (!string.IsNullOrWhiteSpace(input.FilterEnabledColumn))
            HrLookupSqlGuard.RequireIdent(input.FilterEnabledColumn, "启用列");
        if (!string.IsNullOrWhiteSpace(input.FilterEqualColumn))
            HrLookupSqlGuard.RequireIdent(input.FilterEqualColumn, "等值列");
        if (!string.IsNullOrWhiteSpace(input.LangTable))
            HrLookupSqlGuard.RequireIdent(input.LangTable, "翻译表");
        if (!string.IsNullOrWhiteSpace(input.LangValueColumn))
            HrLookupSqlGuard.RequireIdent(input.LangValueColumn, "译文值列");
        if (!string.IsNullOrWhiteSpace(input.LangCultureColumn))
            HrLookupSqlGuard.RequireIdent(input.LangCultureColumn, "文化列");
        if (!string.IsNullOrWhiteSpace(input.LangLabelColumn))
            HrLookupSqlGuard.RequireIdent(input.LangLabelColumn, "译文显示列");

        var hasEnCol = !string.IsNullOrWhiteSpace(input.FilterEnabledColumn);
        var hasEnVal = !string.IsNullOrWhiteSpace(input.FilterEnabledValue);
        if (hasEnCol != hasEnVal)
            throw Oops.Oh("启用列与启用值须成对配置");
    }

    private static void ValidateDefRuntime(HrLookupDef def)
    {
        HrLookupSqlGuard.RequireIdent(def.TableName, "表名");
        HrLookupSqlGuard.RequireIdent(def.ValueColumn, "值列");
        HrLookupSqlGuard.RequireIdent(def.LabelColumn, "显示列");
        if (!string.IsNullOrWhiteSpace(def.OrderColumn))
            HrLookupSqlGuard.RequireIdent(def.OrderColumn, "排序列");
        if (!string.IsNullOrWhiteSpace(def.FilterEnabledColumn))
            HrLookupSqlGuard.RequireIdent(def.FilterEnabledColumn, "启用列");
        if (!string.IsNullOrWhiteSpace(def.FilterEqualColumn))
            HrLookupSqlGuard.RequireIdent(def.FilterEqualColumn, "等值列");
        if (!string.IsNullOrWhiteSpace(def.LangTable))
            HrLookupSqlGuard.RequireIdent(def.LangTable, "翻译表");
    }

    private static HrLookupDef Apply(HrLookupDef entity, HrLookupDefSaveInput input, string code)
    {
        entity.Code = code;
        entity.Name = input.Name.Trim();
        entity.TableName = input.TableName.Trim();
        entity.ValueColumn = input.ValueColumn.Trim();
        entity.LabelColumn = input.LabelColumn.Trim();
        entity.OrderColumn = NullIfEmpty(input.OrderColumn);
        entity.FilterUseIsDelete = input.FilterUseIsDelete;
        entity.FilterEnabledColumn = NullIfEmpty(input.FilterEnabledColumn);
        entity.FilterEnabledValue = NullIfEmpty(input.FilterEnabledValue);
        entity.FilterEqualColumn = NullIfEmpty(input.FilterEqualColumn);
        entity.FilterEqualValue = NullIfEmpty(input.FilterEqualValue);
        entity.LangTable = NullIfEmpty(input.LangTable);
        entity.LangValueColumn = NullIfEmpty(input.LangValueColumn);
        entity.LangCultureColumn = NullIfEmpty(input.LangCultureColumn);
        entity.LangLabelColumn = NullIfEmpty(input.LangLabelColumn);
        entity.MaintainPath = NullIfEmpty(input.MaintainPath);
        entity.Enabled = input.Enabled;
        entity.Remark = NullIfEmpty(input.Remark);
        return entity;
    }

    private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
