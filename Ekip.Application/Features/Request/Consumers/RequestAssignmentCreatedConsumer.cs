using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Request.Consumers
{
    public class RequestAssignmentCreatedConsumer : IConsumer<AssignmentProcessedEvent>
    {
        private readonly IRequestReadRepository _requestRead;
        public RequestAssignmentCreatedConsumer(IRequestReadRepository requestRead)
        {
            _requestRead = requestRead;
        }
        public async Task Consume(ConsumeContext<AssignmentProcessedEvent> context)
        {
            var requestAssignment = context.Message;

            var mongoToPostgres = new RequestAssignmentReadModel
            {
                Id = requestAssignment.Id,
                RequestRef = requestAssignment.RequestRef,
                Status = requestAssignment.AssignmentStatus,
                SenderRef = requestAssignment.SenderRef,
                CreateDate = requestAssignment.CreateDate,
                Description = requestAssignment.Description,
                ActionDate = requestAssignment.ActionDate
            };

            await _requestRead.AddAssignmentAsync(mongoToPostgres, context.CancellationToken);

        }
    }
}
