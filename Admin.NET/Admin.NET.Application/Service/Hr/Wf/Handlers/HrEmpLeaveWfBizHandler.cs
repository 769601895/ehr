namespace Admin.NET.Application;

/// <summary>
/// 员工离职（hrEmpLeave）：通过 → 待生效；计划日=离职日+offset 写 emp_state/emp_lea_date + h_emh_hist。
/// </summary>
public class HrEmpLeaveWfBizHandler : IHrWfBizHandler, ITransient
{
    private readonly HrWfEffectApplyService _effect;

    public HrEmpLeaveWfBizHandler(HrWfEffectApplyService effect)
    {
        _effect = effect;
    }

    public string BizType => HrWfConsts.BizTypeLeave;

    public Task OnApprovedAsync(HrWfCallback ctx) => _effect.OnApprovedAsync(ctx);

    public Task OnRejectedAsync(HrWfCallback ctx)
    {
        var bizId = string.IsNullOrWhiteSpace(ctx.BizId) ? ctx.InstId.ToString() : ctx.BizId.Trim();
        return _effect.CancelPendingAsync(ctx.BizType, bizId, "流程驳回");
    }

    public Task OnCancelledAsync(HrWfCallback ctx)
    {
        var bizId = string.IsNullOrWhiteSpace(ctx.BizId) ? ctx.InstId.ToString() : ctx.BizId.Trim();
        return _effect.CancelPendingAsync(ctx.BizType, bizId, "流程撤销");
    }
}
