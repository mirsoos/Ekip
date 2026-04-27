using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestCommand : IRequest<NewRequestDto>
    {
        public Guid ProfileRef { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredMembers { get; set; }
        public string[]? Tags { get; set; }
        public DateTime RequestDateTime { get; set; }
        public List<RequestFilter>? RequestFilters { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
        public TargetGender TargetGender { get; set; }
        public int? RequiredLevel { get; set; }
        public double? MinimumScore { get; set; }
    }
}
