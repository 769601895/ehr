namespace Admin.NET.Application;

/// <summary>异动模块常量：状态、字段键、单号前缀</summary>
public static class HrEmpChgConst
{
    /// <summary>单号前缀</summary>
    public const string NoPrefix = "CHG";

    /// <summary>异动类别代码类</summary>
    public const string CodeFieldType = "EmpChgType";

    /// <summary>
    /// 本模块对应的审批流程业务类型（与测试用 hrEmh 分开）。
    /// 无启用流程时：提交即自动通过。
    /// </summary>
    public const string WfBizType = "hrEmpChg";

    // ---------- 单据状态（存库） ----------
    public const string StatusDraft = "Draft";
    public const string StatusInReview = "InReview";
    public const string StatusApproved = "Approved";
    public const string StatusEffective = "Effective";
    public const string StatusRejected = "Rejected";
    public const string StatusVoided = "Voided";

    /// <summary>发起拦截：草稿/审核中/待生效/已生效</summary>
    public static readonly string[] BlockStatuses =
    [
        StatusDraft, StatusInReview, StatusApproved, StatusEffective
    ];

    // ---------- 明细字段键（与人员字段池对齐） ----------
    public const string FieldCcCode = "ccCode";
    public const string FieldPostId = "postId";

    public const string ColCcCode = "cc_code";
    public const string ColPostId = "post_id";

    public const string LabelCcCode = "部门";
    public const string LabelPostId = "岗位";
}
