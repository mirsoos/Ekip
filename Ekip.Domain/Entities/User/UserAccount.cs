using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Events.User;
using Ekip.Domain.Extensions;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.Entities.User;

public class UserAccount : BaseEntity, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public Username Username { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Email? Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserAccountStatus AccountStatus { get; private set; } = UserAccountStatus.Active;
    public bool TwoFactorEnabled { get; private set; }
    public bool IsFaceVerified { get; private set; }
    public bool IsNationalIdVerified { get; private set; }
    public string? NationalIdHash { get; private set; }
    public string ReferralCode { get; private set; }
    public Guid? ReferredById { get; private set; }
    public bool HasCompletedTour { get; private set; }
    public DateTime TermsAcceptedAt { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private readonly List<OAuthProviderLogin> _oauthProviders = [];
    public IReadOnlyCollection<OAuthProviderLogin> OAuthProviders => _oauthProviders.AsReadOnly();

    private UserAccount()
    {
        Username = null!;
        ReferralCode = null!;
    }

    private UserAccount(
        Username username,
        PhoneNumber? phoneNumber,
        Email? email,
        string? passwordHash,
        string referralCode,
        Guid? referredById,
        DateTime termsAcceptedAt)
    {
        Username = username;
        PhoneNumber = phoneNumber;
        Email = email;
        PasswordHash = passwordHash;
        ReferralCode = referralCode;
        ReferredById = referredById;
        TermsAcceptedAt = termsAcceptedAt;
    }

    public static Result<UserAccount> Register(
        Username username,
        string referralCode,
        DateTime utcNow,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        string? passwordHash = null,
        OAuthProviderLogin? initialOAuthProvider = null,
        Guid? referredById = null)
    {
        ArgumentNullException.ThrowIfNull(username);

        if (string.IsNullOrWhiteSpace(referralCode))
            return UserErrors.InvalidReferralCode;

        if (!referralCode.TryNormalizeCode(4, 20, out var normalizedReferral))
            return UserErrors.InvalidReferralCode;

        if (referredById == Guid.Empty) 
            throw new ArgumentException("ReferredById cannot be empty when provided.", nameof(referredById));

        var normalizedPasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? null : passwordHash.Trim();
        if (normalizedPasswordHash is not null && normalizedPasswordHash.Length > 512)
            return UserErrors.InvalidPasswordHash;

        if (initialOAuthProvider is null)
        {
            if (normalizedPasswordHash is null)
                return UserErrors.MissingCredentialsOrOAuth;

            if (email is null && phoneNumber is null)
                return UserErrors.MissingContactMethod;
        }

        var account = new UserAccount(
            username,
            phoneNumber,
            email,
            normalizedPasswordHash,
            normalizedReferral,
            referredById,
            utcNow);

        if (referredById == account.Id)
            return UserErrors.SelfReferralNotAllowed;

        if (initialOAuthProvider is not null)
            account._oauthProviders.Add(initialOAuthProvider);

        account.AddDomainEvent(new UserRegisteredDomainEvent(account.Id, account.ReferredById, utcNow));
        return account;
    }

    public Result UpdateUsername(Username username)
    {
        ArgumentNullException.ThrowIfNull(username);

        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        Username = username;
        return Result.Success();
    }

    public Result SetPasswordHash(string passwordHash)
    {
        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        if (string.IsNullOrWhiteSpace(passwordHash) || passwordHash.Length > 512)
            return UserErrors.InvalidPasswordHash;

        PasswordHash = passwordHash;
        return Result.Success();
    }

    public Result ChangeEmail(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        Email = email;
        return Result.Success();
    }

    public Result ChangePhoneNumber(PhoneNumber phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);

        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        PhoneNumber = phoneNumber;
        return Result.Success();
    }

    public Result LinkOAuthProvider(OAuthProviderLogin provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        if (_oauthProviders.Any(p => p.Provider == provider.Provider))
            return UserErrors.OAuthProviderAlreadyLinked;

        _oauthProviders.Add(provider);
        return Result.Success();
    }

    public Result UnlinkOAuthProvider(string provider)
    {
        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        if (string.IsNullOrWhiteSpace(provider))
            return UserErrors.InvalidOAuthProvider;

        var normalized = provider.Trim().ToLowerInvariant();
        var existing = _oauthProviders.FirstOrDefault(p => p.Provider == normalized);
        if (existing is null)
            return UserErrors.OAuthProviderNotFound;

        if (PasswordHash is null && _oauthProviders.Count <= 1)
            return UserErrors.CannotUnlinkLastLoginMethod;

        _oauthProviders.Remove(existing);
        return Result.Success();
    }

    public Result SetTwoFactor(bool enabled)
    {
        if (AccountStatus != UserAccountStatus.Active)
            return UserErrors.AccountNotActive;

        TwoFactorEnabled = enabled;
        return Result.Success();
    }

    public Result MarkFaceVerified(DateTime utcNow)
    {
        if (AccountStatus == UserAccountStatus.Banned)
            return UserErrors.AlreadyBanned;

        if (IsFaceVerified)
            return Result.Success();

        IsFaceVerified = true;
        AddDomainEvent(new UserFaceVerifiedDomainEvent(Id, utcNow));
        return Result.Success();
    }

    public Result VerifyNationalId(string nationalIdHash, DateTime utcNow)
    {
        if (AccountStatus == UserAccountStatus.Banned)
            return UserErrors.AlreadyBanned;

        if (IsNationalIdVerified)
            return UserErrors.NationalIdAlreadyVerified;

        if (string.IsNullOrWhiteSpace(nationalIdHash) || nationalIdHash.Length > 128)
            return UserErrors.InvalidNationalIdHash;

        NationalIdHash = nationalIdHash.Trim();
        if (!IsNationalIdVerified)
        {
            IsNationalIdVerified = true;
            AddDomainEvent(new UserNationalIdVerifiedDomainEvent(Id, utcNow));
        }

        return Result.Success();
    }

    public void CompleteTour() => HasCompletedTour = true;

    public Result ChangeStatus(UserAccountStatus newStatus, DateTime utcNow)
    {
        if (AccountStatus == UserAccountStatus.Banned && newStatus == UserAccountStatus.Banned)
            return UserErrors.AlreadyBanned;

        if (AccountStatus == newStatus)
            return Result.Success();

        AccountStatus = newStatus;
        AddDomainEvent(new UserAccountStatusChangedDomainEvent(Id, newStatus, utcNow));
        return Result.Success();
    }
}