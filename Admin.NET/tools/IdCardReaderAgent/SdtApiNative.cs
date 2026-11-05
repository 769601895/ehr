using System.Runtime.InteropServices;
using System.Text;

namespace IdCardReaderAgent;

/// <summary>
/// 公安部标准接口常见导出（sdtapi.dll）。
/// </summary>
internal static class SdtApiNative
{
    private const string Dll = "sdtapi.dll";

    [DllImport(Dll, CallingConvention = CallingConvention.StdCall)]
    public static extern int InitComm(int iPort);

    [DllImport(Dll, CallingConvention = CallingConvention.StdCall)]
    public static extern int CloseComm();

    [DllImport(Dll, CallingConvention = CallingConvention.StdCall)]
    public static extern int Authenticate();

    /// <summary>读基本信息；部分驱动成功后在工作目录生成 photo.bmp / zp.bmp</summary>
    [DllImport(Dll, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public static extern int ReadBaseInfos(
        StringBuilder name,
        StringBuilder gender,
        StringBuilder folk,
        StringBuilder birthDay,
        StringBuilder code,
        StringBuilder address,
        StringBuilder agency,
        StringBuilder expireStart,
        StringBuilder expireEnd);
}
