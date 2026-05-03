
namespace Ekip.Application.Contracts.Events
{
    public record ScoreSubmittedEvent
    {
        public Guid RequestRef { get; init; }
        public Guid TargetUserRef { get; init; }
        public Guid SourceUserRef { get; init; }
        public double ScoreGiven { get; init; }
        public string? Comment { get; init; }
    }
}
