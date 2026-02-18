
using Ekip.Domain.Enums.Identity.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekip.Domain.Entities.ReadModels
{
    public class ProfileReadModel 
    {
        public Guid Id { get; set; }
        public Guid UserRef { get; set; }
        public string? AvatarUrl { get; set; }
        public double? Score { get; set; }
        public int Experience { get; set; }
        public VerificationLevel VerificationLevel { get; set; }

        [ForeignKey("UserRef")]
        public virtual UserReadModel User { get; set; }
        public virtual ICollection<RequestReadModel> Requests { get; set; } = new List<RequestReadModel>();
        public virtual ICollection<RequestAssignmentReadModel> RequestAssignments { get; set; } = new List<RequestAssignmentReadModel>();
    }
}
