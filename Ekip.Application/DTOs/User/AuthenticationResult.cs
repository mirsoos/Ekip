namespace Ekip.Application.DTOs.User
{
    public class AuthenticationResult
    {
        public Guid ProfileRef { get; set; }
        public string Token { get; set; }
    }
}
