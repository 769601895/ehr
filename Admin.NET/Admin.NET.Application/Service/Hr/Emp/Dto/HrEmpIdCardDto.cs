namespace Admin.NET.Application;

/// <summary>是否启用二代证读卡（按当前用户管辖部门解析厂别；CcCode 忽略）</summary>
public class HrEmpIdCardFeatureInput
{
    /// <summary>已废弃：读卡开关不跟员工部门挂钩，保留字段仅兼容旧调用</summary>
    public string? CcCode { get; set; }
}

public class HrEmpIdCardFeatureOutput
{
    /// <summary>公司参数 IdCardV2Enabled=1</summary>
    public bool Enabled { get; set; }

    public int IdCardV2Enabled { get; set; }

    public string? SiteCode { get; set; }

    public string? Message { get; set; }
}

/// <summary>解析身份证（国籍非中国时跳过校验与拆解）</summary>
public class HrEmpIdCardParseInput
{
    public string? EmpCard { get; set; }
    public string? EmpCountry { get; set; }
    /// <summary>用于取公司参数 IdCardV2Enabled</summary>
    public string? CcCode { get; set; }
}

public class HrEmpIdCardParseOutput
{
    /// <summary>是否按中国身份证处理（空国籍或 CN）</summary>
    public bool NeedValidate { get; set; }

    public bool Valid { get; set; }

    public string? Message { get; set; }

    public DateTime? EmpBirth { get; set; }

    public string? EmpSex { get; set; }

    /// <summary>籍贯省码 → emp_province</summary>
    public string? EmpProvince { get; set; }

    /// <summary>籍贯市码 → emp_city</summary>
    public string? EmpCity { get; set; }

    public string? ProvinceName { get; set; }

    public string? CityName { get; set; }
}
