namespace Ekip.Domain.Base;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOnUtc { get; }
}

public abstract record DomainEventBase(Guid EventId, DateTime OccurredOnUtc) : IDomainEvent
{
    protected DomainEventBase() : this(Guid.CreateVersion7(), DateTime.UtcNow)
    {
    }
}