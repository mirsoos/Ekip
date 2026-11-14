using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.Errors.User;
using Ekip.Domain.Events.User;
using Ekip.Domain.Results;

namespace Ekip.Domain.Entities.User;

public class UserRelation : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public Guid RequesterId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public UserRelationStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private UserRelation() { }

    private UserRelation(Guid requesterId, Guid receiverId, UserRelationStatus status)
    {
        RequesterId = requesterId;
        ReceiverId = receiverId;
        Status = status;
    }

    public static Result<UserRelation> SendRequest(Guid requesterId, Guid receiverId, DateTime utcNow)
    {
        if (requesterId == Guid.Empty)
            throw new ArgumentException("RequesterId cannot be empty.", nameof(requesterId));
        if (receiverId == Guid.Empty)
            throw new ArgumentException("ReceiverId cannot be empty.", nameof(receiverId));

        if (requesterId == receiverId)
            return UserErrors.SelfRelationNotAllowed;

        var relation = new UserRelation(requesterId, receiverId, UserRelationStatus.Pending);
        relation.AddDomainEvent(new UserRelationChangedDomainEvent(relation.Id, requesterId, receiverId, relation.Status, utcNow));
        return relation;
    }

    public static Result<UserRelation> CreateBlocked(Guid blockerId, Guid blockedId, DateTime utcNow)
    {
        if (blockerId == Guid.Empty)
            throw new ArgumentException("BlockerId cannot be empty.", nameof(blockerId));
        if (blockedId == Guid.Empty)
            throw new ArgumentException("BlockedId cannot be empty.", nameof(blockedId));

        if (blockerId == blockedId)
            return UserErrors.SelfRelationNotAllowed;

        var relation = new UserRelation(blockerId, blockedId, UserRelationStatus.Blocked);
        relation.AddDomainEvent(new UserRelationChangedDomainEvent(relation.Id, blockerId, blockedId, relation.Status, utcNow));
        return relation;
    }

    public Result Accept(Guid actorUserId, DateTime utcNow)
    {
        if (actorUserId != ReceiverId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status != UserRelationStatus.Pending)
            return UserErrors.InvalidRelationTransition;

        Status = UserRelationStatus.Accepted;
        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }

    public Result Reject(Guid actorUserId, DateTime utcNow)
    {
        if (actorUserId != ReceiverId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status != UserRelationStatus.Pending)
            return UserErrors.InvalidRelationTransition;

        Status = UserRelationStatus.Rejected;
        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }

    public Result Unfollow(Guid actorUserId, DateTime utcNow)
    {
        if (actorUserId != RequesterId && actorUserId != ReceiverId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status is not (UserRelationStatus.Pending or UserRelationStatus.Accepted))
            return UserErrors.InvalidRelationTransition;

        Status = UserRelationStatus.UnFollowed;
        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }

    public Result ReRequest(Guid actorUserId, DateTime utcNow)
    {
        if (actorUserId != RequesterId && actorUserId != ReceiverId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status is not (UserRelationStatus.Rejected or UserRelationStatus.UnFollowed))
            return UserErrors.InvalidRelationTransition;

        var targetId = actorUserId == RequesterId ? ReceiverId : RequesterId;
        RequesterId = actorUserId;
        ReceiverId = targetId;
        Status = UserRelationStatus.Pending;

        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }

    public Result Block(Guid blockerId, DateTime utcNow)
    {
        if (blockerId != RequesterId && blockerId != ReceiverId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status == UserRelationStatus.Blocked)
            return UserErrors.InvalidRelationTransition;

        var blockedId = blockerId == RequesterId ? ReceiverId : RequesterId;
        RequesterId = blockerId;
        ReceiverId = blockedId;
        Status = UserRelationStatus.Blocked;

        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }

    public Result Unblock(Guid actorUserId, DateTime utcNow)
    {
        if (actorUserId != RequesterId)
            return UserErrors.UnauthorizedRelationAction;

        if (Status != UserRelationStatus.Blocked)
            return UserErrors.InvalidRelationTransition;

        Status = UserRelationStatus.UnFollowed;
        AddDomainEvent(new UserRelationChangedDomainEvent(Id, RequesterId, ReceiverId, Status, utcNow));
        return Result.Success();
    }
}