namespace Ekip.Application.DTOs.User
{
    public class UserWithProfileDto
    {
        public Guid UserRef { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
    }
}
