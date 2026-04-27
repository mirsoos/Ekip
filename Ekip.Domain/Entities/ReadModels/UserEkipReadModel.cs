using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.ReadModels
{
    public class UserEkipReadModel
    {
        public Guid RequestRef { get; set; }
        public bool IsDeleted { get; set; }
        public string? Description { get; set; }
        public Guid CreatorRef { get; set; }
        public string? CreatorAvatar { get; set; }
        public string EkipTitle { get; set; }
        public string CreatorName { get; set; }
        public RequestStatus Status { get; set; }
        public RequestType RequestType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public int RequiredMembers { get; set; }
        public string[]? Tags { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public int CurrentMembersCount { get; set; }
        public int? MaximumRequiredMembers { get; set; }
        public List<PendingAssignmentInfo>? PendingAssignments { get; set; }
        public List<EkipMember> AcceptedMembers { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? RequiredLevel { get; set; }
        public double? MinimumScore { get; set; }
        public TargetGender TargetGender { get; set; }
        public int? MaximumAge { get; set; }
        public int? MinimumAge { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
    }
}
