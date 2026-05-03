using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Enums.Identity.Enums;
using MassTransit;

namespace Ekip.Application.Features.Authentication.Consumers
{
    public class FaceVerificationConsumer : IConsumer<FaceVerificationEvent>
    {
        private readonly IFaceVerificationService _faceService;
        private readonly IUserWriteRepository _userWrite;
        private readonly IEventPublisher _eventPublisher;
        public FaceVerificationConsumer(IFaceVerificationService faceService , IUserWriteRepository userWrite, IEventPublisher eventPublisher)
        {
            _faceService = faceService;
            _userWrite = userWrite;
            _eventPublisher = eventPublisher;
        }
        public async Task Consume(ConsumeContext<FaceVerificationEvent> context)
        {
            var message = context.Message;

            var result = await _faceService.VerifyAsync(message.AvatarUrl, message.CapturedPhotoUrl, context.CancellationToken);

            if (result.IsMatch)
            {
                var archivedPhotoUrl = await _faceService.ArchiveFileAsync(message.CapturedPhotoUrl, context.CancellationToken);

                await _userWrite.UpdateFaceVerificationStatusAsync(
                    message.UserRef,
                    result.ReferenceId,
                    archivedPhotoUrl,
                    result.provider,
                    context.CancellationToken);

                await _eventPublisher.Publish(new FaceVerificationCompletedEvent
                {
                    UserRef = message.UserRef,
                    VerificationLevel = VerificationLevel.PhotoVerified,
                    ReferenceId = result.ReferenceId,
                    archivedPhotoUrl = archivedPhotoUrl,
                    Provider = result.provider
                },context.CancellationToken);
            }
            else
            {
                await _faceService.DeleteFileAsync(message.CapturedPhotoUrl, context.CancellationToken);
                await _eventPublisher.Publish(new FaceVerificationCompletedEvent
                {
                    UserRef = message.UserRef,
                    VerificationLevel = VerificationLevel.rejected
                },context.CancellationToken);
            }
        }
    }
}
