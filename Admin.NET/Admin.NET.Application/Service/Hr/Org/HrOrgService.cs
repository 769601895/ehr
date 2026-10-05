namespace Admin.NET.Application;

/// <summary>
/// 人事组织（SysOrg 结构 + cc_mstr 扩展）
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Name = "hrOrg", Order = 471, Description = "人事组织")]
public class HrOrgService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SysOrgService _sysOrgService;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SqlSugarRepository<CcMstr> _ccRep;
    private readonly SqlSugarRepository<CodeMstr> _codeRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;

    public HrOrgService(
        UserManager userManager,
        SysOrgService sysOrgService,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SqlSugarRepository<CcMstr> ccRep,
        SqlSugarRepository<CodeMstr> codeRep,
        SqlSugarRepository<HrEmployee> empRep)
    {
        _userManager = userManager;
        _sysOrgService = sysOrgService;
        _sysOrgRep = sysOrgRep;
        _ccRep = ccRep;
        _codeRep = codeRep;
        _empRep = empRep;
    }

    /// <summary>
    /// 获取机构列表（含 cc_mstr 全字段）
    /// </summary>
    [DisplayName("获取人事机构列表")]
    public async Task<List<HrOrgOutput>> GetList([FromQuery] OrgInput input)
    {
        var tree = await _sysOrgService.GetList(input);
        var ccList = await TryGetCcList();
        var ccMap = ccList
            .Where(u => !string.IsNullOrWhiteSpace(u.CcCode))
            .GroupBy(u => u.CcCode)
            .ToDictionary(u => u.Key, u => u.First());
        return tree.Select(u => ToOutput(u, ccMap)).ToList();
    }

    /// <summary>
    /// 增加机构并写入 cc_mstr
    /// </summary>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加人事机构")]
    public async Task<long> Add(HrOrgSaveInput input)
    {
        await EnsureOrgIdColumn();
        var ccCode = string.IsNullOrWhiteSpace(input.Code) ? await NextCcCode() : input.Code.Trim();
        if (await _ccRep.IsAnyAsync(u => u.CcCode == ccCode))
            throw Oops.Oh("部门编码已存在");

        var ccLevel = await BuildCcLevel(input.Pid, null);
        var parentCc = await GetParentCc(input.Pid);
        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;

        var orgId = await _sysOrgService.AddOrg(ToAddOrgInput(input, ccCode, ccLevel));
        var entity = ToCcEntity(input, ccCode, ccLevel, parentCc, orgId, userName, now, isNew: true);
        await _ccRep.InsertAsync(entity);
        return orgId;
    }

    /// <summary>
    /// 更新机构并同步 cc_mstr
    /// </summary>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新人事机构")]
    public async Task Update(HrOrgSaveInput input)
    {
        await EnsureOrgIdColumn();
        var org = await _sysOrgRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var oldCode = org.Code;
        var ccCode = string.IsNullOrWhiteSpace(input.Code) ? oldCode : input.Code.Trim();
        if (string.IsNullOrWhiteSpace(ccCode))
            throw Oops.Oh("部门编码不能为空");
        if (ccCode != oldCode && await _ccRep.IsAnyAsync(u => u.CcCode == ccCode))
            throw Oops.Oh("部门编码已存在");

        var pidChanged = org.Pid != input.Pid;
        var ccLevel = pidChanged ? await BuildCcLevel(input.Pid, input.Id) : (await GetCcByOrg(org))?.CcLevel;
        ccLevel ??= await BuildCcLevel(input.Pid, input.Id);

        var oldCc = await GetCcByOrg(org);
        var oldPrefix = oldCc?.CcLevel;

        await _sysOrgService.UpdateOrg(ToUpdateOrgInput(input, ccCode, ccLevel));

        var parentCc = await GetParentCc(input.Pid);
        var now = DateTime.Now;
        var userName = _userManager.RealName ?? _userManager.Account;
        var entity = ToCcEntity(input, ccCode, ccLevel, parentCc, input.Id, userName, now, isNew: oldCc == null);
        if (oldCc == null)
        {
            await _ccRep.InsertAsync(entity);
        }
        else if (!string.IsNullOrWhiteSpace(oldCode) && oldCode != ccCode)
        {
            entity.CcCreateDate = oldCc.CcCreateDate ?? entity.CcCreateDate;
            await _ccRep.DeleteAsync(u => u.CcCode == oldCode);
            await _ccRep.InsertAsync(entity);
        }
        else
        {
            entity.CcIdentityId = oldCc.CcIdentityId;
            entity.CcCreateDate = oldCc.CcCreateDate ?? entity.CcCreateDate;
            await _ccRep.AsUpdateable(entity).IgnoreColumns(u => new { u.CcIdentityId }).ExecuteCommandAsync();
        }

        if (pidChanged && !string.IsNullOrWhiteSpace(oldPrefix) && oldPrefix != ccLevel)
            await RecalcDescendants(input.Id, oldPrefix, ccLevel);
    }

    /// <summary>
    /// 删除机构及 cc_mstr
    /// </summary>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除人事机构")]
    public async Task Delete(DeleteOrgInput input)
    {
        await EnsureOrgIdColumn();
        var org = await _sysOrgRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var childOrgs = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var codes = childOrgs.Select(u => u.Code).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (codes.Count > 0 && await _empRep.IsAnyAsync(u => codes.Contains(u.CcCode)))
            throw Oops.Oh("该机构或下级机构下存在员工，禁止删除");

        await _sysOrgService.DeleteOrg(input);
        if (codes.Count > 0)
            await _ccRep.DeleteAsync(u => codes.Contains(u.CcCode));
        await _ccRep.DeleteAsync(u => u.OrgId != null && childOrgs.Select(o => o.Id).Contains(u.OrgId.Value));
    }

    /// <summary>
    /// 从 cc_mstr 导入/对齐 SysOrg（结构以导入后的 SysOrg 为准继续维护）
    /// </summary>
    [ApiDescriptionSettings(Name = "SyncFromCcMstr"), HttpPost]
    [DisplayName("从人事部门导入机构")]
    public async Task<int> SyncFromCcMstr()
    {
        if (!_userManager.SuperAdmin)
            throw Oops.Oh(ErrorCodeEnum.D2003);

        await EnsureOrgIdColumn();
        var tenantId = _userManager.TenantId;
        var rootId = tenantId;
        var list = (await _ccRep.GetListAsync())
            .OrderBy(u => (u.CcLevel ?? "").Length)
            .ThenBy(u => u.CcLevel)
            .ThenBy(u => u.CcCode)
            .ToList();

        var idToOrgId = new Dictionary<int, long>();
        var count = 0;
        foreach (var cc in list)
        {
            long pid = rootId;
            if (cc.ParentId is > 0 && idToOrgId.TryGetValue(cc.ParentId.Value, out var parentOrgId))
                pid = parentOrgId;

            var exist = await _sysOrgRep.GetFirstAsync(u => u.Code == cc.CcCode && u.TenantId == tenantId);
            var name = Truncate(cc.CcDesc, 64);
            var level = string.IsNullOrWhiteSpace(cc.CcLevel) ? 1 : Math.Max(1, cc.CcLevel.Length / 2);
            var status = ToStatus(cc.CcEnable);
            if (exist == null)
            {
                var orgId = await _sysOrgService.AddOrg(new AddOrgInput
                {
                    Pid = pid,
                    Name = name,
                    Code = cc.CcCode,
                    Type = "201",
                    Level = level,
                    OrderNo = ParseOrder(cc.CcLevel),
                    Status = status,
                    Remark = cc.CcDeleRemark
                });
                cc.OrgId = orgId;
                idToOrgId[cc.CcIdentityId] = orgId;
            }
            else
            {
                exist.Pid = pid;
                exist.Name = name;
                exist.Level = level;
                exist.Status = status;
                exist.OrderNo = ParseOrder(cc.CcLevel);
                await _sysOrgRep.AsUpdateable(exist).IgnoreColumns(true).ExecuteCommandAsync();
                cc.OrgId = exist.Id;
                idToOrgId[cc.CcIdentityId] = exist.Id;
            }

            if (cc.ParentId is > 0 && idToOrgId.TryGetValue(cc.ParentId.Value, out var pOrgId))
            {
                var pOrg = await _sysOrgRep.GetByIdAsync(pOrgId);
                cc.CcCodeParent = pOrg?.Code;
            }
            else
            {
                cc.CcCodeParent = null;
                cc.ParentId = 0;
            }

            await _ccRep.AsUpdateable(cc).UpdateColumns(u => new { u.OrgId, u.CcCodeParent, u.ParentId }).ExecuteCommandAsync();
            count++;
        }

        return count;
    }

    /// <summary>
    /// 人事代码下拉
    /// </summary>
    [DisplayName("人事代码下拉")]
    public async Task<List<HrCodeItemOutput>> GetCodeList([FromQuery] string codeField)
    {
        if (string.IsNullOrWhiteSpace(codeField))
            return [];
        var list = await _codeRep.AsQueryable()
            .Where(u => u.CodeField == codeField)
            .OrderBy(u => u.CodeSort)
            .OrderBy(u => u.CodeValue)
            .ToListAsync();
        return list.Select(u => new HrCodeItemOutput
        {
            Value = u.CodeValue,
            Label = string.IsNullOrWhiteSpace(u.CodeDesc) ? u.CodeValue : $"{u.CodeValue} {u.CodeDesc}"
        }).ToList();
    }

    private async Task<List<CcMstr>> TryGetCcList()
    {
        try
        {
            await EnsureOrgIdColumn();
            return await _ccRep.GetListAsync();
        }
        catch
        {
            return [];
        }
    }

    private async Task EnsureOrgIdColumn()
    {
        await _ccRep.Context.Ado.ExecuteCommandAsync("""
            IF COL_LENGTH('dbo.cc_mstr', 'OrgId') IS NULL
                ALTER TABLE dbo.cc_mstr ADD OrgId bigint NULL;
            """);
    }

    private async Task<string> NextCcCode()
    {
        var codes = await _ccRep.AsQueryable().Select(u => u.CcCode).ToListAsync();
        var max = codes.Select(s => int.TryParse(s, out var n) ? n : 0).DefaultIfEmpty(21536).Max();
        return (max + 1).ToString("000000");
    }

    private async Task<CcMstr?> GetParentCc(long pid)
    {
        if (pid == 0) return null;
        var parent = await _sysOrgRep.GetByIdAsync(pid);
        return await GetCcByOrg(parent);
    }

    private async Task<CcMstr?> GetCcByOrg(SysOrg? org)
    {
        if (org == null) return null;
        if (org.Id > 0)
        {
            var byOrg = await _ccRep.GetFirstAsync(u => u.OrgId == org.Id);
            if (byOrg != null) return byOrg;
        }
        if (!string.IsNullOrWhiteSpace(org.Code))
            return await _ccRep.GetFirstAsync(u => u.CcCode == org.Code);
        return null;
    }

    private async Task<string> BuildCcLevel(long pid, long? excludeOrgId)
    {
        var parentCc = await GetParentCc(pid);
        var parentLevel = parentCc?.CcLevel ?? "";
        var siblings = await _sysOrgRep.AsQueryable().Where(u => u.Pid == pid).ToListAsync();
        var siblingLevels = new List<string>();
        foreach (var s in siblings)
        {
            if (excludeOrgId != null && s.Id == excludeOrgId) continue;
            var cc = await GetCcByOrg(s);
            if (!string.IsNullOrWhiteSpace(cc?.CcLevel))
                siblingLevels.Add(cc.CcLevel);
        }
        var max = 0;
        foreach (var level in siblingLevels)
        {
            if (!level.StartsWith(parentLevel) || level.Length != parentLevel.Length + 2) continue;
            if (int.TryParse(level[^2..], out var n) && n > max)
                max = n;
        }
        return parentLevel + (max + 1).ToString("00");
    }

    private async Task RecalcDescendants(long orgId, string oldPrefix, string newPrefix)
    {
        var children = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, orgId, false);
        foreach (var child in children)
        {
            var cc = await GetCcByOrg(child);
            if (cc == null || string.IsNullOrWhiteSpace(cc.CcLevel) || !cc.CcLevel.StartsWith(oldPrefix))
                continue;
            cc.CcLevel = newPrefix + cc.CcLevel[oldPrefix.Length..];
            var parent = await _sysOrgRep.GetByIdAsync(child.Pid);
            var parentCc = await GetCcByOrg(parent);
            cc.CcCodeParent = parentCc?.CcCode;
            cc.ParentId = parentCc?.CcIdentityId ?? 0;
            cc.OrgId = child.Id;
            await _ccRep.AsUpdateable(cc).UpdateColumns(u => new { u.CcLevel, u.CcCodeParent, u.ParentId, u.OrgId }).ExecuteCommandAsync();

            child.Level = Math.Max(1, cc.CcLevel.Length / 2);
            await _sysOrgRep.AsUpdateable(child).UpdateColumns(u => new { u.Level }).ExecuteCommandAsync();
        }
    }

    private AddOrgInput ToAddOrgInput(HrOrgSaveInput input, string ccCode, string ccLevel)
    {
        return new AddOrgInput
        {
            Pid = input.Pid,
            Name = Truncate(string.IsNullOrWhiteSpace(input.CcDesc) ? input.Name : input.CcDesc, 64),
            Code = ccCode,
            Type = string.IsNullOrWhiteSpace(input.Type) ? "201" : input.Type,
            Level = Math.Max(1, ccLevel.Length / 2),
            DirectorId = input.DirectorId,
            Longitude = input.Longitude,
            Latitude = input.Latitude,
            OrderNo = input.OrderNo == 0 ? ParseOrder(ccLevel) : input.OrderNo,
            Status = input.Status,
            Remark = input.Remark ?? input.CcDeleRemark
        };
    }

    private UpdateOrgInput ToUpdateOrgInput(HrOrgSaveInput input, string ccCode, string ccLevel)
    {
        var add = ToAddOrgInput(input, ccCode, ccLevel);
        var update = add.Adapt<UpdateOrgInput>();
        update.Id = input.Id;
        return update;
    }

    private CcMstr ToCcEntity(HrOrgSaveInput input, string ccCode, string ccLevel, CcMstr? parentCc, long orgId, string userName, DateTime now, bool isNew)
    {
        var enable = input.Status == StatusEnum.Disable ? 1 : 0;
        return new CcMstr
        {
            CcCode = ccCode,
            CcCodeParent = parentCc?.CcCode,
            CcDept = input.CcDept,
            CcDesc = string.IsNullOrWhiteSpace(input.CcDesc) ? input.Name : input.CcDesc,
            CcLevel = ccLevel,
            CcGroup = input.CcGroup,
            CcIndet = input.CcIndet,
            CcSub = input.CcSub ?? 0,
            CcSite = input.CcSite,
            CcState = input.CcState,
            CcCost = input.CcCost,
            CcWageRate = input.CcWageRate ?? 0,
            CcStatBasic = input.CcStatBasic,
            CcEnable = enable,
            CcDeleDate = input.Status == StatusEnum.Disable ? (input.CcDeleDate ?? now) : null,
            CcCountGroup = input.CcCountGroup,
            CcUser = userName,
            CcDate = now,
            CcKqgroup = input.CcKqgroup ?? 1,
            CcWoca = input.CcWoca,
            CcBzUnit = input.CcBzUnit,
            CcDeleRemark = input.CcDeleRemark ?? input.Remark,
            CcDescEn = input.CcDescEn,
            CcLevelCensor = input.CcLevelCensor,
            CcXzGroup = input.CcXzGroup,
            CcCreateDate = isNew ? (input.CcCreateDate ?? now) : input.CcCreateDate,
            MachineFloor = input.MachineFloor,
            KqNewSysuser = input.KqNewSysuser,
            KqNewSysdate = input.KqNewSysdate,
            ParentId = parentCc?.CcIdentityId ?? 0,
            CcCode2 = input.CcCode2,
            OrgId = orgId
        };
    }

    private static HrOrgOutput ToOutput(SysOrg org, Dictionary<string, CcMstr> ccMap)
    {
        var output = org.Adapt<HrOrgOutput>();
        if (!string.IsNullOrWhiteSpace(org.Code) && ccMap.TryGetValue(org.Code, out var cc))
            FillCc(output, cc);
        else
        {
            var byOrg = ccMap.Values.FirstOrDefault(u => u.OrgId == org.Id);
            if (byOrg != null) FillCc(output, byOrg);
        }

        if (org.Children is { Count: > 0 })
            output.Children = org.Children.Select(u => ToOutput(u, ccMap)).ToList();
        return output;
    }

    private static void FillCc(HrOrgOutput output, CcMstr cc)
    {
        output.CcCode = cc.CcCode;
        output.CcCodeParent = cc.CcCodeParent;
        output.CcDept = cc.CcDept;
        output.CcDesc = cc.CcDesc;
        output.CcLevel = cc.CcLevel;
        output.CcGroup = cc.CcGroup;
        output.CcIndet = cc.CcIndet;
        output.CcSub = cc.CcSub;
        output.CcSite = cc.CcSite;
        output.CcState = cc.CcState;
        output.CcCost = cc.CcCost;
        output.CcWageRate = cc.CcWageRate;
        output.CcStatBasic = cc.CcStatBasic;
        output.CcEnable = cc.CcEnable;
        output.CcDeleDate = cc.CcDeleDate;
        output.CcCountGroup = cc.CcCountGroup;
        output.CcUser = cc.CcUser;
        output.CcDate = cc.CcDate;
        output.CcKqgroup = cc.CcKqgroup;
        output.CcWoca = cc.CcWoca;
        output.CcBzUnit = cc.CcBzUnit;
        output.CcDeleRemark = cc.CcDeleRemark;
        output.CcDescEn = cc.CcDescEn;
        output.CcLevelCensor = cc.CcLevelCensor;
        output.CcXzGroup = cc.CcXzGroup;
        output.CcCreateDate = cc.CcCreateDate;
        output.MachineFloor = cc.MachineFloor;
        output.KqNewSysuser = cc.KqNewSysuser;
        output.KqNewSysdate = cc.KqNewSysdate;
        output.CcIdentityId = cc.CcIdentityId;
        output.ParentId = cc.ParentId;
        output.CcCode2 = cc.CcCode2;
        output.OrgId = cc.OrgId;
        if (!string.IsNullOrWhiteSpace(cc.CcDesc))
            output.Name = cc.CcDesc;
    }

    private static StatusEnum ToStatus(int? ccEnable) => ccEnable == 1 ? StatusEnum.Disable : StatusEnum.Enable;

    private static int ParseOrder(string? ccLevel)
    {
        if (string.IsNullOrWhiteSpace(ccLevel) || ccLevel.Length < 2) return 100;
        return int.TryParse(ccLevel[^2..], out var n) ? n : 100;
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return value ?? "";
        return value.Length <= max ? value : value[..max];
    }
}
