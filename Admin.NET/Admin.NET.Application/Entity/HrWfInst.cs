namespace Admin.NET.Application;

/// <summary>
/// 审批流程实例
/// </summary>
[Tenant(ApplicationConst.HrDbConfigId)]
[SugarTable("hr_wf_inst", "审批流程实例")]
public class HrWfInst : EntityBase
{
    [SugarColumn(ColumnName = "biz_type", ColumnDescription = "业务类型", Length = 50)]
    public string BizType { get; set; }

    [SugarColumn(ColumnName = "biz_id", ColumnDescription = "业务单Id", Length = 64)]
    public string BizId { get; set; }

    [SugarColumn(ColumnName = "def_id", ColumnDescription = "流程定义Id")]
    public long DefId { get; set; }

    [SugarColumn(ColumnName = "form_id", ColumnDescription = "表单方案Id")]
    public long FormId { get; set; }

    [SugarColumn(ColumnName = "status", ColumnDescription = "状态", Length = 20)]
    public string Status { get; set; }

    [SugarColumn(ColumnName = "title", ColumnDescription = "标题", Length = 200, IsNullable = true)]
    public string? Title { get; set; }

    [SugarColumn(ColumnName = "applicant_user_id", ColumnDescription = "发起人")]
    public long ApplicantUserId { get; set; }

    [SugarColumn(ColumnName = "subject_emp_id", ColumnDescription = "当事人员工号", Length = 50, IsNullable = true)]
    public string? SubjectEmpId { get; set; }

    [SugarColumn(ColumnName = "subject_cc_code", ColumnDescription = "单据部门内码", Length = 64)]
    public string SubjectCcCode { get; set; }

    [SugarColumn(ColumnName = "form_data_json", ColumnDescription = "表单数据JSON", ColumnDataType = "nvarchar(max)")]
    public string FormDataJson { get; set; }

    [SugarColumn(ColumnName = "form_schema_snapshot", ColumnDescription = "表单Schema快照", ColumnDataType = "nvarchar(max)")]
    public string FormSchemaSnapshot { get; set; }

    [SugarColumn(ColumnName = "def_nodes_snapshot", ColumnDescription = "节点定义快照", ColumnDataType = "nvarchar(max)")]
    public string DefNodesSnapshot { get; set; }

    [SugarColumn(ColumnName = "current_node_key", ColumnDescription = "当前节点", Length = 50, IsNullable = true)]
    public string? CurrentNodeKey { get; set; }

    [SugarColumn(ColumnName = "allow_edit_while_running", ColumnDescription = "审批中允许改字段")]
    public bool AllowEditWhileRunning { get; set; }

    [SugarColumn(ColumnName = "edit_by_applicant", ColumnDescription = "发起人可改")]
    public bool EditByApplicant { get; set; }

    [SugarColumn(ColumnName = "edit_by_assignee", ColumnDescription = "审批人可改")]
    public bool EditByAssignee { get; set; }

    [SugarColumn(ColumnName = "started_time", ColumnDescription = "发起时间")]
    public DateTime StartedTime { get; set; }

    [SugarColumn(ColumnName = "finished_time", ColumnDescription = "结束时间", IsNullable = true)]
    public DateTime? FinishedTime { get; set; }
}
