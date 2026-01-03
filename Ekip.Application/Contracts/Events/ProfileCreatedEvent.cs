using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Contracts.Events
{
    public record ProfileCreatedEvent
    {
        public Guid Id { get; init; }
        public Guid UserRef { get; init; }
        public string AvatarUrl { get; init; }
        public double? Score { get; init; }
        public int Experience { get; init; }
    }
}
