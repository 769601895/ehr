using System.Text.Json;
using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>高级查询方案（个人 + 同角色分享）</summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrAdvQueryScheme", Order = 468, Description = "高级查询方案")]
public class HrAdvQuerySchemeService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrAdvQueryScheme> _rep;
    private readonly UserManager _userManager;

    public HrAdvQuerySchemeService(SqlSugarRepository<HrAdvQueryScheme> rep, UserManager userManager)
    {
        _rep = rep;
        _userManager = userManager;
    }

    [ApiDescriptionSettings(Name = "List"), HttpGet]
    [DisplayName("高级查询方案列表")]
    public async Task<List<HrAdvQuerySchemeOutput>> List([FromQuery] string moduleCode)
    {
        if (string.IsNullOrWhiteSpace(moduleCode)) throw Oops.Oh("模块码不能为空");
        var mod = moduleCode.Trim();
        var uid = _userManager.UserId;
        var roleIds = _userManager.RoleIds ?? [];

        var rows = await _rep.AsQueryable()
            .Where(u => u.ModuleCode == mod)
            .Where(u => u.OwnerUserId == uid || u.IsShared)
            .OrderBy(u => u.Name)
            .ToListAsync();

        return rows
            .Where(u => u.OwnerUserId == uid || RoleIntersect(u.ShareRoleIds, roleIds))
            .Select(u => ToOutput(u, uid))
            .ToList();
    }

    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    [DisplayName("高级查询方案详情")]
    public async Task<HrAdvQuerySchemeOutput> Detail([FromQuery] long id)
    {
        var row = await GetVisibleAsync(id);
        return ToOutput(row, _userManager.UserId);
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存高级查询方案")]
    public async Task<long> Save(HrAdvQuerySchemeSaveInput input)
    {
        if (input == null) throw Oops.Oh("参数无效");
        if (string.IsNullOrWhiteSpace(input.ModuleCode)) throw Oops.Oh("模块码不能为空");
        if (string.IsNullOrWhiteSpace(input.Name)) throw Oops.Oh("方案名称不能为空");
        if (string.IsNullOrWhiteSpace(input.FilterJson)) throw Oops.Oh("条件不能为空");

        ValidateFilterJson(input.FilterJson);
        var uid = _userManager.UserId;
        var shareJson = SerializeRoles(input.IsShared ? input.ShareRoleIds : null);

        if (input.Id is > 0)
        {
            var row = await _rep.GetFirstAsync(u => u.Id == input.Id.Value)
                ?? throw Oops.Oh("方案不存在");
            if (row.OwnerUserId != uid) throw Oops.Oh("仅所有者可修改方案");
            row.Name = input.Name.Trim();
            row.FilterJson = input.FilterJson.Trim();
            row.IsShared = input.IsShared;
            row.ShareRoleIds = shareJson;
            await _rep.UpdateAsync(row);
            return row.Id;
        }

        var entity = new HrAdvQueryScheme
        {
            Id = YitIdHelper.NextId(),
            ModuleCode = input.ModuleCode.Trim(),
            Name = input.Name.Trim(),
            FilterJson = input.FilterJson.Trim(),
            OwnerUserId = uid,
            IsShared = input.IsShared,
            ShareRoleIds = shareJson
        };
        await _rep.InsertAsync(entity);
        return entity.Id;
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除高级查询方案")]
    public async Task Delete(HrAdvQuerySchemeIdInput input)
    {
        var row = await _rep.GetFirstAsync(u => u.Id == input.Id)
            ?? throw Oops.Oh("方案不存在");
        if (row.OwnerUserId != _userManager.UserId) throw Oops.Oh("仅所有者可删除方案");
        await _rep.DeleteAsync(row);
    }

    private async Task<HrAdvQueryScheme> GetVisibleAsync(long id)
    {
        var row = await _rep.GetFirstAsync(u => u.Id == id)
            ?? throw Oops.Oh("方案不存在");
        var uid = _userManager.UserId;
        if (row.OwnerUserId == uid) return row;
        if (row.IsShared && RoleIntersect(row.ShareRoleIds, _userManager.RoleIds ?? []))
            return row;
        throw Oops.Oh("无权查看该方案");
    }

    private static void ValidateFilterJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
                throw Oops.Oh("条件格式无效");
        }
        catch (JsonException)
        {
            throw Oops.Oh("条件格式无效");
        }
    }

    private static string? SerializeRoles(List<long>? roles)
    {
        if (roles is not { Count: > 0 }) return null;
        return JsonSerializer.Serialize(roles.Distinct().ToList());
    }

    private static List<long> ParseRoles(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<long>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static bool RoleIntersect(string? shareJson, List<long> myRoles)
    {
        if (myRoles.Count == 0) return false;
        var share = ParseRoles(shareJson);
        if (share.Count == 0) return false;
        return share.Any(myRoles.Contains);
    }

    private static HrAdvQuerySchemeOutput ToOutput(HrAdvQueryScheme u, long uid) => new()
    {
        Id = u.Id,
        ModuleCode = u.ModuleCode,
        Name = u.Name,
        FilterJson = u.FilterJson,
        OwnerUserId = u.OwnerUserId,
        IsShared = u.IsShared,
        ShareRoleIds = ParseRoles(u.ShareRoleIds),
        IsOwner = u.OwnerUserId == uid,
        UpdateTime = u.UpdateTime
    };
}
