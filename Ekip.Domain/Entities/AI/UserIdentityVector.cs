using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.AI;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.Entities.AI;

public class UserIdentityVector : Entity, IAuditableEntity, IAggregateRoot
{
    public const int VectorDimensions = 1536;

    public Guid UserId { get; private set; }
    public float[] PersonalityVector { get; private set; } = [];
    public string? AiPersonaSummary { get; private set; }
    public Guid? CityId { get; private set; }
    public Gender UserGender { get; private set; }
    public int BirthYear { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalHonor { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private UserIdentityVector() { }

    private UserIdentityVector(
        Guid userId,
        float[] personalityVector,
        string? aiPersonaSummary,
        Guid? cityId,
        Gender userGender,
        int birthYear,
        int currentLevel,
        int totalHonor)
    {
        UserId = userId;
        PersonalityVector = personalityVector;
        AiPersonaSummary = aiPersonaSummary;
        CityId = cityId;
        UserGender = userGender;
        BirthYear = birthYear;
        CurrentLevel = currentLevel;
        TotalHonor = totalHonor;
    }

    public static Result<UserIdentityVector> Create(
        Guid userId,
        float[] personalityVector,
        Gender gender,
        int birthYear,
        DateTime utcNow,
        Guid? cityId = null,
        string? aiPersonaSummary = null,
        int currentLevel = 1,
        int totalHonor = 0)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        ArgumentNullException.ThrowIfNull(personalityVector);

        if (personalityVector.Length != VectorDimensions)
            return AiErrors.InvalidVectorDimensions;

        if (cityId == Guid.Empty)
            throw new ArgumentException("CityId cannot be empty when provided.", nameof(cityId));

        if (!AllFinite(personalityVector))
            return AiErrors.InvalidVectorValues;

        if (!Enum.IsDefined(gender))
            return UserErrors.InvalidGender;

        if (!IsValidBirthYear(birthYear, utcNow))
            return AiErrors.InvalidBirthYear;

        if (currentLevel < 1)
            return AiErrors.InvalidLevel;

        var normalizedSummary = string.IsNullOrWhiteSpace(aiPersonaSummary) ? null : aiPersonaSummary.Trim();
        if (normalizedSummary?.Length > 4000)
            return AiErrors.InvalidPersonaSummary;

        return new UserIdentityVector(
            userId,
            (float[])personalityVector.Clone(),
            normalizedSummary,
            cityId,
            gender,
            birthYear,
            currentLevel,
            totalHonor);
    }

    public Result UpdateVector(float[] personalityVector, string? aiPersonaSummary = null)
    {
        ArgumentNullException.ThrowIfNull(personalityVector);

        if (personalityVector.Length != VectorDimensions)
            return AiErrors.InvalidVectorDimensions;

        if (!AllFinite(personalityVector))
            return AiErrors.InvalidVectorValues;

        var normalizedSummary = string.IsNullOrWhiteSpace(aiPersonaSummary) ? null : aiPersonaSummary.Trim();
        if (normalizedSummary?.Length > 4000)
            return AiErrors.InvalidPersonaSummary;

        PersonalityVector = (float[])personalityVector.Clone();
        AiPersonaSummary = normalizedSummary;
        return Result.Success();
    }

    public Result SyncProfileMetadata(Guid? cityId, Gender gender, int birthYear, DateTime utcNow)
    {
        if (!Enum.IsDefined(gender))
            return UserErrors.InvalidGender;

        if (!IsValidBirthYear(birthYear, utcNow))
            return AiErrors.InvalidBirthYear;

        if (cityId == Guid.Empty)
            throw new ArgumentException("CityId cannot be empty when provided.", nameof(cityId));

        CityId = cityId;
        UserGender = gender;
        BirthYear = birthYear;
        return Result.Success();
    }

    public Result SyncReputation(int currentLevel, int totalHonor)
    {
        if (currentLevel < 1)
            return AiErrors.InvalidLevel;

        CurrentLevel = currentLevel;
        TotalHonor = totalHonor;
        return Result.Success();
    }

    private static bool AllFinite(float[] values)
    {
        foreach (var value in values)
        {
            if (!float.IsFinite(value))
                return false;
        }

        return true;
    }

    private static bool IsValidBirthYear(int birthYear, DateTime utcNow) =>
        birthYear >= utcNow.Year - 120 && birthYear <= utcNow.Year - 18;
}