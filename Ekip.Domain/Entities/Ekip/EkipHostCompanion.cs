using Ekip.Domain.Base;
using Ekip.Domain.Enums.Ekip;

namespace Ekip.Domain.Entities.Ekip
{
    public class EkipHostCompanion : BaseEntity , ICreatedAuditable
    {
        public Guid EkipId { get; private set; }
        public Guid CompanionUserId { get; private set; }
        public HostCompanionStatus CompanionStatus { get; private set; } = HostCompanionStatus.Pending;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? RespondedAtUtc { get; private set; }
    }
}
