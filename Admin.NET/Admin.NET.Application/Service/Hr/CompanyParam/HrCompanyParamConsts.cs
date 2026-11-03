namespace Admin.NET.Application;

public static class HrCompanyParamConsts
{
    public const string ErrSiteDup = "HR_CP_SITE_DUP";
    public const string ErrNotFound = "HR_CP_NOT_FOUND";
    public const string ErrCodeInvalid = "HR_CP_CODE_INVALID";
    public const string ErrSiteCodeEmpty = "HR_CP_SITE_CODE_EMPTY";
    public const string ErrOrgNotCompany = "HR_CP_ORG_NOT_COMPANY";
    public const string ErrBelongSiteInvalid = "HR_CP_BELONG_SITE_INVALID";
    public const string OrgTypeCompany = "2";

    /// <summary>下拉 code_field = 列名。</summary>
    public static readonly string[] CodeFields =
    [
        nameof(HrCompanyParam.EmpIdMode),
        nameof(HrCompanyParam.ProbationOnHire),
        nameof(HrCompanyParam.ProbationDayInclusive),
        nameof(HrCompanyParam.DimissionAudit),
        nameof(HrCompanyParam.TransferAudit),
        nameof(HrCompanyParam.RegularizeAudit),
        nameof(HrCompanyParam.IdCardV2Enabled),
        nameof(HrCompanyParam.HireDayAttMode),
        nameof(HrCompanyParam.LeaveDayAttMode),
        nameof(HrCompanyParam.RequiredAttDayMode),
        nameof(HrCompanyParam.PayrollAttMonth),
        nameof(HrCompanyParam.OtAuditMode),
        nameof(HrCompanyParam.LeaveAudit),
        nameof(HrCompanyParam.OtCrossdayMode),
        nameof(HrCompanyParam.CardMakeupAudit),
        nameof(HrCompanyParam.InsMonthReportMode),
        nameof(HrCompanyParam.InsFeeToPayroll),
        nameof(HrCompanyParam.InsSameMonthReport),
    ];
}
