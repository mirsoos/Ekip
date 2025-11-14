
using Ekip.Application.DTOs.User;

namespace Ekip.Application.DTOs.Request
{
    public class RequestDetailsDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RequestCreatorDto Creator { get; set; }
        public int RequiredAssignments { get; set; }
        public int? MaximumRequiredAssignments { get; set; }
        public string[]? Tags { get; set; }
        public string Status { get; set; }
        public List<AssignmentMemberDto> Members { get; set; }
        public DateTime RequestCreateDate { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public int? MinimumLevelRequired { get; set; }
        public double? MinimumRateRequired { get; set; }
        public string RequestType { get; set; }
        public string ApplicantType { get; set; }
        public int? MinimumAgeRequired { get; set; }
        public int? MaximumAgeRequired { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatable { get; set; }
        public string RepeatType { get; set; }
        public DateTime? NextRepeatDate { get; set; }
        public bool CanAssignRequest { get; set; }
    }
}
