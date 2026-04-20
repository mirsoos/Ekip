using Ekip.Application.DTOs.Request;
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Application.DTOs.User
{
    public class ProfileDto
    {
        public Guid ProfileRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public double? Score { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public GenderType Gender { get; set; }
        public int Experience { get; set; }
    }
}
