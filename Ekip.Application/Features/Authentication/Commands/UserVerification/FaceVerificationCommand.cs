using MediatR;

namespace Ekip.Application.Features.Authentication.Commands.UserVerification
{
    public class FaceVerificationCommand : IRequest<string>
    {
        public Guid ProfileRef { get; set; }
        public string CapturedPhotoUrl { get; set; }
    }
}
