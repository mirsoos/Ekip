namespace Ekip.Domain.ValueObjects.Chat;

public sealed record ChatTheme(
    Guid ChatId,
    string BackgroundAssetId,
    DateTime SetAtUtc
);