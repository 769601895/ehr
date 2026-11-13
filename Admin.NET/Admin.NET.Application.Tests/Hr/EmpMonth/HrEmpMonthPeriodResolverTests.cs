using Admin.NET.Application;
using Xunit;

namespace Admin.NET.Application.Tests.Hr.EmpMonth;

public class HrEmpMonthPeriodResolverTests
{
    [Fact]
    public void Resolve_MidMonthClose_SameMonthPayroll_WindowSpansPriorMonth()
    {
        var r = HrEmpMonthPeriodResolver.Resolve("202611", 25, 0);
        Assert.Equal("202611", r.PayPeriod);
        Assert.Equal("202611", r.AttMonth);
        Assert.Equal(new DateTime(2026, 10, 26), r.AttFrom.Date);
        Assert.Equal(new DateTime(2026, 11, 25), r.AttTo.Date);
    }

    [Fact]
    public void Resolve_CloseDay31_November_IsNaturalMonth()
    {
        var r = HrEmpMonthPeriodResolver.Resolve("202611", 31, 0);
        Assert.Equal("202611", r.AttMonth);
        Assert.Equal(new DateTime(2026, 11, 1), r.AttFrom.Date);
        Assert.Equal(new DateTime(2026, 11, 30), r.AttTo.Date);
    }

    [Fact]
    public void Resolve_PayrollAttMonth1_UsesPriorCalendarMonth()
    {
        var r = HrEmpMonthPeriodResolver.Resolve("202611", 31, 1);
        Assert.Equal("202610", r.AttMonth);
        Assert.Equal(new DateTime(2026, 10, 1), r.AttFrom.Date);
        Assert.Equal(new DateTime(2026, 10, 31), r.AttTo.Date);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("20261")]
    [InlineData("2026111")]
    [InlineData("abcdef")]
    public void Resolve_InvalidPayPeriod_Throws(string? payPeriod)
    {
        Assert.ThrowsAny<Exception>(() => HrEmpMonthPeriodResolver.Resolve(payPeriod!, 25, 0));
    }

    [Fact]
    public void Resolve_InvalidPayrollAttMonth_Throws()
    {
        Assert.ThrowsAny<Exception>(() => HrEmpMonthPeriodResolver.Resolve("202611", 25, 2));
    }

    [Fact]
    public void Resolve_CloseDay0_TreatedAsMonthEnd()
    {
        var r = HrEmpMonthPeriodResolver.Resolve("202611", 0, 0);
        Assert.Equal(new DateTime(2026, 11, 1), r.AttFrom.Date);
        Assert.Equal(new DateTime(2026, 11, 30), r.AttTo.Date);
    }
}
