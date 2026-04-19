
using Ekip.Application.DTOs.Request;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetPendingAssignment
{
    public class GetPendingAssignmentQuery : IRequest<List<PendingAssignmentsDto>>
    {
        public Guid ProfileRef { get; set; }
    }
}
