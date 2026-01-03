using Ekip.Domain.Enums.Requests.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekip.Domain.Entities.ReadModels
{
    public class RequestAssignmentReadModel
    {
        public Guid Id { get; set; }
        public Guid RequestRef { get; set; }
        public DateTime CreateDate { get; set; }
        public AssignmentStatus Status { get; set; }
        public Guid SenderRef { get; set; }
        public string? Description { get; set; }
        public DateTime ActionDate { get; set; }

        [ForeignKey("SenderRef")]
        public virtual ProfileReadModel SenderProfile { get; set; }
        [ForeignKey("RequestRef")]
        public virtual RequestReadModel Requests { get; set; }
    }
}
