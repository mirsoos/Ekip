using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.ValueObjects;
using RequestEntity = Ekip.Domain.Entities.Requests.Entities.Request;
using MediatR;

namespace Ekip.Application.Features.Request.Commands
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, NewRequestDto>
    {
        private readonly ICreateRequestWriteRepository _createRequest;
        private readonly IProfileWriteRepository _profileWrite;
        public CreateRequestCommandHandler(ICreateRequestWriteRepository createRequest,IProfileWriteRepository profileWrite)
        {
            _createRequest = createRequest;
            _profileWrite = profileWrite;
        }
        public async Task<NewRequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {

            var creator = await _profileWrite.GetByIdAsync(request.ProfileRef, cancellationToken);
            
            if(creator == null)
            {
                throw new Exception($"Profile with ID {request.ProfileRef} Not Found"); 
            };

            var requestFilters = request.RequestFilters?.Select(rf => 
            
            new RequestFilter(rf.Value,rf.Type,rf.Kind)
            ).ToHashSet();

            var newRequest = new RequestEntity(
                creator,
                request.Title,
                request.RequiredMembers ,
                request.RequestDateTime,
                request.Description,
                request.Tags,
                request.RequestType,
                request.MemberType,
                request.IsAutoAccept,
                requestFilters  
                );

           var savedRequest = await _createRequest.AddRequestAsync(newRequest,cancellationToken);

            var requestResultDto = new NewRequestDto
            {
                RequestRef = savedRequest.Id,
                Title = savedRequest.Title,
                Description = savedRequest.Description,
                Creator = new RequestCreatorDto {
                    ProfileId = savedRequest.Creator.Id,
                    FirstName = savedRequest.Creator.UserDetails.FirstName,
                    LastName = savedRequest.Creator.UserDetails.LastName,
                    AvatarUrl = savedRequest.Creator.AvatarUrl,
                    UserName = savedRequest.Creator.UserDetails.UserName,
                },
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
                RequestFilters = savedRequest.RequestFilters?.Select(f => new RequestFilterDto
                {
                    Value = f.Value,
                    Type = f.Type,
                    Kind = f.Kind,
                }).ToArray(),
            };
            return requestResultDto;
        }
    }
}
