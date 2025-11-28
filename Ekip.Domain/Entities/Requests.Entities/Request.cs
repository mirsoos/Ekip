using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Requests.Entities
{
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
        public DateTime RequestForbidDateTime { get; private set; }

        private readonly List<JoinRequest> _joinRequests = new();
        public IReadOnlyCollection<JoinRequest> JoinRequests => _joinRequests.AsReadOnly();

        private readonly List<RequestAssignment> _assignments = new();
        public IReadOnlyCollection<RequestAssignment> Assignments => _assignments.AsReadOnly();

        private List<RequestFilter>? _requestFilters = [];
        public IReadOnlyCollection<RequestFilter>? RequestFilters => _requestFilters.AsReadOnly();

        public Request(Profile creator, string title, int requiredMember, DateTime requestDateTime, string? description, string[]? tags, RequestType requestType, MemberType memberType, bool isAutoAccept, HashSet<RequestFilter>? requestFilters)
        {
            if (creator == null) throw new Exception("Request must have a Creator");
            if (string.IsNullOrWhiteSpace(title)) throw new Exception("Request must have a Title");
            if (requiredMember < 1) throw new Exception("Request cannot be created with 0 required members");

            Creator = creator;
            Title = title;
            Status = RequestStatus.Open;
            RequiredMembers = requiredMember;
            RequestDateTime = requestDateTime;
            RequestForbidDateTime = requestDateTime.AddHours(-12);

            Description = description;
            Tags = tags;
            RequestType = requestType;
            MemberType = memberType;
            IsAutoAccept = isAutoAccept;
            _requestFilters = requestFilters?.ToList();
        }

        public void AddJoinRequest(Profile member, string? description)
        {
            if (!this.IsRequestOpenToNewMember())
                throw new Exception("Cannot add a member. Request is closed or full.");

            if (_joinRequests.Any(jr => jr.Member.Id == member.Id))
                throw new Exception("This User already sent a Join Request");

            if (_assignments.Any(a => a.Member.Id == member.Id))
                throw new Exception("User is already a member");

            var newJoinRequest = new JoinRequest(this, member, description);
            _joinRequests.Add(newJoinRequest);
        }

        public void AcceptMember(Profile owner, JoinRequest joinRequestToAccept)
        {
            if (owner.Id != Creator.Id)
                throw new Exception("Only the Owner can Accept the Request");

            var capacity = MaximumRequiredMembers ?? RequiredMembers;
            if (_assignments.Count >= capacity)
                throw new Exception("Team is already full");

            joinRequestToAccept.Accept();

            var assignment = new RequestAssignment(this, joinRequestToAccept.Member);
            _assignments.Add(assignment);

            if (Status == RequestStatus.Open)
                Status = RequestStatus.InProgress;

            CheckForCompletion();
        }

        public void RejectMember(Profile owner, JoinRequest joinRequestToReject)
        {
            if (owner.Id != Creator.Id)
                throw new Exception("Only the Owner can Reject the Request");

            if (joinRequestToReject.Request.Id != Id)
                throw new Exception("The Join Request does not belong to this Request");

            joinRequestToReject.Decline();
        }

        public bool IsRequestOpenToNewMember()
        {
            var capacity = MaximumRequiredMembers ?? RequiredMembers;
            var hasSpace = _assignments.Count < capacity;

            var validDateTime = DateTime.UtcNow < RequestForbidDateTime;
            var validState = Status == RequestStatus.Open || Status == RequestStatus.InProgress;

            return hasSpace && validDateTime && validState;
        }

        public void ChangeValidity(bool newValue)
        {
            IsValid = newValue;
        }

        public void CheckForCompletion()
        {
            if (_assignments.Count >= RequiredMembers)
                Status = RequestStatus.Completed;
        }

        private Request() { }
    }
}