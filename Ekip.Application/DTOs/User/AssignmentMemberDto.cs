
namespace Ekip.Application.DTOs.User
{
    public class AssignmentMemberDto
    {
        public long ProfileRef { get; set; }
        public string UserName { get; set; }
        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
