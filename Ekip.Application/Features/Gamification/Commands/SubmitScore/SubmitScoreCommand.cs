using MediatR;

namespace Ekip.Application.Features.Gamification.Commands.SubmitScore
{
    public class SubmitScoreCommand : IRequest<Unit>
    {
        public Guid RequestRef { get; set; }
        public Guid TargetUserRef { get; set; }
        public Guid SourceUserRef { get; set; }
        public int ScoreGiven { get; set; }
        public string? Comment { get; set; }
    }
}
