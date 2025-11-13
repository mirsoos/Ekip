
namespace Ekip.Application.DTOs
{
    public class ProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public UserDto Rating { get; set; }
        public int MyProperty { get; set; }
        public List<RequestDto> Requests { get; set; }
        public List<JoinRequestDto> JoinRequests { get; set; }
        public List<ReuqestAssignmentDto> RequestAssignments { get; set; }
    }
}
