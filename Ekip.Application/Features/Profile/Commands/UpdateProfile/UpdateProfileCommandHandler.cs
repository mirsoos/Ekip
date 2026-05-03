using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IUserWriteRepository _userWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRedisCacheService _redisCache;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IEventPublisher eventPublisher , IRedisCacheService redisCache , IUserWriteRepository userWrite , IUnitOfWork unitOfWork)
        {
            _eventPublisher = eventPublisher;
            _redisCache = redisCache;
            _userWrite = userWrite;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {

            var user = await _userWrite.GetByUserIdAsync(request.UserRef , cancellationToken);
            if (user == null)
                return false;

            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                var userUpdated = await _userWrite.UpdateAsync(user, innerCt);

                await _eventPublisher.Publish(new ProfileUpdatedEvent
                {
                    FirstName = userUpdated.FirstName,
                    LastName = userUpdated.LastName,
                    Age = userUpdated.Age,
                    UserName = userUpdated.UserName,
                    Email = userUpdated.Email.Value
                }, innerCt);

                await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(user.Id), innerCt);

            },cancellationToken);

            return true;
        }
    }
}
