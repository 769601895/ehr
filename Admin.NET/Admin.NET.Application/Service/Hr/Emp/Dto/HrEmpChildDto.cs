namespace Admin.NET.Application;

public class HrEmpChildSaveInput
{
    public string Section { get; set; }

    public string EmpId { get; set; }

    /// <summary>空表示新增</summary>
    public string? Id { get; set; }

    public Dictionary<string, string?>? Values { get; set; }
}

public class HrEmpChildDeleteInput
{
    public string Section { get; set; }

    public string EmpId { get; set; }

    public string Id { get; set; }
}

public class HrEmpChildSortInput
{
    public string Section { get; set; }

    public string EmpId { get; set; }

    public List<string> Ids { get; set; } = [];
}

public class HrEmpChildCountOutput
{
    public int Family { get; set; }

    public int Edu { get; set; }

    public int Work { get; set; }

    public int Title { get; set; }
}
