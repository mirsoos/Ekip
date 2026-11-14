using Ekip.Domain.Base;
using Ekip.Domain.Enums.Media;
using Ekip.Domain.Errors.Media;
using Ekip.Domain.Events.Media;
using Ekip.Domain.Results;

namespace Ekip.Domain.Entities.Media;

public class MediaAsset : BaseEntity, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public Guid UploaderId { get; private set; }
    public MediaUsageContext UsageContext { get; private set; }
    public string StorageKey { get; private set; }
    public decimal? NsfwProbability { get; private set; }
    public MediaModerationStatus ModerationStatus { get; private set; } = MediaModerationStatus.Pending;
    public string? Error { get; private set; }
    public DateTime? ScannedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private MediaAsset()
    {
        StorageKey = null!;
    }

    private MediaAsset(Guid uploaderId, MediaUsageContext usageContext, string storageKey)
    {
        UploaderId = uploaderId;
        UsageContext = usageContext;
        StorageKey = storageKey;
        ModerationStatus = MediaModerationStatus.Pending;
    }

    public static Result<MediaAsset> Create(
        Guid uploaderId,
        MediaUsageContext usageContext,
        string storageKey,
        DateTime utcNow)
    {
        if (uploaderId == Guid.Empty)
            throw new ArgumentException("UploaderId cannot be empty.", nameof(uploaderId));

        if (!Enum.IsDefined(usageContext))
            return MediaErrors.InvalidUsageContext;

        if (string.IsNullOrWhiteSpace(storageKey) || storageKey.Trim().Length > 500)
            return MediaErrors.InvalidStorageKey;

        var normalizedKey = storageKey.Trim();
        var asset = new MediaAsset(uploaderId, usageContext, normalizedKey);
        asset.AddDomainEvent(new MediaUploadedDomainEvent(asset.Id, uploaderId, usageContext, normalizedKey, utcNow));
        return asset;
    }

    public Result ApplyAiScanResult(
        decimal nsfwProbability,
        decimal approveThreshold,
        decimal rejectThreshold,
        DateTime utcNow)
    {
        if (approveThreshold < 0m || rejectThreshold > 1m || approveThreshold > rejectThreshold)
            throw new ArgumentException("Invalid moderation threshold range.");

        if (ModerationStatus != MediaModerationStatus.Pending)
            return MediaErrors.InvalidModerationTransition;

        if (nsfwProbability is < 0m or > 1m)
            return MediaErrors.InvalidNsfwProbability;

        NsfwProbability = decimal.Round(nsfwProbability, 4, MidpointRounding.AwayFromZero);
        ScannedAtUtc = utcNow;
        Error = null;

        ModerationStatus = NsfwProbability < approveThreshold
            ? MediaModerationStatus.Approved
            : NsfwProbability >= rejectThreshold
                ? MediaModerationStatus.Rejected
                : MediaModerationStatus.EscalatedToAdmin;

        AddDomainEvent(new MediaModerationCompletedDomainEvent(Id, UploaderId, ModerationStatus, utcNow));
        return Result.Success();
    }

    public Result MarkAiScanFailed(string error, DateTime utcNow)
    {
        if (ModerationStatus != MediaModerationStatus.Pending)
            return MediaErrors.InvalidModerationTransition;

        if (string.IsNullOrWhiteSpace(error) || error.Trim().Length > 1000)
            return MediaErrors.InvalidErrorMessage;

        Error = error.Trim();
        ScannedAtUtc = utcNow;
        ModerationStatus = MediaModerationStatus.EscalatedToAdmin;

        AddDomainEvent(new MediaModerationCompletedDomainEvent(Id, UploaderId, ModerationStatus, utcNow));
        return Result.Success();
    }

    public Result ModerateByAdmin(bool approve, string? reason, DateTime utcNow)
    {
        var normalizedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (normalizedReason?.Length > 1000)
            return MediaErrors.InvalidErrorMessage;

        var targetStatus = approve ? MediaModerationStatus.Approved : MediaModerationStatus.Rejected;
        if (ModerationStatus == targetStatus && Error == (approve ? null : normalizedReason))
            return Result.Success();

        var statusChanged = ModerationStatus != targetStatus;
        ModerationStatus = targetStatus;
        Error = approve ? null : normalizedReason;

        if (statusChanged)
            AddDomainEvent(new MediaModerationCompletedDomainEvent(Id, UploaderId, ModerationStatus, utcNow));

        return Result.Success();
    }
}