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
        public AcceptOrRejectAssignmentCommandHandler(IRequestWriteRepository requestWrite , IPublishEndpoint publishEndpoint)
        {
            _requestWrite = requestWrite;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<AssignmentDecisionDto> Handle(AcceptOrRejectAssignmentCommand request, CancellationToken cancellationToken)
        {

           var ekip =  await _requestWrite.GetRequestByIdAsync(request.RequestRef , cancellationToken);
            if (ekip == null)
                throw new Exception("Ekip Not Found.");

            if(request.Decision == AssignmentDecision.Accepted)
            {
                ekip.AcceptMember(request.OwnerId , request.AssignmentRef);

                //await _publishEndpoint.Publish(new 
                //{

                //});
            }
            else if(request.Decision == AssignmentDecision.Rejected)
            {
                ekip.RejectMember(request.OwnerId , request.AssignmentRef);
            }
            await _requestWrite.UpdateAsync(ekip , cancellationToken);

            return new AssignmentDecisionDto();
        }
    }
}
