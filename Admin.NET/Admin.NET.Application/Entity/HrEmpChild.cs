namespace Admin.NET.Application;

/// <summary>家庭成员 h_emp_family</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emp_family", "家庭成员")]
[SugarIndex("ix_h_emp_family_emp", nameof(EmpId), OrderByType.Asc, nameof(SortNo), OrderByType.Asc)]
public class HrEmpFamily
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnName = "emp_id", Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "sort_no")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "relation", Length = 20, IsNullable = true)]
    public string? Relation { get; set; }

    [SugarColumn(ColumnName = "member_name", Length = 50, IsNullable = true)]
    public string? MemberName { get; set; }

    [SugarColumn(ColumnName = "workplace", Length = 100, IsNullable = true)]
    public string? Workplace { get; set; }

    [SugarColumn(ColumnName = "duty", Length = 50, IsNullable = true)]
    public string? Duty { get; set; }
}

/// <summary>教育经历 h_emp_edu</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emp_edu", "教育经历")]
[SugarIndex("ix_h_emp_edu_emp", nameof(EmpId), OrderByType.Asc, nameof(SortNo), OrderByType.Asc)]
public class HrEmpEdu
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnName = "emp_id", Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "sort_no")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "start_date", IsNullable = true)]
    public DateTime? StartDate { get; set; }

    [SugarColumn(ColumnName = "end_date", IsNullable = true)]
    public DateTime? EndDate { get; set; }

    [SugarColumn(ColumnName = "school", Length = 100, IsNullable = true)]
    public string? School { get; set; }

    [SugarColumn(ColumnName = "major", Length = 100, IsNullable = true)]
    public string? Major { get; set; }

    [SugarColumn(ColumnName = "degree", Length = 20, IsNullable = true)]
    public string? Degree { get; set; }

    [SugarColumn(ColumnName = "is_highest")]
    public bool IsHighest { get; set; }

    [SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>工作履历 h_emp_career</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emp_career", "工作履历")]
[SugarIndex("ix_h_emp_career_emp", nameof(EmpId), OrderByType.Asc, nameof(SortNo), OrderByType.Asc)]
public class HrEmpCareer
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnName = "emp_id", Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "sort_no")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "start_date", IsNullable = true)]
    public DateTime? StartDate { get; set; }

    [SugarColumn(ColumnName = "end_date", IsNullable = true)]
    public DateTime? EndDate { get; set; }

    [SugarColumn(ColumnName = "workplace", Length = 100, IsNullable = true)]
    public string? Workplace { get; set; }

    [SugarColumn(ColumnName = "dept_name", Length = 100, IsNullable = true)]
    public string? DeptName { get; set; }

    [SugarColumn(ColumnName = "post_name", Length = 50, IsNullable = true)]
    public string? PostName { get; set; }

    [SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>职称情况 h_emp_title</summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emp_title", "职称情况")]
[SugarIndex("ix_h_emp_title_emp", nameof(EmpId), OrderByType.Asc, nameof(SortNo), OrderByType.Asc)]
public class HrEmpTitle
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public long Id { get; set; }

    [SugarColumn(ColumnName = "emp_id", Length = 50)]
    public string EmpId { get; set; }

    [SugarColumn(ColumnName = "sort_no")]
    public int SortNo { get; set; }

    [SugarColumn(ColumnName = "title_name", Length = 50, IsNullable = true)]
    public string? TitleName { get; set; }

    [SugarColumn(ColumnName = "title_level", Length = 20, IsNullable = true)]
    public string? TitleLevel { get; set; }

    [SugarColumn(ColumnName = "cert_no", Length = 50, IsNullable = true)]
    public string? CertNo { get; set; }

    [SugarColumn(ColumnName = "obtain_date", IsNullable = true)]
    public DateTime? ObtainDate { get; set; }

    [SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}
