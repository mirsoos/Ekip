using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.Enums.Requests.Enums;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AcceptOrRejectAssignment
{
    public class AcceptOrRejectAssignmentCommandHandler : IRequestHandler<AcceptOrRejectAssignmentCommand, AssignmentDecisionDto>
    {
        private readonly IRequestWriteRepository _requestWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        private readonly IUnitOfWork _unitOfWork;
        public AcceptOrRejectAssignmentCommandHandler(IRequestWriteRepository requestWrite , IEventPublisher eventPublisher,IRedisCacheService redisCache , IUnitOfWork unitOfWork)
        {
            _requestWrite = requestWrite;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
            _unitOfWork = unitOfWork;
        }

        public async Task<AssignmentDecisionDto> Handle(AcceptOrRejectAssignmentCommand request, CancellationToken cancellationToken)
        {

            var ekip = await _requestWrite.GetRequestByIdAsync(request.RequestRef, cancellationToken);
            if (ekip == null)
                throw new Exception("Ekip Not Found.");

            Guid senderRef = Guid.Empty;
            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                if (request.Decision == AssignmentDecision.Accepted)
                {
                    senderRef = ekip.AcceptMember(request.OwnerId, request.AssignmentRef);

                    await _eventPublisher.Publish(new AssignmentDecisionMadeEvent
                    {
                        AssignmentRef = request.AssignmentRef,
                        NewStatus = AssignmentStatus.Accepted,
                    }, innerCt);
                }
                else if (request.Decision == AssignmentDecision.Rejected)
                {
                    senderRef = ekip.RejectMember(request.OwnerId, request.AssignmentRef);

                    await _eventPublisher.Publish(new AssignmentDecisionMadeEvent
                    {
                        AssignmentRef = request.AssignmentRef,
                        NewStatus = AssignmentStatus.Declined,
                    }, innerCt);
                }

                await _requestWrite.UpdateAsync(ekip, innerCt);

                await _redisCache.RemoveAsync(CacheKeySchema.UserAssignmentsKey(senderRef), innerCt);
                await _redisCache.RemoveAsync(CacheKeySchema.RequestKey(ekip.Id), innerCt);
            },cancellationToken);
            

            return new AssignmentDecisionDto()
            {
                AssignmentRef = request.AssignmentRef,
                Decision = request.Decision,
                OwnerId = ekip.Creator,
                SenderRef = senderRef,
            };
        }
    }
}
