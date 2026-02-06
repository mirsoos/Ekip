using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using MassTransit;

namespace Ekip.Application.Features.Request.Consumers
{
    public class RequestUpdatedConsumer : IConsumer<RequestUpdatedEvent>
    {
        private readonly IRequestReadRepository _requestRead;

        public RequestUpdatedConsumer(IRequestReadRepository requestRead)
        {
            _requestRead = requestRead;
        }
        public async Task Consume(ConsumeContext<RequestUpdatedEvent> context)
        {
            var message = context.Message;

            await _requestRead.UpdateAsync(message.RequestRef,message.Status,context.CancellationToken);
        }
    }
}
