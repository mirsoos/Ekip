
namespace Ekip.Application.Contracts.Events
{
    public record FaceVerificationEvent
    {
        public Guid UserRef { get; init; }
        public string AvatarUrl { get; set; }
        public string CapturedPhotoUrl { get; init; }
    }
}
