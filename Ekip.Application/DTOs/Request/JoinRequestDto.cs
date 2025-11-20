
namespace Ekip.Application.DTOs.Request
{
    public class JoinRequestDto
    {
        public long SenderProfileRef { get; set; }
        public long RequestRef { get; set; }
        public string? Description { get; set; }
    }
}
