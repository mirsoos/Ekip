using Ekip.Application.Constants;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Request.Qeuries.GetUserEkip
{
    public class GetUserEkipQueryHandler : IRequestHandler<GetUserEkipQuery, List<MyEkipDto>>
    {
        private readonly IRedisCacheService _redisCache;
        private readonly IUserEkipReadRepository _userEkipRead;

        public GetUserEkipQueryHandler(IRedisCacheService redisCache, IUserEkipReadRepository userEkipRead)
        {
            _redisCache = redisCache;
            _userEkipRead = userEkipRead;
        }
        public async Task<List<MyEkipDto>> Handle(GetUserEkipQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeySchema.UserEkipsKey(request.ProfileRef);
            var cachedEkips = await _redisCache.GetAsync<List<MyEkipDto>>(key , cancellationToken);
            if (cachedEkips?.Any() == true)
                return cachedEkips;

            var ekips = await _userEkipRead.GetEkipByProfileIdAsync(request.ProfileRef, cancellationToken);

            if(ekips.Any())
            await _redisCache.SetAsync(key,ekips,TimeSpan.FromHours(12),cancellationToken);

            return ekips;
        }
    }
}
