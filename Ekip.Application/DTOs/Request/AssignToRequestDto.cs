using Ekip.Domain.Enums.Requests.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.DTOs.Request
{
    public class AssignToRequestDto
    {
        public long RequestRef { get; set; }
        public long SenderRef { get; set; }
        public string Description { get; set; }
        public AssignmentStatus Status { get; set; }
    }
}
