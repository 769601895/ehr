namespace Admin.NET.Application;

/// <summary>人事月报考勤月界推导（方案 §6.2）</summary>
public static class HrEmpMonthPeriodResolver
{
    public sealed record EmpMonthPeriodResult(string PayPeriod, string AttMonth, DateTime AttFrom, DateTime AttTo);

    /// <summary>
    /// 由薪酬月 + 公司参数推导出勤归属月与考勤窗 [AttFrom, AttTo]（含首尾）。
    /// </summary>
    public static EmpMonthPeriodResult Resolve(string payPeriod, int attCloseDay, int payrollAttMonth)
    {
        if (string.IsNullOrWhiteSpace(payPeriod) || payPeriod.Length != 6
            || !int.TryParse(payPeriod.AsSpan(0, 4), out var year)
            || !int.TryParse(payPeriod.AsSpan(4, 2), out var month)
            || month is < 1 or > 12)
            throw Oops.Oh("薪酬月格式须为 yyyyMM");

        if (payrollAttMonth is not (0 or 1))
            throw Oops.Oh("出勤所属月参数无效（须为 0 当月或 1 上月）");

        var payMonthStart = new DateTime(year, month, 1);
        var attMonthStart = payrollAttMonth == 0 ? payMonthStart : payMonthStart.AddMonths(-1);
        var attMonth = $"{attMonthStart.Year:D4}{attMonthStart.Month:D2}";
        var daysInAtt = DateTime.DaysInMonth(attMonthStart.Year, attMonthStart.Month);

        // 0/31 且当月不足 31 → 月末；>当月天数 → 月末
        var close = attCloseDay;
        if (close is 0 or 31 || close > daysInAtt)
            close = daysInAtt;
        if (close < 1)
            close = daysInAtt;

        DateTime attFrom;
        DateTime attTo;
        if (close == daysInAtt)
        {
            attFrom = attMonthStart;
            attTo = attMonthStart.AddDays(daysInAtt - 1);
        }
        else
        {
            attTo = new DateTime(attMonthStart.Year, attMonthStart.Month, close);
            var prev = attMonthStart.AddMonths(-1);
            var prevDays = DateTime.DaysInMonth(prev.Year, prev.Month);
            var fromDay = close + 1;
            if (fromDay > prevDays)
                fromDay = prevDays;
            attFrom = new DateTime(prev.Year, prev.Month, fromDay);
        }

        return new EmpMonthPeriodResult(payPeriod, attMonth, attFrom.Date, attTo.Date);
    }
}
