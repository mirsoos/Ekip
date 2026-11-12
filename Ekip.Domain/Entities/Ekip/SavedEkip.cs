using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Ekip
{
    public class SavedEkip : BaseEntity,IAggregateRoot,ICreatedAuditable
    {
        public Guid UserId { get; private set; }
        public Guid EkipId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
