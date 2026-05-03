using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Gamification.Commands.SubmitScore
{
    public class SubmitScoreCommandHandler : IRequestHandler<SubmitScoreCommand, Unit>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IUserWriteRepository _userWrite;
        private readonly IUnitOfWork _unitOfWork;
        public SubmitScoreCommandHandler(IEventPublisher eventPublisher , IUserWriteRepository userWrite, IUnitOfWork unitOfWork)
        {
            _eventPublisher = eventPublisher;
            _userWrite = userWrite;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SubmitScoreCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                await _userWrite.UpdateScoreAsync(request.TargetUserRef,request.ScoreGiven,innerCt);
                await _eventPublisher.Publish(new ScoreSubmittedEvent
                {
                    RequestRef = request.RequestRef,
                    SourceUserRef = request.SourceUserRef,
                    TargetUserRef = request.TargetUserRef,
                    ScoreGiven = request.ScoreGiven,
                    Comment = request.Comment
                }, innerCt);
            },cancellationToken);

            return Unit.Value;
        }
    }
}
