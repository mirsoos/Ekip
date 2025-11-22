using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums.Requests.Enums;
using MediatR;

namespace Ekip.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestCommand : IRequest<NewRequestDto>
    {
        public long ProfileRef { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredMembers { get; set; }
        public string[]? Tags { get; set; }
        public DateTime RequestDateTime { get; set; }
        public RequestFilterDto[]? RequestFilters { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatble { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
    }
}
