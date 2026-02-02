
namespace Ekip.Application.DTOs.User
{
    public class CreatedProfileDto
    {
        public Guid ProfileRef { get; set; }
        public Guid UserRef { get; set; }
        public double? Score { get; set; }
        public int Experience { get; set; }
    }
}
