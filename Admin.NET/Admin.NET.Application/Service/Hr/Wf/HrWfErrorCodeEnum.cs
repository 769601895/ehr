namespace Admin.NET.Application;

/// <summary>
/// 审批引擎业务错误码（面向用户的本地化文案；领域层仍可用 Wf.* 键，经 <see cref="HrWfErrors"/> 映射）。
/// </summary>
[ErrorCodeType]
[Description("审批引擎错误码")]
public enum HrWfErrorCodeEnum
{
    [ErrorCodeItemMetadata("审核等级不存在")]
    LevelNotFound,

    [ErrorCodeItemMetadata("审核等级编码已存在")]
    LevelCodeDup,

    [ErrorCodeItemMetadata("该审核等级仍被审批角色使用，无法删除")]
    LevelHasRoles,

    [ErrorCodeItemMetadata("审批角色不存在")]
    RoleNotFound,

    [ErrorCodeItemMetadata("审批角色编码已存在")]
    RoleCodeDup,

    [ErrorCodeItemMetadata("不能挂到自身或下级节点下，否则会形成环")]
    RoleTreeCycle,

    [ErrorCodeItemMetadata("同一汇报路径上审核等级不能重复，请更换等级或上级节点")]
    DuplicateLevelOnPath,

    [ErrorCodeItemMetadata("当事人岗位匹配到多个同层级起点角色，无法唯一确定汇报起点")]
    AmbiguousStartRole,

    [ErrorCodeItemMetadata("审批表单不存在或未启用")]
    FormNotFound,

    [ErrorCodeItemMetadata("表单字段键重复")]
    FormFieldKeyDup,

    [ErrorCodeItemMetadata("表单已被流程定义引用，无法删除")]
    FormInUse,

    [ErrorCodeItemMetadata("流程定义不存在或未启用")]
    DefNotFound,

    [ErrorCodeItemMetadata("流程定义至少需要一个节点")]
    DefNoNodes,

    [ErrorCodeItemMetadata("流程节点键重复")]
    DefNodeKeyDup,

    [ErrorCodeItemMetadata("审批实例不存在")]
    InstNotFound,

    [ErrorCodeItemMetadata("已存在进行中的同类单据，请勿重复提交")]
    DuplicateRunning,

    [ErrorCodeItemMetadata("当前任务不是待办或已处理")]
    TaskNotPending,

    [ErrorCodeItemMetadata("您不是当前待办处理人")]
    NotAssignee,

    [ErrorCodeItemMetadata("审批进行中，不允许修改表单")]
    FormEditDenied,

    [ErrorCodeItemMetadata("驳回时必须填写意见")]
    RejectOpinionRequired,

    [ErrorCodeItemMetadata("表单校验未通过，请检查必填项与格式")]
    ValidationFailed,

    [ErrorCodeItemMetadata("开单必须指定当事人")]
    SubjectEmpRequired,

    [ErrorCodeItemMetadata("当事人未设置岗位，无法发起审批")]
    SubjectPostMissing,

    [ErrorCodeItemMetadata("当事人岗位未绑定任何审批角色，无法发起审批")]
    SubjectPostNoRole,

    [ErrorCodeItemMetadata("当事人已处于汇报树根节点，没有上级可审批")]
    SubjectIsRoot,

    [ErrorCodeItemMetadata("汇报路径上找不到审批人，单据已取消")]
    NoAssigneeOnPath,
}
