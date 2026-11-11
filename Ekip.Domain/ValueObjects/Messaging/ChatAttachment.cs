namespace Ekip.Domain.ValueObjects.Messaging;

public sealed record ChatAttachment(
    Guid AssetId,
    string FileName,
    string MimeType,
    long FileSizeBytes,
    int? DurationSeconds = null,
    int? Width = null,
    int? Height = null
);