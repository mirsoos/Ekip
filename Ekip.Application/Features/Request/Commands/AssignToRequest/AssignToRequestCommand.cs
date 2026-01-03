using Ekip.Application.DTOs.Request;
using Ekip.Domain.Enums.Requests.Enums;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.AssignToRequest
{
    public class AssignToRequestCommand : IRequest<AssignToRequestDto>
    {
        public Guid RequestRef { get; set; }
        public Guid SenderRef { get; set; }
        public string Description { get; set; }
        public AssignmentStatus Status { get; set; }

    }
}
