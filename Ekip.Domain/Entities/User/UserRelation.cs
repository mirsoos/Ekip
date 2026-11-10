using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;

namespace Ekip.Domain.Entities.User
{
    public class UserRelation : BaseEntity , IAggregateRoot , IAuditableEntity
    {
        public Guid RequesterId { get; private set; }
        public Guid ReceiverId { get; private set; }
        public UserRelationStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
    }
}
