namespace Admin.NET.Application;

public class HrCityPageInput : BasePageInput
{
    public string? ProvinceCode { get; set; }
    public new string? Keyword { get; set; }
    public string? Status { get; set; }
}

public class HrCityOutput
{
    public long Id { get; set; }
    public string ProvinceCode { get; set; }
    public string CityCode { get; set; }
    public string CityName { get; set; }
    public int OrderNo { get; set; }
    public string Status { get; set; }
    public string? Remark { get; set; }
}

public class HrCitySaveInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "所属省代码不能为空")]
    [MaxLength(12)]
    public string ProvinceCode { get; set; }

    [Required(ErrorMessage = "市代码不能为空")]
    [MaxLength(12)]
    public string CityCode { get; set; }

    [Required(ErrorMessage = "市名称不能为空")]
    [MaxLength(50)]
    public string CityName { get; set; }

    public int OrderNo { get; set; } = 100;

    [MaxLength(20)]
    public string Status { get; set; } = HrRegionConsts.StatusActive;

    [MaxLength(256)]
    public string? Remark { get; set; }
}

public class HrCityDeleteInput
{
    [Required]
    public long Id { get; set; }
}

public class HrCityLangSaveInput
{
    [Required(ErrorMessage = "市代码不能为空")]
    public string CityCode { get; set; }

    public List<HrUiLangItemInput> Labels { get; set; } = [];
}
