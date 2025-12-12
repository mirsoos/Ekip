using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.ValueObjects;
using RequestEntity = Ekip.Domain.Entities.Requests.Entities.Request;
using MediatR;
using MassTransit;
using Ekip.Application.Contracts.Events;

namespace Ekip.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, NewRequestDto>
    {
        private readonly IRequestWriteRepository _createRequest;
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateRequestCommandHandler(IRequestWriteRepository createRequest,IProfileWriteRepository profileWrite,IPublishEndpoint publishEndpoint)
        {
            _createRequest = createRequest;
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<NewRequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {

            var creator = await _profileWrite.GetByIdAsync(request.ProfileRef, cancellationToken);
            
            if(creator == null)
            {
                throw new Exception($"Profile with ID {request.ProfileRef} Not Found"); 
            };

            var requestFilter = request.RequestFilters?.Select(rf =>
                new RequestFilter(rf.Value, rf.Type, rf.Kind)
                ).ToHashSet();

            var newRequest = new RequestEntity(
                creator.Id,
                request.Title,
                request.RequiredMembers ,
                request.RequestDateTime,
                request.Description,
                request.Tags,
                request.RequestType,
                request.MemberType,
                request.IsAutoAccept,
                requestFilter
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
                MaximumRequiredAssignmnets = savedRequest.MaximumRequiredMembers,
                Tags = savedRequest.Tags,
                RequestFilters = savedRequest.RequestFilters?.Select(rf=> new RequestFilterDto {
                Value = rf.Value,Type = rf.Type,Kind=rf.Kind
                }).ToArray(),
                RequestCreateDateTime = savedRequest.CreateDate,
                RequestDateTime = savedRequest.RequestDateTime,
                RequestForbidDateTime = savedRequest.RequestForbidDateTime,
                RequiredMembers = savedRequest.RequiredMembers,

            });

            var requestResultDto = new NewRequestDto
            {
                RequestRef = savedRequest.Id,
                Title = savedRequest.Title,
                Description = savedRequest.Description,
                Creator = savedRequest.Creator,
                RequiredMembers = savedRequest.RequiredMembers,
                MaximumRequiredAssignmnets = savedRequest.MaximumRequiredMembers,
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
            return requestResultDto;
        }
    }
}
