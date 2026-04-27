using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.ValueObjects;
using RequestEntity = Ekip.Domain.Entities.Requests.Entities.Request;
using MediatR;
using MassTransit;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Constants;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, NewRequestDto>
    {
        private readonly IRequestWriteRepository _createRequest;
        private readonly IProfileReadRepository _profileRead;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;
        public CreateRequestCommandHandler(IRequestWriteRepository createRequest,IProfileReadRepository profileRead,IPublishEndpoint publishEndpoint , IRedisCacheService redisCache)
        {
            _createRequest = createRequest;
            _profileRead = profileRead;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
        }
        public async Task<NewRequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {

            var creator = await _profileRead.GetProfileByIdAsync(request.ProfileRef, cancellationToken);
            
            if(creator == null)
            {
                throw new Exception($"Profile with ID {request.ProfileRef} Not Found"); 
            };

            var requestFilter = request.RequestFilters?.Select(rf =>
                new RequestFilter(rf.Value, rf.Type, rf.Kind)
                ).ToHashSet();

            var newRequest = new RequestEntity(
                creator.ProfileRef,
                request.Title,
                request.RequiredMembers ,
                request.RequestDateTime,
                request.Description,
                request.Tags,
                request.RequestType,
                request.MemberType,
                request.IsAutoAccept,
                request.IsRepeatable,
                request.RepeatType,
                requestFilter,
                request.TargetGender,
                request.RequiredLevel,
                request.MinimumScore
                );

           var savedRequest = await _createRequest.AddRequestAsync(newRequest,cancellationToken);

            await _publishEndpoint.Publish(new RequestCreatedEvent
            {
                RequestRef = savedRequest.Id,
                CreatorRef = savedRequest.Creator,
                Title = savedRequest.Title,
                Description = savedRequest.Description,
                RequestType = savedRequest.RequestType,
                MemberType = savedRequest.MemberType,
                IsAutoAccept = savedRequest.IsAutoAccept,
                IsRepeatable = savedRequest.IsRepeatable,
                RepeatType = savedRequest.RepeatType,
                MaximumRequiredAssignments = savedRequest.MaximumRequiredMembers,
                Tags = savedRequest.Tags,
                RequestFilters = savedRequest.RequestFilters?.Select(rf=> new RequestFilterDto {
                Value = rf.Value,Type = rf.Type,Kind=rf.Kind
                }).ToArray(),
                RequestCreateDateTime = savedRequest.CreateDate,
                RequestDateTime = savedRequest.RequestDateTime,
                RequestForbidDateTime = savedRequest.RequestForbidDateTime,
                RequiredMembers = savedRequest.RequiredMembers,
                Status = savedRequest.Status,
                TargetGender = savedRequest.TargetGender,
                RequiredLevel = savedRequest.RequiredLevel,
                MinimumScore = savedRequest.MinimumScore
            });

            var requestResultDto = new NewRequestDto
            {
                RequestRef = savedRequest.Id,
                Title = savedRequest.Title,
                Description = savedRequest.Description,
                Creator = savedRequest.Creator,
                RequiredMembers = savedRequest.RequiredMembers,
                MaximumRequiredAssignments = savedRequest.MaximumRequiredMembers,
                Tags = savedRequest.Tags,
                RequestCreateDateTime = savedRequest.CreateDate,
                RequestDateTime = savedRequest.RequestDateTime,
                RequestForbidDateTime = savedRequest.RequestForbidDateTime,
                IsAutoAccept = savedRequest.IsAutoAccept,
                IsRepeatable = savedRequest.IsRepeatable,
                RequestType = savedRequest.RequestType,
                RepeatType = savedRequest.RepeatType,
                MemberType = savedRequest.MemberType,
                RequestFilters = savedRequest.RequestFilters?.Select(rf => new RequestFilterDto
                {
                    Value = rf.Value,
                    Type = rf.Type,
                    Kind = rf.Kind
                }).ToArray(),
            };

            await _publishEndpoint.Publish(new UserEkipCreatedUpdaterEvent
            {
                RequestRef = savedRequest.Id,
                CreateDate = savedRequest.CreateDate,
                CreatorRef = savedRequest.Creator,
                RequiredMembers = savedRequest.RequiredMembers,
                EkipTitle = savedRequest.Title,
                Description = savedRequest.Description,
                IsDeleted = savedRequest.IsDeleted,
                IsRepeatable = savedRequest.IsRepeatable,
                IsAutoAccept = savedRequest.IsAutoAccept,
                RequestDateTime = savedRequest.RequestDateTime,
                MemberType = savedRequest.MemberType,
                TargetGender = savedRequest.TargetGender,
                MaximumRequiredMembers = savedRequest.MaximumRequiredMembers,
                Status = savedRequest.Status,
                RequestType = savedRequest.RequestType,
                RequiredLevel = savedRequest.RequiredLevel,
                MinimumScore = savedRequest.MinimumScore,
                RepeatType = savedRequest.RepeatType,
                MaximumAge = savedRequest.MaximumRequiredAge,
                MinimumAge = savedRequest.MinimumRequiredAge,
                CreatorName = creator.FirstName + " " + creator.LastName,
                CreatorAvatar = creator.AvatarUrl,
                CurrentMembersCount = savedRequest.Assignments.Count(x=>x.Status == AssignmentStatus.Accepted),
                RequestForbidDateTime = savedRequest.RequestForbidDateTime,
                LastUpdated = savedRequest.CreateDate,
                Tags = savedRequest.Tags,
                AcceptedMembers = savedRequest.Assignments.Where(x => x.Status == AssignmentStatus.Accepted)
                .Select(s => new EkipMember(creator.ProfileRef, creator.FirstName, creator.LastName, creator.AvatarUrl)).ToList(),
                PendingAssignments = new List<PendingAssignmentInfo>()
            });

            await _redisCache.RemoveAsync(CacheKeySchema.UserEkipsKey(creator.ProfileRef), cancellationToken);
            await _redisCache.RemoveAsync(CacheKeySchema.RequestKey(savedRequest.Id) , cancellationToken);
            await _redisCache.RemoveAsync(CacheKeySchema.UserRequestsKey(savedRequest.Creator) , cancellationToken);

            return requestResultDto;
        }
    }
}
