using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Configuration
{
    public class SystemSetting : BaseEntity,IAggregateRoot,IAuditableEntity
    {
        public string Key { get; private set; }
        public string ValueJson { get; private set; }
        public string Description { get; private set; }
        public bool IsPublic { get; private set; }
        public Guid CreatedByAdminId { get; private set; }
        public Guid? UpdatedByAdminId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
    }
}
