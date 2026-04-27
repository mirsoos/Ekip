using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Application.Contracts.Events
{
    public record UserEkipCreatedUpdaterEvent
    {
        public Guid RequestRef { get; init; }
        public bool IsDeleted { get; init; }
        public string? Description { get; init; }
        public Guid CreatorRef { get; init; }
        public string? CreatorAvatar { get; init; }
        public string EkipTitle { get; init; }
        public string CreatorName { get; init; }
        public RequestStatus Status { get; init; }
        public RequestType RequestType { get; init; }
        public DateTime CreateDate { get; init; }
        public DateTime RequestDateTime { get; init; }
        public DateTime RequestForbidDateTime { get; init; }
        public int RequiredMembers { get; init; }
        public string[]? Tags { get; init; }
        public MemberType MemberType { get; init; }
        public bool IsAutoAccept { get; init; }
        public int CurrentMembersCount { get; init; }
        public int? MaximumRequiredMembers { get; init; }
        public List<PendingAssignmentInfo>? PendingAssignments { get; init; }
        public List<EkipMember> AcceptedMembers { get; init; }
        public DateTime LastUpdated { get; init; }
        public int? RequiredLevel { get; init; }
        public double? MinimumScore { get; init; }
        public TargetGender TargetGender { get; init; }
        public int? MaximumAge { get; init; }
        public int? MinimumAge { get; init; }
        public bool IsRepeatable { get; init; }
        public RequestRepeatType? RepeatType { get; init; }
    }
}
