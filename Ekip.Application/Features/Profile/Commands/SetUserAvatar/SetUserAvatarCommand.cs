using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommand : IRequest<string>
    {
        public Guid ProfileRef { get; set; }
        public string AvatarUrl { get; set; }
    }
}
