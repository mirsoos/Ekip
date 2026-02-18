
namespace Ekip.Domain.ValueObjects
{
    public record PhotoEvidence
    {
        public Guid ReferenceId { get; private set; }
        public DateTime VerifiedAt { get; private set; }
        public string? Provider { get; private set; }
        public string CapturedPhotoUrl { get; set; }
        public PhotoEvidence(Guid referenceId, string provider,string capturedPhotoUrl)
        {
            ReferenceId = referenceId;
            Provider = provider;
            CapturedPhotoUrl = capturedPhotoUrl;
            VerifiedAt = DateTime.UtcNow;
        }
    }
}