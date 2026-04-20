using Ekip.Domain.Base;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    public sealed class PendingAssignmentInfo : ValueObject
    {
        public Guid AssignmentRef { get; private set; }
        public Guid ApplicantId { get; private set; }
        public string ApplicantName { get; private set; }
        public string? ApplicantAvatar { get; private set; }
        public DateTime AppliedDate { get; private set; }
        public string? Description { get; private set; }
        public AssignmentStatus ApplicantStatus { get; private set; }

        public PendingAssignmentInfo(Guid assignmentRef , Guid applicantId , string applicantName , string applicantAvatar , DateTime appliedDate , string? description , AssignmentStatus assignmentStatus)
        {
            if(assignmentRef == Guid.Empty)
                throw new ArgumentException("assignmentRef cannot be Null" , nameof(assignmentRef));
            if (applicantId == Guid.Empty)
                throw new ArgumentException("applicantId cannot be Null" , nameof(applicantId));
            if (string.IsNullOrEmpty(applicantName))
                throw new ArgumentException("applicantName cannot be Null" , nameof(applicantName));

            AssignmentRef = assignmentRef;
            ApplicantId = applicantId;
            ApplicantName = applicantName;
            ApplicantAvatar = applicantAvatar;
            AppliedDate = appliedDate;
            Description = description;
            ApplicantStatus = assignmentStatus;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AssignmentRef;
        }
        private PendingAssignmentInfo() { }
    }
}
