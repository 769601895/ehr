using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 人事 UI 布局（列表列 / 表单占位）。字段池来自 hr_ui_field_def，控件与码类以池为准。
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrUiLayout", Order = 473, Description = "人事UI布局")]
public class HrUiLayoutService : IDynamicApiController, ITransient
{
    public const string ModuleHrEmp = HrUiModuleCodes.HrEmp;

    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<HrUiLayout> _layoutRep;
    private readonly SqlSugarRepository<HrUiLayoutField> _fieldRep;
    private readonly SqlSugarRepository<HrUiLayoutFieldLang> _langRep;
    private readonly HrUiFieldDefService _fieldDefService;

    public HrUiLayoutService(
        UserManager userManager,
        SqlSugarRepository<HrUiLayout> layoutRep,
        SqlSugarRepository<HrUiLayoutField> fieldRep,
        SqlSugarRepository<HrUiLayoutFieldLang> langRep,
        HrUiFieldDefService fieldDefService)
    {
        _userManager = userManager;
        _layoutRep = layoutRep;
        _fieldRep = fieldRep;
        _langRep = langRep;
        _fieldDefService = fieldDefService;
    }

    /// <summary>字段池</summary>
    [ApiDescriptionSettings(Name = "FieldPool"), HttpGet]
    [DisplayName("人事布局字段池")]
    public async Task<List<HrUiFieldPoolItemOutput>> FieldPool([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string scene = null)
    {
        return await _fieldDefService.QueryPool(moduleCode, scene);
    }

    /// <summary>当前生效方案（用户个人 → 系统默认 → 内置）</summary>
    [ApiDescriptionSettings(Name = "Effective"), HttpGet]
    [DisplayName("获取生效布局")]
    public async Task<HrUiLayoutOutput> GetEffective([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string scene = "list")
    {
        HrUiModuleCatalog.Require(moduleCode);
        HrUiModuleCatalog.EnsureScene(scene);
        var userId = _userManager.UserId;

        var mine = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.OwnerUserId == userId);
        if (mine != null)
            return await ToOutput(mine, "user");

        var sys = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.IsDefault && u.OwnerUserId == null);
        if (sys != null)
            return await ToOutput(sys, "system");

        return await Builtin(moduleCode, scene);
    }

    /// <summary>系统默认方案</summary>
    [ApiDescriptionSettings(Name = "Default"), HttpGet]
    [DisplayName("获取默认布局")]
    public async Task<HrUiLayoutOutput> GetDefault([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string scene = "list")
    {
        HrUiModuleCatalog.Require(moduleCode);
        HrUiModuleCatalog.EnsureScene(scene);
        var sys = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.IsDefault && u.OwnerUserId == null);
        return sys == null ? await Builtin(moduleCode, scene) : await ToOutput(sys, "system");
    }

    /// <summary>保存系统默认（管理端定义页）</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDefault"), HttpPost]
    [DisplayName("保存默认布局")]
    public async Task SaveDefault(HrUiLayoutSaveInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        HrUiModuleCatalog.EnsureScene(input.Scene);
        var fields = await NormalizeAndValidate(module.Code, input.Scene, input.Fields);

        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == module.Code && u.Scene == input.Scene && u.IsDefault && u.OwnerUserId == null);

        if (layout == null)
        {
            layout = new HrUiLayout
            {
                Id = YitIdHelper.NextId(),
                ModuleCode = module.Code,
                Scene = input.Scene,
                Name = string.IsNullOrWhiteSpace(input.Name)
                    ? (input.Scene == "list" ? "人员资料默认列表" : "人员资料默认表单")
                    : input.Name.Trim(),
                IsDefault = true,
                OwnerUserId = null,
                Version = 1
            };
            await _layoutRep.InsertAsync(layout);
        }
        else
        {
            layout.Name = string.IsNullOrWhiteSpace(input.Name) ? layout.Name : input.Name.Trim();
            layout.Version += 1;
            await _layoutRep.AsUpdateable(layout).UpdateColumns(u => new { u.Name, u.Version, u.UpdateTime }).ExecuteCommandAsync();
            await ClearFields(layout.Id);
        }

