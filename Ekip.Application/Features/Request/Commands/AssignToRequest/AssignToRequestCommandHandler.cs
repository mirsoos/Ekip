using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.ValueObjects;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AssignToRequest
{
    public class AssignToRequestCommandHandler : IRequestHandler<AssignToRequestCommand, AssignToRequestDto>
    {
        private readonly IRequestWriteRepository _requestWrite;
        private readonly IProfileReadRepository _profileRead;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;
        public AssignToRequestCommandHandler(IRequestWriteRepository requestWrite, IProfileReadRepository profileRead, IPublishEndpoint publishEndpoint , IRedisCacheService redisCache)
        {
            _requestWrite = requestWrite;
            _profileRead = profileRead;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
        }

        public async Task<AssignToRequestDto> Handle(AssignToRequestCommand request, CancellationToken cancellationToken)
        {
            var currentRequest = await _requestWrite.GetRequestByIdAsync(request.RequestRef,cancellationToken);

            if (currentRequest == null)
                throw new Exception("Request Not Found");

            var SenderProfile = await _profileRead.GetProfileByIdAsync(request.SenderRef,cancellationToken);

            if (SenderProfile == null)
                throw new Exception("User Profile Not Found");

            var eligibility = new MemberEligibility(SenderProfile.Age , SenderProfile.Experience,SenderProfile.Score,SenderProfile.Gender);
            
            var newAssignment = currentRequest.AddJoinRequest(request.SenderRef, eligibility ,request.Description);

            await _requestWrite.UpdateAsync(currentRequest,cancellationToken);

            await _publishEndpoint.Publish(new AssignmentProcessedEvent
            {
                Id = newAssignment.Id,
                ActionDate = newAssignment.ActionDate,
                CreateDate = newAssignment.CreateDate,
                Description = newAssignment.Description,
                SenderRef = newAssignment.SenderRef,
                AssignmentStatus = newAssignment.Status,
                RequestRef = currentRequest.Id,
                RequestStatus = currentRequest.Status
            });

            await _redisCache.RemoveAsync(CacheKeySchema.UserAssignmentsKey(newAssignment.SenderRef), cancellationToken);
            await _redisCache.RemoveAsync(CacheKeySchema.RequestKey(currentRequest.Id), cancellationToken);

            return new AssignToRequestDto{
                SenderRef = request.SenderRef,
                Description = request.Description,
                RequestRef = currentRequest.Id,
                Status = newAssignment.Status
            };
        }
    }
}
