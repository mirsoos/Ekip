using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MediatR;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;

namespace Ekip.Application.Features.Profile.Commands.SetUserProfile
{
    public class SetUserProfileCommandHandler : IRequestHandler<SetUserProfileCommand, CreatedProfileDto>
    {
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IUserReadRepository _userRead;

        public SetUserProfileCommandHandler(IProfileWriteRepository profileWrite,IUserReadRepository userRead)
        {
            _profileWrite = profileWrite;
            _userRead = userRead;
        }

        public async Task<CreatedProfileDto> Handle(SetUserProfileCommand request, CancellationToken cancellationToken)
        {
            var getUserDetails = await _userRead.GetByUserNameAsync(request.UserName,cancellationToken);

            if(getUserDetails == null)
                throw new Exception($"User with username '{request.UserName}' not found.");

            var profileExists = await _profileWrite.DoesProfileExistForUserAsync(getUserDetails.Id, cancellationToken);

            if (profileExists)
                throw new InvalidOperationException($"A Profile already Exists for User '{request.UserName}'.");
            
            var newProfile = new ProfileEntity(getUserDetails);

            var savedProfile = await _profileWrite.AddAsync(newProfile,cancellationToken);

            var resultDto = new CreatedProfileDto {
                ProfileRef = savedProfile.Id,
                 FirstName = savedProfile.UserDetails.FirstName,
                 LastName = savedProfile.UserDetails.LastName,
                 Email = savedProfile.UserDetails.Email,
                 Gender = savedProfile.UserDetails.Gender,
                 UserName = savedProfile.UserDetails.UserName,
                 AvatarUrl = savedProfile.AvatarUrl,
                 Exprience = savedProfile.Exprience,
                 Score = savedProfile.Score
            };

            return resultDto;
        }
    }
}
