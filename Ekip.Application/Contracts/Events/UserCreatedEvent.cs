
using Ekip.Domain.Enums.Identity.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record UserCreatedEvent
    {
        public Guid Id { get; init; }
        public Guid ProfileRef { get; set; }
        public DateTime CreateDate { get; init; }
        public bool IsDeleted { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public GenderType Gender { get; init; }
        public int Age { get; init; }
        public string PhoneNumber { get; init; }
        public Guid UserRef { get; set; }
        public int Experience { get; set; }
        public double? Score { get; set; }
        public string? Bio { get; set; }
    }
}
