using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using Ekip.Domain.Enums.Requests.Enums;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AssignToRequest
{
    public class AssignToRequestCommandHandler : IRequestHandler<AssignToRequestCommand, AssignToRequestDto>
    {
        private readonly IRequestWriteRepository _requestWrite;
        private readonly IProfileReadRepository _profileRead;
        private readonly IPublishEndpoint _publishEndpoint;
        public AssignToRequestCommandHandler(IRequestWriteRepository requestWrite, IProfileReadRepository profileRead, IPublishEndpoint publishEndpoint)
        {
            _requestWrite = requestWrite;
            _profileRead = profileRead;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<AssignToRequestDto> Handle(AssignToRequestCommand request, CancellationToken cancellationToken)
        {
            var currentRequest = await _requestWrite.GetRequestByIdAsync(request.RequestRef,cancellationToken);

            if (currentRequest == null)
                throw new Exception("Request Not Found");

            var SenderProfile = await _profileRead.GetProfileByIdAsync(request.SenderRef,cancellationToken);

            if (SenderProfile == null)
                throw new Exception("User Profile Not Found");

            var newAssignment = currentRequest.AddJoinRequest(request.SenderRef ,request.Description);

            var assign = await _requestWrite.AssignRequest(request.RequestRef,newAssignment, cancellationToken);

            await _publishEndpoint.Publish(new RequestAssignmentCreatedEvent
            {
                Id = assign.Id,
                ActionDate = assign.ActionDate,
                CreateDate = assign.CreateDate,
                Description = assign.Description,
                SenderRef = assign.SenderRef,
                Status = assign.Status
            });

            return new AssignToRequestDto{
                SenderRef = request.SenderRef,
                Description = request.Description,
                RequestRef = currentRequest.Id,
                Status = assign.Status
            };
        }
    }
}
