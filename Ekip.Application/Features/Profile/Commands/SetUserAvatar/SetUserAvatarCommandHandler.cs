using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommandHandler : IRequestHandler<SetUserAvatarCommand, string>
    {
        private readonly IUserWriteRepository _userWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        public SetUserAvatarCommandHandler(IUserWriteRepository userWrite , IEventPublisher eventPublisher,IRedisCacheService redisCache)
        {
            _userWrite = userWrite;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
        }

        public async Task<string> Handle(SetUserAvatarCommand command, CancellationToken cancellationToken)
        {
            if (command.AvatarUrl == null)
                throw new Exception("File Not Found.");

            var user = await _userWrite.GetByUserIdAsync(command.UserRef,cancellationToken);

            if (user == null)
                throw new Exception("User Not Found.");

            user.Profile.SetAvatar(command.AvatarUrl);

            await _userWrite.UpdateAsync(user,cancellationToken);

            await _eventPublisher.Publish(new ProfileAvatarUpdatedEvent
            {
                UserRef = user.Id,
                AvatarUrl = command.AvatarUrl
            },cancellationToken);

            await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(user.Id), cancellationToken);

            return command.AvatarUrl;
        }
    }
}
