using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Contracts.Events
{
    public record ProfileUpdatedEvent
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string UserName { get; init; }
        public string? Bio { get; init; }
        public int Age { get; init; }
        public string Email { get; init; }
    }
}
