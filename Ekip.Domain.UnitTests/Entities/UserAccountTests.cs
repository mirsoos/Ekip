using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Events.User;
using Ekip.Domain.UnitTests.Helpers;
using Ekip.Domain.ValueObjects.User;
using UserEntity = Ekip.Domain.Entities.User.UserAccount;

namespace Ekip.Domain.UnitTests.Entities;

public sealed class UserAccountTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Register_With_Email_And_Password_Should_Succeed_And_Emit_Event()
    {
        var username = ResultAssert.Success(Username.Create("Ali_Reza"));
        var email = ResultAssert.Success(Email.Create("ali@example.com"));

        var result = UserEntity.Register(
            username,
            "ref2026",
            UtcNow,
            email: email,
            passwordHash: "hashed-secret");

        var account = ResultAssert.Success(result);
        Assert.Equal("ali_reza", account.Username.Value);
        Assert.Equal("REF2026", account.ReferralCode);
        Assert.Equal(UserAccountStatus.Active, account.AccountStatus);

        var ev = ResultAssert.HasDomainEvent<UserRegisteredDomainEvent>(account);
        Assert.Equal(account.Id, ev.UserId);
    }

    [Fact]
    public void Register_Without_OAuth_And_Without_Password_Should_Fail()
    {
        var username = ResultAssert.Success(Username.Create("alireza"));
        var email = ResultAssert.Success(Email.Create("ali@example.com"));

        var result = UserEntity.Register(username, "REF123", UtcNow, email: email);

        ResultAssert.Failure(result, UserErrors.MissingCredentialsOrOAuth);
    }

    [Fact]
    public void Register_With_OAuth_Only_Should_Succeed_Without_Password()
    {
        var username = ResultAssert.Success(Username.Create("oauth_user"));
        var oauth = ResultAssert.Success(OAuthProviderLogin.Create("google", "g-1", null, UtcNow));

        var result = UserEntity.Register(username, "REF123", UtcNow, initialOAuthProvider: oauth);

        var account = ResultAssert.Success(result);
        Assert.Single(account.OAuthProviders);
        Assert.Null(account.PasswordHash);
    }

    [Fact]
    public void UnlinkOAuthProvider_Should_Fail_When_No_Password_And_Last_Provider()
    {
        var username = ResultAssert.Success(Username.Create("oauth_user"));
        var oauth = ResultAssert.Success(OAuthProviderLogin.Create("google", "g-1", null, UtcNow));
        var account = ResultAssert.Success(UserEntity.Register(username, "REF123", UtcNow, initialOAuthProvider: oauth));

        var result = account.UnlinkOAuthProvider("google");

        ResultAssert.Failure(result, UserErrors.CannotUnlinkLastLoginMethod);
    }

    [Fact]
    public void UpdateUsername_Should_Fail_When_Account_Is_Not_Active()
    {
        var username = ResultAssert.Success(Username.Create("alireza"));
        var email = ResultAssert.Success(Email.Create("ali@example.com"));
        var account = ResultAssert.Success(UserEntity.Register(username, "REF123", UtcNow, email: email, passwordHash: "hash"));

        ResultAssert.Success(account.ChangeStatus(UserAccountStatus.Suspended, UtcNow));

        var newUsername = ResultAssert.Success(Username.Create("new_name"));
        var result = account.UpdateUsername(newUsername);

        ResultAssert.Failure(result, UserErrors.AccountNotActive);
    }

    [Fact]
    public void MarkFaceVerified_Should_Be_Idempotent_And_Emit_Event_Once()
    {
        var username = ResultAssert.Success(Username.Create("alireza"));
        var email = ResultAssert.Success(Email.Create("ali@example.com"));
        var account = ResultAssert.Success(UserEntity.Register(username, "REF123", UtcNow, email: email, passwordHash: "hash"));
        account.ClearDomainEvents();

        ResultAssert.Success(account.MarkFaceVerified(UtcNow));
        ResultAssert.Success(account.MarkFaceVerified(UtcNow.AddMinutes(5)));

        Assert.True(account.IsFaceVerified);
        Assert.Single(account.DomainEvents.OfType<UserFaceVerifiedDomainEvent>());
    }
}