using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 审批角色（管辖部门 + 岗位）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrWfRole", Order = 481, Description = "审批角色")]
public class HrWfRoleService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<HrWfRole> _rep;
    private readonly SqlSugarRepository<HrWfRoleScope> _scopeRep;
    private readonly SqlSugarRepository<HrWfRolePost> _postRep;
    private readonly SqlSugarRepository<HrWfLevel> _levelRep;

    public HrWfRoleService(
        SqlSugarRepository<HrWfRole> rep,
        SqlSugarRepository<HrWfRoleScope> scopeRep,
        SqlSugarRepository<HrWfRolePost> postRep,
        SqlSugarRepository<HrWfLevel> levelRep)
    {
        _rep = rep;
        _scopeRep = scopeRep;
        _postRep = postRep;
        _levelRep = levelRep;
    }

    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("审批角色分页")]
    public async Task<SqlSugarPagedList<HrWfRoleOutput>> Page(HrWfRolePageInput input)
    {
        input ??= new HrWfRolePageInput();
        var keyword = input.Keyword?.Trim();
        var query = _rep.AsQueryable()
            .LeftJoin<HrWfLevel>((r, l) => r.LevelId == l.Id)
            .WhereIF(input.LevelId.HasValue, (r, l) => r.LevelId == input.LevelId)
            .WhereIF(input.ParentId.HasValue, (r, l) => r.ParentId == input.ParentId)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), (r, l) => r.Code.Contains(keyword!) || r.Name.Contains(keyword!))
            .WhereIF(input.Enabled.HasValue, (r, l) => r.Enabled == input.Enabled)
            .Select((r, l) => new HrWfRoleOutput
            {
                Id = r.Id,
                LevelId = r.LevelId,
                LevelName = l.Name,
                Code = r.Code,
                Name = r.Name,
                ParentId = r.ParentId,
                OrderNo = r.OrderNo,
                Enabled = r.Enabled,
                Remark = r.Remark
            });

        NormalizeSort(input);
        return await query
            .MergeTable()
            .OrderBuilder(input, defaultSortField: nameof(HrWfRoleOutput.OrderNo), descSort: false)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [ApiDescriptionSettings(Name = "Tree"), HttpGet]
    [DisplayName("审批角色汇报树")]
    public async Task<List<HrWfRoleTreeOutput>> Tree()
    {
        var rows = await _rep.AsQueryable()
            .LeftJoin<HrWfLevel>((r, l) => r.LevelId == l.Id)
            .OrderBy((r, l) => r.OrderNo)
            .Select((r, l) => new HrWfRoleTreeOutput
            {
                Id = r.Id,
                ParentId = r.ParentId,
                Name = r.Name,
                Code = r.Code,
                LevelId = r.LevelId,
                LevelName = l.Name,
                OrderNo = r.OrderNo,
                Enabled = r.Enabled
            })
            .ToListAsync();

        foreach (var row in rows.Where(u => u.LevelId is not > 0))
            row.LevelName = "（组织节点）";

        var map = rows.ToDictionary(u => u.Id);
        var roots = new List<HrWfRoleTreeOutput>();
        foreach (var row in rows)
        {
            if (row.ParentId is > 0 && map.TryGetValue(row.ParentId.Value, out var parent))
                parent.Children.Add(row);
            else
                roots.Add(row);
        }
        return roots;
    }

    [ApiDescriptionSettings(Name = "Get"), HttpGet]
    [DisplayName("审批角色详情")]
    public async Task<HrWfRoleOutput> Get([FromQuery] long id)
    {
        var role = await _rep.GetFirstAsync(u => u.Id == id) ?? throw Oops.Oh(HrWfErrorCodeEnum.RoleNotFound);
        var level = role.LevelId is > 0
            ? await _levelRep.GetFirstAsync(u => u.Id == role.LevelId)
            : null;
        var scopes = await _scopeRep.AsQueryable()
            .Where(u => u.RoleId == id)
            .Select(u => new HrWfRoleScopeItem { CcCode = u.CcCode, IncludeChild = u.IncludeChild })
            .ToListAsync();
        var postIds = await _postRep.AsQueryable()
            .Where(u => u.RoleId == id)
            .Select(u => u.PostId)
            .ToListAsync();

        return new HrWfRoleOutput
        {
            Id = role.Id,
            LevelId = role.LevelId,
            LevelName = role.LevelId is > 0 ? level?.Name : "（组织节点）",
            Code = role.Code,
            Name = role.Name,
            ParentId = role.ParentId,
            OrderNo = role.OrderNo,
            Enabled = role.Enabled,
            Remark = role.Remark,
            Scopes = scopes,
            PostIds = postIds
        };
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Save"), HttpPost]
    [DisplayName("保存审批角色")]
    public async Task<long> Save(HrWfRoleSaveInput input)
    {
        var isVirtual = input.LevelId is not > 0;
        if (!isVirtual && !await _levelRep.IsAnyAsync(u => u.Id == input.LevelId))
            throw Oops.Oh(HrWfErrorCodeEnum.LevelNotFound);

        if (input.ParentId is > 0 && !await _rep.IsAnyAsync(u => u.Id == input.ParentId))
            throw Oops.Oh(HrWfErrorCodeEnum.RoleNotFound);

        var code = input.Code.Trim();
        var name = input.Name.Trim();
        long? levelId = isVirtual ? null : input.LevelId;
        // 虚拟节点禁止绑岗位/管辖（不参与审核）
        var scopes = isVirtual ? [] : input.Scopes;
        var postIds = isVirtual ? [] : input.PostIds;
        long roleId;

        if (input.Id is > 0)
        {
            var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(HrWfErrorCodeEnum.RoleNotFound);
            var dup = await _rep.IsAnyAsync(u => u.LevelId == levelId && u.Code == code && u.Id != entity.Id);
            if (dup) throw Oops.Oh(HrWfErrorCodeEnum.RoleCodeDup);

            await AssertTreeConstraintsAsync(entity.Id, input.ParentId, levelId);

            entity.LevelId = levelId;
            entity.Code = code;
            entity.Name = name;
            entity.ParentId = input.ParentId;
            entity.OrderNo = input.OrderNo;
            entity.Enabled = input.Enabled;
            entity.Remark = input.Remark?.Trim();
            await _rep.UpdateAsync(entity);
            roleId = entity.Id;
        }
        else
        {
            var dup = await _rep.IsAnyAsync(u => u.LevelId == levelId && u.Code == code);
            if (dup) throw Oops.Oh(HrWfErrorCodeEnum.RoleCodeDup);

            await AssertTreeConstraintsAsync(0, input.ParentId, levelId);

            var inserted = await _rep.InsertReturnEntityAsync(new HrWfRole
            {
                LevelId = levelId,
                Code = code,
                Name = name,
                ParentId = input.ParentId,
                OrderNo = input.OrderNo,
                Enabled = input.Enabled,
                Remark = input.Remark?.Trim()
            });
            roleId = inserted.Id;
        }

        await ReplaceScopesAsync(roleId, scopes);
        await ReplacePostsAsync(roleId, postIds);
        return roleId;
    }

    private async Task AssertTreeConstraintsAsync(long nodeId, long? newParentId, long? newLevelId)
    {
        var all = await _rep.AsQueryable().Select(u => new { u.Id, u.ParentId, u.LevelId }).ToListAsync();
        var byId = all.ToDictionary(
            u => u.Id,
            u => new HrWfRoleNode(u.Id, u.ParentId, u.LevelId, 0));

        // 新增节点不在 byId：用占位加入以便 CollectPathLevelIds
        if (nodeId == 0 || !byId.ContainsKey(nodeId))
        {
            var tempId = nodeId == 0 ? -1L : nodeId;
            byId[tempId] = new HrWfRoleNode(tempId, newParentId, newLevelId, 0);
            try
            {
                HrWfRoleTreeValidation.AssertSaveValid(tempId, newParentId, newLevelId, byId);
            }
            catch (InvalidOperationException ex)
            {
                throw HrWfErrors.Oh(ex.Message);
            }
            return;
        }

        try
        {
            HrWfRoleTreeValidation.AssertSaveValid(nodeId, newParentId, newLevelId, byId);
        }
        catch (InvalidOperationException ex)
        {
            throw HrWfErrors.Oh(ex.Message);
        }
    }

    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除审批角色")]
    public async Task Delete(HrWfRoleDeleteInput input)
    {
        if (!await _rep.IsAnyAsync(u => u.Id == input.Id))
            throw Oops.Oh(HrWfErrorCodeEnum.RoleNotFound);

        await _scopeRep.AsDeleteable().Where(u => u.RoleId == input.Id).ExecuteCommandAsync();
        await _postRep.AsDeleteable().Where(u => u.RoleId == input.Id).ExecuteCommandAsync();
        await _rep.FakeDeleteAsync(input.Id);
    }

    private async Task ReplaceScopesAsync(long roleId, List<HrWfRoleScopeItem>? scopes)
    {
        await _scopeRep.AsDeleteable().Where(u => u.RoleId == roleId).ExecuteCommandAsync();
        var rows = (scopes ?? [])
            .Where(u => !string.IsNullOrWhiteSpace(u.CcCode))
            .GroupBy(u => u.CcCode.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var last = g.Last();
                return new HrWfRoleScope
                {
                    RoleId = roleId,
                    CcCode = g.Key,
                    IncludeChild = last.IncludeChild
                };
            })
            .ToList();
        if (rows.Count > 0)
            await _scopeRep.InsertRangeAsync(rows);
    }

    private async Task ReplacePostsAsync(long roleId, List<string>? postIds)
    {
        await _postRep.AsDeleteable().Where(u => u.RoleId == roleId).ExecuteCommandAsync();
        var ids = (postIds ?? [])
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (ids.Count == 0) return;
        await _postRep.InsertRangeAsync(ids.Select(pid => new HrWfRolePost { RoleId = roleId, PostId = pid }).ToList());
    }

    private static void NormalizeSort(BasePageInput input)
    {
        if (!string.IsNullOrWhiteSpace(input.Field) && char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";
    }
}
