using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Profile.Consumers
{
    public class ProfileCreatedConsumer : IConsumer<ProfileCreatedEvent>
    {
        private readonly IProfileReadRepository _profileRead;

        public ProfileCreatedConsumer(IProfileReadRepository profileRead)
        {
            _profileRead = profileRead;
        }

        public async Task Consume(ConsumeContext<ProfileCreatedEvent> context)
        {
            var profile = context.Message;

            var mongoToPostgre = new ProfileReadModel
            {
                Id = profile.Id,
                UserRef = profile.UserRef,
                AvatarUrl = profile.AvatarUrl,
                Experience = profile.Experience,
                Score = profile.Score
            };

            await _profileRead.AddProfileAsync(mongoToPostgre, context.CancellationToken);
        }
    }
}
