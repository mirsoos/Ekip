using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Gamification.Commands.SubmitScore
{
    public class SubmitScoreCommandHandler : IRequestHandler<SubmitScoreCommand, Unit>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IUserWriteRepository _userWrite;
        public SubmitScoreCommandHandler(IEventPublisher eventPublisher , IUserWriteRepository userWrite)
        {
            _eventPublisher = eventPublisher;
            _userWrite = userWrite;
        }

        public async Task<Unit> Handle(SubmitScoreCommand request, CancellationToken cancellationToken)
        {
            await _userWrite.UpdateScoreAsync(request.TargetUserRef,request.ScoreGiven,cancellationToken);

            await _eventPublisher.Publish(new ScoreSubmittedEvent
            {
                RequestRef = request.RequestRef,
                SourceUserRef = request.SourceUserRef,
                TargetUserRef = request.TargetUserRef,
                ScoreGiven = request.ScoreGiven,
                Comment = request.Comment
            },cancellationToken);

            return Unit.Value;
        }
    }
}
