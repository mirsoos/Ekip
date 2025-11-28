using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;
using ProfileEntity = Ekip.Domain.Entities.Identity.Entities.Profile;

namespace Ekip.Application.Features.Profile.Commands.SetUserProfile
{
    public class SetUserProfileCommandHandler : IRequestHandler<SetUserProfileCommand, CreatedProfileDto>
    {
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IUserReadRepository _userRead;
        private readonly IPublishEndpoint _publishEndpoint;

        public SetUserProfileCommandHandler(IProfileWriteRepository profileWrite,IUserReadRepository userRead , IPublishEndpoint publishEndpoint)
        {
            _profileWrite = profileWrite;
            _userRead = userRead;
            _publishEndpoint = publishEndpoint;
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

            await _publishEndpoint.Publish(new ProfileCreatedEvent
            {
                AvatarUrl = savedProfile.AvatarUrl,
                Experience = savedProfile.Experience,
                Score = savedProfile.Score,
                Id = savedProfile.Id,
                UserRef = savedProfile.UserDetails.Id
                
            });

            var resultDto = new CreatedProfileDto {
                ProfileRef = savedProfile.Id,
                 FirstName = savedProfile.UserDetails.FirstName,
                 LastName = savedProfile.UserDetails.LastName,
                 Email = savedProfile.UserDetails.Email,
                 Gender = savedProfile.UserDetails.Gender,
                 UserName = savedProfile.UserDetails.UserName,
                 AvatarUrl = savedProfile.AvatarUrl,
                 Experience = savedProfile.Experience,
                 Score = savedProfile.Score
            };

            return resultDto;
        }
    }
}
