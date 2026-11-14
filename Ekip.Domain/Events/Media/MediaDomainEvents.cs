using Ekip.Domain.Base;
using Ekip.Domain.Enums.Media;

namespace Ekip.Domain.Events.Media;

public sealed record MediaUploadedDomainEvent(
    Guid MediaAssetId,
    Guid UploaderId,
    MediaUsageContext UsageContext,
    string StorageKey,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record MediaModerationCompletedDomainEvent(
    Guid MediaAssetId,
    Guid UploaderId,
    MediaModerationStatus Status,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);