        await InsertFields(layout.Id, fields);
    }

    /// <summary>保存当前用户个人方案</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveMy"), HttpPost]
    [DisplayName("保存个人布局")]
    public async Task SaveMy(HrUiLayoutSaveInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        HrUiModuleCatalog.EnsureScene(input.Scene);
        var fields = await NormalizeAndValidate(module.Code, input.Scene, input.Fields);
        var userId = _userManager.UserId;

        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == module.Code && u.Scene == input.Scene && u.OwnerUserId == userId);

        if (layout == null)
        {
            layout = new HrUiLayout
            {
                Id = YitIdHelper.NextId(),
                ModuleCode = module.Code,
                Scene = input.Scene,
                Name = string.IsNullOrWhiteSpace(input.Name) ? "我的方案" : input.Name.Trim(),
                IsDefault = false,
                OwnerUserId = userId,
                Version = 1
            };
            await _layoutRep.InsertAsync(layout);
        }
        else
        {
            layout.Name = string.IsNullOrWhiteSpace(input.Name) ? layout.Name : input.Name.Trim();
            layout.Version += 1;
            await _layoutRep.AsUpdateable(layout).UpdateColumns(u => new { u.Name, u.Version, u.UpdateTime }).ExecuteCommandAsync();
            await ClearFields(layout.Id);
        }

        await InsertFields(layout.Id, fields);
    }

    /// <summary>删除个人方案，回退默认</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "ResetMy"), HttpPost]
    [DisplayName("重置个人布局")]
    public async Task ResetMy(HrUiLayoutQueryInput input)
    {
        var module = HrUiModuleCatalog.Require(input.ModuleCode);
        HrUiModuleCatalog.EnsureScene(input.Scene);
        var userId = _userManager.UserId;
        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == module.Code && u.Scene == input.Scene && u.OwnerUserId == userId);
        if (layout == null) return;

        await ClearFields(layout.Id);
        await _layoutRep.FakeDeleteAsync(layout.Id);
    }

    private async Task ClearFields(long layoutId)
    {
        // 超管不走软删过滤器，假删除的旧行会再次出现在列表里。布局字段整批替换，直接物理删除。
        var oldIds = await _fieldRep.AsQueryable().ClearFilter()
            .Where(u => u.LayoutId == layoutId)
            .Select(u => u.Id)
            .ToListAsync();
        if (oldIds.Count > 0)
            await _langRep.DeleteAsync(u => oldIds.Contains(u.LayoutFieldId));
        await _fieldRep.AsDeleteable().Where(u => u.LayoutId == layoutId).ExecuteCommandAsync();
    }

    private async Task InsertFields(long layoutId, List<NormalizedLayoutField> fields)
    {
        var rows = fields.Select(f =>
        {
            var id = YitIdHelper.NextId();
            f.Id = id;
            return new HrUiLayoutField
            {
                Id = id,
                LayoutId = layoutId,
                FieldKey = f.FieldKey,
                DbColumn = f.DbColumn,
                Label = f.MainLabel != null && f.MainLabel.Length > 50 ? f.MainLabel[..50] : f.MainLabel,
                SortNo = f.SortNo,
                Visible = f.Visible,
                WidthPx = f.WidthPx,
                SpanSlots = f.SpanSlots,
                GroupCode = f.GroupCode,
                ControlType = f.ControlType,
                Required = f.Required,
                Readonly = f.Readonly
            };
        }).ToList();
        if (rows.Count > 0)
            await _fieldRep.InsertRangeAsync(rows);

        var langs = new List<HrUiLayoutFieldLang>();
        foreach (var f in fields)
        {
            foreach (var lb in f.Labels)
            {
                if (string.IsNullOrWhiteSpace(lb.Label)) continue;
                langs.Add(new HrUiLayoutFieldLang
                {
                    LayoutFieldId = f.Id,
                    Culture = lb.Culture,
                    Label = lb.Label.Trim()
                });
            }
        }
        if (langs.Count > 0)
            await _langRep.InsertRangeAsync(langs);
    }

    private async Task<HrUiLayoutOutput> ToOutput(HrUiLayout layout, string source)
    {
        var fields = (await _fieldRep.AsQueryable().ClearFilter()
            .Where(u => u.LayoutId == layout.Id && u.IsDelete == false)
            .OrderBy(u => u.SortNo)
            .ToListAsync())
            .GroupBy(u => u.FieldKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderBy(u => u.SortNo).First())
            .ToList();
        var pool = await _fieldDefService.QueryPool(layout.ModuleCode, layout.Scene);
        var poolMap = pool.ToDictionary(u => u.FieldKey, StringComparer.OrdinalIgnoreCase);
        var culture = HrUiCulture.Current();
        var ids = fields.Select(u => u.Id).ToList();
        var langRows = ids.Count == 0
            ? []
            : await _langRep.AsQueryable()
                .Where(u => ids.Contains(u.LayoutFieldId) && u.Culture == culture)
                .ToListAsync();
        var langMap = langRows.ToDictionary(u => u.LayoutFieldId, u => u.Label);

        return new HrUiLayoutOutput
        {
            Id = layout.Id,
            ModuleCode = layout.ModuleCode,
            Scene = layout.Scene,
            Name = layout.Name,
            IsDefault = layout.IsDefault,
            OwnerUserId = layout.OwnerUserId,
            Version = layout.Version,
            Source = source,
            Fields = fields.Select(f => MapField(f, poolMap, langMap, culture)).ToList()
        };
    }

    private static HrUiLayoutFieldOutput MapField(
        HrUiLayoutField f,
        Dictionary<string, HrUiFieldPoolItemOutput> poolMap,
        Dictionary<long, string> langMap,
        string culture)
    {
        poolMap.TryGetValue(f.FieldKey, out var meta);
        langMap.TryGetValue(f.Id, out var layoutLabel);
        IReadOnlyDictionary<string, string> layoutLang = string.IsNullOrWhiteSpace(layoutLabel)
            ? null
            : new Dictionary<string, string> { [culture] = layoutLabel };
        // 布局当前语言译文优先；否则用池侧已解析 Label（field_def_lang → 池主语言 → FieldKey）。
        var label = layoutLang != null && layoutLang.TryGetValue(culture, out var ov) && !string.IsNullOrWhiteSpace(ov)
            ? ov.Trim()
            : (meta?.Label ?? f.Label);
        return new HrUiLayoutFieldOutput
        {
            Id = f.Id,
            FieldKey = f.FieldKey,
            DbColumn = meta?.DbColumn ?? f.DbColumn,
            Label = label,
            SortNo = f.SortNo,
            Visible = f.Visible,
            WidthPx = f.WidthPx,
            SpanSlots = f.SpanSlots,
            GroupCode = f.GroupCode ?? meta?.GroupCode,
            ControlType = HrUiControlGuess.Resolve(meta?.ControlType ?? f.ControlType, meta?.CodeField, meta?.LookupSource),
            Required = f.Required,
            Readonly = f.Readonly,
            Sortable = meta?.Sortable ?? false,
            Locked = meta?.Locked ?? false,
            CodeField = meta?.CodeField,
            LookupSource = meta?.LookupSource,
            Origin = meta?.Origin,
            DataType = meta?.DataType,
            IsStale = meta == null
        };
    }

    private async Task<List<NormalizedLayoutField>> NormalizeAndValidate(string moduleCode, string scene, List<HrUiLayoutFieldInput> fields)
    {
        if (fields == null || fields.Count == 0)
            throw Oops.Oh("HR_UI_LAYOUT_EMPTY");

        var pool = (await _fieldDefService.QueryPool(moduleCode, scene))
            .ToDictionary(u => u.FieldKey, StringComparer.OrdinalIgnoreCase);
        var result = new List<NormalizedLayoutField>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var culture = HrUiCulture.Current();

        foreach (var f in fields.OrderBy(u => u.SortNo))
        {
            if (string.IsNullOrWhiteSpace(f.FieldKey))
                throw Oops.Oh("HR_UI_FIELD_NOT_IN_POOL");
            if (!pool.TryGetValue(f.FieldKey.Trim(), out var meta))
                throw Oops.Oh("HR_UI_FIELD_NOT_IN_POOL");
            if (!seen.Add(meta.FieldKey))
                continue;

            var span = f.SpanSlots ?? meta.DefaultSpanSlots;
            if (scene == "form" && (span < 1 || span > 6))
                throw Oops.Oh("字段占位格数须在 1～6");

            result.Add(new NormalizedLayoutField
            {
                FieldKey = meta.FieldKey,
                DbColumn = meta.DbColumn,
                MainLabel = string.IsNullOrWhiteSpace(meta.MainLabel) ? meta.Label : meta.MainLabel,
                SortNo = result.Count + 1,
                Visible = f.Visible,
                WidthPx = scene == "list" ? (f.WidthPx ?? meta.DefaultWidthPx ?? 100) : null,
                SpanSlots = scene == "form" ? span : null,
                GroupCode = string.IsNullOrWhiteSpace(f.GroupCode) ? meta.GroupCode : f.GroupCode,
                ControlType = meta.ControlType,
                Required = f.Required,
                Readonly = f.Readonly,
                Labels = CollectLabels(f, meta, culture)
            });
        }

        foreach (var locked in pool.Values.Where(u => u.Locked))
        {
            if (result.Any(u => u.FieldKey.Equals(locked.FieldKey, StringComparison.OrdinalIgnoreCase)))
                continue;
            result.Insert(0, new NormalizedLayoutField
            {
                FieldKey = locked.FieldKey,
                DbColumn = locked.DbColumn,
                MainLabel = string.IsNullOrWhiteSpace(locked.MainLabel) ? locked.Label : locked.MainLabel,
                SortNo = 0,
                Visible = true,
                WidthPx = locked.DefaultWidthPx,
                SpanSlots = locked.DefaultSpanSlots,
                GroupCode = locked.GroupCode,
                ControlType = locked.ControlType,
                Required = locked.Required,
                Readonly = locked.Readonly,
                Labels = []
            });
        }

        if (result.Count == 0)
            throw Oops.Oh("HR_UI_LAYOUT_EMPTY");

        for (var i = 0; i < result.Count; i++)
            result[i].SortNo = i + 1;
        return result;
    }

    private static List<HrUiLayoutLabelInput> CollectLabels(HrUiLayoutFieldInput input, HrUiFieldPoolItemOutput meta, string culture)
    {
        var list = new List<HrUiLayoutLabelInput>();
        if (input.Labels != null)
        {
            foreach (var lb in input.Labels)
            {
                list.Add(new HrUiLayoutLabelInput
                {
                    Culture = HrUiCulture.Normalize(lb.Culture),
                    Label = lb.Label?.Trim()
                });
            }
        }
        else if (!string.IsNullOrWhiteSpace(input.Label))
        {
            var main = string.IsNullOrWhiteSpace(meta.MainLabel) ? meta.Label : meta.MainLabel;
            if (!string.Equals(input.Label.Trim(), main?.Trim(), StringComparison.Ordinal))
            {
                list.Add(new HrUiLayoutLabelInput
                {
                    Culture = culture,
                    Label = input.Label.Trim()
                });
            }
        }

        return list
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
    }

    private async Task<HrUiLayoutOutput> Builtin(string moduleCode, string scene)
    {
        var pool = await _fieldDefService.QueryPool(moduleCode, scene);
        return new HrUiLayoutOutput
        {
            Id = 0,
            ModuleCode = moduleCode,
            Scene = scene,
            Name = scene == "list" ? "内置列表" : "内置表单",
            IsDefault = true,
            Source = "builtin",
            Version = 0,
            Fields = pool.Select((p, i) => new HrUiLayoutFieldOutput
            {
                FieldKey = p.FieldKey,
                DbColumn = p.DbColumn,
                Label = p.Label,
                SortNo = i + 1,
                Visible = true,
                WidthPx = p.DefaultWidthPx,
                SpanSlots = p.DefaultSpanSlots,
                GroupCode = p.GroupCode,
                ControlType = p.ControlType,
                Required = p.Required,
                Readonly = p.Readonly,
                Sortable = p.Sortable,
                Locked = p.Locked,
                CodeField = p.CodeField,
                LookupSource = p.LookupSource,
                Origin = p.Origin,
                DataType = p.DataType,
                IsStale = false
            }).ToList()
        };
    }

    private sealed class NormalizedLayoutField
    {
        public long Id { get; set; }
        public string FieldKey { get; set; }
        public string DbColumn { get; set; }
        public string MainLabel { get; set; }
        public int SortNo { get; set; }
        public bool Visible { get; set; }
        public int? WidthPx { get; set; }
        public int? SpanSlots { get; set; }
        public string GroupCode { get; set; }
        public string ControlType { get; set; }
        public bool Required { get; set; }
        public bool Readonly { get; set; }
        public List<HrUiLayoutLabelInput> Labels { get; set; } = [];
    }
}
