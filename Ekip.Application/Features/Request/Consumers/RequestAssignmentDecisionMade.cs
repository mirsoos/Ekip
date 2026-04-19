using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Features.Request.Consumers
{
    public class RequestAssignmentDecisionMade : IConsumer<AssignmentDecisionMadeEvent>
    {
        private readonly IRequestReadRepository _requestRead;
        public RequestAssignmentDecisionMade(IRequestReadRepository requestRead)
        {
            _requestRead = requestRead;
        }
        public async Task Consume(ConsumeContext<AssignmentDecisionMadeEvent> context)
        {
            var message = context.Message;

            await _requestRead.UpdateAssignmentDecisionAsync(message.AssignmentRef , message.NewStatus,context.CancellationToken);
        }
    }
}
