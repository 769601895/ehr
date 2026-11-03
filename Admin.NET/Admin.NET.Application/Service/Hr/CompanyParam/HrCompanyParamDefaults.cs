namespace Admin.NET.Application;

public static class HrCompanyParamDefaults
{
    public static HrCompanyParam New(string siteCode, string siteName) => new()
    {
        SiteCode = siteCode,
        SiteName = siteName,
        EmpIdMode = 1,
        EmpIdLength = 8,
        EmpIdFixedPrefix = null,
        ProbationOnHire = 1,
        ProbationHireDays = 2,
        ProbationDayInclusive = 1,
        DimissionAudit = 0,
        TransferAudit = 0,
        RegularizeAudit = 0,
        IdCardV2Enabled = 1,
        HireDayAttMode = 1,
        LeaveDayAttMode = 1,
        AttCloseDay = 31,
        RequiredAttDayMode = 0,
        PayrollAttMonth = 0,
        OtAuditMode = 0,
        LeaveAudit = 0,
        OtCrossdayMode = 0,
        CardMakeupAudit = 0,
        OtMinUnitHour = 0.50m,
        InsMonthReportMode = 0,
        InsFeeToPayroll = 1,
        InsSameMonthReport = 1,
    };
}
