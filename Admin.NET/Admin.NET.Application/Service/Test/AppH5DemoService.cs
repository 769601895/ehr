// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application.Service.Test;

/// <summary>
/// AppH5 演示接口
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100, Name = "AppH5Demo")]
[AppApiDescription("App_Test")]
public class AppH5DemoService : IDynamicApiController, ITransient
{
    private static int haveCntInvokeCnt = -1; //统计第几次调用，用来随机产生错误，检验前端捕获错误的能力。
    private static int notCntInvokeCnt = -1; //统计第几次调用，用来随机产生错误，检验前端捕获错误的能力。

    /// <summary>
    /// 级营情况统计
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BusinessStatus", Description = "级营情况统计", Order = 990), HttpGet]
    [DisplayName("级营情况统计")]
    public async Task<PyStatisticsBusinessStatusOutput> BusinessStatus([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        // 故意做到正常、异常、业务异常来返回
        if (startDate.HasValue || endDate.HasValue)
        {
            haveCntInvokeCnt++;
            if (haveCntInvokeCnt % 3 == 1)
                throw Oops.Oh("故意抛 500 异常");
            if (haveCntInvokeCnt % 3 == 2)
                throw Oops.Bah("抛业务异常");
        }
        else
        {
            notCntInvokeCnt++;
            if (notCntInvokeCnt % 3 == 1)
                throw Oops.Oh("故意抛 500 异常");
            if (notCntInvokeCnt % 3 == 2)
                throw Oops.Bah("抛业务异常");
        }
        Random rnd = new Random();
        var result = new PyStatisticsBusinessStatusOutput();
        result.DeviceCount = rnd.Next(10, 100);
        result.OrderCount = rnd.Next(1, 10);
        result.OrderCount = rnd.Next(1, 100);
        result.OrderTotalValue = rnd.Next(20, 1000);
        result.OrderNotPayValue = rnd.Next(20, 1000);
        return result;
    }

    public class PyStatisticsBusinessStatusOutput
    {
        /// <summary>
        /// 设备总数
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        /// 在线设备总数
        /// </summary>
        public int OnlineDeviceCount { get; set; }

        /// <summary>
        /// 定单总数
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 定单总金额
        /// </summary>
        public double OrderTotalValue { get; set; }

        /// <summary>
        /// 未结算金额
        /// </summary>
        public double OrderNotPayValue { get; set; }
    }
}