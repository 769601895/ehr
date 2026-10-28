namespace Admin.NET.Application;

public interface IHrWfBizHandler
{
    string BizType { get; }
    Task OnApprovedAsync(HrWfCallback ctx);
    Task OnRejectedAsync(HrWfCallback ctx);
    Task OnCancelledAsync(HrWfCallback ctx);
}

public class HrWfCallback
{
    public long InstId { get; set; }
    public string BizType { get; set; }
    public string BizId { get; set; }
    public string FormDataJson { get; set; }
    public long ApplicantUserId { get; set; }
    public string SubjectCcCode { get; set; }
}
