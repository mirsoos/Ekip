using MediatR;

namespace Ekip.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<bool>
    {
        public Guid UserRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
