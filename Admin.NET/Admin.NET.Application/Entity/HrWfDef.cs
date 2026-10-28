namespace Admin.NET.Application;

/// <summary>
/// 审批流程定义
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_def", "审批流程定义")]
public class HrWfDef : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "code", ColumnDescription = "流程编码", Length = 50)]
    public string Code { get; set; }

    [SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "form_id", ColumnDescription = "表单方案Id")]
    public long FormId { get; set; }

    [SugarColumn(ColumnName = "version", ColumnDescription = "版本")]
    public int Version { get; set; } = 1;

    [SugarColumn(ColumnName = "enabled", ColumnDescription = "启用")]
    public bool Enabled { get; set; } = true;

    [SugarColumn(ColumnName = "allow_edit_while_running", ColumnDescription = "审批中允许改字段")]
    public bool AllowEditWhileRunning { get; set; }

    [SugarColumn(ColumnName = "edit_by_applicant", ColumnDescription = "发起人可改")]
    public bool EditByApplicant { get; set; }

    [SugarColumn(ColumnName = "edit_by_assignee", ColumnDescription = "审批人可改")]
    public bool EditByAssignee { get; set; }

    [SugarColumn(ColumnName = "default_timeout_hours", ColumnDescription = "默认超时小时")]
    public int DefaultTimeoutHours { get; set; }

    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", Length = 256, IsNullable = true)]
    public string? Remark { get; set; }
}
