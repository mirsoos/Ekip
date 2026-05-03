using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Identity.Enums;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserReadRepository _userRead;
        private readonly IProfileReadRepository _profileRead;
        public UserCreatedConsumer(IUserReadRepository userRead , IProfileReadRepository profileRead)
        {
            _userRead = userRead;
            _profileRead = profileRead;
        }
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;

            var userMongoToPostgres = new UserReadModel
            {
                Id = user.UserRef,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CreateDate = user.CreateDate,
                Age = user.Age,
                Email = user.Email,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
            };

            var profileMongoToPostgre = new ProfileReadModel
            {
                UserRef = user.UserRef,
                Experience = user.Experience,
                Score = user.Score,
                VerificationLevel = VerificationLevel.None,
                Bio = user.Bio
            };

            await _userRead.AddUserAsync(userMongoToPostgres, context.CancellationToken);
            await _profileRead.AddProfileAsync(profileMongoToPostgre, context.CancellationToken);
        }
    }
}
