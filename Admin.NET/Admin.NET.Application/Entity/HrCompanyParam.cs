namespace Admin.NET.Application;

/// <summary>
/// 公司参数（一行一公司）
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("HrCompanyParam", "公司参数")]
public class HrCompanyParam : EntityBase
{
    [SugarColumn(ColumnDescription = "公司代码", Length = 32)]
    public string SiteCode { get; set; }

    [SugarColumn(ColumnDescription = "公司名称", Length = 100)]
    public string SiteName { get; set; }

    [SugarColumn(ColumnDescription = "工号编码方式")]
    public int EmpIdMode { get; set; }

    [SugarColumn(ColumnDescription = "工号长度")]
    public int EmpIdLength { get; set; }

    [SugarColumn(ColumnDescription = "固定号", Length = 32, IsNullable = true)]
    public string? EmpIdFixedPrefix { get; set; }

    [SugarColumn(ColumnDescription = "新进试用否")]
    public int ProbationOnHire { get; set; }

    [SugarColumn(ColumnDescription = "新进试用期限")]
    public int ProbationHireDays { get; set; }

    [SugarColumn(ColumnDescription = "试用期当天是否有效")]
    public int ProbationDayInclusive { get; set; }

    [SugarColumn(ColumnDescription = "离职审核")]
    public int DimissionAudit { get; set; }

    [SugarColumn(ColumnDescription = "异动审核")]
    public int TransferAudit { get; set; }

    [SugarColumn(ColumnDescription = "转正审核")]
    public int RegularizeAudit { get; set; }

    [SugarColumn(ColumnDescription = "启用二代身份证")]
    public int IdCardV2Enabled { get; set; }

    [SugarColumn(ColumnDescription = "入职日考勤")]
    public int HireDayAttMode { get; set; }

    [SugarColumn(ColumnDescription = "离职日考勤否")]
    public int LeaveDayAttMode { get; set; }

    [SugarColumn(ColumnDescription = "考勤结束日")]
    public int AttCloseDay { get; set; }

    [SugarColumn(ColumnDescription = "应出勤天计算")]
    public int RequiredAttDayMode { get; set; }

    [SugarColumn(ColumnDescription = "薪资考勤月份")]
    public int PayrollAttMonth { get; set; }

    [SugarColumn(ColumnDescription = "加班审核")]
    public int OtAuditMode { get; set; }

    [SugarColumn(ColumnDescription = "请假审核")]
    public int LeaveAudit { get; set; }

    [SugarColumn(ColumnDescription = "加班跨日计算")]
    public int OtCrossdayMode { get; set; }

    [SugarColumn(ColumnDescription = "补卡审核")]
    public int CardMakeupAudit { get; set; }

    [SugarColumn(ColumnDescription = "加班最小单位小时", DecimalDigits = 2, Length = 18)]
    public decimal OtMinUnitHour { get; set; }

    [SugarColumn(ColumnDescription = "开始月份生成月报方式")]
    public int InsMonthReportMode { get; set; }

    [SugarColumn(ColumnDescription = "保费提交薪资方式")]
    public int InsFeeToPayroll { get; set; }

    [SugarColumn(ColumnDescription = "当月投保当月退保月报产生方式")]
    public int InsSameMonthReport { get; set; }
}
