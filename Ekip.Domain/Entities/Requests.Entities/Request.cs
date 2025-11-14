    using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Enums;

namespace Ekip.Domain.Entities.Requests.Entities
{
    /// <summary>
    /// درخواست اکیپ یابی
    /// </summary>
    public class Request : BaseEntitiy
    {
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public Profile Creator { get; private set; }
        public RequestStatus Status { get; private set; }
        public bool IsValid { get; private set; }
        public int RequiredAssignments { get; private set; }
        public int? MaximumRequiredAssigments { get; private set; }
        public int? MaximumAgeRequired { get; set; }
        public int? MinimumAgeRequired { get; set; }
        public string? Tags { get; private set; }
        public bool IsAutoAccept { get; private set; }
        public bool IsRepeatable { get; private set; }
        public RequestRepeatType? RepeatType { get; set; }
        public RequestType RequestType { get; private set; }
        public ApplicantType ApplicantType { get; set; }
        public DateTime RequestDateTime { get; private set; }
        public DateTime RequestForbidDateTime => RequestDateTime.AddHours(-12);

        private readonly List<JoinRequest> _joinRequests = new();
        public IReadOnlyCollection<JoinRequest> JoinRequests => _joinRequests.AsReadOnly();

        private readonly List<RequestAssignment> _assignments = new();
        public IReadOnlyCollection<RequestAssignment> Assignments => _assignments.AsReadOnly();

        public Request(Profile creator , string title , int requiredAssignment,DateTime requestDateTime,string? description, string? tags,RequestType requestType,ApplicantType applicantType,bool isAutoAccept) 
        {
            if (creator == null)
                throw new Exception("a Request Must Have an Creator");
            if (title == null)
                throw new Exception("a Request Must Have a Name");
            if (requiredAssignment < 1)
                throw new Exception("a Request cannot Create with 0 Person");

            Creator = creator;
            Title = title;
            Status = RequestStatus.Open;
            RequiredAssignments = requiredAssignment;
            RequestDateTime = requestDateTime;
            Description = description;
            Tags = tags;
            RequestType = requestType;
            ApplicantType = applicantType;
            IsAutoAccept = isAutoAccept;
        }

        public void AddJoinRequest(Profile applicant,string? description)
        {
            if(this.IsRequestOpenToNewApplicant())
                throw new Exception("cannot Add an Applicant to a Request that is Not Open");
            if (_joinRequests.Any(jr => jr.Applicant.Id == applicant.Id))
                throw new Exception("this User already Sent a Join Request");
            var newJoinRequest = new JoinRequest(this, applicant, description);
            _joinRequests.Add(newJoinRequest);
        }

        public void AcceptApplicant(Profile owner , JoinRequest joinRequestToAccept)
        {

            if (owner.Id != Creator.Id)
                throw new Exception("only the Owner Can Accept the Request");

            joinRequestToAccept.Accept();

            var assignment = new RequestAssignment(this , joinRequestToAccept.Applicant);

            _assignments.Add(assignment);

            Status = RequestStatus.InProgress;

            CheckForCompletion();

        }
        public void RejectApplicant(Profile owner , JoinRequest joinRequestToReject)
        {
            if (owner.Id != Creator.Id)
                throw new Exception("only the Owner Can Reject the Request");
            if (joinRequestToReject.Request.Id != Id)
                throw new Exception("the Join Request dos not Belong to this Request");
            joinRequestToReject.Decline();
        }

        /// <summary>
        /// اینکه ایا عضو جدیدی قبول میتونه بکنه یا نه؟
        /// </summary>
        /// <returns>بول</returns>
        public bool IsRequestOpenToNewApplicant()
        {
            var hasSpace = RequiredAssignments < MaximumRequiredAssigments || MaximumRequiredAssigments == null;
            var validDateTime = DateTime.UtcNow < RequestForbidDateTime;
            var validState = Status == RequestStatus.Open || Status == RequestStatus.InProgress;

            return  hasSpace && validDateTime && validState;
        }

        /// <summary>
        /// عوض کردن قابل قبول بودن
        /// </summary>
        /// <param name="newValue"></param>
        public void ChangeValidity(bool newValue)
        {
            IsValid = newValue;
        }

        public void CheckForCompletion()
        {
            if(_assignments.Count >= RequiredAssignments)
                Status = RequestStatus.Completed;
        }

        private Request() { }
    }
}
