
namespace Ekip.Application.DTOs.Request
{
    public class JoinRequestDto
    {
        public Guid SenderProfileRef { get; set; }
        public Guid RequestRef { get; set; }
        public string? Description { get; set; }
    }
}
