using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.User;

public sealed record MoodWidget
{
    public string? MoodEmoji { get; private init; }
    public string? MoodDescription { get; private init; }
    public string? MoodColor { get; private init; }
    public DateTime CreatedAtUtc { get; private init; }

    private MoodWidget() { }

    private MoodWidget(
        string? moodEmoji,
        string? moodDescription,
        string? moodColor,
        DateTime createdAtUtc)
    {
        MoodEmoji = moodEmoji;
        MoodDescription = moodDescription;
        MoodColor = moodColor;
        CreatedAtUtc = createdAtUtc;
    }

    public static Result<MoodWidget> Create(
        string? moodEmoji,
        string? moodDescription,
        string? moodColor,
        DateTime createdAtUtc)
    {
        var emoji = string.IsNullOrWhiteSpace(moodEmoji) ? null : moodEmoji.Trim();
        var desc = string.IsNullOrWhiteSpace(moodDescription) ? null : moodDescription.Trim();
        var color = string.IsNullOrWhiteSpace(moodColor) ? null : moodColor.Trim();

        if (emoji is null && desc is null)
            return UserErrors.InvalidMoodWidget;

        if (emoji?.Length > 16 || desc?.Length > 140 || color?.Length > 20)
            return UserErrors.InvalidMoodWidget;

        return new MoodWidget(emoji, desc, color, createdAtUtc);
    }
}