using Ekip.Application.DTOs.User;
using Ekip.Domain.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class NewRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ProfileDto Creator { get; set; }
        public int RequiredAssignments { get; set; }
        public int? MaximumRequiredAssignmnets { get; set; }
        public string? Tags { get; set; }
        public DateTime RequestCreateDateTime { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public int? MinimumRequiredLevel { get; set; }
        public int? MinimumRateRequired { get; set; }
        public RequestType RequestType { get; set; }
        public ApplicantType ApplicantType { get; set; }
        public int? MinimumAgeRequired { get; set; }
        public int? MaximumAgeRequired { get; set; }
        public bool IsAutoAccept { get; set; }
        public bool IsRepeatble { get; set; }
        public RequestRepeatType? RepeatType { get; set; }
    }
}
