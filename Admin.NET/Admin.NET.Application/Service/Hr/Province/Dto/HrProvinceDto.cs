namespace Admin.NET.Application;

public class HrProvincePageInput : BasePageInput
{
    public new string? Keyword { get; set; }
    public string? Status { get; set; }
}

public class HrProvinceOutput
{
    public long Id { get; set; }
    public string ProvinceCode { get; set; }
    public string ProvinceName { get; set; }
    public int OrderNo { get; set; }
    public string Status { get; set; }
    public string? Remark { get; set; }
}

public class HrProvinceSaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "省代码不能为空")]
    [MaxLength(12)]
    public string ProvinceCode { get; set; }

    [Required(ErrorMessage = "省名称不能为空")]
    [MaxLength(50)]
    public string ProvinceName { get; set; }

    public int OrderNo { get; set; } = 100;

    [MaxLength(20)]
    public string Status { get; set; } = HrRegionConsts.StatusActive;

    [MaxLength(256)]
    public string? Remark { get; set; }
}

public class HrProvinceDeleteInput
{
    [Required]
    public long Id { get; set; }
}

public class HrProvinceLangSaveInput
{
    [Required(ErrorMessage = "省代码不能为空")]
    public string ProvinceCode { get; set; }

    public List<HrUiLangItemInput> Labels { get; set; } = [];
}

