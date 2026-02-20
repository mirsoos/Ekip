using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class AssignToRequestDto
    {
        public Guid RequestRef { get; set; }
        public Guid SenderRef { get; set; }
        public string Description { get; set; }
        public AssignmentStatus Status { get; set; }
    }
}
