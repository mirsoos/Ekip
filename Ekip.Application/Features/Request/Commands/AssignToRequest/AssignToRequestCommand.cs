using Ekip.Application.DTOs.Request;
using Ekip.Domain.Enums.Requests.Enums;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AssignToRequest
{
    public class AssignToRequestCommand : IRequest<AssignToRequestDto>
    {
        public long RequestRef { get; set; }
        public long SenderRef { get; set; }
        public string Description { get; set; }
        public AssignmentStatus Status { get; set; }

    }
}
