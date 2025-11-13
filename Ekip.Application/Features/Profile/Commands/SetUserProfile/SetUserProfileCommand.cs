using MediatR;
using Ekip.Application.DTOs;

namespace Ekip.Application.Features.Profile.Commands.SetUserProfile
{
    public class SetUserProfileCommand : IRequest<ProfileDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
    }
}
