using Ekip.Domain.Base;
using Ekip.Domain.Enums.Ekip;
using Ekip.Domain.ValueObjects.Ekip;

namespace Ekip.Domain.Entities.Ekip;

public class RecurringEkip : BaseEntity, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public Guid CreatorId { get; private set; }
    public Guid SourceEkipId { get; private set; }
    public RecurrenceFrequency Frequency { get; private set; }
    public int JoinDeadlineOffsetHours { get; private set; }
    public int OccurrencesCount { get; private set; } = 1;
    public int? MaxOccurrences { get; private set; }
    public DateTime LastEventDateTimeUtc { get; private set; }
    public DateTime? EndsAtUtc { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAtUtc { get; private set; }
}