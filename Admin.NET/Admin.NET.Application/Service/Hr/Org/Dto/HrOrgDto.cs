namespace Admin.NET.Application;

/// <summary>
/// 部门树查询（含列排序：Field/Order/DescStr）
/// </summary>
public class HrOrgListInput : BasePageInput
{
    /// <summary>仅返回该节点下子树；空为整树</summary>
    public string? ParentCcCode { get; set; }

    /// <summary>true=含停用（历史部门）</summary>
    public bool IncludeDisabled { get; set; }

    /// <summary>人数统计人员状态：1在职 2停职 3离职（默认在职）</summary>
    public int? EmpState { get; set; } = 1;

    /// <summary>true=人数统计全部人员，忽略 EmpState</summary>
    public bool EmpStateAll { get; set; }
}

/// <summary>
/// 部门明细分页查询（Keyword 继承自 BaseFilter：名称/编码/代号）
/// </summary>
public class HrOrgPageInput : BasePageInput
{
    /// <summary>限定在该部门层级码前缀下（含子级）</summary>
    public string? ParentCcCode { get; set; }

    /// <summary>true=含停用</summary>
    public bool IncludeDisabled { get; set; }
}

/// <summary>
/// 部门树/列表输出（基于 cc_mstr）
/// </summary>
public class HrOrgOutput
{
    /// <summary>自增辅助键，便于树控件绑定</summary>
    public int Id { get; set; }

    /// <summary>父节点辅助键</summary>
    public int Pid { get; set; }

    public string CcCode { get; set; }
    public string? CcCodeParent { get; set; }
    public string? CcDept { get; set; }
    public string CcDesc { get; set; }
    public string? CcDescEn { get; set; }
    public string CcLevel { get; set; }
    public int? CcGroup { get; set; }
    public string? CcIndet { get; set; }
    public float? CcSub { get; set; }
    public string? CcSite { get; set; }
    public int? CcState { get; set; }
    public string? CcCost { get; set; }
    public float? CcWageRate { get; set; }
    public string? CcStatBasic { get; set; }
    public int? CcEnable { get; set; }
    public DateTime? CcDeleDate { get; set; }
    public string? CcCountGroup { get; set; }
    public string? CcUser { get; set; }
    public DateTime? CcDate { get; set; }
    public int? CcKqgroup { get; set; }
    public string? CcWoca { get; set; }
    public string? CcBzUnit { get; set; }
    public string? CcDeleRemark { get; set; }
    public string? CcLevelCensor { get; set; }
    public string? CcXzGroup { get; set; }
    public DateTime? CcCreateDate { get; set; }
    public string? MachineFloor { get; set; }
    public string? KqNewSysuser { get; set; }
    public DateTime? KqNewSysdate { get; set; }
    public string? CcCode2 { get; set; }

    /// <summary>本节点人数（含子部门汇总，受 empState 筛选）</summary>
    public int EmpCount { get; set; }

    public List<HrOrgOutput> Children { get; set; } = [];
}

/// <summary>
/// 保存部门
/// </summary>
public class HrOrgSaveInput
{
    /// <summary>部门编码；空则自动生成</summary>
    public string? CcCode { get; set; }

    /// <summary>父部门编码；空表示根级</summary>
    public string? CcCodeParent { get; set; }

    [Required(ErrorMessage = "部门名称不能为空")]
    public string CcDesc { get; set; }

    public string? CcDept { get; set; }
    public string? CcDescEn { get; set; }
    public int? CcGroup { get; set; }
    public string? CcIndet { get; set; }
    public float? CcSub { get; set; }
    public string? CcSite { get; set; }
    public int? CcState { get; set; }
    public string? CcCost { get; set; }
    public float? CcWageRate { get; set; }
    public string? CcStatBasic { get; set; }
    public string? CcCountGroup { get; set; }
    public int? CcKqgroup { get; set; }
    public string? CcWoca { get; set; }
    public string? CcBzUnit { get; set; }
    public string? CcLevelCensor { get; set; }
    public string? CcXzGroup { get; set; }
    public string? MachineFloor { get; set; }
    public string? CcCode2 { get; set; }
}

/// <summary>
/// 停用部门
/// </summary>
public class HrOrgDisableInput
{
    [Required(ErrorMessage = "部门编码不能为空")]
    public string CcCode { get; set; }

    public string? Remark { get; set; }
}

public class HrCodeItemOutput
{
    public string Value { get; set; }
    public string Label { get; set; }
}

public class HrCostItemOutput
{
    public string Value { get; set; }
    public string Label { get; set; }
}

public class HrShiftItemOutput
{
    public string Value { get; set; }
    public string Label { get; set; }
}

/// <summary>
/// 合并部门
/// </summary>
public class HrMergeDeptInput
{
    [Required(ErrorMessage = "请选择合并前部门一")]
    public string OldCode1 { get; set; }

    [Required(ErrorMessage = "请选择合并前部门二")]
    public string OldCode2 { get; set; }

    [Required(ErrorMessage = "请选择合并后目标部门")]
    public string NewCode { get; set; }
}

/// <summary>
/// 调动部门：将 FromCcCode 整棵子树挂到 ToParentCcCode 下
/// </summary>
public class HrTransferDeptInput
{
    [Required(ErrorMessage = "请选择调动部门")]
    public string FromCcCode { get; set; }

    /// <summary>调入后的归属父部门；空表示挂到根级</summary>
    public string? ToParentCcCode { get; set; }
}

/// <summary>
/// 按工号查询人事资料（账号关联带出姓名，只读）
/// </summary>
public class HrEmpBriefOutput
{
    public string EmpId { get; set; }
    public string? EmpName { get; set; }
}
