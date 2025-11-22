using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MediatR;

namespace Ekip.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileRead;

        public GetUserProfileQueryHandler(IProfileReadRepository profileRead)
        {
            _profileRead = profileRead;
        }

        public async Task<ProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var profileDto = await _profileRead.GetProfileDetailsByIdAsync(request.ProfileRef,cancellationToken);

            if (profileDto == null)
                throw new Exception($"Profile with Id'{request.ProfileRef}'Not Found");

            return profileDto;
        }
    }
}
