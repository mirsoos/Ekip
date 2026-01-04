using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Contracts.Events
{
    record class ProfileAvatarUpdatedEvent
    {
        public Guid ProfileRef { get; set; }
        public string AvatarUrl { get; set; }
    }
}
