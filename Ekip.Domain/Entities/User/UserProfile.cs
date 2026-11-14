using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Events.User;
using Ekip.Domain.Extensions;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.Entities.User;

public class UserProfile : Entity, IAggregateRoot, IAuditableEntity
{
    private const int MinAge = 18;
    private const int MaxAge = 120;
    private const int MaxInterestsCount = 20;
    private const int MaxSocialLinksCount = 10;

    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Gender UserGender { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Guid? CityId { get; private set; }
    public Guid? AvatarAssetId { get; private set; }
    public Guid? BannerAssetId { get; private set; }
    public string? Bio { get; private set; }
    public string? JobTitle { get; private set; }
    public PersonalMbtiTypes? MbtiTypes { get; private set; }
    public MoodWidget? MoodWidget { get; private set; }
    public bool IsSearchable { get; private set; } = true;
    public bool ShowContacts { get; private set; } = true;
    public bool ShowLastSeen { get; private set; } = true;
    public DateTime? LastSeenAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private readonly List<SocialLink> _socialLinks = [];
    public IReadOnlyCollection<SocialLink> SocialLinks => _socialLinks.AsReadOnly();

    private readonly List<string> _interests = [];
    public IReadOnlyCollection<string> Interests => _interests.AsReadOnly();

    private UserProfile()
    {
        FirstName = null!;
        LastName = null!;
    }

    private UserProfile(
        Guid userId,
        string firstName,
        string lastName,
        Gender userGender,
        DateOnly birthDate,
        Guid? cityId)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        UserGender = userGender;
        BirthDate = birthDate;
        CityId = cityId;
    }

    public static Result<UserProfile> Create(
        Guid userId,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly birthDate,
        DateTime utcNow,
        Guid? cityId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(firstName) || firstName.Trim().Length > 100)
            return UserErrors.InvalidFirstName;

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Trim().Length > 100)
            return UserErrors.InvalidLastName;

        if (!Enum.IsDefined(gender))
            return UserErrors.InvalidGender;

        if (cityId == Guid.Empty) 
            throw new ArgumentException("CityId cannot be empty when provided.", nameof(cityId));

        var today = DateOnly.FromDateTime(utcNow);
        if (birthDate > today)
            return UserErrors.InvalidBirthDate;

        var age = birthDate.CalculateAge(today);
        if (birthDate > today.AddYears(-age))
            age--;

        if (age > MaxAge)
            return UserErrors.InvalidBirthDate;

        if (age < MinAge)
            return UserErrors.UnderageUser;

        return new UserProfile(
            userId,
            firstName.Trim(),
            lastName.Trim(),
            gender,
            birthDate,
            cityId);
    }

    public Result UpdateDetails(
        string firstName,
        string lastName,
        string? bio,
        string? jobTitle,
        Guid? cityId,
        PersonalMbtiTypes? mbtiTypes,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Trim().Length > 100)
            return UserErrors.InvalidFirstName;

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Trim().Length > 100)
            return UserErrors.InvalidLastName;

        if (cityId == Guid.Empty) 
            throw new ArgumentException("CityId cannot be empty when provided.", nameof(cityId));

        var normalizedBio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        if (normalizedBio?.Length > 1000)
            return UserErrors.InvalidBio;

        var normalizedJob = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle.Trim();
        if (normalizedJob?.Length > 150)
            return UserErrors.InvalidJobTitle;

        var vectorFieldsChanged =
            Bio != normalizedBio
            || JobTitle != normalizedJob
            || CityId != cityId
            || MbtiTypes != mbtiTypes;

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Bio = normalizedBio;
        JobTitle = normalizedJob;
        CityId = cityId;
        MbtiTypes = mbtiTypes;

        if (vectorFieldsChanged)
            AddDomainEvent(new UserProfileUpdatedDomainEvent(UserId, utcNow));

        return Result.Success();
    }

    public void UpdateMedia(Guid? avatarAssetId, Guid? bannerAssetId)
    {

        if (avatarAssetId == Guid.Empty)
            throw new ArgumentException("AvatarAssetId cannot be empty when provided.", nameof(avatarAssetId));
        if (bannerAssetId == Guid.Empty)
            throw new ArgumentException("BannerAssetId cannot be empty when provided.", nameof(bannerAssetId));

        AvatarAssetId = avatarAssetId;
        BannerAssetId = bannerAssetId;
    }

    public void SetMoodWidget(MoodWidget? moodWidget) => MoodWidget = moodWidget;

    public void UpdatePrivacy(bool isSearchable, bool showContacts, bool showLastSeen)
    {
        IsSearchable = isSearchable;
        ShowContacts = showContacts;
        ShowLastSeen = showLastSeen;
    }

    public void SyncLastSeen(DateTime lastSeenAtUtc)
    {
        if (LastSeenAtUtc is null || lastSeenAtUtc > LastSeenAtUtc)
            LastSeenAtUtc = lastSeenAtUtc;
    }

    public Result ReplaceInterests(IEnumerable<string> interests, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(interests);

        var normalized = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var raw in interests)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return UserErrors.InvalidInterest;

            var tag = raw.Trim();
            if (tag.Length > 100)
                return UserErrors.InvalidInterest;

            if (seen.Add(tag))
                normalized.Add(tag);
        }

        if (normalized.Count > MaxInterestsCount)
            return UserErrors.TooManyInterests;

        if (_interests.SequenceEqual(normalized, StringComparer.OrdinalIgnoreCase))
            return Result.Success();

        _interests.Clear();
        _interests.AddRange(normalized);
        AddDomainEvent(new UserProfileUpdatedDomainEvent(UserId, utcNow));
        return Result.Success();
    }

    public Result ReplaceSocialLinks(IEnumerable<SocialLink> links)
    {
        ArgumentNullException.ThrowIfNull(links);

        var distinctLinks = links
            .DistinctBy(l => l.Platform)
            .ToList();

        if (distinctLinks.Count > MaxSocialLinksCount)
            return UserErrors.TooManySocialLinks;

        _socialLinks.Clear();
        _socialLinks.AddRange(distinctLinks);
        return Result.Success();
    }
}