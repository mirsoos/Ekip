using Ekip.Application.Constants;
using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;

namespace Ekip.Application.Features.Profile.Commands.SetUserProfile
{
    public class SetUserProfileCommandHandler : IRequestHandler<SetUserProfileCommand, CreatedProfileDto>
    {
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IUserWriteRepository _userWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCache;
        public SetUserProfileCommandHandler(IProfileWriteRepository profileWrite,IUserWriteRepository userWrite, IPublishEndpoint publishEndpoint,IRedisCacheService redisCache)
        {
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
            _userWrite = userWrite;
            _redisCache = redisCache;
        }

        public async Task<CreatedProfileDto> Handle(SetUserProfileCommand request, CancellationToken cancellationToken)
        {
            var getUserDetails = await _userWrite.GetByUserNameAsync(request.UserName,cancellationToken);

            if(getUserDetails == null)
                throw new Exception($"User with username '{request.UserName}' not found.");

            var profileExists = await _profileWrite.DoesProfileExistForUserAsync(getUserDetails.Id, cancellationToken);

            if (profileExists)
                throw new InvalidOperationException($"A Profile already Exists for User '{request.UserName}'.");
            
            var newProfile = new ProfileEntity(getUserDetails.Id);

            var savedProfile = await _profileWrite.AddAsync(newProfile,cancellationToken);

            await _publishEndpoint.Publish(new ProfileCreatedEvent
            {
                Experience = savedProfile.Experience,
                Score = savedProfile.Score,
                Id = savedProfile.Id,
                UserRef = savedProfile.UserRef
                
            });

            await _redisCache.RemoveAsync(CacheKeySchema.ProfileKey(savedProfile.Id), cancellationToken);

            var resultDto = new CreatedProfileDto {
                ProfileRef = savedProfile.Id,
                UserRef = savedProfile.UserRef,
                 Experience = savedProfile.Experience,
                 Score = savedProfile.Score
            };

            return resultDto;
        }
    }
}
