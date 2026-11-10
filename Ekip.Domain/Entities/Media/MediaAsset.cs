using Ekip.Domain.Base;
using Ekip.Domain.Enums.Media;

namespace Ekip.Domain.Entities.Media
{
    public class MediaAsset : BaseEntity, IAggregateRoot,IAuditableEntity,ISoftDeletable
    {
        public Guid UploaderId { get; private set; }
        public MediaUsageContext UsageContext { get; private set; }
        public string StorageKey { get; private set; }
        public decimal? NsfwProbability { get; private set; }
        public MediaModerationStatus ModerationStatus { get; private set; }
        public string? Error { get; private set; }
        public DateTime? ScannedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
    }
}
