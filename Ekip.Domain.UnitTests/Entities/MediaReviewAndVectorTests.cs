using Ekip.Domain.Entities.AI;
using Ekip.Domain.Entities.Media;
using Ekip.Domain.Entities.Review;
using Ekip.Domain.Enums.Media;
using Ekip.Domain.Enums.Review;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.AI;
using Ekip.Domain.Errors.Media;
using Ekip.Domain.Errors.Review;
using Ekip.Domain.Events.Media;
using Ekip.Domain.Events.Review;
using Ekip.Domain.UnitTests.Helpers;

namespace Ekip.Domain.UnitTests.Entities;

public sealed class MediaReviewAndVectorTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(0.05, MediaModerationStatus.Approved)]
    [InlineData(0.50, MediaModerationStatus.EscalatedToAdmin)]
    [InlineData(0.90, MediaModerationStatus.Rejected)]
    public void MediaAsset_ApplyAiScanResult_Should_Route_To_Expected_Status(
        decimal probability,
        MediaModerationStatus expectedStatus)
    {
        var asset = ResultAssert.Success(MediaAsset.Create(
            Guid.CreateVersion7(),
            MediaUsageContext.EkipCover,
            "ekips/covers/1.jpg",
            UtcNow));

        ResultAssert.Success(asset.ApplyAiScanResult(probability, 0.20m, 0.80m, UtcNow));

        Assert.Equal(expectedStatus, asset.ModerationStatus);
        var ev = ResultAssert.HasDomainEvent<MediaModerationCompletedDomainEvent>(asset);
        Assert.Equal(expectedStatus, ev.Status);
    }

    [Fact]
    public void MediaAsset_ApplyAiScanResult_Twice_Should_Fail()
    {
        var asset = ResultAssert.Success(MediaAsset.Create(
            Guid.CreateVersion7(),
            MediaUsageContext.EkipCover,
            "ekips/covers/1.jpg",
            UtcNow));
        ResultAssert.Success(asset.ApplyAiScanResult(0.10m, 0.20m, 0.80m, UtcNow));

        var secondResult = asset.ApplyAiScanResult(0.10m, 0.20m, 0.80m, UtcNow);

        ResultAssert.Failure(secondResult, MediaErrors.InvalidModerationTransition);
    }

    [Fact]
    public void MediaAsset_MarkAiScanFailed_And_AdminOverride_Should_Work()
    {
        var asset = ResultAssert.Success(MediaAsset.Create(
            Guid.CreateVersion7(),
            MediaUsageContext.EkipCover,
            "ekips/covers/1.jpg",
            UtcNow));

        ResultAssert.Success(asset.MarkAiScanFailed("AI timeout", UtcNow));
        Assert.Equal(MediaModerationStatus.EscalatedToAdmin, asset.ModerationStatus);
        Assert.Equal("AI timeout", asset.Error);

        ResultAssert.Success(asset.ModerateByAdmin(true, null, UtcNow));
        Assert.Equal(MediaModerationStatus.Approved, asset.ModerationStatus);
        Assert.Null(asset.Error);
    }

    [Fact]
    public void ProfileReview_Without_Comment_Should_AutoApprove_Without_AiScan()
    {
        var result = ProfileReview.Submit(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            5,
            "   ",
            Guid.CreateVersion7(),
            UtcNow);

        var review = ResultAssert.Success(result);
        Assert.Equal(ProfileReviewModerationStatus.Approved, review.ModerationStatus);

        var ev = ResultAssert.HasDomainEvent<ProfileReviewSubmittedDomainEvent>(review);
        Assert.False(ev.RequiresAiScan);
    }

    [Fact]
    public void ProfileReview_With_Comment_Should_Moderate_And_Support_Admin_Override()
    {
        var review = ResultAssert.Success(ProfileReview.Submit(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            4,
            "Great organizer!",
            Guid.CreateVersion7(),
            UtcNow));

        Assert.Equal(ProfileReviewModerationStatus.PendingByAI, review.ModerationStatus);

        ResultAssert.Success(review.ApplyAiModerationResult(0.05m, 0.20m, 0.70m, UtcNow));
        Assert.Equal(ProfileReviewModerationStatus.Approved, review.ModerationStatus);

        review.ClearDomainEvents();
        ResultAssert.Success(review.ModerateByAdmin(false, UtcNow));
        Assert.Equal(ProfileReviewModerationStatus.Rejected, review.ModerationStatus);

        var ev = ResultAssert.HasDomainEvent<ProfileReviewModerationCompletedDomainEvent>(review);
        Assert.Equal(ProfileReviewModerationStatus.Approved, ev.OldStatus);
        Assert.Equal(ProfileReviewModerationStatus.Rejected, ev.NewStatus);
    }

    [Fact]
    public void ProfileReview_Submit_Should_Reject_SelfReview()
    {
        var userId = Guid.CreateVersion7();

        var result = ProfileReview.Submit(userId, userId, Guid.CreateVersion7(), 5, null, Guid.CreateVersion7(), UtcNow);

        ResultAssert.Failure(result, ReviewErrors.SelfReviewNotAllowed);
    }

    [Fact]
    public void UserIdentityVector_Create_Should_Validate_Dimensions_Values_And_Clone_Array()
    {
        var wrongLength = new float[128];
        ResultAssert.Failure(
            UserIdentityVector.Create(Guid.CreateVersion7(), wrongLength, Gender.Male, 1998, UtcNow),
            AiErrors.InvalidVectorDimensions);

        var nanVector = new float[UserIdentityVector.VectorDimensions];
        nanVector[0] = float.NaN;
        ResultAssert.Failure(
            UserIdentityVector.Create(Guid.CreateVersion7(), nanVector, Gender.Male, 1998, UtcNow),
            AiErrors.InvalidVectorValues);

        var validVector = new float[UserIdentityVector.VectorDimensions];
        validVector[0] = 0.42f;
        var created = ResultAssert.Success(
            UserIdentityVector.Create(Guid.CreateVersion7(), validVector, Gender.Female, 1998, UtcNow));

        validVector[0] = 0.99f;
        Assert.Equal(0.42f, created.PersonalityVector[0]);
    }
}