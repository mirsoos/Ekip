
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.User
{
    public class AssignmentMemberDto
    {
        public Guid UserRef { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
        public AssignmentStatus Status { get; set; }
    }
}
