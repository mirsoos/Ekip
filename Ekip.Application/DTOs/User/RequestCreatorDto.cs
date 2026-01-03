
namespace Ekip.Application.DTOs.User
{
    public class RequestCreatorDto
    {
        public Guid ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
