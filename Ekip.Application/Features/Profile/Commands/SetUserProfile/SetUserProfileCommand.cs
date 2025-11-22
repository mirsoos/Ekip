using Ekip.Application.DTOs.User;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserProfile
{
    public class SetUserProfileCommand : IRequest<CreatedProfileDto>
    {
        public string UserName { get; set; }
    }
}
