using Ekip.Application.Constants;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Request.Qeuries.GetRequest
{
    public class GetRequestQueryHandler : IRequestHandler<GetRequestQuery, RequestDetailsDto>
    {
        private readonly IRequestReadRepository _requestRead;
        private readonly IRedisCacheService _redisCache;
        public GetRequestQueryHandler(IRequestReadRepository requestRead, IRedisCacheService redisCache)
        {
            _requestRead = requestRead;
            _redisCache = redisCache;
        }

        public async Task<RequestDetailsDto> Handle(GetRequestQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeySchema.RequestKey(request.RequestRef);
            var cachedRequest = await _redisCache.GetAsync<RequestDetailsDto>(key,cancellationToken);
            if(cachedRequest != null)
                return cachedRequest;

            var requestDetailsDto = await _requestRead.GetRequestByIdAsync(request.RequestRef,cancellationToken);

            if (requestDetailsDto == null)
                throw new Exception($"Request with RequestId '{request.RequestRef}' Not Found");

            await _redisCache.SetAsync(key, requestDetailsDto, TimeSpan.FromHours(2), cancellationToken);

            return requestDetailsDto;
        }
    }
}
