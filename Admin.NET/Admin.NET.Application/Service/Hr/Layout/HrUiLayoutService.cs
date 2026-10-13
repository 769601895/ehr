using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 人事 UI 布局（列表列 / 表单占位）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrUiLayout", Order = 473, Description = "人事UI布局")]
public class HrUiLayoutService : IDynamicApiController, ITransient
{
    public const string ModuleHrEmp = "hrEmp";

    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<HrUiLayout> _layoutRep;
    private readonly SqlSugarRepository<HrUiLayoutField> _fieldRep;

    public HrUiLayoutService(
        UserManager userManager,
        SqlSugarRepository<HrUiLayout> layoutRep,
        SqlSugarRepository<HrUiLayoutField> fieldRep)
    {
        _userManager = userManager;
        _layoutRep = layoutRep;
        _fieldRep = fieldRep;
    }

    /// <summary>字段池</summary>
    [DisplayName("人事布局字段池")]
    public List<HrUiFieldPoolItemOutput> FieldPool([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string? scene = null)
    {
        EnsureModule(moduleCode);
        return GetPool(scene).ToList();
    }

    /// <summary>当前生效方案（用户个人 → 系统默认 → 内置）</summary>
    [DisplayName("获取生效布局")]
    public async Task<HrUiLayoutOutput> GetEffective([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string scene = "list")
    {
        EnsureModule(moduleCode);
        EnsureScene(scene);
        var userId = _userManager.UserId;

        var mine = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.OwnerUserId == userId);
        if (mine != null)
            return await ToOutput(mine, "user");

        var sys = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.IsDefault && u.OwnerUserId == null);
        if (sys != null)
            return await ToOutput(sys, "system");

        return Builtin(moduleCode, scene);
    }

