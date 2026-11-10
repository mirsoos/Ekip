namespace Ekip.Domain.Enums.Review
{
    public enum ProfileReviewModerationStatus
    {
        PendingByAI = 1,
        Approved,
        Rejected,
        EscalatedToAdmin
    }
}