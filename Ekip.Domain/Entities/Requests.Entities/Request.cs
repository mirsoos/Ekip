using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Requests.Entities
{
    public class Request : BaseEntity
    {
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public Guid Creator { get; private set; }
        public RequestStatus Status { get; private set; }
        public int RequiredMembers { get; private set; }
        public int? MaximumRequiredMembers { get; private set; }
        public int? MaximumRequiredAge { get; private set; }
        public int? MinimumRequiredAge { get; private set; }
        public string[]? Tags { get; private set; }
        public bool IsAutoAccept { get; private set; }
        public bool IsRepeatable { get; private set; }
        public RequestRepeatType RepeatType { get; private set; }
        public RequestType RequestType { get; private set; }
        public MemberType MemberType { get; private set; }
        public DateTime RequestDateTime { get; private set; }
        public DateTime RequestForbidDateTime { get; private set; }

        private List<RequestAssignment> _assignments;
        public IReadOnlyCollection<RequestAssignment> Assignments => _assignments.AsReadOnly();

        private List<RequestFilter> _requestFilters;
        public IReadOnlyCollection<RequestFilter>? RequestFilters => _requestFilters.AsReadOnly();

        public Request(Guid creator, string title, int requiredMember, DateTime requestDateTime, string? description, string[]? tags, RequestType requestType, MemberType memberType, bool isAutoAccept, HashSet<RequestFilter>? requestFilters)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Request must have a Title");
            if (requiredMember < 1)
                throw new Exception("Request cannot be created with 0 required members");
            _assignments = new List<RequestAssignment>();
            _requestFilters = new List<RequestFilter>();
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
            _requestFilters = requestFilters?.ToList() ?? new List<RequestFilter>();

            _assignments.Add(new RequestAssignment(creator, "Creator" , AssignmentStatus.Accepted));
        }

        public RequestAssignment AddJoinRequest(Guid member,MemberEligibility memberEligibility , string description)
        {
            if (!this.IsRequestOpenToNewMember())
                throw new Exception("Cannot add a member. Request is closed or full.");

            if (!this.AssignmentRequirements(memberEligibility))
                throw new Exception("you dont meet Requirements to Join.");

            if (_assignments.Any(jr => jr.SenderRef == member))
                throw new Exception("This User already sent a Join Request");

            var assignmentStatus = IsAutoAccept ? AssignmentStatus.Accepted : AssignmentStatus.Pending;

            var newAssignment = new RequestAssignment(member, description, assignmentStatus);
            _assignments.Add(newAssignment);

            if (IsAutoAccept && Status == RequestStatus.Open)
            {
                Status = RequestStatus.InProgress;
            }

            CheckForCompletion();

            return newAssignment;
        }

        public void AcceptMember(Guid owner, RequestAssignment assignmentToAccept)
        {
            if (owner != Creator)
                throw new Exception("Only the Owner can Accept the Request");

            var acceptedCount = _assignments.Count(a => a.Status == AssignmentStatus.Accepted);
            var capacity = MaximumRequiredMembers ?? RequiredMembers;

            if (acceptedCount >= capacity)
                throw new Exception("Ekip is already full");

            assignmentToAccept.Accept();

            if (Status == RequestStatus.Open)
                Status = RequestStatus.InProgress;

            CheckForCompletion();
        }

        public void RejectMember(Guid owner, RequestAssignment assignmentToReject)
        {
            if (owner != Creator)
                throw new Exception("Only the Owner can Reject the Request");

            assignmentToReject.Decline();
        }

        public bool IsRequestOpenToNewMember()
        {
            var acceptedCount = _assignments.Count(a => a.Status == AssignmentStatus.Accepted);
            var capacity = MaximumRequiredMembers ?? RequiredMembers;
            var hasSpace = acceptedCount < capacity;

            var validDateTime = DateTime.UtcNow < RequestForbidDateTime;
            var validState = Status == RequestStatus.Open || Status == RequestStatus.InProgress;

            return hasSpace && validDateTime && validState;
        }

        public void CheckForCompletion()
        {
            var acceptedCount = _assignments.Count(a => a.Status == AssignmentStatus.Accepted);

            if (acceptedCount >= RequiredMembers)
                Status = RequestStatus.Completed;
        }

        public bool AssignmentRequirements(MemberEligibility member)
        {
            if (_requestFilters == null || !_requestFilters.Any())
                return true;

            return _requestFilters.All(filter => filter.IsSatisfiedBy(member));
        }

        private Request() {
            _assignments = new List<RequestAssignment>();
            _requestFilters = new List<RequestFilter>();

        }
    }
}