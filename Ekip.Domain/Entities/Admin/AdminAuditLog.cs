using Ekip.Domain.Base;
using Ekip.Domain.Enums.Admin;

namespace Ekip.Domain.Entities.Admin
{
    public class AdminAuditLog : BaseEntity , IAggregateRoot,ICreatedAuditable
    {
        public Guid AdminId { get; private set; }
        public AdminActionType ActionType { get; private set; }
        public string TargetEntity { get; private set; }
        public Guid TargetId { get; private set; }
        public string? OldValueJson { get; private set; }
        public string NewValueJson { get; private set; }
        public string? Reason { get; private set; }
        public string AdminIpAddress { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
