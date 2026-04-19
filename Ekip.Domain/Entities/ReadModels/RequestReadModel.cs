using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Requests.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekip.Domain.Entities.ReadModels
{
    public class RequestReadModel
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid CreatorRef { get; set; }
        public string Title { get; set; }
        public int RequiredMembers { get; set; }
        public int? MaximumRequiredAssignments { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime RequestForbidDateTime { get; set; }
        public string Description { get; set; }
        public string? Tags { get; set; }
        public RequestStatus Status { get; set; }
        public RequestType RequestType { get; set; }
        public MemberType MemberType { get; set; }
        public bool IsAutoAccept { get; set; }
        public string? RequestFilters { get; set; }
        public bool IsRepeatable { get; set; }
        public RequestRepeatType? RepeatType { get; set; }

        [ForeignKey("CreatorRef")]
        public virtual ProfileReadModel Creator { get; set; }

        public virtual ICollection<RequestAssignmentReadModel> Assignments { get; set; }
    }
}
