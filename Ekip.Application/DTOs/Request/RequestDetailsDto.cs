using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class RequestDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RequestCreatorDto Creator { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredMembers { get; set; }
        public string[]? Tags { get; set; }
        public RequestStatus Status { get; set; }
        public List<AssignmentMemberDto> Members { get; set; }
        public DateTime RequestCreateDate { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public List<RequestFilterDto>? RequestFilters { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
        public DateTime? NextRepeatDate { get; set; }
        public bool CanAssignRequest { get; set; }
    }
}
