using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommandHandler : IRequestHandler<SetUserAvatarCommand, string>
    {
        private readonly IFileService _fileService;
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        public SetUserAvatarCommandHandler(IFileService fileService , IProfileWriteRepository profileWrite , IPublishEndpoint publishEndpoint)
        {
            _fileService = fileService;
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<string> Handle(SetUserAvatarCommand command, CancellationToken cancellationToken)
        {
            if (command.file == null)
                throw new Exception("FIle Not Found");

            var profile = await _profileWrite.GetByIdAsync(command.ProfileRef,cancellationToken);

            if (profile == null)
                throw new Exception("Profile Not Found.");

            var avatarUrl =  await _fileService.UploadImageAsync(command.file, "Avatars", cancellationToken);

            profile.SetAvatar(avatarUrl);

            await _profileWrite.UpdateAsync(profile,cancellationToken);

            await _publishEndpoint.Publish(new ProfileAvatarUpdatedEvent
            {
                ProfileRef = profile.Id,
                AvatarUrl = avatarUrl
            });

            return avatarUrl;
        }
    }
}
