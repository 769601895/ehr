using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace IdCardReaderAgent;

internal sealed class IdCardOptions
{
    /// <summary>读卡器端口，USB 常见 1001</summary>
    public int CommPort { get; set; } = 1001;

    /// <summary>sdtapi.dll 所在目录（空=进程目录 / PATH）</summary>
    public string DllDirectory { get; set; } = "";

    /// <summary>无硬件时返回模拟数据，便于联调前端</summary>
    public bool Mock { get; set; }
}

internal sealed class IdCardReadResult
{
    public bool Ok { get; set; }
    public string? Code { get; set; }
    public string? Message { get; set; }
    public string? Name { get; set; }
    public string? Sex { get; set; }
    public string? Nation { get; set; }
    public string? Birth { get; set; }
    public string? Address { get; set; }
    public string? IdNumber { get; set; }
    public string? Agency { get; set; }
    public string? ExpireStart { get; set; }
    public string? ExpireEnd { get; set; }
    /// <summary>证件头像 JPEG/BMP 的 Base64（不含 data: 前缀）</summary>
    public string? PhotoBase64 { get; set; }
}

internal sealed class IdCardReadService
{
    private readonly IdCardOptions _opt;
    private readonly ILogger<IdCardReadService> _log;
    private readonly object _gate = new();

    public IdCardReadService(IdCardOptions opt, ILogger<IdCardReadService> log)
    {
        _opt = opt;
        _log = log;
    }

    public IdCardReadResult Read()
    {
        if (_opt.Mock)
            return MockResult();

        lock (_gate)
        {
            return ReadCore();
        }
    }

    private IdCardReadResult ReadCore()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Fail("UNSUPPORTED_OS", "读卡仅支持 Windows");

        var workDir = Path.Combine(Path.GetTempPath(), "ehr-idcard-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(workDir);
        var prevCwd = Environment.CurrentDirectory;

        try
        {
            if (!string.IsNullOrWhiteSpace(_opt.DllDirectory))
            {
                var dir = _opt.DllDirectory.Trim();
                if (!Directory.Exists(dir))
                    return Fail("DLL_DIR", $"DllDirectory 不存在: {dir}");
                Environment.SetEnvironmentVariable("PATH", dir + Path.PathSeparator + Environment.GetEnvironmentVariable("PATH"));
                try { NativeLibrary.Load(Path.Combine(dir, "sdtapi.dll")); } catch { /* 交给 DllImport */ }
            }

            Environment.CurrentDirectory = workDir;

            var port = _opt.CommPort <= 0 ? 1001 : _opt.CommPort;
            var init = SdtApiNative.InitComm(port);
            if (init != 1)
                return Fail("NO_DEVICE", $"InitComm 失败（返回 {init}）。请检查读卡器连接与端口 {port}");

            try
            {
                var auth = SdtApiNative.Authenticate();
                if (auth != 1)
                    return Fail("NO_CARD", $"寻卡/认证失败（返回 {auth}）。请放好身份证后重试");

                var name = Buf();
                var gender = Buf();
                var folk = Buf();
                var birth = Buf();
                var code = Buf();
                var address = Buf();
                var agency = Buf();
                var expireStart = Buf();
                var expireEnd = Buf();

                var read = SdtApiNative.ReadBaseInfos(name, gender, folk, birth, code, address, agency, expireStart, expireEnd);
                if (read != 1)
                    return Fail("READ_FAIL", $"读卡失败（返回 {read}）");

                var idNumber = TrimSb(code);
                if (string.IsNullOrWhiteSpace(idNumber))
                    return Fail("EMPTY", "读卡成功但证号为空");

                var photoB64 = TryReadPhotoBase64(workDir);

                return new IdCardReadResult
                {
                    Ok = true,
                    Name = TrimSb(name),
                    Sex = NormalizeSex(TrimSb(gender)),
                    Nation = NormalizeNation(TrimSb(folk)),
                    Birth = NormalizeBirth(TrimSb(birth)),
                    Address = TrimSb(address),
                    IdNumber = idNumber,
                    Agency = TrimSb(agency),
                    ExpireStart = NormalizeBirth(TrimSb(expireStart)),
                    ExpireEnd = NormalizeBirth(TrimSb(expireEnd)),
                    PhotoBase64 = photoB64
                };
            }
            finally
            {
                try { SdtApiNative.CloseComm(); } catch (Exception ex) { _log.LogDebug(ex, "CloseComm"); }
            }
        }
        catch (DllNotFoundException)
        {
            return Fail("NO_DLL", "未找到 sdtapi.dll。请将厂商提供的标准接口 DLL 放到 Agent 目录或配置 IdCard:DllDirectory");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "读卡异常");
            return Fail("ERROR", ex.Message);
        }
        finally
        {
            try { Environment.CurrentDirectory = prevCwd; } catch { /* ignore */ }
            try { Directory.Delete(workDir, true); } catch { /* ignore */ }
        }
    }

    private static StringBuilder Buf() => new(128);

    private static string TrimSb(StringBuilder sb) => (sb?.ToString() ?? "").Trim('\0', ' ', '\t', '\r', '\n');

    private static string NormalizeSex(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        if (s.Contains('男') || s is "1" or "M" or "m") return "男";
        if (s.Contains('女') || s is "2" or "F" or "f") return "女";
        return s;
    }

    private static string NormalizeNation(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        s = s.Trim();
        if (s.EndsWith("族", StringComparison.Ordinal)) return s;
        // 芯片常见「汉」→ 业务码表多为「汉族」
        if (s.Length <= 2 && !s.Contains('族')) return s + "族";
        return s;
    }

    private static string NormalizeBirth(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        s = s.Trim();
        var digits = Regex.Replace(s, @"\D", "");
        if (digits.Length >= 8)
            return $"{digits[..4]}-{digits.Substring(4, 2)}-{digits.Substring(6, 2)}";
        if (DateTime.TryParse(s, out var dt))
            return dt.ToString("yyyy-MM-dd");
        return s;
    }

    private static string? TryReadPhotoBase64(string workDir)
    {
        string[] names = ["photo.bmp", "Photo.bmp", "zp.bmp", "ZP.BMP", "head.bmp"];
        foreach (var n in names)
        {
            var path = Path.Combine(workDir, n);
            if (!File.Exists(path)) continue;
            try
            {
                var bytes = File.ReadAllBytes(path);
                if (bytes.Length > 0) return Convert.ToBase64String(bytes);
            }
            catch { /* next */ }
        }

        // 当前目录偶然写出的其它 bmp
        try
        {
            var bmp = Directory.GetFiles(workDir, "*.bmp").OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
            if (bmp != null)
                return Convert.ToBase64String(File.ReadAllBytes(bmp));
        }
        catch { /* ignore */ }

        return null;
    }

    private static IdCardReadResult MockResult() => new()
    {
        Ok = true,
        Name = "测试人员",
        Sex = "男",
        Nation = "汉族",
        Birth = "1990-01-15",
        Address = "广东省深圳市南山区测试路1号",
        // 仅联调用；真实环境关闭 Mock。校验位按 GB11643 示例号段，现场以芯片为准。
        IdNumber = "110101199003078096",
        Agency = "深圳市公安局南山分局",
        ExpireStart = "2020-01-01",
        ExpireEnd = "2040-01-01",
        Message = "Mock"
    };

    private static IdCardReadResult Fail(string code, string message) => new()
    {
        Ok = false,
        Code = code,
        Message = message
    };
}
