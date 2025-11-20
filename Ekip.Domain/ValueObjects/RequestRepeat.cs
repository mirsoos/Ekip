
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.ValueObjects
{
    public class RequestRepeat
    {
        public bool IsRepeatable { get; set; }
        public RequestRepeatType Type { get; set; }
    }
}
