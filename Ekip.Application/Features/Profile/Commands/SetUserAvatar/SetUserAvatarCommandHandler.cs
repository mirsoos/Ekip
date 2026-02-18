using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommandHandler : IRequestHandler<SetUserAvatarCommand, string>
    {
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        public SetUserAvatarCommandHandler(IProfileWriteRepository profileWrite , IPublishEndpoint publishEndpoint)
        {
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<string> Handle(SetUserAvatarCommand command, CancellationToken cancellationToken)
        {
            if (command.AvatarUrl == null)
                throw new Exception("File Not Found.");

            var profile = await _profileWrite.GetByIdAsync(command.ProfileRef,cancellationToken);

            if (profile == null)
                throw new Exception("Profile Not Found.");

            profile.SetAvatar(command.AvatarUrl);

            await _profileWrite.UpdateAsync(profile,cancellationToken);

            await _publishEndpoint.Publish(new ProfileAvatarUpdatedEvent
            {
                ProfileRef = profile.Id,
                AvatarUrl = command.AvatarUrl
            });

            return command.AvatarUrl;
        }
    }
}
