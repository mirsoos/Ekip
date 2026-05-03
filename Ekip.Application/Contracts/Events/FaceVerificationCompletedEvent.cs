
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record FaceVerificationCompletedEvent
    {
        public Guid UserRef { get; set; }
        public VerificationLevel VerificationLevel { get; set; }
        public Guid ReferenceId { get; set; }
        public string archivedPhotoUrl { get; set; }
        public string Provider { get; set; }
    }
}
