using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Entities.User;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.Factories;

public static class UserRegistrationFactory
{
    public static Result<UserRegistrationBundle> CreateWithPassword(
        Username username,
        string referralCode,
        string passwordHash,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly birthDate,
        DateTime utcNow,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        Guid? cityId = null,
        Guid? referredById = null)
    {
        var accountResult = UserAccount.Register(
            username: username,
            referralCode: referralCode,
            utcNow: utcNow,
            email: email,
            phoneNumber: phoneNumber,
            passwordHash: passwordHash,
            initialOAuthProvider: null,
            referredById: referredById);

        if (accountResult.IsFailure)
            return accountResult.Error;

        return CompleteBundle(accountResult.Value, firstName, lastName, gender, birthDate, utcNow, cityId);
    }

    public static Result<UserRegistrationBundle> CreateWithOAuth(
        Username username,
        string referralCode,
        OAuthProviderLogin initialOAuthProvider,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly birthDate,
        DateTime utcNow,
        string? passwordHash = null,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        Guid? cityId = null,
        Guid? referredById = null)
    {
        ArgumentNullException.ThrowIfNull(initialOAuthProvider);

        var accountResult = UserAccount.Register(
            username: username,
            referralCode: referralCode,
            utcNow: utcNow,
            email: email,
            phoneNumber: phoneNumber,
            passwordHash: passwordHash,
            initialOAuthProvider: initialOAuthProvider,
            referredById: referredById);

        if (accountResult.IsFailure)
            return accountResult.Error;

        return CompleteBundle(accountResult.Value, firstName, lastName, gender, birthDate, utcNow, cityId);
    }

    public static Result<UserRegistrationBundle> Create(
        Username username,
        string referralCode,
        string passwordHash,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly birthDate,
        DateTime utcNow,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        Guid? cityId = null,
        Guid? referredById = null)
        => CreateWithPassword(
            username,
            referralCode,
            passwordHash,
            firstName,
            lastName,
            gender,
            birthDate,
            utcNow,
            email,
            phoneNumber,
            cityId,
            referredById);

    private static Result<UserRegistrationBundle> CompleteBundle(
        UserAccount account,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly birthDate,
        DateTime utcNow,
        Guid? cityId)
    {
        var profileResult = UserProfile.Create(
            userId: account.Id,
            firstName: firstName,
            lastName: lastName,
            gender: gender,
            birthDate: birthDate,
            utcNow: utcNow,
            cityId: cityId);

        if (profileResult.IsFailure)
            return profileResult.Error;

        var summary = UserGamificationSummary.Create(account.Id);

        return new UserRegistrationBundle(
            account,
            profileResult.Value,
            summary);
    }
}