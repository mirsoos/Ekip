
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record RequestUpdatedEvent
    {
        public Guid RequestRef { get; set; }
        public RequestStatus Status { get; set; }
    }
}
