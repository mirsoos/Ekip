using Ekip.Domain.Base;
using Ekip.Domain.Enums.Ekip;
using Ekip.Domain.ValueObjects.Ekip;

namespace Ekip.Domain.Entities.Ekip
{
    public class EkipMember :BaseEntity , IAuditableEntity, IAggregateRoot
    {
        public Guid EkipId { get; private set; }
        public Guid UserId { get; private set; }
        public EkipRole Role { get; private set; }
        public Guid? ApplicationId { get; private set; }
        public EkipMembershipStatus MembershipStatus { get; private set; } = EkipMembershipStatus.Active;
        public AttendanceStatus AttendanceStatus { get; private set; } = AttendanceStatus.Registered;
        public DateTime? CheckInAtUtc { get; private set; }
        public GeoLocation? CheckInLocation { get; private set; }
        public int NoShowReportedCount { get; private set; } = 0;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
    }
}
