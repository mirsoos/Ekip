using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class FaceVerificationCompletedConsumer : IConsumer<FaceVerificationCompletedEvent>
    {
        private readonly IProfileReadRepository _profileRead;
        public FaceVerificationCompletedConsumer(IProfileReadRepository profileRead)
        {
            _profileRead = profileRead;
        }
        public async Task Consume(ConsumeContext<FaceVerificationCompletedEvent> context)
        {
            var message = context.Message;

            await _profileRead.UpdateFaceVerificationStatusAsync(message.ProfileRef , message.VerificationLevel , context.CancellationToken);
        }
    }
}
