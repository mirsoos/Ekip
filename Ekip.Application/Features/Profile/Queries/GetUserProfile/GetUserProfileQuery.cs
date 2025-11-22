using Ekip.Application.DTOs.User;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<ProfileDto>
    {
        public long ProfileRef { get; set; }
    }
}
