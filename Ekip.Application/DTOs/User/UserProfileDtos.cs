using Ekip.Domain.Enums.User;

namespace Ekip.Application.DTOs.User;

public sealed record MoodWidgetDto(
    string? MoodEmoji,
    string? MoodDescription,
    string? MoodColor,
    DateTime CreatedAtUtc);

public sealed record SocialLinkDto(
    string Platform,
    string Url,
    DateTime LinkedAtUtc);

public sealed record UserProfileDto(
    Guid UserId,
    string Username,
    string FirstName,
    string LastName,
    Gender UserGender,
    DateOnly BirthDate,
    Guid? CityId,
    Guid? AvatarAssetId,
    Guid? BannerAssetId,
    string? Bio,
    string? JobTitle,
    PersonalMbtiTypes? MbtiTypes,
    MoodWidgetDto? MoodWidget,
    IReadOnlyList<SocialLinkDto> SocialLinks,
    IReadOnlyList<string> Interests,
    bool IsFaceVerified,
    bool IsNationalIdVerified,
    bool IsSearchable,
    bool ShowContacts,
    bool ShowLastSeen,
    DateTime? LastSeenAtUtc,
    int TotalXp,
    int CurrentLevel,
    int NextLevelXp,
    decimal PublicScore,
    int RatingsCount,
    DateTime CreatedAtUtc);