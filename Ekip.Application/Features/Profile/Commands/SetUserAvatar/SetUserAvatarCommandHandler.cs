using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.SetUserAvatar
{
    public class SetUserAvatarCommandHandler : IRequestHandler<SetUserAvatarCommand, string>
    {
        private readonly IUserWriteRepository _userWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        private readonly IUnitOfWork _unitOfWork;
        public SetUserAvatarCommandHandler(IUserWriteRepository userWrite , IEventPublisher eventPublisher,IRedisCacheService redisCache ,IUnitOfWork unitOfWork)
        {
            _userWrite = userWrite;
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(SetUserAvatarCommand command, CancellationToken cancellationToken)
        {
            if (command.AvatarUrl == null)
                throw new Exception("File Not Found.");

            var user = await _userWrite.GetByUserIdAsync(command.UserRef,cancellationToken);

            if (user == null)
                throw new ArgumentNullException("User Not Found.");

            user.Profile.SetAvatar(command.AvatarUrl);

            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                await _userWrite.UpdateAsync(user,innerCt);

                await _eventPublisher.Publish(new ProfileAvatarUpdatedEvent
                {
                    UserRef = user.Id,
                    AvatarUrl = command.AvatarUrl
                }, innerCt);

                await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(user.Id), innerCt);

            },cancellationToken);

            return command.AvatarUrl;
        }
    }
}
