using Ekip.Application.DTOs.Request;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Features.Request.Qeuries
{
    public class GetRequestQuery : IRequest<RequestDetailsDto>
    {
        public Guid RequestRef { get; set; }
    }
}
