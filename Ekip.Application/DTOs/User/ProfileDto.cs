using Ekip.Application.DTOs.Request;

namespace Ekip.Application.DTOs.User
{
    public class ProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public double Rating { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public int Level { get; set; }
        public List<NewRequestDto> Requests { get; set; }
        public List<JoinRequestDto> JoinRequests { get; set; }
        public List<ReuqestAssignmentDto> RequestAssignments { get; set; }
    }
}
