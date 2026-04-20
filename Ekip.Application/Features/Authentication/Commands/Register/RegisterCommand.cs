using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums.Identity.Enums;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommand : IRequest<AuthenticationResult>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public GenderType Gender { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
    }
}
