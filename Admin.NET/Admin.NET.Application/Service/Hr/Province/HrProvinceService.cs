using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 省份代码设置
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrProvince", Order = 460, Description = "省市设置-省")]
public class HrProvinceService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrProvince> _rep;
    private readonly SqlSugarRepository<HrCity> _cityRep;
    private readonly SqlSugarRepository<HrProvinceLang> _langRep;

    public HrProvinceService(
        SqlSugarRepository<HrProvince> rep,
        SqlSugarRepository<HrCity> cityRep,
        SqlSugarRepository<HrProvinceLang> langRep)
    {
        _rep = rep;
        _cityRep = cityRep;
        _langRep = langRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("省份分页")]
    public async Task<SqlSugarPagedList<HrProvinceOutput>> Page(HrProvincePageInput input)
    {
        input ??= new HrProvincePageInput();
        var keyword = input.Keyword?.Trim();
        var status = input.Status?.Trim();

        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.ProvinceCode.Contains(keyword!) || u.ProvinceName.Contains(keyword!))
            .WhereIF(!string.IsNullOrWhiteSpace(status), u => u.Status == status);

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(HrProvince.OrderNo), descSort: false)
            .Select(u => new HrProvinceOutput
            {
                Id = u.Id,
                ProvinceCode = u.ProvinceCode,
                ProvinceName = u.ProvinceName,
                OrderNo = u.OrderNo,
                Status = u.Status,
                Remark = u.Remark
            })
            .ToPagedListAsync(input.Page, input.PageSize);
        await FillLocalizedNamesAsync(page.Items);
        return page;
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("省份详情")]
    public async Task<HrProvinceOutput> Get([FromQuery] long id)
    {
        // 编辑用主表原文，避免把译文写回主名称
        var row = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("省份不存在");
        return row.Adapt<HrProvinceOutput>();
    }

    [ApiDescriptionSettings(Name = "ListEnabled"), HttpGet]
    [DisplayName("现用省份列表")]
    public async Task<List<HrProvinceOutput>> ListEnabled()
    {
        var list = await _rep.AsQueryable()
            .Where(u => u.Status == HrRegionConsts.StatusActive)
            .OrderBy(u => u.OrderNo)
            .OrderBy(u => u.ProvinceCode)
            .Select(u => new HrProvinceOutput
            {
                Id = u.Id,
                ProvinceCode = u.ProvinceCode,
                ProvinceName = u.ProvinceName,
                OrderNo = u.OrderNo,
                Status = u.Status,
                Remark = u.Remark
            })
            .ToListAsync();
        await FillLocalizedNamesAsync(list);
        return list;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存省份")]
    public async Task<long> Save(HrProvinceSaveInput input)
    {
        var code = input.ProvinceCode.Trim();
        var name = input.ProvinceName.Trim();
        var status = NormalizeStatus(input.Status);

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("省份不存在");
            if (await _rep.IsAnyAsync(u => u.ProvinceCode == code && u.Id != entity.Id))
                throw Oops.Oh("省代码已存在");

            var oldCode = entity.ProvinceCode;
            entity.ProvinceCode = code;
            entity.ProvinceName = name;
            entity.OrderNo = input.OrderNo;
            entity.Status = status;
            entity.Remark = input.Remark?.Trim();
            await _rep.UpdateAsync(entity);

            if (!string.Equals(oldCode, code, StringComparison.OrdinalIgnoreCase))
            {
                await _cityRep.AsUpdateable()
                    .SetColumns(u => u.ProvinceCode == code)
                    .Where(u => u.ProvinceCode == oldCode)
                    .ExecuteCommandAsync();
                await _langRep.AsUpdateable()
                    .SetColumns(u => u.ProvinceCode == code)
                    .Where(u => u.ProvinceCode == oldCode)
                    .ExecuteCommandAsync();
            }
            return entity.Id;
        }

        if (await _rep.IsAnyAsync(u => u.ProvinceCode == code))
            throw Oops.Oh("省代码已存在");

        var inserted = await _rep.InsertReturnEntityAsync(new HrProvince
        {
            ProvinceCode = code,
            ProvinceName = name,
            OrderNo = input.OrderNo,
            Status = status,
            Remark = input.Remark?.Trim()
        });
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除省份")]
    public async Task Delete(HrProvinceDeleteInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("省份不存在");
        if (await _cityRep.IsAnyAsync(u => u.ProvinceCode == entity.ProvinceCode))
            throw Oops.Oh("该省下仍有城市，请先删除或迁移城市后再删省");
        await _langRep.DeleteAsync(u => u.ProvinceCode == entity.ProvinceCode);
        await _rep.FakeDeleteAsync(input.Id);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("省份译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] string provinceCode)
    {
        var code = provinceCode?.Trim();
        if (string.IsNullOrWhiteSpace(code)) return [];
        var rows = await _langRep.AsQueryable().Where(u => u.ProvinceCode == code).ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.ProvinceName }).ToList();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存省份译文")]
    public async Task SaveLang(HrProvinceLangSaveInput input)
    {
        var code = input.ProvinceCode?.Trim();
        if (string.IsNullOrWhiteSpace(code))
            throw Oops.Oh("省代码不能为空");
        if (!await _rep.IsAnyAsync(u => u.ProvinceCode == code))
            throw Oops.Oh("省份不存在");

        await _langRep.DeleteAsync(u => u.ProvinceCode == code);
        var rows = (input.Labels ?? [])
            .Select(u => new HrProvinceLang
            {
                ProvinceCode = code,
                Culture = HrUiCulture.Normalize(u.Culture),
                ProvinceName = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.ProvinceName))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }

    private async Task FillLocalizedNamesAsync(IEnumerable<HrProvinceOutput>? items)
    {
        if (items == null) return;
        var list = items as IList<HrProvinceOutput> ?? items.ToList();
        if (list.Count == 0) return;

        var culture = HrUiCulture.Current();
        if (string.Equals(culture, "zh-CN", StringComparison.OrdinalIgnoreCase))
            return;

        var codes = list.Select(u => u.ProvinceCode).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (codes.Count == 0) return;

        var langs = await _langRep.AsQueryable()
            .Where(u => codes.Contains(u.ProvinceCode) && u.Culture == culture)
            .ToListAsync();
        var map = langs
            .Where(u => !string.IsNullOrWhiteSpace(u.ProvinceName))
            .ToDictionary(u => u.ProvinceCode, u => u.ProvinceName!, StringComparer.OrdinalIgnoreCase);
        foreach (var item in list)
        {
            if (map.TryGetValue(item.ProvinceCode, out var name))
                item.ProvinceName = name;
        }
    }

    private static string NormalizeStatus(string? status)
    {
        var s = (status ?? HrRegionConsts.StatusActive).Trim();
        if (string.Equals(s, HrRegionConsts.StatusInactive, StringComparison.OrdinalIgnoreCase)
            || s is "停用" or "0" or "false")
            return HrRegionConsts.StatusInactive;
        return HrRegionConsts.StatusActive;
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
