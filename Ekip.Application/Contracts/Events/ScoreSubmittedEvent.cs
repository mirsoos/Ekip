
namespace Ekip.Application.Contracts.Events
{
    public record ScoreSubmittedEvent
    {
        public Guid RequestRef { get; init; }
        public Guid TargetUserProfileRef { get; init; }
        public Guid SourceUserProfileRef { get; init; }
        public double ScoreGiven { get; init; }
        public string? Comment { get; init; }
    }
}
