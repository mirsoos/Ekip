using Ekip.Application.Constants;
using Ekip.Application.DTOs.Request;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetPendingAssignment
{
    public class GetPendingAssignmentQueryHandler : IRequestHandler<GetPendingAssignmentQuery, List<PendingAssignmentsDto>>
    {
        private readonly IRequestReadRepository _requestRead;
        private readonly IRedisCacheService _redisCache;
        public GetPendingAssignmentQueryHandler(IRequestReadRepository requestRead , IRedisCacheService redisCache)
        {
            _redisCache = redisCache;
            _requestRead = requestRead;
        }
        public async Task<List<PendingAssignmentsDto>> Handle(GetPendingAssignmentQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeySchema.UserAssignmentsKey(request.ProfileRef);
            var cachedPendingAssignment = await _redisCache.GetAsync<List<PendingAssignmentsDto>>(key,cancellationToken);
            if(cachedPendingAssignment?.Any() == true)
                return cachedPendingAssignment;

            var pendingAssignment = await _requestRead.GetPendingAssignmentByProfileId(request.ProfileRef , cancellationToken);
            if(pendingAssignment?.Any() == true)
                await _redisCache.SetAsync(key, pendingAssignment, TimeSpan.FromMinutes(5), cancellationToken);

            return pendingAssignment;
        }
    }
}
