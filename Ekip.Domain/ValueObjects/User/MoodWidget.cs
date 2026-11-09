namespace Ekip.Domain.ValueObjects.User;
public sealed record MoodWidget(
    string? MoodEmoji,
    string? MoodDescription,
    string? MoodColor,
    DateTime CreatedAtUtc
);
