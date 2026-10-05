// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using PaddleOCRSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace Admin.NET.Plugin.PaddleOCR;

/// <summary>
/// 识别文本块工具类
/// </summary>
public static class TextBlockUtil
{
    /// <summary>
    /// 解析身份证姓名
    /// </summary>
    /// <param name="textBlocks"></param>
    /// <returns></returns>
    public static string ReadIdCardName(List<TextBlock> textBlocks)
    {
        var result = "";
        foreach (var item in textBlocks)
        {
            var txt = item.Text.Replace(" ", "").Trim();
            if (txt.Contains("性别") || txt.Contains("民族") || txt.Contains("住址") || txt.Contains("公民身份证号码") || txt.Contains("身份") || txt.Contains("号码"))
                continue;

            if (Regex.IsMatch(txt, @"^姓名[\u4e00-\u9fa5]{2,4}$"))
            {
                result = txt.TrimStart('姓', '名');
                break;
            }
            else if (Regex.IsMatch(txt, @"^名[\u4e00-\u9fa5]{2,4}$"))
            {
                result = txt.TrimStart('名');
                break;
            }
            else if (Regex.IsMatch(txt, @"^[\u4e00-\u9fa5]{2,4}$"))
            {
                result = txt;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 解析身份证号码
    /// </summary>
    /// <param name="textBlocks"></param>
    /// <returns></returns>
    public static string ReadIdCardNo(List<TextBlock> textBlocks)
    {
        var result = "";
        foreach (var item in textBlocks)
        {
            var txt = item.Text.Replace(" ", "").Trim();
            if (Regex.IsMatch(txt, @"^\d{15}$|^\d{17}(\d|X|x)$"))
            {
                result = txt;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 解析身份证地址
    /// </summary>
    /// <param name="textBlocks"></param>
    /// <returns></returns>
    public static string ReadIdCardAddress(List<TextBlock> textBlocks)
    {
        var sb = new StringBuilder();
        string[] temps = { "省", "市", "县", "区", "镇", "乡", "村", "组", "室", "栋", "街道", "号" };

        foreach (var item in textBlocks)
        {
            var txt = item.Text.Replace(" ", "").Trim();
            if (txt.Contains("姓名") || txt.Contains("号码"))
                continue;

            if (temps.Where(t => txt.Contains(t)).Count() > 0)
            {
                sb.Append(txt);
            }
        }
        sb = sb.Replace("住址", "");
        return sb.ToString();
    }
}