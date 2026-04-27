using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Gamification.Commands.SubmitScore
{
    public class SubmitScoreCommandHandler : IRequestHandler<SubmitScoreCommand, Unit>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IProfileWriteRepository _profileWrite;
        public SubmitScoreCommandHandler(IPublishEndpoint publishEndpoint , IProfileWriteRepository profileWrite)
        {
            _publishEndpoint = publishEndpoint;
            _profileWrite = profileWrite;
        }

        public async Task<Unit> Handle(SubmitScoreCommand request, CancellationToken cancellationToken)
        {
            await _profileWrite.UpdateScoreAsync(request.TargetUserProfileRef,request.ScoreGiven,cancellationToken);

            await _publishEndpoint.Publish(new ScoreSubmittedEvent
            {
                RequestRef = request.RequestRef,
                SourceUserProfileRef = request.SourceUserProfileRef,
                TargetUserProfileRef = request.TargetUserProfileRef,
                ScoreGiven = request.ScoreGiven,
                Comment = request.Comment
            });

            return Unit.Value;
        }
    }
}
