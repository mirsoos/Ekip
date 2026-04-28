
namespace Ekip.Application.DTOs.User
{
    public class UpdateProfileRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

    }
}
