namespace Admin.NET.Application;

/// <summary>
/// 人员资料 h_emp_mstr
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("h_emp_mstr", "人员资料")]
[SugarIndex("ix_h_emp_mstr_cc_code", nameof(CcCode), OrderByType.Asc)]
[SugarIndex("ix_h_emp_mstr_emp_state", nameof(EmpState), OrderByType.Asc)]
public partial class HrEmployee
{
    /// <summary>内部主键（与 emp_id 组成复合主键）</summary>
    [SugarColumn(ColumnName = "iempid", ColumnDescription = "内部主键", IsPrimaryKey = true)]
    public long IEmpId { get; set; }

    [SugarColumn(ColumnName = "emp_id", ColumnDescription = "工号", IsPrimaryKey = true, Length = 50)]
    public string EmpId { get; set; }

    /// <summary>所属部门，对应 cc_mstr.cc_code</summary>
    [SugarColumn(ColumnName = "cc_code", ColumnDescription = "所属部门", Length = 50, IsNullable = true)]
    public string? CcCode { get; set; }

    [SugarColumn(ColumnName = "duty_id", ColumnDescription = "职务", Length = 50, IsNullable = true)]
    public string? DutyId { get; set; }

    [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位", Length = 50, IsNullable = true)]
    public string? PostId { get; set; }

    [SugarColumn(ColumnName = "emp_name", ColumnDescription = "姓名", Length = 100, IsNullable = true)]
    public string? EmpName { get; set; }

    /// <summary>人员状态：1在职 2停职 3离职；合并时仅处理 &lt;&gt; 3</summary>
    [SugarColumn(ColumnName = "emp_state", ColumnDescription = "人员状态", IsNullable = true)]
    public int? EmpState { get; set; }

    [SugarColumn(ColumnName = "emp_free", ColumnDescription = "自由标记", IsNullable = true)]
    public byte? EmpFree { get; set; }

    [SugarColumn(ColumnName = "emp_in_date", ColumnDescription = "公司入职日", IsNullable = true)]
    public DateTime? EmpInDate { get; set; }

    [SugarColumn(ColumnName = "emp_beg_date", ColumnDescription = "试用起始日", IsNullable = true)]
    public DateTime? EmpBegDate { get; set; }

    [SugarColumn(ColumnName = "emp_try_date", ColumnDescription = "试用到期日", IsNullable = true)]
    public DateTime? EmpTryDate { get; set; }

    [SugarColumn(ColumnName = "emp_from_date", ColumnDescription = "转正日", IsNullable = true)]
    public DateTime? EmpFromDate { get; set; }

    [SugarColumn(ColumnName = "emp_sal", ColumnDescription = "薪酬类别", Length = 2, IsNullable = true)]
    public string? EmpSal { get; set; }

    [SugarColumn(ColumnName = "emp_sex", ColumnDescription = "性别", Length = 20, IsNullable = true)]
    public string? EmpSex { get; set; }

    [SugarColumn(ColumnName = "emp_card", ColumnDescription = "身份证号", Length = 50, IsNullable = true)]
    public string? EmpCard { get; set; }

    [SugarColumn(ColumnName = "emp_birth", ColumnDescription = "出生日期", IsNullable = true)]
    public DateTime? EmpBirth { get; set; }

    [SugarColumn(ColumnName = "emp_rgt", ColumnDescription = "户籍类型", Length = 4, IsNullable = true)]
    public string? EmpRgt { get; set; }

    [SugarColumn(ColumnName = "emp_province", ColumnDescription = "省份", Length = 50, IsNullable = true)]
    public string? EmpProvince { get; set; }

    [SugarColumn(ColumnName = "emp_residence", ColumnDescription = "户口所在地", Length = 80, IsNullable = true)]
    public string? EmpResidence { get; set; }

    [SugarColumn(ColumnName = "emp_nation", ColumnDescription = "民族", Length = 50, IsNullable = true)]
    public string? EmpNation { get; set; }

    [SugarColumn(ColumnName = "emp_marry", ColumnDescription = "婚姻状况", Length = 20, IsNullable = true)]
    public string? EmpMarry { get; set; }

    [SugarColumn(ColumnName = "emp_home", ColumnDescription = "家庭住址", Length = 100, IsNullable = true)]
    public string? EmpHome { get; set; }

    [SugarColumn(ColumnName = "emp_addr", ColumnDescription = "现住址", Length = 100, IsNullable = true)]
    public string? EmpAddr { get; set; }

    [SugarColumn(ColumnName = "emp_addrter", ColumnDescription = "联系电话", Length = 50, IsNullable = true)]
    public string? EmpAddrter { get; set; }

    [SugarColumn(ColumnName = "emp_longmon", ColumnDescription = "入司前工龄(月)", IsNullable = true)]
    public int? EmpLongmon { get; set; }

    [SugarColumn(ColumnName = "emp_kq_lea_date", ColumnDescription = "考勤结束日", IsNullable = true)]
    public DateTime? EmpKqLeaDate { get; set; }

    [SugarColumn(ColumnName = "emp_kq_indate", ColumnDescription = "考勤开始日", IsNullable = true)]
    public DateTime? EmpKqIndate { get; set; }

    [SugarColumn(ColumnName = "emp_card_date", ColumnDescription = "身份证发证日期", IsNullable = true)]
    public DateTime? EmpCardDate { get; set; }

    [SugarColumn(ColumnName = "emp_card_limit", ColumnDescription = "身份证期限", IsNullable = true)]
    public int? EmpCardLimit { get; set; }

    [SugarColumn(ColumnName = "emp_type", ColumnDescription = "人员类别", Length = 50, IsNullable = true)]
    public string? EmpType { get; set; }

    [SugarColumn(ColumnName = "emp_card_overdate", ColumnDescription = "身份证到期日", IsNullable = true)]
    public DateTime? EmpCardOverdate { get; set; }

    [SugarColumn(ColumnName = "emp_work_date", ColumnDescription = "参加工作时间", IsNullable = true)]
    public DateTime? EmpWorkDate { get; set; }

    [SugarColumn(ColumnName = "emp_in_date_group", ColumnDescription = "集团入职日", IsNullable = true)]
    public DateTime? EmpInDateGroup { get; set; }

    /// <summary>照片路径/URL（上传后写入 SysFile.Url）</summary>
    [SugarColumn(ColumnName = "emp_pic", ColumnDescription = "照片", Length = 255, IsNullable = true)]
    public string? EmpPic { get; set; }

    [SugarColumn(ColumnName = "bs_id", ColumnDescription = "自增辅助键", IsIdentity = true, IsNullable = false)]
    public int BsId { get; set; }
}
