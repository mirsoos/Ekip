using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;
using System.Text.Json;

namespace Ekip.Application.Features.Request.Consumer
{
    public class RequestCreatedConsumer : IConsumer<RequestCreatedEvent>
    {
        private readonly IRequestReadRepository _requestRead;
        public RequestCreatedConsumer(IRequestReadRepository requestRead)
        {
            _requestRead = requestRead;
        }
        public async Task Consume(ConsumeContext<RequestCreatedEvent> context)
        {
            var message = context.Message;

            var mongoToPostgres = new RequestReadModel
            {
                Id = message.RequestRef,
                CreatorRef = message.CreatorRef,
                Title = message.Title,
                CreateDate = message.RequestCreateDateTime,
                Description = message.Description,
                IsAutoAccept = message.IsAutoAccept,
                IsDeleted = false,
                IsRepeatable = message.IsRepeatable,
                MaximumRequiredAssignmnets = message.MaximumRequiredAssignmnets,
                MemberType = message.MemberType,
                RepeatType = message.RepeatType,
                RequestDateTime = message.RequestDateTime,
                RequestForbidDateTime = message.RequestForbidDateTime,
                RequestType = message.RequestType,
                RequiredMembers = message.RequiredMembers,
                Tags = message.Tags != null ? string.Join(",",message.Tags) : null,
                RequestFilters = message.RequestFilters != null ? JsonSerializer.Serialize(message.RequestFilters) : null ,
                Status = message.Status
            };

           await _requestRead.AddRequestAsync(mongoToPostgres, context.CancellationToken);
        }
    }
}
