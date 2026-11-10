using Ekip.Domain.Base;
using Ekip.Domain.Enums.Review;

namespace Ekip.Domain.Entities.Review
{
    public class ProfileReview : BaseEntity , IAggregateRoot, IAuditableEntity, ISoftDeletable
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
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
    }
}
