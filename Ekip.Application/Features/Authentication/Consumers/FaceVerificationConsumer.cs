using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Enums.Identity.Enums;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class FaceVerificationConsumer : IConsumer<FaceVerificationEvent>
    {
        private readonly IFaceVerificationService _faceService;
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IPublishEndpoint _publishEndpoint;
        public FaceVerificationConsumer(IFaceVerificationService faceService , IProfileWriteRepository profileWrite, IPublishEndpoint publishEndpoint)
        {
            _faceService = faceService;
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<FaceVerificationEvent> context)
        {
            var message = context.Message;

            var result = await _faceService.VerifyAsync(message.AvatarUrl, message.CapturedPhotoUrl, context.CancellationToken);

            if (result.IsMatch)
            {
                var archivedPhotoUrl = await _faceService.ArchiveFile(message.CapturedPhotoUrl, context.CancellationToken);

                await _profileWrite.UpdateFaceVerificationStatusAsync(
                    message.ProfileRef,
                    result.ReferenceId,
                    archivedPhotoUrl,
                    result.provider,
                    context.CancellationToken);
                await _publishEndpoint.Publish(new FaceVerificationCompletedEvent
                {
                    ProfileRef = message.ProfileRef,
                    VerificationLevel = VerificationLevel.PhotoVerified
                });
            }
            else
            {
                await _faceService.DeleteFile(message.CapturedPhotoUrl, context.CancellationToken);
                await _publishEndpoint.Publish(new FaceVerificationCompletedEvent
                {
                    ProfileRef = message.ProfileRef,
                    VerificationLevel = VerificationLevel.rejected
                });
            }
        }
    }
}
