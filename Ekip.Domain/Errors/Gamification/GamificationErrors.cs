using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.Gamification;

public static class GamificationErrors
{
    public static readonly Error InvalidEventType = Error.Validation(
        "GAMIFICATION.INVALID_EVENT_TYPE",
        "Reputation event type is invalid.");

    public static readonly Error InvalidReputationDelta = Error.Validation(
        "GAMIFICATION.INVALID_REPUTATION_DELTA",
        "At least one of XP delta or Honor delta must be non-zero.");

    public static readonly Error SelfActorNotAllowed = Error.Validation(
        "GAMIFICATION.SELF_ACTOR_NOT_ALLOWED",
        "A user cannot trigger a peer reputation event for themselves.");

    public static readonly Error InvalidRatingScore = Error.Validation(
        "GAMIFICATION.INVALID_RATING_SCORE",
        "Rating score must be between 1 and 5.");

    public static readonly Error InvalidDescription = Error.Validation(
        "GAMIFICATION.INVALID_DESCRIPTION",
        "Description cannot exceed 500 characters.");

    public static readonly Error InvalidReferenceEntityType = Error.Validation(
        "GAMIFICATION.INVALID_REFERENCE_ENTITY_TYPE",
        "Reference entity type cannot exceed 100 characters.");

    public static readonly Error InvalidBalance = Error.Validation(
        "GAMIFICATION.INVALID_BALANCE",
        "XP balance after transaction cannot be negative.");

    public static readonly Error ManualAdjustmentRequiresExplicitDeltas = Error.Validation(
        "GAMIFICATION.MANUAL_ADJUSTMENT_REQUIRES_EXPLICIT_DELTAS",
        "Admin manual reputation adjustments require explicit XP or Honor deltas.");
}