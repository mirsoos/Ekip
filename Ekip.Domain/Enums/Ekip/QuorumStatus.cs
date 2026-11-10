namespace Ekip.Domain.Enums.Ekip
{
    public enum QuorumStatus
    {
        None = 1,
        PendingPoll,
        ConfirmedToProceed,
        AbortedByVote
    }
}
