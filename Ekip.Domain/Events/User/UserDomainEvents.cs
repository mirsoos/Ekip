using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;

namespace Ekip.Domain.Events.User;

public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    Guid? ReferredById,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserFaceVerifiedDomainEvent(
    Guid UserId,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserNationalIdVerifiedDomainEvent(
    Guid UserId,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserAccountStatusChangedDomainEvent(
    Guid UserId,
    UserAccountStatus NewStatus,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserProfileUpdatedDomainEvent(
    Guid UserId,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserRelationChangedDomainEvent(
    Guid RelationId,
    Guid RequesterId,
    Guid ReceiverId,
    UserRelationStatus Status,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);