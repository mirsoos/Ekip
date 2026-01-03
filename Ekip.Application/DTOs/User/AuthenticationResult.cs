namespace Ekip.Application.DTOs.User
{
    public class AuthenticationResult
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }
}
