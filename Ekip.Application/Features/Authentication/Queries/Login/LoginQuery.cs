using Ekip.Application.DTOs;
using MediatR;

namespace Ekip.Application.Features.Authentication.Queries.Login
{
    public class LoginQuery : IRequest<AuthenticationResult>
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
    }
}
