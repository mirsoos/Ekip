
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record FaceVerificationCompletedEvent
    {
        public Guid ProfileRef { get; set; }
        public VerificationLevel VerificationLevel { get; set; }
    }
}
