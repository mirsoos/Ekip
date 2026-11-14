using Ekip.Domain.Base;
using Ekip.Domain.Enums.Review;

namespace Ekip.Domain.Events.Review;

public sealed record ProfileReviewSubmittedDomainEvent(
    Guid ReviewId,
    Guid TargetUserId,
    Guid AuthorUserId,
    Guid EkipId,
    int Score,
    bool RequiresAiScan,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record ProfileReviewModerationCompletedDomainEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int Score,
    ProfileReviewModerationStatus OldStatus,
    ProfileReviewModerationStatus NewStatus,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record ProfileReviewUpdatedDomainEvent(
    Guid ReviewId,
    Guid TargetUserId,
    int OldScore,
    ProfileReviewModerationStatus OldStatus,
    int NewScore,
    ProfileReviewModerationStatus NewStatus,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);