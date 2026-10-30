using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 字段池：查询、维护、从物理表同步。
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrUiFieldDef", Order = 474, Description = "人事字段元数据")]
public class HrUiFieldDefService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrUiFieldDef> _defRep;
    private readonly SqlSugarRepository<HrUiFieldScene> _sceneRep;
    private readonly SqlSugarRepository<HrUiFieldDefLang> _langRep;
    private readonly SqlSugarRepository<HrUiLayout> _layoutRep;
    private readonly SqlSugarRepository<HrUiLayoutField> _layoutFieldRep;
    private readonly SysDatabaseService _databaseService;

    public HrUiFieldDefService(
        SqlSugarRepository<HrUiFieldDef> defRep,
        SqlSugarRepository<HrUiFieldScene> sceneRep,
        SqlSugarRepository<HrUiFieldDefLang> langRep,
        SqlSugarRepository<HrUiLayout> layoutRep,
        SqlSugarRepository<HrUiLayoutField> layoutFieldRep,
        SysDatabaseService databaseService)
    {
        _defRep = defRep;
        _sceneRep = sceneRep;
        _langRep = langRep;
        _layoutRep = layoutRep;
        _layoutFieldRep = layoutFieldRep;
        _databaseService = databaseService;
    }

    /// <summary>
    /// 某场景可进设计器的字段。Label 已按当前文化解析（尚无布局译文）。
    /// </summary>
    [NonAction]
    public async Task<List<HrUiFieldPoolItemOutput>> QueryPool(string moduleCode, string scene)
    {
        var module = HrUiModuleCatalog.Require(moduleCode);
        if (!string.IsNullOrWhiteSpace(scene))
            HrUiModuleCatalog.EnsureScene(scene);

        var sceneRows = await _sceneRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code)
            .WhereIF(!string.IsNullOrWhiteSpace(scene), u => u.Scene == scene)
            .ToListAsync();
        var keys = sceneRows.Select(u => u.FieldKey).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (keys.Count == 0) return [];

        var defs = await _defRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code && u.Enabled && u.IsInPool && keys.Contains(u.FieldKey))
            .OrderBy(u => u.SortNo)
            .ToListAsync();
        var culture = HrUiCulture.Current();
        var fieldKeys = defs.Select(u => u.FieldKey).ToList();
        var langs = await _langRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code && fieldKeys.Contains(u.FieldKey) && u.Culture == culture)
            .ToListAsync();
        var langMap = langs
            .GroupBy(u => u.FieldKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Culture, x => x.Label, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

        return defs.Select(d =>
        {
            langMap.TryGetValue(d.FieldKey, out var fieldLang);
            return new HrUiFieldPoolItemOutput
            {
                FieldKey = d.FieldKey,
                DbColumn = d.DbColumn,
                MainLabel = d.Label,
                Label = HrUiI18nResolver.ResolveFieldLabel(d.FieldKey, d.Label, culture, null, fieldLang),
                ControlType = HrUiControlGuess.Resolve(d.ControlType, d.CodeField, d.LookupSource),
                GroupCode = d.GroupCode,
                DefaultSpanSlots = d.DefaultSpanSlots <= 0 ? 1 : d.DefaultSpanSlots,
                DefaultWidthPx = d.DefaultWidthPx,
                Sortable = d.Sortable,
                Required = d.Required,
                Readonly = d.Readonly,
                Locked = d.Locked,
                TableName = d.TableName,
                CodeField = d.CodeField,
                LookupSource = d.LookupSource,
                Origin = d.Origin,
                DataType = d.DataType
            };
        }).ToList();
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("字段元数据分页")]
    public async Task<SqlSugarPagedList<HrUiFieldDefOutput>> Page(HrUiFieldDefPageInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        var keyword = input.Keyword?.Trim();
        var query = _defRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u =>
                u.FieldKey.Contains(keyword) || u.Label.Contains(keyword) || u.DbColumn.Contains(keyword))
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled)
            .WhereIF(input.IsInPool.HasValue, u => u.IsInPool == input.IsInPool)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ControlType), u => u.ControlType == input.ControlType);

        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(HrUiFieldDef.SortNo), descSort: false)
            .Select<HrUiFieldDefOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
        return page;
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("字段元数据详情")]
    public async Task<HrUiFieldDefOutput> Detail([FromQuery] long id)
    {
        var row = await _defRep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("HR_UI_FIELD_NOT_FOUND");
        return row.Adapt<HrUiFieldDefOutput>();
    }

    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新字段元数据")]
    public async Task Update(HrUiFieldDefUpdateInput input)
    {
        var row = await _defRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("HR_UI_FIELD_NOT_FOUND");
        var label = input.Label?.Trim();
        if (string.IsNullOrWhiteSpace(label) || label.Length > 256)
            throw Oops.Oh("显示名不能为空且不超过256");
        var origin = string.IsNullOrWhiteSpace(input.Origin) ? row.Origin : input.Origin.Trim();
        if (!HrUiControlGuess.Origins.Contains(origin))
            throw Oops.Oh("字段来源不合法");
        var control = string.IsNullOrWhiteSpace(input.ControlType) ? "text" : input.ControlType.Trim();
        if (!HrUiControlGuess.ControlTypes.Contains(control))
            throw Oops.Oh("控件类型不合法");
        if (input.DefaultSpanSlots < 1 || input.DefaultSpanSlots > 6)
            throw Oops.Oh("字段占位格数须在 1～6");
        if (input.Locked && !input.IsInPool)
            throw Oops.Oh("HR_UI_FIELD_NOT_IN_POOL");

        var codeField = string.IsNullOrWhiteSpace(input.CodeField) ? null : input.CodeField.Trim();
        var lookupSource = string.IsNullOrWhiteSpace(input.LookupSource) ? null : input.LookupSource.Trim();
        if (codeField != null && lookupSource != null)
            throw Oops.Oh("代码类与查找数据源不能同时设置");

        row.Label = label;
        row.DataType = input.DataType?.Trim();
        row.CodeField = codeField;
        row.LookupSource = lookupSource;
        row.ControlType = HrUiControlGuess.Resolve(control, row.CodeField, row.LookupSource);
        row.Origin = origin;
        row.DefaultWidthPx = input.DefaultWidthPx;
        row.DefaultSpanSlots = input.DefaultSpanSlots;
        row.GroupCode = input.GroupCode?.Trim();
        row.IsInPool = input.IsInPool;
        row.Sortable = input.Sortable;
        row.Required = input.Required;
        row.Readonly = input.Readonly;
        row.Locked = input.Locked;
        row.SortNo = input.SortNo;
        row.Enabled = input.Enabled;
        row.IsNullable = input.IsNullable;
        if (origin is "computed" or "lookup")
            row.DbColumn = string.IsNullOrWhiteSpace(input.DbColumn) ? null : input.DbColumn.Trim();
        else if (!string.IsNullOrWhiteSpace(input.DbColumn))
            row.DbColumn = input.DbColumn.Trim();
        if (!string.IsNullOrWhiteSpace(input.TableName))
            row.TableName = input.TableName.Trim();

        await _defRep.AsUpdateable(row).IgnoreColumns(u => new { u.CreateTime, u.CreateUserId, u.CreateUserName, u.FieldKey, u.ModuleCode }).ExecuteCommandAsync();

        // 布局行控件与池一致：有代码类时落成 select，避免生效布局仍读到历史 text
        var layoutIds = await _layoutRep.AsQueryable()
            .Where(u => u.ModuleCode == row.ModuleCode && !u.IsDelete)
            .Select(u => u.Id)
            .ToListAsync();
        if (layoutIds.Count > 0)
        {
            await _layoutFieldRep.AsUpdateable()
                .SetColumns(u => u.ControlType == row.ControlType)
                .Where(u => layoutIds.Contains(u.LayoutId) && u.FieldKey == row.FieldKey && !u.IsDelete)
                .ExecuteCommandAsync();
        }
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SyncFromDb"), HttpPost]
    [DisplayName("从物理表同步字段池")]
    public async Task<HrUiFieldSyncOutput> SyncFromDb(HrUiFieldSyncInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        var tables = _databaseService.GetTableList(ApplicationConst.HrDbConfigId);
        var exists = tables.Any(t => string.Equals(t.Name, module.MainTable, StringComparison.OrdinalIgnoreCase));
        if (!exists)
            throw Oops.Oh("HR_UI_SYNC_TABLE_MISSING");

        var columns = _databaseService.GetColumnList(module.MainTable, ApplicationConst.HrDbConfigId);
        if (columns == null || columns.Count == 0)
            throw Oops.Oh("HR_UI_SYNC_TABLE_MISSING");

        var existing = await _defRep.AsQueryable().Where(u => u.ModuleCode == module.Code).ToListAsync();
        var plan = HrUiFieldSyncPlanner.Plan(module.MainTable, existing.Select(u => new HrUiSyncRow
        {
            Id = u.Id,
            FieldKey = u.FieldKey,
            Origin = u.Origin,
            DbColumn = u.DbColumn,
            Enabled = u.Enabled,
            DataType = u.DataType,
            IsNullable = u.IsNullable,
            TableName = u.TableName
        }).ToList(), columns.Select(c => new HrUiSyncColumn
        {
            DbColumn = c.DbColumnName,
            DataType = c.DataType,
            IsNullable = c.IsNullable,
            Description = c.ColumnDescription
        }).ToList());

        var sortNo = existing.Count == 0 ? 0 : existing.Max(u => u.SortNo);
        if (plan.Inserts.Count > 0)
        {
            var rows = plan.Inserts.Select(ins =>
            {
                sortNo++;
                return new HrUiFieldDef
                {
                    Id = YitIdHelper.NextId(),
                    ModuleCode = module.Code,
                    FieldKey = ins.FieldKey,
                    Origin = "physical",
                    DbColumn = ins.DbColumn,
                    TableName = ins.TableName,
                    Label = ins.Label,
                    DataType = ins.DataType,
                    IsNullable = ins.IsNullable,
                    ControlType = ins.ControlType,
                    DefaultSpanSlots = 1,
                    DefaultWidthPx = 120,
                    GroupCode = "basic",
                    IsInPool = true,
                    Sortable = true,
                    SortNo = sortNo,
                    Enabled = true
                };
            }).ToList();
            await _defRep.InsertRangeAsync(rows);
            var scenes = new List<HrUiFieldScene>();
            foreach (var row in rows)
            {
                foreach (var scene in module.Scenes)
                    scenes.Add(new HrUiFieldScene { ModuleCode = module.Code, FieldKey = row.FieldKey, Scene = scene });
            }
            await _sceneRep.InsertRangeAsync(scenes);
        }

        foreach (var change in plan.Structural)
        {
            await _defRep.AsUpdateable()
                .SetColumns(u => u.DataType == change.DataType)
                .SetColumns(u => u.IsNullable == change.IsNullable)
                .SetColumns(u => u.TableName == change.TableName)
                .Where(u => u.Id == change.Id)
                .ExecuteCommandAsync();
        }
        if (plan.DisableIds.Count > 0)
        {
            await _defRep.AsUpdateable()
                .SetColumns(u => u.Enabled == false)
                .Where(u => plan.DisableIds.Contains(u.Id))
                .ExecuteCommandAsync();
        }
        if (plan.ReenableIds.Count > 0)
        {
            await _defRep.AsUpdateable()
                .SetColumns(u => u.Enabled == true)
                .Where(u => plan.ReenableIds.Contains(u.Id))
                .ExecuteCommandAsync();
        }

        return new HrUiFieldSyncOutput
        {
            Inserted = plan.Inserts.Count,
            StructuralUpdated = plan.Structural.Count,
            Disabled = plan.DisableIds.Count,
            Reenabled = plan.ReenableIds.Count,
            Warnings = plan.Warnings
        };
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("字段池译文")]
    public async Task<List<HrUiFieldLangOutput>> LangList([FromQuery] string moduleCode, [FromQuery] string fieldKey)
    {
        var module = HrUiModuleCatalog.Require(moduleCode);
        var rows = await _langRep.AsQueryable()
            .Where(u => u.ModuleCode == module.Code && u.FieldKey == fieldKey)
            .ToListAsync();
        return rows.Select(u => new HrUiFieldLangOutput { Culture = u.Culture, Label = u.Label }).ToList();
    }

    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存字段池译文")]
    public async Task SaveLang(HrUiFieldLangSaveInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        var key = input.FieldKey?.Trim();
        var exists = await _defRep.IsAnyAsync(u => u.ModuleCode == module.Code && u.FieldKey == key);
        if (!exists) throw Oops.Oh("HR_UI_FIELD_NOT_FOUND");

        await _langRep.DeleteAsync(u => u.ModuleCode == module.Code && u.FieldKey == key);
        var rows = (input.Labels ?? [])
            .Select(u => new HrUiFieldDefLang
            {
                ModuleCode = module.Code,
                FieldKey = key,
                Culture = HrUiCulture.Normalize(u.Culture),
                Label = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.Label))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }
}
