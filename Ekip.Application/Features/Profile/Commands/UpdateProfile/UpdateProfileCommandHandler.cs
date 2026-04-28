using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IUserWriteRepository _userWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;

        public UpdateProfileCommandHandler(IPublishEndpoint publishEndpoint , IRedisCacheService redisCache , IUserWriteRepository userWrite)
        {
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
            _userWrite = userWrite;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {

            var user = await _userWrite.GetByProfileIdAsync(request.ProfileRef , cancellationToken);
            if (user == null)
                return false;

            var userUpdated = await _userWrite.UpdateAsync(user,cancellationToken);

            await _publishEndpoint.Publish(new ProfileUpdatedEvent
            {
                FirstName = userUpdated.FirstName,
                LastName = userUpdated.LastName,
                Age = userUpdated.Age,
                UserName = userUpdated.UserName,
                Email = userUpdated.Email.Value
            });

            await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(user.ProfileRef),cancellationToken);

            return true;
        }
    }
}
