namespace Admin.NET.Application;

public static class HrEmpSuspendConst
{
    public const string NoPrefix = "TZ";
    public const string EmhType = "TZ";

    public static readonly string[] OpenStatuses =
    [
        HrEmpLeaveConst.StatusDraft,
        HrEmpLeaveConst.StatusInReview,
        HrEmpLeaveConst.StatusApproved
    ];
}
