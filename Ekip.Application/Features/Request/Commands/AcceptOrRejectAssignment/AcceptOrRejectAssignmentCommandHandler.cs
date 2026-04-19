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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;

        public AcceptOrRejectAssignmentCommandHandler(IRequestWriteRepository requestWrite , IPublishEndpoint publishEndpoint,IRedisCacheService redisCache)
        {
            _requestWrite = requestWrite;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
        }

        public async Task<AssignmentDecisionDto> Handle(AcceptOrRejectAssignmentCommand request, CancellationToken cancellationToken)
        {

            var ekip = await _requestWrite.GetRequestByIdAsync(request.RequestRef, cancellationToken);
            if (ekip == null)
                throw new Exception("Ekip Not Found.");

            Guid senderRef = Guid.Empty;

            if (request.Decision == AssignmentDecision.Accepted)
            {
                senderRef = ekip.AcceptMember(request.OwnerId, request.AssignmentRef);

                await _publishEndpoint.Publish(new AssignmentDecisionMadeEvent
                {
                    AssignmentRef = request.AssignmentRef,
                    NewStatus = AssignmentStatus.Accepted,
                });
            }
            else if (request.Decision == AssignmentDecision.Rejected)
            {
                senderRef = ekip.RejectMember(request.OwnerId, request.AssignmentRef);

                await _publishEndpoint.Publish(new AssignmentDecisionMadeEvent
                {
                    AssignmentRef = request.AssignmentRef,
                    NewStatus = AssignmentStatus.Declined,
                });
            }
            await _requestWrite.UpdateAsync(ekip, cancellationToken);

            await _redisCache.RemoveAsync(CacheKeySchema.UserAssignmentsKey(senderRef), cancellationToken);
            await _redisCache.RemoveAsync(CacheKeySchema.RequestKey(ekip.Id), cancellationToken);

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
