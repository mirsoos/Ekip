using Ekip.Application.Constants;
using Ekip.Application.DTOs.User;
using Ekip.Application.Features.Profile.Queries.GetUserEkip;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetuserEkip
{
    public class GetUserEkipQueryHandler : IRequestHandler<GetUserEkipQuery, List<MyEkipDto>>
    {
        private readonly IRequestReadRepository _requestRead;
        private readonly IRedisCacheService _redisCache;

        public GetUserEkipQueryHandler(IRequestReadRepository requestRead , IRedisCacheService redisCache)
        {
            _requestRead = requestRead;
            _redisCache = redisCache;
        }
        public async Task<List<MyEkipDto>> Handle(GetUserEkipQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeySchema.UserEkipsKey(request.ProfileRef);
            var cachedEkips = await _redisCache.GetAsync<List<MyEkipDto>>(key , cancellationToken);
            if (cachedEkips?.Any() == true)
                return cachedEkips;

            var ekips = await _requestRead.GetEkipsByProfileId(request.ProfileRef, cancellationToken);

            if(ekips.Any())
            await _redisCache.SetAsync(key,ekips,TimeSpan.FromHours(12),cancellationToken);

            return ekips;
        }
    }
}
