using Admin.NET.Core;

namespace Admin.NET.Application;

/// <summary>
/// 人员身份证校验与籍贯/出生日期解析（中国国籍默认校验；非中国跳过）。
/// </summary>
public class HrEmpIdCardService : ITransient
{
    public const string ChinaCountryCode = "CN";

    private readonly SqlSugarRepository<HrProvince> _provinceRep;
    private readonly SqlSugarRepository<HrCity> _cityRep;
    private readonly SqlSugarRepository<HrCompanyParam> _paramRep;
    private readonly HrCompanySiteResolveService _siteResolve;

    public HrEmpIdCardService(
        SqlSugarRepository<HrProvince> provinceRep,
        SqlSugarRepository<HrCity> cityRep,
        SqlSugarRepository<HrCompanyParam> paramRep,
        HrCompanySiteResolveService siteResolve)
    {
        _provinceRep = provinceRep;
        _cityRep = cityRep;
        _paramRep = paramRep;
        _siteResolve = siteResolve;
    }

    /// <summary>空或 CN（及「中国」）按中国身份证处理。</summary>
    public static bool NeedChineseIdCard(string? empCountry)
    {
        if (string.IsNullOrWhiteSpace(empCountry)) return true;
        var c = empCountry.Trim();
        return string.Equals(c, ChinaCountryCode, StringComparison.OrdinalIgnoreCase)
               || string.Equals(c, "中国", StringComparison.Ordinal);
    }

    /// <summary>
    /// 是否允许硬件读卡：公司参数「启用二代身份证」。
    /// 按<strong>当前用户管辖部门</strong>同属厂别解析，不跟员工任职部门挂钩。
    /// </summary>
    public async Task<HrEmpIdCardFeatureOutput> GetFeatureAsync(HrEmpIdCardFeatureInput? input)
    {
        _ = input; // 保留入参兼容；故意忽略员工 ccCode
        try
        {
            var site = await _siteResolve.TryResolveDefaultSiteCodeForCurrentUserAsync();
            if (string.IsNullOrWhiteSpace(site))
            {
                return new HrEmpIdCardFeatureOutput
                {
                    Enabled = false,
                    IdCardV2Enabled = 0,
                    Message = "无法确定公司参数：管辖部门未配置归属公司，或管辖跨多个公司"
                };
            }

            var param = await _paramRep.GetFirstAsync(u => u.SiteCode == site);
            if (param == null)
            {
                return new HrEmpIdCardFeatureOutput
                {
                    Enabled = false,
                    IdCardV2Enabled = 0,
                    SiteCode = site,
                    Message = "未找到公司参数"
                };
            }

            var enabled = param.IdCardV2Enabled != 0;
            return new HrEmpIdCardFeatureOutput
            {
                Enabled = enabled,
                IdCardV2Enabled = param.IdCardV2Enabled,
                SiteCode = site,
                Message = enabled ? null : "公司参数未启用二代身份证"
            };
        }
        catch (Exception ex)
        {
            return new HrEmpIdCardFeatureOutput
            {
                Enabled = false,
                IdCardV2Enabled = 0,
                Message = string.IsNullOrWhiteSpace(ex.Message)
                    ? "无法确定公司参数：管辖部门未配置归属公司，或管辖跨多个公司"
                    : ex.Message
            };
        }
    }

    public async Task<HrEmpIdCardParseOutput> ParseAsync(HrEmpIdCardParseInput input)
    {
        input ??= new HrEmpIdCardParseInput();
        var card = input.EmpCard?.Trim();
        var need = NeedChineseIdCard(input.EmpCountry);

        var output = new HrEmpIdCardParseOutput
        {
            NeedValidate = need,
            Valid = true
        };

        if (!need)
            return output;

        if (string.IsNullOrWhiteSpace(card))
            return output;

        var v2 = await IsIdCardV2EnabledAsync(input.CcCode);
        if (v2 && card.Length != 18)
        {
            output.Valid = false;
            output.Message = "身份证号无效（已启用二代身份证，须为18位）";
            return output;
        }

        if (!IdCardHelper.CheckIdCard(card))
        {
            output.Valid = false;
            output.Message = "身份证号无效";
            return output;
        }

        var birth = IdCardHelper.GetBirthday(card);
        if (birth != default)
            output.EmpBirth = birth.Date;

        var sex = IdCardHelper.GetGender(card);
        if (!string.IsNullOrWhiteSpace(sex))
            output.EmpSex = sex;

        await FillNativeRegionAsync(card, output);
        return output;
    }

    /// <summary>保存前：中国证件强制校验并始终覆盖生日/省/市/性别。</summary>
    public async Task ApplyOnSaveAsync(HrEmpSaveInput input)
    {
        var parsed = await ParseAsync(new HrEmpIdCardParseInput
        {
            EmpCard = input.EmpCard,
            EmpCountry = input.EmpCountry,
            CcCode = input.CcCode
        });

        if (!parsed.NeedValidate)
            return;

        if (string.IsNullOrWhiteSpace(input.EmpCard))
            return;

        if (!parsed.Valid)
            throw Oops.Oh(parsed.Message ?? "身份证号无效");

        if (parsed.EmpBirth.HasValue)
            input.EmpBirth = parsed.EmpBirth;
        if (!string.IsNullOrWhiteSpace(parsed.EmpSex))
            input.EmpSex = parsed.EmpSex;

        // 命中省主数据才覆盖：始终写省码；市无匹配则清空（避免省变市旧）。未命中省则保留原值。
        if (!string.IsNullOrWhiteSpace(parsed.EmpProvince))
        {
            input.EmpProvince = parsed.EmpProvince;
            input.EmpCity = parsed.EmpCity;
        }
    }

    private async Task FillNativeRegionAsync(string card, HrEmpIdCardParseOutput output)
    {
        if (card.Length < 6) return;
        var area = card[..6];
        if (!area.All(char.IsDigit)) return;

        var provinceCode = area[..2] + "0000";
        var cityCode = area[..4] + "00";

        var province = await _provinceRep.GetFirstAsync(u => u.ProvinceCode == provinceCode);
        if (province == null) return;

        output.EmpProvince = province.ProvinceCode;
        output.ProvinceName = province.ProvinceName;

        var city = await _cityRep.GetFirstAsync(u => u.CityCode == cityCode);
        if (city != null)
        {
            output.EmpCity = city.CityCode;
            output.CityName = city.CityName;
            // 以市表所属省为准（防止脏数据）
            if (!string.IsNullOrWhiteSpace(city.ProvinceCode))
                output.EmpProvince = city.ProvinceCode;
        }
        else
        {
            output.EmpCity = null;
            output.CityName = null;
        }
    }

    /// <summary>二代证位数规则：按用户管辖同属厂别取参（与读卡开关同源，不跟员工部门）。</summary>
    private async Task<bool> IsIdCardV2EnabledAsync(string? ccCode)
    {
        _ = ccCode;
        try
        {
            var site = await _siteResolve.TryResolveDefaultSiteCodeForCurrentUserAsync();
            if (string.IsNullOrWhiteSpace(site)) return true;
            var param = await _paramRep.GetFirstAsync(u => u.SiteCode == site);
            if (param == null) return true;
            return param.IdCardV2Enabled != 0;
        }
        catch
        {
            return true;
        }
    }
}
