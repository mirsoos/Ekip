
namespace Ekip.Application.Contracts.Events
{
    public record FaceVerificationEvent
    {
        public Guid ProfileRef { get; init; }
        public string AvatarUrl { get; set; }
        public string CapturedPhotoUrl { get; init; }
    }
}
