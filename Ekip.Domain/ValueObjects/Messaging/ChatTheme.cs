namespace Ekip.Domain.ValueObjects.Messaging;

public sealed record ChatTheme(
    Guid BackgroundAssetId,
    DateTime SetAtUtc
);