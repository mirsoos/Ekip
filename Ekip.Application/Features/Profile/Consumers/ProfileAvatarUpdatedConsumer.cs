using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;

namespace Ekip.Application.Features.Profile.Consumers
{
    public class ProfileAvatarUpdatedConsumer : IConsumer<ProfileAvatarUpdatedEvent>
    {
        private readonly IProfileReadRepository _profileRead;
        public ProfileAvatarUpdatedConsumer(IProfileReadRepository profileRead)
        {
            _profileRead = profileRead;
        }
        async Task IConsumer<ProfileAvatarUpdatedEvent>.Consume(ConsumeContext<ProfileAvatarUpdatedEvent> context)
        {
            var newAvatarUrl = context.Message;

            await _profileRead.UpdateAvatarAsync(newAvatarUrl.ProfileRef , newAvatarUrl.AvatarUrl,context.CancellationToken);
        }
    }
}
