using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
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

            await _requestWrite.UpdateAsync(currentRequest,cancellationToken);

            //await _requestWrite.AssignRequest(request.RequestRef,newAssignment, cancellationToken);
            await _publishEndpoint.Publish(new RequestUpdatedEvent
            {
                RequestRef = currentRequest.Id,
                Status = currentRequest.Status
            });

            await _publishEndpoint.Publish(new RequestAssignmentCreatedEvent
            {
                Id = newAssignment.Id,
                ActionDate = newAssignment.ActionDate,
                CreateDate = newAssignment.CreateDate,
                Description = newAssignment.Description,
                SenderRef = newAssignment.SenderRef,
                Status = newAssignment.Status,
                RequestRef = currentRequest.Id
            });

            return new AssignToRequestDto{
                SenderRef = request.SenderRef,
                Description = request.Description,
                RequestRef = currentRequest.Id,
                Status = newAssignment.Status
            };
        }
    }
}
