namespace Admin.NET.Application;

/// <summary>
/// 员工异动（hrEmh）：审核通过 → 待生效队列；到期写部门/岗位。驳回/撤销取消待生效。
/// </summary>
public class HrEmhWfBizHandler : IHrWfBizHandler, ITransient
{
    private readonly HrWfEffectApplyService _effect;

    public HrEmhWfBizHandler(HrWfEffectApplyService effect)
    {
        _effect = effect;
    }

    public string BizType => HrWfConsts.BizTypeEmh;

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
