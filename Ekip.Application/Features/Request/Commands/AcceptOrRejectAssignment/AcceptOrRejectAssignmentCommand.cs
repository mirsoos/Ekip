using Ekip.Application.DTOs.Request;
using Ekip.Domain.Enums.Requests.Enums;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AcceptOrRejectAssignment
{
    public class AcceptOrRejectAssignmentCommand : IRequest<AssignmentDecisionDto>
    {
        public Guid OwnerId { get; set; }
        public Guid AssignmentRef { get; set; }
        public Guid RequestRef { get; set; }
        public AssignmentDecision Decision { get; set; }
    }
}
