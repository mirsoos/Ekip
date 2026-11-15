using Ekip.Application.Contracts;
using Ekip.Application.DTOs.User;

namespace Ekip.Application.Features.User.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IQuery<UserProfileDto>;