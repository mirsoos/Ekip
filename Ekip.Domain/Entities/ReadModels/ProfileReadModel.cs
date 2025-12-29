
using System.ComponentModel.DataAnnotations.Schema;

namespace Ekip.Domain.Entities.ReadModels
{
    public class ProfileReadModel 
    {
        public long Id { get; set; }
        public long UserRef { get; set; }
        public string? AvatarUrl { get; set; }
        public double? Score { get; set; }
        public int Experience { get; set; }

        [ForeignKey("Userref")]
        public virtual UserReadModel User { get; set; }
    }
}
