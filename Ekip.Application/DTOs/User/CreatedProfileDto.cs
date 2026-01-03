
namespace Ekip.Application.DTOs.User
{
    public class CreatedProfileDto
    {
        public Guid ProfileRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public double? Score { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public int Experience { get; set; }
    }
}
