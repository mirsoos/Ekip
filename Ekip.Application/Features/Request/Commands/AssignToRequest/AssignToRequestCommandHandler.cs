using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.ValueObjects;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AssignToRequest
{
    public class AssignToRequestCommandHandler : IRequestHandler<AssignToRequestCommand, AssignToRequestDto>
    {
        private readonly IRequestWriteRepository _requestWrite;
        private readonly IProfileReadRepository _profileRead;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        private readonly IUnitOfWork _unitOfWork;
        public AssignToRequestCommandHandler(IRequestWriteRepository requestWrite, IProfileReadRepository profileRead, IEventPublisher eventPublisher , IRedisCacheService redisCache, IUnitOfWork unitOfWork)
        {
            _requestWrite = requestWrite;
            _profileRead = profileRead;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
            _unitOfWork = unitOfWork;
        }

        public async Task<AssignToRequestDto> Handle(AssignToRequestCommand request, CancellationToken cancellationToken)
        {
            var currentRequest = await _requestWrite.GetRequestByIdAsync(request.RequestRef,cancellationToken);

            if (currentRequest == null)
                throw new Exception("Request Not Found");

            var SenderProfile = await _profileRead.GetUserByIdAsync(request.SenderRef,cancellationToken);

            if (SenderProfile == null)
                throw new Exception("User Profile Not Found");

            var eligibility = new MemberEligibility(SenderProfile.Age , SenderProfile.Experience,SenderProfile.Score,SenderProfile.Gender);
            
            var newAssignment = currentRequest.AddJoinRequest(request.SenderRef, eligibility ,request.Description);

            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                await _requestWrite.UpdateAsync(currentRequest, innerCt);

                await _eventPublisher.Publish(new AssignmentProcessedEvent
                {
                    Id = newAssignment.Id,
                    ActionDate = newAssignment.ActionDate,
                    CreateDate = newAssignment.CreateDate,
                    Description = newAssignment.Description,
                    SenderRef = newAssignment.SenderRef,
                    AssignmentStatus = newAssignment.Status,
                    RequestRef = currentRequest.Id,
                    RequestStatus = currentRequest.Status
                }, innerCt);

                await _redisCache.RemoveAsync(CacheKeySchema.UserAssignmentsKey(newAssignment.SenderRef), innerCt);
                await _redisCache.RemoveAsync(CacheKeySchema.RequestKey(currentRequest.Id), innerCt);
            },cancellationToken);

            return new AssignToRequestDto{
                SenderRef = request.SenderRef,
                Description = request.Description,
                RequestRef = currentRequest.Id,
                Status = newAssignment.Status
            };
        }
    }
}
