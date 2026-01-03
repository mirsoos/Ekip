using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Password { get; init; }
        public bool Gender { get; init; }
        public int Age { get; init; }
        public string PhoneNumber { get; init; }
    }
}
