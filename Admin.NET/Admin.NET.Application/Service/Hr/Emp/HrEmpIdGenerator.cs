using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 按公司参数 EmpIdMode 预览/正式发号（正式发号占号；预览不占号）
/// </summary>
public class HrEmpIdGenerator : ITransient
{
    public const int ModeManual = 1;
    public const string LiteralDash = "-";
    private const int MaxSkipOccupied = 200;

    private readonly SqlSugarRepository<HrCompanyParam> _paramRep;
    private readonly SqlSugarRepository<HrEmpIdSeq> _seqRep;
    private readonly SqlSugarRepository<HrEmployee> _empRep;
    private readonly HrCompanySiteResolveService _siteResolve;

    public HrEmpIdGenerator(
        SqlSugarRepository<HrCompanyParam> paramRep,
        SqlSugarRepository<HrEmpIdSeq> seqRep,
        SqlSugarRepository<HrEmployee> empRep,
        HrCompanySiteResolveService siteResolve)
    {
        _paramRep = paramRep;
        _seqRep = seqRep;
        _empRep = empRep;
        _siteResolve = siteResolve;
    }

    public async Task<HrEmpIdPreviewOutput> PreviewAsync(string? ccCode)
    {
        var siteCode = await _siteResolve.ResolveSiteCodeAsync(ccCode);
        var param = await GetParamAsync(siteCode);
        var output = new HrEmpIdPreviewOutput
        {
            SiteCode = siteCode,
            EmpIdMode = param.EmpIdMode,
            EmpIdLength = param.EmpIdLength,
            EmpIdFixedPrefix = param.EmpIdFixedPrefix,
            Manual = param.EmpIdMode == ModeManual
        };

        if (output.Manual)
            return output;

        var plan = BuildPlan(param, DateTime.Today);
        var next = await PeekNextValAsync(siteCode, plan.ScopeKey);
        output.EmpId = FormatEmpId(plan, next + 1);
        return output;
    }

    /// <summary>正式发号（占号）。手工模式返回 null，由调用方使用入参工号。</summary>
    public async Task<string?> AllocateAsync(string? ccCode)
    {
        var siteCode = await _siteResolve.ResolveSiteCodeAsync(ccCode);
        var param = await GetParamAsync(siteCode);
        if (param.EmpIdMode == ModeManual)
            return null;

        var plan = BuildPlan(param, DateTime.Today);
        for (var i = 0; i < MaxSkipOccupied; i++)
        {
            var n = await AllocateNextValAsync(siteCode, plan.ScopeKey);
            var empId = FormatEmpId(plan, n);
            if (!await IsEmpIdUsedAsync(empId))
                return empId;
        }

        throw Oops.Oh("无法生成可用工号，请检查号段或已用工号");
    }

    public async Task EnsureEmpIdAvailableAsync(string empId)
    {
        if (await IsEmpIdUsedAsync(empId))
            throw Oops.Oh("工号已存在，不可复用");
    }

    public async Task<bool> IsEmpIdUsedAsync(string empId)
    {
        var id = empId.Trim();
        if (string.IsNullOrEmpty(id)) return false;
        return await _empRep.IsAnyAsync(u => u.EmpId == id);
    }

    private async Task<HrCompanyParam> GetParamAsync(string siteCode)
    {
        var param = await _paramRep.GetFirstAsync(u => u.SiteCode == siteCode)
            ?? throw Oops.Oh($"公司参数不存在：{siteCode}");
        if (param.EmpIdMode is 4 or 8 or 9 && string.IsNullOrWhiteSpace(param.EmpIdFixedPrefix))
            throw Oops.Oh("该公司未配置固定号前缀，无法发号");
        if (param.EmpIdMode != ModeManual && param.EmpIdLength <= 0)
            throw Oops.Oh("工号长度无效，无法发号");
        return param;
    }

    /// <summary>
    /// EmpIdLength = 整段工号总长度（含固定号、年月、符号等前缀）。
    /// 流水位数 = 总长 − 前缀长度。
    /// </summary>
    private static EmpIdPlan BuildPlan(HrCompanyParam param, DateTime date)
    {
        var mode = param.EmpIdMode;
        var totalLen = Math.Max(param.EmpIdLength, 1);
        var fixedPrefix = param.EmpIdFixedPrefix?.Trim() ?? "";
        var yyyy = date.ToString("yyyy");
        var mm = date.ToString("MM");

        return mode switch
        {
            0 => Plan("", "*", totalLen),
            4 => Plan(fixedPrefix, "*", totalLen),
            5 => Plan(yyyy + mm, yyyy + mm, totalLen),
            8 => Plan(fixedPrefix + yyyy + mm + LiteralDash, yyyy + mm, totalLen),
            9 => Plan(fixedPrefix + yyyy + mm, yyyy + mm, totalLen),
            ModeManual => new EmpIdPlan("", "*", 0),
            _ => throw Oops.Oh($"不支持的工号编码方式：{mode}")
        };
    }

    private static EmpIdPlan Plan(string prefix, string scopeKey, int totalLen)
    {
        var seqLen = totalLen - prefix.Length;
        if (seqLen < 1)
            throw Oops.Oh($"工号长度({totalLen})须大于前缀长度({prefix.Length})");
        return new EmpIdPlan(prefix, scopeKey, seqLen);
    }

    private static string FormatEmpId(EmpIdPlan plan, long seq)
    {
        var serial = seq.ToString().PadLeft(plan.SeqLength, '0');
        if (serial.Length > plan.SeqLength)
            serial = seq.ToString(); // 溢出后不截断，避免撞号语义错误
        return plan.Prefix + serial;
    }

    private async Task EnsureSeqRowAsync(string siteCode, string scopeKey)
    {
        var exists = await _seqRep.IsAnyAsync(u => u.SiteCode == siteCode && u.ScopeKey == scopeKey);
        if (exists) return;

        try
        {
            await _seqRep.InsertAsync(new HrEmpIdSeq
            {
                Id = YitIdHelper.NextId(),
                SiteCode = siteCode,
                ScopeKey = scopeKey,
                NextVal = 0,
                UpdateTime = DateTime.Now
            });
        }
        catch
        {
            // 并发插入忽略
        }
    }

    private async Task<long> PeekNextValAsync(string siteCode, string scopeKey)
    {
        await EnsureSeqRowAsync(siteCode, scopeKey);
        var row = await _seqRep.GetFirstAsync(u => u.SiteCode == siteCode && u.ScopeKey == scopeKey);
        return row?.NextVal ?? 0;
    }

    private async Task<long> AllocateNextValAsync(string siteCode, string scopeKey)
    {
        await EnsureSeqRowAsync(siteCode, scopeKey);

        var db = _seqRep.AsSugarClient();
        for (var attempt = 0; attempt < 8; attempt++)
        {
            try
            {
                // 依赖外层 UnitOfWork 事务；TranLock 防止并发撞号
                var row = await db.Queryable<HrEmpIdSeq>()
                    .TranLock(DbLockType.Wait)
                    .Where(u => u.SiteCode == siteCode && u.ScopeKey == scopeKey)
                    .FirstAsync();
                row.NextVal += 1;
                row.UpdateTime = DateTime.Now;
                await db.Updateable(row)
                    .UpdateColumns(u => new { u.NextVal, u.UpdateTime })
                    .ExecuteCommandAsync();
                return row.NextVal;
            }
            catch
            {
                await EnsureSeqRowAsync(siteCode, scopeKey);
            }
        }

        throw Oops.Oh("工号号段占用失败，请重试");
    }

    private sealed record EmpIdPlan(string Prefix, string ScopeKey, int SeqLength);
}
