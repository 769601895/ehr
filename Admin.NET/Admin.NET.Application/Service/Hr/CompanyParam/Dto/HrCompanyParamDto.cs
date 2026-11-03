namespace Admin.NET.Application;

public class HrCompanyParamPageInput : BasePageInput
{
}

public class HrCompanyParamOutput
{
    public long Id { get; set; }
    public string SiteCode { get; set; }
    public string SiteName { get; set; }
    public int EmpIdMode { get; set; }
    public int EmpIdLength { get; set; }
    public string? EmpIdFixedPrefix { get; set; }
    public int ProbationOnHire { get; set; }
    public int ProbationHireDays { get; set; }
    public int ProbationDayInclusive { get; set; }
    public int DimissionAudit { get; set; }
    public int TransferAudit { get; set; }
    public int RegularizeAudit { get; set; }
    public int IdCardV2Enabled { get; set; }
    public int HireDayAttMode { get; set; }
    public int LeaveDayAttMode { get; set; }
    public int AttCloseDay { get; set; }
    public int RequiredAttDayMode { get; set; }
    public int PayrollAttMonth { get; set; }
    public int OtAuditMode { get; set; }
    public int LeaveAudit { get; set; }
    public int OtCrossdayMode { get; set; }
    public int CardMakeupAudit { get; set; }
    public decimal OtMinUnitHour { get; set; }
    public int InsMonthReportMode { get; set; }
    public int InsFeeToPayroll { get; set; }
    public int InsSameMonthReport { get; set; }
}

public class HrCompanyParamAddInput
{
    [Required(ErrorMessage = "公司代码不能为空")]
    [MaxLength(32)]
    public string SiteCode { get; set; }

    [Required(ErrorMessage = "公司名称不能为空")]
    [MaxLength(100)]
    public string SiteName { get; set; }
}

public class HrCompanyParamUpdateInput : HrCompanyParamOutput
{
    [Required]
    public new long Id { get; set; }

    [Required(ErrorMessage = "公司代码不能为空")]
    [MaxLength(32)]
    public new string SiteCode { get; set; }

    [Required(ErrorMessage = "公司名称不能为空")]
    [MaxLength(100)]
    public new string SiteName { get; set; }
}

public class HrCompanyParamDeleteInput
{
    [Required]
    public long Id { get; set; }
}

public class HrCompanyParamLangSaveInput
{
    [Required]
    public long Id { get; set; }

    public List<HrUiLangItemInput> Labels { get; set; } = [];
}

/// <summary>公司型组织（归属公司）分页查询</summary>
public class HrCompanyOrgPageInput : BasePageInput
{
}

public class HrCompanyOrgOutput
{
    public string CcCode { get; set; }
    public string CcDesc { get; set; }
    public string? CcDept { get; set; }
    public string? OrgType { get; set; }
    public string? OrgTypeName { get; set; }
    public string? CcLevel { get; set; }
    /// <summary>归属公司代码（可空）</summary>
    public string? BelongSiteCode { get; set; }
    public string? BelongSiteName { get; set; }
}

public class HrCompanyParamSiteOption
{
    public string SiteCode { get; set; }
    public string SiteName { get; set; }
}

public class HrCompanyOrgSiteItemInput
{
    [Required(ErrorMessage = "部门编码不能为空")]
    [MaxLength(50)]
    public string CcCode { get; set; }

    [MaxLength(32)]
    public string? BelongSiteCode { get; set; }
}

public class HrCompanyOrgSiteSaveInput
{
    [Required]
    [MinLength(1)]
    public List<HrCompanyOrgSiteItemInput> Items { get; set; } = [];
}
