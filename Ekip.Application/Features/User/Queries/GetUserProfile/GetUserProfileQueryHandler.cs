using Ekip.Application.Contracts;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Queries.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Application.Features.User.Queries.GetUserProfile;

internal sealed class GetUserProfileQueryHandler(
    IUserQueries userQueries,
    ICacheService cacheService) : IQueryHandler<GetUserProfileQuery, UserProfileDto>
{
    private static readonly TimeSpan ProfileCacheTtl = TimeSpan.FromMinutes(5);

    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"user:profile:{request.UserId:D}";

        var cachedProfile = await cacheService.GetAsync<UserProfileDto>(cacheKey, cancellationToken);
        if (cachedProfile is not null)
            return cachedProfile;

        var profile = await userQueries.GetProfileByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return UserErrors.UserNotFound;

        await cacheService.SetAsync(cacheKey, profile, ProfileCacheTtl, cancellationToken);

        return profile;
    }
}