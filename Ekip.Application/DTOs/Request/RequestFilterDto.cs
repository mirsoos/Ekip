
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class RequestFilterDto
    {
        public string Value { get; set; }
        public RequestFilterType Type { get; set; }
        public RequestFilterKind Kind { get; set; }
    }
}
