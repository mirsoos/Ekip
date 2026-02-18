using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.UserVerification
{
    public class FaceVerificationCommandHandler : IRequestHandler<FaceVerificationCommand, string>
    {
        private readonly IProfileWriteRepository _profileWrite;
        private readonly IPublishEndpoint _publishEndpoint;

        public FaceVerificationCommandHandler(IProfileWriteRepository profileWrite , IPublishEndpoint publishEndpoint)
        {
            _profileWrite = profileWrite;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<string> Handle(FaceVerificationCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileWrite.GetByIdAsync(request.ProfileRef, cancellationToken);

            if (profile == null) throw new Exception("Profile Not Found.");
            if (string.IsNullOrEmpty(profile.AvatarUrl)) throw new Exception("Profile has no Avatar to compare.");

            await _publishEndpoint.Publish(new FaceVerificationEvent
            {
                CapturedPhotoUrl = request.CapturedPhotoUrl,
                ProfileRef = request.ProfileRef,
                AvatarUrl = profile.AvatarUrl
            });

            return "در صف بررسی...";
        }
    }
}