    /// <summary>系统默认方案</summary>
    [DisplayName("获取默认布局")]
    public async Task<HrUiLayoutOutput> GetDefault([FromQuery] string moduleCode = ModuleHrEmp, [FromQuery] string scene = "list")
    {
        EnsureModule(moduleCode);
        EnsureScene(scene);
        var sys = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == moduleCode && u.Scene == scene && u.IsDefault && u.OwnerUserId == null);
        return sys == null ? Builtin(moduleCode, scene) : await ToOutput(sys, "system");
    }

    /// <summary>保存系统默认（管理端定义页）</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveDefault"), HttpPost]
    [DisplayName("保存默认布局")]
    public async Task SaveDefault(HrUiLayoutSaveInput input)
    {
        EnsureModule(input.ModuleCode);
        EnsureScene(input.Scene);
        var fields = NormalizeAndValidate(input.Scene, input.Fields);

        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == input.ModuleCode && u.Scene == input.Scene && u.IsDefault && u.OwnerUserId == null);

        if (layout == null)
        {
            layout = new HrUiLayout
            {
                Id = YitIdHelper.NextId(),
                ModuleCode = input.ModuleCode,
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
            await _fieldRep.AsUpdateable()
                .SetColumns(u => u.IsDelete == true)
                .SetColumns(u => u.DeleteTime == DateTime.Now)
                .Where(u => u.LayoutId == layout.Id)
                .ExecuteCommandAsync();
        }

        await InsertFields(layout.Id, fields);
    }

    /// <summary>保存当前用户个人方案</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveMy"), HttpPost]
    [DisplayName("保存个人布局")]
    public async Task SaveMy(HrUiLayoutSaveInput input)
    {
        EnsureModule(input.ModuleCode);
        EnsureScene(input.Scene);
        var fields = NormalizeAndValidate(input.Scene, input.Fields);
        var userId = _userManager.UserId;

        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == input.ModuleCode && u.Scene == input.Scene && u.OwnerUserId == userId);

        if (layout == null)
        {
            layout = new HrUiLayout
            {
                Id = YitIdHelper.NextId(),
                ModuleCode = input.ModuleCode,
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
            await _fieldRep.AsUpdateable()
                .SetColumns(u => u.IsDelete == true)
                .SetColumns(u => u.DeleteTime == DateTime.Now)
                .Where(u => u.LayoutId == layout.Id)
                .ExecuteCommandAsync();
        }

        await InsertFields(layout.Id, fields);
    }

    /// <summary>删除个人方案，回退默认</summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "ResetMy"), HttpPost]
    [DisplayName("重置个人布局")]
    public async Task ResetMy(HrUiLayoutQueryInput input)
    {
        EnsureModule(input.ModuleCode);
        EnsureScene(input.Scene);
        var userId = _userManager.UserId;
        var layout = await _layoutRep.GetFirstAsync(u =>
            u.ModuleCode == input.ModuleCode && u.Scene == input.Scene && u.OwnerUserId == userId);
        if (layout == null) return;

        await _fieldRep.AsUpdateable()
            .SetColumns(u => u.IsDelete == true)
            .SetColumns(u => u.DeleteTime == DateTime.Now)
            .Where(u => u.LayoutId == layout.Id)
            .ExecuteCommandAsync();
        await _layoutRep.FakeDeleteAsync(layout.Id);
    }

    private async Task InsertFields(long layoutId, List<HrUiLayoutFieldInput> fields)
    {
        var rows = fields.Select((f, i) => new HrUiLayoutField
        {
            Id = YitIdHelper.NextId(),
            LayoutId = layoutId,
            FieldKey = f.FieldKey,
            DbColumn = f.DbColumn,
            Label = f.Label.Trim(),
            SortNo = f.SortNo > 0 ? f.SortNo : i + 1,
            Visible = f.Visible,
            WidthPx = f.WidthPx,
            SpanSlots = f.SpanSlots,
            GroupCode = f.GroupCode,
            ControlType = string.IsNullOrWhiteSpace(f.ControlType) ? "text" : f.ControlType,
            Required = f.Required,
            Readonly = f.Readonly
        }).ToList();
        if (rows.Count > 0)
            await _fieldRep.InsertRangeAsync(rows);
    }

    private async Task<HrUiLayoutOutput> ToOutput(HrUiLayout layout, string source)
    {
        var fields = await _fieldRep.AsQueryable()
            .Where(u => u.LayoutId == layout.Id)
            .OrderBy(u => u.SortNo)
            .ToListAsync();
        var poolMap = GetPool(layout.Scene).ToDictionary(u => u.FieldKey, StringComparer.OrdinalIgnoreCase);
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
            Fields = fields.Select(f =>
            {
                poolMap.TryGetValue(f.FieldKey, out var meta);
                return new HrUiLayoutFieldOutput
                {
                    Id = f.Id,
                    FieldKey = f.FieldKey,
                    DbColumn = f.DbColumn ?? meta?.DbColumn,
                    Label = f.Label,
                    SortNo = f.SortNo,
                    Visible = f.Visible,
                    WidthPx = f.WidthPx,
                    SpanSlots = f.SpanSlots,
                    GroupCode = f.GroupCode ?? meta?.GroupCode,
                    ControlType = f.ControlType,
                    Required = f.Required || (meta?.Required ?? false),
                    Readonly = f.Readonly || (meta?.Readonly ?? false),
                    Sortable = meta?.Sortable ?? true,
                    Locked = meta?.Locked ?? false
                };
            }).ToList()
        };
    }

    private List<HrUiLayoutFieldInput> NormalizeAndValidate(string scene, List<HrUiLayoutFieldInput> fields)
    {
        if (fields == null || fields.Count == 0)
            throw Oops.Oh("至少保留一个字段");

        var pool = GetPool(scene).ToDictionary(u => u.FieldKey, StringComparer.OrdinalIgnoreCase);
        var result = new List<HrUiLayoutFieldInput>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var f in fields.OrderBy(u => u.SortNo))
        {
            if (string.IsNullOrWhiteSpace(f.FieldKey))
                throw Oops.Oh("字段键不能为空");
            if (!pool.TryGetValue(f.FieldKey.Trim(), out var meta))
                throw Oops.Oh($"字段 {f.FieldKey} 不在白名单中");
            if (!seen.Add(meta.FieldKey))
                continue;

            var span = f.SpanSlots ?? meta.DefaultSpanSlots;
            if (scene == "form" && (span < 1 || span > 6))
                throw Oops.Oh($"字段 {meta.Label} 占位格数须在 1～6");

            result.Add(new HrUiLayoutFieldInput
            {
                FieldKey = meta.FieldKey,
                DbColumn = meta.DbColumn,
                Label = string.IsNullOrWhiteSpace(f.Label) ? meta.Label : f.Label.Trim(),
                SortNo = result.Count + 1,
                Visible = f.Visible,
                WidthPx = scene == "list" ? (f.WidthPx ?? meta.DefaultWidthPx ?? 100) : null,
                SpanSlots = scene == "form" ? span : null,
                GroupCode = string.IsNullOrWhiteSpace(f.GroupCode) ? meta.GroupCode : f.GroupCode,
                ControlType = string.IsNullOrWhiteSpace(f.ControlType) ? meta.ControlType : f.ControlType,
                Required = meta.Required || f.Required,
                Readonly = meta.Readonly || f.Readonly
            });
        }

        // 强制补回锁定字段
        foreach (var locked in pool.Values.Where(u => u.Locked))
        {
            if (result.Any(u => u.FieldKey.Equals(locked.FieldKey, StringComparison.OrdinalIgnoreCase)))
                continue;
            result.Insert(0, new HrUiLayoutFieldInput
            {
                FieldKey = locked.FieldKey,
                DbColumn = locked.DbColumn,
                Label = locked.Label,
                SortNo = 0,
                Visible = true,
                WidthPx = locked.DefaultWidthPx,
                SpanSlots = locked.DefaultSpanSlots,
                GroupCode = locked.GroupCode,
                ControlType = locked.ControlType,
                Required = locked.Required,
                Readonly = locked.Readonly
            });
        }

        for (var i = 0; i < result.Count; i++)
            result[i].SortNo = i + 1;
        return result;
    }

    private static void EnsureModule(string moduleCode)
    {
        if (!string.Equals(moduleCode, ModuleHrEmp, StringComparison.OrdinalIgnoreCase))
            throw Oops.Oh("暂不支持的模块码");
    }

    private static void EnsureScene(string scene)
    {
        if (scene is not ("list" or "form"))
            throw Oops.Oh("场景仅支持 list / form");
    }

    private HrUiLayoutOutput Builtin(string moduleCode, string scene)
    {
        var pool = GetPool(scene);
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
                Locked = p.Locked
            }).ToList()
        };
    }

    /// <summary>hrEmp 字段池（与 MVP 对照一致）</summary>
    private static IEnumerable<HrUiFieldPoolItemOutput> GetPool(string? scene)
    {
        var all = new List<HrUiFieldPoolItemOutput>
        {
            P("empId", "emp_id", "工号", "text", "basic", 1, 100, true, true, false, true),
            P("empName", "emp_name", "姓名", "text", "basic", 1, 100, true, true, false, true),
            P("empSex", "emp_sex", "性别", "text", "basic", 1, 70),
            P("empBirth", "emp_birth", "出生日期", "date", "basic", 1, 120),
            P("age", null, "年龄", "computed", "basic", 1, 70, false, false, true),
            P("empPic", "emp_pic", "照片", "photo", "basic", 1, 80, false),
            P("empCard", "emp_card", "身份证号", "text", "basic", 2, 160),
            P("empCardOverdate", "emp_card_overdate", "身份证到期日", "date", "basic", 1, 120),
            P("empCardDate", "emp_card_date", "发证日期", "date", "basic", 1, 120),
            P("empCardLimit", "emp_card_limit", "期限", "number", "basic", 1, 80),
            P("empAddrter", "emp_addrter", "联系电话", "text", "basic", 1, 120),
            P("empRgt", "emp_rgt", "户籍类型", "text", "basic", 1, 90),
            P("empNation", "emp_nation", "民族", "text", "basic", 1, 80),
            P("empProvince", "emp_province", "省份", "text", "basic", 1, 90),
            P("empMarry", "emp_marry", "婚姻状况", "text", "basic", 1, 90),
            P("empResidence", "emp_residence", "户口所在地", "text", "basic", 2, 160),
            P("empAddr", "emp_addr", "现住址", "text", "basic", 2, 160),
            P("empHome", "emp_home", "家庭住址", "text", "basic", 2, 160),
            P("ccCode", "cc_code", "部门", "cascader", "job", 2, 140, false),
            P("ccDesc", null, "部门", "text", "job", 1, 140, false), // 列表展示名
            P("dutyId", "duty_id", "职务", "text", "job", 1, 90),
            P("postId", "post_id", "岗位", "text", "job", 1, 90),
            P("empType", "emp_type", "人员类别", "text", "job", 1, 100),
            P("empSal", "emp_sal", "薪酬类别", "text", "job", 1, 90),
            P("empWorkDate", "emp_work_date", "参加工作时间", "date", "job", 1, 120),
            P("empInDateGroup", "emp_in_date_group", "集团入职日", "date", "job", 1, 120),
            P("empInDate", "emp_in_date", "公司入职日", "date", "job", 1, 120),
            P("empBegDate", "emp_beg_date", "试用起始日", "date", "job", 1, 120),
            P("empTryDate", "emp_try_date", "试用到期日", "date", "job", 1, 120),
            P("empFromDate", "emp_from_date", "转正日", "date", "job", 1, 120),
            P("empLongmon", "emp_longmon", "入司前工龄(月)", "number", "job", 1, 110),
            P("empKqIndate", "emp_kq_indate", "考勤开始日", "date", "job", 1, 120),
            P("empKqLeaDate", "emp_kq_lea_date", "考勤结束日", "date", "job", 1, 120),
            P("empState", "emp_state", "人员状态", "select", "job", 1, 80),
        };

        if (scene == "list")
            return all.Where(u => u.FieldKey is not ("empPic" or "ccCode" or "empBirth" or "empCard"
                or "empCardOverdate" or "empCardDate" or "empCardLimit" or "empRgt" or "empNation"
                or "empProvince" or "empMarry" or "empResidence" or "empAddr" or "empHome"
                or "empWorkDate" or "empInDateGroup" or "empBegDate" or "empTryDate" or "empFromDate"
                or "empLongmon" or "empKqIndate" or "empKqLeaDate"));
        // form 不含列表专用 ccDesc
        return all.Where(u => u.FieldKey != "ccDesc");
    }

    private static HrUiFieldPoolItemOutput P(
        string key, string? db, string label, string ctrl, string group,
        int span, int? width, bool sortable = true, bool required = false, bool readonly_ = false, bool locked = false)
        => new()
        {
            FieldKey = key,
            DbColumn = db,
            Label = label,
            ControlType = ctrl,
            GroupCode = group,
            DefaultSpanSlots = span,
            DefaultWidthPx = width,
            Sortable = sortable,
            Required = required,
            Readonly = readonly_,
            Locked = locked
        };
}
