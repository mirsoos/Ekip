using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class FaceVerificationCompletedConsumer : IConsumer<FaceVerificationCompletedEvent>
    {
        private readonly IUserWriteRepository _userWrite;
        public FaceVerificationCompletedConsumer(IUserWriteRepository userWrite)
        {
            _userWrite = userWrite;
        }
        public async Task Consume(ConsumeContext<FaceVerificationCompletedEvent> context)
        {
            var message = context.Message;

            await _userWrite.UpdateFaceVerificationStatusAsync(message.UserRef ,message.ReferenceId, message.archivedPhotoUrl , message.Provider , context.CancellationToken);
        }
    }
}
