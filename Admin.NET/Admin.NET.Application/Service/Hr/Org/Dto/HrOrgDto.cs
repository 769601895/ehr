namespace Admin.NET.Application;

/// <summary>
/// 机构+人事部门聚合输出
/// </summary>
public class HrOrgOutput
{
    public long Id { get; set; }
    public long Pid { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public int? Level { get; set; }
    public string? Type { get; set; }
    public long? DirectorId { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public int OrderNo { get; set; }
    public StatusEnum Status { get; set; }
    public string? Remark { get; set; }
    public bool Disabled { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public List<HrOrgOutput> Children { get; set; }

    public string? CcCode { get; set; }
    public string? CcCodeParent { get; set; }
    public string? CcDept { get; set; }
    public string? CcDesc { get; set; }
    public string? CcLevel { get; set; }
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
    public string? CcDescEn { get; set; }
    public string? CcLevelCensor { get; set; }
    public string? CcXzGroup { get; set; }
    public DateTime? CcCreateDate { get; set; }
    public string? MachineFloor { get; set; }
    public string? KqNewSysuser { get; set; }
    public DateTime? KqNewSysdate { get; set; }
    public int CcIdentityId { get; set; }
    public int? ParentId { get; set; }
    public string? CcCode2 { get; set; }
    public long? OrgId { get; set; }
}

/// <summary>
/// 保存机构（结构字段 + cc_mstr 全字段）
/// </summary>
public class HrOrgSaveInput
{
    public long Id { get; set; }
    public long Pid { get; set; }

    [Required(ErrorMessage = "机构名称不能为空")]
    public string Name { get; set; }

    public string? Code { get; set; }
    public int? Level { get; set; }
    public string? Type { get; set; }
    public long? DirectorId { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public int OrderNo { get; set; } = 100;
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
    public string? Remark { get; set; }

    public string? CcCode { get; set; }
    public string? CcCodeParent { get; set; }
    public string? CcDept { get; set; }
    public string? CcDesc { get; set; }
    public string? CcLevel { get; set; }
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
    public string? CcDescEn { get; set; }
    public string? CcLevelCensor { get; set; }
    public string? CcXzGroup { get; set; }
    public DateTime? CcCreateDate { get; set; }
    public string? MachineFloor { get; set; }
    public string? KqNewSysuser { get; set; }
    public DateTime? KqNewSysdate { get; set; }
    public int CcIdentityId { get; set; }
    public int? ParentId { get; set; }
    public string? CcCode2 { get; set; }
    public long? OrgId { get; set; }
}

public class HrCodeItemOutput
{
    public string Value { get; set; }
    public string Label { get; set; }
}
