using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommand : IRequest<string>
    {
        public Guid ProfileRef { get; set; }
        public IFormFile file { get; set; }
    }
}
