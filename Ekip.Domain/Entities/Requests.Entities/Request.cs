using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

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
        public int RequiredMembers { get; private set; }
        public int? MaximumRequiredMembers { get; private set; }
        public int? MaximumRequiredAge { get; set; }
        public int? MinimumRequiredAge { get; set; }
        public string[]? Tags { get; private set; }
        public bool IsAutoAccept { get; private set; }
        public bool IsRepeatable { get; private set; }
        public RequestRepeatType? RepeatType { get; set; }
        public RequestType RequestType { get; private set; }
        public MemberType MemberType { get; set; }
        public DateTime RequestDateTime { get; private set; }
        public DateTime RequestForbidDateTime => RequestDateTime.AddHours(-12);

        private readonly List<JoinRequest> _joinRequests = new();
        public IReadOnlyCollection<JoinRequest> JoinRequests => _joinRequests.AsReadOnly();

        private readonly List<RequestAssignment> _assignments = new();
        public IReadOnlyCollection<RequestAssignment> Assignments => _assignments.AsReadOnly();

        private List<RequestFilter>? _requestFilters = [];
        public IReadOnlyCollection<RequestFilter>? RequestFilters => _requestFilters.AsReadOnly();

        public Request(Profile creator , string title , int requiredAssignment,DateTime requestDateTime,string? description, string[]? tags,RequestType requestType,MemberType memberType,bool isAutoAccept,HashSet<RequestFilter>? requestFilters) 
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
            MemberType = memberType;
            IsAutoAccept = isAutoAccept;
            _requestFilters = requestFilters?.ToList();
        }

        public void AddJoinRequest(Profile member,string? description)
        {
            if(this.IsRequestOpenToNewMember())
                throw new Exception("cannot Add an Member to a Request that is Not Open");
            if (_joinRequests.Any(jr => jr.Member.Id == member.Id))
                throw new Exception("this User already Sent a Join Request");
            var newJoinRequest = new JoinRequest(this, member, description);
            _joinRequests.Add(newJoinRequest);
        }

        public void AcceptMember(Profile owner , JoinRequest joinRequestToAccept)
        {

            if (owner.Id != Creator.Id)
                throw new Exception("only the Owner Can Accept the Request");

            joinRequestToAccept.Accept();

            var assignment = new RequestAssignment(this , joinRequestToAccept.Member);

            _assignments.Add(assignment);

            Status = RequestStatus.InProgress;

            CheckForCompletion();

        }
        public void RejectMember(Profile owner , JoinRequest joinRequestToReject)
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
        public bool IsRequestOpenToNewMember()
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
