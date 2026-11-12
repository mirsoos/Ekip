using Ekip.Domain.Base;
using Ekip.Domain.Enums.Moderation;

namespace Ekip.Domain.Entities.Moderation
{
    public class AbuseReport : BaseEntity,IAggregateRoot,IAuditableEntity
    {
        public Guid IdempotencyKey { get; private set; }
        public Guid ReporterId { get; private set; }
        public Guid TargetId { get; private set; }
        public ReportReasonCategory Reason { get; private set; }
        public ReportTargetType TargetType { get; private set; }
        public AbuseReportStatus Status { get; private set; } = AbuseReportStatus.Submitted;
        public string? AdminNote { get; private set; }
        public Guid? ResolvedByAdminId { get; private set; }
        public DateTime? ResolvedAtUtc { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        private readonly List<Guid> _evidenceMediaAssetIds = [];
        public IReadOnlyCollection<Guid> EvidenceMediaAssetIds => _evidenceMediaAssetIds.AsReadOnly();
    }
}
