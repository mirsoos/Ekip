using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class RequestRepeatDto
    {
        public bool IsRepeatable { get; set; }
        public RequestRepeatType Type { get; set; }
    }
}
