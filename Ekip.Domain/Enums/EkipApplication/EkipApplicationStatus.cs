namespace Ekip.Domain.Enums.EkipApplication
{
    public enum EkipApplicationStatus
    {
        WaitingCompanionApproval = 1,
        PendingHostApproval,
        Waitlisted,
        Approved,
        Rejected,
        Canceled,
        InvalidatedByCompanion
    }
}
