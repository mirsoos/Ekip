using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.Review;

public static class ReviewErrors
{
    public static readonly Error SelfReviewNotAllowed = Error.Validation(
        "REVIEW.SELF_REVIEW_NOT_ALLOWED",
        "A user cannot submit a review for themselves.");

    public static readonly Error InvalidScore = Error.Validation(
        "REVIEW.INVALID_SCORE",
        "Review score must be between 1 and 5.");

    public static readonly Error InvalidComment = Error.Validation(
        "REVIEW.INVALID_COMMENT",
        "Review comment cannot exceed 1000 characters.");

    public static readonly Error InvalidToxicityScore = Error.Validation(
        "REVIEW.INVALID_TOXICITY_SCORE",
        "Toxicity score must be between 0 and 1.");

    public static readonly Error InvalidModerationTransition = Error.Conflict(
        "REVIEW.INVALID_MODERATION_TRANSITION",
        "This moderation transition is not allowed for the current review status.");
}