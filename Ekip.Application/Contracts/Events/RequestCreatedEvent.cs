using Ekip.Application.DTOs.Request;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record RequestCreatedEvent
    {
        public long RequestRef { get; init; }
        public long CreatorRef { get; init; }
        public string Title { get; init; }
        public int RequiredMembers { get; init; }
        public int? MaximumRequiredAssignmnets { get; init; }
        public DateTime RequestCreateDateTime { get; init; }
        public DateTime RequestDateTime { get; init; }
        public DateTime RequestForbidDateTime { get; init; }
        public string Description { get; init; }
        public string[]? Tags { get; init; }
        public RequestType RequestType { get; init; }
        public MemberType MemberType { get; init; }
        public bool IsAutoAccept { get; init; }
        public RequestFilterDto[]? RequestFilters { get; init; }
        public bool IsRepeatable { get; init; }
        public RequestRepeatType? RepeatType { get; init; }
    }
}
