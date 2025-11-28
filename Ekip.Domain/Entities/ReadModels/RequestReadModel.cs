using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.Entities.ReadModels
{
    public class RequestReadModel : BaseEntitiy
    {
        public long CreatorRef { get; set; }
        public string Title { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredAssignmnets { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public string Description { get; set; }
        public string? Tags { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public string? RequestFilters { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
    }
}
