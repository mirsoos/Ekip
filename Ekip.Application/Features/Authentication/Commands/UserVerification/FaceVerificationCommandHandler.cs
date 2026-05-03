using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.UserVerification
{
    public class FaceVerificationCommandHandler : IRequestHandler<FaceVerificationCommand, string>
    {
        private readonly IUserWriteRepository _userWrite;
        private readonly IEventPublisher _eventPublisher;

        public FaceVerificationCommandHandler(IUserWriteRepository userWrite , IEventPublisher eventPublisher)
        {
            _userWrite = userWrite;
            _eventPublisher = eventPublisher;
        }

        public async Task<string> Handle(FaceVerificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userWrite.GetByUserIdAsync(request.UserRef, cancellationToken);

            if (user == null) throw new Exception("User Not Found.");
            if (string.IsNullOrEmpty(user.Profile.AvatarUrl)) throw new Exception("User has no Avatar to compare.");

            await _eventPublisher.Publish(new FaceVerificationEvent
            {
                CapturedPhotoUrl = request.CapturedPhotoUrl,
                UserRef = request.UserRef,
                AvatarUrl = user.Profile.AvatarUrl
            },cancellationToken);

            return "در صف بررسی...";
        }
    }
}
