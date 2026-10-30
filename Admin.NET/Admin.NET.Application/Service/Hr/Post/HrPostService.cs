using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 岗位代码设置
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrPost", Order = 470, Description = "岗位代码设置")]
public class HrPostService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrPost> _rep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<HrPostLang> _langRep;

    public HrPostService(
        SqlSugarRepository<HrPost> rep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<HrPostLang> langRep)
    {
        _rep = rep;
        _ccRep = ccRep;
        _langRep = langRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("岗位代码分页")]
    public async Task<SqlSugarPagedList<HrPostOutput>> Page(HrPostPageInput input)
    {
        input ??= new HrPostPageInput();
        var keyword = input.Keyword?.Trim();
        var status = input.Status?.Trim();
        var ccCode = input.CcCode?.Trim();

        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), u => u.PostCode.Contains(keyword!) || u.PostName.Contains(keyword!))
            .WhereIF(!string.IsNullOrWhiteSpace(status), u => u.Status == status)
            .WhereIF(!string.IsNullOrWhiteSpace(ccCode), u => u.CcCode == ccCode)
            .WhereIF(input.NeedAudit.HasValue, u => u.NeedAudit == input.NeedAudit);

        NormalizeSort(input);
        var page = await query
            .OrderBuilder(input, defaultSortField: nameof(HrPost.PostCode), descSort: false)
            .Select(u => new HrPostOutput
            {
                Id = u.Id,
                PostCode = u.PostCode,
                PostName = u.PostName,
                Grade = u.Grade,
                Status = u.Status,
                CcCode = u.CcCode,
                NeedAudit = u.NeedAudit,
                Remark = u.Remark
            })
            .ToPagedListAsync(input.Page, input.PageSize);

        await FillCcNamesAsync(page.Items);
        return page;
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("岗位代码详情")]
    public async Task<HrPostOutput> Get([FromQuery] long id)
    {
        var row = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh("岗位不存在");
        var output = row.Adapt<HrPostOutput>();
        await FillCcNamesAsync([output]);
        return output;
    }

    [ApiDescriptionSettings(Name = "ListEnabled"), HttpGet]
    [DisplayName("现用岗位列表")]
    public async Task<List<HrPostOutput>> ListEnabled()
    {
        var list = await _rep.AsQueryable()
            .Where(u => u.Status == HrPostConsts.StatusActive)
            .OrderBy(u => u.PostCode)
            .Select(u => new HrPostOutput
            {
                Id = u.Id,
                PostCode = u.PostCode,
                PostName = u.PostName,
                Grade = u.Grade,
                Status = u.Status,
                CcCode = u.CcCode,
                NeedAudit = u.NeedAudit,
                Remark = u.Remark
            })
            .ToListAsync();
        await FillCcNamesAsync(list);
        return list;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存岗位代码")]
    public async Task<long> Save(HrPostSaveInput input)
    {
        var code = input.PostCode.Trim();
        var name = input.PostName.Trim();
        var status = NormalizeStatus(input.Status);
        var ccCode = string.IsNullOrWhiteSpace(input.CcCode) ? null : input.CcCode.Trim();

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("岗位不存在");
            if (await _rep.IsAnyAsync(u => u.PostCode == code && u.Id != entity.Id))
                throw Oops.Oh("岗位代码已存在");

            var oldCode = entity.PostCode;
            entity.PostCode = code;
            entity.PostName = name;
            entity.Grade = input.Grade;
            entity.Status = status;
            entity.CcCode = ccCode;
            entity.NeedAudit = input.NeedAudit;
            entity.Remark = input.Remark?.Trim();
            await _rep.UpdateAsync(entity);
            if (!string.Equals(oldCode, code, StringComparison.OrdinalIgnoreCase))
            {
                await _langRep.AsUpdateable()
                    .SetColumns(u => u.PostCode == code)
                    .Where(u => u.PostCode == oldCode)
                    .ExecuteCommandAsync();
            }
            return entity.Id;
        }

        if (await _rep.IsAnyAsync(u => u.PostCode == code))
            throw Oops.Oh("岗位代码已存在");

        var inserted = await _rep.InsertReturnEntityAsync(new HrPost
        {
            PostCode = code,
            PostName = name,
            Grade = input.Grade,
            Status = status,
            CcCode = ccCode,
            NeedAudit = input.NeedAudit,
            Remark = input.Remark?.Trim()
        });
        return inserted.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除岗位代码")]
    public async Task Delete(HrPostDeleteInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh("岗位不存在");
        await _langRep.DeleteAsync(u => u.PostCode == entity.PostCode);
        await _rep.FakeDeleteAsync(input.Id);
    }

    [ApiDescriptionSettings(Name = "LangList"), HttpGet]
    [DisplayName("岗位代码译文")]
    public async Task<List<HrCodeLangOutput>> LangList([FromQuery] string postCode)
    {
        var code = postCode?.Trim();
        if (string.IsNullOrWhiteSpace(code)) return [];
        var rows = await _langRep.AsQueryable().Where(u => u.PostCode == code).ToListAsync();
        return rows.Select(u => new HrCodeLangOutput { Culture = u.Culture, Label = u.PostName }).ToList();
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "SaveLang"), HttpPost]
    [DisplayName("保存岗位代码译文")]
    public async Task SaveLang(HrPostLangSaveInput input)
    {
        var code = input.PostCode?.Trim();
        if (string.IsNullOrWhiteSpace(code))
            throw Oops.Oh("岗位代码不能为空");
        if (!await _rep.IsAnyAsync(u => u.PostCode == code))
            throw Oops.Oh("岗位不存在");

        await _langRep.DeleteAsync(u => u.PostCode == code);
        var rows = (input.Labels ?? [])
            .Select(u => new HrPostLang
            {
                PostCode = code,
                Culture = HrUiCulture.Normalize(u.Culture),
                PostName = u.Label?.Trim()
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.PostName))
            .GroupBy(u => u.Culture, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();
        if (rows.Count > 0)
            await _langRep.InsertRangeAsync(rows);
    }

    private async Task FillCcNamesAsync(IEnumerable<HrPostOutput>? items)
    {
        if (items == null) return;
        var list = items.Where(u => !string.IsNullOrWhiteSpace(u.CcCode)).ToList();
        if (list.Count == 0) return;

        var codes = list.Select(u => u.CcCode!).Distinct().ToList();
        var map = await _ccRep.AsQueryable()
            .Where(u => codes.Contains(u.CcCode))
            .Select(u => new { u.CcCode, u.CcDesc })
            .ToListAsync();
        var dict = map.ToDictionary(u => u.CcCode, u => u.CcDesc ?? u.CcCode, StringComparer.OrdinalIgnoreCase);
        foreach (var item in list)
            item.CcName = dict.TryGetValue(item.CcCode!, out var name) ? name : item.CcCode;
    }

    private static string NormalizeStatus(string? status)
    {
        var s = (status ?? HrPostConsts.StatusActive).Trim();
        if (string.Equals(s, HrPostConsts.StatusInactive, StringComparison.OrdinalIgnoreCase)
            || s is "停用" or "0" or "false")
            return HrPostConsts.StatusInactive;
        return HrPostConsts.StatusActive;
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
