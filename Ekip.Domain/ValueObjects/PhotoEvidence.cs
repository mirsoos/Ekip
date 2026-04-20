
namespace Ekip.Domain.ValueObjects
{
    public record PhotoEvidence
    {
        public Guid ReferenceId { get; init; }
        public DateTime VerifiedAt { get; init; }
        public string Provider { get; init; }
        public string CapturedPhotoUrl { get; init; }
        public PhotoEvidence(Guid referenceId, string provider,string capturedPhotoUrl)
        {
            ReferenceId = referenceId;
            Provider = provider;
            CapturedPhotoUrl = capturedPhotoUrl;
            VerifiedAt = DateTime.UtcNow;
        }
    }
}