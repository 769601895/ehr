using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 城市代码设置
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrCity", Order = 461, Description = "省市设置-市")]
public class HrCityService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrCity> _rep;
    private readonly SqlSugarRepository<HrProvince> _provinceRep;
    private readonly SqlSugarRepository<HrCityLang> _langRep;

    public HrCityService(
        SqlSugarRepository<HrCity> rep,
        SqlSugarRepository<HrProvince> provinceRep,
        SqlSugarRepository<HrCityLang> langRep)
    {
        _rep = rep;
        _provinceRep = provinceRep;
        _langRep = langRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("城市分页")]
    public async Task<SqlSugarPagedList<HrCityOutput>> Page(HrCityPageInput input)
    {
        input ??= new HrCityPageInput();
        var provinceCode = input.ProvinceCode?.Trim();
        var keyword = input.Keyword?.Trim();
        var status = input.Status?.Trim();

        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(provinceCode), u => u.ProvinceCode == provinceCode)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                u => u.CityCode.Contains(keyword!) || u.CityName.Contains(keyword!))
            .WhereIF(!string.IsNullOrWhiteSpace(status), u => u.Status == status);

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(HrCity.OrderNo), descSort: false)
            .Select(u => new HrCityOutput
            {
                Id = u.Id,
                ProvinceCode = u.ProvinceCode,
                CityCode = u.CityCode,
                CityName = u.CityName,
                OrderNo = u.OrderNo,
                Status = u.Status,
                Remark = u.Remark
            })
            .ToPagedListAsync(input.Page, input.PageSize);
        await FillLocalizedNamesAsync(page.Items);
        return page;
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("城市详情")]
    public async Task<HrCityOutput> Get([FromQuery] long id)
    {
        // 编辑用主表原文，避免把译文写回主名称
        var row = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("城市不存在");
        return row.Adapt<HrCityOutput>();
    }

    [ApiDescriptionSettings(Name = "ListByProvince"), HttpGet]
    [DisplayName("按省查询现用城市")]
    public async Task<List<HrCityOutput>> ListByProvince([FromQuery] string provinceCode)
    {
        var code = provinceCode?.Trim();
        if (string.IsNullOrWhiteSpace(code)) return [];

        var list = await _rep.AsQueryable()
            .Where(u => u.ProvinceCode == code && u.Status == HrRegionConsts.StatusActive)
            .OrderBy(u => u.OrderNo)
            .OrderBy(u => u.CityCode)
            .Select(u => new HrCityOutput
            {
                Id = u.Id,
                ProvinceCode = u.ProvinceCode,
                CityCode = u.CityCode,
                CityName = u.CityName,
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
    [DisplayName("保存城市")]
    public async Task<long> Save(HrCitySaveInput input)
    {
        var provinceCode = input.ProvinceCode.Trim();
        var cityCode = input.CityCode.Trim();
        var cityName = input.CityName.Trim();
        var status = NormalizeStatus(input.Status);

        if (!await _provinceRep.IsAnyAsync(u => u.ProvinceCode == provinceCode))
            throw Oops.Oh("所属省份不存在");

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("城市不存在");
            if (await _rep.IsAnyAsync(u => u.CityCode == cityCode && u.Id != entity.Id))
                throw Oops.Oh("市代码已存在");

            var oldCode = entity.CityCode;
            entity.ProvinceCode = provinceCode;
            entity.CityCode = cityCode;
            entity.CityName = cityName;
            entity.OrderNo = input.OrderNo;
            entity.Status = status;
            entity.Remark = input.Remark?.Trim();
            await _rep.UpdateAsync(entity);
            if (!string.Equals(oldCode, cityCode, StringComparison.OrdinalIgnoreCase))
            {
                await _langRep.AsUpdateable()
                    .SetColumns(u => u.CityCode == cityCode)
                    .Where(u => u.CityCode == oldCode)
                    .ExecuteCommandAsync();
            }
            return entity.Id;
        }

        if (await _rep.IsAnyAsync(u => u.CityCode == cityCode))
            throw Oops.Oh("市代码已存在");

        var inserted = await _rep.InsertReturnEntityAsync(new HrCity
        {
            ProvinceCode = provinceCode,
            CityCode = cityCode,
            CityName = cityName,
            OrderNo = input.OrderNo,
            Status = status,
            Remark = input.Remark?.Trim()
        });
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除城市")]
    public async Task Delete(HrCityDeleteInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("城市不存在");
        await _langRep.DeleteAsync(u => u.CityCode == entity.CityCode);
        await _rep.FakeDeleteAsync(input.Id);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("城市译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] string cityCode)
    {
        var code = cityCode?.Trim();
        if (string.IsNullOrWhiteSpace(code)) return [];
        var rows = await _langRep.AsQueryable().Where(u => u.CityCode == code).ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.CityName }).ToList();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存城市译文")]
    public async Task SaveLang(HrCityLangSaveInput input)
    {
        var code = input.CityCode?.Trim();
        if (string.IsNullOrWhiteSpace(code))
            throw Oops.Oh("市代码不能为空");
        if (!await _rep.IsAnyAsync(u => u.CityCode == code))
            throw Oops.Oh("城市不存在");

        await _langRep.DeleteAsync(u => u.CityCode == code);
        var rows = (input.Labels ?? [])
            .Select(u => new HrCityLang
            {
                CityCode = code,
                Culture = HrUiCulture.Normalize(u.Culture),
                CityName = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.CityName))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }

    private async Task FillLocalizedNamesAsync(IEnumerable<HrCityOutput>? items)
    {
        if (items == null) return;
        var list = items as IList<HrCityOutput> ?? items.ToList();
        if (list.Count == 0) return;

        var culture = HrUiCulture.Current();
        if (string.Equals(culture, "zh-CN", StringComparison.OrdinalIgnoreCase))
            return;

        var codes = list.Select(u => u.CityCode).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (codes.Count == 0) return;

        var langs = await _langRep.AsQueryable()
            .Where(u => codes.Contains(u.CityCode) && u.Culture == culture)
            .ToListAsync();
        var map = langs
            .Where(u => !string.IsNullOrWhiteSpace(u.CityName))
            .ToDictionary(u => u.CityCode, u => u.CityName!, StringComparer.OrdinalIgnoreCase);
        foreach (var item in list)
        {
            if (map.TryGetValue(item.CityCode, out var name))
                item.CityName = name;
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
