using Ekip.Domain.Errors.Location;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Results;
using Ekip.Domain.UnitTests.Helpers;
using Ekip.Domain.ValueObjects.Ekip;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.UnitTests.ValueObjects;

public sealed class IdentityAndLocationValueObjectTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Username_Create_Should_Trim_And_Lowercase_Valid_Input()
    {
        var result = Username.Create("  Ali_Reza.99  ");

        var username = ResultAssert.Success(result);
        Assert.Equal("ali_reza.99", username.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("ali@reza")]
    [InlineData("ali-reza")]
    public void Username_Create_Should_Fail_For_Invalid_Input(string? raw)
    {
        var result = Username.Create(raw);

        ResultAssert.Failure(result, UserErrors.InvalidUsername);
    }

    [Fact]
    public void Email_Create_Should_Normalize_Valid_Email()
    {
        var result = Email.Create("  User.Name@Example.COM  ");

        var email = ResultAssert.Success(result);
        Assert.Equal("user.name@example.com", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("user@localhost")]
    [InlineData("Ali <user@example.com>")]
    public void Email_Create_Should_Fail_For_Invalid_Email(string? raw)
    {
        var result = Email.Create(raw);

        ResultAssert.Failure(result, UserErrors.InvalidEmail);
    }

    [Fact]
    public void PhoneNumber_Create_Should_Strip_Separators_And_Keep_Plus()
    {
        var result = PhoneNumber.Create(" +98 912-345 (6789) ");

        var phone = ResultAssert.Success(result);
        Assert.Equal("+989123456789", phone.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("12345")]
    [InlineData("+98abc12345")]
    public void PhoneNumber_Create_Should_Fail_For_Invalid_Input(string? raw)
    {
        var result = PhoneNumber.Create(raw);

        ResultAssert.Failure(result, UserErrors.InvalidPhoneNumber);
    }

    [Fact]
    public void OAuthProviderLogin_Create_Should_Allow_Null_Email_And_Normalize_Provider()
    {
        var result = OAuthProviderLogin.Create(" Google ", "sub-123", null, UtcNow);

        var oauth = ResultAssert.Success(result);
        Assert.Equal("google", oauth.Provider);
        Assert.Equal("sub-123", oauth.ProviderUserId);
        Assert.Null(oauth.Email);
    }

    [Fact]
    public void MoodWidget_Create_Should_Fail_When_Both_Emoji_And_Description_Are_Empty()
    {
        var result = MoodWidget.Create("  ", null, "#FFFFFF", UtcNow);

        ResultAssert.Failure(result, UserErrors.InvalidMoodWidget);
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://example.com")]
    [InlineData("not-a-url")]
    public void SocialLink_Create_Should_Reject_NonHttp_Or_Invalid_Urls(string url)
    {
        var result = SocialLink.Create("instagram", url, UtcNow);

        ResultAssert.Failure(result, UserErrors.InvalidSocialLink);
    }

    [Fact]
    public void GeoLocation_Create_Should_Aggregate_Errors_When_Both_Coordinates_Are_Invalid()
    {
        var result = GeoLocation.Create(95.0, -200.0);

        Assert.True(result.IsFailure);
        var validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.Equal(2, validationError.Errors.Length);
        Assert.Contains(LocationErrors.InvalidLatitude, validationError.Errors);
        Assert.Contains(LocationErrors.InvalidLongitude, validationError.Errors);
    }

    [Fact]
    public void GeoLocation_CreateJitteredPoint_Should_Offset_Within_Expected_Range()
    {
        var center = ResultAssert.Success(GeoLocation.Create(35.6892, 51.3890));

        var jittered = center.CreateJitteredPoint(300, 500);
        var distance = center.DistanceInMetersTo(jittered);

        Assert.InRange(distance, 295.0, 505.0);
    }
}