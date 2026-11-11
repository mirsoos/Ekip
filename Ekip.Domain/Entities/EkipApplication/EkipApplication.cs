using Ekip.Domain.Base;
using Ekip.Domain.Enums.Ekip;
using Ekip.Domain.Enums.EkipApplication;

namespace Ekip.Domain.Entities.EkipApplication
{
    public class EkipApplication : BaseEntity , ISoftDeletable,IAuditableEntity,IAggregateRoot
    {
        public Guid IdempotencyKey { get; private set; }
        public Guid EkipId { get; private set; }
        public Guid ApplicantId { get; private set; }
        public EkipComposition JoinType { get; private set; }
        public string IntroNote { get; private set; }
        public EkipApplicationStatus ApplicationStatus { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
        private readonly List<ApplicationCompanion> _companions = [];
        public IReadOnlyCollection<ApplicationCompanion> Companions => _companions.AsReadOnly();
        public int PartySize => _companions.Count + 1;
    }
}
