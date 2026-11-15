using System.Text.Json;
using Dapper;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Queries.User;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Policies;

namespace Ekip.Persistence.Queries.User;

internal sealed class UserQueries(ISqlConnectionFactory connectionFactory) : IUserQueries
{
    private static readonly int DefaultNextLevelXp = ReputationPolicy.GetNextLevelXp(1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string GetProfileByIdSql = """
        SELECT
            u.id AS UserId,
            u.username AS Username,
            p.first_name AS FirstName,
            p.last_name AS LastName,
            p.user_gender AS UserGender,
            p.birth_date AS BirthDate,
            p.city_id AS CityId,
            p.avatar_asset_id AS AvatarAssetId,
            p.banner_asset_id AS BannerAssetId,
            p.bio AS Bio,
            p.job_title AS JobTitle,
            p.mbti_types AS MbtiTypes,
            p.mood_widget AS MoodWidgetJson,
            p.social_links AS SocialLinksJson,
            p.interests AS Interests,
            u.is_face_verified AS IsFaceVerified,
            u.is_national_id_verified AS IsNationalIdVerified,
            p.is_searchable AS IsSearchable,
            p.show_contacts AS ShowContacts,
            p.show_last_seen AS ShowLastSeen,
            p.last_seen_at_utc AS LastSeenAtUtc,
            COALESCE(g.total_xp, 0) AS TotalXp,
            COALESCE(g.current_level, 1) AS CurrentLevel,
            COALESCE(g.next_level_xp, @DefaultNextLevelXp) AS NextLevelXp,
            COALESCE(g.public_score, 0) AS PublicScore,
            COALESCE(g.ratings_count, 0) AS RatingsCount,
            p.created_at_utc AS CreatedAtUtc
        FROM user_account u
        INNER JOIN user_profile p ON p.user_id = u.id
        LEFT JOIN user_gamification_summary g ON g.user_id = u.id
        WHERE u.id = @UserId AND u.is_deleted = false;
        """;

    public async Task<UserProfileDto?> GetProfileByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            GetProfileByIdSql,
            new { UserId = userId, DefaultNextLevelXp },
            cancellationToken: cancellationToken);

        var row = await connection.QuerySingleOrDefaultAsync<UserProfileRow>(command);
        if (row is null)
            return null;

        var moodWidget = string.IsNullOrWhiteSpace(row.MoodWidgetJson)
            ? null
            : JsonSerializer.Deserialize<MoodWidgetDto>(row.MoodWidgetJson, JsonOptions);

        var socialLinks = string.IsNullOrWhiteSpace(row.SocialLinksJson)
            ? []
            : JsonSerializer.Deserialize<List<SocialLinkDto>>(row.SocialLinksJson, JsonOptions) ?? [];

        var birthDate = row.BirthDate switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => DateOnly.MinValue
        };

        return new UserProfileDto(
            row.UserId,
            row.Username,
            row.FirstName,
            row.LastName,
            (Gender)row.UserGender,
            birthDate,
            row.CityId,
            row.AvatarAssetId,
            row.BannerAssetId,
            row.Bio,
            row.JobTitle,
            row.MbtiTypes.HasValue ? (PersonalMbtiTypes)row.MbtiTypes.Value : null,
            moodWidget,
            socialLinks,
            row.Interests ?? [],
            row.IsFaceVerified,
            row.IsNationalIdVerified,
            row.IsSearchable,
            row.ShowContacts,
            row.ShowLastSeen,
            row.LastSeenAtUtc,
            row.TotalXp,
            row.CurrentLevel,
            row.NextLevelXp,
            row.PublicScore,
            row.RatingsCount,
            row.CreatedAtUtc);
    }

    private sealed class UserProfileRow
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public int UserGender { get; init; }
        public object BirthDate { get; init; } = null!;
        public Guid? CityId { get; init; }
        public Guid? AvatarAssetId { get; init; }
        public Guid? BannerAssetId { get; init; }
        public string? Bio { get; init; }
        public string? JobTitle { get; init; }
        public int? MbtiTypes { get; init; }
        public string? MoodWidgetJson { get; init; }
        public string? SocialLinksJson { get; init; }
        public string[]? Interests { get; init; }
        public bool IsFaceVerified { get; init; }
        public bool IsNationalIdVerified { get; init; }
        public bool IsSearchable { get; init; }
        public bool ShowContacts { get; init; }
        public bool ShowLastSeen { get; init; }
        public DateTime? LastSeenAtUtc { get; init; }
        public int TotalXp { get; init; }
        public int CurrentLevel { get; init; }
        public int NextLevelXp { get; init; }
        public decimal PublicScore { get; init; }
        public int RatingsCount { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}