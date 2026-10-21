using Furion.DatabaseAccessor;

namespace Admin.NET.Application;

/// <summary>
/// 人事组织（部门主档 cc_mstr）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrOrg", Order = 471, Description = "人事组织")]
public class HrOrgService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly HrCodeMstrService _codeMstrService;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly SqlSugarRepository<HCost> _costRep;
    private readonly SqlSugarRepository<HShiftSet> _shiftRep;

    public HrOrgService(
        UserManager userManager,
        SqlSugarRepository<CcMstr> ccRep,
        HrCodeMstrService codeMstrService,
        SqlSugarRepository<HrEmployee> empRep,
        SqlSugarRepository<HCost> costRep,
        SqlSugarRepository<HShiftSet> shiftRep)
    {
        _userManager = userManager;
        _ccRep = ccRep;
        _codeMstrService = codeMstrService;
        _empRep = empRep;
        _costRep = costRep;
        _shiftRep = shiftRep;
    }

    /// <summary>
    /// 获取部门树（默认仅启用；支持 Field/Order/DescStr 列排序）
    /// </summary>
    [DisplayName("获取人事机构列表")]
    public async Task<List<HrOrgOutput>> GetList([FromQuery] HrOrgListInput input)
    {
        input ??= new HrOrgListInput();
        var q = _ccRep.AsQueryable();
        if (!input.IncludeDisabled)
            q = q.Where(u => u.CcEnable == null || u.CcEnable == 0);

        // 树节点同级顺序由扁平查询排序决定；有 Field 时走 OrderBuilder，否则默认层级码+编码
        var list = await ApplyOrgSort(q, input).ToListAsync();

        var outputs = list.Select(ToOutput).ToList();
        await FillDirectEmpCounts(outputs, input.EmpStateAll ? null : input.EmpState);
        var tree = BuildTree(outputs, input.ParentCcCode);
        foreach (var root in tree)
            RollupEmpCount(root);
        return tree;
    }

    /// <summary>
    /// 部门明细分页（扁平列表，服务端排序）
    /// </summary>
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    [DisplayName("获取人事机构分页")]
    public async Task<SqlSugarPagedList<HrOrgOutput>> Page(HrOrgPageInput input)
    {
        var q = _ccRep.AsQueryable();
        if (!input.IncludeDisabled)
            q = q.Where(u => u.CcEnable == null || u.CcEnable == 0);

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            var kw = input.Keyword.Trim();
            q = q.Where(u => u.CcDesc.Contains(kw) || u.CcCode.Contains(kw) || (u.CcDept != null && u.CcDept.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(input.ParentCcCode))
        {
            var parent = await _ccRep.GetFirstAsync(u => u.CcCode == input.ParentCcCode)
                ?? throw Oops.Oh("父部门不存在");
            var level = parent.CcLevel ?? "";
            q = q.Where(u => u.CcLevel != null && u.CcLevel.StartsWith(level));
        }

        var page = await ApplyOrgSort(q, input).ToPagedListAsync(input.Page, input.PageSize);

        return new SqlSugarPagedList<HrOrgOutput>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            Total = page.Total,
            TotalPages = page.TotalPages,
            Items = page.Items.Select(ToOutput).ToList(),
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage
        };
    }

    /// <summary>有列排序参数时用 OrderBuilder；否则固定 CcLevel、CcCode 升序</summary>
    private static ISugarQueryable<CcMstr> ApplyOrgSort(ISugarQueryable<CcMstr> query, BasePageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Field))
            return query.OrderBy(u => u.CcLevel).OrderBy(u => u.CcCode);

        // 前端 camelCase → 实体属性 PascalCase，供 OrderBuilder/GetDbColumnName 解析
        if (char.IsLower(input.Field[0]))
            input.Field = char.ToUpperInvariant(input.Field[0]) + input.Field[1..];
        if (string.IsNullOrWhiteSpace(input.DescStr))
            input.DescStr = "desc";

        return query.OrderBuilder(input, defaultSortField: nameof(CcMstr.CcLevel), descSort: false);
    }

    /// <summary>
    /// 获取直接下级部门列表（固定 CcLevel/CcCode 序，无列头排序）
    /// </summary>
    [DisplayName("获取直接下级部门")]
    public async Task<List<HrOrgOutput>> GetChildren([FromQuery] string? parentCcCode, [FromQuery] bool includeDisabled = false)
    {
        var parentLevel = "";
        int? parentId = null;
        if (!string.IsNullOrWhiteSpace(parentCcCode))
        {
            var parent = await _ccRep.GetFirstAsync(u => u.CcCode == parentCcCode)
                ?? throw Oops.Oh("父部门不存在");
            parentLevel = parent.CcLevel ?? "";
            parentId = parent.CcIdentityId;
        }

        var expectLen = parentLevel.Length + 2;
        var q = _ccRep.AsQueryable()
            .Where(u => u.CcLevel != null && u.CcLevel.Length == expectLen);
        if (!string.IsNullOrEmpty(parentLevel))
            q = q.Where(u => u.CcLevel!.StartsWith(parentLevel));
        else
            q = q.Where(u => u.CcLevel!.Length == 2);

        if (parentId is > 0)
            q = q.Where(u => u.ParentId == parentId || u.CcCodeParent == parentCcCode);
        else if (string.IsNullOrWhiteSpace(parentCcCode))
            q = q.Where(u => u.ParentId == null || u.ParentId == 0 || u.CcCodeParent == null || u.CcCodeParent == "");

        if (!includeDisabled)
            q = q.Where(u => u.CcEnable == null || u.CcEnable == 0);

        var list = await q.OrderBy(u => u.CcLevel).OrderBy(u => u.CcCode).ToListAsync();
        return list.Select(ToOutput).ToList();
    }

    /// <summary>
    /// 新增部门
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加人事机构")]
    public async Task<string> Add(HrOrgSaveInput input)
    {
        var ccCode = string.IsNullOrWhiteSpace(input.CcCode) ? await NextCcCode() : input.CcCode.Trim();
        if (await _ccRep.IsAnyAsync(u => u.CcCode == ccCode))
            throw Oops.Oh("部门编码已存在");
        if (string.IsNullOrWhiteSpace(input.CcDept))
            throw Oops.Oh("部门代号不能为空");
        if (string.IsNullOrWhiteSpace(input.CcDesc))
            throw Oops.Oh("部门名称不能为空");

        CcMstr? parent = null;
        if (!string.IsNullOrWhiteSpace(input.CcCodeParent))
        {
            parent = await _ccRep.GetFirstAsync(u => u.CcCode == input.CcCodeParent)
                ?? throw Oops.Oh("父部门不存在");
            if (parent.CcEnable == 1)
                throw Oops.Oh("父部门已停用，不能新增下级");
        }

        var ccLevel = await BuildCcLevel(parent);
        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;
        var entity = MapSaveToEntity(input, ccCode, ccLevel, parent, userName, now, isNew: true);
        await _ccRep.InsertAsync(entity);
        return ccCode;
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新人事机构")]
    public async Task Update(HrOrgSaveInput input)
    {
        if (string.IsNullOrWhiteSpace(input.CcCode))
            throw Oops.Oh("部门编码不能为空");
        if (string.IsNullOrWhiteSpace(input.CcDept))
            throw Oops.Oh("部门代号不能为空");
        if (string.IsNullOrWhiteSpace(input.CcDesc))
            throw Oops.Oh("部门名称不能为空");

        var entity = await _ccRep.GetFirstAsync(u => u.CcCode == input.CcCode)
            ?? throw Oops.Oh("部门不存在");
        if (entity.CcEnable == 1)
            throw Oops.Oh("部门已停用，不能修改");

        var parentCode = input.CcCodeParent?.Trim();
        if (parentCode == entity.CcCode)
            throw Oops.Oh("父部门不能是自身");

        var parentChanged = !string.Equals(entity.CcCodeParent ?? "", parentCode ?? "", StringComparison.OrdinalIgnoreCase);
        CcMstr? parent = null;
        if (!string.IsNullOrWhiteSpace(parentCode))
        {
            parent = await _ccRep.GetFirstAsync(u => u.CcCode == parentCode)
                ?? throw Oops.Oh("父部门不存在");
            if (IsAncestorLevel(entity.CcLevel, parent.CcLevel))
                throw Oops.Oh("不能将部门挂到自身下级之下");
        }

        var oldLevel = entity.CcLevel ?? "";
        var newLevel = parentChanged ? await BuildCcLevel(parent) : oldLevel;
        if (parentChanged && newLevel.Length > 20)
            throw Oops.Oh("组织架构层级深度已达极限");

        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;
        ApplySaveFields(entity, input, userName, now);
        entity.CcCodeParent = parent?.CcCode;
        entity.ParentId = parent?.CcIdentityId ?? 0;
        entity.CcLevel = newLevel;
        await _ccRep.AsUpdateable(entity)
            .IgnoreColumns(u => new { u.CcIdentityId, u.CcCreateDate, u.CcCode })
            .ExecuteCommandAsync();

        if (parentChanged && oldLevel != newLevel)
            await RecalcDescendants(oldLevel, newLevel);
    }

    /// <summary>
    /// 停用部门（软删除，含子树）
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("停用人事机构")]
    public async Task Delete(HrOrgDisableInput input)
    {
        var entity = await _ccRep.GetFirstAsync(u => u.CcCode == input.CcCode)
            ?? throw Oops.Oh("部门不存在");
        if (entity.CcEnable == 1)
            return;

        var level = entity.CcLevel ?? "";
        var subtree = await _ccRep.AsQueryable()
            .Where(u => u.CcLevel != null && u.CcLevel.StartsWith(level) && (u.CcEnable == null || u.CcEnable == 0))
            .ToListAsync();
        var codes = subtree.Select(u => u.CcCode).Distinct().ToList();
        if (codes.Count == 0) return;

        // 在职(1)或停职等非离职占用则拒绝；离职通常为 3
        if (await _empRep.IsAnyAsync(u => codes.Contains(u.CcCode!) && u.EmpState != null && u.EmpState != 3))
            throw Oops.Oh("该部门或下级仍有在职/停职人员，请先调离后再停用");

        var now = DateTime.Now;
        var remark = string.IsNullOrWhiteSpace(input.Remark) ? "停用" : input.Remark.Trim();
        foreach (var row in subtree)
        {
            row.CcEnable = 1;
            row.CcDeleDate = now;
            row.CcDeleRemark = remark;
            row.CcUser = _userManager.RealName ?? _userManager.Account;
            row.CcDate = now;
        }
        await _ccRep.AsUpdateable(subtree)
            .UpdateColumns(u => new { u.CcEnable, u.CcDeleDate, u.CcDeleRemark, u.CcUser, u.CcDate })
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 合并部门
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "MergeDept"), HttpPost]
    [DisplayName("合并部门")]
    public async Task MergeDept(HrMergeDeptInput input)
    {
        if (input.OldCode1 == input.OldCode2)
            throw Oops.Oh("合并前部门一与合并前部门二不能相同");

        var newCc = await _ccRep.GetFirstAsync(u => u.CcCode == input.NewCode)
            ?? throw Oops.Oh("合并后目标部门不存在");
        if (newCc.CcEnable == 1)
            throw Oops.Oh("合并后目标部门已停用");

        var oldCodes = new[] { input.OldCode1, input.OldCode2 };
        var oldRows = new List<CcMstr>();
        foreach (var oldCode in oldCodes)
        {
            var oldCc = await _ccRep.GetFirstAsync(u => u.CcCode == oldCode)
                ?? throw Oops.Oh($"合并前部门 {oldCode} 不存在");
            if (await HasEnabledChildren(oldCc))
                throw Oops.Oh($"{oldCc.CcDesc} 下还有启用中的子部门，请先处理后再合并");
            oldRows.Add(oldCc);
        }

        await _empRep.AsUpdateable()
            .SetColumns(u => u.CcCode == input.NewCode)
            .Where(u => (u.CcCode == input.OldCode1 || u.CcCode == input.OldCode2) && (u.EmpState == null || u.EmpState != 3))
            .ExecuteCommandAsync();

        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;
        foreach (var oldCc in oldRows)
        {
            if (oldCc.CcCode == input.NewCode) continue;
            var other = oldRows.First(u => u.CcCode != oldCc.CcCode);
            oldCc.CcEnable = 1;
            oldCc.CcDeleDate = now;
            oldCc.CcDeleRemark = $"与{other.CcDesc}合并为{newCc.CcDesc}";
            oldCc.CcUser = userName;
            oldCc.CcDate = now;
            await _ccRep.AsUpdateable(oldCc)
                .UpdateColumns(u => new { u.CcEnable, u.CcDeleDate, u.CcDeleRemark, u.CcUser, u.CcDate })
                .ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 调动部门：整棵子树改挂父级并重算层级码；原位置写入停用历史镜像（新编码）
    /// </summary>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "TransferDept"), HttpPost]
    [DisplayName("调动部门")]
    public async Task TransferDept(HrTransferDeptInput input)
    {
        var fromCc = await _ccRep.GetFirstAsync(u => u.CcCode == input.FromCcCode)
            ?? throw Oops.Oh("调动部门不存在");
        if (fromCc.CcEnable == 1)
            throw Oops.Oh("调动部门已停用");

        CcMstr? toParent = null;
        if (!string.IsNullOrWhiteSpace(input.ToParentCcCode))
        {
            if (input.ToParentCcCode == input.FromCcCode)
                throw Oops.Oh("调动的部门前后一致，不能调往");
            toParent = await _ccRep.GetFirstAsync(u => u.CcCode == input.ToParentCcCode)
                ?? throw Oops.Oh("调至归属部门不存在");
            if (toParent.CcEnable == 1)
                throw Oops.Oh("调至归属部门已停用");
            if (IsAncestorLevel(fromCc.CcLevel, toParent.CcLevel))
                throw Oops.Oh("不能调往子部门");
        }

        var oldPrefix = fromCc.CcLevel ?? "";
        if (string.IsNullOrEmpty(oldPrefix))
            throw Oops.Oh("调动部门层级码无效");

        var newPrefix = await BuildCcLevel(toParent);
        if (newPrefix.Length > 20)
            throw Oops.Oh("组织架构层级深度已达极限，不能调往");

        // 改挂前快照原子树（含自身）；历史行在层级改写后再插入，避免被 RecalcDescendants 误伤
        var subtree = await _ccRep.AsQueryable()
            .Where(u => u.CcLevel != null && u.CcLevel.StartsWith(oldPrefix))
            .OrderBy(u => u.CcLevel)
            .ToListAsync();
        if (subtree.Count == 0)
            throw Oops.Oh("未找到可调动的部门子树");

        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;
        var toDesc = toParent?.CcDesc ?? "根级";
        var historyRemark = $"被调至{toDesc}";

        fromCc.CcLevel = newPrefix;
        fromCc.CcCodeParent = toParent?.CcCode;
        fromCc.ParentId = toParent?.CcIdentityId ?? 0;
        fromCc.CcUser = userName;
        fromCc.CcDate = now;
        await _ccRep.AsUpdateable(fromCc)
            .UpdateColumns(u => new { u.CcLevel, u.CcCodeParent, u.ParentId, u.CcUser, u.CcDate })
            .ExecuteCommandAsync();

        if (oldPrefix != newPrefix)
            await RecalcDescendants(oldPrefix, newPrefix);

        await InsertTransferHistoryMirrors(subtree, historyRemark, userName, now);
    }

    /// <summary>
    /// 调动后写入历史部门镜像：新 cc_code、停用、保留调动前层级与挂接，备注「被调至…」
    /// </summary>
    private async Task InsertTransferHistoryMirrors(List<CcMstr> subtreeSnapshot, string remark, string userName, DateTime now)
    {
        var newCodes = await NextCcCodes(subtreeSnapshot.Count);
        var codeMap = subtreeSnapshot
            .Select((row, i) => (Old: row.CcCode, New: newCodes[i]))
            .ToDictionary(x => x.Old, x => x.New, StringComparer.OrdinalIgnoreCase);

        var mirrors = new List<CcMstr>(subtreeSnapshot.Count);
        foreach (var src in subtreeSnapshot)
        {
            var srcParent = src.CcCodeParent;
            var parentInSubtree = !string.IsNullOrWhiteSpace(srcParent) && codeMap.ContainsKey(srcParent);
            var parentCode = parentInSubtree ? codeMap[srcParent!] : srcParent;

            mirrors.Add(new CcMstr
            {
                CcCode = codeMap[src.CcCode],
                CcCodeParent = parentCode,
                CcDept = src.CcDept,
                CcDesc = src.CcDesc,
                CcLevel = src.CcLevel,
                CcGroup = src.CcGroup,
                CcIndet = src.CcIndet,
                CcSub = src.CcSub,
                CcSite = src.CcSite,
                CcState = src.CcState,
                CcCost = src.CcCost,
                CcWageRate = src.CcWageRate,
                CcStatBasic = src.CcStatBasic,
                CcEnable = 1,
                CcDeleDate = now,
                CcCountGroup = src.CcCountGroup,
                CcUser = userName,
                CcDate = now,
                CcKqgroup = src.CcKqgroup,
                CcWoca = src.CcWoca,
                CcBzUnit = src.CcBzUnit,
                CcDeleRemark = remark,
                CcDescEn = src.CcDescEn,
                CcLevelCensor = src.CcLevelCensor,
                CcXzGroup = src.CcXzGroup,
                CcCreateDate = src.CcCreateDate,
                MachineFloor = src.MachineFloor,
                CcCode2 = src.CcCode2,
                // 子树内父级插入后回填；树挂接以 CcCodeParent 为准
                ParentId = parentInSubtree ? 0 : src.ParentId
            });
        }

        await _ccRep.AsInsertable(mirrors).ExecuteCommandAsync();

        // 回填历史子树内部 ParentId（自增 ID 已生成）
        var inserted = await _ccRep.AsQueryable()
            .Where(u => newCodes.Contains(u.CcCode))
            .ToListAsync();
        var idByCode = inserted.ToDictionary(u => u.CcCode, u => u.CcIdentityId, StringComparer.OrdinalIgnoreCase);
        var needParentFix = new List<CcMstr>();
        foreach (var row in inserted)
        {
            if (string.IsNullOrWhiteSpace(row.CcCodeParent)) continue;
            if (!idByCode.TryGetValue(row.CcCodeParent, out var pid)) continue;
            if (row.ParentId == pid) continue;
            row.ParentId = pid;
            needParentFix.Add(row);
        }
        if (needParentFix.Count > 0)
        {
            await _ccRep.AsUpdateable(needParentFix)
                .UpdateColumns(u => new { u.ParentId })
                .ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 按工号取人事姓名（账号关联带出，只读不落库）
    /// </summary>
    [DisplayName("按工号获取人事姓名")]
    public async Task<HrEmpBriefOutput?> GetEmpById([FromQuery] string empId)
    {
        if (string.IsNullOrWhiteSpace(empId)) return null;
        var id = empId.Trim();
        var emp = await _empRep.GetFirstAsync(u => u.EmpId == id);
        if (emp == null) return null;
        return new HrEmpBriefOutput { EmpId = emp.EmpId, EmpName = emp.EmpName };
    }

    /// <summary>
    /// 人事代码下拉
    /// </summary>
    [DisplayName("人事代码下拉")]
    public Task<List<HrCodeItemOutput>> GetCodeList([FromQuery] string codeField)
    {
        return _codeMstrService.GetCodeList(codeField);
    }

    /// <summary>
    /// 成本中心下拉
    /// </summary>
    [DisplayName("成本中心下拉")]
    public async Task<List<HrCostItemOutput>> GetCostCenterList([FromQuery] string? site)
    {
        var q = _costRep.AsQueryable().Where(u => u.CostState == "Y");
        if (!string.IsNullOrWhiteSpace(site))
            q = q.Where(u => u.CostSite == site);
        var list = await q.OrderBy(u => u.CostName).ToListAsync();
        return list.Select(u => new HrCostItemOutput
        {
            Value = u.CostId,
            Label = string.IsNullOrWhiteSpace(u.CostName) ? u.CostId : $"{u.CostId} {u.CostName}"
        }).ToList();
    }

    /// <summary>
    /// 考勤组别下拉
    /// </summary>
    [DisplayName("考勤组别下拉")]
    public async Task<List<HrShiftItemOutput>> GetShiftGroupList([FromQuery] string? tpkSite)
    {
        var q = _shiftRep.AsQueryable();
        if (!string.IsNullOrWhiteSpace(tpkSite))
            q = q.Where(u => u.ShiftTpkSite == tpkSite);
        var list = await q.OrderBy(u => u.ShiftDesc).ToListAsync();
        return list.Select(u => new HrShiftItemOutput
        {
            Value = u.ShiftId,
            Label = string.IsNullOrWhiteSpace(u.ShiftDesc) ? u.ShiftId : $"{u.ShiftId} {u.ShiftDesc}"
        }).ToList();
    }

    private async Task<bool> HasEnabledChildren(CcMstr cc)
    {
        var level = cc.CcLevel ?? "";
        return await _ccRep.IsAnyAsync(u =>
            u.CcCode != cc.CcCode
            && u.CcLevel != null
            && u.CcLevel.StartsWith(level)
            && u.CcLevel.Length > level.Length
            && (u.CcEnable == null || u.CcEnable == 0));
    }

    private async Task<string> NextCcCode()
    {
        var list = await NextCcCodes(1);
        return list[0];
    }

    private async Task<List<string>> NextCcCodes(int count)
    {
        if (count <= 0) return [];
        var codes = await _ccRep.AsQueryable().Select(u => u.CcCode).ToListAsync();
        var max = codes.Select(s => int.TryParse(s, out var n) ? n : 0).DefaultIfEmpty(0).Max();
        return Enumerable.Range(1, count).Select(i => (max + i).ToString("000000")).ToList();
    }

    private async Task<string> BuildCcLevel(CcMstr? parent)
    {
        var parentLevel = parent?.CcLevel ?? "";
        var expectLen = parentLevel.Length + 2;
        var siblings = await _ccRep.AsQueryable()
            .Where(u => u.CcLevel != null && u.CcLevel.Length == expectLen)
            .WhereIF(!string.IsNullOrEmpty(parentLevel), u => u.CcLevel!.StartsWith(parentLevel))
            .Select(u => u.CcLevel)
            .ToListAsync();

        var maxSuffix = 0;
        foreach (var level in siblings)
        {
            if (string.IsNullOrEmpty(level) || level.Length < 2) continue;
            var suffix = level[^2..];
            if (int.TryParse(suffix, out var n) && n > maxSuffix)
                maxSuffix = n;
        }

        if (maxSuffix >= 99)
            throw Oops.Oh("同级部门数量已达上限");

        return parentLevel + (maxSuffix + 1).ToString("00");
    }

    private async Task RecalcDescendants(string oldPrefix, string newPrefix)
    {
        var children = await _ccRep.AsQueryable()
            .Where(u => u.CcLevel != null && u.CcLevel.StartsWith(oldPrefix) && u.CcLevel.Length > oldPrefix.Length)
            .ToListAsync();
        if (children.Count == 0) return;

        foreach (var child in children.OrderBy(u => (u.CcLevel ?? "").Length))
        {
            var suffix = (child.CcLevel ?? "")[oldPrefix.Length..];
            child.CcLevel = newPrefix + suffix;
            if (child.CcLevel.Length >= 2)
            {
                var parentLevel = child.CcLevel[..^2];
                var parent = await _ccRep.GetFirstAsync(u => u.CcLevel == parentLevel);
                child.CcCodeParent = parent?.CcCode;
                child.ParentId = parent?.CcIdentityId ?? 0;
            }
        }

        await _ccRep.AsUpdateable(children)
            .UpdateColumns(u => new { u.CcLevel, u.CcCodeParent, u.ParentId })
            .ExecuteCommandAsync();
    }

    /// <summary>判断 candidate 是否为 node 的子孙层级（或自身）</summary>
    private static bool IsAncestorLevel(string? nodeLevel, string? candidateLevel)
    {
        if (string.IsNullOrEmpty(nodeLevel) || string.IsNullOrEmpty(candidateLevel))
            return false;
        return candidateLevel.StartsWith(nodeLevel, StringComparison.Ordinal);
    }

    private static CcMstr MapSaveToEntity(HrOrgSaveInput input, string ccCode, string ccLevel, CcMstr? parent, string userName, DateTime now, bool isNew)
    {
        var entity = new CcMstr
        {
            CcCode = ccCode,
            CcLevel = ccLevel,
            CcCodeParent = parent?.CcCode,
            ParentId = parent?.CcIdentityId ?? 0,
            CcEnable = 0,
            CcUser = userName,
            CcDate = now,
            CcCreateDate = isNew ? now : null
        };
        ApplySaveFields(entity, input, userName, now);
        // 新增时继承上级厂别/工时等
        if (isNew && parent != null)
        {
            entity.CcSite ??= parent.CcSite;
            entity.CcWoca ??= parent.CcWoca;
            entity.CcBzUnit ??= parent.CcBzUnit;
            entity.CcStatBasic ??= parent.CcStatBasic;
        }
        return entity;
    }

    private static void ApplySaveFields(CcMstr entity, HrOrgSaveInput input, string userName, DateTime now)
    {
        entity.CcDesc = input.CcDesc.Trim();
        entity.CcDept = input.CcDept?.Trim();
        entity.CcDescEn = input.CcDescEn;
        entity.CcGroup = input.CcGroup;
        entity.CcIndet = input.CcIndet;
        entity.CcSub = input.CcSub;
        entity.CcSite = input.CcSite;
        entity.CcState = input.CcState;
        entity.CcCost = input.CcCost;
        entity.CcWageRate = input.CcWageRate;
        entity.CcStatBasic = input.CcStatBasic;
        entity.CcCountGroup = input.CcCountGroup;
        entity.CcKqgroup = input.CcKqgroup;
        entity.CcWoca = input.CcWoca;
        entity.CcBzUnit = input.CcBzUnit;
        entity.CcLevelCensor = input.CcLevelCensor;
        entity.CcXzGroup = input.CcXzGroup;
        entity.MachineFloor = input.MachineFloor;
        entity.CcCode2 = input.CcCode2;
        entity.CcUser = userName;
        entity.CcDate = now;
    }

    private static HrOrgOutput ToOutput(CcMstr cc)
    {
        return new HrOrgOutput
        {
            Id = cc.CcIdentityId,
            Pid = cc.ParentId ?? 0,
            CcCode = cc.CcCode,
            CcCodeParent = cc.CcCodeParent,
            CcDept = cc.CcDept,
            CcDesc = cc.CcDesc,
            CcDescEn = cc.CcDescEn,
            CcLevel = cc.CcLevel,
            CcGroup = cc.CcGroup,
            CcIndet = cc.CcIndet,
            CcSub = cc.CcSub,
            CcSite = cc.CcSite,
            CcState = cc.CcState,
            CcCost = cc.CcCost,
            CcWageRate = cc.CcWageRate,
            CcStatBasic = cc.CcStatBasic,
            CcEnable = cc.CcEnable,
            CcDeleDate = cc.CcDeleDate,
            CcCountGroup = cc.CcCountGroup,
            CcUser = cc.CcUser,
            CcDate = cc.CcDate,
            CcKqgroup = cc.CcKqgroup,
            CcWoca = cc.CcWoca,
            CcBzUnit = cc.CcBzUnit,
            CcDeleRemark = cc.CcDeleRemark,
            CcLevelCensor = cc.CcLevelCensor,
            CcXzGroup = cc.CcXzGroup,
            CcCreateDate = cc.CcCreateDate,
            MachineFloor = cc.MachineFloor,
            KqNewSysuser = cc.KqNewSysuser,
            KqNewSysdate = cc.KqNewSysdate,
            CcCode2 = cc.CcCode2,
            Children = []
        };
    }

    private async Task FillDirectEmpCounts(List<HrOrgOutput> outputs, int? empState)
    {
        if (outputs.Count == 0) return;

        // 服务端 GROUP BY，避免把全表 cc_code 拉进内存（切语言整页刷新时会并发多次 GetList）
        var empQ = _empRep.AsQueryable().Where(u => u.CcCode != null && u.CcCode != "");
        if (empState.HasValue)
            empQ = empQ.Where(u => u.EmpState == empState.Value);

        var rows = await empQ
            .GroupBy(u => u.CcCode)
            .Select(u => new { CcCode = u.CcCode, Cnt = SqlFunc.AggregateCount(u.CcCode) })
            .ToListAsync();
        var map = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.CcCode))
            .ToDictionary(r => r.CcCode!, r => r.Cnt, StringComparer.OrdinalIgnoreCase);
        foreach (var node in outputs)
            node.EmpCount = map.TryGetValue(node.CcCode, out var c) ? c : 0;
    }

    /// <summary>将本节点直接人数与子树人数汇总到 EmpCount</summary>
    private static int RollupEmpCount(HrOrgOutput node)
    {
        var sum = node.EmpCount;
        foreach (var child in node.Children)
            sum += RollupEmpCount(child);
        node.EmpCount = sum;
        return sum;
    }

    private static List<HrOrgOutput> BuildTree(List<HrOrgOutput> flat, string? rootParentCode)
    {
        var byCode = flat.Where(u => !string.IsNullOrWhiteSpace(u.CcCode))
            .GroupBy(u => u.CcCode)
            .ToDictionary(u => u.Key, u => u.First());
        var roots = new List<HrOrgOutput>();

        foreach (var node in flat)
        {
            var parentCode = node.CcCodeParent;
            if (string.IsNullOrWhiteSpace(rootParentCode))
            {
                if (string.IsNullOrWhiteSpace(parentCode) || !byCode.ContainsKey(parentCode))
                    roots.Add(node);
                else
                    byCode[parentCode].Children.Add(node);
            }
            else
            {
                if (string.Equals(parentCode, rootParentCode, StringComparison.OrdinalIgnoreCase))
                    roots.Add(node);
                else if (!string.IsNullOrWhiteSpace(parentCode) && byCode.TryGetValue(parentCode, out var p))
                    p.Children.Add(node);
            }
        }

        return roots;
    }
}
