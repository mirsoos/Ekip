
using Ekip.Domain.Enums;

namespace Ekip.Domain.Entities
{
    public class Request
    {
        public long Id { get; private set; }
        public string Title { get; private set; }
        public Profile Creator { get; private set; }
        public RequestStatus Status { get; private set; }
        public int RequiredAssignments { get; private set; }

        private readonly List<JoinRequest> _joinRequests = new();
        public IReadOnlyCollection<JoinRequest> JoinRequests => _joinRequests.AsReadOnly();

        private readonly List<RequestAssignment> _assignments = new();
        public IReadOnlyCollection<RequestAssignment> Assignments => _assignments.AsReadOnly();

        public Request(Profile creator , string title , int requiredAssignment) 
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
        }

        public void AddJoinRequest(Profile applicant)
        {
            if(Status != RequestStatus.Open)
                throw new Exception("cannot Add an Applicant to a Request that is Not Open");
            if (_joinRequests.Any(jr => jr.Applicant.Id == applicant.Id))
                throw new Exception("this User already Sent a Join Request");
            var newJoinRequest = new JoinRequest(this, applicant);
            _joinRequests.Add(newJoinRequest);
        }

        public void AcceptApplicant(Profile owner , JoinRequest joinRequestToAccept)
        {

            if (owner.Id != this.Creator.Id)
                throw new Exception("only the Owner Can Accept the Request");

            joinRequestToAccept.Accept();

            var assignment = new RequestAssignment(this , joinRequestToAccept.Applicant);

            _assignments.Add(assignment);

            Status = RequestStatus.InProgress;

            CheckForCompletion();

        }
        public void RejectApplicant(Profile owner , JoinRequest joinRequestToReject)
        {
            if (owner.Id != this.Creator.Id)
                throw new Exception("only the Owner Can Reject the Request");
            if (joinRequestToReject.Request.Id != this.Id)
                throw new Exception("the Join Request dos not Belong to this Request");
            joinRequestToReject.Decline();
        }



        private void CheckForCompletion()
        {
            if(_assignments.Count >= RequiredAssignments)
                Status = RequestStatus.Completed;
        }

        private Request() { }
    }
}
