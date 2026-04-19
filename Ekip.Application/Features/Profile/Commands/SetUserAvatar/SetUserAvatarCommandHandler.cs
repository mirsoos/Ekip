using Ekip.Application.Constants;
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
        private readonly IRedisCacheService _redisCache;
        public SetUserAvatarCommandHandler(IProfileWriteRepository profileWrite , IPublishEndpoint publishEndpoint,IRedisCacheService redisCache)
        {
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
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

            await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(profile.Id), cancellationToken);

            return command.AvatarUrl;
        }
    }
}
