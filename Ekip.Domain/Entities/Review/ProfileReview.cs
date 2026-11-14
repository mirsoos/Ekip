using Ekip.Domain.Base;
using Ekip.Domain.Enums.Review;
using Ekip.Domain.Errors.Review;
using Ekip.Domain.Events.Review;
using Ekip.Domain.Results;

namespace Ekip.Domain.Entities.Review;

public class ProfileReview : BaseEntity, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public Guid TargetUserId { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public Guid EkipId { get; private set; }
    public int Score { get; private set; }
    public string? CommentText { get; private set; }
    public decimal? ToxicityScore { get; private set; }
    public ProfileReviewModerationStatus ModerationStatus { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private ProfileReview() { }

    private ProfileReview(
        Guid targetUserId,
        Guid authorUserId,
        Guid ekipId,
        int score,
        string? commentText,
        ProfileReviewModerationStatus moderationStatus,
        Guid idempotencyKey)
    {
        TargetUserId = targetUserId;
        AuthorUserId = authorUserId;
        EkipId = ekipId;
        Score = score;
        CommentText = commentText;
        ModerationStatus = moderationStatus;
        IdempotencyKey = idempotencyKey;
    }

    public static Result<ProfileReview> Submit(
        Guid targetUserId,
        Guid authorUserId,
        Guid ekipId,
        int score,
        string? commentText,
        Guid idempotencyKey,
        DateTime utcNow)
    {
        if (targetUserId == Guid.Empty)
            throw new ArgumentException("TargetUserId cannot be empty.", nameof(targetUserId));
        if (authorUserId == Guid.Empty)
            throw new ArgumentException("AuthorUserId cannot be empty.", nameof(authorUserId));
        if (ekipId == Guid.Empty)
            throw new ArgumentException("EkipId cannot be empty.", nameof(ekipId));
        if (idempotencyKey == Guid.Empty)
            throw new ArgumentException("IdempotencyKey cannot be empty.", nameof(idempotencyKey));

        if (authorUserId == targetUserId)
            return ReviewErrors.SelfReviewNotAllowed;

        if (score is < 1 or > 5)
            return ReviewErrors.InvalidScore;

        var normalizedComment = string.IsNullOrWhiteSpace(commentText) ? null : commentText.Trim();
        if (normalizedComment?.Length > 1000)
            return ReviewErrors.InvalidComment;

        var requiresAiScan = normalizedComment is not null;
        var initialStatus = requiresAiScan
            ? ProfileReviewModerationStatus.PendingByAI
            : ProfileReviewModerationStatus.Approved;

        var review = new ProfileReview(
            targetUserId,
            authorUserId,
            ekipId,
            score,
            normalizedComment,
            initialStatus,
            idempotencyKey);

        review.AddDomainEvent(new ProfileReviewSubmittedDomainEvent(
            review.Id,
            targetUserId,
            authorUserId,
            ekipId,
            score,
            requiresAiScan,
            utcNow));

        return review;
    }

    public Result ApplyAiModerationResult(
        decimal toxicityScore,
        decimal approveThreshold,
        decimal rejectThreshold,
        DateTime utcNow)
    {
        if (approveThreshold < 0m || rejectThreshold > 1m || approveThreshold > rejectThreshold)
            throw new ArgumentException("Invalid moderation threshold range.");

        if (ModerationStatus != ProfileReviewModerationStatus.PendingByAI)
            return ReviewErrors.InvalidModerationTransition;

        if (toxicityScore is < 0m or > 1m)
            return ReviewErrors.InvalidToxicityScore;

        var oldStatus = ModerationStatus;
        ToxicityScore = decimal.Round(toxicityScore, 4, MidpointRounding.AwayFromZero);

        ModerationStatus = ToxicityScore < approveThreshold
            ? ProfileReviewModerationStatus.Approved
            : ToxicityScore >= rejectThreshold
                ? ProfileReviewModerationStatus.Rejected
                : ProfileReviewModerationStatus.EscalatedToAdmin;

        AddDomainEvent(new ProfileReviewModerationCompletedDomainEvent(
            Id,
            TargetUserId,
            Score,
            oldStatus,
            ModerationStatus,
            utcNow));

        return Result.Success();
    }

    public Result ModerateByAdmin(bool approve, DateTime utcNow)
    {
        var newStatus = approve
            ? ProfileReviewModerationStatus.Approved
            : ProfileReviewModerationStatus.Rejected;

        if (ModerationStatus == newStatus)
            return Result.Success();

        var oldStatus = ModerationStatus;
        ModerationStatus = newStatus;

        AddDomainEvent(new ProfileReviewModerationCompletedDomainEvent(
            Id,
            TargetUserId,
            Score,
            oldStatus,
            newStatus,
            utcNow));

        return Result.Success();
    }

    public Result UpdateContent(int newScore, string? newCommentText, DateTime utcNow)
    {
        if (newScore is < 1 or > 5)
            return ReviewErrors.InvalidScore;

        var normalizedComment = string.IsNullOrWhiteSpace(newCommentText) ? null : newCommentText.Trim();
        if (normalizedComment?.Length > 1000)
            return ReviewErrors.InvalidComment;

        var oldScore = Score;
        var oldStatus = ModerationStatus;

        Score = newScore;
        CommentText = normalizedComment;
        ToxicityScore = null;
        ModerationStatus = normalizedComment is null
            ? ProfileReviewModerationStatus.Approved
            : ProfileReviewModerationStatus.PendingByAI;

        AddDomainEvent(new ProfileReviewUpdatedDomainEvent(
            Id,
            TargetUserId,
            oldScore,
            oldStatus,
            Score,
            ModerationStatus,
            utcNow));

        return Result.Success();
    }
}