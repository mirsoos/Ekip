using Ekip.Application.Constants;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileRead;
        private readonly IRedisCacheService _redisCache;

        public GetUserProfileQueryHandler(IProfileReadRepository profileRead, IRedisCacheService redisCache)
        {
            _profileRead = profileRead;
            _redisCache = redisCache;
        }

        public async Task<ProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeySchema.ProfileKey(request.UserRef);
            var cachedProfile = await _redisCache.GetAsync<ProfileDto>(key , cancellationToken);
            if (cachedProfile != null)
                return cachedProfile;
            
            var profileDto = await _profileRead.GetUserDetailsByIdAsync(request.UserRef,cancellationToken);

            if (profileDto == null)
                throw new Exception($"user with Id'{request.UserRef}'Not Found");

            await _redisCache.SetAsync(key,profileDto,TimeSpan.FromDays(1),cancellationToken);

            return profileDto;
        }
    }
}
