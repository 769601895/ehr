namespace Admin.NET.Application;

/// <summary>
/// 人员资料分页查询
/// </summary>
public class HrEmpPageInput : BasePageInput
{
    /// <summary>限定部门编码（含子树，按 cc_level 前缀）</summary>
    public string? CcCode { get; set; }

    /// <summary>人员状态：1在职 2停职 3离职；空=不限</summary>
    public int? EmpState { get; set; }

    /// <summary>工号（精确/模糊由 Keyword 兼用时另传）</summary>
    public string? EmpId { get; set; }

    /// <summary>姓名</summary>
    public string? EmpName { get; set; }
}

/// <summary>
/// 人员资料详情/列表输出
/// </summary>
public partial class HrEmpOutput
{
    public long IEmpId { get; set; }
    public string EmpId { get; set; }
    public string? CcCode { get; set; }
    public string? CcDesc { get; set; }
    public string? DutyId { get; set; }
    public string? PostId { get; set; }
    public string? EmpName { get; set; }
    public int? EmpState { get; set; }
    public byte? EmpFree { get; set; }
    public DateTime? EmpInDate { get; set; }
    public DateTime? EmpBegDate { get; set; }
    public DateTime? EmpTryDate { get; set; }
    public DateTime? EmpFromDate { get; set; }
    public string? EmpSal { get; set; }
    public string? EmpSex { get; set; }
    public string? EmpCard { get; set; }
    public DateTime? EmpBirth { get; set; }
    /// <summary>年龄（由出生日期计算，不落库）</summary>
    public int? Age { get; set; }
    public string? EmpRgt { get; set; }
    public string? EmpProvince { get; set; }
    public string? EmpResidence { get; set; }
    public string? EmpNation { get; set; }
    public string? EmpMarry { get; set; }
    public string? EmpHome { get; set; }
    public string? EmpAddr { get; set; }
    public string? EmpAddrter { get; set; }
    public int? EmpLongmon { get; set; }
    public DateTime? EmpKqLeaDate { get; set; }
    public DateTime? EmpKqIndate { get; set; }
    public DateTime? EmpCardDate { get; set; }
    public int? EmpCardLimit { get; set; }
    public string? EmpType { get; set; }
    public DateTime? EmpCardOverdate { get; set; }
    public DateTime? EmpWorkDate { get; set; }
    public DateTime? EmpInDateGroup { get; set; }
    /// <summary>照片 URL</summary>
    public string? EmpPic { get; set; }
    public int BsId { get; set; }
}

/// <summary>
/// 新增/编辑人员资料
/// </summary>
public partial class HrEmpSaveInput
{
    [Required(ErrorMessage = "工号不能为空")]
    public string EmpId { get; set; }

    public string? CcCode { get; set; }
    public string? DutyId { get; set; }
    public string? PostId { get; set; }

    [Required(ErrorMessage = "姓名不能为空")]
    public string EmpName { get; set; }

    /// <summary>默认 1=在职</summary>
    public int? EmpState { get; set; }

    public DateTime? EmpInDate { get; set; }
    public DateTime? EmpBegDate { get; set; }
    public DateTime? EmpTryDate { get; set; }
    public DateTime? EmpFromDate { get; set; }
    public string? EmpSal { get; set; }
    public string? EmpSex { get; set; }
    public string? EmpCard { get; set; }
    public DateTime? EmpBirth { get; set; }
    public string? EmpRgt { get; set; }
    public string? EmpProvince { get; set; }
    public string? EmpResidence { get; set; }
    public string? EmpNation { get; set; }
    public string? EmpMarry { get; set; }
    public string? EmpHome { get; set; }
    public string? EmpAddr { get; set; }
    public string? EmpAddrter { get; set; }
    public int? EmpLongmon { get; set; }
    public DateTime? EmpKqLeaDate { get; set; }
    public DateTime? EmpKqIndate { get; set; }
    public DateTime? EmpCardDate { get; set; }
    public int? EmpCardLimit { get; set; }
    public string? EmpType { get; set; }
    public DateTime? EmpCardOverdate { get; set; }
    public DateTime? EmpWorkDate { get; set; }
    public DateTime? EmpInDateGroup { get; set; }
    /// <summary>照片 URL</summary>
    public string? EmpPic { get; set; }
}

/// <summary>
/// 按工号删除/离职
/// </summary>
public class HrEmpDeleteInput
{
    [Required(ErrorMessage = "工号不能为空")]
    public string EmpId { get; set; }
}
