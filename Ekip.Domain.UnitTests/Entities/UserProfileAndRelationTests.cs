using Ekip.Domain.Entities.User;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Events.User;
using Ekip.Domain.UnitTests.Helpers;

namespace Ekip.Domain.UnitTests.Entities;

public sealed class UserProfileAndRelationTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Profile_Create_Should_Fail_For_Underage_User()
    {
        var result = UserProfile.Create(
            Guid.CreateVersion7(),
            "Ali",
            "Rezaei",
            Gender.Male,
            new DateOnly(2010, 1, 1),
            UtcNow);

        ResultAssert.Failure(result, UserErrors.UnderageUser);
    }

    [Fact]
    public void Profile_UpdateDetails_Should_Emit_Event_Only_When_Vector_Fields_Change()
    {
        var profile = ResultAssert.Success(UserProfile.Create(
            Guid.CreateVersion7(),
            "Ali",
            "Rezaei",
            Gender.Male,
            new DateOnly(1998, 5, 10),
            UtcNow));

        ResultAssert.Success(profile.UpdateDetails("Ali2", "Rezaei2", null, null, null, null, UtcNow));
        Assert.Empty(profile.DomainEvents);

        ResultAssert.Success(profile.UpdateDetails("Ali2", "Rezaei2", "Hiking lover", null, null, PersonalMbtiTypes.INTJ, UtcNow));
        ResultAssert.HasDomainEvent<UserProfileUpdatedDomainEvent>(profile);
    }

    [Fact]
    public void Profile_ReplaceInterests_Should_Deduplicate_CaseInsensitively()
    {
        var profile = ResultAssert.Success(UserProfile.Create(
            Guid.CreateVersion7(),
            "Sara",
            "Mohammadi",
            Gender.Female,
            new DateOnly(1999, 3, 20),
            UtcNow));

        var result = profile.ReplaceInterests(["Music", "music", " Coffee "], UtcNow);

        ResultAssert.Success(result);
        Assert.Equal(2, profile.Interests.Count);
        ResultAssert.HasDomainEvent<UserProfileUpdatedDomainEvent>(profile);
    }

    [Fact]
    public void Relation_Accept_Should_Only_Allow_Receiver()
    {
        var requesterId = Guid.CreateVersion7();
        var receiverId = Guid.CreateVersion7();
        var relation = ResultAssert.Success(UserRelation.SendRequest(requesterId, receiverId, UtcNow));

        var wrongActorResult = relation.Accept(requesterId, UtcNow);
        ResultAssert.Failure(wrongActorResult, UserErrors.UnauthorizedRelationAction);

        ResultAssert.Success(relation.Accept(receiverId, UtcNow));
        Assert.Equal(UserRelationStatus.Accepted, relation.Status);
    }

    [Fact]
    public void Relation_ReRequest_After_Rejection_Should_Allow_Receiver_To_Initiate()
    {
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var relation = ResultAssert.Success(UserRelation.SendRequest(userA, userB, UtcNow));
        ResultAssert.Success(relation.Reject(userB, UtcNow));

        ResultAssert.Success(relation.ReRequest(userB, UtcNow));

        Assert.Equal(userB, relation.RequesterId);
        Assert.Equal(userA, relation.ReceiverId);
        Assert.Equal(UserRelationStatus.Pending, relation.Status);
    }
}