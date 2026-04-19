using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Application.DTOs.User
{
    public class MyEkipDto
    {
        public Guid RequestRef { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public List<AssignmentMemberDto> Members { get; set; }
        public string Creator { get; set; }
        public int CurrentMembers { get; set; }
        public int RequiredMembers { get; set; }
        public DateTime StartEventDateTime { get; set; }
    }
}
