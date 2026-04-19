using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Application.DTOs.Request
{
    public class NewRequestDto
    {
        public Guid RequestRef { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid Creator { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredAssignments { get; set; }
        public string[]? Tags { get; set; }
        public DateTime RequestCreateDateTime { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public RequestFilterDto[]? RequestFilters { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
    }
}
