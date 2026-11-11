using Ekip.Domain.Base;
using Ekip.Domain.Enums.EkipApplication;

namespace Ekip.Domain.Entities.EkipApplication
{
    public class ApplicationCompanion : BaseEntity ,ICreatedAuditable
    {
        public Guid EkipApplicationId { get; private set; }
        public Guid CompanionUserId { get; private set; }
        public ApplicationCompanionStatus Status { get; private set; } = ApplicationCompanionStatus.Pending;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? RespondedAtUtc { get; private set; }
    }
}
